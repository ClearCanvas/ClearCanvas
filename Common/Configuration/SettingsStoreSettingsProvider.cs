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
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Configuration
{
    /// <summary>
    /// Communicates with an <see cref="ISettingsStore"/> to manage loading and saving of
    /// settings from the store.
    /// </summary>
    /// <remarks>
	/// Supports the framework and is not intended for use by application code.  Use 
	/// <see cref="StandardSettingsProvider"/> instead.
	/// </remarks>
    public class SettingsStoreSettingsProvider : SettingsProvider, IApplicationSettingsProvider, ISharedApplicationSettingsProvider
    {
        private string _appName;
        private readonly ISettingsStore _store;

        public SettingsStoreSettingsProvider(ISettingsStore store)
        {
            _store = store;
        }

        /// <summary>
        /// Gets the Application Name used to initialize the settings subsystem.
        /// </summary>
		public override string ApplicationName
        {
            get
            {
                return _appName;
            }
            set
            {
                _appName = value;
            }
        }

        private static SettingsPropertyValueCollection GetSettingsValues(SettingsPropertyCollection properties, IDictionary<string, string> storedValues)
        {
            // Create new collection of values
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

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

        private static bool AnyUserScoped(SettingsPropertyCollection properties)
        {
        	return CollectionUtils.SelectFirst<SettingsProperty>(properties, SettingsPropertyExtensions.IsUserScoped) != null;
        }

        private SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties, bool returnPrevious)
        {
        	bool anyUserScopedSettings = AnyUserScoped(properties);
        	Type settingsClass = (Type) context["SettingsClassType"];
        	string settingsKey = (string) context["SettingsKey"];
        	string user = anyUserScopedSettings ? Thread.CurrentPrincipal.Identity.Name : null;

			var group = new SettingsGroupDescriptor(settingsClass);
			if (returnPrevious)
			    group = _store.GetPreviousSettingsGroup(group);
			else if (anyUserScopedSettings)
				SettingsMigrator.MigrateUserSettings(group);

			var storedValues = new Dictionary<string, string>();

			if (group != null)
			{
				foreach (var userDefault in _store.GetSettingsValues(group, null, settingsKey))
					storedValues[userDefault.Key] = userDefault.Value;

				if (anyUserScopedSettings)
				{
					foreach (var userValue in _store.GetSettingsValues(group, user, settingsKey))
						storedValues[userValue.Key] = userValue.Value;
				}
			}

		    return GetSettingsValues(properties, storedValues);
		}

    	///<summary>
    	///Returns the collection of settings property values for the specified application instance and settings property group.
    	///</summary>
    	public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
        {
			return GetPropertyValues(context, props, false);
        }

    	///<summary>
    	///Sets the values of the specified group of property settings.
    	///</summary>
    	public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection settings)
        {
			SetPropertyValues(context, settings, Thread.CurrentPrincipal.Identity.Name);
        }

		private void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection settings, string user)
		{
			bool storeSharedValues = String.IsNullOrEmpty(user);
            // locate dirty values that should be saved
            Dictionary<string, string> valuesToStore = new Dictionary<string, string>();
            foreach (SettingsPropertyValue value in settings)
            {
				//If storing the shared values, we store everything, otherwise only store user values.
				if (value.IsDirty && (storeSharedValues || IsUserScoped(value.Property)))
                    valuesToStore[value.Name] = (string)value.SerializedValue;
            }

			if (valuesToStore.Count > 0)
            {
                Type settingsClass = (Type)context["SettingsClassType"];
                string settingsKey = (string)context["SettingsKey"];

                _store.PutSettingsValues(new SettingsGroupDescriptor(settingsClass), user, settingsKey, valuesToStore);

				// successfully saved user settings are no longer dirty
				foreach (var storedValue in valuesToStore)
					settings[storedValue.Key].IsDirty = false;
			}
        }

    	private static bool IsUserScoped(SettingsProperty property)
        {
			return SettingsPropertyExtensions.IsUserScoped(property);
        }

        #region IApplicationSettingsProvider Members

        /// <summary>
        /// Not implemented.
        /// </summary>
        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
        {
			SettingsPropertyCollection properties = new SettingsPropertyCollection { property };
			//Stinks that we have to get the whole document each time, but oh well.
        	SettingsPropertyValueCollection values = GetPropertyValues(context, properties, true);
			return values[property.Name];
        }

        /// <summary>
        /// Resets all settings back to the defaults.
        /// </summary>
        /// <remarks>
		/// Note that this implementation resets the user-scoped settings only.  It does not modify application-scoped settings.
		/// </remarks>
        public void Reset(SettingsContext context)
        {
            Type settingsClass = (Type)context["SettingsClassType"];
            string settingsKey = (string)context["SettingsKey"];
            string user = Thread.CurrentPrincipal.Identity.Name;

            _store.RemoveUserSettings(new SettingsGroupDescriptor(settingsClass), user, settingsKey);
        }

        private void Upgrade(SettingsContext context, SettingsPropertyCollection properties, string user)
        {
            Type settingsClass = (Type)context["SettingsClassType"];
            string settingsKey = (string)context["SettingsKey"];
            bool isUserUpgrade = !String.IsNullOrEmpty(user);

            SettingsGroupDescriptor group = new SettingsGroupDescriptor(settingsClass);
            Dictionary<string, string> previousValues = GetPreviousSettingsValues(group, user, settingsKey);
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
                SetPropertyValues(context, upgradedValues, user);
        }

        /// <summary>
        /// Upgrades the settings from a previous version.
        /// </summary>
		/// <remarks>
		/// Note that this implementation upgrades user-scoped settings only; it does not modify application-scoped settings.
		/// </remarks>
        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
        {
            Upgrade(context, properties, Thread.CurrentPrincipal.Identity.Name);
        }

        #endregion

		private Dictionary<string, string> GetPreviousSettingsValues(SettingsGroupDescriptor group, string user, string settingsKey)
		{
			var allGroups = _store.ListSettingsGroups();
			var lessGroups = CollectionUtils.Select(allGroups, 
				other => other.AssemblyQualifiedTypeName == group.AssemblyQualifiedTypeName && other.Version < group.Version);

			// sort descending
			lessGroups.Sort((other1, other2) => other2.Version.CompareTo(other1.Version));
			if (lessGroups.Count == 0)
				return null;

			foreach (var g in lessGroups)
			{
				Dictionary<string, string> userValues = null;
				if (!String.IsNullOrEmpty(user))
				{
					userValues = _store.GetSettingsValues(g, user, settingsKey);
					if (userValues == null || userValues.Count == 0)
						continue;
				}

				var values = new Dictionary<string, string>();
				foreach (var binaryDefault in _store.ListSettingsProperties(g))
					values[binaryDefault.Name] = binaryDefault.DefaultValue;

				foreach (var userDefault in _store.GetSettingsValues(g, null, settingsKey))
					values[userDefault.Key] = userDefault.Value;

				if (userValues != null)
				{
					foreach (var userValue in userValues)
						values[userValue.Key] = userValue.Value;
				}

				return values;
			}

			return null;
		}

        private SettingsPropertyValueCollection GetSharedPropertyValues(SettingsGroupDescriptor group, string settingsKey, SettingsPropertyCollection properties)
        {
            var sharedValues = new Dictionary<string, string>();
            foreach (var sharedValue in _store.GetSettingsValues(group, null, settingsKey))
                sharedValues[sharedValue.Key] = sharedValue.Value;

            return GetSettingsValues(properties, sharedValues);
        }

        #region ISharedApplicationSettingsProvider Members

		public bool CanUpgradeSharedPropertyValues(SettingsContext context)
		{
			Type settingsClass = (Type)context["SettingsClassType"];
			string settingsKey = (string)context["SettingsKey"];
			var group = new SettingsGroupDescriptor(settingsClass);

			var storedValues = _store.GetSettingsValues(group, null, settingsKey);
			return storedValues == null || storedValues.Count == 0;
		}
		
		public void UpgradeSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
        {
			if (CanUpgradeSharedPropertyValues(context))
				Upgrade(context, properties, null);
        }

        public SettingsPropertyValueCollection GetPreviousSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties, string ignoredPreviousExeConfigFilename)
        {
            Type settingsClass = (Type)context["SettingsClassType"];
            string settingsKey = (string)context["SettingsKey"];

            var group = new SettingsGroupDescriptor(settingsClass);
            group = _store.GetPreviousSettingsGroup(group);
            return group == null ? new SettingsPropertyValueCollection() : GetSharedPropertyValues(group, settingsKey, properties);
        }

        public SettingsPropertyValueCollection GetSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
        {
            Type settingsClass = (Type)context["SettingsClassType"];
            string settingsKey = (string)context["SettingsKey"];

            var group = new SettingsGroupDescriptor(settingsClass);
            return GetSharedPropertyValues(group, settingsKey, properties);
        }

        public void SetSharedPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
        {
            Dictionary<string, string> valuesToStore = new Dictionary<string, string>();
            foreach (SettingsPropertyValue value in values)
            {
                if (value.IsDirty)
                    valuesToStore[value.Name] = (string)value.SerializedValue;
            }

            if (valuesToStore.Count > 0)
            {
                Type settingsClass = (Type)context["SettingsClassType"];
                string settingsKey = (string)context["SettingsKey"];

                _store.PutSettingsValues(new SettingsGroupDescriptor(settingsClass), null, settingsKey, valuesToStore);
            }

            foreach (SettingsPropertyValue value in values)
                value.IsDirty = false;
        }

        #endregion
    }
}
