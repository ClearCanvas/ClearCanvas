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

#if	UNIT_TESTS
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Configuration;
using System.IO;
using System.Xml;

namespace ClearCanvas.Common.Configuration.Tests
{
	[TestFixture]
	public class SettingsMigrationTests : SettingsTestBase
	{
		private void ValidateLocalMixedScopeSettingsValuesInConfig(System.Configuration.Configuration configuration, SettingValue settingValue)
		{
			Type settingsClass = typeof (LocalMixedScopeSettings);
			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
			settings.Reload();

			SettingsProperty property = settings.Properties[LocalMixedScopeSettings.PropertyApp1];
			string expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
			string actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);

			property = settings.Properties[LocalMixedScopeSettings.PropertyApp2];
			expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);

			property = settings.Properties[LocalMixedScopeSettings.PropertyUser1];
			expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);

			property = settings.Properties[LocalMixedScopeSettings.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);

			var values = SystemConfigurationHelper.GetSettingsValues(configuration, settingsClass, SettingScope.Application);
			Assert.AreEqual(2, values.Count);
			property = settings.Properties[LocalMixedScopeSettings.PropertyApp1];
			expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
			actual = values[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[LocalMixedScopeSettings.PropertyApp2];
			expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
			actual = values[property.Name];
			Assert.AreEqual(expected, actual);

			values = SystemConfigurationHelper.GetSettingsValues(configuration, settingsClass, SettingScope.User);
			Assert.AreEqual(2, values.Count);
			property = settings.Properties[LocalMixedScopeSettings.PropertyUser1];
			expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
			actual = values[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[LocalMixedScopeSettings.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
			actual = values[property.Name];
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestLocalSharedSettingsMigration()
		{
			Type settingsClass = typeof(LocalMixedScopeSettings);
			var configuration = SystemConfigurationHelper.GetExeConfiguration();
			var values = CreateSettingsValues(settingsClass, MigrationScope.Shared, SettingValue.Current);
			SystemConfigurationHelper.PutSettingsValues(configuration, settingsClass, values);

			string directory = Path.GetDirectoryName(configuration.FilePath);
			string previousExeFilename = String.Format("{0}{1}Previous.exe.config", directory, Path.DirectorySeparatorChar);

			try
			{
				TestConfigResourceToFile(previousExeFilename);

				ValidateLocalMixedScopeSettingsValuesInConfig(configuration, SettingValue.Current);
				SettingsMigrator.MigrateSharedSettings(settingsClass, previousExeFilename);
				configuration = SystemConfigurationHelper.GetExeConfiguration();
				ValidateLocalMixedScopeSettingsValuesInConfig(configuration, SettingValue.Previous);
			}
			finally
			{
				File.Delete(previousExeFilename);
				SystemConfigurationHelper.RemoveSettingsValues(configuration, settingsClass);
			}
		}

		[Test]
		public void TestMigrateXmlSettings()
		{
			Type settingsClass = typeof(LocalXmlSettings);
			var configuration = SystemConfigurationHelper.GetExeConfiguration();

			var settings = (LocalXmlSettings)ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
			XmlDocument document = new XmlDocument();
			document.LoadXml((string)settings.Properties[LocalXmlSettings.PropertyApp].DefaultValue);
			var node = document.SelectSingleNode("//test");
			node.InnerText = "CurrentApp";
            Dictionary<string, string> values = new Dictionary<string, string>();
			values[LocalXmlSettings.PropertyApp] = document.InnerXml;
			SystemConfigurationHelper.PutSettingsValues(configuration, settingsClass, values);

			string directory = Path.GetDirectoryName(configuration.FilePath);
			string previousExeFilename = String.Format("{0}{1}Previous.exe.config", directory, Path.DirectorySeparatorChar);

			try
			{
				TestConfigResourceToFile(previousExeFilename);
				SettingsMigrator.MigrateSharedSettings(settingsClass, previousExeFilename);
				configuration = SystemConfigurationHelper.GetExeConfiguration();
				settings = (LocalXmlSettings)ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
				settings.Reload();
				document = (XmlDocument)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, LocalXmlSettings.PropertyApp);
				Assert.AreEqual("PreviousApp", document.SelectSingleNode("//test").InnerXml);
				document = settings.App;
				Assert.AreEqual("PreviousApp", document.SelectSingleNode("//test").InnerXml);
			}
			finally
			{
				File.Delete(previousExeFilename);
				SystemConfigurationHelper.RemoveSettingsValues(configuration, settingsClass);
			}
		}

		[Test]
		public void TestMultipleUserSettingsMigration1()
		{
			TestMultipleUserSettingsMigration(typeof(MixedScopeSettings));
		}

		[Test]
		public void TestMultipleUserSettingsMigration2()
		{
			TestMultipleUserSettingsMigration(typeof(InstanceMixedScopeSettings));
		}

		[Test]
		public void TestMultipleSharedSettingsMigration1()
		{
			TestMultipleSharedSettingsMigrated(typeof(MixedScopeSettings));
		}

		[Test]
		public void TestMultipleSharedSettingsMigration2()
		{
			TestMultipleSharedSettingsMigrated(typeof(InstanceMixedScopeSettings));
		}

		[Test]
		public void TestCustomUserSettingsMigration()
		{
			ResetSimpleStore();
			ResetAllSettingsClasses();

			Type settingsClass = typeof (CustomMigrationMixedScopeSettings);
			PopulateSimpleStore(settingsClass);

			SettingsMigrator.MigrateUserSettings(settingsClass);
			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);

			SettingsProperty property = settings.Properties[CustomMigrationMixedScopeSettings.PropertyUser1];
			string expected = "CustomUser1";
			string actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[CustomMigrationMixedScopeSettings.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Previous);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[CustomMigrationMixedScopeSettings.PropertyApp1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[CustomMigrationMixedScopeSettings.PropertyApp2];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[CustomMigrationMixedScopeSettings.PropertyUser1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);

			property = settings.Properties[CustomMigrationMixedScopeSettings.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestCustomSharedSettingsMigration()
		{
			ResetSimpleStore();
			ResetAllSettingsClasses();

			Type settingsClass = typeof(CustomMigrationMixedScopeSettings);
			PopulateSimpleStore(settingsClass);

			SettingsMigrator.MigrateSharedSettings(settingsClass, null);
			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);

			SettingsProperty property = settings.Properties[CustomMigrationMixedScopeSettings.PropertyUser1];
			string expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Current);
			string actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[CustomMigrationMixedScopeSettings.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Current);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[CustomMigrationMixedScopeSettings.PropertyApp1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[CustomMigrationMixedScopeSettings.PropertyApp2];
			expected = "CustomApp2";
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[CustomMigrationMixedScopeSettings.PropertyUser1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Previous);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);

			property = settings.Properties[CustomMigrationMixedScopeSettings.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);
		}

		private static void TestMultipleUserSettingsMigration(Type mixedScopeSettingsClass)
		{
			if (!mixedScopeSettingsClass.IsSubclassOf(typeof(MixedScopeSettingsBase)))
				throw new ArgumentException();

			ResetSimpleStore();
			ResetAllSettingsClasses();

			PopulateSimpleStore(mixedScopeSettingsClass);

			SettingsMigrator.MigrateUserSettings(mixedScopeSettingsClass);
			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(mixedScopeSettingsClass);

			SettingsProperty property = settings.Properties[MixedScopeSettingsBase.PropertyUser1];
			string expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Previous);
			string actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Previous);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyApp1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string) settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyApp2];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyUser1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);
		}

		private static void TestMultipleSharedSettingsMigrated(Type mixedScopeSettingsClass)
		{
			if (!mixedScopeSettingsClass.IsSubclassOf(typeof(MixedScopeSettingsBase)))
				throw new ArgumentException();
			
			ResetSimpleStore();
			ResetAllSettingsClasses();

			PopulateSimpleStore(mixedScopeSettingsClass);

			SettingsMigrator.MigrateSharedSettings(mixedScopeSettingsClass, null);
			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(mixedScopeSettingsClass);

			SettingsProperty property = settings.Properties[MixedScopeSettingsBase.PropertyUser1];
			string expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Current);
			string actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Current);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyApp1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Previous);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyApp2];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Previous);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyUser1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Previous);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Previous);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);
		}
	}
}

#endif