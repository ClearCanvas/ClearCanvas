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
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy.Extensions
{
    /// <summary>
    /// Plugin to backup study when it is deleted for auditing purpose.
    /// </summary>
    [ExtensionOf(typeof(DeleteStudyProcessorExtensionPoint))]
    internal class StudyDeleteProcessorExtension:IDeleteStudyProcessorExtension
    {
        #region Private Members
        private DeleteStudyContext _context;
        private bool _enabled = true;
        private string _backupSubFolder;
        private string _backupZipFile;
        private DeletedStudyArchiveInfoCollection _archives;
        
        private string BackupSubPath
        {
            get
            {
                if (String.IsNullOrEmpty(_backupSubFolder))
                {

                    string pathPrefix = Path.Combine(_context.ServerPartition.PartitionFolder, "Deleted");
                    _backupSubFolder = Path.Combine(pathPrefix, _context.StorageLocation.StudyInstanceUid);
                }

                return _backupSubFolder;
            }
        }

        private string BackupZipFileRelativePath
        {
            get
            {
                if (String.IsNullOrEmpty(_backupZipFile))
                {
                    _backupZipFile = Path.Combine(BackupSubPath, Platform.Time.ToString("yyyy-MM-dd-hhmm")) + ".zip";

                }

                return _backupZipFile;
            }
        }

        private string BackupFullPath
        {
            get
            {
                return _context.Filesystem.ResolveAbsolutePath(BackupZipFileRelativePath);
            }
        }
        #endregion

        #region IDeleteStudyProcessorExtension Members

        public bool Enabled
        {
            get { return _enabled; }
        }

        #endregion

        #region IDeleteStudyProcessorExtension Members

        public void Initialize(DeleteStudyContext context)
        {
            Platform.CheckForNullReference(context, "context");
            _context = context;


            _enabled = context.ServerPartition.AuditDeleteStudy;
        }

        public void OnStudyDeleting()
        {
            if (!Enabled)
            {
                return;
            }

            StudyStorageLocation storage = _context.StorageLocation;
            IList < ArchiveStudyStorage> archives = StudyStorageLocation.GetArchiveLocations(storage.GetKey());
            if (archives!=null && archives.Count>0)
            {
                _archives = new DeletedStudyArchiveInfoCollection();
                foreach (ArchiveStudyStorage archive in archives)
                {
                    DeletedStudyArchiveInfo archiveInfo = new DeletedStudyArchiveInfo();
                    archiveInfo.ArchiveTime = archive.ArchiveTime;
                    archiveInfo.ArchiveXml = archive.ArchiveXml;
                    
                    archiveInfo.PartitionArchiveRef = PartitionArchive.Load(archive.PartitionArchiveKey).GetKey().Key;
                    archiveInfo.TransferSyntaxUid = archive.ServerTransferSyntax.Uid;
                    _archives.Add(archiveInfo);
                }
            }
            
                        

            // only backup if study is manually deleted
            if (_context.WorkQueueItem.WorkQueueTypeEnum == WorkQueueTypeEnum.WebDeleteStudy)
            {
                
                using (var processor = new ServerCommandProcessor("Backup deleted study"))
                {
                    string path = _context.Filesystem.ResolveAbsolutePath(BackupSubPath);

                    Platform.Log(LogLevel.Info, "Saving a copy of the study to {0}...", path);

                    var mkdir = new CreateDirectoryCommand(path);
                    processor.AddCommand(mkdir);

                    var zip = new ZipStudyFolderCommand(storage.GetStudyPath(), BackupFullPath);
                    processor.AddCommand(zip);

                    if (!processor.Execute())
                    {
                        throw new ApplicationException(String.Format("Unable to backup study: {0}", processor.FailureReason));
                    }
                }
            }
            
        }

        public void OnStudyDeleted()
        {
            if (!Enabled)
            {
                return;
            }

            if (_context.WorkQueueItem.WorkQueueTypeEnum == WorkQueueTypeEnum.WebDeleteStudy)
            {
                Study study = _context.Study;

				if (study == null)
				{
					Platform.Log(LogLevel.Info, "Not logging Study Delete information due to missing Study record for study: {0} on partition {1}",
					             _context.StorageLocation.StudyInstanceUid,
								 _context.ServerPartition.AeTitle);
					return;
				}

                StudyStorageLocation storage = _context.StorageLocation;

                using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    // Setup the parameters
                    IStudyDeleteRecordEntityBroker broker = updateContext.GetBroker<IStudyDeleteRecordEntityBroker>();

                    StudyDeleteRecordUpdateColumns parms = new StudyDeleteRecordUpdateColumns();
                    parms.Timestamp = Platform.Time;
                    WebDeleteStudyLevelQueueData extendedInfo =
                        XmlUtils.Deserialize<WebDeleteStudyLevelQueueData>(_context.WorkQueueItem.Data);

                    parms.Reason = extendedInfo != null? 
                                                           extendedInfo.Reason:_context.WorkQueueItem.WorkQueueTypeEnum.LongDescription;

                    parms.ServerPartitionAE = _context.ServerPartition.AeTitle;
                    parms.FilesystemKey = storage.FilesystemKey;

                    parms.AccessionNumber = study.AccessionNumber;
                    parms.PatientId = study.PatientId;
                    parms.PatientsName = study.PatientsName;
                    parms.StudyInstanceUid = study.StudyInstanceUid;
                    parms.StudyDate = study.StudyDate;
                    parms.StudyDescription = study.StudyDescription;
                    parms.StudyTime = study.StudyTime;

                    parms.BackupPath = BackupZipFileRelativePath;

                    if (_archives != null && _archives.Count>0)
                    {
                        parms.ArchiveInfo = XmlUtils.SerializeAsXmlDoc(_archives);
                    }

                    StudyDeleteExtendedInfo extInfo = new StudyDeleteExtendedInfo();
                    extInfo.ServerInstanceId = ServerPlatform.ServerInstanceId;
                    extInfo.UserId = _context.UserId;
                    extInfo.UserName = _context.UserName;
                    parms.ExtendedInfo = XmlUtils.SerializeAsString(extInfo);

                    StudyDeleteRecord deleteRecord = broker.Insert(parms);
                    if (deleteRecord == null)
                    {
                        Platform.Log(LogLevel.Error, "Unexpected error when trying to create study delete record: {0} on partition {1}",
                                     study.StudyInstanceUid, _context.ServerPartition.Description);
                    }
                    else
                        updateContext.Commit();
                }
            }
        }

        #endregion
       
    }

    /// <summary>
    /// Extended information for a deleted study record
    /// </summary>
    [Serializable]
    public class StudyDeleteExtendedInfo
    {
        #region Private Fields
        private string _serverInstanceId;
        private string _userId;
        private string _userName;
        #endregion

        #region Public Properties
        /// <summary>
        /// The server which processed the study delete request.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string ServerInstanceId
        {
            get { return _serverInstanceId; }
            set { _serverInstanceId = value; }
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        #endregion

    }

    /// <summary>
    /// Archive entry information for a deleted study
    /// </summary>
    [Serializable]
    public class DeletedStudyArchiveInfo
    {
        #region Private Fields
        private DateTime _archiveTime;
        private object _partitionArchiveRef;
        private string _transferSyntaxUid;
        private XmlDocument _archiveXml;
        private PartitionArchive _partitionArchive;
        #endregion

        #region Public Properties
        public DateTime ArchiveTime
        {
            get { return _archiveTime; }
            set { _archiveTime = value; }
        }

        public object PartitionArchiveRef
        {
            get { return _partitionArchiveRef; }
            set { _partitionArchiveRef = value; }
        }

        public string TransferSyntaxUid
        {
            get { return _transferSyntaxUid; }
            set { _transferSyntaxUid = value; }
        }

        public XmlDocument ArchiveXml
        {
            get { return _archiveXml; }
            set { _archiveXml = value; }
        }

        [XmlIgnore]
        public PartitionArchive PartitionArchive
        {
            get
            {
                if (PartitionArchiveRef == null)
                    throw new ApplicationException("PartitionArchiveRef must be set");

                if (_partitionArchive == null)
                {
                    ServerEntityKey key = new ServerEntityKey("ParitionArchive", PartitionArchiveRef);
                    _partitionArchive = PartitionArchive.Load(key);

                }

                return _partitionArchive;
            }
        }

        #endregion

    }

    /// <summary>
    /// Collection of archive entry information for a deleted study
    /// </summary>
    [Serializable]
    public class DeletedStudyArchiveInfoCollection : List<DeletedStudyArchiveInfo>
    {
    }
}