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
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy
{
    public class DeleteStudyContext
    {
        private ServerPartition _serverPartition;
        private StudyStorageLocation _storage;
        private Study _study;
        private Model.WorkQueue _item;
        private ServerFilesystemInfo _filesystem;
        private string _userId;
        private string _userName;

        public ServerPartition ServerPartition
        {
            get { return _serverPartition; }
            set { _serverPartition = value; }
        }
        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public StudyStorageLocation StorageLocation
        {
            get { return _storage; }
            set { _storage = value; }
        }

        public ServerFilesystemInfo Filesystem
        {
            get { return _filesystem; }
            set { _filesystem = value; }
        }

        public Model.WorkQueue WorkQueueItem
        {
            get { return _item; }
            set { _item = value; }
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
    }

    public interface IDeleteStudyProcessorExtension
    {
        bool Enabled { get; }
        void Initialize(DeleteStudyContext context);
        void OnStudyDeleting();
        void OnStudyDeleted();
    }

    public class DeleteStudyProcessorExtensionPoint:ExtensionPoint<IDeleteStudyProcessorExtension>
    {}
}
