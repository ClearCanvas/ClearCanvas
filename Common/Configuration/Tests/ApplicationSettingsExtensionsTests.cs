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
using NUnit.Framework;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace ClearCanvas.Common.Configuration.Tests
{
	[TestFixture]
	public class ApplicationSettingsExtensionsTests : SettingsTestBase
	{
		private static readonly Type _settingsClass = typeof(LocalMixedScopeSettings);

		private static System.Configuration.Configuration GetExeConfiguration()
		{
			return SystemConfigurationHelper.GetExeConfiguration();
		}

		private static void RemoveSettings()
		{
			SystemConfigurationHelper.RemoveSettingsValues(GetExeConfiguration(), _settingsClass);
		}

		[TestFixtureSetUp]
		public void Setup()
		{
			ResetAllSettingsClasses();
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			RemoveSettings();
		}

		[Test]
		public void TestGetSharedSettings_NoneExist()
		{
			RemoveSettings();

			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(_settingsClass);
			settings.Reload();

			foreach (SettingsProperty property in settings.Properties)
			{
				var shared = ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
				Assert.AreEqual(property.DefaultValue, shared);

				if (SettingsPropertyExtensions.IsAppScoped(property))
					Assert.AreEqual(property.DefaultValue, settings[property.Name]);
			}
		}

		[Test]
		public void TestGetSharedSettings_Exists()
		{
			RemoveSettings();

			SystemConfigurationHelperTests.WriteSharedValuesToConfig(_settingsClass, SettingValue.Current);
			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(_settingsClass);
			settings.Reload();

			foreach (SettingsProperty property in settings.Properties)
			{
				var shared = ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
				string expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
				Assert.AreEqual(expected, shared);

				if (SettingsPropertyExtensions.IsAppScoped(property))
					Assert.AreEqual(expected, settings[property.Name]);
			}
		}

		[Test]
		public void TestGetPreviousSharedValues_NoneExist()
		{
			ResetAllSettingsClasses();

			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(_settingsClass);
			settings.Reload();
			foreach (SettingsProperty property in settings.Properties)
			{
				var previous = ApplicationSettingsExtensions.GetPreviousSharedPropertyValue(settings, property.Name, null);
				Assert.IsNull(previous);
			}
		}

		[Test]
		public void TestGetPreviousSharedValues_Exists()
		{
			ResetAllSettingsClasses();

			string path = Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath);
			string fileName = String.Format("{0}{1}TestPrevious.exe.config", path, System.IO.Path.DirectorySeparatorChar);
			TestConfigResourceToFile(fileName);

			try
			{
				var settings = ApplicationSettingsHelper.GetSettingsClassInstance(_settingsClass);
				settings.Reload();
				foreach (SettingsProperty property in settings.Properties)
				{
					var actual = ApplicationSettingsExtensions.GetPreviousSharedPropertyValue(settings, property.Name, fileName);
					var expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Previous);
					Assert.AreEqual(expected, actual);
				}
			}
			finally
			{
				File.Delete(fileName);
			}
		}
	}
}

#endif