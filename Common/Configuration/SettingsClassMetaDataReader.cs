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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Enum defining the scope of a setting.
	/// </summary>
	[Serializable]
	public enum SettingScope
	{
		/// <summary>
		/// Indicates that a setting has application scope.
		/// </summary>
		Application,

		/// <summary>
		/// Indicates that a setting has user scope.
		/// </summary>
		User
	}

	/// <summary>
	/// Utility class for reading meta-data associated with a settings class
	/// (a subclass of <see cref="SettingsBase"/>).
	/// </summary>
	public static class SettingsClassMetaDataReader
	{
		/// <summary>
		/// Obtains the version of the settings class.
		/// </summary>
		/// <remarks>
		/// The version of the settings class is the version of the assembly in which the class is contained.
		/// If <paramref name="truncate"/> is true, the returned value does not contain Build or Revision components.
		/// </remarks>
		public static Version GetVersion(Type settingsClass, bool truncate)
		{
			var v = settingsClass.Assembly.GetName().Version;
			return truncate ? new Version(v.Major, v.Minor) : v;
		}

		/// <summary>
		/// Obtains the name of the settings group, which is always the full name of the settings class.
		/// </summary>
		public static string GetGroupName(Type settingsClass)
		{
			return settingsClass.FullName;
		}

		/// <summary>
		/// Gets the provider associated with the specified settings class.
		/// </summary>
		/// <param name="settingsClass"></param>
		/// <returns></returns>
		public static Type GetSettingsProvider(Type settingsClass)
		{
			var a = AttributeUtils.GetAttribute<SettingsProviderAttribute>(settingsClass, false);
			return a == null ? typeof(LocalFileSettingsProvider) : Type.GetType(a.ProviderTypeName, true);
		}

		/// <summary>
		/// Obtains the settings group description from the <see cref="SettingsGroupDescriptionAttribute"/>.
		/// </summary>
		public static string GetGroupDescription(Type settingsClass)
		{
			var a = AttributeUtils.GetAttribute<SettingsGroupDescriptionAttribute>(settingsClass, false);
			return a == null ? "" : a.Description;
		}

		/// <summary>
		/// Gets the collection of properties that represent settings.
		/// </summary>
		public static PropertyInfo[] GetSettingsProperties(Type settingsClass)
		{
			return settingsClass.GetProperties().Where(property => IsUserScoped(property) || IsAppScoped(property)).ToArray();
		}

		/// <summary>
		/// Gets the collection of properties that represent settings of the specified scope.
		/// </summary>
		/// <param name="settingsClass"></param>
		/// <param name="scope"></param>
		/// <returns></returns>
		public static PropertyInfo[] GetSettingsProperties(Type settingsClass, SettingScope scope)
		{
			return GetSettingsProperties(settingsClass).Where(property => GetScope(property) == scope).ToArray();
		}

		/// <summary>
		/// Returns true if the specified settings class has any settings that are user-scoped.
		/// </summary>
		/// <param name="settingsClass"></param>
		/// <returns></returns>
		public static bool HasUserScopedSettings(Type settingsClass)
		{
			return GetSettingsProperties(settingsClass).Any(IsUserScoped);
		}

		/// <summary>
		/// Returns true if the specified settings class has any settings that are application-scoped.
		/// </summary>
		/// <param name="settingsClass"></param>
		/// <returns></returns>
		public static bool HasAppScopedSettings(Type settingsClass)
		{
			return GetSettingsProperties(settingsClass).Any(IsAppScoped);
		}

		/// <summary>
		/// Gets the settings provider associated with the specified property.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public static Type GetSettingsProvider(PropertyInfo property)
		{
			var a = AttributeUtils.GetAttribute<SettingsProviderAttribute>(property, false);
			return a == null ? GetSettingsProvider(property.DeclaringType) : Type.GetType(a.ProviderTypeName, true);
		}

		/// <summary>
		/// Obtains the default value of a setting from the <see cref="DefaultSettingValueAttribute"/>.
		/// </summary>
		/// <remarks>
		/// If the value is the name of an embedded resource, it is automatically translated.
		/// </remarks>
		public static string GetDefaultValue(PropertyInfo property)
		{
			return GetDefaultValue(property, true);
		}

		/// <summary>
		/// Obtains the default value of a setting from the <see cref="DefaultSettingValueAttribute"/>.
		/// </summary>
		/// <remarks>
		/// If translate is true, and the value is the name of an embedded resource, it is automatically translated.
		/// </remarks>
		public static string GetDefaultValue(PropertyInfo property, bool translate)
		{
			var a = AttributeUtils.GetAttribute<DefaultSettingValueAttribute>(property, false);
			if (a == null)
				return "";

			return !translate ? a.Value : TranslateDefaultValue(property.ReflectedType, a.Value);
		}

		/// <summary>
		/// Translates the default value for a settings class given the raw value.
		/// </summary>
		/// <remarks>
		/// If the specified raw value is the name of an embedded resource (embedded in the same
		/// assembly as the specified settings class), the contents of the resource are returned.
		/// Otherwise, the raw value is simply returned.
		/// </remarks>
		public static string TranslateDefaultValue(Type settingsClass, string rawValue)
		{
			// short circuit if nothing translatable
			if (string.IsNullOrEmpty(rawValue))
				return rawValue;

			// does the raw value look like it could be an embedded resource?
			if (Regex.IsMatch(rawValue, @"^([\w]+\.)+\w+$"))
			{
				try
				{
					// try to open the resource
					IResourceResolver resolver = new ResourceResolver(settingsClass.Assembly);
					using (var resourceStream = resolver.OpenResource(rawValue))
					{
						var r = new StreamReader(resourceStream);
						return r.ReadToEnd();
					}
				}
				catch (MissingManifestResourceException)
				{
					// guess it was not an embedded resource, so return the raw value
					return rawValue;
				}
			}
			return rawValue;
		}

		/// <summary>
		/// Obtains the setting description from the <see cref="SettingsDescriptionAttribute"/>.
		/// </summary>
		public static string GetDescription(PropertyInfo property)
		{
			var a = AttributeUtils.GetAttribute<SettingsDescriptionAttribute>(property, false);
			return a == null ? "" : a.Description;
		}

		/// <summary>
		/// Returns a <see cref="SettingScope"/> enum describing the scope of the property.
		/// </summary>
		public static SettingScope GetScope(PropertyInfo property)
		{
			if (IsAppScoped(property))
				return SettingScope.Application;
			if (IsUserScoped(property))
				return SettingScope.User;

			throw new Exception(SR.MessageSettingsScopeNotDefined);
		}

		/// <summary>
		/// Determines how a particular property should be serialized based on its type.
		/// </summary>
		/// <param name="property">the property whose SerializeAs method is to be determined</param>
		/// <returns>a <see cref="SettingsSerializeAs"/> value</returns>
		public static SettingsSerializeAs GetSerializeAs(PropertyInfo property)
		{
			var a = AttributeUtils.GetAttribute<SettingsSerializeAsAttribute>(property, false);
			if (a != null)
				return a.SerializeAs;

			var converter = TypeDescriptor.GetConverter(property.PropertyType);
			var stringType = typeof(string);
			if (converter.CanConvertTo(stringType) && converter.CanConvertFrom(stringType))
				return SettingsSerializeAs.String;

			return SettingsSerializeAs.Xml;
		}

		/// <summary>
		/// Returns the name of the settings property.
		/// </summary>
		public static string GetName(PropertyInfo property)
		{
			return property.Name;
		}

		/// <summary>
		/// Returns the <see cref="Type"/> of the settings property.
		/// </summary>
		public static Type GetType(PropertyInfo property)
		{
			return property.PropertyType;
		}

		/// <summary>
		/// Returns true if the property is decorated with a <see cref="UserScopedSettingAttribute"/>.
		/// </summary>
		public static bool IsUserScoped(PropertyInfo property)
		{
			return AttributeUtils.HasAttribute<UserScopedSettingAttribute>(property, false);
		}

		/// <summary>
		/// Returns true if the property is decorated with a <see cref="ApplicationScopedSettingAttribute"/>.
		/// </summary>
		public static bool IsAppScoped(PropertyInfo property)
		{
			return AttributeUtils.HasAttribute<ApplicationScopedSettingAttribute>(property, false);
		}
	}
}