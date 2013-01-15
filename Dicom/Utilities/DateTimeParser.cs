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
	    /// TODO (CR Sep 2011): Are we sure this is 100% correct?
		public static readonly string DicomFullDateTimeFormatWithTimeZone = "yyyyMMddHHmmss.ffffff&zzzz";
		public static readonly string DicomFullDateTimeFormat = "yyyyMMddHHmmss.ffffff";

		private static readonly char[] _plusMinus = { '+', '-' };

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
            var formats = new[] { "yyyy", "yyyyMM", "yyyyMMdd" };
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
					SetDateAttributeValues(value,dicomAttributeProvider[dicomDateTag]);
				else
					SetDateTimeAttributeValues(value, dicomAttributeProvider[dicomDateTag], dicomAttributeProvider[dicomTimeTag]);
			}
		}

	}
}