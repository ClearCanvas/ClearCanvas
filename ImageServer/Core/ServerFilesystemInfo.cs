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
using System.Runtime.InteropServices;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core
{
    public class ServerFilesystemInfo
    {
        #region Private Members
        private Filesystem _filesystem;
        private readonly object _lock = new object();
        private float _freeBytes;
        private float _totalBytes;
        private bool _online;
        private DateTime _lastOfflineLog = Platform.Time.AddDays(-1);
        #endregion

        #region Public Properties
        /// <summary>
        /// The <see cref="Filesystem"/> domain object.
        /// </summary>
        public Filesystem Filesystem
        {
            get { return _filesystem; }
            internal set { _filesystem = value; }
        }

        /// <summary>
        /// Is the filesystem Online?
        /// </summary>
        public bool Online
        {
            get { return _online; }
        }

        /// <summary>
        /// Returns a boolean value indicating whether the filesystem is writable.
        /// </summary>
        public bool Writeable
        {
            get
            {
                if (!_online || _filesystem.ReadOnly || !_filesystem.Enabled)
                    return false;

                return !Full;
            }
        }

        public bool ReadOnly
        {
            get { return _filesystem.ReadOnly; }
        }
        
        /// <summary>
        /// Returns a boolean value indicating whether the filesystem is readonly.
        /// </summary>
        public bool Readable
        {
            get
            {
                if (!_online || _filesystem.WriteOnly || !_filesystem.Enabled)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Returns a boolean value indicating whether the filesystem is full.
        /// </summary>
        public bool Full
        {
            get
            {
                return FreeBytes / 1024f /1024f < Settings.Default.MinStorageRequiredInMB; 
            }
        }

        /// <summary>
        /// Returns a text that describing the different statuses of the filesystem.
        /// </summary>
        public string StatusString
        {
            get
            {
                StringBuilder status = new StringBuilder();
                status.Append(Enable ? "Enabled" : "Disabled");
                status.Append(" | ");
                status.Append(Online ? "Online" : "Offline");
                status.Append(" | ");
                status.Append(Readable ? "Readable" : "Not Readable");
                status.Append(" | ");
                status.Append(Writeable ? "Writable" : "Not Writable");
                status.Append(" | ");
                status.Append(Full 
                                ? String.Format("Full (Min Req: {0} MB)", Settings.Default.MinStorageRequiredInMB) 
                                : String.Format("{0} Available", ByteCountFormatter.Format((ulong)FreeBytes)));

                return status.ToString();
            }
        }

        /// <summary>
        /// Returns the number of bytes available in the filesystem.
        /// </summary>
        public float FreeBytes
        {
            get { return _freeBytes; }
        }

        /// <summary>
        /// Returns the number of bytes below the <see cref="Model.Filesystem.HighWatermark"/>
        /// </summary>
        /// <remarks>
        /// If the filesystem is above high watermark, <see cref="HighwaterMarkMargin"/> will become negative
        /// </remarks>
        public float HighwaterMarkMargin
        {
            get
            {
                return (_totalBytes*(float) _filesystem.HighWatermark / 100.0f) - (_totalBytes*UsedSpacePercentage/100.0f);
            }
        }


        public float TotalBytes
        {
            get { return _totalBytes; }
        }
        public float UsedSpacePercentage
        {
            get { return ((_totalBytes - _freeBytes) / _totalBytes) * 100.0F; }
        }

        public float BytesToRemove
        {
            get
            {
                float desiredUsedBytes = (((float) Filesystem.LowWatermark)/100.0f)*TotalBytes;

                return (TotalBytes - FreeBytes) - desiredUsedBytes;
            }
        }


        /// <summary>
        /// Is the filesystem above the low watermark?
        /// </summary>
        public bool AboveLowWatermark
        {
            get
            {
                return (UsedSpacePercentage > (float)Filesystem.LowWatermark);
            }
        }

        /// <summary>
        /// Is the filesystem above the high watermark?
        /// </summary>
        public bool AboveHighWatermark
        {
            get
            {
                return (UsedSpacePercentage > (float)Filesystem.HighWatermark);
            }
        }

        public bool Enable
        {
            get
            {
                return _filesystem.Enabled;
            }
        }

        #endregion


        public ServerFilesystemInfo(ServerFilesystemInfo copy)
        {
            _filesystem = copy.Filesystem;
            _online = copy.Online;
            _freeBytes = copy.FreeBytes;
            _totalBytes = copy.TotalBytes;
        }

        public string ResolveAbsolutePath(string relativePath)
        {
            return _filesystem.GetAbsolutePath(relativePath);
        } 

        internal ServerFilesystemInfo(Filesystem filesystem)
        {
            _filesystem = filesystem;
            _online = true;
            LoadFreeSpace();
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GetDiskFreeSpaceEx(string drive, out long freeBytesForUser, out long totalBytes, out long freeBytes);

        internal void LoadFreeSpace()
        {
        	lock (_lock)
            {
            	long freeBytesForUser;
            	long totalBytes;
            	long freeBytes;
            	if (false == GetDiskFreeSpaceEx(_filesystem.FilesystemPath, out freeBytesForUser, out totalBytes, out freeBytes))
                {
                    // Only log ever 30 minutes.
                    if (_lastOfflineLog.AddMinutes(30) < Platform.Time)
                    {
                        Platform.Log(LogLevel.Error, "Filesystem {0} is offline.  Unable to retrieve free disk space.", _filesystem.Description);
                        _lastOfflineLog = Platform.Time;
                    }
                    _online = false;
                }
                else
                {
                    if (!_online)
                    {
                        Platform.Log(LogLevel.Error, "Filesystem {0} has gone back online.", _filesystem.Description);
                        _online = true;
                    }
                }

                _totalBytes = totalBytes;
                _freeBytes = freeBytes;
            }
        }

        
    }

    
}
