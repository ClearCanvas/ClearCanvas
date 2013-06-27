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
	    private static readonly Type[] _settingsClasses = new[] {typeof (LocalMixedScopeSettings), typeof (ExtendedLocalMixedScopeSettings)};

		private static System.Configuration.Configuration GetExeConfiguration()
		{
			return SystemConfigurationHelper.GetExeConfiguration();
		}

		private static void RemoveSettings(Type settingsClass)
		{
			GetExeConfiguration().RemoveSettingsValues(settingsClass);
		}

		[TestFixtureSetUp]
		public void Setup()
		{
			ResetAllSettingsClasses();
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
            foreach (var settingsClass in _settingsClasses)
                RemoveSettings(settingsClass);
		}

		[Test]
		public void TestGetSharedSettings_NoneExist()
		{
            foreach (var settingsClass in _settingsClasses)
                TestGetSharedSettings_NoneExist(settingsClass);
        }

	    [Test]
		public void TestGetSharedSettings_Exists()
		{
            foreach (var settingsClass in _settingsClasses)
                TestGetSharedSettings_Exists(settingsClass);
        }
        [Test]
        public void TestSetSharedSettings()
        {
            foreach (var settingsClass in _settingsClasses)
                TestSetSharedSettings(settingsClass);
        }

	    [Test]
		public void TestGetPreviousSharedValues_NoneExist()
		{
            ResetAllSettingsClasses();
            foreach (var settingsClass in _settingsClasses)
                TestGetPreviousSharedValues_NoneExist(settingsClass);
        }

	    [Test]
		public void TestGetPreviousSharedValues_Exists()
		{
            ResetAllSettingsClasses();
            foreach (var settingsClass in _settingsClasses)
                TestGetPreviousSharedValues_Exists(settingsClass);
        }

        private void TestSetSharedSettings(Type settingsClass)
        {
            if (!settingsClass.IsSubclassOf(typeof(MixedScopeSettingsBase)))
                throw new ArgumentException();

            RemoveSettings(settingsClass);
            var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
            
            Assert.AreEqual(MixedScopeSettingsBase.PropertyApp1, settings[MixedScopeSettingsBase.PropertyApp1]);
            Assert.AreEqual(MixedScopeSettingsBase.PropertyApp2, settings[MixedScopeSettingsBase.PropertyApp2]);
            Assert.AreEqual(MixedScopeSettingsBase.PropertyUser1, settings[MixedScopeSettingsBase.PropertyUser1]);
            Assert.AreEqual(MixedScopeSettingsBase.PropertyUser2, settings[MixedScopeSettingsBase.PropertyUser2]);

            settings.SetSharedPropertyValue(MixedScopeSettingsBase.PropertyApp1, "TestApp1");
            settings.SetSharedPropertyValue(MixedScopeSettingsBase.PropertyApp2, "TestApp2");
            settings.SetSharedPropertyValue(MixedScopeSettingsBase.PropertyUser1, "TestUser1");
            settings.SetSharedPropertyValue(MixedScopeSettingsBase.PropertyUser2, "TestUser2");

            //There is no default, so this is a new instance.
            settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);

            Assert.AreEqual("TestApp1", settings[MixedScopeSettingsBase.PropertyApp1]);
            Assert.AreEqual("TestApp2", settings[MixedScopeSettingsBase.PropertyApp2]);
            Assert.AreEqual("TestUser1", settings.GetSharedPropertyValue(MixedScopeSettingsBase.PropertyUser1));
            Assert.AreEqual("TestUser2", settings.GetSharedPropertyValue(MixedScopeSettingsBase.PropertyUser2));
/*
            //Just because ... test setting a user value.
            settings[MixedScopeSettings.PropertyUser1] = "TestUserblah";
            settings.Save();

            settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
            Assert.AreEqual("TestUserblah", settings[MixedScopeSettings.PropertyUser1]);
            Assert.AreEqual("TestUser1", settings.GetSharedPropertyValue(MixedScopeSettingsBase.PropertyUser1));
            settings.Reset();
*/
        }

	    private void TestGetSharedSettings_Exists(Type settingsClass)
	    {
	        RemoveSettings(settingsClass);

	        SystemConfigurationHelperTests.WriteSharedValuesToConfig(settingsClass, SettingValue.Current);
	        var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
	        settings.Reload();

	        foreach (SettingsProperty property in settings.Properties)
	        {
	            var shared = settings.GetSharedPropertyValue(property.Name);
	            string expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
	            Assert.AreEqual(expected, shared);

	            if (SettingsPropertyExtensions.IsAppScoped(property))
	                Assert.AreEqual(expected, settings[property.Name]);
	        }
	    }

	    private void TestGetSharedSettings_NoneExist(Type settingsClass)
	    {
	        RemoveSettings(settingsClass);

	        var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
	        settings.Reload();

	        foreach (SettingsProperty property in settings.Properties)
	        {
	            var shared = settings.GetSharedPropertyValue(property.Name);
	            Assert.AreEqual(property.DefaultValue, shared);

	            if (SettingsPropertyExtensions.IsAppScoped(property))
	                Assert.AreEqual(property.DefaultValue, settings[property.Name]);
	        }
	    }

	    private void TestGetPreviousSharedValues_NoneExist(Type settingsClass)
	    {
	        var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
	        settings.Reload();
	        foreach (SettingsProperty property in settings.Properties)
	        {
	            var previous = settings.GetPreviousSharedPropertyValue(property.Name, null);
	            Assert.IsNull(previous);
	        }
	    }

	    private void TestGetPreviousSharedValues_Exists(Type settingsClass)
        {
			string path = Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath);
			string fileName = String.Format("{0}{1}TestPrevious.exe.config", path, System.IO.Path.DirectorySeparatorChar);
			TestConfigResourceToFile(fileName);

			try
			{
				var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
				settings.Reload();
				foreach (SettingsProperty property in settings.Properties)
				{
					var actual = settings.GetPreviousSharedPropertyValue(property.Name, fileName);
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