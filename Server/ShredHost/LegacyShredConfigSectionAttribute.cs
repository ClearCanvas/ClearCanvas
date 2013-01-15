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

namespace ClearCanvas.Server.ShredHost
{
	/// <summary>
	/// Defines the method to be called to manually migrate the legacy shred configuration section class into the new settings class.
	/// </summary>
	public interface IMigrateLegacyShredConfigSection
	{
		void Migrate();
	}

	/// <summary>
	/// Declares the section path in legacy shred configuration files that is handled by this settings type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class LegacyShredConfigSectionAttribute : Attribute
	{
		private readonly string _sourceSectionPath;

		public LegacyShredConfigSectionAttribute(string sourceSectionPath)
		{
			_sourceSectionPath = sourceSectionPath;
		}

		public static bool IsMatchingLegacyShredConfigSectionType(Type configurationSectionType, string sectionPath)
		{
			foreach (LegacyShredConfigSectionAttribute attribute in GetCustomAttributes(configurationSectionType, typeof (LegacyShredConfigSectionAttribute), false))
			{
				if (attribute._sourceSectionPath == sectionPath)
					return true;
			}
			return false;
		}
	}
}