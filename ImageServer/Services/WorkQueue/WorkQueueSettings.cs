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
using System.Text;
using System.ComponentModel;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    internal partial class WorkQueueSettings
    {
        public const int DefaultWorkQueueQueryDelay = 10000;
        public const int DefaultWorkQueueThreadCount = 10;
        public const int DefaultPriorityWorkQueueThreadCount = 2;
        public const int DefaultMemoryLimitedWorkQueueThreadCount = 4;
        public const int DefaultWorkQueueMinimumFreeMemoryMB = 256;
        public const bool DefaultEnableStudyIntegrityValidation = true;
        public const int DefaultTierMigrationProgressUpdateInSeconds = 30;


        private static WorkQueueSettingsProxy _instance;

        public static WorkQueueSettingsProxy Instance
        {
            get { return _instance ?? (_instance = new WorkQueueSettingsProxy(Default)); }
        }

        public sealed class WorkQueueSettingsProxy
        {
            private readonly WorkQueueSettings _settings;

            public WorkQueueSettingsProxy(WorkQueueSettings settings)
            {
                _settings = settings;
            }

            private object this[string propertyName]
            {
                get { return _settings[propertyName]; }
                set { ApplicationSettingsExtensions.SetSharedPropertyValue(_settings, propertyName, value); }
            }

            [DefaultValue(DefaultWorkQueueQueryDelay)]
            public int WorkQueueQueryDelay
            {
                get { return (int)this["WorkQueueQueryDelay"]; }
                set { this["WorkQueueQueryDelay"] = value; }
            }

            [DefaultValue(DefaultEnableStudyIntegrityValidation)]
            public bool EnableStudyIntegrityValidation
            {
                get { return (bool)this["EnableStudyIntegrityValidation"]; }
                set { this["EnableStudyIntegrityValidation"] = value; }
            }

            [DefaultValue(DefaultMemoryLimitedWorkQueueThreadCount)]
            public int MemoryLimitedWorkQueueThreadCount
            {
                get { return (int)this["MemoryLimitedWorkQueueThreadCount"]; }
                set { this["MemoryLimitedWorkQueueThreadCount"] = value; }
            }

            [DefaultValue(DefaultPriorityWorkQueueThreadCount)]
            public int PriorityWorkQueueThreadCount
            {
                get { return (int)this["PriorityWorkQueueThreadCount"]; }
                set { this["PriorityWorkQueueThreadCount"] = value; }
            }

            [DefaultValue(DefaultTierMigrationProgressUpdateInSeconds)]
            public int TierMigrationProgressUpdateInSeconds
            {
                get { return (int)this["TierMigrationProgressUpdateInSeconds"]; }
                set { this["TierMigrationProgressUpdateInSeconds"] = value; }
            }

            [DefaultValue(DefaultWorkQueueMinimumFreeMemoryMB)]
            public int WorkQueueMinimumFreeMemoryMB
            {
                get { return (int)this["WorkQueueMinimumFreeMemoryMB"]; }
                set { this["WorkQueueMinimumFreeMemoryMB"] = value; }
            }

            [DefaultValue(DefaultWorkQueueThreadCount)]
            public int WorkQueueThreadCount
            {
                get { return (int)this["WorkQueueThreadCount"]; }
                set { this["WorkQueueThreadCount"] = value; }
            }


            public void Save()
            {
                _settings.Save();
            }
        }
    }
}
