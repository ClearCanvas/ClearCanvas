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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
    internal class ServerTreeLocalServer : IServerTreeLocalServer
    {
        private string _path;
        private string _parentPath;

        private DicomServerConfiguration _dicomServerConfiguration;
        private StorageConfiguration _storageConfiguration;

        private DicomServerConfiguration DicomServerConfiguration
        {
            get { return _dicomServerConfiguration ?? (_dicomServerConfiguration = DicomServer.GetConfiguration()); }
        }

        private StorageConfiguration StorageConfiguration
        {
            get { return _storageConfiguration ?? (_storageConfiguration = StudyStore.GetConfiguration()); }
        }

        #region IServerTreeLocalServer Members

        public string AETitle { get { return DicomServerConfiguration.AETitle; } }
        public string HostName { get { return DicomServerConfiguration.HostName; } }
        public int? Port { get { return DicomServerConfiguration.Port; } }
        
        public string FileStoreLocation { get { return StorageConfiguration.FileStoreDirectory; } }
        
        public void Refresh()
        {
            _dicomServerConfiguration = DicomServer.GetConfiguration();
            _storageConfiguration = StudyStore.GetConfiguration();
        }

        #endregion

        #region IServerTreeNode Members

        public bool IsChecked { get; set; }

        public bool IsLocalDataStore
        {
            get { return IsLocalServer; }
        }

        public bool IsLocalServer
        {
            get { return true; }
        }

        public bool IsServer
        {
            get { return false; }
        }

        public bool IsServerGroup
        {
            get { return false;	}
        }

        public bool IsRoot
        {
            get { return false; }
        }

        public string ParentPath
        {
            get { return _parentPath; }
        }

        public string Path
        {
            get { return _path; }
        }

        public string Name
        {
            get { return @"My Studies"; }
        }

        public string DisplayName
        {
            get { return SR.MyStudiesTitle; }
        }

        #endregion

        public override string ToString()
        {
            try
            {
                var formatString = DicomServer.IsListening
                                        ? SR.FormatLocalServerDetails
                                        : SR.FormatLocalServerOfflineDetails;

                return String.Format(formatString,
                                        DisplayName,
                                        DicomServerConfiguration.AETitle,
                                        DicomServerConfiguration.HostName,
                                        DicomServerConfiguration.Port);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
            }

            return String.Format(SR.FormatLocalServerConfigurationUnavailable, DisplayName);
        }
        
        internal void ChangeParentPath(string parentPath)
        {
            _parentPath = parentPath ?? "";
            _path = _parentPath + Name;
        }
    }
}