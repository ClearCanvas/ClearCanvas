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
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Authentication;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Core.Query;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
	/// <summary>
	/// Model used in study list grid control <see cref="Study"/>.
	/// </summary>
	/// <remarks>
	/// </remarks>
	[Serializable]
	public class StudySummary
	{
		#region Private members

		private bool? _requiresWorkQueueAttention;

	    #endregion Private members


		#region Public Properties

		public ServerEntityKey Key { get; set; }

		public string PatientId { get; set; }

		public string PatientsName { get; set; }

		public string StudyDate { get; set; }

		public string AccessionNumber { get; set; }

		public string StudyDescription { get; set; }

        public string ResponsiblePerson { get; set; }

        public string ResponsiblePersonRole { get; set; }

        public string ResponsibleOrganization { get; set; }

        public string Species { get; set; }

        public string Breed { get; set; }

		public int NumberOfStudyRelatedSeries { get; set; }

		public int NumberOfStudyRelatedInstances { get; set; }

		public StudyStatusEnum StudyStatusEnum { get; set; }

		public QueueStudyStateEnum QueueStudyStateEnum { get; set; }

		public string StudyStatusEnumString
		{
			get
			{
				if (!QueueStudyStateEnum.Equals(QueueStudyStateEnum.Idle))
                    return String.Format("{0}, {1}", 
                        ServerEnumDescription.GetLocalizedDescription(StudyStatusEnum), 
                        ServerEnumDescription.GetLocalizedDescription(QueueStudyStateEnum));

				return ServerEnumDescription.GetLocalizedDescription(StudyStatusEnum);
			}
		}

		public string ModalitiesInStudy { get; set; }

		public Study TheStudy { get; set; }

		public ServerPartition ThePartition { get; set; }

		public ArchiveStudyStorage TheArchiveLocation { get; set; }

		public bool IsArchived
		{
			get
			{
				return TheArchiveLocation != null;
			}
		}

	    public bool IsOnlineLossy
        {
            get { return TheStudyStorage.StudyStatusEnum.Equals(StudyStatusEnum.OnlineLossy); }
        }

		public bool IsLocked { get; set; }

		public bool IsProcessing { get; set; }

		public bool IsArchiving { get; set; }

		public bool IsReconcileRequired { get; set; }

		public bool IsNearline
		{
			get { return StudyStatusEnum == StudyStatusEnum.Nearline; }
		}

		public string StudyInstanceUid { get; set; }

		public string ReferringPhysiciansName { get; set; }

		public string StudyTime { get; set; }

		public string StudyId { get; set; }

		public StudyStorage TheStudyStorage { get; set; }

		public bool HasPendingWorkQueueItems { get; set; }

        public bool HasPendingExternalEdit { get; set; }

		#endregion Public Properties


        public bool RequiresWorkQueueAttention
        {
            get
            {
                if (_requiresWorkQueueAttention==null)
                {
                    _requiresWorkQueueAttention = false;
                    var controller = new StudyController();
                    IList<WorkQueue> workqueueItems = controller.GetWorkQueueItems(TheStudy);
                    foreach (WorkQueue item in workqueueItems)
                    {
                        if (!WorkQueueHelper.IsActiveWorkQueue(item))
                        {
                            _requiresWorkQueueAttention = true;
                            break;
                        }
                    }
                }

                return _requiresWorkQueueAttention.Value;
            }
        }

		public bool HasOrder { get; set; }
		public bool OrderRequiresQC { get; set; }
		public bool StudyIsQCed { get; set; }

		public bool CanScheduleDelete(out string reason)
		{
			if (IsLocked)
			{
				reason = SR.ActionNotAllowed_StudyIsLocked;
				return false;
			}
			if (IsReconcileRequired)
			{
			    reason = SR.ActionNotAllowed_NeedReconcile;
				return false;
			}
			if (IsNearline)
			{
				reason = SR.ActionNotAllowed_StudyIsNearline;
				return false;
			}
			if (ThePartition.ServerPartitionTypeEnum.Equals(ServerPartitionTypeEnum.VFS))
			{
				reason = SR.ActionNotAllowed_ResearchPartition;
				return false;
			}

			reason = String.Empty;
			return true;
		}

        public bool CanScheduleSeriesDelete(out string reason)
        {
            if (IsLocked)
            {
                reason = SR.ActionNotAllowed_StudyIsLocked;
                return false;
            }
            if (IsReconcileRequired)
            {
                reason = SR.ActionNotAllowed_NeedReconcile;
                return false;
            }
            if (IsNearline)
            {
                reason = SR.ActionNotAllowed_StudyIsNearline;
                return false;
            }

            if (StudyStatusEnum.Equals(StudyStatusEnum.OnlineLossy)
            && IsArchivedLossless)
            {
                reason = SR.ActionNotAllowed_StudyIsLossyOnline;
                return false;
            }
            reason = String.Empty;
            return true;
        }

		public bool CanScheduleEdit(out string reason)
		{
			if (IsLocked)
			{
                reason = SR.ActionNotAllowed_StudyIsLocked;
				return false;
			}

			if (IsProcessing)
			{
				reason = SR.ActionNotAllowed_StudyIsBeingProcessing;
				return false;
			}

			if (IsNearline)
			{
				reason = SR.ActionNotAllowed_StudyIsNearline;
				return false;
			}

			if (HasPendingWorkQueueItems)
			{
				reason = SR.ActionNotAllowed_StudyHasPendingWorkQueue;
				return false;
			}

			if (IsReconcileRequired)
			{
				reason = SR.ActionNotAllowed_NeedReconcile;
				return false;
			}

            if (StudyStatusEnum.Equals(StudyStatusEnum.OnlineLossy) 
                && IsArchivedLossless)
            {
                reason = SR.ActionNotAllowed_StudyIsLossyOnline;
                return false;
            }

			reason = String.Empty;
			return true;
		}

	    public bool IsArchivedLossless
	    {
            get { return TheArchiveLocation != null && TheArchiveLocation.ServerTransferSyntax.Lossless; }
	    }


		public bool CanScheduleMove(out string reason)
		{
			if (IsLocked)
			{
				reason = SR.ActionNotAllowed_StudyIsLocked;
				return false;
			}
            
			if (IsReconcileRequired)
			{
				reason = SR.ActionNotAllowed_NeedReconcile;
				return false;
			}
            
			if (IsNearline)
			{
				reason = SR.ActionNotAllowed_StudyIsNearline;
				return false;
			}

			reason = String.Empty;
			return true;
		}

        public bool CanViewImages(out string reason)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Study.ViewImages))
            {
                reason = "You are not authorized to view images.";
                return false;
            }
            
            if (IsNearline)
            {
                reason = SR.ActionNotAllowed_StudyIsNearline;
                return false;
            }

            reason = String.Empty;
            return true;
        }

	    public bool CanScheduleRestore(out string reason)
	    {
		    if (IsNearline)
		    {
			    reason = String.Empty;
				return true;
			} 
			
			if (IsArchiving)
			{
				reason = SR.ActionNotAllowed_StudyIsBeingArchived;
				return false;
			}

			if (!IsArchived)
			{
				reason = SR.ActionNotAllowed_StudyIsNotArchived;
				return false;
			}
            
			if (IsLocked)
			{
				reason = SR.ActionNotAllowed_StudyIsLocked;
				return false;
			}
            
			reason = String.Empty;
			return true;
		}


	    public bool CanScheduleReconcile(out string reason)
		{
            if (IsLocked)
			{
				reason = SR.ActionNotAllowed_StudyIsLocked;
				return false;
			}

            if (IsProcessing)
            {
                reason = SR.ActionNotAllowed_StudyIsBeingProcessing;
				return false;
            }

			if (IsNearline)
			{
				reason = SR.ActionNotAllowed_StudyIsNearline;
				return false;
			}

			reason = String.Empty;
			return true;
		}

	    public bool CanReprocess(out string reason)
	    {
            if (IsNearline)
            {
                reason = SR.ActionNotAllowed_StudyIsNearline;
                return false;
            }
            
            
            if (HasPendingWorkQueueItems)
            {
                reason = SR.ActionNotAllowed_StudyHasPendingWorkQueue;
                return false;
            } 
            
            if (IsProcessing)
            {
                reason = SR.ActionNotAllowed_StudyIsBeingProcessing;
                return false;
            }

            if (IsArchivedLossless && IsOnlineLossy)
            {
                reason = SR.ActionNotAllowed_StudyIsLossyOnline;
                return false;
            }

            reason = String.Empty;
            return true;
	    }
	}

	/// <summary>
	/// Datasource for use with the ObjectDataSource to select a subset of results
	/// </summary>
	public class StudyDataSource
	{
		#region Public Delegates
		public delegate void StudyFoundSetDelegate(IList<StudySummary> list);

		public StudyFoundSetDelegate StudyFoundSet;
		#endregion

		#region Private Members
		private readonly StudyController _searchController = new StudyController();
		private IList<StudySummary> _list = new List<StudySummary>();
		private readonly string STUDYDATE_DATEFORMAT = "yyyyMMdd";

		#endregion

		#region Public Properties

		public string AccessionNumber { get; set; }

		public string PatientId { get; set; }

		public string PatientName { get; set; }

		public string StudyDescription { get; set; }

		public string ToStudyDate { get; set; }

		public string FromStudyDate { get; set; }

        public string ResponsiblePerson { get; set; }

        public string ResponsibleOrganization { get; set; }

		public ServerPartition Partition { get; set; }

		public string DateFormats { get; set; }

		public IList<StudySummary> List
		{
			get { return _list; }
		}

		public int ResultCount { get; set; }

		public string[] Modalities { get; set; }

        public string ReferringPhysiciansName { get; set; }

		public string[] Statuses { get; set; }

		public QCStatusEnum[] QCStatuses { get; set; }

		#endregion

		#region Private Methods
		private StudySelectCriteria GetSelectCriteria()
		{
			var criteria = new StudySelectCriteria();

			// only query for device in this partition
			criteria.ServerPartitionKey.EqualTo(Partition.Key);

            QueryHelper.SetGuiStringCondition(criteria.PatientId,PatientId);
            QueryHelper.SetGuiStringCondition(criteria.PatientsName, PatientName);

			criteria.PatientsName.SortAsc(0);

            QueryHelper.SetGuiStringCondition(criteria.AccessionNumber, AccessionNumber);
			
            if (!String.IsNullOrEmpty(ToStudyDate) && !String.IsNullOrEmpty(FromStudyDate))
			{
				string toKey = DateTime.ParseExact(ToStudyDate, DateFormats, null).ToString(STUDYDATE_DATEFORMAT, CultureInfo.InvariantCulture) + " 23:59:59.997";
				string fromKey = DateTime.ParseExact(FromStudyDate, DateFormats, null).ToString(STUDYDATE_DATEFORMAT, CultureInfo.InvariantCulture);
				criteria.StudyDate.Between(fromKey, toKey);
            }
            else if (!String.IsNullOrEmpty(ToStudyDate))
            {
				string toKey = DateTime.ParseExact(ToStudyDate, DateFormats, null).ToString(STUDYDATE_DATEFORMAT, CultureInfo.InvariantCulture);
                criteria.StudyDate.LessThanOrEqualTo(toKey);
            }
            else if (!String.IsNullOrEmpty(FromStudyDate))
            {
				string fromKey = DateTime.ParseExact(FromStudyDate, DateFormats, null).ToString(STUDYDATE_DATEFORMAT, CultureInfo.InvariantCulture);
                criteria.StudyDate.MoreThanOrEqualTo(fromKey);
            }

            QueryHelper.SetGuiStringCondition(criteria.StudyDescription, StudyDescription);
            QueryHelper.SetGuiStringCondition(criteria.ReferringPhysiciansName, ReferringPhysiciansName);
            QueryHelper.SetGuiStringCondition(criteria.ResponsiblePerson, ResponsiblePerson);
            QueryHelper.SetGuiStringCondition(criteria.ResponsibleOrganization, ResponsibleOrganization);

            if(Modalities != null && Modalities.Length > 0)
			{
				var seriesCriteria = new SeriesSelectCriteria();

                QueryHelper.SetStringArrayCondition(seriesCriteria.Modality, Modalities);

				criteria.SeriesRelatedEntityCondition.Exists(seriesCriteria);
			}

            if (Statuses != null && Statuses.Length > 0)
            {
                var storageCriteria = new StudyStorageSelectCriteria();
                if (Statuses.Length == 1)
                    storageCriteria.StudyStatusEnum.EqualTo(StudyStatusEnum.GetEnum(Statuses[0]));
                else
                {
                    var statusList = new List<StudyStatusEnum>();
                    foreach(string status in Statuses)
                    {
                        statusList.Add(StudyStatusEnum.GetEnum(status));
                    }

                    storageCriteria.StudyStatusEnum.In(statusList);
                }

                criteria.StudyStorageRelatedEntityCondition.Exists(storageCriteria);
            }


			if (QCStatuses != null && QCStatuses.Length > 0)
			{
				criteria.QCStatusEnum.In(QCStatuses);
			}

			return criteria;
		}

		#endregion

		#region Public Methods
		public IEnumerable<StudySummary> Select(int startRowIndex, int maximumRows)
		{
			if (maximumRows == 0 || Partition == null) return new List<StudySummary>();

			StudySelectCriteria criteria = GetSelectCriteria();

			IList<Study> studyList = _searchController.GetRangeStudies(criteria, startRowIndex, maximumRows);

			_list = new List<StudySummary>();

			foreach (Study study in studyList)
				_list.Add(StudySummaryAssembler.CreateStudySummary(HttpContext.Current.GetSharedPersistentContext(), study));

			if (StudyFoundSet != null)
				StudyFoundSet(_list);

			return _list;
		}

		public int SelectCount()
		{
			if (Partition == null) return 0;

			StudySelectCriteria criteria = GetSelectCriteria();

			ResultCount = _searchController.GetStudyCount(criteria);

			return ResultCount;
		}


		#endregion
	}

	public class StudySummaryAssembler
	{

		/// <summary>
		/// Returns an instance of <see cref="StudySummary"/> based on a <see cref="Study"/> object.
		/// </summary>
		/// <param name="study"></param>
		/// <param name="read"></param>
		/// <returns></returns>
		/// <remark>
		/// 
		/// </remark>
		static public StudySummary CreateStudySummary(IPersistenceContext read, Study study)
		{
            if (study==null)
			{
			    return null;
			}

			var studySummary = new StudySummary();
			var controller = new StudyController();

			studySummary.TheStudy = study;

			studySummary.Key = study.GetKey();
			studySummary.AccessionNumber = study.AccessionNumber;
			studySummary.NumberOfStudyRelatedInstances = study.NumberOfStudyRelatedInstances;
			studySummary.NumberOfStudyRelatedSeries = study.NumberOfStudyRelatedSeries;
			studySummary.PatientId = study.PatientId;
			studySummary.PatientsName = study.PatientsName;
			studySummary.StudyDate = study.StudyDate;
			studySummary.StudyInstanceUid = study.StudyInstanceUid;
			studySummary.StudyDescription = study.StudyDescription;
			studySummary.ModalitiesInStudy = controller.GetModalitiesInStudy(read, study);
		    studySummary.ReferringPhysiciansName = study.ReferringPhysiciansName;
		    studySummary.ResponsibleOrganization = study.ResponsibleOrganization;
		    studySummary.ResponsiblePerson = study.ResponsiblePerson;
			studySummary.StudyTime = study.StudyTime;
			studySummary.StudyId = study.StudyId;
			studySummary.HasOrder = study.OrderKey != null;

			if (study.OrderKey != null)
			{
				var order = Order.Load(study.OrderKey);
				studySummary.OrderRequiresQC = order.QCExpected;
				studySummary.StudyIsQCed = (study.QCStatusEnum != null && study.QCStatusEnum != QCStatusEnum.NA);
			}
			
			

			studySummary.ThePartition = ServerPartitionMonitor.Instance.FindPartition(study.ServerPartitionKey) ??
			                            ServerPartition.Load(read, study.ServerPartitionKey);

		    studySummary.ReferringPhysiciansName = study.ReferringPhysiciansName;
			studySummary.TheStudyStorage = StudyStorage.Load(read, study.StudyStorageKey);
			studySummary.StudyStatusEnum = studySummary.TheStudyStorage.StudyStatusEnum;
			studySummary.QueueStudyStateEnum = studySummary.TheStudyStorage.QueueStudyStateEnum;

			studySummary.TheArchiveLocation = controller.GetFirstArchiveStudyStorage(read, studySummary.TheStudyStorage.Key);

			studySummary.IsArchiving = controller.GetArchiveQueueCount(study) > 0;

			studySummary.IsProcessing = studySummary.TheStudyStorage.WriteLock;

			// the study is considered "locked" if it's being processed or some action which requires the lock has been scheduled
			// No additional action should be allowed on the study until everything is completed.
			studySummary.IsLocked = studySummary.IsProcessing ||
			                        (studySummary.TheStudyStorage.QueueStudyStateEnum != QueueStudyStateEnum.Idle);
           
    		if (controller.GetStudyIntegrityQueueCount(studySummary.TheStudyStorage.Key) > 0)
			{
				studySummary.IsReconcileRequired = true;
			}

		    studySummary.HasPendingExternalEdit = controller.GetCountPendingExternalEditWorkQueueItems(study) > 0;
            if (studySummary.HasPendingExternalEdit)            
                studySummary.HasPendingWorkQueueItems = true;            
            else
		        studySummary.HasPendingWorkQueueItems = controller.GetCountPendingWorkQueueItems(study) > 0;
            
            var ep = new StudySummaryAssemblerExtensionPoint();
            foreach (IStudySummaryAssembler assemblerPlugin in ep.CreateExtensions())
            {
                assemblerPlugin.PopulateStudy(studySummary, study);
            }

			return studySummary;
		}
	}
}