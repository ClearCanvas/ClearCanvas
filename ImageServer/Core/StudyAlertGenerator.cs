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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core
{

    enum StudyAlertType
    {
        BadDicomData
    }

    class StudyAlert : IEquatable<StudyAlert>
    {
        internal string ApplicationContext { get; private set; }

        internal String ServerPartitionAE { get; private set; }
        internal String StudyInstanceUid { get; private set; }
        internal StudyAlertType AlertType { get; private set; }
        internal string Message { get; private set; }
        internal DateTime Timestamp { get; private set; }

        internal StudyAlert(string applicationContext, string serverPartitionAE, string studyInstanceUid, StudyAlertType type, string message)
        {
            ServerPartitionAE = serverPartitionAE;
            ApplicationContext = applicationContext;
            Timestamp = Platform.Time;
            StudyInstanceUid = studyInstanceUid;
            AlertType = type;
            Message = message;
        }


        public bool Equals(StudyAlert other)
        {
            return StudyInstanceUid.Equals(other.StudyInstanceUid) &&
                   AlertType == other.AlertType &&
                   Message.Equals(other.Message, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    /// <summary>
    /// Helper class to generate alerts for a study
    /// </summary>
    class StudyAlertGenerator
    {
        private static readonly List<StudyAlert> _recentAlerts = new List<StudyAlert>();
        private static readonly object _syncLock = new object();

        /// <summary>
        /// Generates a <see cref="StudyAlert"/>. 
        /// </summary>
        /// <param name="alert"></param>
        /// <remarks>
        /// If the same alert is generated in the last 30 minutes, the new one will be dropped
        /// </remarks>
        public static void Generate(StudyAlert alert)
        {
            lock (_syncLock)
            {
                if (!_recentAlerts.Contains(alert))
                {
                    _recentAlerts.Add(alert);

                    // TODO: Map StudyAlertCode to system alert code
                    const int code = -1000;
                    ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Warning, alert.ApplicationContext, code,
                                         new StudyAlertContextInfo(alert.ServerPartitionAE, alert.StudyInstanceUid),
                                         TimeSpan.FromMinutes(30), alert.Message);
                }

                _recentAlerts.RemoveAll(a => Platform.Time - a.Timestamp > TimeSpan.FromMinutes(30));
            }
        }

    }
}