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
	internal enum SHGNO : uint
	{
		SHGDN_NORMAL = 0x0000, // Default (display purpose)
		SHGDN_INFOLDER = 0x0001, // Displayed under a folder (relative)
		SHGDN_FOREDITING = 0x1000, // For in-place editing
		SHGDN_FORADDRESSBAR = 0x4000, // UI friendly parsing name (remove ugly stuff)
		SHGDN_FORPARSING = 0x8000, // Parsing name for ParseDisplayName()
	}
}

// ReSharper restore InconsistentNaming