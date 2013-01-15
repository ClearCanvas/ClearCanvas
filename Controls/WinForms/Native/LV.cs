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

namespace ClearCanvas.Controls.WinForms.Native
{
	internal static class LV
	{
		public const int LVS_SMALLICON = 0x0002;
		public const int LVS_SHAREIMAGELISTS = 0x0040;
		public const int LVM_SETIMAGELIST = 0x1003;

		public const int LVSIL_NORMAL = 0;
		public const int LVSIL_SMALL = 1;
		public const int LVSIL_STATE = 2;
	}
}

// ReSharper restore InconsistentNaming