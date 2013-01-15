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

namespace ClearCanvas.Controls.WinForms.Native
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid(IID.IID_IShellFolder)]
	internal interface IShellFolder
	{
		// Translates a file object's or folder's display name into an item identifier list.
		// Return value: error code, if any
		[PreserveSig()]
		uint ParseDisplayName(
			IntPtr hwnd, // Optional window handle
			IntPtr pbc, // Optional bind context that controls the parsing operation. This parameter is normally set to NULL. 
			[In(), MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, // Null-terminated UNICODE string with the display name.
			out uint pchEaten, // Pointer to a ULONG value that receives the number of characters of the display name that was parsed.
			out IntPtr ppidl, // Pointer to an ITEMIDLIST pointer that receives the item identifier list for the object.
			ref uint pdwAttributes); // Optional parameter that can be used to query for file attributes. This can be values from the SFGAO enum

		// Allows a client to determine the contents of a folder by creating an item identifier enumeration object and returning its IEnumIDList interface.
		// Return value: error code, if any
		[PreserveSig()]
		uint EnumObjects(
			IntPtr hwnd, // If user input is required to perform the enumeration, this window handle should be used by the enumeration object as the parent window to take user input.
			SHCONTF grfFlags, // Flags indicating which items to include in the enumeration. For a list of possible values, see the SHCONTF enum. 
			out IEnumIDList ppenumIDList); // Address that receives a pointer to the IEnumIDList interface of the enumeration object created by this method. 

		// Retrieves an IShellFolder object for a subfolder.
		// Return value: error code, if any
		[PreserveSig()]
		uint BindToObject(
			IntPtr pidl, // Address of an ITEMIDLIST structure (PIDL) that identifies the subfolder.
			IntPtr pbc, // Optional address of an IBindCtx interface on a bind context object to be used during this operation.
			[In()] ref Guid riid, // Identifier of the interface to return. 
			out IShellFolder ppv); // Address that receives the interface pointer.

		// Requests a pointer to an object's storage interface. 
		// Return value: error code, if any
		[PreserveSig()]
		uint BindToStorage(
			IntPtr pidl, // Address of an ITEMIDLIST structure that identifies the subfolder relative to its parent folder. 
			IntPtr pbc, // Optional address of an IBindCtx interface on a bind context object to be used during this operation.
			[In()] ref Guid riid, // Interface identifier (IID) of the requested storage interface.
			[MarshalAs(UnmanagedType.Interface)] out object ppv); // Address that receives the interface pointer specified by riid.

		// Determines the relative order of two file objects or folders, given their item identifier lists. 
		// Return value: If this method is successful, the CODE field of the HRESULT contains one of the following values (the code can be retrived using the helper function GetHResultCode)...
		// A negative return value indicates that the first item should precede the second (pidl1 < pidl2). 
		// A positive return value indicates that the first item should follow the second (pidl1 > pidl2).  Zero A return value of zero indicates that the two items are the same (pidl1 = pidl2). 
		[PreserveSig()]
		int CompareIDs(
			int lParam, // Value that specifies how the comparison should be performed. The lower sixteen bits of lParam define the sorting rule.
			// The upper sixteen bits of lParam are used for flags that modify the sorting rule. values can be from the SHCIDS enum
			IntPtr pidl1, // Pointer to the first item's ITEMIDLIST structure.
			IntPtr pidl2); // Pointer to the second item's ITEMIDLIST structure.

		// Requests an object that can be used to obtain information from or interact with a folder object.
		// Return value: error code, if any
		[PreserveSig()]
		uint CreateViewObject(
			IntPtr hwndOwner, // Handle to the owner window.
			[In()] ref Guid riid, // Identifier of the requested interface.
			[MarshalAs(UnmanagedType.Interface)] out object ppv); // Address of a pointer to the requested interface. 

		// Retrieves the attributes of one or more file objects or subfolders. 
		// Return value: error code, if any
		[PreserveSig()]
		uint GetAttributesOf(
			int cidl, // Number of file objects from which to retrieve attributes. 
			out IntPtr apidl, // Address of an array of pointers to ITEMIDLIST structures, each of which uniquely identifies a file object relative to the parent folder.
			out SFGAO rgfInOut); // Address of a single ULONG value that, on entry, contains the attributes that the caller is requesting. On exit, this value contains the requested attributes that are common to all of the specified objects. this value can be from the SFGAO enum

		// Retrieves an OLE interface that can be used to carry out actions on the specified file objects or folders. 
		// Return value: error code, if any
		[PreserveSig()]
		uint GetUIObjectOf(
			IntPtr hwndOwner, // Handle to the owner window that the client should specify if it displays a dialog box or message box.
			int cidl, // Number of file objects or subfolders specified in the apidl parameter. 
			[In(), MarshalAs(UnmanagedType.LPArray)] IntPtr[]
				apidl, // Address of an array of pointers to ITEMIDLIST structures, each of which uniquely identifies a file object or subfolder relative to the parent folder.
			[In()] ref Guid riid, // Identifier of the COM interface object to return.
			IntPtr rgfReserved, // Reserved. 
			[MarshalAs(UnmanagedType.Interface)] out object ppv); // Pointer to the requested interface.

		// Retrieves the display name for the specified file object or subfolder. 
		// Return value: error code, if any
		[PreserveSig()]
		uint GetDisplayNameOf(
			IntPtr pidl, // Address of an ITEMIDLIST structure (PIDL) that uniquely identifies the file object or subfolder relative to the parent folder. 
			SHGNO uFlags, // Flags used to request the type of display name to return. For a list of possible values. 
			out STRRET pName); // Address of a STRRET structure in which to return the display name.

		// Sets the display name of a file object or subfolder, changing the item identifier in the process.
		// Return value: error code, if any
		[PreserveSig()]
		uint SetNameOf(
			IntPtr hwnd, // Handle to the owner window of any dialog or message boxes that the client displays.
			IntPtr pidl, // Pointer to an ITEMIDLIST structure that uniquely identifies the file object or subfolder relative to the parent folder. 
			[In(), MarshalAs(UnmanagedType.LPWStr)] string pszName, // Pointer to a null-terminated string that specifies the new display name. 
			SHGNO uFlags, // Flags indicating the type of name specified by the lpszName parameter. For a list of possible values, see the description of the SHGNO enum. 
			out IntPtr ppidlOut); // Address of a pointer to an ITEMIDLIST structure which receives the new ITEMIDLIST. 
	}
}

// ReSharper restore InconsistentNaming