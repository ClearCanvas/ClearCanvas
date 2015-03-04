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
using ClearCanvas.ImageServer.Core.Reconcile.CreateStudy;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
    /// <summary>
    /// Command to save the <see cref="UidMapper"/> used in the reconciliation.
    /// </summary>
    public class SaveUidMapXmlCommand : CommandBase, IDisposable
    {
        #region Private Members
        private readonly UidMapper _map;
        private readonly StudyStorageLocation _studyLocation;
        private string _path;
        private string _backupPath; 
        #endregion

        #region Constructors

        public SaveUidMapXmlCommand(StudyStorageLocation studyLocation, UidMapper mapper) :
            base("SaveUidMap", true)
        {
            _studyLocation = studyLocation;
            _map = mapper;
        } 
        #endregion

        #region Overridden Protected Methods
        protected override void OnExecute(CommandProcessor theProcessor)
        {
            if (_map == null)
                return;// nothing to save

            _path = Path.Combine(_studyLocation.GetStudyPath(), "UidMap.xml");
            if (RequiresRollback)
                Backup();

            _map.Save(_path);
        }

        protected override void OnUndo()
        {
            if (File.Exists(_backupPath))
            {
                File.Copy(_backupPath, _path, true);
            }
        } 
        #endregion

        #region Private Methods

        private void Backup()
        {
            _backupPath = FileUtils.Backup(_path, ProcessorContext.BackupDirectory);
        }
        
        #endregion

        #region Public Methods
        public void Dispose()
        {
            FileUtils.Delete(_backupPath);
        } 
        #endregion
    }
}