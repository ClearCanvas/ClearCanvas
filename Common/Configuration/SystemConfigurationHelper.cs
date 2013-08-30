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
using System.ComponentModel;
using System.Configuration;
using System.Reflection;
using System.Xml;
using SystemConfiguration = System.Configuration.Configuration;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Helper class that allows settings values for <see cref="ApplicationSettingsBase"/>-derived classes
	/// to be easily read/written to/from a <see cref="SystemConfiguration"/> object.
	/// </summary>
	public static class SystemConfigurationHelper
	{
		internal static SettingsSerializeAs GetSerializeAs(string serializeAs)
		{
			var converter = new EnumConverter(typeof (SettingsSerializeAs));
			if (!String.IsNullOrEmpty(serializeAs))
				return (SettingsSerializeAs) converter.ConvertFromInvariantString(serializeAs);

			return default(SettingsSerializeAs);
		}

		internal static string GetElementValue(XmlNode xmlNode, SettingsSerializeAs serializeAs)
		{
			return serializeAs == SettingsSerializeAs.Xml ? xmlNode.InnerXml : xmlNode.InnerText;
		}

		private static string GetElementValue(SettingElement element)
		{
			if (element.Value == null || element.Value.ValueXml == null)
				return String.Empty; //If the element even exists, we return "".

			return GetElementValue(element.Value.ValueXml, element.SerializeAs);
		}

		private static void SetElementValue(SettingElement element, string value)
		{
			var temp = new XmlDocument();
			XmlNode valueXml = temp.CreateElement("value");

			if (element.SerializeAs == SettingsSerializeAs.String)
			{
				XmlElement escaper = temp.CreateElement("temp");
				escaper.InnerText = value ?? "";
				valueXml.InnerXml = escaper.InnerXml;
			}
			else
			{
				if (!String.IsNullOrEmpty(value))
				{
					XmlElement tempElement = temp.CreateElement("temp");
					tempElement.InnerXml = value;
					RemoveXmlDeclaration(tempElement);
					valueXml.InnerXml = tempElement.InnerXml;
				}
			}

			element.Value.ValueXml = valueXml;
		}

		private static void RemoveXmlDeclaration(XmlElement element)
		{
			XmlNode declaration = element.FirstChild;
			while (declaration != null && declaration.NodeType != XmlNodeType.XmlDeclaration)
				declaration = declaration.NextSibling;

			if (declaration != null)
				element.RemoveChild(declaration);
		}

		private static ClientSettingsSection CastToClientSection(ConfigurationSection section)
		{
			var castToClientSection = section as ClientSettingsSection;
			if (castToClientSection != null)
				return castToClientSection;

			throw new NotSupportedException(String.Format(
				"The specified ConfigurationSection must be of Type ClientSettingsSection: {0}.", section.GetType().FullName));
		}

		private static SettingElement GetSettingElement(ClientSettingsSection clientSection, PropertyInfo property, bool create)
		{
			SettingElement element = clientSection.Settings.Get(property.Name);
			if (element == null && create)
			{
				element = new SettingElement(property.Name, SettingsClassMetaDataReader.GetSerializeAs(property));
				clientSection.Settings.Add(element);
			}

			return element;
		}

		private static bool UpdateSection(ClientSettingsSection section, IEnumerable<PropertyInfo> properties, IDictionary<string, string> newValues)
		{
			bool modified = false;

			foreach (PropertyInfo property in properties)
			{
				string newValue;
				if (!newValues.TryGetValue(property.Name, out newValue))
					continue;

				SettingElement element = GetSettingElement(section, property, false);
				if (newValue == null)
				{
					if (element != null)
					{
						section.Settings.Remove(element);
						modified = true;
					}

					continue;
				}

				if (element != null)
				{
					string currentValue = GetElementValue(element);
					if (Equals(newValue, currentValue))
						continue;
				}
				else
				{
					element = GetSettingElement(section, property, true);
				}

				modified = true;
				SetElementValue(element, newValue);
			}

			return modified;
		}

		private static bool UpdateSection(SystemConfiguration configuration, ConfigurationSectionPath sectionPath, IEnumerable<PropertyInfo> properties, IDictionary<string, string> newValues)
		{
			var section = sectionPath.GetSection(configuration);
			if (section != null)
				return UpdateSection(CastToClientSection(section), properties, newValues);

			var group = sectionPath.GroupPath.GetSectionGroup(configuration, true);
			section = sectionPath.CreateSection();

			if (!UpdateSection(CastToClientSection(section), properties, newValues))
				return false;

			group.Sections.Add(sectionPath.SectionName, section);
			return true;
		}

		private static Dictionary<string, string> GetSettingsValues(
			SystemConfiguration configuration,
			ConfigurationSectionPath sectionPath,
			ICollection<PropertyInfo> properties)
		{
			var values = new Dictionary<string, string>();
			if (properties.Count > 0)
			{
				var section = sectionPath.GetSection(configuration);
				if (section != null)
				{
					var clientSection = CastToClientSection(section);
					foreach (PropertyInfo property in properties)
					{
						SettingElement element = GetSettingElement(clientSection, property, false);
						if (element != null) //If the setting element is there, we'll assume it means the value is to be set.
							values[property.Name] = GetElementValue(element);
					}
				}
			}

			return values;
		}

		private static ICollection<PropertyInfo> GetProperties(Type settingsClass, SettingScope scope)
		{
			return SettingsClassMetaDataReader.GetSettingsProperties(settingsClass, scope);
		}

		/// <summary>
		/// Gets only those settings values that are different from the defaults for the given settings group.
		/// </summary>
		/// <param name="configuration">the configuration where the values will be taken from</param>
		/// <param name="settingsClass">the settings class for which to get the values</param>
		/// <param name="settingScope">the scope of the settings for which to get the values</param>
		public static Dictionary<string, string> GetSettingsValues(this SystemConfiguration configuration, Type settingsClass, SettingScope settingScope)
		{
			var properties = GetProperties(settingsClass, settingScope);
			var sectionPath = new ConfigurationSectionPath(settingsClass, settingScope);
			return GetSettingsValues(configuration, sectionPath, properties);
		}

		/// <summary>
		/// Gets only those settings values that are different from the defaults for the given settings group.
		/// </summary>
		/// <param name="configuration">the configuration where the values will be taken from</param>
		/// <param name="settingsClass">the settings class for which to get the values</param>
		public static Dictionary<string, string> GetSettingsValues(this SystemConfiguration configuration, Type settingsClass)
		{
			var applicationScopedValues = GetSettingsValues(configuration, settingsClass, SettingScope.Application);
			var userScopedValues = GetSettingsValues(configuration, settingsClass, SettingScope.User);

			foreach (KeyValuePair<string, string> userScopedValue in userScopedValues)
				applicationScopedValues[userScopedValue.Key] = userScopedValue.Value;

			return applicationScopedValues;
		}

		/// <summary>
		/// Stores the settings values for a given settings class.
		/// </summary>
		/// <param name="configuration">the configuration where the values will be stored</param>
		/// <param name="settingsClass">the settings class for which to store the values</param>
		/// <param name="dirtyValues">contains the values to be stored</param>
		public static void PutSettingsValues(this SystemConfiguration configuration, Type settingsClass, IDictionary<string, string> dirtyValues)
		{
			var applicationScopedProperties = GetProperties(settingsClass, SettingScope.Application);
			var userScopedProperties = GetProperties(settingsClass, SettingScope.User);

			bool modified = false;
			if (applicationScopedProperties.Count > 0)
			{
				var sectionPath = new ConfigurationSectionPath(settingsClass, SettingScope.Application);
				modified = UpdateSection(configuration, sectionPath, applicationScopedProperties, dirtyValues);
			}
			if (userScopedProperties.Count > 0)
			{
				var sectionPath = new ConfigurationSectionPath(settingsClass, SettingScope.User);
				if (UpdateSection(configuration, sectionPath, userScopedProperties, dirtyValues))
					modified = true;
			}

			if (modified)
				configuration.Save(ConfigurationSaveMode.Minimal, true);
		}

		public static void RemoveSettingsValues(this SystemConfiguration configuration, Type settingsClass, SettingScope? scope = null)
		{
			var removeApplicationSettings = !scope.HasValue || scope.Value == SettingScope.Application;
			var removeUserSettings = !scope.HasValue || scope.Value == SettingScope.User;

			if (removeApplicationSettings)
			{
				var sectionPath = new ConfigurationSectionPath(settingsClass, SettingScope.Application);
				ConfigurationSectionGroup group = configuration.GetSectionGroup(sectionPath.GroupPath);
				if (group != null)
					group.Sections.Remove(sectionPath.SectionName);
			}

			if (removeUserSettings)
			{
				var sectionPath = new ConfigurationSectionPath(settingsClass, SettingScope.User);
				var group = configuration.GetSectionGroup(sectionPath.GroupPath);
				if (group != null)
					group.Sections.Remove(sectionPath.SectionName);
			}

			configuration.Save(ConfigurationSaveMode.Minimal, true);
		}

		public static SystemConfiguration GetExeConfiguration()
		{
			return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		}

		public static SystemConfiguration GetExeConfiguration(string fileName)
		{
			var fileMap = new ExeConfigurationFileMap {ExeConfigFilename = fileName};
			return ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
		}
	}
}