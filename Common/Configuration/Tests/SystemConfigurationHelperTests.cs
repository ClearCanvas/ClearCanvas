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
        private static readonly Type[] settingsClasses = new[] { typeof(LocalMixedScopeSettings), typeof(ExtendedLocalMixedScopeSettings) };

        private static System.Configuration.Configuration GetExeConfiguration()
        {
            return SystemConfigurationHelper.GetExeConfiguration();
        }

        private static void RemoveSettings(Type settingsClass)
        {
            GetExeConfiguration().RemoveSettingsValues(settingsClass);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            foreach (var settingsClass in settingsClasses)
                RemoveSettings(settingsClass);
        }

		[Test]
		public void TestReadSettingsValues_NoneExist()
		{
            TearDown();
            foreach (var settingsClass in settingsClasses)
                Assert.AreEqual(0, GetExeConfiguration().GetSettingsValues(settingsClass).Count);
		}

		[Test]
		public void TestReadWriteValues_String()
		{
		    TearDown();
		    foreach (var settingsClass in settingsClasses)
		        TestReadWriteValues_String(settingsClass);
		}

	    [Test]
		public void TestReadWriteValues_Xml()
		{
            TestReadWriteValues_Xml(typeof(LocalXmlSettings));
            TestReadWriteValues_Xml(typeof(ExtendedLocalXmlSettings));
		}

	    [Test]
		public void TestWriteEmptyString_String()
		{
            TearDown();
		    foreach (var settingsClass in settingsClasses)
		        TestWriteEmptyString_String(settingsClass);
		}

	    [Test]
		public void TestWriteEmptyString_Xml()
		{
		    TestWriteEmptyString_Xml(typeof (LocalXmlSettings));
            TestWriteEmptyString_Xml(typeof(ExtendedLocalXmlSettings));
        }

	    [Test]
	    public void TestWriteNull_String()
	    {
	        TearDown();
	        foreach (var settingsClass in settingsClasses)
	            TestWriteNull_String(settingsClass);
	    }

	    [Test]
		public void TestWriteNull_Xml()
		{
			TestWriteNull_Xml(typeof (LocalXmlSettings));
			TestWriteNull_Xml(typeof (ExtendedLocalXmlSettings));
		}

	    [Test]
		public void TestRepeatedWritesDifferentProperties()
		{
            TearDown();
		    foreach (var settingsClass in settingsClasses)
		        TestRepeatedWritesDifferentProperties(settingsClass);
		}

	    private void TestReadWriteValues_String(Type settingsClass)
	    {
	        WriteSharedValuesToConfig(settingsClass, SettingValue.Default);
	        ValidateValuesInConfig(settingsClass, MigrationScope.Shared, SettingValue.Default);

	        WriteSharedValuesToConfig(settingsClass, SettingValue.Previous);
	        ValidateValuesInConfig(settingsClass, MigrationScope.Shared, SettingValue.Previous);

	        WriteSharedValuesToConfig(settingsClass, SettingValue.Current);
	        ValidateValuesInConfig(settingsClass, MigrationScope.Shared, SettingValue.Current);
	    }

	    private void TestReadWriteValues_Xml(Type settingsClass)
	    {
	        SystemConfigurationHelper.GetExeConfiguration().RemoveSettingsValues(settingsClass);

	        try
	        {
	            var values = GetExeConfiguration().GetSettingsValues(settingsClass);
	            Assert.AreEqual(0, values.Count);

	            values = new Dictionary<string, string>();
	            values[LocalXmlSettingsBase.PropertyUser] = LocalXmlSettingsBase.DefaultValueUser;
	            values[LocalXmlSettingsBase.PropertyApp] = LocalXmlSettingsBase.DefaultValueApp;

	            SystemConfigurationHelper.GetExeConfiguration().PutSettingsValues(settingsClass, values);
	            values = GetExeConfiguration().GetSettingsValues(settingsClass);
	            Assert.AreEqual(1, values.Count);

	            LocalXmlSettingsBase settings = (LocalXmlSettingsBase)ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
	            Assert.IsNull(settings.GetSharedPropertyValue(LocalXmlSettingsBase.PropertyUser));

	            XmlDocument defaultDoc = new XmlDocument();
	            defaultDoc.LoadXml(LocalXmlSettingsBase.DefaultValueApp);
	            Assert.AreEqual(defaultDoc.DocumentElement.OuterXml, settings.App.DocumentElement.OuterXml);
	        }
	        finally
	        {
	            SystemConfigurationHelper.GetExeConfiguration().RemoveSettingsValues(settingsClass);
	        }
	    }

	    private void TestWriteEmptyString_String(Type settingsClass)
	    {
	        var configuration = SystemConfigurationHelper.GetExeConfiguration();

	        var values = new Dictionary<string, string>();
	        values[MixedScopeSettingsBase.PropertyApp1] = "Test1";
	        values[MixedScopeSettingsBase.PropertyApp2] = "Test2";

	        configuration.PutSettingsValues(settingsClass, values);
	        values = GetExeConfiguration().GetSettingsValues(settingsClass);
	        Assert.AreEqual(2, values.Count);
	        Assert.AreEqual("Test1", values[MixedScopeSettingsBase.PropertyApp1]);
	        Assert.AreEqual("Test2", values[MixedScopeSettingsBase.PropertyApp2]);

	        values = new Dictionary<string, string>();
	        values[MixedScopeSettingsBase.PropertyApp1] = "";
	        values[MixedScopeSettingsBase.PropertyApp2] = "";

	        configuration.PutSettingsValues(settingsClass, values);
	        values = GetExeConfiguration().GetSettingsValues(settingsClass);
	        Assert.AreEqual(2, values.Count);
	        Assert.AreEqual("", values[MixedScopeSettingsBase.PropertyApp1]);
	        Assert.AreEqual("", values[MixedScopeSettingsBase.PropertyApp2]);

	        //For string values, empty string means empty string.
	        var settings = (MixedScopeSettingsBase)ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
	        Assert.AreEqual("", settings.App1);
	        Assert.AreEqual("", settings.App2);
	    }

	    private void TestWriteEmptyString_Xml(Type settingsClass)
	    {
	        SystemConfigurationHelper.GetExeConfiguration().RemoveSettingsValues(settingsClass);

	        try
	        {
	            var values = GetExeConfiguration().GetSettingsValues(settingsClass);
	            Assert.AreEqual(0, values.Count);

	            values = new Dictionary<string, string>();
	            values[LocalXmlSettingsBase.PropertyApp] = "";

	            SystemConfigurationHelper.GetExeConfiguration().PutSettingsValues(settingsClass, values);
	            values = GetExeConfiguration().GetSettingsValues(settingsClass);
	            Assert.AreEqual(1, values.Count);
	            Assert.AreEqual("", values[LocalXmlSettingsBase.PropertyApp]);

	            //For xml values, empty string means "default".
	            LocalXmlSettingsBase settings = (LocalXmlSettingsBase)ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
	            XmlDocument defaultDoc = new XmlDocument();
	            defaultDoc.LoadXml(LocalXmlSettingsBase.DefaultValueApp);
	            Assert.AreEqual(defaultDoc.DocumentElement.OuterXml, settings.App.DocumentElement.OuterXml);
	        }
	        finally
	        {
	            SystemConfigurationHelper.GetExeConfiguration().RemoveSettingsValues(settingsClass);
	        }
	    }

	    private void TestWriteNull_String(Type settingsClass)
	    {
	        var configuration = SystemConfigurationHelper.GetExeConfiguration();

	        var values = new Dictionary<string, string>();
	        values[MixedScopeSettingsBase.PropertyApp1] = "Test1";
	        values[MixedScopeSettingsBase.PropertyApp2] = "Test2";

	        configuration.PutSettingsValues(settingsClass, values);
	        values = GetExeConfiguration().GetSettingsValues(settingsClass);
	        Assert.AreEqual(2, values.Count);
	        Assert.AreEqual("Test1", values[MixedScopeSettingsBase.PropertyApp1]);
	        Assert.AreEqual("Test2", values[MixedScopeSettingsBase.PropertyApp2]);

	        values = new Dictionary<string, string>();
	        values[MixedScopeSettingsBase.PropertyApp1] = null;
	        values[MixedScopeSettingsBase.PropertyApp2] = null;

	        //writing null essentially means to reset it to the default, which is equivalent to removing it.
	        configuration.PutSettingsValues(settingsClass, values);
	        values = GetExeConfiguration().GetSettingsValues(settingsClass);
	        Assert.AreEqual(0, values.Count);

	        var settings = (MixedScopeSettingsBase)ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
	        Assert.AreEqual(MixedScopeSettingsBase.PropertyApp1, settings.App1);
	        Assert.AreEqual(MixedScopeSettingsBase.PropertyApp2, settings.App2);
	    }

	    private void TestWriteNull_Xml(Type settingsClass)
	    {
	        SystemConfigurationHelper.GetExeConfiguration().RemoveSettingsValues(settingsClass);

	        try
	        {
	            var values = GetExeConfiguration().GetSettingsValues(settingsClass);
	            Assert.AreEqual(0, values.Count);

	            values = new Dictionary<string, string>();
	            values[LocalXmlSettingsBase.PropertyApp] = null;

	            SystemConfigurationHelper.GetExeConfiguration().PutSettingsValues(settingsClass, values);
	            values = GetExeConfiguration().GetSettingsValues(settingsClass);
	            Assert.AreEqual(0, values.Count);

	            LocalXmlSettingsBase settings = (LocalXmlSettingsBase)ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
	            XmlDocument defaultDoc = new XmlDocument();
	            defaultDoc.LoadXml(LocalXmlSettingsBase.DefaultValueApp);
	            Assert.AreEqual(defaultDoc.DocumentElement.OuterXml, settings.App.DocumentElement.OuterXml);
	        }
	        finally
	        {
	            SystemConfigurationHelper.GetExeConfiguration().RemoveSettingsValues(settingsClass);
	        }
	    }

	    private void TestRepeatedWritesDifferentProperties(Type settingsClass)
	    {
	        var configuration = SystemConfigurationHelper.GetExeConfiguration();
	        var expectedValue1 = CreateSettingValue(MixedScopeSettingsBase.PropertyApp1, SettingValue.Current);
	        var expectedValue2 = CreateSettingValue(MixedScopeSettingsBase.PropertyApp2, SettingValue.Current);

	        var values = new Dictionary<string, string>();
	        values[MixedScopeSettingsBase.PropertyApp1] = expectedValue1;
	        configuration.PutSettingsValues(settingsClass, values);

	        values = configuration.GetSettingsValues(settingsClass);
	        Assert.AreEqual(1, values.Count);
	        Assert.AreEqual(expectedValue1, values[MixedScopeSettingsBase.PropertyApp1]);

	        values.Clear();
	        values[MixedScopeSettingsBase.PropertyApp2] = expectedValue2;
	        configuration.PutSettingsValues(settingsClass, values);

	        values = configuration.GetSettingsValues(settingsClass);
	        Assert.AreEqual(2, values.Count);
	        Assert.AreEqual(expectedValue1, values[MixedScopeSettingsBase.PropertyApp1]);
	        Assert.AreEqual(expectedValue2, values[MixedScopeSettingsBase.PropertyApp2]);
	    }

	    internal static void WriteSharedValuesToConfig(Type type, SettingValue settingValue)
		{
			var configuration = SystemConfigurationHelper.GetExeConfiguration();
			var values = CreateSettingsValues(type, MigrationScope.Shared, settingValue);
			configuration.PutSettingsValues(type, values);
		}

		private void ValidateValuesInConfig(Type settingsClass, MigrationScope migrationScope, SettingValue settingValue)
		{
			var configuration = GetExeConfiguration();
			var values = configuration.GetSettingsValues(settingsClass);
			ValidateValues(settingsClass, values, migrationScope, settingValue);
		}

        private void ValidateValues(Type settingsClass, Dictionary<string, string> values, MigrationScope migrationScope, SettingValue settingValue)
		{
			var expected = CreateSettingsValues(settingsClass, migrationScope, settingValue);
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