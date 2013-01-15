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

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Utility class that assists with formatting objects for display.
    /// </summary>
    public static class Format
    {
        /// <summary>
        /// Gets or sets the default date format string.
        /// </summary>
        public static string DateFormat
        {
            get { return FormatSettings.Default.DateFormat; }
			set 
			{
				FormatSettings.Default.DateFormat = value;
				FormatSettings.Default.Save();
			}
        }

        /// <summary>
        /// Gets or sets the default time format string.
        /// </summary>
        public static string TimeFormat
        { 
            get { return FormatSettings.Default.TimeFormat; }
			set
			{
				FormatSettings.Default.TimeFormat = value;
				FormatSettings.Default.Save();
			}
        }

        /// <summary>
        /// Gets or sets the default date-time format string.
        /// </summary>
        public static string DateTimeFormat
        {
            get { return !string.IsNullOrEmpty(FormatSettings.Default.DateTimeFormat) ? FormatSettings.Default.DateTimeFormat : DateFormat + ' ' + TimeFormat; }
			set
			{ 
				FormatSettings.Default.DateTimeFormat = value;
				FormatSettings.Default.Save();
			}
        }

        /// <summary>
		/// Formats the specified <see cref="System.DateTime"/> as a date.
        /// </summary>
        public static string Date(DateTime dt)
        {
            return dt.ToString(DateFormat);
        }

        /// <summary>
		/// Formats the specified <see cref="System.DateTime"/> as a date, returning an empty string if null.
        /// </summary>
        public static string Date(DateTime? dt)
        {
            return dt == null ? "" : Date(dt.Value);
        }

        /// <summary>
        /// Formats the specific <see cref="System.DateTime"/> as a date, descriptive if set by condition, returns an empty string if input date is null.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="descriptive"></param>
        /// <returns></returns>
        public static string Date(DateTime? dt, bool descriptive)
        {
            if(descriptive && dt != null)
            {
                DateTime? today = System.DateTime.Today;

                if(FormatSettings.Default.DescriptiveDateThresholdInDays < 0)
                    FormatSettings.Default.DescriptiveDateThresholdInDays = 0;

            	//TODO (cr Oct 2009): 86399?  What's this magic#?
                if (FormatSettings.Default.DescriptiveFormattingEnabled &&  
                    dt >= today.Value.AddDays(-FormatSettings.Default.DescriptiveDateThresholdInDays) &&
                    dt <= today.Value.AddDays(FormatSettings.Default.DescriptiveDateThresholdInDays) + TimeSpan.FromSeconds(86399))
                {
                    DateTime? yesterday = today.Value.AddDays(-1);
                    DateTime? tomorrow = today.Value.AddDays(1);
                    DateTime? afterTomorrow = tomorrow.Value.AddDays(1);

					//TODO (CR Oct 2009): strings to resources.
                    if (dt < yesterday)
                    {
                        return (int) Math.Ceiling(((today.Value - dt.Value).TotalDays)) + " days ago";
                    }
                    else if (dt >= yesterday && dt < today)
                    {
                        return "Yesterday " + Time(dt);
                    }
                    else if (dt >= today && dt < tomorrow)
                    {
                        return "Today " + Time(dt);
                    }
                    else if (dt >= tomorrow && dt < afterTomorrow)
                    {
                        return "Tomorrow " + Time(dt);
                    }
                    else
                    {
                        return (dt - today).Value.Days + " days from now";
                    }
                }
                else
                {
                    return Date(dt);
                }
            }
            else
            {
                return Date(dt);
            }
        }

        /// <summary>
		/// Formats the specified <see cref="System.DateTime"/> as a time.
        /// </summary>
        public static string Time(DateTime dt)
        {
            return dt.ToString(TimeFormat);
        }

        /// <summary>
		/// Formats the specified <see cref="System.DateTime"/> as a time, returning an empty string if null.
        /// </summary>
        public static string Time(DateTime? dt)
        {
            return dt == null ? "" : dt.Value.ToString(TimeFormat);
        }

        /// <summary>
		/// Formats the specified <see cref="System.DateTime"/> as a date + time.
        /// </summary>
        public static string DateTime(DateTime dt)
        {
            return dt.ToString(DateTimeFormat);
        }

        /// <summary>
		/// Formats the specified <see cref="System.DateTime"/> as a date + time.
        /// </summary>
        public static string DateTime(DateTime? dt)
        {
            return dt == null ? "" : dt.Value.ToString(DateTimeFormat);
        }
    }
}
