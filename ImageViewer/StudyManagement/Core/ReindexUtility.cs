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
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{
    // TODO (CR Jun 2012 - Med): Should re-index be auditing all the studies it finds/deletes?

    /// <summary>
    /// Class for performing a Reindex of the database.
    /// </summary>
    public class ReindexUtility
    {
        #region Private Sub-Class

        private class ThreadPoolStopProxy
        {
            private readonly ReprocessStudyFolder _r;

            public ThreadPoolStopProxy(ReprocessStudyFolder r)
            {
                _r = r;
            }

            public void ProxyDelegate(object sender, ItemEventArgs<ThreadPoolBase.StartStopState> e)
            {
                if (e.Item == ThreadPoolBase.StartStopState.Stopping)
                    _r.Cancel();
            }
        }

        #endregion

        #region Private members

        private event EventHandler<StudyEventArgs> _studyDeletedEvent;
        private event EventHandler<StudyEventArgs> _studyFolderProcessedEvent;
        private event EventHandler<StudyEventArgs> _studyProcessedEvent;
        private event EventHandler<StudiesEventArgs> _studiesRestoredEvent;
        private event EventHandler<StudyEventArgs> _studyFailedEvent;

        private readonly object _syncLock = new object();
        private bool _cancelRequested;
        private readonly ItemProcessingThreadPool<ReprocessStudyFolder> _threadPool = new ItemProcessingThreadPool<ReprocessStudyFolder>(WorkItemServiceSettings.Default.NormalThreadCount);
        #endregion

        #region Public Events

        public class StudyEventArgs : EventArgs
        {
            public string StudyInstanceUid;
            public string Message;
        }

        public class StudiesEventArgs : EventArgs
        {
            public IList<string> StudyInstanceUids;
        }    

        public event EventHandler<StudyEventArgs> StudyDeletedEvent
        {
            add
            {
                lock (_syncLock)
                {
                    _studyDeletedEvent += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _studyDeletedEvent -= value;
                }
            }
        }

        public event EventHandler<StudyEventArgs> StudyFolderProcessedEvent
        {
            add
            {
                lock (_syncLock)
                {
                    _studyFolderProcessedEvent += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _studyFolderProcessedEvent -= value;
                }
            }
        }

        public event EventHandler<StudyEventArgs> StudyReindexFailedEvent
        {
            add
            {
                lock (_syncLock)
                {
                    _studyFailedEvent += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _studyFailedEvent -= value;
                }
            }
        }

        public event EventHandler<StudyEventArgs> StudyProcessedEvent
        {
            add
            {
                lock (_syncLock)
                {
                    _studyProcessedEvent += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _studyProcessedEvent -= value;
                }
            }
        }

        public event EventHandler<StudiesEventArgs> StudiesRestoredEvent
        {
            add
            {
                lock (_syncLock)
                {
                    _studiesRestoredEvent += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _studiesRestoredEvent -= value;
                }
            }
        }

        #endregion

        #region Public Properties

        public int StudyFoldersToScan { get; private set; }

        public int DatabaseStudiesToScan { get; private set; }

        public string FilestoreDirectory { get; private set; }

        public List<string> DirectoryList { get;private set; }

        public List<long> StudyOidList { get; private set; }

        #endregion

        #region Constructors

        public ReindexUtility()
        {
            FilestoreDirectory = GetFileStoreDirectory();
            DirectoryList = new List<string>();
            _threadPool.ThreadPoolName = "Re-index";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize the Reindex.  Determine the number of studies in the database and the number of folders on disk to be used
        /// for progress.
        /// </summary>
        public void Initialize()
        {
            // Before scanning the study folders, cleanup any empty directories.
            CleanupFilestoreDirectory();

            try
            {
                DirectoryList = new List<string>(Directory.GetDirectories(FilestoreDirectory));
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x);
                throw;
            }

            StudyFoldersToScan = DirectoryList.Count;

            // TODO (CR Jun 2012): Seems we're using the "work item" mutex for all updates to the database.
            // Should we just pass in a boolean specifying whether or not to use a mutex?
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetStudyBroker();

                StudyOidList = new List<long>(); 
                
                var studyList = broker.GetStudies();
                foreach (var study in studyList)
                {
                    study.Reindex = true;
                    StudyOidList.Add(study.Oid);
                }
                context.Commit();
            }

            DatabaseStudiesToScan = StudyOidList.Count;   
        
            _threadPool.Start();
        }

        /// <summary>
        /// Process the Reindex.
        /// </summary>
        public void Process()
        {            
            ProcessStudiesInDatabase();

            if (_cancelRequested)
            {
                ResetReindexStudies();
                return;
            }

            ProcessFilesystem();

            // Before scanning the study folders, cleanup any empty directories.
            CleanupFilestoreDirectory();

            if (_cancelRequested)
            {
                ResetReindexStudies();
            }
        }

        /// <summary>
        /// Cancel the reindex
        /// </summary>
        public void Cancel()
        {
            if (_cancelRequested) return;
            
            _cancelRequested = true;             
        }

        #endregion

        #region Private Methods

        private void ResetReindexStudies()
        {
            var resetStudyUids = new List<string>();
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var studyBroker = context.GetStudyBroker();

                var studyList = studyBroker.GetReindexStudies();
                foreach (var study in studyList)
                {                    
                    if (study.Reindex)
                    {
                        resetStudyUids.Add(study.StudyInstanceUid);
                        study.Reindex = false;
                    }
                }
                context.Commit();
            }

            if (resetStudyUids.Count > 0)
                EventsHelper.Fire(_studiesRestoredEvent, this, new StudiesEventArgs { StudyInstanceUids = resetStudyUids });
        }

        private void CleanupFilestoreDirectory()
        {
            try
            {
                DirectoryUtility.DeleteEmptySubDirectories(FilestoreDirectory, true);
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Warn, x, "Unexpected exception cleaning up empty subdirectories in filestore: {0}",
                             FilestoreDirectory);
            }
        }

        private void ProcessStudiesInDatabase()
        {
            foreach (long oid in StudyOidList)
            {
                try
                {
                    using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
                    {
                        var broker = context.GetStudyBroker();
                        var study = broker.GetStudy(oid);

                        var location = new StudyLocation(study.StudyInstanceUid);
                        if (!Directory.Exists(location.StudyFolder))
                        {
                            broker.Delete(study);
                            context.Commit();

                            EventsHelper.Fire(_studyDeletedEvent, this, new StudyEventArgs { StudyInstanceUid = study.StudyInstanceUid });
                            Platform.Log(LogLevel.Info, "Deleted Study that wasn't on disk, but in the database: {0}",
                                         study.StudyInstanceUid);
                        }
                        else
                            EventsHelper.Fire(_studyProcessedEvent, this, new StudyEventArgs { StudyInstanceUid = study.StudyInstanceUid });
                    }                    
                }
                catch (Exception x)
                {
                    Platform.Log(LogLevel.Warn, "Unexpected exception attempting to reindex StudyOid {0}: {1}", oid, x.Message);
                }

                if (_cancelRequested) return;
            }
        }

        private void ProcessFilesystem()
        {
            foreach (string studyFolder in DirectoryList)
            {
                ProcessStudyFolder(studyFolder);

                if (_cancelRequested)
                    break;
            }

            while (_threadPool.Active && (_threadPool.QueueCount > 0 || _threadPool.ActiveCount > 0))
            {
                if (_cancelRequested)
                    break;
                Thread.Sleep(500);
                if (_cancelRequested)
                    break;
            }
            
            _threadPool.Stop(false);
        }

        private void ProcessStudyFolder(string folder)
        {
            try
            {
                string studyInstanceUid = Path.GetFileName(folder);
                var location = new StudyLocation(studyInstanceUid);

                var reprocessStudyFolder = new ReprocessStudyFolder(location);

                _threadPool.Enqueue(reprocessStudyFolder, delegate(ReprocessStudyFolder r)
                                                              {
                                                                  var del = new ThreadPoolStopProxy(r);
                                                                  _threadPool.StartStopStateChangedEvent += del.ProxyDelegate;
                                                                  // TODO (CR Jun 2012): Should check the thread pool state right after
                                                                  // subscribing; otherwise, the folder will process to completion, rather than canceling.

                                                                  r.Process();

                                                                  lock (_syncLock)
                                                                      EventsHelper.Fire(
                                                                          r.Failed ? _studyFailedEvent
                                                                                   : _studyFolderProcessedEvent, 
                                                                                   this,
                                                                          new StudyEventArgs
                                                                              {
                                                                                  StudyInstanceUid = r.Location.Study.StudyInstanceUid,
                                                                                  Message = r.FailureMessage
                                                                              });

                                                                  _threadPool.StartStopStateChangedEvent -= del.ProxyDelegate;
                                                              });
            }
            catch (Exception x)
            {
                // TODO (CR Jun 2012): Shouldn't this cause the work item to fail?
                Platform.Log(LogLevel.Error, x, "Unexpected exception reindexing folder: {0}", folder);
            }
        }

        private static string GetFileStoreDirectory()
        {
            string directory = StudyStore.FileStoreDirectory;
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }

        #endregion
    }
}
