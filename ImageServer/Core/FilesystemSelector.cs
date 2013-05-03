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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Core
{
    /// <summary>
    /// Provide sorting function on <see cref="ServerFilesystemInfo"/>
    /// </summary>
    public static class FilesystemSorter
    {
        public static int SortByFreeSpace(ServerFilesystemInfo fs1, ServerFilesystemInfo fs2)
        {
            Platform.CheckForNullReference(fs1, "fs1");
            Platform.CheckForNullReference(fs2, "fs2");
            Platform.CheckForNullReference(fs1.Filesystem, "fs1.Filesystem");
            Platform.CheckForNullReference(fs2.Filesystem, "fs2.Filesystem");

            if (fs1 == fs2)
                return 0;

            if (fs1.Filesystem.FilesystemTierEnum.Enum.Equals(fs2.Filesystem.FilesystemTierEnum.Enum))
            {
                // descending order on available size.. smaller margin means less available space
                return fs2.HighwaterMarkMargin.CompareTo(fs1.HighwaterMarkMargin);
            }
            else
            {
                // ascending order on tier
                return fs1.Filesystem.FilesystemTierEnum.Enum.CompareTo(fs2.Filesystem.FilesystemTierEnum.Enum);
            }
        }
    }

    /// <summary>
    /// Class used for incoming studies to select which filesystem the study should be 
    /// stored to.
    /// </summary>
    public class FilesystemSelector
    {
        private readonly FilesystemMonitor _monitor;

        public FilesystemSelector(FilesystemMonitor monitor)
        {
            _monitor = monitor;
        }

        public ServerFilesystemInfo SelectFilesystem(DicomMessageBase msg)
        {
            return SelectFilesystem();
        }

        public ServerFilesystemInfo SelectFilesystem()
        {
            IList<ServerFilesystemInfo> list = new List<ServerFilesystemInfo>(_monitor.GetFilesystems());
            IList<ServerFilesystemInfo> writableFS = CollectionUtils.Select(list, delegate(ServerFilesystemInfo fs) { return fs.Writeable; });

            StringBuilder log = new StringBuilder();
            if (writableFS == null || writableFS.Count == 0)
            {
                log.AppendLine("No writable storage found");
                foreach (ServerFilesystemInfo fs in list)
                {
                    log.AppendLine(string.Format("\t{0} : {1}", fs.Filesystem.Description, fs.StatusString));
                }
                Platform.Log(LogLevel.Warn, log.ToString());
                return null;
            }

            writableFS = CollectionUtils.Sort(writableFS, FilesystemSorter.SortByFreeSpace);
            ServerFilesystemInfo selectedFS = CollectionUtils.FirstElement(writableFS);
            Platform.CheckForNullReference(selectedFS, "selectedFS");
            return selectedFS;
        }
    }
}
