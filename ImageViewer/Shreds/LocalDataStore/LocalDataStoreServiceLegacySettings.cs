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

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	[Obsolete("Use LocalDataStoreServiceSettings")]
	[LegacyShredConfigSection(@"LocalDataStoreServiceSettings")]
	internal sealed class LegacyLocalDataStoreServiceSettings : ShredConfigSection, IMigrateLegacyShredConfigSection
	{
		public const string DefaultStorageDirectory = @"c:\dicom_datastore\filestore\";
		public const string DefaultBadFileDirectory = @"c:\dicom_datastore\badfiles\";
		public const uint DefaultSendReceiveImportConcurrency = 2;
		public const uint DefaultDatabaseUpdateFrequencyMilliseconds = 5000;
		public const uint DefaultPurgeTimeMinutes = 120;

		private static LegacyLocalDataStoreServiceSettings _instance;

		private LegacyLocalDataStoreServiceSettings()
		{
		}

		public static string SettingName
		{
			get { return "LocalDataStoreServiceSettings"; }
		}

		public static LegacyLocalDataStoreServiceSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = ShredConfigManager.GetConfigSection(LegacyLocalDataStoreServiceSettings.SettingName) as LegacyLocalDataStoreServiceSettings;
					if (_instance == null)
					{
						_instance = new LegacyLocalDataStoreServiceSettings();
						ShredConfigManager.UpdateConfigSection(LegacyLocalDataStoreServiceSettings.SettingName, _instance);
					}
				}

				return _instance;
			}
		}

		public static void Save()
		{
			ShredConfigManager.UpdateConfigSection(LegacyLocalDataStoreServiceSettings.SettingName, _instance);
		}

		#region Public Properties

		[ConfigurationProperty("StorageDirectory", DefaultValue = LegacyLocalDataStoreServiceSettings.DefaultStorageDirectory)]
		public string StorageDirectory
		{
			get { return (string)this["StorageDirectory"]; }
			set { this["StorageDirectory"] = value; }
		}

		[ConfigurationProperty("BadFileDirectory", DefaultValue = LegacyLocalDataStoreServiceSettings.DefaultBadFileDirectory)]
		public string BadFileDirectory
		{
			get { return (string)this["BadFileDirectory"]; }
			set { this["BadFileDirectory"] = value; }
		}

		[ConfigurationProperty("SendReceiveImportConcurrency", DefaultValue = LegacyLocalDataStoreServiceSettings.DefaultSendReceiveImportConcurrency)]
		public uint SendReceiveImportConcurrency
		{
			get { return (uint)this["SendReceiveImportConcurrency"]; }
			set { this["SendReceiveImportConcurrency"] = value; }
		}

		[ConfigurationProperty("DatabaseUpdateFrequencyMilliseconds", DefaultValue = LegacyLocalDataStoreServiceSettings.DefaultDatabaseUpdateFrequencyMilliseconds)]
		public uint DatabaseUpdateFrequencyMilliseconds
		{
			get { return (uint)this["DatabaseUpdateFrequencyMilliseconds"]; }
			set { this["DatabaseUpdateFrequencyMilliseconds"] = value; }
		}

		[ConfigurationProperty("PurgeTimeMinutes", DefaultValue = LegacyLocalDataStoreServiceSettings.DefaultPurgeTimeMinutes)]
		public uint PurgeTimeMinutes
		{
			get { return (uint)this["PurgeTimeMinutes"]; }
			set { this["PurgeTimeMinutes"] = value; }
		}

		#endregion

		public override object Clone()
		{
			LegacyLocalDataStoreServiceSettings clone = new LegacyLocalDataStoreServiceSettings();

			clone.StorageDirectory = _instance.StorageDirectory;
			clone.BadFileDirectory = _instance.BadFileDirectory;
			clone.SendReceiveImportConcurrency = _instance.SendReceiveImportConcurrency;
			clone.DatabaseUpdateFrequencyMilliseconds = _instance.DatabaseUpdateFrequencyMilliseconds;
			clone.PurgeTimeMinutes = _instance.PurgeTimeMinutes;

			return clone;
		}

		void IMigrateLegacyShredConfigSection.Migrate()
		{
			LocalDataStoreServiceSettings.Instance.StorageDirectory = StorageDirectory;
			LocalDataStoreServiceSettings.Instance.BadFileDirectory = BadFileDirectory;
			LocalDataStoreServiceSettings.Instance.SendReceiveImportConcurrency = SendReceiveImportConcurrency;
			LocalDataStoreServiceSettings.Instance.DatabaseUpdateFrequencyMilliseconds = DatabaseUpdateFrequencyMilliseconds;
			LocalDataStoreServiceSettings.Instance.PurgeTimeMinutes = PurgeTimeMinutes;
			LocalDataStoreServiceSettings.Instance.Save();
		}
	}
}
