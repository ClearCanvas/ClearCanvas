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
using System.Linq;
using System.Text;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common
{
    /// <summary>
    /// Helper class to determine disk usage
    /// </summary>
    public static class LocalStorageMonitor
    {
        #region Private Fields

        static private readonly object _sync = new object();
        static DateTime? _scheduledRefreshTime;
        static DateTime? _lastConfigUpdate;

        // Note: _storageConfig is used to retrieve the configuration only (in most cases, we are only interested in MaximumUsedSpacePercent)
        // This object is reloaded every 10 seconds
        static StorageConfiguration _storageConfig;

        // Note: _diskspace is used instead of relying on the Diskspace object in _storageConfig because _storageConfig is reloaded every 10 seconds.
        // In fact, WQI processors should avoid loading a StorageConfiguration object through StudyStore.GetConfiguration() to check the disk usage.
        static Diskspace _diskspace;

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if max used space is exceeded in the local study storage location
        /// </summary>
        static public bool IsMaxUsedSpaceExceeded
        {
            get
            {
                lock (_sync)
                {

                    RefreshDiskspace();

                    // Note: Because _storageConfig is reloaded every 10 seconds to detect any configuration change. 
                    // Using _storageConfig.IsMaxUsedSpaceExceeded will cause diskspace to be recalculated every 10 seconds
                    return FileStoreDiskSpace.UsedSpacePercent > StorageConfiguration.MaximumUsedSpacePercent;
                }
            }
            
        }

        public static double MaximumUsedSpacePercent
        {
            get 
            {
                lock (_sync)
                {
                    return StorageConfiguration.MaximumUsedSpacePercent;
                }
            }
        }

        public static double UsedSpacePercent
        {
            get
            {
                lock (_sync)
                {
                    return FileStoreDiskSpace.UsedSpacePercent;
                }
            }
        }

        private static StorageConfiguration StorageConfiguration
        {
            get
            {
                if (!_lastConfigUpdate.HasValue || _lastConfigUpdate.Value.OlderThan(TimeSpan.FromSeconds(10)))
                {

                    double prevMaxUsed = _storageConfig != null ? _storageConfig.MaximumUsedSpacePercent : 0;
                    _storageConfig = StudyStore.GetConfiguration();

                    // detect change
                    if (prevMaxUsed == 0 || prevMaxUsed != _storageConfig.MaximumUsedSpacePercent)
                    {
                        FileStoreDiskSpace = null;
                        RefreshDiskspace();
                    }
                    _lastConfigUpdate = Platform.Time;
                }

                return _storageConfig;
            }
        }

        private static Diskspace FileStoreDiskSpace 
        {
            get
            {
                if (_diskspace == null)
                {
                    _diskspace = new Diskspace(FileStoreRootPath);
                }

                return _diskspace;
            }
            set 
            { 
                _diskspace = value; 
                if (value == null)
                    _scheduledRefreshTime = null; // force diskspace to be recalculated                        
            }
        }

        private static string FileStoreRootPath
        {
            get
            {
                return StorageConfiguration.FileStoreRootPath;
            }
        }
        
        private static long MaximumUsedSpaceBytes
        {
            get
            {

                // note: not using StorageConfiguration.MaxUsedSpaceBytes because that will create another Diskspace object unnecessarily
                return (long) (FileStoreDiskSpace.TotalSpace / 100 * StorageConfiguration.MaximumUsedSpacePercent);
            }
        }

        #endregion

        #region Private Methods


        static private void RefreshDiskspace()
        {
            var needRefresh = !_scheduledRefreshTime.HasValue || _scheduledRefreshTime < Platform.Time;
            
            if (needRefresh)
            {
                FileStoreDiskSpace.Refresh();

                const long GB = 1024 * 1024 * 1024;
                double delay;

                double remain = MaximumUsedSpaceBytes - FileStoreDiskSpace.UsedSpace;

                // Note: Ideally we should calculate the how fast the usage is changing and estimate how long it will take to reach the max level
                if (Math.Abs(remain) <= 5 * GB)
                {
                    // within the critical level window. Check more often.
                    delay = 15; // 15 seconds
                }
                else
                {
                    delay = 5 * 60; // 5 minutes.
                }

                _scheduledRefreshTime = Platform.Time.AddSeconds(delay);


                if (remain>0 && remain < StorageMonitorSettings.Default.LowStorageWarningThresholdInMB * 1024 * 1024)
                {
                    Platform.Log(LogLevel.Warn, "File Storage usage is approaching the critical limit!!");
                }

                Platform.Log(LogLevel.Debug, "Diskspace updated. Check again in {0} seconds", delay);
            }

        }

        #endregion
    }

    static public class DateTimeExtensions
    {
        public static bool OlderThan(this DateTime dt, TimeSpan span)
        {
            return (Platform.Time - dt) > span;
        }
    }
}
