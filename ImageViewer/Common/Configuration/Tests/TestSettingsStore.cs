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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Common.Configuration.Tests
{
    public class TestSystemConfigurationServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof(ISystemConfigurationSettingsStore))
                return null;

            return TestSettingsStore.Instance;
        }

        #endregion
    }

    public class TestSettingsStore : ISystemConfigurationSettingsStore
    {
        public static TestSettingsStore Instance = new TestSettingsStore();
        public static string TestString = "Prior Setting";

        private class Setting
        {
            public SettingsGroupDescriptor Group;
            public Dictionary<string, string> Values;

        }

        private Setting FindSetting(SettingsGroupDescriptor @group)
        {
            foreach (var setting in _settingsList)
            {
                if (group.Name == setting.Group.Name && group.Version == setting.Group.Version)
                {
                    return setting;
                }
            }
            return null;
        }


        private List<Setting> _settingsList = new List<Setting>();

        public SettingsGroupDescriptor GetPreviousSettingsGroup(SettingsGroupDescriptor @group)
        {
            var newGroup = new SettingsGroupDescriptor(@group.Name, new Version(0, 9), @group.Description,
                                           @group.AssemblyQualifiedTypeName,
                                           @group.HasUserScopedSettings);

            var matchingSetting = FindSetting(@group);
            var previousSetting = FindSetting(newGroup);
            if (matchingSetting == null)
            {
                if (previousSetting != null)
                    return newGroup;

                return null;
            }
            
            if (previousSetting == null)
            {
                var newSetting = new Setting
                                     {
                                         Group = newGroup,
                                         Values = new Dictionary<string, string>()
                                     };
                _settingsList.Add(newSetting);

                foreach (var key in matchingSetting.Values.Keys)
                {
                    var val = matchingSetting.Values[key];

                    newSetting.Values[key] = val + TestString;
                }
            }

            return newGroup;            
        }

        public Dictionary<string, string> GetSettingsValues(SettingsGroupDescriptor @group, string user, string instanceKey)
        {
            foreach (var setting in _settingsList)
            {
                if (group.Name == setting.Group.Name && group.Version == setting.Group.Version)
                {
                    return setting.Values;
                }
            }
            return new Dictionary<string, string>();
        }

        public void PutSettingsValues(SettingsGroupDescriptor @group, string user, string instanceKey, Dictionary<string, string> dirtyValues)
        {
            foreach (var setting in _settingsList)
            {
                if (group.Name == setting.Group.Name && group.Version == setting.Group.Version)
                {
                    foreach (var key in dirtyValues.Keys)
                    {
                        string val = dirtyValues[key];
                        setting.Values[key] = val;
                    }

                    return;
                }
            }

            var newSetting = new Setting
                                 {
                                     Group = @group, 
                                     Values = dirtyValues
                                 };
            _settingsList.Add(newSetting);
        }

        public void Reset()
        {
            _settingsList = new List<Setting>();
        }

        public void RemoveSettingsGroup(SettingsGroupDescriptor @group)
        {
            Setting settingToRemove = null;
            foreach (var setting in _settingsList)
            {
                if (group.Name == setting.Group.Name && group.Version == setting.Group.Version)
                {
                    settingToRemove = setting;
                    break;
                }
            }

            if (settingToRemove != null)
                _settingsList.Remove(settingToRemove);            
        }
    }
}

#endif
