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

namespace ClearCanvas.Controls.WinForms
{
	/// <summary>
	/// Specifies the formatting style for displaying file sizes.
	/// </summary>
	public enum FileSizeFormat
	{
		/// <summary>
		/// Specifies that file sizes should be displayed in binary octet units using binary prefixes (i.e. KiB = 2<sup>10</sup> bytes, MiB = 2<sup>20</sup> bytes, etc.).
		/// </summary>
		BinaryOctets,

		/// <summary>
		/// Specifies that file sizes should be displayed in binary octet units using legacy prefixes (i.e. KB = 2<sup>10</sup> bytes, MB = 2<sup>20</sup> bytes, etc.).
		/// </summary>
		LegacyOctets,

		/// <summary>
		/// Specifies that file sizes should be displayed in metric octet units using SI prefixes (i.e. KB = 10<sup>3</sup> bytes, MB = 10<sup>6</sup> bytes, etc.).
		/// </summary>
		MetricOctets
	}
}