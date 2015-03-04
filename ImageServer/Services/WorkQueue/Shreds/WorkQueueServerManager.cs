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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Services.WorkQueue.Shreds
{
	/// <summary>
	/// Shreds namespace manager of processing threads for the WorkQueue.
	/// </summary>
	/// <remarks>
	/// The service manager is currently setup to create two WorkQueue processors,
	/// a primary processor and secondary processor.  Through configuration, the specific types
	/// of WorkQueue entries supported by each processor can be set.  By default, the primary
	/// processor supports high priority queue types for processing studies, editing studies, and 
	/// doing moves/auto-routes.  The secondary processor will process any queue entries.
	/// </remarks>
	public sealed class WorkQueueServerManager : ThreadedService
	{
		#region Private Members

		private static WorkQueueServerManager _instance;
		private WorkQueueProcessor _theProcessor;
		private readonly int _threadCount;
		private readonly TimeSpan _retryDelay = TimeSpan.FromMinutes(2);

		#endregion

		#region Constructors

		/// <summary>
		/// **** For internal use only***
		/// </summary>
		private WorkQueueServerManager(string name)
			: base(name)
		{
			_threadCount = WorkQueueSettings.Instance.WorkQueueThreadCount;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Singleton instance of the class.
		/// </summary>
		public static WorkQueueServerManager PrimaryInstance
		{
			get { return _instance ?? (_instance = new WorkQueueServerManager("WorkQueue")); }
			set { _instance = value; }
		}

		#endregion

		#region Protected Methods

		protected override bool Initialize()
		{
			if (_theProcessor == null)
			{
				// Force a read context to be opened.  When developing the retry mechanism 
				// for startup when the DB was down, there were problems when the type
				// initializer for enumerated values were failng first.  For some reason,
				// when the database went back online, they would still give exceptions.
				// changed to force the processor to open a dummy DB connect and cause an 
				// exception here, instead of getting to the enumerated value initializer.
				using (IReadContext readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
				{
					readContext.Dispose();
				}

				var xp = new WorkQueueManagerExtensionPoint();
				IWorkQueueManagerExtensionPoint[] extensions =
					CollectionUtils.Cast<IWorkQueueManagerExtensionPoint>(xp.CreateExtensions()).ToArray();
				foreach (IWorkQueueManagerExtensionPoint extension in extensions)
				{
					try
					{
						extension.OnInitializing(this);
					}
					catch (Exception)
					{
						ThreadRetryDelay = (int) _retryDelay.TotalMilliseconds;
						return false;
					}
				}

				_theProcessor = new WorkQueueProcessor(_threadCount, ThreadStop, Name);
			}

			return true;
		}

		protected override void Run()
		{
			_theProcessor.Run();
		}

		protected override void Stop()
		{
			//TODO CR (Jan 2014): Move this into the base if it applies to all subclasses?
			PersistentStoreRegistry.GetDefaultStore().ShutdownRequested = true;

			if (_theProcessor != null)
			{
				_theProcessor.Stop();
				_theProcessor = null;
			}
		}

		#endregion

	}
}