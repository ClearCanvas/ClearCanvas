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
using System.Configuration;
using System.IO;
using System.Xml;
using NUnit.Framework;

namespace ClearCanvas.Common.Configuration.Tests
{
	[TestFixture]
	public class SettingsMigrationTests : SettingsTestBase
	{
	    [Test]
		public void TestLocalSharedSettingsMigration()
		{
            TestLocalSharedSettingsMigration(typeof(LocalMixedScopeSettings));
            TestLocalSharedSettingsMigration(typeof(ExtendedLocalMixedScopeSettings));
        }

	    [Test]
		public void TestMigrateXmlSettings()
		{
            TestMigrateXmlSettings(typeof(LocalXmlSettings));
            TestMigrateXmlSettings(typeof(ExtendedLocalXmlSettings));
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

			SettingsProperty property = settings.Properties[MixedScopeSettingsBase.PropertyUser1];
			string expected = "CustomUser1";
			var actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

            property = settings.Properties[MixedScopeSettingsBase.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Previous);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

            property = settings.Properties[MixedScopeSettingsBase.PropertyApp1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

            property = settings.Properties[MixedScopeSettingsBase.PropertyApp2];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

            property = settings.Properties[MixedScopeSettingsBase.PropertyUser1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, actual);

            property = settings.Properties[MixedScopeSettingsBase.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
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

            SettingsProperty property = settings.Properties[MixedScopeSettingsBase.PropertyUser1];
			string expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Current);
			var actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

            property = settings.Properties[MixedScopeSettingsBase.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Current);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

            property = settings.Properties[MixedScopeSettingsBase.PropertyApp1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

            property = settings.Properties[MixedScopeSettingsBase.PropertyApp2];
			expected = "CustomApp2";
			actual = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

            property = settings.Properties[MixedScopeSettingsBase.PropertyUser1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Previous);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, actual);

            property = settings.Properties[MixedScopeSettingsBase.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
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
			var actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Previous);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyApp1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string) settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyApp2];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyUser1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
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
			var actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Current);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyApp1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Previous);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyApp2];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Previous);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, actual);
			actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyUser1];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Previous);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, actual);

			property = settings.Properties[MixedScopeSettingsBase.PropertyUser2];
			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Previous);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, actual);
		}

	    private static void ValidateLocalMixedScopeSettingsValuesInConfig(Type mixedScopeSettingsClass, System.Configuration.Configuration configuration, SettingValue settingValue)
	    {
            if (!mixedScopeSettingsClass.IsSubclassOf(typeof(MixedScopeSettingsBase)))
                throw new ArgumentException();

            var settings = ApplicationSettingsHelper.GetSettingsClassInstance(mixedScopeSettingsClass);
	        settings.Reload();

            SettingsProperty property = settings.Properties[MixedScopeSettingsBase.PropertyApp1];
	        string expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
	        var actual = (string)settings[property.Name];
	        Assert.AreEqual(expected, actual);
	        actual = (string)settings.GetSharedPropertyValue(property.Name);
	        Assert.AreEqual(expected, actual);

            property = settings.Properties[MixedScopeSettingsBase.PropertyApp2];
	        expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
	        actual = (string)settings[property.Name];
	        Assert.AreEqual(expected, actual);
	        actual = (string)settings.GetSharedPropertyValue(property.Name);
	        Assert.AreEqual(expected, actual);

            property = settings.Properties[MixedScopeSettingsBase.PropertyUser1];
	        expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
	        actual = (string)settings.GetSharedPropertyValue(property.Name);
	        Assert.AreEqual(expected, actual);

            property = settings.Properties[MixedScopeSettingsBase.PropertyUser2];
	        expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
	        actual = (string)settings.GetSharedPropertyValue(property.Name);
	        Assert.AreEqual(expected, actual);

	        var values = configuration.GetSettingsValues(mixedScopeSettingsClass, SettingScope.Application);
	        Assert.AreEqual(2, values.Count);
            property = settings.Properties[MixedScopeSettingsBase.PropertyApp1];
	        expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
	        actual = values[property.Name];
	        Assert.AreEqual(expected, actual);

            property = settings.Properties[MixedScopeSettingsBase.PropertyApp2];
	        expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
	        actual = values[property.Name];
	        Assert.AreEqual(expected, actual);

	        values = configuration.GetSettingsValues(mixedScopeSettingsClass, SettingScope.User);
	        Assert.AreEqual(2, values.Count);
            property = settings.Properties[MixedScopeSettingsBase.PropertyUser1];
	        expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
	        actual = values[property.Name];
	        Assert.AreEqual(expected, actual);

            property = settings.Properties[MixedScopeSettingsBase.PropertyUser2];
	        expected = CreateSettingValue(property, MigrationScope.Shared, settingValue);
	        actual = values[property.Name];
	        Assert.AreEqual(expected, actual);
	    }

        private static void TestLocalSharedSettingsMigration(Type mixedScopeSettingsClass)
	    {
            if (!mixedScopeSettingsClass.IsSubclassOf(typeof(MixedScopeSettingsBase)))
                throw new ArgumentException();

	        var configuration = SystemConfigurationHelper.GetExeConfiguration();
	        var values = CreateSettingsValues(mixedScopeSettingsClass, MigrationScope.Shared, SettingValue.Current);
	        configuration.PutSettingsValues(mixedScopeSettingsClass, values);

	        string directory = Path.GetDirectoryName(configuration.FilePath);
	        string previousExeFilename = String.Format("{0}{1}Previous.exe.config", directory, Path.DirectorySeparatorChar);

	        try
	        {
	            TestConfigResourceToFile(previousExeFilename);

	            ValidateLocalMixedScopeSettingsValuesInConfig(mixedScopeSettingsClass, configuration, SettingValue.Current);
	            SettingsMigrator.MigrateSharedSettings(mixedScopeSettingsClass, previousExeFilename);
	            configuration = SystemConfigurationHelper.GetExeConfiguration();
	            ValidateLocalMixedScopeSettingsValuesInConfig(mixedScopeSettingsClass, configuration, SettingValue.Previous);
	        }
	        finally
	        {
	            File.Delete(previousExeFilename);
	            configuration.RemoveSettingsValues(mixedScopeSettingsClass);
	        }
	    }

        private static void TestMigrateXmlSettings(Type xmlSettingsClass)
	    {
            if (!xmlSettingsClass.IsSubclassOf(typeof(LocalXmlSettingsBase)))
                throw new ArgumentException();

	        var configuration = SystemConfigurationHelper.GetExeConfiguration();

	        var settings = (LocalXmlSettingsBase)ApplicationSettingsHelper.GetSettingsClassInstance(xmlSettingsClass);
	        var document = new XmlDocument();
	        document.LoadXml((string)settings.Properties[LocalXmlSettingsBase.PropertyApp].DefaultValue);
	        var node = document.SelectSingleNode("//test");
	        node.InnerText = "CurrentApp";
	        var values = new Dictionary<string, string>();
            values[LocalXmlSettingsBase.PropertyApp] = document.InnerXml;
	        configuration.PutSettingsValues(xmlSettingsClass, values);

	        string directory = Path.GetDirectoryName(configuration.FilePath);
	        string previousExeFilename = String.Format("{0}{1}Previous.exe.config", directory, Path.DirectorySeparatorChar);

	        try
	        {
	            TestConfigResourceToFile(previousExeFilename);
	            SettingsMigrator.MigrateSharedSettings(xmlSettingsClass, previousExeFilename);
	            configuration = SystemConfigurationHelper.GetExeConfiguration();
                settings = (LocalXmlSettingsBase)ApplicationSettingsHelper.GetSettingsClassInstance(xmlSettingsClass);
	            settings.Reload();
	            document = (XmlDocument)settings.GetSharedPropertyValue(LocalXmlSettingsBase.PropertyApp);
	            Assert.AreEqual("PreviousApp", document.SelectSingleNode("//test").InnerXml);
	            document = settings.App;
	            Assert.AreEqual("PreviousApp", document.SelectSingleNode("//test").InnerXml);
	        }
	        finally
	        {
	            File.Delete(previousExeFilename);
	            configuration.RemoveSettingsValues(xmlSettingsClass);
	        }
	    }
	}
}

#endif