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

using System.ComponentModel;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Server.ShredHost
{
	[SettingsGroupDescription("Configuration for the Shred Host Service.")]
	[SettingsProvider(typeof (LocalFileSettingsProvider))]
	[SharedSettingsMigrationDisabled]
	internal sealed partial class ShredHostServiceSettings
	{
		public const int DefaultShredHostHttpPort = 51121;
		public const int DefaultSharedHttpPort = 51122;
		public const int DefaultSharedTcpPort = 51123;
		public const string DefaultServiceAddressBase = "";

		private static Proxy _instance;

		public static Proxy Instance
		{
			get { return _instance ?? (_instance = new Proxy(Default)); }
		}

		public sealed class Proxy
		{
			private readonly ShredHostServiceSettings _settings;

			public Proxy(ShredHostServiceSettings settings)
			{
				_settings = settings;
			}

			private object this[string propertyName]
			{
				get { return _settings[propertyName]; }
				set { ApplicationSettingsExtensions.SetSharedPropertyValue(_settings, propertyName, value); }
			}

			[DefaultValue(DefaultShredHostHttpPort)]
			public int ShredHostHttpPort
			{
				get { return (int) this["ShredHostHttpPort"]; }
				set { this["ShredHostHttpPort"] = value; }
			}

			[DefaultValue(DefaultSharedHttpPort)]
			public int SharedHttpPort
			{
				get { return (int) this["SharedHttpPort"]; }
				set { this["SharedHttpPort"] = value; }
			}

			[DefaultValue(DefaultSharedTcpPort)]
			public int SharedTcpPort
			{
				get { return (int) this["SharedTcpPort"]; }
				set { this["SharedTcpPort"] = value; }
			}

			[DefaultValue(DefaultServiceAddressBase)]
			public string ServiceAddressBase
			{
				get { return (string) this["ServiceAddressBase"]; }
				set { this["ServiceAddressBase"] = value; }
			}

			public void Save()
			{
				_settings.Save();
			}
		}
	}
}