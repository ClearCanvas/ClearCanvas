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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Enterprise.Command;

namespace ClearCanvas.ImageServer.Services.ServiceLock
{
    /// <summary>
    /// Represents the execution context of a <see cref="ServiceLock"/> item.
    /// </summary>
    public class ServiceLockProcessorContext : ServerExecutionContext
    {
        #region Private fields
        private readonly Model.ServiceLock _item;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="ServiceLockProcessorContext"/>
        /// </summary>
        /// <param name="item"></param>
        public ServiceLockProcessorContext(Model.ServiceLock item)
            :base(item.GetKey().Key.ToString())
        {
            Platform.CheckForNullReference(item, "item");
            _item = item;
        }
        #endregion
       
        #region Private Methods
        protected override string GetTemporaryPath()
        {
            ServerFilesystemInfo filesystem = FilesystemMonitor.Instance.GetFilesystemInfo(_item.FilesystemKey);
            if (filesystem == null)
            {
                // not ready?
                return base.GetTemporaryPath();
            }
            else
            {
                String basePath = GetTempPathRoot();
                if (String.IsNullOrEmpty(basePath))
                {
                    basePath = Path.Combine(filesystem.Filesystem.FilesystemPath, "temp");
                }
                String tempDirectory = Path.Combine(basePath, String.Format("{0}-{1}", _item.ServiceLockTypeEnum.Lookup, _item.GetKey()));
                
                for (int i = 2; i < 1000; i++)
                {
                    if (!Directory.Exists(tempDirectory))
                    {
                        break;
                    }

                    tempDirectory = Path.Combine(basePath, String.Format("{0}-{1}({2})", _item.ServiceLockTypeEnum.Lookup, _item.GetKey(), i));
                }

                if (!Directory.Exists(tempDirectory))
                    Directory.CreateDirectory(tempDirectory);

                return tempDirectory;
            }
        }
        #endregion

    }
}