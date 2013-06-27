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
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Configuration;
using System.Reflection;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// This class contains some helper methods to do silly conversions that are an artifact
	/// of the mismatch between the types of programming patterns available in .NET 1.0 vs .NET 4.0.
	/// </summary>
	static class SettingsPropertyExtensions
	{
		public static SettingsPropertyCollection ToSettingsPropertyCollection(this IEnumerable<SettingsProperty> settingsProperties)
		{
			var c = new SettingsPropertyCollection();
			foreach (var property in settingsProperties)
			{
				c.Add(property);
			}
			return c;
		}

		public static SettingsPropertyValueCollection ToSettingsPropertyValueCollection(this IEnumerable<SettingsPropertyValue> values)
		{
			var c = new SettingsPropertyValueCollection();
			foreach (var value in values)
			{
				c.Add(value);
			}
			return c;
		}
	}

	public partial class SettingsManagementComponent
	{
		/// <summary>
		/// Represents a settings group that is part of the locally installed set of plugins.
		/// </summary>
		internal class LocallyInstalledGroup : Group
		{
			private readonly Type _settingsClass;

			public LocallyInstalledGroup(SettingsGroupDescriptor descriptor)
				: base(descriptor)
			{
				_settingsClass = ApplicationSettingsHelper.GetSettingsClass(this.Descriptor);
			}

            private Property CreateProperty(PropertyInfo property, SettingsProperty settingsProperty, SettingsPropertyValueCollection settingsPropertyValues)
            {
                var descriptor = new SettingsPropertyDescriptor(property);
                var settingsPropertyValue = settingsPropertyValues[settingsProperty.Name];
                var defaultValue = settingsProperty.DefaultValue;
                var serializedValue = settingsPropertyValue == null ? null : settingsPropertyValue.SerializedValue;

                return new Property(descriptor, (serializedValue ?? defaultValue).ToString());
            }
            
            protected override IEnumerable<Property> LoadProperties()
			{
				var properties = (from p in SettingsClassMetaDataReader.GetSettingsProperties(_settingsClass)
								  select new { Property = p, Setting = CreateSettingsProperty(p) })
								 .ToList();

				// retrieve the "shared" property values, which may differ from the values on an instance of the settings class!
				// We use the provider directly, because we are dealing in serialized values which cannot be obtained from the settings class.
				// Note that we assume that each property uses the settings class-level provider, and while this is a blatantly false
				// assumption in general (a property can have its own provider), it seems to hold true for settings as used within
				// the ClearCanvas codebase.
				var providerClass = SettingsClassMetaDataReader.GetSettingsProvider(_settingsClass);
				var provider = (ISharedApplicationSettingsProvider)GetProviderInstance(providerClass);
				var settingsPropertiesColl = properties.Select(p => p.Setting).ToSettingsPropertyCollection();
				var values = provider.GetSharedPropertyValues(GetSettingsContext(), settingsPropertiesColl);

				return from p in properties select CreateProperty(p.Property, p.Setting, values);
			}

		    protected override void SaveProperties(IList<Property> properties)
			{
				var q = from p in properties
						where p.Dirty
						select new SettingsPropertyValue(CreateSettingsProperty(_settingsClass.GetProperty(p.Name))) { SerializedValue = p.Value, IsDirty = true };

				// Save the "shared" property values, which may differ from the values on an instance of the settings class!
				// We use the provider directly, because we are dealing in serialized values which cannot be obtained from the settings class.
				// Note that we assume that each property uses the settings class-level provider, and while this is a blatantly false
				// assumption in general (a property can have its own provider), it seems to hold true for settings as used within
				// the ClearCanvas codebase.
				var providerClass = SettingsClassMetaDataReader.GetSettingsProvider(_settingsClass);
				var provider = (ISharedApplicationSettingsProvider)GetProviderInstance(providerClass);
				provider.SetSharedPropertyValues(GetSettingsContext(), q.ToSettingsPropertyValueCollection());
			}

			#region Helpers

			private SettingsContext GetSettingsContext()
			{
				var context = new SettingsContext();
				context["GroupName"] = _settingsClass.FullName;
				context["SettingsKey"] = null;
				context["SettingsClassType"] = _settingsClass;
				return context;
			}

			private static SettingsProperty CreateSettingsProperty(PropertyInfo property)
			{
				return new SettingsProperty(SettingsClassMetaDataReader.GetName(property))
				{
					PropertyType = SettingsClassMetaDataReader.GetType(property),
					IsReadOnly = AttributeUtils.HasAttribute<ReadOnlyAttribute>(property, false),
					DefaultValue = SettingsClassMetaDataReader.GetDefaultValue(property),
					SerializeAs = SettingsClassMetaDataReader.GetSerializeAs(property),
					Provider = GetProviderInstance(SettingsClassMetaDataReader.GetSettingsProvider(property))
				};
			}

			private static SettingsProvider GetProviderInstance(Type providerType)
			{
				var provider = (SettingsProvider)Activator.CreateInstance(providerType);
				provider.Initialize(null, null);
				return provider;
			}

			#endregion
		}
	}
}
