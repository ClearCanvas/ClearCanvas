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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	public partial class SearchResult
	{
		private string _serverGroupName;
		private bool _isLocalServer;
		private int _numberOfChildServers;

	    private readonly StudyTable _studyTable;
        private readonly List<StudyTableItem> _hiddenItems;

		private bool _filterDuplicates;
        private DateTime? _lastSearchEndTime;
        private string _resultsTitle;

        private readonly object _tableRefreshLock = new object();
        private bool _isTableRefreshCheckPending;
        private DateTime? _lastTableRefreshCheck;
        private DateTime? _lastTableRefreshTime;

	    public SearchResult()
		{
			HasDuplicates = false;

			_serverGroupName = "";
			_isLocalServer = false;
			_numberOfChildServers = 1;

            _hiddenItems = new List<StudyTableItem>();
            _studyTable = new StudyTable();
            _setChangedStudies = new Dictionary<string, string>();
		}

		#region Properties

		public string ServerGroupName
		{
			get { return _serverGroupName; }
			set { _serverGroupName = value; }
		}

		public bool IsLocalServer
		{
			get { return _isLocalServer; }
			set
			{
                if (Equals(_isLocalServer, value))
                    return;

				_isLocalServer = value;
				UpdateColumnVisibility();
                if (_isLocalServer)
                    StartMonitoringStudies();
                else
                    StopMonitoringStudies();
			}
		}

		public int NumberOfChildServers
		{
			get { return _numberOfChildServers; }
			set
			{
				_numberOfChildServers = value;
                UpdateColumnVisibility();
			}
		}

	    public StudyTable StudyTable
		{
			get { return _studyTable; }
		}

		public string ResultsTitle
		{
            get { return _resultsTitle; }
            private set
            {
                if (Equals(_resultsTitle, value))
                    return;

                _resultsTitle = value;
                EventsHelper.Fire(ResultsTitleChanged, this, EventArgs.Empty);
            }
		}

	    public event EventHandler ResultsTitleChanged;

		public bool HasDuplicates { get; private set; }

		public bool FilterDuplicates
		{
			get { return _filterDuplicates; }
			set
			{
				_filterDuplicates = value;
				if (_filterDuplicates)
				{
					if (_hiddenItems.Count == 0)
					{
					    RemoveDuplicates(_studyTable.Items, _hiddenItems);
					}
				}
				else
				{
					if (_hiddenItems.Count > 0)
					{
						_studyTable.Items.AddRange(_hiddenItems);
						_hiddenItems.Clear();
					}
				}

				StudyTable.Sort();
                SetResultsTitle();
			}
		}
		
        #endregion

		public void Initialize()
		{
            _studyTable.Initialize();
            //_studyTable.UseSinglePatientNameColumn = true;
            SetResultsTitle();
		}

        public void SearchStarted()
        {
            SearchInProgress = true;
        }

        public void SearchCanceled()
        {
            SearchInProgress = false;
        }

        public void SearchEnded(List<StudyTableItem> tableItems, bool filterDuplicates)
        {
            _lastSearchEndTime = _lastTableRefreshTime = DateTime.Now;
	        SearchInProgress = false;
			_filterDuplicates = filterDuplicates;

			_hiddenItems.Clear();
            var filteredItems = new List<StudyTableItem>(tableItems);
			RemoveDuplicates(filteredItems, _hiddenItems);
			HasDuplicates = _hiddenItems.Count > 0;

			if (!_filterDuplicates)
			{
				_hiddenItems.Clear();
				_studyTable.Items.Clear();
				_studyTable.Items.AddRange(tableItems);
			}
			else
			{
				_studyTable.Items.Clear();
				_studyTable.Items.AddRange(filteredItems);
			}

			StudyTable.Sort();
            SetResultsTitle();
        }

        // Hack for #10072. We can't inspect the values that are actually shown in the table to see if they need
        // updated because, for example, midnight crossed and a value needs to change. So, we're hacking it
        // just for the one specific case where midnight is crossed and the "Delete On" column needs to say
        // "Today" instead of "Yesterday", for example.

        #region Study Table Refresh Hack

        private bool StudyTableNeedsRefresh()
        {
            var now = DateTime.Now;
            lock (_syncLock)
            {
                _lastTableRefreshCheck = now;
                _isTableRefreshCheckPending = false;
            }

            //The user has never searched, so there is nothing to refresh.
            if (!_lastSearchEndTime.HasValue)
                return false;

            //There is nothing in the table to refresh.
            if (_studyTable.Items.Count == 0)
                return false;

            //Has midnight been crossed since the last time the table was refreshed?
            var lastRefreshTime = _lastTableRefreshTime.HasValue ? _lastTableRefreshTime.Value : _lastSearchEndTime.Value;
            var timeSinceLastRefresh = now - lastRefreshTime;
            var nowTimeOfDay = now.TimeOfDay;
            if (timeSinceLastRefresh < nowTimeOfDay)
                return false; //haven't crossed midnight yet.

            return true;
        }

        private void RefreshStudyTable()
        {
            _lastTableRefreshTime = DateTime.Now;
            var allItems = new List<StudyTableItem>(_studyTable.Items);

            using (_studyTable.Items.BeginTransaction())
            {
                _studyTable.Items.Clear();
                _studyTable.Items.AddRange(allItems);
            }
        }

        #endregion

        private void SetResultsTitle()
        {
            var everSearched = _lastSearchEndTime.HasValue;
            if (!everSearched)
            {
                ResultsTitle = Reindexing
                                   ? String.Format(SR.FormatNeverSearchedReindexing, _serverGroupName)
                                   : _serverGroupName;
            }
            else
            {
                ResultsTitle = Reindexing
                                   ? String.Format(SR.FormatStudiesFoundReindexing, _studyTable.Items.Count, _serverGroupName)
                                   : String.Format(SR.FormatStudiesFound, _studyTable.Items.Count, _serverGroupName);
            }
        }

        private static void RemoveDuplicates(IList<StudyTableItem> allItems, List<StudyTableItem> removedItems)
		{
			removedItems.Clear();

            var uniqueItems = new Dictionary<string, StudyTableItem>();
            foreach (StudyTableItem item in allItems)
			{
				StudyTableItem existing;
                if (uniqueItems.TryGetValue(item.StudyInstanceUid, out existing))
                {
                    var server = item.Server;
					//we will only replace an existing entry if this study's server is streaming.
					if (server != null && server.StreamingParameters != null)
					{
						//only replace existing entry if it is on a non-streaming server.
                        server = existing.Server;
                        if (server == null || server.StreamingParameters == null)
						{
							removedItems.Add(existing);
							uniqueItems[item.StudyInstanceUid] = item;
							continue;
						}
					}

					//this study is a duplicate.
					removedItems.Add(item);
				}
				else
				{
                    uniqueItems[item.StudyInstanceUid] = item;
				}
			}

			foreach (StudyTableItem removedItem in removedItems)
				allItems.Remove(removedItem);
		}

        internal void UpdateColumnVisibility()
        {
            _studyTable.SetAvailabilityColumnsVisibility(!_isLocalServer);
            _studyTable.SetServerColumnVisibility(!_isLocalServer && _numberOfChildServers > 1);
            _studyTable.SetColumnVisibility(StudyTable.ColumnNameDeleteOn, _isLocalServer);
        }

        public static SearchResultColumnOptionCollection ColumnOptions
        {
            get
            {
                try
                {
                    return new SearchResultColumnOptionCollection(DicomExplorerConfigurationSettings.Default.ResultColumns);
                }
                catch (Exception)
                {
                    return new SearchResultColumnOptionCollection();
                }
            }
            set
            {
                DicomExplorerConfigurationSettings.Default.ResultColumns = value;
                DicomExplorerConfigurationSettings.Default.Save();
            }
        }
    }
}
