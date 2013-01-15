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
using System.Runtime.InteropServices;

namespace ClearCanvas.Controls.WinForms
{
	/// <summary>
	/// Represents the system HIMAGELIST for this process.
	/// </summary>
	internal sealed class SystemImageList
	{
		public static readonly SystemImageList LargeIcons = new SystemImageList(false);
		public static readonly SystemImageList SmallIcons = new SystemImageList(true);

		private readonly IntPtr _handle = IntPtr.Zero;

		private SystemImageList(bool useSmallIcons)
		{
			//JY: We use static instances here without IDisposable here because the system image list is allocated
			// to us by shell32, and we only get one per process for a given icon size. Even doing an ImageList_Destroy
			// in a destructor can cause funny business.
			const int FILE_ATTRIBUTE_NORMAL = 0x80;

			// retrieve the info for a fake file so we can get the image list handle.
			Native.SHFILEINFO shInfo = new Native.SHFILEINFO();
			Native.SHGFI uFlags = Native.SHGFI.SHGFI_USEFILEATTRIBUTES | Native.SHGFI.SHGFI_SYSICONINDEX;
			if (useSmallIcons)
				uFlags |= Native.SHGFI.SHGFI_SMALLICON;
			else
				uFlags |= Native.SHGFI.SHGFI_LARGEICON;
			_handle = Native.Shell32.SHGetFileInfo(".txt", FILE_ATTRIBUTE_NORMAL, out shInfo, (uint) Marshal.SizeOf(shInfo), uFlags);
		}

		public IntPtr Handle
		{
			get
			{
				if (IntPtr.Zero.Equals(_handle))
					throw new Exception("Unable to retrieve system image list handle.");
				return _handle;
			}
		}

		public static implicit operator IntPtr(SystemImageList imageList)
		{
			if (imageList == null)
				throw new ArgumentNullException("imageList");
			if (IntPtr.Zero.Equals(imageList._handle))
				throw new InvalidCastException("Unable to retrieve system image list handle.");
			return imageList._handle;
		}
	}
}