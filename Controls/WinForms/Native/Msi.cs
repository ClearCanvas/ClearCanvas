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

using System.Runtime.InteropServices;
using System.Text;

namespace ClearCanvas.Controls.WinForms.Native
{
	internal static class Msi
	{
		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		public static extern uint MsiGetShortcutTargetW(
			string szShortcutTarget,
			[Out] StringBuilder szProductCode,
			[Out] StringBuilder szFeatureId,
			[Out] StringBuilder szComponentCode);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		public static extern uint MsiGetComponentPath(
			string szProduct,
			string szComponent,
			[Out] StringBuilder lpPathBuf,
			ref uint pcchBuf);
	}
}

// ReSharper restore InconsistentNaming