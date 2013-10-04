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
using System.Configuration;
using System.Reflection;

namespace ClearCanvas.Common.Configuration
{
	public static class ApplicationSettingsHelper
	{
		/// <summary>
		/// Gets a value indicating whether the specified settings class stores its settings locally or not.
		/// </summary>
		/// <param name="settingsClass"></param>
		/// <returns>True if the specified settings class stores its settings locally.</returns>
		internal static bool IsLocallyStored(Type settingsClass)
		{
			var attributes = settingsClass.GetCustomAttributes(typeof(SettingsProviderAttribute), true);
			if (attributes.Length == 0)
				return true;

			var attribute = (SettingsProviderAttribute)attributes[0];
			if (attribute.ProviderTypeName == typeof(LocalFileSettingsProvider).AssemblyQualifiedName)
				return true;

			if (attribute.ProviderTypeName == typeof(ExtendedLocalFileSettingsProvider).AssemblyQualifiedName)
				return true;

			if (attribute.ProviderTypeName == typeof(ApplicationCriticalSettingsProvider).AssemblyQualifiedName)
				return true;

			if (!SettingsStore.IsSupported && attribute.ProviderTypeName == typeof(StandardSettingsProvider).AssemblyQualifiedName)
				return true;

			return false;
		}

		public static bool IsUserSettingsMigrationEnabled(Type settingsClass)
		{
			CheckType(settingsClass);
			return !settingsClass.IsDefined(typeof(UserSettingsMigrationDisabledAttribute), false);
		}

		public static bool IsSharedSettingsMigrationEnabled(Type settingsClass)
		{
			CheckType(settingsClass);
			return !settingsClass.IsDefined(typeof(SharedSettingsMigrationDisabledAttribute), false);
		}

		public static void CheckType(Type settingsClass)
		{
			if (!settingsClass.IsSubclassOf(typeof(ApplicationSettingsBase)))
			{
				throw new ArgumentException(
					String.Format("Type does not derive from ApplicationSettingsBase: {0}", settingsClass.FullName));
			}
		}

		public static Type GetSettingsClass(SettingsGroupDescriptor group)
		{
			var settingsClass = Type.GetType(group.AssemblyQualifiedTypeName, true);
			CheckType(settingsClass);
		    return settingsClass;
		}

		public static ApplicationSettingsBase GetDefaultInstance(Type settingsClass)
		{
            CheckType(settingsClass);

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            var defaultProperty = settingsClass.GetProperty("Default", bindingFlags);

            if (defaultProperty != null)
                return (ApplicationSettingsBase)defaultProperty.GetValue(null, null);

		    return null;
		}

	    public static ApplicationSettingsBase GetSettingsClassInstance(Type settingsClass)
	    {
	        var defaultInstance = GetDefaultInstance(settingsClass);
			//try to return an instance of the class
			return defaultInstance ?? (ApplicationSettingsBase)Activator.CreateInstance(settingsClass);
		}
	}
}