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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public partial class SearchResult : IDisposable
    {
        private volatile SynchronizationContext _synchronizationContext;
        private IWorkItemActivityMonitor _activityMonitor;

        private System.Threading.Timer _processChangedStudiesTimer;

        private readonly object _syncLock = new object();
        private readonly Dictionary<string, string> _setChangedStudies;

        private DateTime? _lastUpdateQueryEndTime;
        private bool _queryingForUpdates;
        private bool _hastenUpdateQuery;

        private DateTime? _lastStudyChangeTime;

        private bool _reindexing;

        public void Dispose()
        {
            StopMonitoringStudies();
        }

        public bool SearchInProgress { get; private set; }

        private bool Reindexing
        {
            get { return _reindexing; }
            set
            {
                if (Equals(value, _reindexing))
                    return;

                _reindexing = value;
                SetResultsTitle();

                lock (_syncLock)
                    _hastenUpdateQuery = true; //make a hastier update query when re-indexing status changes.
            }
        }

        private void UpdateReindexing(bool value)
        {
            if (_activityMonitor == null || !_activityMonitor.IsConnected)
                value = false;
            
            Reindexing = value;
        }

        private void UpdateReindexing()
        {
            if (_activityMonitor == null || !_activityMonitor.IsConnected)
            {
                Reindexing = false;
                return;
            }
            
            var request = new WorkItemQueryRequest { Type = ReindexRequest.WorkItemTypeString };
            IEnumerable<WorkItemData> reindexItems = null;
            
            try
            {
                Platform.Log(LogLevel.Debug, "Querying for a re-index work item that is in progress.");

                Platform.GetService<IWorkItemService>(s => reindexItems = s.Query(request).Items);
                Reindexing = reindexItems != null && reindexItems.Any(item => item.Status == WorkItemStatusEnum.InProgress);
            }
            catch (Exception e)
            {
                Reindexing = false;
                Platform.Log(LogLevel.Debug, e);
            }
        }

        private void StartMonitoringStudies()
        {
            if (_activityMonitor != null)
                return;

            _synchronizationContext = SynchronizationContext.Current;
            //Don't use the sync context when monitoring work item activity, since we'll be processing
            //the changed studies asynchronously anyway.
            _activityMonitor = WorkItemActivityMonitor.Create(false);
            _activityMonitor.IsConnectedChanged += OnIsConnectedChangedAsync;
            _activityMonitor.WorkItemsChanged += OnWorkItemsChangedAsync;
            _activityMonitor.StudiesCleared += OnStudiesClearedAsync;

            UpdateReindexing();

            _processChangedStudiesTimer = new System.Threading.Timer(ProcessChangedStudiesAsync, null, 100, 100);
        }

        private void StopMonitoringStudies()
        {
            _synchronizationContext = null;
            if (_activityMonitor != null)
            {
                _activityMonitor.IsConnectedChanged -= OnIsConnectedChangedAsync;
                _activityMonitor.WorkItemsChanged -= OnWorkItemsChangedAsync;
                _activityMonitor.StudiesCleared -= OnStudiesClearedAsync;
                _activityMonitor.Dispose();
                _activityMonitor = null;
            }

            if (_processChangedStudiesTimer != null)
            {
                _processChangedStudiesTimer.Dispose();
                _processChangedStudiesTimer = null;
            }
            
            UpdateReindexing();
        }

        private void OnStudiesCleared()
        {
            Platform.Log(LogLevel.Debug, "Processing 'studies cleared' message from Work Item service.");

            lock (_syncLock)
            {
                _setChangedStudies.Clear();
            }

            _hiddenItems.Clear();
            StudyTable.Items.Clear();
            SetResultsTitle();
        }

        /// <summary>
        /// Marshaled back to the UI thread from a worker thread.
        /// </summary>
        private void UpdatedChangedStudies(DateTime queryStartTime, IList<string> deletedStudyUids, IList<StudyEntry> updatedStudies)
        {
            //If the sync context is null, it's because we're not monitoring studies anymore (e.g. study browser closed).
            if (_synchronizationContext == null)
                return;

            bool hasUserSearched = SearchInProgress || _lastSearchEndTime.HasValue;
            bool updateQueryStartedBeforeUserSearchEnded = _lastSearchEndTime.HasValue && queryStartTime < _lastSearchEndTime.Value;

            //If the user has ever searched, and the query for study updates started before the user's search completed,
            //then we just ignore these updates. Otherwise, the user might get out-of-date updates, and may even see
            //studies appearing in the table while the search is still in progress.
            if (hasUserSearched && updateQueryStartedBeforeUserSearchEnded)
            {
                Platform.Log(LogLevel.Debug, "Disregarding updated studies ({0} changed, {1} deleted); update query started before user search ended.", updatedStudies.Count, deletedStudyUids.Count);
                return;
            }

            Platform.Log(LogLevel.Debug, "{0} studies changed, {1} studies deleted.", updatedStudies.Count, deletedStudyUids.Count);
            
            foreach (var updatedStudy in updatedStudies)
                UpdateTableItem(updatedStudy);

            foreach (string deletedUid in deletedStudyUids)
                DeleteStudy(deletedUid);

            Platform.Log(LogLevel.Debug, "Study count = {0}.", _studyTable.Items.Count);

            SetResultsTitle();
        }

        private void UpdateTableItem(StudyEntry entry)
        {
            //don't need to check this again, it's just paranoia
            if (!StudyExists(entry.Study.StudyInstanceUid))
            {
                StudyTable.Items.Add(new StudyTableItem(entry));
            }
            else
            {
                int index = GetStudyIndex(entry.Study.StudyInstanceUid);
                //just update this since the rest won't change.
                StudyTable.Items[index].Entry = entry;
                StudyTable.Items.NotifyItemUpdated(index);
            }
        }

        private void DeleteStudy(string studyInstanceUid)
        {
            int foundIndex = StudyTable.Items.FindIndex(test => test.StudyInstanceUid == studyInstanceUid);
            if (foundIndex >= 0)
                StudyTable.Items.RemoveAt(foundIndex);

            foundIndex = _hiddenItems.FindIndex(test => test.StudyInstanceUid == studyInstanceUid);
            if (foundIndex >= 0)
                _hiddenItems.RemoveAt(foundIndex);
        }

        private bool StudyExists(string studyInstanceUid)
        {
            return GetStudyIndex(studyInstanceUid) >= 0;
        }

        private int GetStudyIndex(string studyInstanceUid)
        {
            return StudyTable.Items.FindIndex(test => test.StudyInstanceUid == studyInstanceUid);
        }

        #region Async Operations

        private void OnIsConnectedChangedAsync(object sender, EventArgs e)
        {
            var syncContext = _synchronizationContext;
            if (syncContext != null)
                syncContext.Post(ignore => UpdateReindexing(), null);
        }

        /// <summary>
        /// Studies cleared (from activity monitor) event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStudiesClearedAsync(object sender, EventArgs e)
        {
            var syncContext = _synchronizationContext;
            if (syncContext != null)
                syncContext.Post(ignore => OnStudiesCleared(), null);
        }

        /// <summary>
        /// Work Items Changed event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnWorkItemsChangedAsync(object sender, WorkItemsChangedEventArgs args)
        {
			// ignore refresh events
            if (args.EventType == WorkItemsChangedEventType.Refresh || args.ChangedItems == null)
                return;

            var syncContext = _synchronizationContext;
            if (syncContext == null)
                return;

            lock (_syncLock)
            {
                var previousChangedStudiesCount = _setChangedStudies.Count;
                foreach (var item in args.ChangedItems)
                {
                    var theItem = item;
                    if (theItem.Type.Equals(ReindexRequest.WorkItemTypeString))
                        syncContext.Post(ignore => UpdateReindexing(theItem.Status == WorkItemStatusEnum.InProgress), false);
                    
                    if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyRead)
                        continue; //If it's a "read" operation, don't update anything.

                    if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyDelete)
                        _hastenUpdateQuery = true; //We want deleted studies to disappear quickly, so make a hasty update query.

                    //If it's not a read operation, but it has a Study UID, then it's probably updating studies.
                    //(e.g. re-index, re-apply rules, import, process study).
                    if (!String.IsNullOrEmpty(item.StudyInstanceUid))
                    {
                        if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                            Platform.Log(LogLevel.Debug, "Study '{0}' has changed (Work Item: {1}, {2})", item.StudyInstanceUid, item.Identifier, item.Type);

                        _setChangedStudies[item.StudyInstanceUid] = item.StudyInstanceUid;
                    }
                }

                if (_setChangedStudies.Count > previousChangedStudiesCount)
                {
                    Platform.Log(LogLevel.Debug, "{0} studies changed since the last update.", _setChangedStudies.Count);

                    //Only update this if we've received a message for a study not already in the set.
                    //We're only interested in the unique set of studies that has changed (or is changing)
                    //since the last time we updated the studies in the study list.
                    _lastStudyChangeTime = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Process studies timer method.
        /// </summary>
        /// <param name="state"></param>
        private void ProcessChangedStudiesAsync(object state)
        {
            //If the sync context is null, it's because we're not monitoring studies anymore (e.g. study browser closed).
            var syncContext = _synchronizationContext;
            if (syncContext == null)
                return;

            lock (_tableRefreshLock)
            {
                if (!_isTableRefreshCheckPending)
                {
                    //This timer executes every 100ms (10/sec), which is way too frequently for this check, however,
                    //it's also silly to have another less frequent timer dedicated to this, so we just slow it down.
                    if (!_lastTableRefreshCheck.HasValue || (DateTime.Now - _lastTableRefreshCheck.Value).TotalSeconds > 5)
                    {
                        _isTableRefreshCheckPending = true;
                        syncContext.Post(delegate
                                             {
                                                 //Do the entire thing (check+refresh) on the UI thread, so we don't
                                                 //need to worry about synchronization of the 2 "last" time variables.
                                                 //(_lastSearchEndTime, _lastTableRefreshTime).
                                                 if (StudyTableNeedsRefresh())
                                                     RefreshStudyTable();
                                             }, null);
                    }
                }
            }

            DateTime? queryStartTime;
            List<string> deletedStudyUids;
            List<StudyEntry> updatedStudies;
                
            ProcessChangedStudiesAsync(out queryStartTime, out deletedStudyUids, out updatedStudies);
            if (deletedStudyUids.Count > 0 || updatedStudies.Count > 0)
                syncContext.Post(ignore => UpdatedChangedStudies(queryStartTime.Value, deletedStudyUids, updatedStudies), null);
        }

        /// <summary>
        /// Figure out which studies have been deleted and/or updated.
        /// </summary>
        private void ProcessChangedStudiesAsync(out DateTime? queryStartTime, out List<string> deletedStudyUids, out List<StudyEntry> updatedStudies)
        {
            deletedStudyUids = new List<string>();
            updatedStudies = new List<StudyEntry>();
            queryStartTime = null;
            DateTime now = DateTime.Now;

            var fiveSeconds = TimeSpan.FromSeconds(5);
            var rapidChangeInterval = TimeSpan.FromMilliseconds(300);

            lock (_syncLock)
            {
                if (_queryingForUpdates)
                    return; //Already querying.

                //Nothing to query for? Return.
                if (_setChangedStudies.Count == 0 || !_lastStudyChangeTime.HasValue)
                    return;

                bool studiesChanging = now - _lastStudyChangeTime.Value < rapidChangeInterval;
                if (studiesChanging)
                {
                    //Many DIFFERENT studies are changing in very rapid succession. Delay until it settles down, which usually isn't long.
                    Platform.Log(LogLevel.Debug, "Studies are still actively changing - delaying update.");
                    return;
                }
                
                if (!_hastenUpdateQuery)
                {
                    bool updatedRecently = _lastUpdateQueryEndTime.HasValue && now - _lastUpdateQueryEndTime < fiveSeconds;
                    if (updatedRecently)
                    {
                        //We just finished an update query less than 5 seconds ago.
                        Platform.Log(LogLevel.Debug, "Studies were updated within the last 5 seconds - delaying update.");
                        return;
                    }
                }

                //Reset this before the immediate query.
                _hastenUpdateQuery = false;

                //Add everything to the deleted list.
                deletedStudyUids.AddRange(_setChangedStudies.Keys);
                _setChangedStudies.Clear();

                //We are officially querying for updates.
                _queryingForUpdates = true;
            }

            queryStartTime = now;
            string studyUids = DicomStringHelper.GetDicomStringArray(deletedStudyUids);

            try
            {
                var clock = new CodeClock();
                clock.Start();
             
                var criteria = new StudyRootStudyIdentifier { StudyInstanceUid = studyUids };
                var request = new GetStudyEntriesRequest { Criteria = new StudyEntry { Study = criteria } };
                
                IList<StudyEntry> entries = null;
                
                //We're doing it this way here because it's local only.
                Platform.GetService<IStudyStoreQuery>(s => entries = s.GetStudyEntries(request).StudyEntries);

                foreach (var entry in entries)
                {
                    //If we got a result back, then it's not deleted.
                    deletedStudyUids.Remove(entry.Study.StudyInstanceUid);
                    updatedStudies.Add(entry);
                }

                clock.Stop();
                Platform.Log(LogLevel.Debug, "Study update query took {0}.", clock);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
            }
            finally
            {
                lock (_syncLock)
                {
                    //Finished querying for updates.
                    _queryingForUpdates = false;
                    _lastUpdateQueryEndTime = now;
                }
            }
        }

        #endregion
    }
}
