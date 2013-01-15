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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Formatting
{
    public static class ExternalPractitionerContactPointFormat
    {
        /// <summary>
        /// Formats the address according to the default format as specified in <see cref="FormatSettings"/>
        /// </summary>
        public static string Format(ExternalPractitionerContactPointDetail cp)
        {
            return Format(cp, FormatSettings.Default.ExternalPractitionerContactPointDefaultFormat);
        }

        /// <summary>
        /// Formats the address according to the specified format string.
        /// </summary>
        /// <remarks>
        /// Valid format specifiers are as follows:
        ///     %N - contact point name
        ///     %D - contact point description
        ///     %A - contact point address
        ///     %F - contact point fax
        ///     %T - contact point phone
        ///     %E - contact point email address
        /// </remarks>
        /// <returns></returns>
        public static string Format(ExternalPractitionerContactPointDetail cp, string format)
        {
            var result = format;
            result = result.Replace("%N", StringUtilities.EmptyIfNull(cp.Name));
            result = result.Replace("%D", StringUtilities.EmptyIfNull(cp.Description));
            result = result.Replace("%A", cp.CurrentAddress == null ? "" : AddressFormat.Format(cp.CurrentAddress));
            result = result.Replace("%F", cp.CurrentFaxNumber == null ? "" : TelephoneFormat.Format(cp.CurrentFaxNumber));
            result = result.Replace("%T", cp.CurrentPhoneNumber == null ? "" : TelephoneFormat.Format(cp.CurrentPhoneNumber));
            result = result.Replace("%E", cp.CurrentEmailAddress == null ? "" : cp.CurrentEmailAddress.Address);

            return result.Trim();
        }
    }
}
