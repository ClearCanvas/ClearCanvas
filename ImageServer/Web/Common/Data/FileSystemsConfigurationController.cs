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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Defines the interface of a file system configuration controller.
    /// </summary>
    public interface IFileSystemsConfigurationController
    {
        bool AddFileSystem(Filesystem filesystem);
        bool UpdateFileSystem(Filesystem filesystem);
        IList<Filesystem> GetFileSystems(FilesystemSelectCriteria criteria);
        IList<Filesystem> GetAllFileSystems();

        IList<FilesystemTierEnum> GetFileSystemTiers();
    }

    public class FileSystemsConfigurationController
    {
        #region Private members

        /// <summary>
        /// The adapter class to retrieve/set filesystems from Filesystem table
        /// </summary>
        private FileSystemDataAdapter _adapter = new FileSystemDataAdapter();

        #endregion

        #region public methods

        public bool AddFileSystem(Filesystem filesystem)
        {
            Platform.Log(LogLevel.Info, "Adding new filesystem : description = {0}, path={1}", filesystem.Description,
                         filesystem.FilesystemPath);

            bool ok = _adapter.AddFileSystem(filesystem);

            Platform.Log(LogLevel.Info, "New filesystem added : description = {0}, path={1}", filesystem.Description,
                         filesystem.FilesystemPath);

            return ok;
        }

        public bool UpdateFileSystem(Filesystem filesystem)
        {
            Platform.Log(LogLevel.Info, "Updating filesystem : description = {0}, path={1}", filesystem.Description,
                         filesystem.FilesystemPath);

            bool ok = _adapter.Update(filesystem);

            if (ok)
                Platform.Log(LogLevel.Info, "Filesystem updated: description = {0}, path={1}", filesystem.Description,
                             filesystem.FilesystemPath);
            else
                Platform.Log(LogLevel.Info, "Unable to update Filesystem: description = {0}, path={1}",
                             filesystem.Description, filesystem.FilesystemPath);

            return ok;
        }

        public Filesystem LoadFileSystem(ServerEntityKey key)
        {
            return Filesystem.Load(key);
        }

        public IList<Filesystem> GetFileSystems(FilesystemSelectCriteria criteria)
        {
            return _adapter.GetFileSystems(criteria);
        }

        public IList<Filesystem> GetFileSystems(IList<ServerEntityKey> keys)
        {
            List<Filesystem> fileSystems = new List<Filesystem>();

            foreach(ServerEntityKey key in keys)
            {
                fileSystems.Add(LoadFileSystem(key));
            }

            return fileSystems;
        }

        public IList<Filesystem> GetAllFileSystems()
        {
            return _adapter.GetAllFileSystems();
        }

        public IList<FilesystemTierEnum> GetFileSystemTiers()
        {
            return _adapter.GetFileSystemTiers();
        }

        #endregion public methods
    }
}
