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
using System.Configuration;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
    [Obsolete("Use DiskspaceManagerSettings")]
    [LegacyShredConfigSection(@"DiskspaceManagerSettings")]
    internal sealed class LegacyDiskspaceManagerSettings : ShredConfigSection, IMigrateLegacyShredConfigSection
    {
		public const float LowWaterMarkDefault = 60F;
		public const float HighWaterMarkDefault = 80F;
		public const int CheckFrequencyDefault = 10;
    	public const int StudyLimitDefault = 500;
    	public const int MinStudyLimitDefault = 30;
		public const int MaxStudyLimitDefault = 10000;

        private static LegacyDiskspaceManagerSettings _instance;

        private LegacyDiskspaceManagerSettings()
        {
        }

        public static string SettingName
        {
            get { return "DiskspaceManagerSettings"; }
        }

        public static LegacyDiskspaceManagerSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = ShredConfigManager.GetConfigSection(LegacyDiskspaceManagerSettings.SettingName) as LegacyDiskspaceManagerSettings;
                    if (_instance == null)
                    {
                        _instance = new LegacyDiskspaceManagerSettings();
                        ShredConfigManager.UpdateConfigSection(LegacyDiskspaceManagerSettings.SettingName, _instance);
                    }
                }

                return _instance;
            }
        }

        public static void Save()
        {
            ShredConfigManager.UpdateConfigSection(LegacyDiskspaceManagerSettings.SettingName, _instance);
        }

        #region Public Properties

		[ConfigurationProperty("LowWatermark", DefaultValue = LowWaterMarkDefault)]
        public float LowWatermark
        {
            get { return (float)this["LowWatermark"]; }
            set { this["LowWatermark"] = value; }
        }

        [ConfigurationProperty("HighWatermark", DefaultValue = HighWaterMarkDefault)]
        public float HighWatermark
        {
            get { return (float)this["HighWatermark"]; }
            set { this["HighWatermark"] = value; }
        }

        [ConfigurationProperty("CheckFrequency", DefaultValue = CheckFrequencyDefault)]
        public int CheckFrequency
        {
            get { return (int)this["CheckFrequency"]; }
            set { this["CheckFrequency"] = value; }
        }

        [ConfigurationProperty("EnforceStudyLimit", DefaultValue = false)]
		public bool EnforceStudyLimit
		{
            get { return (bool)this["EnforceStudyLimit"]; }
            set { this["EnforceStudyLimit"] = value; }
		}

		[ConfigurationProperty("MinStudyLimit", DefaultValue = MinStudyLimitDefault)]
		public int MinStudyLimit
		{
			get { return (int)this["MinStudyLimit"]; }
			set { this["MinStudyLimit"] = value; }
		}

		[ConfigurationProperty("MaxStudyLimit", DefaultValue = MaxStudyLimitDefault)]
		public int MaxStudyLimit
		{
			get { return (int)this["MaxStudyLimit"]; }
			set { this["MaxStudyLimit"] = value; }
		}

		[ConfigurationProperty("StudyLimit", DefaultValue = StudyLimitDefault)]
		public int StudyLimit
		{
			get { return (int)this["StudyLimit"]; }
			set { this["StudyLimit"] = Math.Min(Math.Max(value, MinStudyLimit), MaxStudyLimit); }
		}
		
		#endregion

        public override object Clone()
        {
            LegacyDiskspaceManagerSettings clone = new LegacyDiskspaceManagerSettings();

            clone.LowWatermark = _instance.LowWatermark;
            clone.HighWatermark = _instance.HighWatermark;
            clone.CheckFrequency = _instance.CheckFrequency;
        	clone.EnforceStudyLimit = _instance.EnforceStudyLimit;
			clone.MaxStudyLimit = _instance.MaxStudyLimit;
			clone.MinStudyLimit = _instance.MinStudyLimit;
			clone.StudyLimit = _instance.StudyLimit;

            return clone;
        }

		void IMigrateLegacyShredConfigSection.Migrate()
		{
			DiskspaceManagerSettings.Instance.LowWatermark = LowWatermark;
			DiskspaceManagerSettings.Instance.HighWatermark = HighWatermark;
			DiskspaceManagerSettings.Instance.CheckFrequency = CheckFrequency;
			DiskspaceManagerSettings.Instance.EnforceStudyLimit = EnforceStudyLimit;
			DiskspaceManagerSettings.Instance.MaxStudyLimit = MaxStudyLimit;
			DiskspaceManagerSettings.Instance.MinStudyLimit = MinStudyLimit;
			DiskspaceManagerSettings.Instance.StudyLimit = StudyLimit;
			DiskspaceManagerSettings.Instance.Save();
		}
    }
}
