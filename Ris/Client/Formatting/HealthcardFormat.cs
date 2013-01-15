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
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Formatting
{
    public static class HealthcardFormat
    {
        /// <summary>
        /// Formats the healthcard according to the default format as specified in <see cref="FormatSettings"/>
        /// </summary>
        /// <param name="hc"></param>
        /// <returns></returns>
        public static string Format(HealthcardDetail hc)
        {
            return Format(hc, FormatSettings.Default.HealthcardDefaultFormat);
        }

        /// <summary>
        /// Formats the healthcard number according to the specified format string.
        /// </summary>
        /// <remarks>
        /// Valid format specifiers are as follows:
        ///     %N - number
        ///     %A - assigning authority
        ///     %V - version code
        ///     %X - expiry date
        /// </remarks>
        /// <param name="hc"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(HealthcardDetail hc, string format)
        {
            string result = format;
            result = result.Replace("%N", hc.Id ?? "");
            result = result.Replace("%A", hc.AssigningAuthority == null ? "" : hc.AssigningAuthority.Code);
            result = result.Replace("%V", hc.VersionCode ?? "");
            result = result.Replace("%X", ClearCanvas.Desktop.Format.Date(hc.ExpiryDate));

            return result.Trim();
        }
    }
}
