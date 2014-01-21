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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.ServiceLock
{

    /// <summary>
    /// Processor for the Services in the <see cref="Model.ServiceLock"/> table.
    /// </summary>
    public class ServiceLockProcessor
    {
        #region Members
        private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private readonly Dictionary<ServiceLockTypeEnum, IServiceLockProcessorFactory> _extensions = new Dictionary<ServiceLockTypeEnum, IServiceLockProcessorFactory>();
        private readonly ServiceLockThreadPool _threadPool;
        private readonly ManualResetEvent _threadStop;
        private readonly ManualResetEvent _terminationEvent;
        private bool _stop = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for ServiceLock processor.
        /// </summary>
        /// <param name="numberThreads">The number of threads allocated to the processor.</param>
        /// <param name="terminationEvent">Stop signal</param>
        public ServiceLockProcessor(int numberThreads, ManualResetEvent terminationEvent)
        {
            _terminationEvent = terminationEvent;
            _threadStop = new ManualResetEvent(false);

			_threadPool = new ServiceLockThreadPool(numberThreads);
        	_threadPool.ThreadPoolName = "ServiceLock Pool";

            ServiceLockFactoryExtensionPoint ep = new ServiceLockFactoryExtensionPoint();
            object[] factories = ep.CreateExtensions();

            if (factories == null || factories.Length == 0)
            {
                // No extension for the ServiceLock processor. 
                Platform.Log(LogLevel.Warn, "No ServiceLockFactory Extension found.");
            }
            else
            {
                foreach (object obj in factories)
                {
                    IServiceLockProcessorFactory factory = obj as IServiceLockProcessorFactory;
                    if (factory != null)
                    {
                        ServiceLockTypeEnum type = factory.GetServiceLockType();
                        _extensions.Add(type, factory);
                    }
                    else
                        Platform.Log(LogLevel.Error, "Unexpected incorrect type loaded for extension: {0}",
                                     obj.GetType());
                }
            }
        }
        #endregion

        #region Public Methods
		/// <summary>
		/// The processing thread.
		/// </summary>
		/// <remarks>
		/// This method queries the database for ServiceLock entries to work on, and then uses
		/// a thread pool to process the entries.
		/// </remarks>
		public void Run()
		{
			// Start the thread pool
			if (!_threadPool.Active)
				_threadPool.Start();

			// Reset any queue items related to this service that are have the Lock bit set.
			try
			{
				ResetLocked();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Fatal, e,
							 "Unable to reset ServiceLock items on startup.  There may be ServiceLock items orphaned in the queue.");
			}

            Platform.Log(LogLevel.Info, "ServiceLock Processor is running");
            while (true)
            {
                
                try
                {
                    if (_threadPool.CanQueueItem)
				    {
					    Model.ServiceLock queueListItem;

					    using (IUpdateContext updateContext = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
					    {
						    var select = updateContext.GetBroker<IQueryServiceLock>();
						    var parms = new ServiceLockQueryParameters
						        {
						            ProcessorId = ServerPlatform.ProcessorId
						        };

					        queueListItem = select.FindOne(parms);
						    updateContext.Commit();
					    }

					    if (queueListItem == null)
					    {
                            WaitHandle.WaitAny(new WaitHandle[] { _terminationEvent, _threadStop }, TimeSpan.FromSeconds(10), false);
						    _threadStop.Reset();
					    }
					    else
					    {
						    if (!_extensions.ContainsKey(queueListItem.ServiceLockTypeEnum))
						    {
							    Platform.Log(LogLevel.Error,
										     "No extensions loaded for ServiceLockTypeEnum item type: {0}.  Failing item.",
										     queueListItem.ServiceLockTypeEnum);

							    //Just fail the ServiceLock item, not much else we can do
							    ResetServiceLock(queueListItem);
							    continue;
						    }

						    IServiceLockProcessorFactory factory = _extensions[queueListItem.ServiceLockTypeEnum];

						    IServiceLockItemProcessor processor;
						    try
						    {
							    processor = factory.GetItemProcessor();
						    }
						    catch (Exception e)
						    {
							    Platform.Log(LogLevel.Error, e, "Unexpected exception creating ServiceLock processor.");
							    ResetServiceLock(queueListItem);
							    continue;
						    }

						    _threadPool.Enqueue(processor, queueListItem, delegate(IServiceLockItemProcessor queueProcessor, Model.ServiceLock queueItem)
												    {
													    try
													    {
															queueProcessor.Process(queueItem);
													    }
													    catch (Exception e)
													    {
														    Platform.Log(LogLevel.Error, e,
																	     "Unexpected exception when processing ServiceLock item of type {0}.  Failing Queue item. (GUID: {1})",
																		 queueItem.ServiceLockTypeEnum,
																		 queueItem.GetKey());

													    	ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Error, "ServiceLockProcessor",
													    	                     AlertTypeCodes.UnableToProcess,
                                                                                 null, TimeSpan.Zero,
													    	                     "Exception thrown when processing {0} ServiceLock item : {1}",
													    	                     queueItem.ServiceLockTypeEnum.Description, e.Message);
															ResetServiceLock(queueItem);
													    }

													    // Cleanup the processor
														queueProcessor.Dispose();

														// Signal the thread to come out of sleep mode
												    	_threadStop.Set();
												    });
					    }
				    }
				    else
				    {
					    // Wait for only 5 seconds when the thread pool is all in use.
                        WaitHandle.WaitAny(new WaitHandle[] { _terminationEvent, _threadStop }, TimeSpan.FromSeconds(5), false);
                        _threadStop.Reset();
				    }
				    
                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Exception has occurred : {0}. Retry later.", ex.Message);
					WaitHandle.WaitAny(new WaitHandle[] { _terminationEvent, _threadStop }, TimeSpan.FromSeconds(5), false);
                    _threadStop.Reset();
                }

                if (_stop)
                    return;
            }
		}

        /// <summary>
        /// Stop the ServiceLock processor
        /// </summary>
		public void Stop()
        {
            _terminationEvent.Set();
        	_stop = true;
        	_threadPool.Stop(true);
        }

    	#endregion

        #region Private Methods
        /// <summary>
        /// Reset queue items that were unadvertly left locked, more than likely due to a crash. 
        /// </summary>
        private void ResetLocked()
        {
            using (IUpdateContext updateContext = _store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                var reset = updateContext.GetBroker<IResetServiceLock>();
                var parms = new ServiceLockResetParameters
                    {
                        ProcessorId = ServerPlatform.ProcessorId
                    };

                IList<Model.ServiceLock> modifiedList = reset.Find(parms);

                if (modifiedList != null)
                {
                    // output the list of items that have been reset
                    foreach (Model.ServiceLock queueItem in modifiedList)
                    {
                        Platform.Log(LogLevel.Info, "Cleanup: Reset ServiceLock Item : {0} --> Type={1} Scheduled={2}",
                                     queueItem.GetKey().Key,
                                     queueItem.ServiceLockTypeEnum,
                                     queueItem.ScheduledTime);
                    }
                }

                updateContext.Commit();
            }
        }

        /// <summary>
        /// Reset the Lock for a specific <see cref="Model.ServiceLock"/> row.
        /// </summary>
        /// <param name="item">The row to reset the lock for.</param>
        private void ResetServiceLock(Model.ServiceLock item)
        {
            // keep trying just in case of db error
            while(true)
            {
                try
                {
                    using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        // Update the ServiceLock item status and times.
                        var update = updateContext.GetBroker<IUpdateServiceLock>();

                        var parms = new ServiceLockUpdateParameters();
                        parms.ServiceLockKey = item.GetKey();
                        parms.Lock = false;
                        parms.ScheduledTime = Platform.Time.AddMinutes(10);
                        parms.ProcessorId = item.ProcessorId;

                        if (false == update.Execute(parms))
                        {
                            Platform.Log(LogLevel.Error, "Unable to update ServiceLock GUID Status: {0}",
                                         item.GetKey().ToString());
                        }

                        updateContext.Commit();
                    }

                    break;
                }
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Exception has occured when trying to reset the entry. Retry later");
                    _terminationEvent.WaitOne(2000, false);
                }
            }
            
        }

        #endregion

    }
}