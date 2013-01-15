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
using System.Threading;
using ClearCanvas.Common.UsageTracking;
using ClearCanvas.Utilities.Manifest;
using System.Globalization;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Shreds.UsageTracking
{
    /// <summary>
    /// Service to send usage information periodically
    /// </summary>
    internal class UsageTrackingService
    {
        #region Private Fields

        private Timer _timer;
        private readonly TimeSpan _repeatEvery24Hours = TimeSpan.FromHours(24);
        private DateTime _startTimestamp;
        private TimeSpan? _uptime;

        #endregion

        #region Public Methods

        public void Start()
        {
            _startTimestamp = DateTime.Now; // don't need to use Platform.Time
            _timer = new Timer(OnTimer, null, _repeatEvery24Hours, _repeatEvery24Hours);

            OnStartup();
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            OnShutdown();
        }

        #endregion

        #region Private Methods

        private void OnStartup()
        {
            try
            {
                UsageMessage theMessage = CreateUsageMessage(UsageType.Startup);

                UsageUtilities.Register(theMessage, UsageTrackingThread.Background);
            }
            catch (Exception ex)
            {
                //catch everything or the shred host will crash
                Platform.Log(LogLevel.Error, ex, "Error occurred when trying to send usage tracking data");
            }
        }

        private UsageMessage CreateUsageMessage(UsageType type)
        {
            UsageMessage theMessage = UsageUtilities.GetUsageMessage();
            theMessage.MessageType = type;
            theMessage.Certified = ManifestVerification.Valid;

            AppendUsageData(theMessage);
            return theMessage;
        }

        private void OnShutdown()
        {
            try
            {
                _uptime = DateTime.Now - _startTimestamp;

                UsageMessage theMessage = CreateUsageMessage(UsageType.Shutdown);

                UsageUtilities.Register(theMessage, UsageTrackingThread.Current /* send in current thread */);
            }
            catch (Exception ex)
            {
                //catch everything or the shred host will crash
                Platform.Log(LogLevel.Error, ex, "Error occurred when trying to send usage tracking data");
            }
        }

        private void OnTimer(object nothing)
        {
            try
            {
                _uptime = DateTime.Now - _startTimestamp;

                UsageMessage theMessage = CreateUsageMessage(UsageType.Other);

                AppendUsageData(theMessage);

                UsageUtilities.Register(theMessage, UsageTrackingThread.Background);
            }
            catch (Exception ex)
            {
                //catch everything or the shred host will crash
                Platform.Log(LogLevel.Error, ex, "Error occurred when trying to send usage tracking data");
            }
        }

        private void AppendUsageData(UsageMessage message)
        {
            message.AppData = new System.Collections.Generic.List<UsageApplicationData>();

            if (_uptime != null)
            {
                message.AppData.Add(new UsageApplicationData()
                {
                    Key = "UPTIME",
                    Value = _uptime.Value.TotalHours.ToString(CultureInfo.InvariantCulture.NumberFormat),
                });
            }

        }

        #endregion
    }
}
