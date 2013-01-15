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
using System.Xml;
using System.Collections.Generic;

namespace ClearCanvas.Common.Configuration
{
	internal class ConfigurationFileReader
	{
		private readonly XmlDocument _document;

		public ConfigurationFileReader(string fileName)
			: this(new XmlDocument())
		{
			_document.Load(fileName);
		}

		public ConfigurationFileReader(XmlDocument document)
		{
			_document = document;
		}

		public IDictionary<string, string> GetSettingsValues(ConfigurationSectionPath path)
		{
			var values = new Dictionary<string, string>();

			if (_document.DocumentElement != null)
			{
				var element = _document.DocumentElement.SelectSingleNode(path) as XmlElement;
				if (element != null)
				{
					foreach (XmlElement setting in element.ChildNodes)
					{
						var nameAttribute = setting.Attributes["name"];
						if (nameAttribute == null)
							continue;

						var name = nameAttribute.Value;
						if (String.IsNullOrEmpty(name))
							continue;

						var valueNode = setting.SelectSingleNode("value");
						if (valueNode == null)
							continue;

						var serializeAsAttribute = setting.Attributes["serializeAs"];
						var serializeAsValue = serializeAsAttribute != null ? serializeAsAttribute.Value : String.Empty;
						var serializeAs = SystemConfigurationHelper.GetSerializeAs(serializeAsValue);
						values.Add(name, SystemConfigurationHelper.GetElementValue(valueNode, serializeAs));
					}
				}
			}

			return values;
		}

		public IDictionary<string, string> GetSettingsValues(Type settingsClass, SettingScope scope)
		{
			return GetSettingsValues(new ConfigurationSectionPath(settingsClass, scope));
		}

		public IDictionary<string, string> GetSettingsValues(Type settingsClass)
		{
			var appSettings = GetSettingsValues(settingsClass, SettingScope.Application);
			var userSettings = GetSettingsValues(settingsClass, SettingScope.User);
			foreach (var setting in userSettings)
				appSettings[setting.Key] = setting.Value;

			return appSettings;
		}
	}
}
