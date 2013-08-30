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
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using SaveDicomFileCommand = ClearCanvas.ImageServer.Core.Command.SaveDicomFileCommand;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
    /// <summary>
    /// A helper class to resolve the location of the images that needs to be reconciled.
    /// Used by <see cref="ImageReconciler"/>.
    /// </summary>
    class ReconcileStorage
    {
        #region Private Members
        readonly private string _folder;
        private readonly StudyStorageLocation _studyLocation;
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ReconcileStorage"/>
        /// </summary>
        /// <param name="studyLocation">The <see cref="StudyStorageLocation"/> of the study which contains the images to be reconciled.</param>
        /// <param name="folder">The name of the folder used for storing the images to be reconciled.</param>
        public ReconcileStorage(StudyStorageLocation studyLocation, string folder)
        {
            Platform.CheckForNullReference(studyLocation, "studyLocation");
            Platform.CheckForEmptyString(folder, "folder");

            _studyLocation = studyLocation;
            _folder = folder;
        }
        
        #endregion

		#region Public Methods

        /// <summary>
        /// Gets the path to the folder that contains the images to be reconciled.
        /// </summary>
        /// <returns></returns>
        public string GetFolderPath()
        {
            string path = Path.Combine(_studyLocation.FilesystemPath, _studyLocation.PartitionFolder);
			path = Path.Combine(path, ServerPlatform.ReconcileStorageFolder);
            path = Path.Combine(path, _folder);
            return Path.Combine(path, _studyLocation.StudyInstanceUid);
        }
        
        /// <summary>
        /// Gets the path to the images, relative to the path returned by <see cref="GetFolderPath"/>
        /// </summary>
        /// <param name="sopUid"></param>
        /// <returns></returns>
        public string GetSopRelativePath(string sopUid)
        {
            return sopUid  + ServerPlatform.DicomFileExtension;
        }

        
        /// <summary>
        /// Gets the path to the image that needs to be reconciled.
        /// </summary>
        /// <param name="sopUid"></param>
        /// <returns></returns>
        public string GetSopInstancePath(string sopUid)
        {
            string path = Path.Combine(GetFolderPath(), GetSopRelativePath(sopUid));
            return path;
		}

		#endregion
	}

    /// <summary>
    /// Helper class to insert a <see cref="StudyIntegrityQueue"/> for manual image reconciliation.
    /// </summary>
	class ImageReconciler
	{
		#region Private Members
        private readonly SopInstanceProcessorContext _context;
	    #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ImageReconciler"/>
        /// </summary>
        /// <param name="context"></param>
        public ImageReconciler(SopInstanceProcessorContext context)
        {
            _context = context;
        }
        
        #endregion


		#region Public Methods
        /// <summary>
        /// Inserts a <see cref="StudyIntegrityQueue"/> entry for manual reconciliation.
        /// </summary>
        /// <param name="file">The DICOM file that needs to be reconciled.</param>
        /// <param name="reason">The type of <see cref="StudyIntegrityQueue"/> entry to be inserted.</param>
        /// <param name="uid">A UID to delete on insert.</param>
        /// <remarks>
        /// A copy of the DICOM file will be stored in a special folder allocated for 
        /// reconciliation purpose. The caller is responsible for managing the original copy.
        /// </remarks>
		public void ScheduleReconcile(DicomFile file, StudyIntegrityReasonEnum reason, WorkQueueUid uid)
		{
            Platform.CheckForNullReference(_context.StudyLocation, "_context.StudyLocation");
          
            Platform.Log(LogLevel.Info, "Scheduling new manual reconciliation for SOP {0}", file.MediaStorageSopInstanceUid);
            ServerFilesystemInfo fs = FilesystemMonitor.Instance.GetFilesystemInfo(_context.StudyLocation.FilesystemKey);
            Platform.CheckForNullReference(fs, "fs");
            
            ReconcileStorage reconcileStorage = new ReconcileStorage(_context.StudyLocation, _context.Group); 

            using(ServerCommandProcessor processor = new ServerCommandProcessor("Schedule Manual Reconciliation"))
            {
            	string path = reconcileStorage.GetSopInstancePath(file.DataSet[DicomTags.SopInstanceUid].ToString());
                DirectoryInfo dir = new DirectoryInfo(path);
				if (dir.Parent != null)
				{
					CreateDirectoryCommand mkdir = new CreateDirectoryCommand(dir.Parent.FullName);
					processor.AddCommand(mkdir);
				}

            	SaveDicomFileCommand saveFileCommand = new SaveDicomFileCommand(path, file, true);
                processor.AddCommand(saveFileCommand);

                InsertSIQCommand updateStudyCommand = new InsertSIQCommand(_context.StudyLocation, reason, file, _context.Group, reconcileStorage);
                processor.AddCommand(updateStudyCommand);

				if (uid != null)
					processor.AddCommand(new DeleteWorkQueueUidCommand(uid));

                if (processor.Execute() == false)
                {
                    throw new ApplicationException(String.Format("Unable to schedule image reconcilation : {0}", processor.FailureReason), processor.FailureException);
                }
            }
		}

		#endregion
	}

    /// <summary>
    /// Command to insert a <see cref="StudyIntegrityQueue"/> entry.
    /// </summary>
    internal class InsertSIQCommand : ServerDatabaseCommand
    {
        #region Private Members
        private readonly StudyStorageLocation _storageLocation;
        private readonly StudyIntegrityReasonEnum _reason;
        private readonly DicomFile _file;
        private readonly ReconcileStorage _reconcileImageStorage;

        private StudyIntegrityQueue _siqItem;
        private readonly string _uidGroup; 
        #endregion

        /// <summary>
        /// Creates an instance of <see cref="InsertSIQCommand"/> to insert a <see cref="StudyIntegrityQueue"/> entry
        /// for a <see cref="DicomFile"/>
        /// </summary>
        /// <param name="studyStorage">The <see cref="StudyStorageLocation"/> of the study</param>
        /// <param name="reason">A <see cref="StudyIntegrityReasonEnum"/> value indicating the type of <see cref="StudyIntegrityQueue"/> entry to be inserted.</param>
        /// <param name="file">The DICOM file that needs to be reconciled</param>
        /// <param name="uidGroup">The string value representing the group of images which the DICOM <paramref name="file"/> belongs to</param>
        /// <param name="reconcileImageStorage">The <see cref="ReconcileStorage"/> where the DICOM file will be stored.</param>
        public InsertSIQCommand(
            StudyStorageLocation studyStorage, 
            StudyIntegrityReasonEnum reason, 
            DicomFile file,
            string uidGroup,
            ReconcileStorage reconcileImageStorage
            )
            :base("Insert SIQ Command")
        {
            _storageLocation = studyStorage;
            _reconcileImageStorage = reconcileImageStorage;
            _reason = reason;
            _file = file;
            _uidGroup = uidGroup;
        }

        /// <summary>
        /// Gets the <see cref="StudyIntegrityQueue"/> inserted by this command.
        /// </summary>
        public StudyIntegrityQueue SIQEntry
        {
            get { return _siqItem; }
        }

        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
        {
            string seriesUid = _file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
            string sopUid = _file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
            ImageSetDescriptor imageSet = new ImageSetDescriptor(_file.DataSet);
            ReconcileStudyQueueDescription queueDesc = GetQueueEntryDescription(_storageLocation, _file);
			ReconcileStudyWorkQueueData detailsData = new ReconcileStudyWorkQueueData
			{
				StoragePath = _reconcileImageStorage.GetFolderPath(),
				Details = new ImageSetDetails(_file.DataSet)
			};
			detailsData.Details.InsertFile(_file);
			
            IInsertStudyIntegrityQueue broker = updateContext.GetBroker<IInsertStudyIntegrityQueue>();
            InsertStudyIntegrityQueueParameters parameters = new InsertStudyIntegrityQueueParameters
                         	{
                         		Description = queueDesc.ToString(),
                         		StudyInstanceUid = _storageLocation.StudyInstanceUid,
                         		ServerPartitionKey = _storageLocation.ServerPartition.Key,
                         		StudyStorageKey = _storageLocation.Key,
                         		StudyIntegrityReasonEnum = _reason,
                         		SeriesInstanceUid = seriesUid,
                         		SeriesDescription = sopUid,
                         		SopInstanceUid =
                         			_file.DataSet[DicomTags.SopInstanceUid].GetString(0,
                         			                                                  String
                         			                                                  	.
                         			                                                  	Empty),
                         		StudyData = XmlUtils.SerializeAsXmlDoc(imageSet),
                         		Details = XmlUtils.SerializeAsXmlDoc(detailsData),
                         		GroupID = _uidGroup,
                         		UidRelativePath =
                         			_reconcileImageStorage.GetSopRelativePath(
                         			_file.DataSet[DicomTags.SopInstanceUid].ToString())
                         	};

        	StudyIntegrityQueue item = broker.FindOne(parameters);
            if (item == null)
            {
                throw new ApplicationException("Unable to update reconcile queue");
            }

            _siqItem = item;
            if (!parameters.Inserted)
            {
                // Need to re-use the path that's already assigned for this entry
                ReconcileStudyWorkQueueData data = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(item.Details);
                data.Details.InsertFile(_file);

                XmlDocument updatedQueueDataXml = XmlUtils.SerializeAsXmlDoc(data);
                IStudyIntegrityQueueEntityBroker updateBroker = updateContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
                StudyIntegrityQueueUpdateColumns columns = new StudyIntegrityQueueUpdateColumns
                                                           	{Details = updatedQueueDataXml};
            	updateBroker.Update(item.GetKey(), columns);
            }
        }

        private static ReconcileStudyQueueDescription GetQueueEntryDescription(StudyStorageLocation existingStorage, DicomMessageBase file)
        {
            ReconcileStudyQueueDescription desc = new ReconcileStudyQueueDescription
                                                  	{
                                                  		ExistingPatientId = existingStorage.Study.PatientId,
                                                  		ExistingPatientName = existingStorage.Study.PatientsName,
                                                  		ExistingAccessionNumber = existingStorage.Study.AccessionNumber,
                                                  		ConflictingPatientName =
                                                  			file.DataSet[DicomTags.PatientsName].GetString(0, String.Empty),
                                                  		ConflictingPatientId =
                                                  			file.DataSet[DicomTags.PatientId].GetString(0, String.Empty),
                                                  		ConflictingAccessionNumber =
                                                  			file.DataSet[DicomTags.AccessionNumber].GetString(0, String.Empty)
                                                  	};

        	return desc;
        }
    }
}