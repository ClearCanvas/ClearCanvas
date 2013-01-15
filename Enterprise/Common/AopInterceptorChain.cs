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
using System.Reflection;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Represents a chain of interceptors as a single interceptor.
	/// </summary>
	/// <remarks>
	/// The purpose of this class is to give our application more control over the interception process than
	/// we would have if we were to use DPs built-in interceptor chaining functionality.  The problem with
	/// the DP implementation is that each call to <see cref="IInvocation.Proceed"/> causes advancement down
	/// the chain to the next interceptor, hence it cannot distinguish situations where Proceed is being called
	/// repeatedly from the *same* interceptor (e.g. due to a retry after an exception is thrown).
	/// </remarks>
	public class AopInterceptorChain : IInterceptor
	{
		#region IntermediateInvocation

		/// <summary>
		/// Helper class for executing the intercept chain.
		/// </summary>
		class IntermediateInvocation : IInvocation
		{
			private readonly AopInterceptorChain _owner;
			private readonly IInvocation _rootInvocation;
			private int _interceptLevel = -1;

			internal IntermediateInvocation(AopInterceptorChain owner, IInvocation rootInvocation)
			{
				_owner = owner;
				_rootInvocation = rootInvocation;
			}

			#region IInvocation Members

			public object InvocationTarget
			{
				get { return _rootInvocation.InvocationTarget; }
			}

			public Type TargetType
			{
				get { return _rootInvocation.TargetType; }
			}

			public object[] Arguments
			{
				get { return _rootInvocation.Arguments; }
			}

			public Type[] GenericArguments
			{
				get { return _rootInvocation.GenericArguments; }
			}

			public MethodInfo Method
			{
				get { return _rootInvocation.Method; }
			}

			public MethodInfo MethodInvocationTarget
			{
				get { return _rootInvocation.MethodInvocationTarget; }
			}

			public object ReturnValue
			{
				get { return _rootInvocation.ReturnValue; }
				set { _rootInvocation.ReturnValue = value; }
			}

			public void Proceed()
			{
				// we can't call _rootInvocation.Proceed() here, because Proceed() can only be invoked once per interceptor
				// (use .NET Reflector to look at the Castle code to understand why this is so)
				// and that doesn't satisfy the requirements of some of our interceptors that want to retry failed calls
				// instead, we take control of the interception process
				try
				{
					_interceptLevel++;

					if (_interceptLevel < _owner.Interceptors.Count)
					{
						// pass to next interceptor
						_owner.Interceptors[_interceptLevel].Intercept(this);
					}
					else
					{
						// no more interceptors, time for the real deal
						// can't call _rootInvocation.Proceed() here, because that would go through the entire interception chain again
						// instead, we invoke a delegate mapped (via reflection) to a protected method, that has no such restriction
						// but this protected method is only available on AbstractInvocation subclasses
						var invocation = _rootInvocation as AbstractInvocation;
						if(invocation == null)
							throw new InvalidOperationException("AopInterceptorChain can only be used with IInvocation implementations derived from Castle.DynamicProxy.AbstractInvocation.");
						
						_surrogateForProceed(invocation);
					}
				}
				finally
				{
					// important to decrement this variable on the way out, so that a given
					// interceptor may call Proceed() multiple times without advancing along the chain
					_interceptLevel--;
				}
			}

			public void SetArgumentValue(int index, object value)
			{
				_rootInvocation.SetArgumentValue(index, value);
			}

			public object GetArgumentValue(int index)
			{
				return _rootInvocation.GetArgumentValue(index);
			}

			public MethodInfo GetConcreteMethod()
			{
				return _rootInvocation.GetConcreteMethod();
			}

			public MethodInfo GetConcreteMethodInvocationTarget()
			{
				return _rootInvocation.GetConcreteMethodInvocationTarget();
			}

			public object Proxy
			{
				get { return _rootInvocation.Proxy; }
			}

			#endregion
		}

		#endregion

		#region Static members

		private static readonly Action<AbstractInvocation> _surrogateForProceed;

		/// <summary>
		/// Class constructor.
		/// </summary>
		static AopInterceptorChain()
		{
			// the AbstractInvocation class has a protected method InvokeMethodOnTarget()
			var method = typeof(AbstractInvocation).GetMethod("InvokeMethodOnTarget", BindingFlags.Instance | BindingFlags.NonPublic);
			if (method == null)
				throw new MissingMethodException("Error finding method named 'InvokeMethodOnTarget' on Castle.DynamicProxy.AbstractInvocation.");

			// create a delegate bound to this method
			_surrogateForProceed = (Action<AbstractInvocation>)Delegate.CreateDelegate(typeof(Action<AbstractInvocation>), null, method);
		}

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="interceptors"></param>
		public AopInterceptorChain(IList<IInterceptor> interceptors)
		{
			this.Interceptors = interceptors ?? new List<IInterceptor>();
		}


		#region IInterceptor Members

		/// <summary>
		/// Intercepts the specified invocation.
		/// </summary>
		/// <param name="invocation"></param>
		public void Intercept(IInvocation invocation)
		{
			new IntermediateInvocation(this, invocation).Proceed();
		}

		#endregion

		/// <summary>
		/// Chain of interceptors.
		/// </summary>
		private IList<IInterceptor> Interceptors { get; set; }

	}
}
