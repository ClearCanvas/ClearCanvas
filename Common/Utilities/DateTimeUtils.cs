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
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Provides convenient utilities for working with <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeUtils
    {
        /// <summary>
        /// Parses an ISO 8601 formatted date string, without milliseconds or timezone.
        /// </summary>
        /// <param name="isoDateString"></param>
        /// <returns></returns>
        public static DateTime? ParseISO(string isoDateString)
        {
            if (string.IsNullOrEmpty(isoDateString))
                return null;

            return DateTime.ParseExact(isoDateString, "s", null);
        }

        /// <summary>
        /// Formats the specified <see cref="DateTime"/> as ISO 8601, without milliseconds or timezone.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string FormatISO(DateTime dt)
        {
            return dt.ToString("s");
        }

        public static TimeSpan? ParseTimeSpan(string timeSpan)
        {
            if (string.IsNullOrEmpty(timeSpan))
                return null;

            return TimeSpan.ParseExact(timeSpan, "c", null);
        }

        public static string FormatTimeSpan(TimeSpan ts)
        {
            return ts.ToString("c");
        }
    }
}
