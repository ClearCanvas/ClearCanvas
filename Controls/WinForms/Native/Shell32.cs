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
	/// Static class containing methods defined in shell32.dll.
	/// </summary>
	internal static class Shell32
	{
		[DllImport("shell32.dll")]
		public static extern int SHGetDesktopFolder(ref IShellFolder ppshf);

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SHGetFileInfo([MarshalAs(UnmanagedType.LPTStr)] string pszPath, uint dwFileAttribs, out SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SHGetFileInfo(IntPtr pIDL, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);

		/// <summary>
		/// Retrieves the path of a folder as an ITEMIDLIST structure.
		/// </summary>
		/// <remarks>
		/// <para>Requires at least Windows 2000 or Windows ME.</para>
		/// </remarks>
		/// <param name="hwndOwner">Reserved.</param>
		/// <param name="nFolder">A CSIDL value that identifies the folder to be located. The folders associated with the CSIDLs might not exist on a particular system.</param>
		/// <param name="hToken">
		///		An access token that can be used to represent a particular user. It is usually set to NULL, but it may be needed when there are multiple users for those folders that are treated as belonging to a single user. The most commonly used folder of this type is My Documents. The calling application is responsible for correct impersonation when hToken is non-NULL. It must have appropriate security privileges for the particular user, and the user's registry hive must be currently mounted. See Access Control for further discussion of access control issues.
		///		Assigning the hToken parameter a value of -1 indicates the Default User. This allows clients of SHGetFolderLocation to find folder locations (such as the Desktop folder) for the Default User. The Default User user profile is duplicated when any new user account is created, and includes special folders such as My Documents and Desktop. Any items added to the Default User folder also appear in any new user account.
		/// </param>
		/// <param name="dwReserved">Reserved. Must be set to 0.</param>
		/// <param name="ppidl">The address of a pointer to an item identifier list structure that specifies the folder's location relative to the root of the namespace (the desktop). The ppidl parameter is set to NULL on failure. The calling application is responsible for freeing this resource by calling ILFree.</param>
		/// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
		[DllImport("shell32.dll")]
		public static extern int SHGetFolderLocation(IntPtr hwndOwner, CSIDL nFolder, IntPtr hToken, uint dwReserved, out IntPtr ppidl);

		/// <summary>
		/// Converts an item identifier list to a file system path.
		/// </summary>
		/// <param name="pidl">Address of an item identifier list that specifies a file or directory location relative to the root of the namespace (the desktop).</param>
		/// <param name="pszPath">Address of a buffer to receive the file system path. This buffer must be at least MAX_PATH characters in size.</param>
		/// <returns>Returns TRUE if successful, or FALSE otherwise. </returns>
		[DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = false)]
		[return : MarshalAs(UnmanagedType.Bool)]
		public static extern bool SHGetPathFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszPath);

		/// <summary>
		/// Translates a Shell namespace object's display name into an item identifier list and returns the attributes of the object.
		/// This function is the preferred method to convert a string to a pointer to an item identifier list (PIDL).
		/// </summary>
		/// <remarks>
		/// <para>You should call this function from a background thread. Failure to do so could cause the user interface (UI) to stop responding.</para>
		/// <para>Requires at least Windows XP.</para>
		/// </remarks>
		/// <param name="pszName">A pointer to a zero-terminated wide string that contains the display name to parse.</param>
		/// <param name="pbc">A bind context that controls the parsing operation. This parameter is normally set to <see cref="IntPtr.Zero"/>.</param>
		/// <param name="ppidl">The address of a pointer to a variable of type ITEMIDLIST that receives the item identifier list for the object. If an error occurs, then this parameter is set to <see cref="IntPtr.Zero"/>.</param>
		/// <param name="sfgaoIn">A <see cref="uint"/> value that specifies the attributes to query. To query for one or more attributes, initialize this parameter with the flags that represent the attributes of interest. For a list of available SFGAO flags, see <see cref="IShellFolder.GetAttributesOf"/>.</param>
		/// <param name="psfgaoOut">A pointer to a <see cref="uint"/>. On return, those attributes that are true for the object and were requested in sfgaoIn are set. An object's attribute flags can be zero or a combination of SFGAO flags. For a list of available SFGAO flags, see <see cref="IShellFolder.GetAttributesOf"/>.</param>
		/// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
		[DllImport("shell32.dll")]
		public static extern int SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string pszName, IntPtr pbc, out IntPtr ppidl, SFGAO sfgaoIn, out SFGAO psfgaoOut);

		/// <summary>
		/// Gets a value indicating if the <see cref="SHParseDisplayName"/> method is supported on this system.
		/// </summary>
		public static readonly bool IsSHParseDisplayNameSupported;

		#region Static Constructor

		static Shell32()
		{
			try
			{
				OperatingSystem os = Environment.OSVersion;
				IsSHParseDisplayNameSupported = (os.Platform == PlatformID.Win32NT && os.Version.CompareTo(new Version(5, 1)) > 0); // Requires at least Windows XP
			}
			catch (InvalidOperationException)
			{
				// if the OS couldn't be retrieved, assume none of the special functions can be used
				IsSHParseDisplayNameSupported = false;
			}
		}

		#endregion
	}
}

// ReSharper restore InconsistentNaming