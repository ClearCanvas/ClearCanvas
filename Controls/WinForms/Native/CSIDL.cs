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
	/// <summary>
	/// CSIDL values provide a unique system-independent way to identify special folders used frequently by applications, but which may not have the same name or location on any given system.
	/// </summary>
	[Flags]
	internal enum CSIDL : uint
	{
		/// <summary>
		/// The virtual folder containing the objects in the user's Recycle Bin.
		/// </summary>
		CSIDL_BITBUCKET = 0x000A,

		/// <summary>
		/// The virtual folder containing icons for the Control Panel applications.
		/// </summary>
		CSIDL_CONTROLS = 0x0003,

		/// <summary>
		/// The virtual folder representing the Windows desktop, the root of the namespace.
		/// </summary>
		CSIDL_DESKTOP = 0x0000,

		/// <summary>
		/// The file system directory used to physically store file objects on the desktop (not to be confused with the desktop folder itself). A typical path is C:\Documents and Settings\username\Desktop.
		/// </summary>
		CSIDL_DESKTOPDIRECTORY = 0x0010,

		/// <summary>
		/// The virtual folder representing My Computer, containing everything on the local computer: storage devices, printers, and Control Panel. The folder may also contain mapped network drives.
		/// </summary>
		CSIDL_DRIVES = 0x0011,

		/// <summary>
		/// The virtual folder representing the My Documents desktop item.
		/// </summary>
		CSIDL_MYDOCUMENTS = 0x000C,

		/// <summary>
		/// The file system directory that serves as a common repository for music files. A typical path is C:\Documents and Settings\User\My Documents\My Music.
		/// </summary>
		CSIDL_MYMUSIC = 0x000D,

		/// <summary>
		/// The file system directory that serves as a common repository for image files. A typical path is C:\Documents and Settings\username\My Documents\My Pictures.
		/// </summary>
		CSIDL_MYPICTURES = 0x0027,

		/// <summary>
		/// The file system directory that serves as a common repository for video files. A typical path is C:\Documents and Settings\username\My Documents\My Videos. 
		/// </summary>
		CSIDL_MYVIDEO = 0x000E,

		/// <summary>
		/// A file system directory containing the link objects that may exist in the My Network Places virtual folder. It is not the same as CSIDL_NETWORK, which represents the network namespace root. A typical path is C:\Documents and Settings\username\NetHood.
		/// </summary>
		CSIDL_NETHOOD = 0x0013,

		/// <summary>
		/// A virtual folder representing Network Neighborhood, the root of the network namespace hierarchy. 
		/// </summary>
		CSIDL_NETWORK = 0x0012,

		/// <summary>
		/// The virtual folder representing the My Documents desktop item. This is equivalent to CSIDL_MYDOCUMENTS. 
		/// </summary>
		CSIDL_PERSONAL = 0x0005,

		/// <summary>
		/// The virtual folder containing installed printers. 
		/// </summary>
		CSIDL_PRINTERS = 0x0004,

		/// <summary>
		/// The file system directory that contains the link objects that can exist in the Printers virtual folder. A typical path is C:\Documents and Settings\username\PrintHood. 
		/// </summary>
		CSIDL_PRINTHOOD = 0x001B,

		/// <summary>
		/// The Windows directory or SYSROOT. This corresponds to the %windir% or %SYSTEMROOT% environment variables. A typical path is C:\Windows.
		/// </summary>
		CSIDL_WINDOWS = 0x0024,
	}
}

// ReSharper restore InconsistentNaming