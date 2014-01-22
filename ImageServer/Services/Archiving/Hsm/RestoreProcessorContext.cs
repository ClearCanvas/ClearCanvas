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
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
    /// <summary>
    /// Represents the execution context of a <cref="RestoreQueue"/> item.
    /// </summary>
    public class RestoreProcessorContext : ServerExecutionContext
    {
        #region Private Fields
        private readonly Model.RestoreQueue _item;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="RestoreProcessorContext"/>
        /// </summary>
        /// <param name="item"></param>
        public RestoreProcessorContext(RestoreQueue item)
            :base(item.GetKey().Key.ToString())
        {
            Platform.CheckForNullReference(item, "item");
            _item = item;
        }
        #endregion

        #region Protected Methods
        protected override string GetTemporaryPath()
        {
            StudyStorageLocation storage = StudyStorageLocation.FindStorageLocations(StudyStorage.Load(_item.StudyStorageKey))[0];
            if (storage == null)
                return base.GetTemporaryPath();
            else
            {
                String basePath = GetTempPathRoot();

                if (String.IsNullOrEmpty(basePath))
                {
                    basePath = Path.Combine(storage.FilesystemPath, "temp");
                }
                String tempDirectory = Path.Combine(basePath, String.Format("RestoreQueue-{0}", _item.GetKey()));

                for (int i = 2; i < 1000; i++)
                {
                    if (!Directory.Exists(tempDirectory))
                    {
                        break;
                    }

                    tempDirectory = Path.Combine(basePath, String.Format("RestoreQueue-{0}({1})", _item.GetKey(), i));
                }

                if (!Directory.Exists(tempDirectory))
                    Directory.CreateDirectory(tempDirectory);

                return tempDirectory;
            }
        }

        #endregion
    }
}