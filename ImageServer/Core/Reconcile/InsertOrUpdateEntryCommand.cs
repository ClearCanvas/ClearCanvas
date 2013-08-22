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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	public class InsertOrUpdateEntryCommand:ServerDatabaseCommand
	{
        private readonly string _groupId;
        private readonly DicomFile _file;
		private readonly StudyStorageLocation _studyLocation;
	    private readonly string _relativePath;
        private readonly List<DicomAttributeComparisonResult> _reasons;
	    private readonly string _duplicateStoragePath;

	    public InsertOrUpdateEntryCommand(String groupId,
		                                        StudyStorageLocation studyLocation, 
                                                DicomFile file, String duplicateStoragePath, String relativePath,
                                                 List<DicomAttributeComparisonResult> reasons) 
			: base("Insert Duplicate Queue Entry Command")
		{
            Platform.CheckForNullReference(groupId, "groupId");
			Platform.CheckForNullReference(studyLocation, "studyLocation");
			Platform.CheckForNullReference(file, "file");

			_file = file;
			_studyLocation = studyLocation;
            _groupId = groupId;
	        _duplicateStoragePath = duplicateStoragePath;
		    _relativePath = relativePath;
	        _reasons = reasons;
		}

		protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
		{
			var broker = updateContext.GetBroker<IInsertDuplicateSopReceivedQueue>();
			var parms = new InsertDuplicateSopReceivedQueueParameters
                      	{
                      		GroupID = _groupId,
                      		ServerPartitionKey = _studyLocation.ServerPartitionKey,
                      		StudyStorageKey = _studyLocation.Key,
                      		StudyInstanceUid = _file.DataSet[DicomTags.StudyInstanceUid].ToString(),
                      		SeriesDescription = _file.DataSet[DicomTags.SeriesDescription].ToString(),
                      		SeriesInstanceUid = _file.DataSet[DicomTags.SeriesInstanceUid].ToString(),
                      		SopInstanceUid = _file.MediaStorageSopInstanceUid
                      	};
			ReconcileStudyQueueDescription queueDesc = CreateQueueEntryDescription(_file);
		    parms.Description = queueDesc != null ? queueDesc.ToString() : String.Empty;
		    var queueData = new DuplicateSIQQueueData
		                        {
		                            StoragePath = _duplicateStoragePath,
		                            Details = new ImageSetDetails(_file.DataSet),
		                            TimeStamp = Platform.Time
		                        };
			if (_reasons != null && _reasons.Count>0)
            {
                queueData.ComparisonResults = _reasons;
            }
            
			var imageSet = new ImageSetDescriptor(_file.DataSet);
			parms.StudyData = XmlUtils.SerializeAsXmlDoc(imageSet);
            parms.Details = XmlUtils.SerializeAsXmlDoc(queueData);
		    parms.UidRelativePath = _relativePath;
			IList<DuplicateSopReceivedQueue> entries = broker.Find(parms);

			Platform.CheckForNullReference(entries, "entries");
			Platform.CheckTrue(entries.Count == 1, "entries.Count==1");

			DuplicateSopReceivedQueue queueEntry = entries[0];

			var data = XmlUtils.Deserialize<DuplicateSIQQueueData>(queueEntry.Details);
			data.Details.InsertFile(_file);

			queueEntry.Details = XmlUtils.SerializeAsXmlDoc(data);

			var siqBroker = updateContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
			if (!siqBroker.Update(queueEntry))
				throw new ApplicationException("Unable to update duplicate queue entry");
		}

        private ReconcileStudyQueueDescription CreateQueueEntryDescription(DicomFile file)
	    {
	        using(var context = new ServerExecutionContext())
	        {
	            Study study = _studyLocation.LoadStudy(context.PersistenceContext);
                if (study!=null)
                {
                    var desc = new ReconcileStudyQueueDescription
                                   {
                                       ExistingPatientId = study.PatientId,
                                       ExistingPatientName = study.PatientsName,
                                       ExistingAccessionNumber = study.AccessionNumber,
                                       ConflictingPatientName = file.DataSet[DicomTags.PatientsName].ToString(),
                                       ConflictingPatientId = file.DataSet[DicomTags.PatientId].ToString(),
                                       ConflictingAccessionNumber = file.DataSet[DicomTags.AccessionNumber].ToString()
                                   };


                    return desc;
                }

	        	return null;
	        }       
        }
	}
}