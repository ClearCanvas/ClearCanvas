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

namespace ClearCanvas.Dicom.Utilities.StudyBuilder
{
	/// <summary>
	/// Internal data conversion class for dates, times, datetimes, and patient sex codestrings
	/// </summary>
	internal static class DicomConverter
	{
		/// <summary>
		/// Combines separate date and time values into a single datetime, using a default value if both components are null
		/// </summary>
		/// <param name="date"></param>
		/// <param name="time"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static DateTime GetDateTime(DateTime? date, DateTime? time, DateTime defaultValue)
		{
			if (date.HasValue)
			{
				if (time.HasValue)
					return date.Value.Add(time.Value.TimeOfDay);
				else
					return date.Value;
			}
			else
			{
				if (time.HasValue)
					return defaultValue.Add(time.Value.TimeOfDay);
				else
					return defaultValue;
			}
		}

		/// <summary>
		/// Combines separate date and time values into a single datetime, using null if both components are null 
		/// </summary>
		/// <param name="date"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public static DateTime? GetDateTime(DateTime? date, DateTime? time)
		{
			if (date.HasValue)
			{
				if (time.HasValue)
					return date.Value.Add(time.Value.TimeOfDay);
				else
					return date.Value;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Gets a <see cref="PatientSex"/> enumeration based on a CS attribute value, using <see cref="PatientSex.Undefined"/> for any unrecognized code strings.
		/// </summary>
		/// <param name="codestring"></param>
		/// <returns></returns>
		public static PatientSex GetSex(string codestring)
		{
			if (codestring == null)
				return PatientSex.Undefined;
			switch (codestring.PadRight(1).Substring(0, 1).ToUpperInvariant())
			{
				case "M":
					return PatientSex.Male;
				case "F":
					return PatientSex.Female;
				case "O":
					return PatientSex.Other;
				default:
					return PatientSex.Undefined;
			}
		}

		/// <summary>
		/// Gets a patient sex CS string based on a <see cref="PatientSex"/> enumeration, using an empty string for <see cref="PatientSex.Undefined"/>
		/// </summary>
		/// <param name="sex"></param>
		/// <returns></returns>
		public static string SetSex(PatientSex sex)
		{
			switch (sex)
			{
				case PatientSex.Male:
					return "M";
				case PatientSex.Female:
					return "F";
				case PatientSex.Other:
					return "O";
				case PatientSex.Undefined:
				default:
					return "";
			}
		}

		/// <summary>
		/// Temporary workaround code in place of DicomAttribute.SetDateTime(0, value) which can throw a IndexOutOfRangeException if attribute is null
		/// </summary>
		/// <param name="attrib"></param>
		/// <param name="value"></param>
		public static void SetDate(DicomAttribute attrib, DateTime? value)
		{
			//TODO - replace this with DicomAttribute.SetDateTime(0, value) when it is fixed (ticket #1411)
			if(value.HasValue)
				attrib.SetString(0, DateParser.ToDicomString(value.Value));
			else
				attrib.SetNullValue();
		}

		/// <summary>
		/// Temporary workaround code in place of DicomAttribute.SetDateTime(0, value) which can throw a IndexOutOfRangeException if attribute is null
		/// </summary>
		/// <param name="attrib"></param>
		/// <param name="value"></param>
		public static void SetTime(DicomAttribute attrib, DateTime? value) {
			//TODO - replace this with DicomAttribute.SetDateTime(0, value) when it is fixed (ticket #1411)
			if (value.HasValue)
				attrib.SetString(0, string.Format(TimeParser.DicomFullTimeFormat, value.Value));
			else
				attrib.SetNullValue();
		}

		/// <summary>
		/// Temporary workaround code in place of DicomAttribute.SetInt32(0, value) which can throw a IndexOutOfRangeException if attribute is null
		/// </summary>
		/// <param name="attrib"></param>
		/// <param name="value"></param>
		public static void SetInt32(DicomAttribute attrib, int value)
		{
			//TODO - replace this with DicomAttribute.SetInt32(0, value) when it is fixed (ticket #1411)
			attrib.SetString(0, value.ToString());
		}
	}
}