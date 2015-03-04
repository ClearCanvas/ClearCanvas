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
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using UpdateItem=ClearCanvas.ImageServer.Core.Edit.UpdateItem;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class StudyController : BaseController
    {
        #region Private Members

        private readonly StudyAdaptor _adaptor = new StudyAdaptor();
        private readonly SeriesSearchAdaptor _seriesAdaptor = new SeriesSearchAdaptor();
		private readonly PartitionArchiveAdaptor _partitionArchiveAdaptor = new PartitionArchiveAdaptor();
        #endregion
        #region Public Methods

        public IList<Study> GetStudies(StudySelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }
		public IList<Study> GetRangeStudies(StudySelectCriteria criteria, int startIndex, int maxRows)
		{
			return _adaptor.GetRange(criteria,startIndex,maxRows);
		}

		public int GetStudyCount(StudySelectCriteria criteria)
		{
			return _adaptor.GetCount(criteria);
		}

        public IList<Series> GetSeries(Study study)
        {
            SeriesSelectCriteria criteria = new SeriesSelectCriteria();

            criteria.StudyKey.EqualTo(study.Key);

            return _seriesAdaptor.Get(criteria);
        }

		public IList<StudyIntegrityQueue> GetStudyIntegrityQueueItems(ServerEntityKey studyStorageKey)
        {
			Platform.CheckForNullReference(studyStorageKey, "storageKey");


            IStudyIntegrityQueueEntityBroker integrityQueueBroker = HttpContext.Current.GetSharedPersistentContext().GetBroker<IStudyIntegrityQueueEntityBroker>();
            StudyIntegrityQueueSelectCriteria parms = new StudyIntegrityQueueSelectCriteria();

			parms.StudyStorageKey.EqualTo(studyStorageKey);

            return integrityQueueBroker.Find(parms);
        
        }

        public int GetStudyIntegrityQueueCount(ServerEntityKey studyStorageKey)
        {
            Platform.CheckForNullReference(studyStorageKey, "storageKey");


            IStudyIntegrityQueueEntityBroker integrityQueueBroker = HttpContext.Current.GetSharedPersistentContext().GetBroker<IStudyIntegrityQueueEntityBroker>();
            StudyIntegrityQueueSelectCriteria parms = new StudyIntegrityQueueSelectCriteria();

            parms.StudyStorageKey.EqualTo(studyStorageKey);

            return integrityQueueBroker.Count(parms);
        }

		/// <summary>
		/// Delete a Study.
		/// </summary>
		public void DeleteStudy(ServerEntityKey studyKey, string reason)
        {
			StudySummary study = StudySummaryAssembler.CreateStudySummary(HttpContext.Current.GetSharedPersistentContext(), Study.Load(HttpContext.Current.GetSharedPersistentContext(), studyKey));
            
			if (study.IsReconcileRequired)
			{
				throw new ApplicationException(
					String.Format("Deleting the study is not allowed at this time : there are items to be reconciled."));

				// NOTE: another check will occur when the delete is actually processed
			}

            using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                StudyDeleteHelper.DeleteStudy(ctx, study.ThePartition, study.StudyInstanceUid, reason);
                ctx.Commit();
            }
        }

        //TODO: Consolidate this and DeleteStudy?
        public void DeleteSeries(Study study, IList<Series> series, string reason)
        {
            // Load the Partition
            var partitionConfigController = new ServerPartitionConfigController();
            ServerPartition partition = partitionConfigController.GetPartition(study.ServerPartitionKey);

            var seriesUids = new List<string>();
            foreach (Series s in series)
            {
                seriesUids.Add(s.SeriesInstanceUid);
            }

            using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                StudyDeleteHelper.DeleteSeries(ctx, partition, study.StudyInstanceUid, seriesUids, reason);
                ctx.Commit();
            }
        }

		/// <summary>
		/// Restore a nearline study.
		/// </summary>
		/// <param name="study">The <see cref="Study"/> to restore.</param>
		/// <returns>true on success, false on failure.</returns>
		public bool RestoreStudy(Study study)
		{
			return _partitionArchiveAdaptor.RestoreStudy(study);
		}

        public bool MoveStudy(Study study, Device device)
        {
            return MoveStudy(study, device, null);
        }

        public bool MoveStudy(Study study, Device device, IList<Series> seriesList)
        {            
			if (seriesList != null)
			{
                using (
					IUpdateContext context = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
                    ServerPartition partition = ServerPartition.Load(study.ServerPartitionKey);

				    List<string> seriesUids = new List<string>();
                    foreach (Series series in seriesList)
					{
					    seriesUids.Add(series.SeriesInstanceUid);    
					}

                    IList<WorkQueue> entries = StudyEditorHelper.MoveSeries(context, partition, study.StudyInstanceUid, device.Key, seriesUids);
                        if(entries != null) context.Commit();

				    return true;
				}
			}
        	WorkQueueAdaptor workqueueAdaptor = new WorkQueueAdaptor();
			DateTime time = Platform.Time;
			WorkQueueUpdateColumns columns = new WorkQueueUpdateColumns
			                                 	{
			                                 		WorkQueueTypeEnum = WorkQueueTypeEnum.WebMoveStudy,
			                                 		WorkQueueStatusEnum = WorkQueueStatusEnum.Pending,
			                                 		ServerPartitionKey = study.ServerPartitionKey,
			                                 		StudyStorageKey = study.StudyStorageKey,
			                                 		FailureCount = 0,
			                                 		DeviceKey = device.Key,
			                                 		ScheduledTime = time,
			                                 		ExpirationTime = time.AddMinutes(4)
			                                 	};

        	workqueueAdaptor.Add(columns);

        	return true;
	    }

        public void EditStudy(Study study, List<UpdateItem> updateItems, string reason)
        {
            Platform.Log(LogLevel.Info, String.Format("Editing study {0}", study.StudyInstanceUid));

			using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
                IList<WorkQueue> entries = StudyEditorHelper.EditStudy(ctx, study.StudyStorageKey, updateItems, reason, ServerHelper.CurrentUserName, EditType.WebEdit);
                if (entries!=null)
			        ctx.Commit();
			}
        }

		public bool UpdateStudy(Study study, StudyUpdateColumns columns)
		{
			return _adaptor.Update(study.Key, columns);
		}

        public bool IsScheduledForEdit(Study study)
        {
            return IsStudyInWorkQueue(study, WorkQueueTypeEnum.WebEditStudy);
        }

        public bool IsScheduledForDelete(Study study)
        {
            return IsStudyInWorkQueue(study, WorkQueueTypeEnum.WebDeleteStudy);
        }

        public bool CanManipulateSeries(ServerEntityKey studyStorageKey)
        {
            StudyStorage storage = StudyStorage.Load(studyStorageKey);
            
            return storage.QueueStudyStateEnum.Equals(QueueStudyStateEnum.Idle);
        }

        /// <summary>
        /// Returns a value indicating whether the specified study has been scheduled for delete.
        /// </summary>
        /// <param name="study"></param>
        /// <param name="workQueueType"></param>
        /// <returns></returns>           
		private static bool IsStudyInWorkQueue(Study study, WorkQueueTypeEnum workQueueType)
        {
        	Platform.CheckForNullReference(study, "Study");

        	WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
        	WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
        	workQueueCriteria.WorkQueueTypeEnum.EqualTo(workQueueType);
        	workQueueCriteria.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
        	workQueueCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);

        	workQueueCriteria.WorkQueueStatusEnum.EqualTo(WorkQueueStatusEnum.Pending);

        	IList<WorkQueue> list = adaptor.Get(workQueueCriteria);
        	if (list != null && list.Count > 0)
        		return true;

        	workQueueCriteria.WorkQueueStatusEnum.EqualTo(WorkQueueStatusEnum.Idle); // not likely but who knows
        	list = adaptor.Get(workQueueCriteria);
        	if (list != null && list.Count > 0)
        		return true;

        	return false;
        }

    	/// <summary>
		/// Returns a value indicating whether the specified study has been scheduled for delete.
		/// </summary>
		/// <param name="study"></param>
		/// <param name="read"></param>
		/// <returns></returns>
		public string GetModalitiesInStudy(IPersistenceContext read, Study study)
		{
			Platform.CheckForNullReference(study, "Study");

            IQueryModalitiesInStudy select = read.GetBroker<IQueryModalitiesInStudy>();
            ModalitiesInStudyQueryParameters parms = new ModalitiesInStudyQueryParameters { StudyKey = study.Key };
            IList<Series> seriesList = select.Find(parms);
			List<string> modalities = new List<string>();
			
			foreach (Series series in seriesList)
			{
				bool found = false;
				foreach (string modality in modalities)
					if (modality.Equals(series.Modality))
					{
						found = true;
						break;
					}
				if (!found)
					modalities.Add(series.Modality);
			}

			string modalitiesInStudy = "";
			foreach (string modality in modalities)
				if (modalitiesInStudy.Length == 0)
					modalitiesInStudy = modality;
				else
					modalitiesInStudy += "\\" + modality;

			return modalitiesInStudy;
		}

        public IList<WorkQueue> GetWorkQueueItems(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
            WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
			workQueueCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);
            workQueueCriteria.ScheduledTime.SortAsc(0);
            return adaptor.Get(workQueueCriteria);
        }

        public int GetCountPendingWorkQueueItems(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            var adaptor = new WorkQueueAdaptor();
            var workQueueCriteria = new WorkQueueSelectCriteria();
            workQueueCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);
            workQueueCriteria.WorkQueueStatusEnum.In(new [] {WorkQueueStatusEnum.Idle, WorkQueueStatusEnum.InProgress, WorkQueueStatusEnum.Pending});
            return adaptor.GetCount(workQueueCriteria);
        }

        public int GetCountPendingExternalEditWorkQueueItems(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            var adaptor = new WorkQueueAdaptor();
            var workQueueCriteria = new WorkQueueSelectCriteria();
            workQueueCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);
            workQueueCriteria.WorkQueueTypeEnum.EqualTo(WorkQueueTypeEnum.ExternalEdit);
            return adaptor.GetCount(workQueueCriteria);
        }

        public IList<FilesystemQueue> GetFileSystemQueueItems(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            FileSystemQueueAdaptor adaptor = new FileSystemQueueAdaptor();
            FilesystemQueueSelectCriteria fileSystemQueueCriteria = new FilesystemQueueSelectCriteria();
			fileSystemQueueCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);
            fileSystemQueueCriteria.ScheduledTime.SortAsc(0);
            return adaptor.Get(fileSystemQueueCriteria);
        }

        public IList<ArchiveQueue> GetArchiveQueueItems(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            ArchiveQueueAdaptor adaptor = new ArchiveQueueAdaptor();
            ArchiveQueueSelectCriteria archiveQueueCriteria = new ArchiveQueueSelectCriteria();
            archiveQueueCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);
			archiveQueueCriteria.ScheduledTime.SortDesc(0);
            return adaptor.Get(archiveQueueCriteria);
        }

		public int GetArchiveQueueCount(Study study)
		{
			Platform.CheckForNullReference(study, "Study");

			ArchiveQueueAdaptor adaptor = new ArchiveQueueAdaptor();
			ArchiveQueueSelectCriteria archiveQueueCriteria = new ArchiveQueueSelectCriteria();
			archiveQueueCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);
			archiveQueueCriteria.ScheduledTime.SortDesc(0);
			return adaptor.GetCount(archiveQueueCriteria);
		}

        public IList<ArchiveStudyStorage> GetArchiveStudyStorage(Study study)
        {
        	Platform.CheckForNullReference(study, "Study");

        	ArchiveStudyStorageAdaptor adaptor = new ArchiveStudyStorageAdaptor();
        	ArchiveStudyStorageSelectCriteria archiveStudyStorageCriteria = new ArchiveStudyStorageSelectCriteria();
        	archiveStudyStorageCriteria.StudyStorageKey.EqualTo(study.StudyStorageKey);
        	archiveStudyStorageCriteria.ArchiveTime.SortDesc(0);

        	return adaptor.Get(archiveStudyStorageCriteria);
        }

		public IList<ArchiveStudyStorage> GetArchiveStudyStorage(IPersistenceContext read, ServerEntityKey studyStorageKey)
        {
            Platform.CheckForNullReference(studyStorageKey, "studyStorageKey");

            ArchiveStudyStorageAdaptor adaptor = new ArchiveStudyStorageAdaptor();
            ArchiveStudyStorageSelectCriteria archiveStudyStorageCriteria = new ArchiveStudyStorageSelectCriteria();
            archiveStudyStorageCriteria.StudyStorageKey.EqualTo(studyStorageKey);
        	archiveStudyStorageCriteria.ArchiveTime.SortDesc(0);

            return adaptor.Get(read, archiveStudyStorageCriteria);
        }
        public ArchiveStudyStorage GetFirstArchiveStudyStorage(IPersistenceContext read, ServerEntityKey studyStorageKey)
        {
            Platform.CheckForNullReference(studyStorageKey, "studyStorageKey");

            ArchiveStudyStorageAdaptor adaptor = new ArchiveStudyStorageAdaptor();
            ArchiveStudyStorageSelectCriteria archiveStudyStorageCriteria = new ArchiveStudyStorageSelectCriteria();
            archiveStudyStorageCriteria.StudyStorageKey.EqualTo(studyStorageKey);
            archiveStudyStorageCriteria.ArchiveTime.SortDesc(0);

            return adaptor.GetFirst(read, archiveStudyStorageCriteria);
        }

        public IList<StudyStorageLocation> GetStudyStorageLocation(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            
            IQueryStudyStorageLocation select = HttpContext.Current.GetSharedPersistentContext().GetBroker<IQueryStudyStorageLocation>();
            StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters
                                                        	{StudyStorageKey = study.StudyStorageKey};

        	IList<StudyStorageLocation> storage = select.Find(parms);

            if (storage == null)
			{
				storage = new List<StudyStorageLocation>();
			    Platform.Log(LogLevel.Warn, "Unable to find storage location for Study item: {0}",
                             study.GetKey().ToString());
            }

            if (storage.Count > 1)
            {
                Platform.Log(LogLevel.Warn,
                             "StudyController:GetStudyStorageLocation: multiple study storage found for study {0}",
                             study.GetKey().Key);
            }

            return storage;        
        }

		public StudyStorage GetStudyStorage(Study study)
		{
			Platform.CheckForNullReference(study, "Study");

			return StudyStorage.Load(study.StudyStorageKey);
		}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="key"></param>
        /// <exception cref="InvalidStudyStateOperationException">Study is in a state that reprocessing is not allowed</exception>
        public void ReprocessStudy(String reason, ServerEntityKey key)
        {
            StudyStorageAdaptor adaptor = new StudyStorageAdaptor();
            StudyStorage storage = adaptor.Get(key);
            StudyStorageLocation storageLocation = StudyStorageLocation.FindStorageLocations(storage)[0];
            StudyReprocessor reprocessor = new StudyReprocessor();
            reprocessor.ReprocessStudy(reason, storageLocation, Platform.Time);
        }

        #endregion      
    }
}
