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

using System.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Configuration
{
	internal class SettingsPropertyExtensions
	{
		public static bool IsUserScoped(SettingsProperty property)
		{
			return CollectionUtils.Contains(property.Attributes.Values, attribute => attribute is UserScopedSettingAttribute);
		}

		public static bool IsAppScoped(SettingsProperty property)
		{
			return CollectionUtils.Contains(property.Attributes.Values, attribute => attribute is ApplicationScopedSettingAttribute);
		}

		public static string GetDescription(SettingsProperty property)
		{
			SettingsDescriptionAttribute a = CollectionUtils.SelectFirst(property.Attributes,
						attribute => attribute is SettingsDescriptionAttribute) as SettingsDescriptionAttribute;

			return a == null ? "" : a.Description;
		}
	}

	internal class SettingsPropertyHelper 
	{
		public static SettingScope GetScope(SettingsProperty property)
		{
			return SettingsPropertyExtensions.IsUserScoped(property) ? SettingScope.User : SettingScope.Application;
		}

		public static SettingsPropertyDescriptor GetDescriptor(SettingsProperty property)
		{
			return new SettingsPropertyDescriptor(property.Name, property.PropertyType.FullName,
				SettingsPropertyExtensions.GetDescription(property), GetScope(property), property.DefaultValue as string);
		}
	}
}
