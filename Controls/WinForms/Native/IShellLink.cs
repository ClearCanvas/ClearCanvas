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
using System.Runtime.InteropServices;
using System.Text;

namespace ClearCanvas.Controls.WinForms.Native
{
	/// <summary>
	/// The IShellLink interface allows Shell links to be created, modified, and resolved.
	/// </summary>
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid(IID.IID_IShellLinkW)]
	internal interface IShellLink
	{
		/// <summary>
		/// Retrieves the path and file name of a Shell link object.
		/// </summary>
		void GetPath([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out WIN32_FIND_DATA pfd, SLGP_FLAGS fFlags);

		/// <summary>
		/// Retrieves the list of item identifiers for a Shell link object.
		/// </summary>
		void GetIDList(out IntPtr ppidl);

		/// <summary>
		/// Sets the pointer to an item identifier list (PIDL) for a Shell link object.
		/// </summary>
		void SetIDList(IntPtr pidl);

		/// <summary>
		/// Retrieves the description string for a Shell link object.
		/// </summary>
		void GetDescription([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);

		/// <summary>
		/// Sets the description for a Shell link object. The description can be any application-defined string.
		/// </summary>
		void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

		/// <summary>
		/// Retrieves the name of the working directory for a Shell link object.
		/// </summary>
		void GetWorkingDirectory([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);

		/// <summary>
		/// Sets the name of the working directory for a Shell link object.
		/// </summary>
		void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

		/// <summary>
		/// Retrieves the command-line arguments associated with a Shell link object.
		/// </summary>
		void GetArguments([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);

		/// <summary>
		/// Sets the command-line arguments for a Shell link object.
		/// </summary>
		void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

		/// <summary>
		/// Retrieves the hot key for a Shell link object.
		/// </summary>
		void GetHotkey(out short pwHotkey);

		/// <summary>
		/// Sets a hot key for a Shell link object.
		/// </summary>
		void SetHotkey(short wHotkey);

		/// <summary>
		/// Retrieves the show command for a Shell link object.
		/// </summary>
		void GetShowCmd(out int piShowCmd);

		/// <summary>
		/// Sets the show command for a Shell link object. The show command sets the initial show state of the window.
		/// </summary>
		void SetShowCmd(int iShowCmd);

		/// <summary>
		/// Retrieves the location (path and index) of the icon for a Shell link object.
		/// </summary>
		void GetIconLocation([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath,
		                     int cchIconPath, out int piIcon);

		/// <summary>
		/// Sets the location (path and index) of the icon for a Shell link object.
		/// </summary>
		void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

		/// <summary>
		/// Sets the relative path to the Shell link object.
		/// </summary>
		void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);

		/// <summary>
		/// Attempts to find the target of a Shell link, even if it has been moved or renamed.
		/// </summary>
		void Resolve(IntPtr hwnd, SLR_FLAGS fFlags);

		/// <summary>
		/// Sets the path and file name of a Shell link object.
		/// </summary>
		void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
	}

	/// <summary>IShellLink.GetPath fFlags: Flags that specify the type of path information to retrieve</summary>
	[Flags()]
	internal enum SLGP_FLAGS
	{
		/// <summary>Retrieves the standard short (8.3 format) file name</summary>
		SLGP_SHORTPATH = 0x1,
		/// <summary>Retrieves the Universal Naming Convention (UNC) path name of the file</summary>
		SLGP_UNCPRIORITY = 0x2,
		/// <summary>Retrieves the raw path name. A raw path is something that might not exist and may include environment variables that need to be expanded</summary>
		SLGP_RAWPATH = 0x4
	}
}

// ReSharper restore InconsistentNaming