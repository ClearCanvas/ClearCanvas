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

using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Formatting
{
	public static class StaffNameAndRoleFormat
	{
		/// <summary>
		/// Formats the staff name and role similar to "Test, Name (Role)".  Name is formatted according to the default person name format as 
		/// specified in <see cref="FormatSettings"/>
		/// </summary>
		/// <param name="staff"></param>
		/// <returns></returns>
		public static string Format(StaffSummary staff)
		{
			return Format(staff, FormatSettings.Default.PersonNameDefaultFormat);
		}

		/// <summary>
		/// Formats the staff name and role similar to "Test, Name (Role)".  Name is formatted according to the default person name format as 
		/// specified in <see cref="FormatSettings"/>
		/// </summary>
		/// <param name="staff"></param>
		/// <returns></returns>
		public static string Format(StaffDetail staff)
		{
			return Format(staff, FormatSettings.Default.PersonNameDefaultFormat);
		}

		/// <summary>
		/// Formats the staff name and role similar to "Test, Name (Role)" with the name formatted according to the specified format string.
		/// </summary>
		/// <remarks>
		/// Valid format specifiers are as follows:
		///     %F - full family name
		///     %f - family name initial
		///     %G - full given name
		///     %g - given name initial
		///     %M - full middle name
		///     %m - middle initial
		/// </remarks>
		/// <param name="staff"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string Format(StaffSummary staff, string format)
		{
			return string.Format("{0} ({1})", PersonNameFormat.Format(staff.Name, format), staff.StaffType.Value);
		}

		/// <summary>
		/// Formats the staff name and role similar to "Test, Name (Role)" with the name formatted according to the specified format string.
		/// </summary>
		/// <remarks>
		/// Valid format specifiers are as follows:
		///     %F - full family name
		///     %f - family name initial
		///     %G - full given name
		///     %g - given name initial
		///     %M - full middle name
		///     %m - middle initial
		/// </remarks>
		/// <param name="staff"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string Format(StaffDetail staff, string format)
		{
			return string.Format("{0} ({1})", PersonNameFormat.Format(staff.Name, format), staff.StaffType.Value);
		}
	}
}
