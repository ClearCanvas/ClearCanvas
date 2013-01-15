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
using System.Configuration;
using System.Reflection;
using ClearCanvas.ImageViewer.Common.Configuration;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Configuration.Tests
{
    internal abstract class TestApplicationSettingsBase : ApplicationSettingsBase
    {
        private static readonly Type _testType = typeof(TestApplicationSettingsBase);

        public static void CheckType(Type settingsClass)
        {
            if (!settingsClass.IsSubclassOf(typeof(ApplicationSettingsBase)))
            {
                throw new ArgumentException(
                    String.Format("Type does not derive from ApplicationSettingsBase: {0}", settingsClass.FullName));
            }
        }

        public static ApplicationSettingsBase GetSettingsClassInstance(Type settingsClass)
        {
            CheckType(settingsClass);

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            PropertyInfo defaultProperty = settingsClass.GetProperty("Default", bindingFlags);

            if (defaultProperty != null)
                return (ApplicationSettingsBase)defaultProperty.GetValue(null, null);

            //try to return an instance of the class
            return (ApplicationSettingsBase)Activator.CreateInstance(settingsClass);
        }

        public static void ResetAll()
        {
            foreach (Type type in _testType.Assembly.GetTypes())
            {
                if (type.IsSubclassOf(_testType) && !type.IsAbstract)
                    GetSettingsClassInstance(type).Reset();
            }
        }
    }

    internal abstract class MixedScopeSettingsBase : TestApplicationSettingsBase
    {
        public const string PropertyUser1 = "User1";
        public const string PropertyUser2 = "User2";
        public const string PropertyApp1 = "App1";
        public const string PropertyApp2 = "App2";

        [ApplicationScopedSetting]
        [DefaultSettingValue(PropertyApp1)]
        public string App1
        {
            get { return (string)base[PropertyApp1]; }
            set { base[PropertyApp1] = value; }
        }

        [ApplicationScopedSetting]
        [DefaultSettingValue(PropertyApp2)]
        public string App2
        {
            get { return (string)base[PropertyApp2]; }
            set { base[PropertyApp2] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue(PropertyUser1)]
        public string User1
        {
            get { return (string)base[PropertyUser1]; }
            set { base[PropertyUser1] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue(PropertyUser2)]
        public string User2
        {
            get { return (string)base[PropertyUser2]; }
            set { base[PropertyUser2] = value; }
        }
    }

    [SettingsProvider(typeof(SystemConfigurationSettingsProvider))]
    internal class MixedScopeSettings : MixedScopeSettingsBase
    {
        private static readonly MixedScopeSettings _default =
            (MixedScopeSettings)Synchronized(new MixedScopeSettings());

        private MixedScopeSettings()
        {
        }

        public static MixedScopeSettings Default { get { return _default; } }
    }

    [SettingsProvider(typeof(SystemConfigurationSettingsProvider))]
    internal class AppSettings : TestApplicationSettingsBase
    {
        public const string PropertyApp = "App";

        public const string DefaultValueApp = "<?xml version=\"1.0\" encoding=\"utf-16\"?><test-xml-values><test>App</test></test-xml-values>";

        [ApplicationScopedSetting]
        [DefaultSettingValue(DefaultValueApp)]
        public string App
        {
            get { return (string)base[PropertyApp]; }
            set { base[PropertyApp] = value; }
        }
    }


    [SettingsProvider(typeof(SystemConfigurationSettingsProvider))]
    internal class MigrationAppSettings : TestApplicationSettingsBase
    {
        public const string PropertyApp1 = "App1";
        public const string PropertyApp2 = "App2";
        public const string PropertyApp3 = "App3";

        public const string DefaultValueApp = "<?xml version=\"1.0\" encoding=\"utf-16\"?><test-xml-values><test>App</test></test-xml-values>";

        [ApplicationScopedSetting]
        [DefaultSettingValue(DefaultValueApp)]
        public string App1
        {
            get { return (string)base[PropertyApp1]; }
            set { base[PropertyApp1] = value; }
        }

        [ApplicationScopedSetting]
        [DefaultSettingValue(DefaultValueApp)]
        public string App2
        {
            get { return (string)base[PropertyApp2]; }
            set { base[PropertyApp2] = value; }
        }

        [ApplicationScopedSetting]
        [DefaultSettingValue(DefaultValueApp)]
        public string App3
        {
            get { return (string)base[PropertyApp3]; }
            set { base[PropertyApp3] = value; }
        }
    }
}

#endif
