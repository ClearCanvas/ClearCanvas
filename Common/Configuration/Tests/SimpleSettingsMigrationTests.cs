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

using NUnit.Framework;
using System;
using System.Configuration;

namespace ClearCanvas.Common.Configuration.Tests
{
	[TestFixture]
	public class SimpleSettingsMigrationTests : SettingsTestBase
	{
		[Test]
		public void TestSimpleUserSettingsMigration()
		{
			ResetUpgradeSettings();

			Type settingsClass = typeof (SimpleUserSettings);
			PopulateSimpleStore(settingsClass);
			Assert.IsTrue(SettingsMigrator.MigrateUserSettings(settingsClass));

			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);

			SettingsProperty property = settings.Properties[SimpleUserSettings.PropertyUser];
			string expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Previous);
			string actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, actual);

			expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Previous);
			Assert.IsFalse(SettingsMigrator.MigrateUserSettings(settingsClass));
			actual = (string)settings[property.Name]; 
			Assert.AreEqual(expected, actual);

			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestNonMigratableSharedSettings()
		{
			ResetUpgradeSettings();

			Type settingsClass = typeof(NonMigratableSharedSettings);
			PopulateSimpleStore(settingsClass);

			Assert.IsFalse(SettingsMigrator.MigrateSharedSettings(settingsClass, null));
			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);

			SettingsProperty property = settings.Properties[NonMigratableSharedSettings.PropertyApp];
			string expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			Assert.AreEqual(expected, settings[property.Name]);

			string current = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, current);
		}

		[Test]
		public void TestUserDefaultSettingsMigrated()
		{
			ResetUpgradeSettings(); 
			
			Type settingsClass = typeof(SimpleUserSettings);
			PopulateSimpleStore(settingsClass);
			
			Assert.IsTrue(SettingsMigrator.MigrateSharedSettings(settingsClass, null));

			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);

			SettingsProperty property = settings.Properties[SimpleUserSettings.PropertyUser];
			string expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Current);
			Assert.AreEqual(expected, settings[property.Name]);

			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Previous);
			string current = (string)settings.GetSharedPropertyValue(property.Name);
			Assert.AreEqual(expected, current);
		}

		[Test]
		public void TestNeverMigrateUpgradeSettings()
		{
			ResetUpgradeSettings();
			ResetAllSettingsClasses();
			Type settingsClass = typeof(UpgradeSettings);

			Assert.IsFalse(SettingsMigrator.MigrateUserSettings(settingsClass));
			Assert.IsFalse(SettingsMigrator.MigrateSharedSettings(settingsClass, null));
		}

		[Test]
		public void TestNeverMigrateProductSettings()
		{
			ResetUpgradeSettings();
			ResetAllSettingsClasses();
			Type settingsClass = typeof(ProductSettings);

			Assert.IsFalse(SettingsMigrator.MigrateUserSettings(settingsClass));
			Assert.IsFalse(SettingsMigrator.MigrateSharedSettings(settingsClass, null));
		}
	}
}

#endif