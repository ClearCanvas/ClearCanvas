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

namespace ClearCanvas.Server.ShredHost
{
	[Obsolete("Use ShredHostServiceSettings")]
	[LegacyShredConfigSection("ShredHostServiceSettings")]
	internal sealed class LegacyShredHostServiceSettings : ShredConfigSection, IMigrateLegacyShredConfigSection
	{
		public const int DefaultShredHostHttpPort = 51121;
		public const int DefaultSharedHttpPort = 51122;
		public const int DefaultSharedTcpPort = 50123;
		public const string DefaultServiceAddressBase = "";

		private static LegacyShredHostServiceSettings _instance;

		private LegacyShredHostServiceSettings()
		{
		}

		public static string SettingName
		{
			get { return "ShredHostServiceSettings"; }
		}

		public static LegacyShredHostServiceSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = ShredConfigManager.GetConfigSection(LegacyShredHostServiceSettings.SettingName) as LegacyShredHostServiceSettings;
					if (_instance == null)
					{
						_instance = new LegacyShredHostServiceSettings();
						ShredConfigManager.UpdateConfigSection(LegacyShredHostServiceSettings.SettingName, _instance);
					}
				}

				return _instance;
			}
		}

		public static void Save()
		{
			ShredConfigManager.UpdateConfigSection(LegacyShredHostServiceSettings.SettingName, _instance);
		}

		#region Public Properties

		[ConfigurationProperty("ShredHostHttpPort", DefaultValue = LegacyShredHostServiceSettings.DefaultShredHostHttpPort)]
		public int ShredHostHttpPort
		{
			get { return (int)this["ShredHostHttpPort"]; }
			set { this["ShredHostHttpPort"] = value; }
		}

		[ConfigurationProperty("SharedHttpPort", DefaultValue = LegacyShredHostServiceSettings.DefaultSharedHttpPort)]
		public int SharedHttpPort
		{
			get { return (int)this["SharedHttpPort"]; }
			set { this["SharedHttpPort"] = value; }
		}

		[ConfigurationProperty("SharedTcpPort", DefaultValue = LegacyShredHostServiceSettings.DefaultSharedTcpPort)]
		public int SharedTcpPort
		{
			get { return (int)this["SharedTcpPort"]; }
			set { this["SharedTcpPort"] = value; }
		}

		[ConfigurationProperty("ServiceAddressBase", DefaultValue = LegacyShredHostServiceSettings.DefaultServiceAddressBase)]
		public string ServiceAddressBase
		{
			get { return (string)this["ServiceAddressBase"]; }
			set { this["ServiceAddressBase"] = value; ; }
		}

		#endregion

		public override object Clone()
		{
			LegacyShredHostServiceSettings clone = new LegacyShredHostServiceSettings();

			clone.ShredHostHttpPort = _instance.ShredHostHttpPort;
			clone.SharedHttpPort = _instance.SharedHttpPort;
			clone.SharedTcpPort = _instance.SharedTcpPort;
			clone.ServiceAddressBase = _instance.ServiceAddressBase;

			return clone;
		}

		void IMigrateLegacyShredConfigSection.Migrate()
		{
			// #10584 - do not migrate these settings! they are used internally by the product, and users generally do not need to change them
			// #10645 - do not remove this interface definition, as it also acts as a NO-OP to prevent the shred config section migration
		}
	}
}
