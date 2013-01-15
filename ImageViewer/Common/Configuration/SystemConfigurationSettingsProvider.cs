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

using System;
using System.Collections.Generic;
using System.Configuration;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common.Configuration
{
    public sealed class SystemConfigurationSettingsProvider : SettingsProvider, ISharedApplicationSettingsProvider
    {
        #region Private Members

        private readonly object _syncLock = new object();
        private ISystemConfigurationSettingsStore _store;

        #endregion

        #region Public Properties

        public override string ApplicationName { get; set; }

        #endregion

        #region Constructors

        public SystemConfigurationSettingsProvider()
		{
			// according to MSDN recommendation, use the name of the executing assembly here
            ApplicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
		}

        #endregion


        #region Public Methods

        ///<summary>
        ///Initializes the provider.
        ///</summary>
        ///
        ///<param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        ///<param name="name">The friendly name of the provider.</param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            lock (_syncLock)
            {
                // obtain a source provider
                _store = Platform.GetService<ISystemConfigurationSettingsStore>();             

                // init source provider
                // according to sample implementations, use the application name here
                base.Initialize(ApplicationName, config);
            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            var values = GetPropertyValues(context, collection, false);
            var settingsClass = (Type)context["SettingsClassType"];
        
            TranslateValues(settingsClass,values);

            return values;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            lock (_syncLock)
            {
                // locate dirty values that should be saved
                var valuesToStore = new Dictionary<string, string>();
                foreach (SettingsPropertyValue value in collection)
                {
                    //If storing the shared values, we store everything, otherwise only store user values.
                     if (value.IsDirty)
                     {
                         valuesToStore[value.Name] = (string) value.SerializedValue;
                     }
                }

                if (valuesToStore.Count > 0)
                {
                    var settingsClass = (Type)context["SettingsClassType"];
                    var settingsKey = (string)context["SettingsKey"];
                    var group = new SettingsGroupDescriptor(settingsClass);
                  
                    if (group.HasUserScopedSettings)
                        throw new ConfigurationErrorsException(SR.SystemConfigurationSettingsProviderUserSetting);
                  
                    _store.PutSettingsValues(group, null, settingsKey, valuesToStore);

                    // successfully saved user settings are no longer dirty
                    foreach (var storedValue in valuesToStore)
                        collection[storedValue.Key].IsDirty = false;
                }
            }
        }

        public bool CanUpgradeSharedPropertyValues(SettingsContext context)
        {
            var settingsClass = (Type)context["SettingsClassType"];
            var settingsKey = (string)context["SettingsKey"];
            var group = new SettingsGroupDescriptor(settingsClass);

            var storedValues = _store.GetSettingsValues(group, null, settingsKey);
            return storedValues == null || storedValues.Count == 0;
        }

        public void UpgradeSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
        {
            lock (_syncLock)
            {
                if (CanUpgradeSharedPropertyValues(context))
                    Upgrade(context, properties, null);
            }
        }

        public SettingsPropertyValueCollection GetPreviousSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
        {
            var settingsClass = (Type)context["SettingsClassType"];
            var settingsKey = (string)context["SettingsKey"];
            
            var group = new SettingsGroupDescriptor(settingsClass);

            if (group.HasUserScopedSettings)
                throw new ConfigurationErrorsException(SR.SystemConfigurationSettingsProviderUserSetting);
     
            var oldGroup = _store.GetPreviousSettingsGroup(group);
            
            var storedValues = new Dictionary<string, string>();

            if (oldGroup != null)
                foreach (var oldValue in _store.GetSettingsValues(oldGroup, null, settingsKey))
                    storedValues[oldValue.Key] = oldValue.Value;

            return GetSettingsValues(properties, storedValues);           
        }

        public SettingsPropertyValueCollection GetSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
        {
            var settingsClass = (Type)context["SettingsClassType"];
            var settingsKey = (string)context["SettingsKey"];
            
            var group = new SettingsGroupDescriptor(settingsClass);
            if (group.HasUserScopedSettings)
                throw new ConfigurationErrorsException(SR.SystemConfigurationSettingsProviderUserSetting);

            var values = GetSharedPropertyValues(group, settingsKey, properties);
            TranslateValues(settingsClass, values);
            return values;
        }

        public void SetSharedPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
        {
            lock (_syncLock)
            {
                var valuesToStore = new Dictionary<string, string>();
                foreach (SettingsPropertyValue value in values)
                {
                    if (value.IsDirty)
                    {
                        if (IsUserScoped(value.Property))
                        {
                            throw new ConfigurationErrorsException(SR.SystemConfigurationSettingsProviderUserSetting);
                        }

                        valuesToStore[value.Name] = (string) value.SerializedValue;
                    }
                }

                if (valuesToStore.Count > 0)
                {
                    var settingsClass = (Type) context["SettingsClassType"];
                    var settingsKey = (string) context["SettingsKey"];
                    var group = new SettingsGroupDescriptor(settingsClass);

                    if (group.HasUserScopedSettings)
                        throw new ConfigurationErrorsException(SR.SystemConfigurationSettingsProviderUserSetting);
     
                    _store.PutSettingsValues(group, null, settingsKey, valuesToStore);
                }

                foreach (SettingsPropertyValue value in values)
                    value.IsDirty = false;
            }
        }

        #endregion

        #region Private Methods

        private static bool IsUserScoped(SettingsProperty property)
        {
            return CollectionUtils.Contains(property.Attributes.Values, attribute => attribute is UserScopedSettingAttribute);
        }

        private static void TranslateValues(Type settingsClass, SettingsPropertyValueCollection values)
        {
            foreach (SettingsPropertyValue value in values)
            {
                if (value.SerializedValue == null || (value.SerializedValue is string) && ((string)value.SerializedValue) == ((string)value.Property.DefaultValue))
                    value.SerializedValue = SettingsClassMetaDataReader.TranslateDefaultValue(settingsClass, (string)value.Property.DefaultValue);
            }
        }

        private void Upgrade(SettingsContext context, SettingsPropertyCollection properties, string user)
        {
            var settingsClass = (Type)context["SettingsClassType"];
            var settingsKey = (string)context["SettingsKey"];
            bool isUserUpgrade = !String.IsNullOrEmpty(user);

            var group = new SettingsGroupDescriptor(settingsClass);
            var priorGroup = _store.GetPreviousSettingsGroup(@group);
            if (priorGroup == null) 
                return;

            Dictionary<string, string> previousValues = _store.GetSettingsValues(priorGroup, null, settingsKey);
            bool oldValuesExist = previousValues != null && previousValues.Count > 0;
            if (!oldValuesExist)
                return; //nothing to migrate.

            var upgradedValues = new SettingsPropertyValueCollection();

            foreach (SettingsProperty property in properties)
            {
                if (isUserUpgrade && !IsUserScoped(property))
                    continue;

                string oldValue;
                if (!previousValues.TryGetValue(property.Name, out oldValue))
                    continue;

                //Unconditionally save every old value; let the store sort out what to do.
                upgradedValues.Add(new SettingsPropertyValue(property) { SerializedValue = oldValue, IsDirty = true });
            }

            if (upgradedValues.Count > 0)
                SetPropertyValues(context, upgradedValues);
        }

        private SettingsPropertyValueCollection GetSharedPropertyValues(SettingsGroupDescriptor group, string settingsKey, SettingsPropertyCollection properties)
        {
            var sharedValues = new Dictionary<string, string>();
            foreach (var sharedValue in _store.GetSettingsValues(group, null, settingsKey))
                sharedValues[sharedValue.Key] = sharedValue.Value;

            return GetSettingsValues(properties, sharedValues);
        }

        private static SettingsPropertyValueCollection GetSettingsValues(SettingsPropertyCollection properties, IDictionary<string, string> storedValues)
        {
            // Create new collection of values
            var values = new SettingsPropertyValueCollection();

            foreach (SettingsProperty setting in properties)
            {
                var value = new SettingsPropertyValue(setting)
                {
                    SerializedValue = storedValues.ContainsKey(setting.Name) ? storedValues[setting.Name] : null,
                    IsDirty = false
                };

                // use the stored value, or set the SerializedValue to null, which tells .NET to use the default value
                values.Add(value);
            }

            return values;
        }

        private SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties, bool returnPrevious)
        {
            var settingsClass = (Type)context["SettingsClassType"];
            var settingsKey = (string)context["SettingsKey"];

            var group = new SettingsGroupDescriptor(settingsClass);

            var storedValues = new Dictionary<string, string>();

            foreach (var userDefault in _store.GetSettingsValues(@group, null, settingsKey))
                storedValues[userDefault.Key] = userDefault.Value;

            return GetSettingsValues(properties, storedValues);
        }

        #endregion
    }
}
