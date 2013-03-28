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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
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
		private static bool _reset = false;
        private readonly TimeSpan _retryDelay = TimeSpan.FromMinutes(2);
		#endregion

		#region Constructors
		/// <summary>
		/// **** For internal use only***
		/// </summary>
		private WorkQueueServerManager(string name) : base(name)
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
			get
			{
				if (_instance == null)
				{
					if (!_reset)
					{
						_reset = true;
						// Reset any queue items related to this system that are in a "In Progress" state.
						try
						{
							ResetFailedItems();
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Fatal, e,
										 "Unable to reset WorkQueue items on startup.  There may be WorkQueue items orphaned in the queue.");
						}
					}
					_instance =
						new WorkQueueServerManager("WorkQueue");
				}

				return _instance;
			}
			set
			{
				_instance = value;
			}
		}
		#endregion

        
		#region Protected Methods
		/// <summary>
		/// Reset queue items that were unadvertly left in "in progress" state by previous run. 
		/// </summary>
		public static void ResetFailedItems()
		{
		    WorkQueueStatusEnum pending = WorkQueueStatusEnum.Pending;
			WorkQueueStatusEnum failed = WorkQueueStatusEnum.Failed;

			using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				IWorkQueueReset reset = ctx.GetBroker<IWorkQueueReset>();
				WorkQueueResetParameters parms = new WorkQueueResetParameters();
				parms.ProcessorID = ServerPlatform.ProcessorId;

				// reschedule to start again now
				parms.RescheduleTime = Platform.Time;
				// retry will expire X minutes from now (so other process MAY NOT remove them)
				parms.RetryExpirationTime = Platform.Time.AddMinutes(2);

				// if an entry has been retried more than WorkQueueMaxFailureCount, it should be failed
				parms.MaxFailureCount = 3;
				// failed item expires now (so other process can remove them if desired)
				parms.FailedExpirationTime = Platform.Time;

				IList<Model.WorkQueue> modifiedList = reset.Find(parms);

				if (modifiedList != null)
				{
					// output the list of items that have been reset
					foreach (Model.WorkQueue queueItem in modifiedList)
					{
						if (queueItem.WorkQueueStatusEnum.Equals(pending))
							Platform.Log(LogLevel.Info, "Cleanup: Reset Queue Item : {0} --> Status={1} Scheduled={2} ExpirationTime={3}",
											queueItem.GetKey().Key,
											queueItem.WorkQueueStatusEnum,
											queueItem.ScheduledTime,
											queueItem.ExpirationTime);
					}

					// output the list of items that have been failed because it exceeds the max retry count
					foreach (Model.WorkQueue queueItem in modifiedList)
					{
						if (queueItem.WorkQueueStatusEnum.Equals(failed))
							Platform.Log(LogLevel.Info, "Cleanup: Fail Queue Item  : {0} : FailureCount={1} ExpirationTime={2}",
											queueItem.GetKey().Key,
											queueItem.FailureCount,
											queueItem.ExpirationTime);
					}
				}

				ctx.Commit();
			}
		}

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
                IWorkQueueManagerExtensionPoint[] extensions = CollectionUtils.Cast<IWorkQueueManagerExtensionPoint>(xp.CreateExtensions()).ToArray();
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
			if (_theProcessor != null)
			{
				_theProcessor.Stop();
				_theProcessor = null;
			}
		}

		#endregion

	}
    
}