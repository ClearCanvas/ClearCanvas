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
using System.Runtime.InteropServices;
using System.Threading;


namespace ClearCanvas.ImageServer.Common.Utilities
{
    /// <summary>
    /// Provides methods to determine existence of a folder and its properties
    /// </summary>
    public class FilesystemUtils
    {
        [DllImport("kernel32", CharSet = CharSet.Auto)]
        static extern int GetDiskFreeSpaceEx(
                                 string lpDirectoryName,
                                 out ulong lpFreeBytesAvailable,
                                 out ulong lpTotalNumberOfBytes,
                                 out ulong lpTotalNumberOfFreeBytes);


        /// <summary>
        /// Checks if a specified directory exists on the network and accessible from local machine.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool DirectoryExists(String dir, int timeout)
        {
            bool exists = false;


            if (timeout > 0)
            {
                var t = new Thread(delegate()
                                       {
                                           exists = Directory.Exists(dir);
                                       });

                t.Start();
                t.Join(timeout);
                t.Abort();
            }
            else
            {
                exists = Directory.Exists(dir);
            }

            return exists;
        }

        
        /// <summary>
        /// Gets information of a directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FilesystemInfo GetDirectoryInfo(String path)
        {
            var fsInfo = new FilesystemInfo
                             {
                                 Path = path,
                                 Exists = DirectoryExists(path, 1000)
                             };

            if (fsInfo.Exists)
            {
                ulong available;
                ulong total;
                ulong free;
                GetDiskFreeSpaceEx(path, out available, out total, out free);

                fsInfo.FreeSizeInKB = available / 1024;
                fsInfo.SizeInKB = total / 1024;

            }

            return fsInfo;
        }
    }
}
