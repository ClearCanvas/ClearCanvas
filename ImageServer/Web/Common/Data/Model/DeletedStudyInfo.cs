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
using System.Text;
using System.Xml;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy.Extensions;

namespace ClearCanvas.ImageServer.Web.Common.Data.Model
{
    [Serializable]
    public class DeletedStudyInfo
    {
        #region Private Fields
        private object _rowkey;
        private string _studyInstanceUid;
        private string _patientsName;
        private string _patientId;
        private string _accessionNumber;
        private string _studyDate;
        private string _partitionAe;
        private string _studyDescription;
        private string _backupFolderPath;
        private string _reasonForDeletion;
        private DateTime _deleteTime;
        private ServerEntityKey _deleteStudyRecord;
        private  DeletedStudyArchiveInfoCollection _archives;

        private bool _restoreScheduled;
        private string _userName;
        private string _userId;

        #endregion

        public string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        public string PatientsName
        {
            get { return _patientsName; }
            set { _patientsName = value; }
        }

        public string PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

        public string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }

        public string PartitionAE
        {
            get { return _partitionAe; }
            set { _partitionAe = value; }
        }

        public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }

        public object RowKey
        {
            get { return _rowkey; }
            set { _rowkey = value; }
        }

        public string BackupFolderPath
        {
            get { return _backupFolderPath; }
            set { _backupFolderPath = value; }
        }

        public string ReasonForDeletion
        {
            get { return _reasonForDeletion; }
            set { _reasonForDeletion = value; }
        }

        public DateTime DeleteTime
        {
            get { return _deleteTime; }
            set { _deleteTime = value; }
        }

        public DeletedStudyArchiveInfoCollection Archives
        {
            get { return _archives; }
            set { _archives = value; }
        }

        public bool RestoreScheduled
        {
            get { return _restoreScheduled; }
            set { _restoreScheduled = value; }
        }

        public ServerEntityKey DeleteStudyRecord
        {
            get { return _deleteStudyRecord; }
            set { _deleteStudyRecord = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
    }

}
