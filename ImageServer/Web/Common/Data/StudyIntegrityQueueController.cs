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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	public class StudyIntegrityQueueController
	{
        private readonly StudyIntegrityQueueAdaptor _adaptor = new StudyIntegrityQueueAdaptor();

        public bool CanReconcile(StudyStorageLocation location, out string reason)
        {
            return location.CanUpdate(out reason);
        }

		public IList<StudyIntegrityQueue> GetStudyIntegrityQueueItems(StudyIntegrityQueueSelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }

        public IList<StudyIntegrityQueue> GetRangeStudyIntegrityQueueItems(StudyIntegrityQueueSelectCriteria criteria, int startIndex, int maxRows)
        {
            return _adaptor.GetRange(criteria, startIndex, maxRows);
        }

        public int GetReconcileQueueItemsCount(StudyIntegrityQueueSelectCriteria criteria)
        {
            return _adaptor.GetCount(criteria);
        }

        public bool DeleteStudyIntegrityQueueItem(StudyIntegrityQueue item)
        {
            return _adaptor.Delete(item.Key);
        }

        private static void ReconcileStudy(string command,StudyIntegrityQueue item )
        {
            //Ignore the reconcile command if the item is null.
            if (item == null) return;

			// Preload the change description so its not done during the DB transaction
			XmlDocument changeDescription = new XmlDocument();
			changeDescription.LoadXml(command);

			// The Xml in the SIQ item was generated when the images were received and put into the SIQ.
			// We now add the user info to it so that it will be logged in the history
            ReconcileStudyWorkQueueData queueData = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(item.Details);
            queueData.TimeStamp = Platform.Time;
            queueData.UserId = ServerHelper.CurrentUserName;

			using (IUpdateContext context = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
                
                LockStudyParameters lockParms = new LockStudyParameters
                                                	{
                                                		QueueStudyStateEnum = QueueStudyStateEnum.ReconcileScheduled,
                                                		StudyStorageKey = item.StudyStorageKey
                                                	};
				ILockStudy broker = context.GetBroker<ILockStudy>();
                broker.Execute(lockParms);
                if (!lockParms.Successful)
                {
                    throw new ApplicationException(lockParms.FailureReason);
                }

                
				//Add to Study History
				StudyHistoryeAdaptor historyAdaptor = new StudyHistoryeAdaptor();
				StudyHistoryUpdateColumns parameters = new StudyHistoryUpdateColumns
				                                       	{
				                                       		StudyData = item.StudyData,
				                                       		ChangeDescription = changeDescription,
				                                       		StudyStorageKey = item.StudyStorageKey,
				                                       		StudyHistoryTypeEnum = StudyHistoryTypeEnum.StudyReconciled
				                                       	};

				StudyHistory history = historyAdaptor.Add(context, parameters);

				//Create WorkQueue Entry
				WorkQueueAdaptor workQueueAdaptor = new WorkQueueAdaptor();
				WorkQueueUpdateColumns row = new WorkQueueUpdateColumns
				                             	{
				                             		Data = XmlUtils.SerializeAsXmlDoc(queueData),
				                             		ServerPartitionKey = item.ServerPartitionKey,
				                             		StudyStorageKey = item.StudyStorageKey,
				                             		StudyHistoryKey = history.GetKey(),
				                             		WorkQueueTypeEnum = WorkQueueTypeEnum.ReconcileStudy,
				                             		WorkQueueStatusEnum = WorkQueueStatusEnum.Pending,
				                             		ScheduledTime = Platform.Time,
				                             		ExpirationTime = Platform.Time.AddHours(1),
                                                    GroupID = item.GroupID
				                             	};
				WorkQueue newWorkQueueItem = workQueueAdaptor.Add(context, row);

				StudyIntegrityQueueUidAdaptor studyIntegrityQueueUidAdaptor = new StudyIntegrityQueueUidAdaptor();
				StudyIntegrityQueueUidSelectCriteria crit = new StudyIntegrityQueueUidSelectCriteria();
				crit.StudyIntegrityQueueKey.EqualTo(item.GetKey());
				IList<StudyIntegrityQueueUid> uidList = studyIntegrityQueueUidAdaptor.Get(context, crit);

				WorkQueueUidAdaptor workQueueUidAdaptor = new WorkQueueUidAdaptor();
				WorkQueueUidUpdateColumns update = new WorkQueueUidUpdateColumns();
				foreach (StudyIntegrityQueueUid uid in uidList)
				{
					update.WorkQueueKey = newWorkQueueItem.GetKey();
					update.SeriesInstanceUid = uid.SeriesInstanceUid;
					update.SopInstanceUid = uid.SopInstanceUid;
				    update.RelativePath = uid.RelativePath;
					workQueueUidAdaptor.Add(context, update);
				}

				//DeleteStudyIntegrityQueue Item
				StudyIntegrityQueueUidSelectCriteria criteria = new StudyIntegrityQueueUidSelectCriteria();
				criteria.StudyIntegrityQueueKey.EqualTo(item.GetKey());
				studyIntegrityQueueUidAdaptor.Delete(context, criteria);

				StudyIntegrityQueueAdaptor studyIntegrityQueueAdaptor = new StudyIntegrityQueueAdaptor();
				studyIntegrityQueueAdaptor.Delete(context, item.GetKey());

				context.Commit();
			}

		}

	    public void CreateNewStudy(ServerEntityKey itemKey)
	    {
            InconsistentDataSIQRecord record = new InconsistentDataSIQRecord(StudyIntegrityQueue.Load(itemKey));
            ReconcileCreateStudyDescriptor command = new ReconcileCreateStudyDescriptor
                                                     	{
                                                     		Automatic = false,
                                                     		UserName = ServerHelper.CurrentUserName,
                                                     		ExistingStudy = record.ExistingStudyInfo,
                                                     		ImageSetData = record.ConflictingImageDescriptor
                                                     	};
	    	command.Commands.Add(new SetTagCommand(DicomTags.StudyInstanceUid, record.ExistingStudyInfo.StudyInstanceUid, DicomUid.GenerateUid().UID));
            String xml = XmlUtils.SerializeAsString(command);
            ReconcileStudy(xml, record.QueueItem);
	    }

	    public void MergeStudy(ServerEntityKey itemKey, Boolean useExistingStudy)
        {
            InconsistentDataSIQRecord record = new InconsistentDataSIQRecord(StudyIntegrityQueue.Load(itemKey));
            ReconcileMergeToExistingStudyDescriptor command = new ReconcileMergeToExistingStudyDescriptor
                                                              	{
                                                              		UserName = ServerHelper.CurrentUserName,
                                                              		Automatic = false,
                                                              		ExistingStudy = record.ExistingStudyInfo,
                                                              		ImageSetData = record.ConflictingImageDescriptor
                                                              	};

	    	if(useExistingStudy)
            {
                command.Description = "Merge using existing study information.";
                String xml = XmlUtils.SerializeAsString(command);
                ReconcileStudy(xml, record.QueueItem);    
            }
            else
            {
                command.Description = "Using study information from the conflicting images.";

                if (record.ConflictingImageDetails!=null)
                {
                    // The conflicting study data is stored in Details column
                    string newPatientName = record.ConflictingImageDetails.StudyInfo.PatientInfo.Name;
                    if (!String.IsNullOrEmpty(newPatientName))
                    {
                        string acceptableName = PatientNameRules.GetAcceptableName(newPatientName);
                        if (!acceptableName.Equals(newPatientName))
                        {
                            //override the value
                            newPatientName = acceptableName;
                        }
                    }
                    command.Commands.Add(new SetTagCommand(DicomTags.PatientsName, record.ExistingStudyInfo.PatientInfo.Name, newPatientName));
                    command.Commands.Add(new SetTagCommand(DicomTags.PatientId, record.ExistingStudyInfo.PatientInfo.PatientId, record.ConflictingImageDetails.StudyInfo.PatientInfo.PatientId));
                    command.Commands.Add(new SetTagCommand(DicomTags.PatientsBirthDate, record.ExistingStudyInfo.PatientInfo.PatientsBirthdate, record.ConflictingImageDetails.StudyInfo.PatientInfo.PatientsBirthdate));
                    command.Commands.Add(new SetTagCommand(DicomTags.PatientsSex, record.ExistingStudyInfo.PatientInfo.Sex, record.ConflictingImageDetails.StudyInfo.PatientInfo.Sex));
                    command.Commands.Add(new SetTagCommand(DicomTags.IssuerOfPatientId, record.ExistingStudyInfo.PatientInfo.IssuerOfPatientId, record.ConflictingImageDetails.StudyInfo.PatientInfo.IssuerOfPatientId));
                    command.Commands.Add(new SetTagCommand(DicomTags.AccessionNumber, record.ExistingStudyInfo.AccessionNumber, record.ConflictingImageDetails.StudyInfo.AccessionNumber));
    
                }
                else
                {
                	// The conflicting study data is stored in StudyData column
                    String patientName = record.ConflictingImageDescriptor[DicomTags.PatientsName] != null
                                             ? record.ConflictingImageDescriptor[DicomTags.PatientsName].Value
                                             : null;

                    if (!String.IsNullOrEmpty(patientName))
                    {
                        string acceptableName = PatientNameRules.GetAcceptableName(patientName);
                        if (!acceptableName.Equals(patientName))
                        {
                            //override the value
                            patientName = acceptableName;
                        }
                    }
                    
                    if (patientName != null)
                        command.Commands.Add(new SetTagCommand(DicomTags.PatientsName, record.ExistingStudyInfo.PatientInfo.Name, patientName));

                    String patientId = record.ConflictingImageDescriptor[DicomTags.PatientId] != null
                                             ? record.ConflictingImageDescriptor[DicomTags.PatientId].Value
                                             : null;
                    if (patientId != null)
                        command.Commands.Add(new SetTagCommand(DicomTags.PatientId,record.ExistingStudyInfo.PatientInfo.PatientId, patientId));

                    String patientsBirthDate = record.ConflictingImageDescriptor[DicomTags.PatientsBirthDate] != null
                                             ? record.ConflictingImageDescriptor[DicomTags.PatientsBirthDate].Value
                                             : null;
                    if (patientsBirthDate != null)
                        command.Commands.Add(new SetTagCommand(DicomTags.PatientsBirthDate, record.ExistingStudyInfo.PatientInfo.PatientsBirthdate, patientsBirthDate));

                    String patientsSex = record.ConflictingImageDescriptor[DicomTags.PatientsSex] != null
                                             ? record.ConflictingImageDescriptor[DicomTags.PatientsSex].Value
                                             : null;
                    if (patientsSex != null)
                        command.Commands.Add(new SetTagCommand(DicomTags.PatientsSex, record.ExistingStudyInfo.PatientInfo.Sex, patientsSex));

                    String issuerOfPatientId = record.ConflictingImageDescriptor[DicomTags.IssuerOfPatientId] != null
                                             ? record.ConflictingImageDescriptor[DicomTags.IssuerOfPatientId].Value
                                             : null;
                    if (issuerOfPatientId != null)
                        command.Commands.Add(new SetTagCommand(DicomTags.IssuerOfPatientId, record.ExistingStudyInfo.PatientInfo.IssuerOfPatientId, issuerOfPatientId));

                    String accessionNumber = record.ConflictingImageDescriptor[DicomTags.AccessionNumber] != null
                                             ? record.ConflictingImageDescriptor[DicomTags.AccessionNumber].Value
                                             : null;
                    if (accessionNumber != null)
                        command.Commands.Add(new SetTagCommand(DicomTags.AccessionNumber, record.ExistingStudyInfo.AccessionNumber, accessionNumber));

                }
                
                String xml = XmlUtils.SerializeAsString(command);
                ReconcileStudy(xml, record.QueueItem); 
            }
        }

        public void Discard(ServerEntityKey itemKey)
        {
            ReconcileDiscardImagesDescriptor command = new ReconcileDiscardImagesDescriptor();
            InconsistentDataSIQRecord record = new InconsistentDataSIQRecord(StudyIntegrityQueue.Load(itemKey));
            command.UserName = ServerHelper.CurrentUserName;
            
            command.Automatic = false;
            command.ExistingStudy = record.ExistingStudyInfo;
            command.ImageSetData = record.ConflictingImageDescriptor;
            String xml = XmlUtils.SerializeAsString(command);
            ReconcileStudy(xml, record.QueueItem);
        }


        public void IgnoreDifferences(ServerEntityKey key)
        {
            ReconcileProcessAsIsDescriptor command = new ReconcileProcessAsIsDescriptor();
            InconsistentDataSIQRecord record = new InconsistentDataSIQRecord(StudyIntegrityQueue.Load(key));
            command.UserName = ServerHelper.CurrentUserName;
            command.Automatic = false;
            command.Description = "Ignore the differences";
            command.ExistingStudy = record.ExistingStudyInfo;
            command.ImageSetData = record.ConflictingImageDescriptor;
            
            String xml = XmlUtils.SerializeAsString(command);
            ReconcileStudy(xml, record.QueueItem);
        }

	}

    internal class InconsistentDataSIQRecord
    {
        private readonly ImageSetDescriptor _conflictingImageDescriptor;
        private readonly ImageSetDetails _conflictingImageDetails;
        private readonly StudyInformation _existingStudyInfo;
        private readonly StudyIntegrityQueue _queueItem;

        public InconsistentDataSIQRecord(StudyIntegrityQueue queue)
        {
            _queueItem = queue;
            ReconcileStudyWorkQueueData data = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(queue.Details);
            _conflictingImageDetails = data.Details;
            _conflictingImageDescriptor = XmlUtils.Deserialize<ImageSetDescriptor>(queue.StudyData);
			StudyStorage storage = StudyStorage.Load(HttpContext.Current.GetSharedPersistentContext(), queue.StudyStorageKey);
			Study study = storage.LoadStudy(HttpContext.Current.GetSharedPersistentContext());
            _existingStudyInfo = new StudyInformation(new ServerEntityAttributeProvider(study));
        }

        public StudyInformation ExistingStudyInfo
        {
            get { return _existingStudyInfo; }
        }

        public ImageSetDescriptor ConflictingImageDescriptor
        {
            get { return _conflictingImageDescriptor; }
        }

        public StudyIntegrityQueue QueueItem
        {
            get { return _queueItem; }
        }

        public ImageSetDetails ConflictingImageDetails
        {
            get { return _conflictingImageDetails; }
        }
    }
}
