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

namespace ClearCanvas.Dicom.Utilities
{
	/// <summary>
	/// Will parse a date range adhering to the dicom format.  For example:
	/// 
	///		DateRange				From (parsed)		To (parsed)			Range?
	/// ----------------------------------------------------------------------------
	///		20070606				20070606			-					No
	/// 	20070606-				20070606			-					Yes
	/// 	-20070606				Beginning of time	20070606			Yes
	///		20060101-20070606		20060101			20070606			Yes
	/// </summary>
	public sealed class DateRangeHelper
	{
		private DateRangeHelper()
		{ 
		}

		/// <summary>
		/// The semantics of the fromDate and toDate, is:
		/// <table>
		/// <tr><td>fromDate</td><td>toDate</td><td>Query</td></tr>
		/// <tr><td>null</td><td>null</td><td>Empty</td></tr>
		/// <tr><td>20060608</td><td>null</td><td>Since: "20060608-"</td></tr>
		/// <tr><td>20060608</td><td>20060610</td><td>Between: "20060608-20060610"</td></tr>
		/// <tr><td>null</td><td>20060610</td><td>Prior to: "-20060610"</td></tr>
		/// </table>
		/// </summary>
		/// <param name="fromDate"></param>
		/// <param name="toDate"></param>
		public static string GetDicomDateRangeQueryString(DateTime? fromDate, DateTime? toDate)
		{
			if (null == fromDate && null == toDate)
			{
				return "";
			}
			else if (fromDate == toDate)
			{
				return ((DateTime)fromDate).ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
			}
			else if (null != fromDate && null == toDate)
			{
				return ((DateTime)fromDate).ToString("yyyyMMdd-", System.Globalization.CultureInfo.InvariantCulture);
			}
			else if (null != fromDate && null != toDate)
			{
				return ((DateTime)fromDate).ToString("yyyyMMdd-", System.Globalization.CultureInfo.InvariantCulture)
				       + ((DateTime)toDate).ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
			}
			else if (null == fromDate && null != toDate)
			{
				return ((DateTime)toDate).ToString("-yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
			}

			return "";
		}

		/// <summary>
		/// Will parse a date range adhering to the dicom date format, returning the dates as <see cref="DateTime"/> objects.
		/// </summary>
		/// <param name="dateRange">the string to be parsed</param>
		/// <param name="fromDate">the "from date", or null</param>
		/// <param name="toDate">the "to date" or null</param>
		/// <param name="isRange">whether or not the input value was actually a range.  If not, then the "from date" value should be taken 
		/// to be an exact value, not a range, depending on the application.</param>
		/// <exception cref="InvalidOperationException">if the input range is poorly formatted</exception>
		public static void Parse(string dateRange, out DateTime? fromDate, out DateTime? toDate, out bool isRange)
		{
			try
			{
				fromDate = null;
				toDate = null;
				isRange = false;

				if (dateRange == null)
					return;

				string fromDateString = "", toDateString = "";
				string[] splitRange = dateRange.Split('-');

				if (splitRange.Length == 1)
				{
					fromDateString = splitRange[0];
				}
				else if (splitRange.Length == 2)
				{
					fromDateString = splitRange[0];
					toDateString = splitRange[1];
					isRange = true;
				}
				else
				{
					throw new InvalidOperationException(string.Format(SR.ExceptionPoorlyFormattedDateRange, dateRange));
				}

				DateTime outDate;

				if (fromDateString == "")
				{
					fromDate = null;
				}
				else
				{
					if (!DateParser.Parse(fromDateString, out outDate))
						throw new InvalidOperationException(string.Format(SR.ExceptionPoorlyFormattedDateRange, dateRange));

					fromDate = outDate;
				}


				if (toDateString == "")
				{
					toDate = null;
				}
				else
				{
					if (!DateParser.Parse(toDateString, out outDate))
						throw new InvalidOperationException(string.Format(SR.ExceptionPoorlyFormattedDateRange, dateRange));

					toDate = outDate;
				}

				if (fromDate != null && toDate != null)
				{
					if (fromDate > toDate)
						throw new InvalidOperationException(string.Format(SR.ExceptionPoorlyFormattedDateRange, dateRange));
				}
			}
			catch
			{
				fromDate = toDate = null;
				throw;
			}
		}

		/// <summary>
		/// Will parse a date range adhering to the dicom date format, returning the dates as integers.
		/// </summary>
		/// <param name="dateRange">the string to be parsed</param>
		/// <param name="fromDate">the "from date", or null</param>
		/// <param name="toDate">the "to date" or null</param>
		/// <param name="isRange">whether or not the input value was actually a range.  If not, then the "from date" value should be taken 
		/// to be an exact value, not a range, depending on the application.</param>
		public static void Parse(string dateRange, out int fromDate, out int toDate, out bool isRange)
		{
			string fromDateString, toDateString;
			Parse(dateRange, out fromDateString, out toDateString, out isRange);

			if (fromDateString == "")
			{
				fromDate = 0;
			}
			else
			{
				//the string is guaranteed to be formatted like "yyyyMMdd", so this is safe.
				fromDate = Convert.ToInt32(fromDateString, System.Globalization.CultureInfo.InvariantCulture);
			}

			if (toDateString == "")
			{
				toDate = 0;
			}
			else
			{
				//the string is guaranteed to be formatted like "yyyyMMdd", so this is safe.
				toDate = Convert.ToInt32(toDateString, System.Globalization.CultureInfo.InvariantCulture);
			}
		}

		/// <summary>
		/// Will parse a date range adhering to the dicom date format, returning the dates as strings.  In the case where the input
		/// dates are formatted according to the old Dicom Standard (yyyy.MM.dd), the resulting strings will be reformatted according
		/// to the current Dicom Standard.
		/// </summary>
		/// <param name="dateRange">the string to be parsed</param>
		/// <param name="fromDate">the "from date", or null</param>
		/// <param name="toDate">the "to date" or null</param>
		/// <param name="isRange">whether or not the input value was actually a range.  If not, then the "from date" value should be taken 
		/// to be an exact value, not a range, depending on the application.</param>
		public static void Parse(string dateRange, out string fromDate, out string toDate, out bool isRange)
		{
			DateTime? from, to;
			Parse(dateRange, out from, out to, out isRange);

			fromDate = (from != null) ? ((DateTime)from).ToString(DateParser.DicomDateFormat, System.Globalization.CultureInfo.InvariantCulture) : "";
			toDate = (to != null) ? ((DateTime)to).ToString(DateParser.DicomDateFormat, System.Globalization.CultureInfo.InvariantCulture) : "";
		}
	}
}