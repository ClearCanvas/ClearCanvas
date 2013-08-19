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
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// Represents the execution context where the WorkQueue item processor is run.
    /// </summary>
    public class WorkQueueProcessorContext : ServerExecutionContext
    {
        #region Private Fields
        private readonly Model.WorkQueue _item;
        #endregion

        public WorkQueueProcessorContext(Model.WorkQueue item)
            :base(item.GetKey().Key.ToString())
        {
            _item = item;
            
        }

        protected override string GetTemporaryPath()
        {
            IList<StudyStorageLocation> storages =
                StudyStorageLocation.FindStorageLocations(StudyStorage.Load(_item.StudyStorageKey));

            if (storages == null || storages.Count == 0)
            {
                // ???
                return base.GetTemporaryPath();
            }

            ServerFilesystemInfo filesystem = FilesystemMonitor.Instance.GetFilesystemInfo(storages[0].FilesystemKey);
            if (filesystem == null)
            {
                // not ready?
                return base.GetTemporaryPath();
            }
            string basePath = GetTempPathRoot();

            if (String.IsNullOrEmpty(basePath))
            {
                basePath = Path.Combine(filesystem.Filesystem.FilesystemPath, "temp");
            }

            String tempDirectory = Path.Combine(basePath,
                                                String.Format("{0}-{1}", _item.WorkQueueTypeEnum.Lookup, _item.GetKey()));

            for (int i = 2; i < 1000; i++)
            {
                if (!Directory.Exists(tempDirectory))
                {
                    break;
                }

                tempDirectory = Path.Combine(basePath, String.Format("{0}-{1}({2})",
                                                                     _item.WorkQueueTypeEnum.Lookup, _item.GetKey(), i));
            }

            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);

            return tempDirectory;
        }
    }
}