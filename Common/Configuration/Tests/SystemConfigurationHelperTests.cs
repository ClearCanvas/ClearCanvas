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
using System.Collections.Generic;
using SystemConfiguration = System.Configuration.Configuration;
using System.Xml;

namespace ClearCanvas.Common.Configuration.Tests
{
	[TestFixture]
	public class SystemConfigurationHelperTests : SettingsTestBase
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

		[TestFixtureTearDown]
		public void TearDown()
		{
			RemoveSettings();
		}

		[Test]
		public void TestReadSettingsValues_NoneExist()
		{
			RemoveSettings();
			Assert.AreEqual(0, SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), _settingsClass).Count);
		}

		[Test]
		public void TestReadWriteValues_String()
		{
			RemoveSettings();

			WriteSharedValuesToConfig(_settingsClass, SettingValue.Default);
			ValidateValuesInConfig(MigrationScope.Shared, SettingValue.Default);

			WriteSharedValuesToConfig(_settingsClass, SettingValue.Previous);
			ValidateValuesInConfig(MigrationScope.Shared, SettingValue.Previous);

			WriteSharedValuesToConfig(_settingsClass, SettingValue.Current);
			ValidateValuesInConfig(MigrationScope.Shared, SettingValue.Current);
		}

		[Test]
		public void TestReadWriteValues_Xml()
		{
			Type settingsClass = typeof(LocalXmlSettings);
			SystemConfigurationHelper.RemoveSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass);

			try
			{
				var values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), settingsClass);
				Assert.AreEqual(0, values.Count);

				values = new Dictionary<string, string>();
				values[LocalXmlSettings.PropertyUser] = LocalXmlSettings.DefaultValueUser;
				values[LocalXmlSettings.PropertyApp] = LocalXmlSettings.DefaultValueApp;

				SystemConfigurationHelper.PutSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass, values);
				values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), settingsClass);
				Assert.AreEqual(1, values.Count); 

				LocalXmlSettings settings = (LocalXmlSettings)ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
				Assert.IsNull(ApplicationSettingsExtensions.GetSharedPropertyValue(settings, LocalXmlSettings.PropertyUser));

				XmlDocument defaultDoc = new XmlDocument();
				defaultDoc.LoadXml(LocalXmlSettings.DefaultValueApp);
				Assert.AreEqual(defaultDoc.DocumentElement.OuterXml, settings.App.DocumentElement.OuterXml);
			}
			finally
			{
				SystemConfigurationHelper.RemoveSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass);
			}
		}

		[Test]
		public void TestWriteEmptyString_String()
		{
			RemoveSettings();
			var configuration = SystemConfigurationHelper.GetExeConfiguration();

			var values = new Dictionary<string, string>();
			values[LocalMixedScopeSettings.PropertyApp1] = "Test1";
			values[LocalMixedScopeSettings.PropertyApp2] = "Test2";

			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);
			values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), _settingsClass);
			Assert.AreEqual(2, values.Count);
			Assert.AreEqual("Test1", values[LocalMixedScopeSettings.PropertyApp1]);
			Assert.AreEqual("Test2", values[LocalMixedScopeSettings.PropertyApp2]);

			values = new Dictionary<string, string>();
			values[LocalMixedScopeSettings.PropertyApp1] = "";
			values[LocalMixedScopeSettings.PropertyApp2] = "";

			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);
			values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), _settingsClass);
			Assert.AreEqual(2, values.Count);
			Assert.AreEqual("", values[LocalMixedScopeSettings.PropertyApp1]);
			Assert.AreEqual("", values[LocalMixedScopeSettings.PropertyApp2]);

			//For string values, empty string means empty string.
			var settings = (LocalMixedScopeSettings)ApplicationSettingsHelper.GetSettingsClassInstance(_settingsClass);
			Assert.AreEqual("", settings.App1);
			Assert.AreEqual("", settings.App2);
		}

		[Test]
		public void TestWriteNull_String()
		{
			RemoveSettings();
			var configuration = SystemConfigurationHelper.GetExeConfiguration();

			var values = new Dictionary<string, string>();
			values[LocalMixedScopeSettings.PropertyApp1] = "Test1";
			values[LocalMixedScopeSettings.PropertyApp2] = "Test2";

			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);
			values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), _settingsClass);
			Assert.AreEqual(2, values.Count);
			Assert.AreEqual("Test1", values[LocalMixedScopeSettings.PropertyApp1]);
			Assert.AreEqual("Test2", values[LocalMixedScopeSettings.PropertyApp2]);

			values = new Dictionary<string, string>();
			values[LocalMixedScopeSettings.PropertyApp1] = null;
			values[LocalMixedScopeSettings.PropertyApp2] = null;

			//writing null essentially means to reset it to the default, which is equivalent to removing it.
			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);
			values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), _settingsClass);
			Assert.AreEqual(0, values.Count);

			var settings = (LocalMixedScopeSettings)ApplicationSettingsHelper.GetSettingsClassInstance(_settingsClass);
			Assert.AreEqual(LocalMixedScopeSettings.PropertyApp1, settings.App1);
			Assert.AreEqual(LocalMixedScopeSettings.PropertyApp2, settings.App2);
		}

		[Test]
		public void TestWriteEmptyString_Xml()
		{
			Type settingsClass = typeof(LocalXmlSettings);
			SystemConfigurationHelper.RemoveSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass);

			try
			{
				var values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), settingsClass);
				Assert.AreEqual(0, values.Count);

				values = new Dictionary<string, string>();
				values[LocalXmlSettings.PropertyApp] = "";

				SystemConfigurationHelper.PutSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass, values);
				values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), settingsClass);
				Assert.AreEqual(1, values.Count);
				Assert.AreEqual("", values[LocalXmlSettings.PropertyApp]);

				//For xml values, empty string means "default".
				LocalXmlSettings settings = (LocalXmlSettings)ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
				XmlDocument defaultDoc = new XmlDocument();
				defaultDoc.LoadXml(LocalXmlSettings.DefaultValueApp);
				Assert.AreEqual(defaultDoc.DocumentElement.OuterXml, settings.App.DocumentElement.OuterXml);
			}
			finally
			{
				SystemConfigurationHelper.RemoveSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass);
			}
		}

		[Test]
		public void TestWriteNull_Xml()
		{
			Type settingsClass = typeof (LocalXmlSettings);
			SystemConfigurationHelper.RemoveSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass);

			try
			{
				var values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), settingsClass);
				Assert.AreEqual(0, values.Count);

				values = new Dictionary<string, string>();
				values[LocalXmlSettings.PropertyApp] = null;

				SystemConfigurationHelper.PutSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass, values);
				values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), settingsClass);
				Assert.AreEqual(0, values.Count);

				LocalXmlSettings settings = (LocalXmlSettings)ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
				XmlDocument defaultDoc = new XmlDocument();
				defaultDoc.LoadXml(LocalXmlSettings.DefaultValueApp);
				Assert.AreEqual(defaultDoc.DocumentElement.OuterXml, settings.App.DocumentElement.OuterXml);
			}
			finally
			{
				SystemConfigurationHelper.RemoveSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass);
			}
		}

		[Test]
		public void TestRepeatedWritesDifferentProperties()
		{
			RemoveSettings();
			var configuration = SystemConfigurationHelper.GetExeConfiguration();
			var expectedValue1 = CreateSettingValue(LocalMixedScopeSettings.PropertyApp1, SettingValue.Current);
			var expectedValue2 = CreateSettingValue(LocalMixedScopeSettings.PropertyApp2, SettingValue.Current);

			var values = new Dictionary<string, string>();
			values[LocalMixedScopeSettings.PropertyApp1] = expectedValue1;
			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);

			values = SystemConfigurationHelper.GetSettingsValues(configuration, _settingsClass);
			Assert.AreEqual(1, values.Count);
			Assert.AreEqual(expectedValue1, values[LocalMixedScopeSettings.PropertyApp1]);

			values.Clear();
			values[LocalMixedScopeSettings.PropertyApp2] = expectedValue2;
			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);

			values = SystemConfigurationHelper.GetSettingsValues(configuration, _settingsClass);
			Assert.AreEqual(2, values.Count);
			Assert.AreEqual(expectedValue1, values[LocalMixedScopeSettings.PropertyApp1]);
			Assert.AreEqual(expectedValue2, values[LocalMixedScopeSettings.PropertyApp2]);
		}

		internal static void WriteSharedValuesToConfig(Type type, SettingValue settingValue)
		{
			var configuration = SystemConfigurationHelper.GetExeConfiguration();
			var values = CreateSettingsValues(type, MigrationScope.Shared, settingValue);
			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);
		}

		private void ValidateValuesInConfig(MigrationScope migrationScope, SettingValue settingValue)
		{
			var configuration = GetExeConfiguration();
			var values = SystemConfigurationHelper.GetSettingsValues(configuration, _settingsClass);
			ValidateValues(values, migrationScope, settingValue);
		}

		private void ValidateValues(Dictionary<string, string> values, MigrationScope migrationScope, SettingValue settingValue)
		{
			var expected = CreateSettingsValues(_settingsClass, migrationScope, settingValue);
			ValidateValues(values, expected);
		}

		private void ValidateValues(Dictionary<string, string> values, Dictionary<string, string> expected)
		{
			Assert.AreEqual(expected.Count, values.Count);

			foreach (var value in expected)
				Assert.AreEqual(value.Value, values[value.Key]);
		}
	}
}

#endif