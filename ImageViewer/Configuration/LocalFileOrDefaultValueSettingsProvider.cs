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
using System.ComponentModel;
using System.Configuration;

namespace ClearCanvas.ImageViewer.Configuration
{
    // TODO (CR Mar 2012): We should get rid of this.
    /// <summary>
    /// Special settings provider to retrieve settings from local configuration file 
    /// or the default values if it is not supported.
    /// </summary>
    /// <remarks>
    /// Use this class instead of LocalFileSettingsProvider if the configuration contains user-scoped settings 
    /// and the code is used in an environment where user-scoped setting is not supported (e.g., IIS).
    /// </remarks>
    public class LocalFileOrDefaultValueSettingsProvider:SettingsProvider
    {
        private LocalFileSettingsProvider _localFileSettingProvider;
        private string _appName;

        public LocalFileOrDefaultValueSettingsProvider()
        {
            _appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        }

        public override string ApplicationName
        {
            get { return _appName; }
            set
            {
                _appName = value;
            }
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            try
            {
                _localFileSettingProvider = new LocalFileSettingsProvider();
            }
            catch (Exception)
            {
            
            }

            base.Initialize(ApplicationName, config);
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            try
            {
                if (_localFileSettingProvider != null)
                {
                    return _localFileSettingProvider.GetPropertyValues(context, collection);
                }
            }
            catch(ConfigurationErrorsException)
            {
                // ignore the error, fall back to the default values
            }

            return GetDefaultValues(context, collection);
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            if (_localFileSettingProvider != null)
            {
                _localFileSettingProvider.SetPropertyValues(context, collection);
            }
        }

        #region Helper Methods

        private SettingsPropertyValueCollection GetDefaultValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            SettingsPropertyValueCollection defaultValues = new SettingsPropertyValueCollection();
            foreach (SettingsProperty property in collection)
            {
                defaultValues.Add(new SettingsPropertyValue(property)
                {
                    PropertyValue = GetDefaultValue(property)
                });
            }
            return defaultValues;
        }
  
        private static object GetDefaultValue(SettingsProperty property)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(property.PropertyType);

            switch (property.SerializeAs)
            {
                case SettingsSerializeAs.String:
                    return converter.ConvertFromString(property.DefaultValue as string);
                default:
                    throw new NotSupportedException(String.Format("Could not get the default value for {0}. LocalOrDefaultValueSettingsProvider does not support settings that are serialized as {1}",
                                                                  property.Name, property.SerializeAs));
            }
        }

        #endregion
    }
}