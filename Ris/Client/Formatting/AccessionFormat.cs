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

using ClearCanvas.Ris.Client;
using System.Text;

namespace ClearCanvas.Ris.Client.Formatting
{
	public static class AccessionFormat
	{
		/// <summary>
		/// Formats the Accession number according to the default format as specified in <see cref="FormatSettings"/>
		/// </summary>
		/// <param name="accession"></param>
		/// <returns></returns>
		public static string Format(string accession)
		{
			return Format(accession, FormatSettings.Default.AccessionNumberDefaultFormat);
		}

		/// <summary>
		/// Formats the Accession number according to the specified format string
		/// </summary>
		/// <remarks>
		/// Valid format specifiers are as follows:
		///		%L - accession number label as specified in <see cref="FormatSettings"/>
		///		%N - number masked as specified in <see cref="FormatSettings"/>
		///		%n - number without mask
		/// </remarks>
		/// <param name="accession"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string Format(string accession, string format)
		{
			string result = format;

			result = result.Replace("%L", FormatSettings.Default.AccessionNumberLabel ?? "");
			result = result.Replace("%N", StringMask.Apply(accession, FormatSettings.Default.AccessionNumberMask) ?? "");
			result = result.Replace("%n", accession ?? "");

			return result.Trim();
		}

	}
}
