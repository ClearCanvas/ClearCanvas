#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Interceptor;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Common.Audit;
using ClearCanvas.Enterprise.Common;


namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Advice class responsible for honouring <see cref="AuditRecorderAttribute"/>s applied to service operation methods.
	/// </summary>
	public class AuditAdvice
	{
		#region RecorderContext class

		/// <summary>
		/// Implementation of <see cref="IServiceOperationRecorderContext"/>
		/// </summary>
		class RecorderContext : IServiceOperationRecorderContext
		{
			private readonly IInvocation _invocation;
			private readonly IServiceOperationRecorder _recorder;
			private readonly AuditLog _auditLog;
			private readonly string _operationName;
			private EntityChangeSet _changeSet;

			internal RecorderContext(IInvocation invocation, IServiceOperationRecorder recorder)
			{
				_invocation = invocation;
				_recorder = recorder;
				_auditLog = new AuditLog(ProductInformation.Component, _recorder.Category);
				_operationName = string.Format("{0}.{1}", _invocation.InvocationTarget.GetType().FullName, _invocation.Method.Name);
			}

			string IServiceOperationRecorderContext.OperationName
			{
				get { return _operationName; }
			}

			Type IServiceOperationRecorderContext.ServiceClass
			{
				get { return _invocation.InvocationTarget.GetType(); }
			}

			MethodInfo IServiceOperationRecorderContext.OperationMethodInfo
			{
				get { return _invocation.MethodInvocationTarget; }
			}

			object IServiceOperationRecorderContext.Request
			{
				get { return _invocation.Arguments.FirstOrDefault(); }
			}

			object IServiceOperationRecorderContext.Response
			{
				get { return _invocation.ReturnValue; }
			}

			void IServiceOperationRecorderContext.Write(string operation, string message)
			{
				_auditLog.WriteEntry(operation ?? _operationName, message);
			}

			void IServiceOperationRecorderContext.Write(string message)
			{
				_auditLog.WriteEntry(_operationName, message);
			}

			EntityChangeSet IServiceOperationRecorderContext.ChangeSet
			{
				get { return _changeSet; }
			}

			internal void SetChangeSet(EntityChangeSet changeSet)
			{
					_changeSet = changeSet;
			}

			internal void PreCommit(IPersistenceContext persistenceContext)
			{
				try
				{
					_recorder.PreCommit(this, persistenceContext);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			internal void PostCommit()
			{
				try
				{
					_recorder.PostCommit(this);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}
		}

		#endregion

		#region InvocationInfo class 

		class InvocationInfo
		{
			private readonly List<RecorderContext> _recorderContexts;

			public InvocationInfo(List<RecorderContext> recorderContexts)
			{
				_recorderContexts = recorderContexts;
			}

			internal void SetChangeSet(EntityChangeSet changeSet)
			{
				foreach (var recorderContext in _recorderContexts)
				{
					recorderContext.SetChangeSet(changeSet);
				}
			}
			internal void PreCommit(IPersistenceContext persistenceContext)
			{
				foreach (var recorderContext in _recorderContexts)
				{
					recorderContext.PreCommit(persistenceContext);
				}
			}

			internal void PostCommit()
			{
				foreach (var recorderContext in _recorderContexts)
				{
					recorderContext.PostCommit();
				}
			}
		}

		#endregion

		#region ChangeSetListener class

		/// <summary>
		/// Change set listener responsible for grabbing the current change set and passing it over to the invocation context.
		/// </summary>
		[ExtensionOf(typeof(EntityChangeSetListenerExtensionPoint))]
		public class ChangeSetListener: IEntityChangeSetListener
		{
			public void PreCommit(EntityChangeSetPreCommitArgs args)
			{
				if (_invocationInfo == null || _invocationInfo.Count == 0)
					return;

				var info = _invocationInfo.Peek();

				// store a copy of the change set for use by recorders,
				// and then invoke their PreCommit callback
				info.SetChangeSet(args.ChangeSet);
				info.PreCommit(args.PersistenceContext);
			}

			public void PostCommit(EntityChangeSetPostCommitArgs args)
			{
			}
		}

		#endregion

		/// <summary>
		/// Outer (post-commit) interceptor.
		/// </summary>
		public class Outer : IInterceptor
		{
			public void Intercept(IInvocation invocation)
			{
				// ensure the thread-static variable is initialized for the current thread
				if (_invocationInfo == null)
					_invocationInfo = new Stack<InvocationInfo>();

				try
				{

					var recorderContexts = AttributeUtils.GetAttributes<AuditRecorderAttribute>(invocation.MethodInvocationTarget, true)
											.Select(a => new RecorderContext(invocation, (IServiceOperationRecorder)Activator.CreateInstance(a.RecorderClass)))
											.ToList();

					_invocationInfo.Push(new InvocationInfo(recorderContexts));

					invocation.Proceed();

					_invocationInfo.Peek().PostCommit();
				}
				finally
				{
					// clear current invocation
					_invocationInfo.Pop();
				}
			}
		}

		/// <summary>
		/// Inner (pre-commit) interceptor.
		/// </summary>
		public class Inner : IInterceptor
		{
			public void Intercept(IInvocation invocation)
			{
				invocation.Proceed();

				var pctx = PersistenceScope.CurrentContext;
				if (pctx is IReadContext && _invocationInfo.Count > 0)
				{
					// if this is a read operation, we call PreCommit here
					// otherwise it gets called by the ChangeSetListener class 
					_invocationInfo.Peek().PreCommit(PersistenceScope.CurrentContext);
				}
			}
		}

		/// <summary>
		/// Keep track of the invocations on the current thread.
		/// </summary>
		[ThreadStatic]
		private static Stack<InvocationInfo> _invocationInfo;
	}
}
