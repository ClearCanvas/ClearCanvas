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
using System.Globalization;

namespace ClearCanvas.Dicom.Utilities
{
	public static class DateTimeParser
	{
		// TODO (CR Sep 2011): Are we sure this is 100% correct?
		public static readonly string DicomFullDateTimeFormatWithTimeZone = "yyyyMMddHHmmss.ffffff&zzzz";
		public static readonly string DicomFullDateTimeFormat = "yyyyMMddHHmmss.ffffff";

		private static readonly char[] _plusMinus = {'+', '-'};

		/// <summary>
		/// Attempts to parse the time string exactly, according to accepted Dicom datetime format(s).
		/// Will *not* throw an exception if the format is invalid (better for when performance is needed).
		/// </summary>
		/// <param name="dateTimeString">the dicom datetime string</param>
		/// <returns>a nullable DateTime</returns>
		public static DateTime? Parse(string dateTimeString)
		{
			DateTime dateTime;
			if (!Parse(dateTimeString, out dateTime))
				return null;

			return dateTime;
		}

		/// <summary>
		/// Parses a dicom Date/Time. The Hour/Minute adjustment factor (as
		/// specified in Dicom for universal time adjustment) is accounted for 
		/// (and parsed) by this function.
		/// </summary>
		/// <param name="dicomDateTime">the dicom date/time string</param>
		/// <param name="dateTime">the date/time as a DateTime object</param>
		/// <returns>true on success, false otherwise</returns>
		public static bool Parse(string dicomDateTime, out DateTime dateTime)
		{
			dateTime = new DateTime();

			if (String.IsNullOrEmpty(dicomDateTime))
				return false;

			int plusMinusIndex = dicomDateTime.IndexOfAny(_plusMinus);
			string dateTimeString = dicomDateTime;

			string offsetString = String.Empty;
			if (plusMinusIndex > 0)
			{
				//It has to be at least beyond the "day" ... there's probably even more complex rules, but I don't think we need to go nuts.
				if (plusMinusIndex < 8)
					return false;

				offsetString = dateTimeString.Substring(plusMinusIndex);
				dateTimeString = dateTimeString.Remove(plusMinusIndex);
			}

			string dateString;
			string timeString = String.Empty;
			if (dateTimeString.Length >= 8)
			{
				dateString = dateTimeString.Substring(0, 8);
				timeString = dateTimeString.Substring(8);
			}
			else
			{
				dateString = dateTimeString;
			}

			int hourOffset = 0;
			int minuteOffset = 0;
			if (!String.IsNullOrEmpty(offsetString))
			{
				if (offsetString.Length > 3)
				{
					if (!Int32.TryParse(offsetString.Substring(3), NumberStyles.Integer, CultureInfo.InvariantCulture, out minuteOffset))
						return false;

					if (!Int32.TryParse(offsetString.Remove(3), NumberStyles.Integer, CultureInfo.InvariantCulture, out hourOffset))
						return false;
				}
				else
				{
					if (!Int32.TryParse(offsetString, NumberStyles.Integer, CultureInfo.InvariantCulture, out hourOffset))
						return false;
				}

				minuteOffset *= Math.Sign(hourOffset);
			}

			DateTime date;
			var formats = new[] {"yyyy", "yyyyMM", "yyyyMMdd"};
			//We do this independently of DateParser here because the rules are slightly different than for a date.
			//In DICOM, DT values can exclude the month and/or day, whereas DA values cannot.
			//The rules for the time component are the same as for TM, so we use TimeParser.
			if (!DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
				return false;

			DateTime time = new DateTime(); //zero datetime
			if (!String.IsNullOrEmpty(timeString))
			{
				if (!TimeParser.Parse(timeString, out time))
					return false;
			}

			dateTime = date;
			dateTime = dateTime.AddHours(hourOffset);
			dateTime = dateTime.AddMinutes(minuteOffset);
			dateTime = dateTime.Add(time.TimeOfDay);

			return true;
		}

		/// <summary>
		/// Convert a datetime object into a DT string
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static string ToDicomString(DateTime datetime, bool toUTC)
		{
			if (toUTC)
			{
				DateTime utc = datetime.ToUniversalTime();
				return utc.ToString(DicomFullDateTimeFormatWithTimeZone, CultureInfo.InvariantCulture);
			}
			else
			{
				return datetime.ToString(DicomFullDateTimeFormat, CultureInfo.InvariantCulture);
			}
		}

		/// <summary>
		/// Parses Dicom Date/Time tags.  The <paramref name="dicomDateTime"/> would be a DateTime tag value - such as AcquisitionDatetime,
		/// the <paramref name="dicomDate"/> would be just a Date tag value - such as AcquisitionDate; and <paramref name="dicomTime"/> would
		/// be just the Time tag value - such as AcquisitionTime.  So, this method will parse the <paramref name="dicomDateTime"/> if it is not empty,
		/// otherwise it will parse the <paramref name="dicomDate"/> and <paramref name="dicomTime"/> together.
		/// </summary>
		/// <param name="dicomDateTime">The dicom date time.</param>
		/// <param name="dicomDate">The dicom date.</param>
		/// <param name="dicomTime">The dicom time.</param>
		/// <param name="outDateTime">The date time.</param>
		/// <returns></returns>
		public static bool ParseDateAndTime(string dicomDateTime, string dicomDate, string dicomTime, out DateTime outDateTime)
		{
			outDateTime = DateTime.MinValue; // set default value

			string dateTimeValue = dicomDateTime == null ? String.Empty : dicomDateTime.Trim();

			// First try to do dateValue and timeValue separately - if both are there then set 
			// dateTimeConcat, and then parse dateTimeConcat the same as if dateTimeValue was set
			if (dateTimeValue != String.Empty)
				return Parse(dateTimeValue, out outDateTime);

			string dateValue = dicomDate == null ? String.Empty : dicomDate.Trim();
			string timeValue = dicomTime == null ? String.Empty : dicomTime.Trim();

			if (dateValue == String.Empty)
				return false;

			DateTime date;
			if (!DateParser.Parse(dateValue, out date))
				return false;

			outDateTime = date;
			if (timeValue == String.Empty)
				return true;

			//Even though we have the date, the time is wrong. The "out" parameter will still have the parsed date in it if anyone cares.
			DateTime time;
			if (!TimeParser.Parse(timeValue, out time))
				return false;

			outDateTime = outDateTime.Add(time.TimeOfDay);
			return true;
		}

		/// <summary>
		/// Parses Dicom Date/Time tags.  The <paramref name="dicomDateTime"/> would be a DateTime tag value - such as AcquisitionDatetime,
		/// the <paramref name="dicomDate"/> would be just a Date tag value - such as AcquisitionDate; and <paramref name="dicomTime"/> would
		/// be just the Time tag value - such as AcquisitionTime.  So, this method will parse the <paramref name="dicomDateTime"/> if it is not empty,
		/// otherwise it will parse the <paramref name="dicomDate"/> and <paramref name="dicomTime"/> together.
		/// </summary>
		/// <param name="dicomDateTime">The dicom date time.</param>
		/// <param name="dicomDate">The dicom date.</param>
		/// <param name="dicomTime">The dicom time.</param>
		/// <returns></returns>
		public static DateTime? ParseDateAndTime(string dicomDateTime, string dicomDate, string dicomTime)
		{
			DateTime dateTime;
			if (!ParseDateAndTime(dicomDateTime, dicomDate, dicomTime, out dateTime))
				return null;

			return dateTime;
		}

		/// <summary>
		/// Parses the date and time.  Gets the values for each tag from the attribute colllection. The <paramref name="dicomDateTime"/> would be a DateTime tag value - such as AcquisitionDatetime,
		/// the <paramref name="dicomDate"/> would be just a Date tag value - such as AcquisitionDate; and <paramref name="dicomTime"/> would
		/// be just the Time tag value - such as AcquisitionTime.  So, this method will parse the <paramref name="dicomDateTime"/> if it is not empty,
		/// otherwise it will parse the <paramref name="dicomDate"/> and <paramref name="dicomTime"/> together.
		/// </summary>
		/// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
		/// <param name="dicomDateTimeTag">The dicom date time tag.</param>
		/// <param name="dicomDateTag">The dicom date tag.</param>
		/// <param name="dicomTimeTag">The dicom time tag.</param>
		/// <returns></returns>
		public static DateTime? ParseDateAndTime(IDicomAttributeProvider dicomAttributeProvider, uint dicomDateTimeTag, uint dicomDateTag, uint dicomTimeTag)
		{
			if (dicomAttributeProvider == null)
				throw new ArgumentNullException("dicomAttributeProvider");

			string dicomDateTime = dicomDateTimeTag == 0 ? String.Empty : dicomAttributeProvider[dicomDateTimeTag].GetString(0, String.Empty);
			string dicomDate = dicomDateTag == 0 ? String.Empty : dicomAttributeProvider[dicomDateTag].GetString(0, String.Empty);
			string dicomTime = dicomTimeTag == 0 ? String.Empty : dicomAttributeProvider[dicomTimeTag].GetString(0, String.Empty);

			return ParseDateAndTime(dicomDateTime, dicomDate, dicomTime);
		}

		/// <summary>
		/// Sets the specified date time attribute values based on the specified <paramref name="value">Date Time value</paramref>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="dateAttribute">The date attribute.</param>
		/// <param name="timeAttribute">The time attribute.</param>
		public static void SetDateTimeAttributeValues(DateTime? value, DicomAttribute dateAttribute, DicomAttribute timeAttribute)
		{
			if (value.HasValue)
			{
				dateAttribute.SetDateTime(0, value.Value);
				timeAttribute.SetDateTime(0, value.Value);
			}
			else
			{
				dateAttribute.SetNullValue();
				timeAttribute.SetNullValue();
			}
		}

		/// <summary>
		/// Sets the specified date attribute values based on the specified <paramref name="value">Date value</paramref>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="dateAttribute">The date attribute.</param>
		public static void SetDateAttributeValues(DateTime? value, DicomAttribute dateAttribute)
		{
			if (value.HasValue)
			{
				dateAttribute.SetDateTime(0, value.Value);
			}
			else
			{
				dateAttribute.SetNullValue();
			}
		}

		/// <summary>
		/// Sets the date time attribute values for the specified dicom attributes.
		/// Will first attempt to write to the <paramref name="dateTimeAttribute"/> if it is not null, otherwise
		/// it will write the values to the separate date and time attributes.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="dateTimeAttribute">The date time attribute.</param>
		/// <param name="dateAttribute">The date attribute.</param>
		/// <param name="timeAttribute">The time attribute.</param>
		public static void SetDateTimeAttributeValues(DateTime? value, DicomAttribute dateTimeAttribute, DicomAttribute dateAttribute, DicomAttribute timeAttribute)
		{
			if (dateTimeAttribute != null)
			{
				if (value.HasValue)
					dateTimeAttribute.SetDateTime(0, value.Value);
				else
					dateTimeAttribute.SetNullValue();
			}
			else
			{
				SetDateTimeAttributeValues(value, dateAttribute, timeAttribute);
			}
		}

		/// <summary>
		/// Sets the date time attribute values for the specified attributes in the specified <paramref name="dicomAttributeCollection"/>.
		/// Will first attempt to write to the <paramref name="dicomDateTimeTag"/> if it is non zero, otherwise
		/// it will write the values to the separate date and time tags.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="dicomAttributeProvider">The dicom attribute provider.</param>
		/// <param name="dicomDateTimeTag">The dicom date time tag.</param>
		/// <param name="dicomDateTag">The dicom date tag.</param>
		/// <param name="dicomTimeTag">The dicom time tag.</param>
		public static void SetDateTimeAttributeValues(DateTime? value, IDicomAttributeProvider dicomAttributeProvider, uint dicomDateTimeTag, uint dicomDateTag, uint dicomTimeTag)
		{
			if (dicomAttributeProvider == null)
				throw new ArgumentNullException("dicomAttributeProvider");
			if (dicomDateTimeTag != 0)
			{
				DicomAttribute dateTimeAttribute = dicomAttributeProvider[dicomDateTimeTag];
				SetDateTimeAttributeValues(value, dateTimeAttribute, null, null);
			}
			else
			{
				if (dicomTimeTag == 0)
					SetDateAttributeValues(value, dicomAttributeProvider[dicomDateTag]);
				else
					SetDateTimeAttributeValues(value, dicomAttributeProvider[dicomDateTag], dicomAttributeProvider[dicomTimeTag]);
			}
		}

		/// <summary>
		/// Gets the date component of the DICOM DT (date/time) string as a DA (date) string.
		/// </summary>
		/// <param name="dateTimeString">A DICOM DT string.</param>
		/// <returns>The date component of the DT string as a DA string.</returns>
		public static string GetDateAttributeValues(string dateTimeString)
		{
			if (string.IsNullOrEmpty(dateTimeString)) return string.Empty;

			// DA is always YYYYMMDD, while DT could be as short as YYYY, so we default month/day to 01 if necessary by padding 0101 before we pull the date substring
			return dateTimeString.Length >= 4 ? string.Concat(dateTimeString, "0101").Substring(0, 8) : dateTimeString;
		}

		/// <summary>
		/// Gets the date value of a DICOM DT attribute or a DA attribute, formatted as a DA string.
		/// </summary>
		/// <param name="dicomAttributeProvider">The source DICOM data set.</param>
		/// <param name="dicomDateTimeTag">The DT tag.</param>
		/// <param name="dicomDateTag">The DA tag.</param>
		/// <returns></returns>
		public static string GetDateAttributeValues(IDicomAttributeProvider dicomAttributeProvider, uint dicomDateTimeTag, uint dicomDateTag)
		{
			DicomAttribute dicomAttribute;

			// if the DT attribute is present, use the date component of that
			if (dicomDateTimeTag != 0 && dicomAttributeProvider.TryGetAttribute(dicomDateTimeTag, out dicomAttribute) && !dicomAttribute.IsEmpty)
			{
				var dtString = dicomAttribute.GetString(0, string.Empty);
				if (!string.IsNullOrEmpty(dtString)) return GetDateAttributeValues(dtString);
			}

			// if the DA attribute is present, return that value
			if (dicomDateTag != 0 && dicomAttributeProvider.TryGetAttribute(dicomDateTag, out dicomAttribute))
				return dicomAttribute.GetString(0, string.Empty);
			return string.Empty;
		}

		/// <summary>
		/// Gets the time component of the DICOM DT (date/time) string as a TM (time) string.
		/// </summary>
		/// <param name="dateTimeString">A DICOM DT string.</param>
		/// <returns>The time component of the DT string as a TM string.</returns>
		public static string GetTimeAttributeValues(string dateTimeString)
		{
			if (string.IsNullOrEmpty(dateTimeString)) return string.Empty;

			// if DT string contains a time offset component, strip it off
			var offsetIndex = dateTimeString.IndexOfAny(_plusMinus);
			if (offsetIndex >= 0) dateTimeString = dateTimeString.Substring(0, offsetIndex);

			// TM must contain at least HH component, while DT could be as short as YYYY, so we check if the DT string is at least YYYYMMDDHH and pull everything starting at the HH component
			return dateTimeString.Length >= 10 ? dateTimeString.Substring(8) : string.Empty;
		}

		/// <summary>
		/// Gets the time value of a DICOM DT attribute or a TM attribute, formatted as a TM string.
		/// </summary>
		/// <param name="dicomAttributeProvider">The source DICOM data set.</param>
		/// <param name="dicomDateTimeTag">The DT tag.</param>
		/// <param name="dicomTimeTag">The TM tag.</param>
		/// <returns></returns>
		public static string GetTimeAttributeValues(IDicomAttributeProvider dicomAttributeProvider, uint dicomDateTimeTag, uint dicomTimeTag)
		{
			DicomAttribute dicomAttribute;

			// if the DT attribute is present, use the time component of that
			if (dicomDateTimeTag != 0 && dicomAttributeProvider.TryGetAttribute(dicomDateTimeTag, out dicomAttribute) && !dicomAttribute.IsEmpty)
			{
				var dtString = dicomAttribute.GetString(0, string.Empty);
				if (!string.IsNullOrEmpty(dtString)) return GetTimeAttributeValues(dtString);
			}

			// if the TM attribute is present, return that value
			if (dicomTimeTag != 0 && dicomAttributeProvider.TryGetAttribute(dicomTimeTag, out dicomAttribute))
				return dicomAttribute.GetString(0, string.Empty);
			return string.Empty;
		}

		/// <summary>
		/// Gets the concatenation of a DICOM DA (date) and TM (time) string as a DT (date/time) string/
		/// </summary>
		/// <param name="dateString">A DICOM DA string.</param>
		/// <param name="timeString">A DICOM TM string.</param>
		/// <returns>The concatenated date/time as a DT string.</returns>
		public static string GetDateTimeAttributeValues(string dateString, string timeString)
		{
			if (string.IsNullOrEmpty(dateString)) return string.Empty;

			// if DA value is valid, try concatenating with the TM value if that is present, otherwise just return the DA value
			return dateString.Length == 8 && !string.IsNullOrEmpty(timeString) ? string.Concat(dateString, timeString) : dateString;
		}

		/// <summary>
		/// Gets the date/time value of either a DICOM DT attribute, or a concatenation of a DT attribute and a TM attribute, formatted as a DT string.
		/// </summary>
		/// <param name="dicomAttributeProvider">The source DICOM data set.</param>
		/// <param name="dicomDateTimeTag">The DT tag.</param>
		/// <param name="dicomDateTag">The DA tag.</param>
		/// <param name="dicomTimeTag">The TM tag.</param>
		/// <returns></returns>
		public static string GetDateTimeAttributeValues(IDicomAttributeProvider dicomAttributeProvider, uint dicomDateTimeTag, uint dicomDateTag, uint dicomTimeTag)
		{
			DicomAttribute dicomAttribute;

			// if the DT attribute is present, return that value
			if (dicomDateTimeTag != 0 && dicomAttributeProvider.TryGetAttribute(dicomDateTimeTag, out dicomAttribute) && !dicomAttribute.IsEmpty)
				return dicomAttribute.GetString(0, string.Empty);

			// check for presence of DA attribute, and return that concatenated with the TM attribute value if it exists
			if (dicomDateTag != 0 && dicomAttributeProvider.TryGetAttribute(dicomDateTag, out dicomAttribute))
			{
				var daString = dicomAttribute.GetString(0, string.Empty);
				if (!string.IsNullOrEmpty(daString))
				{
					string tmString = null;
					if (dicomTimeTag != 0 && dicomAttributeProvider.TryGetAttribute(dicomTimeTag, out dicomAttribute) && !dicomAttribute.IsEmpty)
						tmString = dicomAttribute.GetString(0, string.Empty);
					return GetDateTimeAttributeValues(daString, tmString);
				}
			}
			return string.Empty;
		}
	}
}