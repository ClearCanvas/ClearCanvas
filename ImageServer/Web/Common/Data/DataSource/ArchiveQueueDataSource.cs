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
using ClearCanvas.Web.Enterprise.Authentication;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
	public class ArchiveQueueSummary
	{
		#region Private members

		private string _patientID;
		private string _patientName;
		private ServerPartition _thePartition;
		private ArchiveQueue _theArchiveQueueItem;
		private string _notes;
		private StudyStorage _studyStorage;

		#endregion Private members

		#region Public Properties

		public DateTime ScheduledDateTime
		{
			get { return _theArchiveQueueItem.ScheduledTime; }
		}

		public string StatusString
		{
			get { return ServerEnumDescription.GetLocalizedDescription(_theArchiveQueueItem.ArchiveQueueStatusEnum); }
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
			get { return _theArchiveQueueItem.Key; }
		}

		public string ProcessorId
		{
			get { return _theArchiveQueueItem.ProcessorId; }
		}
		public ArchiveQueue TheArchiveQueueItem
		{
			get { return _theArchiveQueueItem; }
			set { _theArchiveQueueItem = value; }
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

	public class ArchiveQueueDataSource
	{
		#region Public Delegates
		public delegate void ArchiveQueueFoundSetDelegate(IList<ArchiveQueueSummary> list);
		public ArchiveQueueFoundSetDelegate ArchiveQueueFoundSet;
		#endregion

		#region Private Members
		private readonly ArchiveQueueController _searchController = new ArchiveQueueController();
		private string _patientId;
		private string _patientName;
		private string _scheduledDate;
		private int _resultCount;
		private ServerPartition _partition;
		private ArchiveQueueStatusEnum _statusEnum;
		private string _dateFormats;
		private IList<ArchiveQueueSummary> _list = new List<ArchiveQueueSummary>();
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
		public ArchiveQueueStatusEnum StatusEnum
		{
			get { return _statusEnum; }
			set { _statusEnum = value; }
		}
		public string DateFormats
		{
			get { return _dateFormats; }
			set { _dateFormats = value; }
		}
		public IList<ArchiveQueueSummary> List
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
		private IList<ArchiveQueue> InternalSelect(int startRowIndex, int maximumRows, out int resultCount)
		{
			resultCount = 0;

			if (maximumRows == 0) return new List<ArchiveQueue>();

			if (SearchKeys != null)
			{
				IList<ArchiveQueue> archiveQueueList = new List<ArchiveQueue>();
				foreach (ServerEntityKey key in SearchKeys)
					archiveQueueList.Add(ArchiveQueue.Load(key));

				resultCount = archiveQueueList.Count;

				return archiveQueueList;
			}

			WebQueryArchiveQueueParameters parameters = new WebQueryArchiveQueueParameters();
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
				parameters.ArchiveQueueStatusEnum = StatusEnum;

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

			IList<ArchiveQueue> list = _searchController.FindArchiveQueue(parameters);

			resultCount = parameters.ResultCount;

			return list;
		}
		#endregion

		#region Public Methods
		public IEnumerable<ArchiveQueueSummary> Select(int startRowIndex, int maximumRows)
		{
			IList<ArchiveQueue> list = InternalSelect(startRowIndex, maximumRows, out _resultCount);

			_list = new List<ArchiveQueueSummary>();
			foreach (ArchiveQueue item in list)
				_list.Add(CreateWorkQueueSummary(item));

			if (ArchiveQueueFoundSet != null)
				ArchiveQueueFoundSet(_list);

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
		/// Constructs an instance of <see cref="WorkQueueSummary"/> based on a <see cref="WorkQueue"/> object.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		/// <remark>
		/// 
		/// </remark>
		private ArchiveQueueSummary CreateWorkQueueSummary(ArchiveQueue item)
		{
			ArchiveQueueSummary summary = new ArchiveQueueSummary();
			summary.TheArchiveQueueItem = item;
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
			Study theStudy = studyAdaptor.GetFirst(studycriteria);

			if (theStudy == null)
			{
				summary.PatientId = "N/A";
				summary.PatientsName = "N/A";
			}
			else
			{
				summary.PatientId = theStudy.PatientId;
				summary.PatientsName = theStudy.PatientsName;
			}

			return summary;
		}
		#endregion
	}
}