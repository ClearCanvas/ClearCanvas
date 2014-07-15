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
using System.ComponentModel;
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Core.ModelExtensions;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemFileImporter
{

    public class FilesystemFileImporterProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor, ICancelable
    {
        #region Private Fields
        private readonly Queue<DirectoryImporterBackgroundProcess> _queue = new Queue<DirectoryImporterBackgroundProcess>();
        private readonly List<DirectoryImporterBackgroundProcess> _inprogress = new List<DirectoryImporterBackgroundProcess>();
        private readonly ManualResetEvent _allCompleted = new ManualResetEvent(false);
        private readonly object _sync = new object();
        private int _importedSopCounter;
        private bool _restoreTriggered;

        private static readonly object _dbCheckSync = new object();
        private static long _lastDbCheck;
        private static bool? _multipleInstanceDetected= false;
        #endregion

        #region IServiceLockItemProcessor Members

        protected override void OnProcess(Model.ServiceLock item)
        {
            DirectoryImportSettings settings = DirectoryImportSettings.Default;

            // #10419: Avoid running multiple instances of Import service - which can lead to race condition on the input files
            if (CheckMultipleInstances())
            {
                // Instead of disabling duplicate entries automatically, decided it's easier and safer to let user figure out which one should be disabled.
                Platform.Log(LogLevel.Warn, "Detect multiple active instances of the Import File Service. Go to Service Scheduling configuration and make sure only one of them is active.");
                DateTime newScheduledTime = Platform.Time.AddSeconds(Math.Max(settings.RecheckDelaySeconds, 60));
                UnlockServiceLock(item, true, newScheduledTime);
                return;
            }

            
            ServerFilesystemInfo filesystem = EnsureFilesystemIsValid(item);
            if (filesystem != null)
            {
                Platform.Log(LogLevel.Debug, "Start importing dicom files from {0}", filesystem.Filesystem.FilesystemPath);

                foreach (ServerPartition partition in ServerPartitionMonitor.Instance.Partitions)
                {
                    if (partition.Enabled)
                    {
                        String incomingFolder = partition.GetIncomingFolder();
                        if (!string.IsNullOrEmpty(incomingFolder))
                        {
                            DirectoryImporterParameters parms = new DirectoryImporterParameters
                                                                    {
                                                                        Directory = new DirectoryInfo(incomingFolder),
                                                                        PartitionAE = partition.AeTitle,
                                                                        MaxImages = settings.MaxBatchSize,
                                                                        Delay = settings.ImageDelay,
                                                                        Filter = "*.*"
                                                                    };

                            if (!parms.Directory.Exists)
                            {
                                parms.Directory.Create();
                            }

                            DirectoryImporterBackgroundProcess process = new DirectoryImporterBackgroundProcess(parms);
                            process.SopImported += delegate { _importedSopCounter++; };
                            process.RestoreTriggered += delegate { _restoreTriggered = true; };
                            _queue.Enqueue(process);   
                        }
                    }
                }

	            if (_queue.Count > 0)
	            {
		            // start the processes.
		            for (int n = 0; n < settings.MaxConcurrency && n < _queue.Count; n++)
		            {
			            LaunchNextBackgroundProcess();
		            }

		            _allCompleted.WaitOne();

		            if (CancelPending)
			            Platform.Log(LogLevel.Info, "All import processes have completed gracefully.");
	            }
            }

            if (_restoreTriggered)
            {
                DateTime newScheduledTime = Platform.Time.AddSeconds(Math.Max(settings.RecheckDelaySeconds, 60));
                Platform.Log(LogLevel.Info, "Some Study/Studies need to be restored first. File Import will resume until {0}", newScheduledTime);
                UnlockServiceLock(item, true, newScheduledTime);
            }
            else
            {
                UnlockServiceLock(item, true, Platform.Time.AddSeconds(_importedSopCounter > 0 ? 5 : settings.RecheckDelaySeconds));
            }
            
        }

        

        #endregion

        #region Private Methods


        private bool CheckMultipleInstances()
        {
            TimeSpan cacheDuration = TimeSpan.FromMinutes(2);
            lock (_dbCheckSync)
            {
                if (!_multipleInstanceDetected.HasValue || Math.Abs(Environment.TickCount - _lastDbCheck) > cacheDuration.TotalMilliseconds)
                {

                    _lastDbCheck = Environment.TickCount;

                    using (IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                    {
                        var broker = context.GetBroker<IServiceLockEntityBroker>();

                        var criteria = new ServiceLockSelectCriteria();
                        criteria.ServiceLockTypeEnum.EqualTo(ServiceLockTypeEnum.ImportFiles);
                        criteria.Enabled.EqualTo(true);

                        var list = broker.Find(criteria);

                        _multipleInstanceDetected = list.Count > 1;
                    }
                }
                
            }

            return _multipleInstanceDetected.Value;

        }


        private static ServerFilesystemInfo EnsureFilesystemIsValid(Model.ServiceLock item)
        {
            ServerFilesystemInfo filesystem = null;
            if (item.FilesystemKey != null)
            {
                filesystem = FilesystemMonitor.Instance.GetFilesystemInfo(item.FilesystemKey);
                if (filesystem == null)
                {
                    Platform.Log(LogLevel.Warn, "Filesystem for incoming folders is no longer valid.  Assigning new filesystem.");
                    item.FilesystemKey = null;
                    UpdateFilesystemKey(item);
                }
            }


            if (filesystem == null)
            {
                filesystem = SelectFilesystem();

                if (filesystem == null)
                {
                    UnlockServiceLock(item, true, Platform.Time.AddHours(2));
                }
                else
                {
                    item.FilesystemKey = filesystem.Filesystem.Key;
                    UpdateFilesystemKey(item);
                }
            }
            return filesystem;
        }

        private static ServerFilesystemInfo SelectFilesystem()
        {
            IEnumerable<ServerFilesystemInfo> filesystems = FilesystemMonitor.Instance.GetFilesystems();
            IList<ServerFilesystemInfo> sortedFilesystems = CollectionUtils.Sort(filesystems, FilesystemSorter.SortByFreeSpace);

            if (sortedFilesystems == null || sortedFilesystems.Count == 0)
                return null;
        	return sortedFilesystems[0];
        }

        private void OnBackgroundProcessCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (_sync)
            {
                _inprogress.Remove(sender as DirectoryImporterBackgroundProcess);

                if (!CancelPending && _queue.Count>0)
                {
                    if (_queue.Peek() != null)
                    {
                        LaunchNextBackgroundProcess();
                    }
                }
                
                if (_inprogress.Count==0)
                {
                    _allCompleted.Set();
                }
            }
        }

        private void LaunchNextBackgroundProcess()
        {
            DirectoryImporterBackgroundProcess process = _queue.Dequeue();
            if (process!=null)
            {
                process.RunWorkerCompleted += OnBackgroundProcessCompleted;
                _inprogress.Add(process); 
                process.RunWorkerAsync();
            }
        }

        private static void UpdateFilesystemKey(Model.ServiceLock item)
        {
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                ServiceLockUpdateColumns columns = new ServiceLockUpdateColumns {FilesystemKey = item.FilesystemKey};

            	IServiceLockEntityBroker broker = ctx.GetBroker<IServiceLockEntityBroker>();
                broker.Update(item.Key, columns);
                ctx.Commit();
            }
        }

        #endregion

        #region Overriden Protected Methods

        protected override void OnCancelling()
        {
            lock (_sync)
            {
                Platform.Log(LogLevel.Info, "Signalling child processes to stop...");
                foreach (DirectoryImporterBackgroundProcess worker in _inprogress)
                {
                    if (worker.WorkerSupportsCancellation)
                    {
                        worker.CancelAsync();
                    }
                }
            }
            
        }

        #endregion
    }
}
