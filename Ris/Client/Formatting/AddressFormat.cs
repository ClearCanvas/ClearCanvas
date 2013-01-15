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
    public static class AddressFormat
    {
        /// <summary>
        /// Formats the address according to the default format as specified in <see cref="FormatSettings"/>
        /// </summary>
        /// <param name="hc"></param>
        /// <returns></returns>
        public static string Format(AddressDetail addr)
        {
            return Format(addr, FormatSettings.Default.AddressDefaultFormat);
        }

        /// <summary>
        /// Formats the address according to the specified format string.
        /// </summary>
        /// <remarks>
        /// Valid format specifiers are as follows:
        ///     %S - street address, including number and unit/apartment number
        ///     %V - city
        ///     %P - Province
        ///     %Z - Postal/Zip Code
        ///     %C - country
        /// </remarks>
        /// <param name="pn"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(AddressDetail a, string format)
        {
            string result = format;
            result = result.Replace("%S", a.Street == null ? "" : FormatStreet(a));
            result = result.Replace("%V", a.City == null ? "" : a.City);
            result = result.Replace("%P", a.Province == null ? "" : a.Province);
            result = result.Replace("%Z", a.PostalCode == null ? "" : a.PostalCode);
            result = result.Replace("%C", a.Country == null ? "" : a.Country);

            return result.Trim();
        }

        private static string FormatStreet(AddressDetail a)
        {
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(a.Unit))
            {
                sb.Append(a.Unit);
                sb.Append("-");
            }
            sb.Append(a.Street);
            return sb.ToString();
        }

    }
}
