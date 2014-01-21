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
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Core.Reconcile.CreateStudy;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
    internal abstract class ReconcileProcessorBase : ServerCommandProcessor
    {
        private ReconcileStudyProcessorContext _context;

        /// <summary>
        /// Create an instance of <see cref="ReconcileCreateStudyProcessor"/>
        /// </summary>
        protected ReconcileProcessorBase(string name)
            :base(name)
        {
        }

        protected internal ReconcileStudyProcessorContext Context
        {
            get { return _context; }
            set { _context = value; }
        }



        public string Name
        {
            get { return Description; }
        }

        protected void AddCleanupCommands()
        {
            string[] paths = GetFoldersToRemove();
            foreach(string path in paths)
            {
                AddCommand(new DeleteDirectoryCommand(path, false, true));
            }
        }

        protected string[] GetFoldersToRemove()
        {
            List<string> folders = new List<string>();

            Model.WorkQueue workqueueItem = Context.WorkQueueItem;

            // Path = \\Filesystem Path\Reconcile\GroupID\StudyInstanceUid\*.dcm
            if (!String.IsNullOrEmpty(workqueueItem.GroupID))
            {
                StudyStorageLocation storageLocation = Context.WorkQueueItemStudyStorage;
                string path = Path.Combine(storageLocation.FilesystemPath, storageLocation.PartitionFolder);
                path = Path.Combine(path, ServerPlatform.ReconcileStorageFolder);
                path = Path.Combine(path, workqueueItem.GroupID);
                
                string studyFolderPath = Path.Combine(path, storageLocation.StudyInstanceUid);

                folders.Add(studyFolderPath);
                folders.Add(path);
            }
            else
            {
                #region BACKWARD-COMPATIBLE CODE
                string path = Context.ReconcileWorkQueueData.StoragePath;
                folders.Add(path);
                #endregion
            }

            return folders.ToArray();
        }
    }
}