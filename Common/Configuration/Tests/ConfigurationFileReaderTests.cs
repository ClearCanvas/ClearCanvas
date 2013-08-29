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

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace ClearCanvas.Common.Configuration.Tests
{
	[TestFixture]
	public class ConfigurationFileReaderTests : SettingsTestBase
	{
		[Test]
		public void TestReadNoSettings()
		{
		    TestReadNoSettings(typeof (LocalMixedScopeSettings));
            TestReadNoSettings(typeof(ExtendedLocalMixedScopeSettings));
		}

        [Test]
        public void TestReadStringSettings()
        {
            TestReadStringSettings(typeof(LocalMixedScopeSettings));
            TestReadStringSettings(typeof(ExtendedLocalMixedScopeSettings));
        }

        [Test]
        public void TestReadXmlSettings()
        {
            TestReadXmlSettings(typeof(LocalXmlSettings));
            TestReadXmlSettings(typeof(ExtendedLocalXmlSettings));
        }

        private static void TestReadNoSettings(Type localSettingsClass)
        {
			SystemConfigurationHelper.GetExeConfiguration().RemoveSettingsValues(localSettingsClass);

			var reader = new ConfigurationFileReader(SystemConfigurationHelper.GetExeConfiguration().FilePath);
			var path = new ConfigurationSectionPath(localSettingsClass, SettingScope.Application);
			var values = reader.GetSettingsValues(path);
			Assert.AreEqual(0, values.Count);

            path = new ConfigurationSectionPath(localSettingsClass, SettingScope.User);
			values = reader.GetSettingsValues(path);
			Assert.AreEqual(0, values.Count);
		}

		public void TestReadStringSettings(Type localSettingsClass)
        {
			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(localSettingsClass);

			settings.SetSharedPropertyValue(MixedScopeSettingsBase.PropertyApp1, "TestApp1");
            settings.SetSharedPropertyValue(MixedScopeSettingsBase.PropertyApp2, "TestApp2");
            settings.SetSharedPropertyValue(MixedScopeSettingsBase.PropertyUser1, "TestUser1");
            settings.SetSharedPropertyValue(MixedScopeSettingsBase.PropertyUser2, "TestUser2");

			var reader = new ConfigurationFileReader(SystemConfigurationHelper.GetExeConfiguration().FilePath);
			var path = new ConfigurationSectionPath(localSettingsClass, SettingScope.Application);
			var values = reader.GetSettingsValues(path);
			Assert.AreEqual(2, values.Count);
            Assert.AreEqual("TestApp1", values[MixedScopeSettingsBase.PropertyApp1]);
            Assert.AreEqual("TestApp2", values[MixedScopeSettingsBase.PropertyApp2]);

			path = new ConfigurationSectionPath(localSettingsClass, SettingScope.User);
			values = reader.GetSettingsValues(path);
			Assert.AreEqual(2, values.Count);
            Assert.AreEqual("TestUser1", values[MixedScopeSettingsBase.PropertyUser1]);
            Assert.AreEqual("TestUser2", values[MixedScopeSettingsBase.PropertyUser2]);
			
			SystemConfigurationHelper.GetExeConfiguration().RemoveSettingsValues(localSettingsClass);
		}

        private static void TestReadXmlSettings(Type localXmlSettingsClass)
        {
			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(localXmlSettingsClass);

			var appValue = @"<test><app/></test>";
			XmlDocument appDocument = new XmlDocument();
			appDocument.LoadXml(appValue);
            settings.SetSharedPropertyValue(LocalXmlSettingsBase.PropertyApp, appDocument);

			var userValue = @"<test><user/></test>";
			XmlDocument userDocument= new XmlDocument();
			userDocument.LoadXml(userValue);
            settings.SetSharedPropertyValue(LocalXmlSettingsBase.PropertyUser, userDocument);

			var reader = new ConfigurationFileReader(SystemConfigurationHelper.GetExeConfiguration().FilePath);
            var path = new ConfigurationSectionPath(localXmlSettingsClass, SettingScope.Application);
			var values = reader.GetSettingsValues(path);
			Assert.AreEqual(1, values.Count);

			XmlDocument testDocument = new XmlDocument();
            testDocument.LoadXml(values[LocalXmlSettingsBase.PropertyApp]);
			Assert.AreEqual(appDocument.InnerXml, testDocument.InnerXml);

            path = new ConfigurationSectionPath(localXmlSettingsClass, SettingScope.User);
			values = reader.GetSettingsValues(path);
			Assert.AreEqual(1, values.Count);

			testDocument = new XmlDocument();
            testDocument.LoadXml(values[LocalXmlSettingsBase.PropertyUser]);
			Assert.AreEqual(userDocument.InnerXml, testDocument.InnerXml);

			SystemConfigurationHelper.GetExeConfiguration().RemoveSettingsValues(localXmlSettingsClass);
		}
	}
}

#endif