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

// ReSharper disable InconsistentNaming

using System;

namespace ClearCanvas.Controls.WinForms.Native
{
	[Flags]
	internal enum SHCONTF : uint
	{
		SHCONTF_FOLDERS = 0x0020, // Only want folders enumerated (SFGAO_FOLDER)
		SHCONTF_NONFOLDERS = 0x0040, // Include non folders
		SHCONTF_INCLUDEHIDDEN = 0x0080, // Show items normally hidden
		SHCONTF_INIT_ON_FIRST_NEXT = 0x0100, // Allow EnumObject() to return before validating enum
		SHCONTF_NETPRINTERSRCH = 0x0200, // Hint that client is looking for printers
		SHCONTF_SHAREABLE = 0x0400, // Hint that client is looking sharable resources (remote shares)
		SHCONTF_STORAGE = 0x0800, // Include all items with accessible storage and their ancestors
	}
}

// ReSharper restore InconsistentNaming