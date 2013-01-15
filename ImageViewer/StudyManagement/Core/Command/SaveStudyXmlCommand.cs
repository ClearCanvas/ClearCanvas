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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Command
{
    /// <summary>
    /// <see cref="Command"/> for saving a <see cref="StudyXml"/> file for a Study.
    /// </summary>
    public class SaveStudyXmlCommand : CommandBase, IDisposable
    {
        #region Private Members

        private readonly StudyXml _studyXml;
        private readonly StudyLocation _studyStorageLocation;
        private string _backupPath;
        private bool _fileCreated;

        #endregion

        public SaveStudyXmlCommand(StudyXml studyXml, StudyLocation storageLocation)
            : base("Save Study Xml", true)
        {
            _studyXml = studyXml;
            _studyStorageLocation = storageLocation;
        }

        private void Backup()
        {
            if (File.Exists(_studyStorageLocation.GetStudyXmlPath()))
            {
                try
                {
                    _backupPath = FileUtils.Backup(_studyStorageLocation.GetStudyXmlPath(), ProcessorContext.BackupDirectory);
                }
                catch (IOException)
                {
                    _backupPath = null;
                    throw;
                }
            }
        }

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            Backup();

            _studyStorageLocation.SaveStudyXml(_studyXml, out _fileCreated);
            _fileCreated = true;
        }

        protected override void OnUndo()
        {
            if (false == String.IsNullOrEmpty(_backupPath) && File.Exists(_backupPath))
            {
                // restore original file
                File.Copy(_backupPath, _studyStorageLocation.GetStudyXmlPath(), true);
                File.Delete(_backupPath);
                _backupPath = null;
            }
            else if (File.Exists(_studyStorageLocation.GetStudyXmlPath()) && _fileCreated)
                File.Delete(_studyStorageLocation.GetStudyXmlPath());
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (false == String.IsNullOrEmpty(_backupPath) && File.Exists(_backupPath))
            {
                File.Delete(_backupPath);
            }
        }

        #endregion
    }
}
