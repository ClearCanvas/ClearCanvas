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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.Web.Enterprise.Authentication;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
	public class RestoreQueueSummary
	{
		#region Private members

		private string _patientID;
		private string _patientName;
		private ServerPartition _thePartition;
		private RestoreQueue _theRestoreQueueItem;
		private string _notes;
		private StudyStorage _studyStorage;

		#endregion Private members

		#region Public Properties

		public DateTime ScheduledDateTime
		{
			get { return _theRestoreQueueItem.ScheduledTime; }
		}

		public string StatusString
		{
			get { return ServerEnumDescription.GetLocalizedDescription(_theRestoreQueueItem.RestoreQueueStatusEnum); }
		}

		public string PatientId
		{
			get { return _patientID; }
			set { _patientID = value; }
		}

		public string PatientsName
		{
			get { return _patientName; }
			set { _patientName = value; }
		}

		public ServerEntityKey Key
		{
			get { return _theRestoreQueueItem.Key; }
		}

		public string ProcessorId
		{
			get { return _theRestoreQueueItem.ProcessorId; }
		}
		public RestoreQueue TheRestoreQueueItem
		{
			get { return _theRestoreQueueItem; }
			set { _theRestoreQueueItem = value; }
		}
		public ServerPartition ThePartition
		{
			get { return _thePartition; }
			set { _thePartition = value; }
		}
		public String Notes
		{
			get { return _notes; }
			set { _notes = value; }
		}
		public StudyStorage StudyStorage
		{
			get { return _studyStorage; }
			set { _studyStorage = value; }
		}
		#endregion Public Properties
	}

	public class RestoreQueueDataSource
	{
		#region Public Delegates
		public delegate void RestoreQueueFoundSetDelegate(IList<RestoreQueueSummary> list);
		public RestoreQueueFoundSetDelegate RestoreQueueFoundSet;
		#endregion

		#region Private Members
		private readonly RestoreQueueController _searchController = new RestoreQueueController();
		private string _patientId;
		private string _patientName;
		private string _scheduledDate;
		private int _resultCount;
		private ServerPartition _partition;
		private RestoreQueueStatusEnum _statusEnum;
		private string _dateFormats;
		private IList<RestoreQueueSummary> _list = new List<RestoreQueueSummary>();
		private IList<ServerEntityKey> _searchKeys;
		#endregion

		#region Public Properties

		public string PatientName
		{
			get { return _patientName; }
			set { _patientName = value; }
		}
		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}
		public string ScheduledDate
		{
			get { return _scheduledDate; }
			set { _scheduledDate = value; }
		}
		public ServerPartition Partition
		{
			get { return _partition; }
			set { _partition = value; }
		}
		public RestoreQueueStatusEnum StatusEnum
		{
			get { return _statusEnum; }
			set { _statusEnum = value; }
		}
		public string DateFormats
		{
			get { return _dateFormats; }
			set { _dateFormats = value; }
		}
		public IList<RestoreQueueSummary> List
		{
			get { return _list; }
		}
		public int ResultCount
		{
			get { return _resultCount; }
			set { _resultCount = value; }
		}
		public IList<ServerEntityKey> SearchKeys
		{
			get { return _searchKeys; }
			set { _searchKeys = value; }
		}

		#endregion

		#region Private Methods
		private IList<RestoreQueue> InternalSelect(int startRowIndex, int maximumRows, out int resultCount)
		{
			resultCount = 0;

			if (maximumRows == 0) return new List<RestoreQueue>();

			if (SearchKeys != null)
			{
				IList<RestoreQueue> archiveQueueList = new List<RestoreQueue>();
				foreach (ServerEntityKey key in SearchKeys)
					archiveQueueList.Add(RestoreQueue.Load(key));

				resultCount = archiveQueueList.Count;

				return archiveQueueList;
			}

			WebQueryRestoreQueueParameters parameters = new WebQueryRestoreQueueParameters();
			parameters.StartIndex = startRowIndex;
			parameters.MaxRowCount = maximumRows;
			if (Partition != null)
				parameters.ServerPartitionKey = Partition.Key;

			if (!string.IsNullOrEmpty(PatientId))
			{
				string key = PatientId.Replace("*", "%");
				key = key.Replace("?", "_");
				parameters.PatientId = key;
			}
			if (!string.IsNullOrEmpty(PatientName))
			{
				string key = PatientName.Replace("*", "%");
				key = key.Replace("?", "_");
				parameters.PatientsName = key;
			}

			if (String.IsNullOrEmpty(ScheduledDate))
				parameters.ScheduledTime = null;
			else
				parameters.ScheduledTime = DateTime.ParseExact(ScheduledDate, DateFormats, null);

			if (StatusEnum != null)
				parameters.RestoreQueueStatusEnum = StatusEnum;


            List<string> groupOIDs = new List<string>();
            CustomPrincipal user = Thread.CurrentPrincipal as CustomPrincipal;
            if (user != null)
            {
                if (!user.IsInRole(ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllStudies))
                {
                    foreach (var oid in user.Credentials.DataAccessAuthorityGroups)
                        groupOIDs.Add(oid.ToString());

                    parameters.CheckDataAccess = true;
                    parameters.UserAuthorityGroupGUIDs = StringUtilities.Combine(groupOIDs, ",");
                }
            }

			IList<RestoreQueue> list = _searchController.FindRestoreQueue(parameters);

			resultCount = parameters.ResultCount;

			return list;
		}
		#endregion

		#region Public Methods
		public IEnumerable<RestoreQueueSummary> Select(int startRowIndex, int maximumRows)
		{
			IList<RestoreQueue> list = InternalSelect(startRowIndex, maximumRows, out _resultCount);

			_list = new List<RestoreQueueSummary>();
			foreach (RestoreQueue item in list)
				_list.Add(CreateWorkQueueSummary(item));

			if (RestoreQueueFoundSet != null)
				RestoreQueueFoundSet(_list);

			return _list;
		}

		public int SelectCount()
		{
			if (ResultCount != 0) return ResultCount;

			// Ignore the search results
			InternalSelect(0, 1, out _resultCount);

			return ResultCount;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Constructs an instance of <see cref="WorkQueue"/> based on a <see cref="WorkQueueSummary"/> object.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		/// <remark>
		/// 
		/// </remark>
		private RestoreQueueSummary CreateWorkQueueSummary(RestoreQueue item)
		{
			RestoreQueueSummary summary = new RestoreQueueSummary();
			summary.TheRestoreQueueItem = item;
			summary.ThePartition = Partition;

			if (item.FailureDescription == null)
				summary.Notes = String.Empty;
			else
				summary.Notes = item.FailureDescription;

			// Fetch the patient info:
			StudyStorageAdaptor ssAdaptor = new StudyStorageAdaptor();
			summary.StudyStorage = ssAdaptor.Get(item.StudyStorageKey);
			if (summary.StudyStorage == null)
			{
				summary.PatientId = "N/A";
				summary.PatientsName = "N/A";
				return summary;
			}
			StudyAdaptor studyAdaptor = new StudyAdaptor();
			StudySelectCriteria studycriteria = new StudySelectCriteria();
			studycriteria.StudyInstanceUid.EqualTo(summary.StudyStorage.StudyInstanceUid);
			studycriteria.ServerPartitionKey.EqualTo(summary.StudyStorage.ServerPartitionKey);
			IList<Study> studyList = studyAdaptor.Get(studycriteria);

			if (studyList == null || studyList.Count == 0)
			{
				summary.PatientId = "N/A";
				summary.PatientsName = "N/A";
			}
			else
			{
				summary.PatientId = studyList[0].PatientId;
				summary.PatientsName = studyList[0].PatientsName;
			}

			return summary;
		}
		#endregion
	}
}