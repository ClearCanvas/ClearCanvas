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
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using System.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Server.ShredHost
{
	public class ShredSettingsMigrator
	{
		public static bool IsMigrating { get; private set; }

		private class ConfigurationSectionEntry
		{
			public ConfigurationSectionEntry(ConfigurationSectionGroupPath parentPath, ConfigurationSection section)
			{
				ParentPath = parentPath;
				Section = section;
			}

			public readonly ConfigurationSectionGroupPath ParentPath;
			public readonly ConfigurationSection Section;
		}

		private static readonly List<Type> _shredSettingsTypes = ListShredSettingsTypes();

		private static List<Type> ListShredSettingsTypes()
		{
			var types = new List<Type>();

			List<Assembly> assemblies = CollectionUtils.Map(Platform.PluginManager.Plugins, (PluginInfo p) => p.Assembly.Resolve());
			assemblies.Add(typeof(ShredSettingsMigrator).Assembly);
			foreach (var assembly in assemblies)
			{
				foreach (Type t in assembly.GetTypes())
				{
					if (t.IsSubclassOf(typeof(ConfigurationSection)) && !t.IsAbstract)
						types.Add(t);
				}
			}

			return types;
		}

		private static bool IsShredSettingsClass(ConfigurationSection section)
		{
			return _shredSettingsTypes.Contains(section.GetType());
		}

		private delegate ConfigurationSection ConfigurationSectionErrorHandler(ConfigurationSectionGroupPath groupPath, string sectionKey);

		private static IEnumerable<ConfigurationSectionEntry> GetConfigurationSections(Configuration configuration, ConfigurationSectionErrorHandler errorHandler)
		{
			ConfigurationSectionGroupPath rootPath = ConfigurationSectionGroupPath.Root;
			foreach (var childSection in GetConfigurationSections(configuration.RootSectionGroup, rootPath, errorHandler, true))
				yield return childSection;
		}

		private static IEnumerable<ConfigurationSectionEntry> GetConfigurationSections(ConfigurationSectionGroup group, ConfigurationSectionGroupPath groupPath, ConfigurationSectionErrorHandler errorHandler, bool recursive)
		{
			if (recursive)
			{
				foreach (ConfigurationSectionGroup childGroup in group.SectionGroups)
				{
					foreach (var sectionEntry in GetConfigurationSections(childGroup, groupPath.GetChildGroupPath(childGroup.Name), errorHandler, true))
						yield return sectionEntry;
				}
			}

			for (var n = 0; n < group.Sections.Count; ++n)
			{
				ConfigurationSection section = null;
				try
				{
					section = group.Sections[n];
				}
				catch (ConfigurationErrorsException)
				{
					if (errorHandler != null) section = errorHandler.Invoke(groupPath, group.Sections.Keys[n]);
				}
				if (section != null) yield return new ConfigurationSectionEntry(groupPath, section);
			}
		}

		private static void MigrateSection(ConfigurationSection sourceSection, ConfigurationSectionGroupPath groupPath, Configuration destinationConfiguration)
		{
			if (sourceSection.GetType().IsDefined(typeof(SharedSettingsMigrationDisabledAttribute), false))
				return; //disabled

			var destinationGroup = groupPath.GetSectionGroup(destinationConfiguration, true);

			var destinationSection = destinationGroup.Sections[sourceSection.SectionInformation.Name];
			if (destinationSection == null)
			{
				destinationSection = (ConfigurationSection)Activator.CreateInstance(sourceSection.GetType(), true);
				destinationGroup.Sections.Add(sourceSection.SectionInformation.Name, destinationSection);
			}

			var customMigrator = sourceSection as IMigrateSettings;
			foreach (PropertyInformation sourceProperty in sourceSection.ElementInformation.Properties)
			{
				var destinationProperty = destinationSection.ElementInformation.Properties[sourceProperty.Name];
				if (destinationProperty == null)
					continue;

				if (customMigrator != null)
				{
					var migrationValues = new SettingsPropertyMigrationValues(
						sourceProperty.Name, MigrationScope.Shared, destinationProperty.Value, sourceProperty.Value);

					customMigrator.MigrateSettingsProperty(migrationValues);
					if (!Equals(migrationValues.CurrentValue, destinationProperty.Value))
					{
						destinationSection.SectionInformation.ForceSave = true; 
						destinationProperty.Value = migrationValues.CurrentValue;
					}
				}
				else
				{
					destinationSection.SectionInformation.ForceSave = true;
					destinationProperty.Value = sourceProperty.Value;
				}
			}
		}

		public static void MigrateAll(string previousExeConfigFilename)
		{
			IsMigrating = true;

			try
			{
				Configuration previousConfiguration = SystemConfigurationHelper.GetExeConfiguration(previousExeConfigFilename);
				Configuration currentConfiguration = SystemConfigurationHelper.GetExeConfiguration();

				var legacySections = new List<IMigrateLegacyShredConfigSection>();
				foreach (var sectionEntry in GetConfigurationSections(previousConfiguration, (p, k) => HandleLegacyConfigurationSection(previousExeConfigFilename, p, k)))
				{
					// don't migrate legacy sections with the shred settings migration handler
					if (sectionEntry.Section is IMigrateLegacyShredConfigSection)
					{
						legacySections.Add((IMigrateLegacyShredConfigSection) sectionEntry.Section);
						continue;
					}

					if (IsShredSettingsClass(sectionEntry.Section))
						MigrateSection(sectionEntry.Section, sectionEntry.ParentPath, currentConfiguration);
				}

				currentConfiguration.Save(ConfigurationSaveMode.Full);

				// migrate all legacy sections
				foreach (var legacySection in legacySections) legacySection.Migrate();
			}
			finally
			{
				IsMigrating = false;
			}
		}

		private static ConfigurationSection HandleLegacyConfigurationSection(string exeConfigFilename, ConfigurationSectionGroupPath groupPath, string sectionKey)
		{
			var sectionPath = groupPath.GetChildGroupPath(sectionKey).ToString();
			foreach (var shredSettingsType in _shredSettingsTypes)
			{
				if (LegacyShredConfigSectionAttribute.IsMatchingLegacyShredConfigSectionType(shredSettingsType, sectionPath))
				{
					var xmlDocument = new ConfigXmlDocument();
					xmlDocument.Load(exeConfigFilename);

					var sectionElement = xmlDocument.SelectSingleNode("//" + sectionPath);
					if (sectionElement != null)
					{
						var section = (ShredConfigSection) Activator.CreateInstance(shredSettingsType, true);
						section.LoadXml(sectionElement.OuterXml);
						return section;
					}
				}
			}
			return null;
		}
	}
}
