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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using ClearCanvas.Controls.WinForms.Native;

namespace ClearCanvas.Controls.WinForms
{
	// ReSharper disable RedundantAssignment
	// (it only appears redundant to Resharper, but we're dealing with P/Invoke here)
	internal class ShellItem : IDisposable, ICloneable
	{
#if DEBUG
		private static int _instanceCount = 0;

		internal static int InstanceCount
		{
			get { return _instanceCount; }
		}
#endif

		private ShellItem _rootItem = null;
		private Native.IShellFolder _shellFolder = null;
		private Pidl _pidl = null;
		private string _displayName = string.Empty;
		private string _typeName = string.Empty;
		private bool _hasSubFolders = false;
		private bool _isVirtual = false;
		private bool _isFolder = false;
		private int _iconIndex = -1;
		private bool _disposed = false;

		public ShellItem()
		{
			_rootItem = this;
			try
			{
				// create a PIDL for the Desktop shell item.
				_pidl = new Pidl(Environment.SpecialFolder.Desktop);

				var shInfo = new Native.SHFILEINFO();
				Native.Shell32.SHGetFileInfo((IntPtr) _pidl, 0, out shInfo, (uint) Marshal.SizeOf(shInfo), SHGFI.SHGFI_PIDL | SHGFI.SHGFI_DISPLAYNAME | SHGFI.SHGFI_SYSICONINDEX);

				// get the root IShellFolder interface
				int hResult = Native.Shell32.SHGetDesktopFolder(ref _shellFolder);
				if (hResult != 0)
					Marshal.ThrowExceptionForHR(hResult);

				_displayName = shInfo.szDisplayName;
				_typeName = string.Empty;
				_iconIndex = shInfo.iIcon;
				_isFolder = true;
				_isVirtual = true;
				_hasSubFolders = true;
			}
			catch (Exception ex)
			{
				// if an exception happens during construction, we must release the PIDL now (remember, it's a pointer!)
				if (_pidl != null)
					_pidl.Dispose();
				throw new Exception("Creation of the root namespace shell item failed.", ex);
			}

#if DEBUG
			_instanceCount++;
#endif
		}

		public ShellItem(Pidl pidl, ShellItem parentShellItem) : this(pidl, parentShellItem, true) {}

		public ShellItem(Pidl pidl, ShellItem parentShellItem, bool relativePidl)
		{
			int hResult;

			_rootItem = parentShellItem._rootItem;
			try
			{
				IntPtr tempPidl;
				if (relativePidl)
				{
					_pidl = new Pidl(parentShellItem.Pidl, pidl);
					tempPidl = (IntPtr) pidl; // use the relative one from parameters
				}
				else
				{
					_pidl = pidl.Clone();
					tempPidl = (IntPtr) _pidl; // use the absolute one that we constructed just now
				}

				const Native.SHGFI flags = Native.SHGFI.SHGFI_PIDL // indicates that we're specifying the item by PIDL
				                           | Native.SHGFI.SHGFI_DISPLAYNAME // indicates that we want the item's display name
				                           | Native.SHGFI.SHGFI_SYSICONINDEX // indicates that we want the item's icon's index in the system image list
				                           | Native.SHGFI.SHGFI_ATTRIBUTES // indicates that we want the item's attributes
				                           | Native.SHGFI.SHGFI_TYPENAME; // indicates that we want the item's type name
				Native.SHFILEINFO shInfo = new Native.SHFILEINFO();
				Native.Shell32.SHGetFileInfo((IntPtr) _pidl, 0, out shInfo, (uint) Marshal.SizeOf(shInfo), flags);

				// read item attributes
				Native.SFGAO attributeFlags = (Native.SFGAO) shInfo.dwAttributes;

				// create the item's IShellFolder interface
				if ((attributeFlags & Native.SFGAO.SFGAO_FOLDER) != 0)
				{
					Guid iidIShellFolder = new Guid(IID.IID_IShellFolder);
					if (_pidl == _rootItem._pidl)
					{
						// if the requested PIDL is the root namespace (the desktop) we can't use the the BindToObject method, so get it directly
						hResult = Native.Shell32.SHGetDesktopFolder(ref _shellFolder);
					}
					else
					{
						if (relativePidl)
							hResult = (int) parentShellItem._shellFolder.BindToObject(tempPidl, IntPtr.Zero, ref iidIShellFolder, out _shellFolder);
						else
							hResult = (int) _rootItem._shellFolder.BindToObject(tempPidl, IntPtr.Zero, ref iidIShellFolder, out _shellFolder);
					}

					if (hResult != 0)
					{
						// some objects are marked as folders, but really aren't and thus cannot be bound to an IShellFolder
						// log these events for future study, but it's not exactly something to be concerned about in isolated cases.
						// Marshal.ThrowExceptionForHR(hResult);
						if ((attributeFlags & Native.SFGAO.SFGAO_HASSUBFOLDER) == 0)
							attributeFlags = attributeFlags & ~Native.SFGAO.SFGAO_FOLDER;
					}
				}

				_displayName = shInfo.szDisplayName;
				_typeName = shInfo.szTypeName;
				_iconIndex = shInfo.iIcon;
				_isFolder = (attributeFlags & Native.SFGAO.SFGAO_FOLDER) != 0;
				_isVirtual = (attributeFlags & Native.SFGAO.SFGAO_FILESYSTEM) == 0;
				_hasSubFolders = (attributeFlags & Native.SFGAO.SFGAO_HASSUBFOLDER) != 0;
			}
			catch (UnauthorizedAccessException ex)
			{
				// if an exception happens during construction, we must release the PIDL now (remember, it's a pointer!)
				var path = string.Empty;
				if (_pidl != null)
				{
					path = _pidl.Path;
					_pidl.Dispose();
				}
				throw new PathAccessException(path, ex);
			}
			catch (Exception ex)
			{
				// if an exception happens during construction, we must release the PIDL now (remember, it's a pointer!)
				if (_pidl != null)
					_pidl.Dispose();
				throw new Exception("Creation of the specified shell item failed.", ex);
			}

#if DEBUG
			_instanceCount++;
#endif
		}

		~ShellItem()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
#if DEBUG
				--_instanceCount;
#endif

				if (disposing)
				{
					if (_pidl != null)
					{
						_pidl.Dispose();
						_pidl = null;
					}

					if (_rootItem != null)
					{
						// do not dispose this - we don't own it!
						_rootItem = null;
					}
				}

				if (_shellFolder != null)
				{
					// release the IShellFolder interface of this shell item
					Marshal.ReleaseComObject(_shellFolder);
					_shellFolder = null;
				}

				_disposed = true;
			}
		}

		public ShellItem Clone()
		{
			return new ShellItem(_pidl, _rootItem, false);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public IEnumerable<ShellItem> EnumerateChildren()
		{
			return this.EnumerateChildren(ChildType.Files | ChildType.Folders, true);
		}

		public IEnumerable<ShellItem> EnumerateChildren(bool includeHiddenItems)
		{
			return this.EnumerateChildren(ChildType.Files | ChildType.Folders, includeHiddenItems);
		}

		public IEnumerable<ShellItem> EnumerateChildren(ChildType types)
		{
			return this.EnumerateChildren(types, true);
		}

		public IEnumerable<ShellItem> EnumerateChildren(ChildType types, bool includeHiddenItems)
		{
			Native.SHCONTF flags = 0;
			flags |= ((types & ChildType.Files) == ChildType.Files) ? Native.SHCONTF.SHCONTF_NONFOLDERS : 0;
			flags |= ((types & ChildType.Folders) == ChildType.Folders) ? Native.SHCONTF.SHCONTF_FOLDERS : 0;
			flags |= (includeHiddenItems) ? Native.SHCONTF.SHCONTF_INCLUDEHIDDEN : 0;

			List<ShellItem> children = new List<ShellItem>();
			foreach (Pidl pidl in EnumerateChildPidls(flags))
			{
				children.Add(new ShellItem(pidl, this));
				pidl.Dispose();
			}
			return children;
		}

		public IEnumerable<Pidl> EnumerateChildPidls()
		{
			return this.EnumerateChildPidls(ChildType.Files | ChildType.Folders, true);
		}

		public IEnumerable<Pidl> EnumerateChildPidls(bool includeHiddenItems)
		{
			return this.EnumerateChildPidls(ChildType.Files | ChildType.Folders, includeHiddenItems);
		}

		public IEnumerable<Pidl> EnumerateChildPidls(ChildType types)
		{
			return this.EnumerateChildPidls(types, true);
		}

		public IEnumerable<Pidl> EnumerateChildPidls(ChildType types, bool includeHiddenItems)
		{
			Native.SHCONTF flags = 0;
			flags |= ((types & ChildType.Files) == ChildType.Files) ? Native.SHCONTF.SHCONTF_NONFOLDERS : 0;
			flags |= ((types & ChildType.Folders) == ChildType.Folders) ? Native.SHCONTF.SHCONTF_FOLDERS : 0;
			flags |= (includeHiddenItems) ? Native.SHCONTF.SHCONTF_INCLUDEHIDDEN : 0;
			return this.EnumerateChildPidls(flags);
		}

		private IEnumerable<Pidl> EnumerateChildPidls(Native.SHCONTF flags)
		{
			if (!_isFolder)
				throw new InvalidOperationException("Children can only be enumerated on a folder-type item.");
			if (_shellFolder == null)
				return new Pidl[0];

			// Get the IEnumIDList interface pointer.
			Native.IEnumIDList pEnum = null;

			try
			{
				uint hRes = _shellFolder.EnumObjects(IntPtr.Zero, flags, out pEnum);
				if (hRes != 0)
					Marshal.ThrowExceptionForHR((int) hRes);
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new PathAccessException(_pidl.Path, ex);
			}
			catch (Exception ex)
			{
				throw new Exception("IShellFolder::EnumObjects failed to enumerate child objects.", ex);
			}

			try
			{
				return Pidl.ConvertPidlEnumeration(pEnum);
			}
			finally
			{
				// Free the interface pointer.
				Marshal.ReleaseComObject(pEnum);
			}
		}

		public string DisplayName
		{
			get { return _displayName; }
		}

		public string TypeName
		{
			get { return _typeName; }
		}

		public Pidl Pidl
		{
			get { return _pidl; }
		}

		public int IconIndex
		{
			get { return _iconIndex; }
		}

		public bool IsFolder
		{
			get { return _isFolder; }
		}

		public bool IsVirtual
		{
			get { return _isVirtual; }
		}

		public bool HasSubFolders
		{
			get { return _hasSubFolders; }
		}

		public string Path
		{
			get { return _pidl.Path; }
		}

		public ShellItem Root
		{
			get { return _rootItem; }
		}

		[Flags]
		public enum ChildType
		{
			Files = 1,
			Folders = 2
		}

		#region Link Resolution

		/// <summary>
		/// Attempts to resolve the specified file system link to the path of the link target.
		/// </summary>
		/// <remarks>
		/// Target resolution will fail if it is unable to automatically find the target within 3000 milliseconds.
		/// </remarks>
		/// <param name="linkPath">The full path to the file system link.</param>
		/// <param name="resolvedPath">The resolved path to the link target. If the link target does not exist, simply returns <paramref name="linkPath"/>.</param>
		/// <returns>True if link target resolution completed successfully or target was not found; False if an error was encountered while attempting to resolve the link.</returns>
		public static bool TryResolveLink(string linkPath, out string resolvedPath)
		{
			return TryResolveLink(linkPath, 0, out resolvedPath);
		}

		/// <summary>
		/// Attempts to resolve the specified file system link to the path of the link target.
		/// </summary>
		/// <remarks>
		/// Target resolution will fail if it is unable to automatically find the target within the specified timeout.
		/// </remarks>
		/// <param name="linkPath">The full path to the file system link.</param>
		/// <param name="timeout">Timeout for target resolution in milliseconds. If 0, the system default of 3000 milliseconds is used.</param>
		/// <param name="resolvedPath">The resolved path to the link target. If the link target does not exist, simply returns <paramref name="linkPath"/>.</param>
		/// <returns>True if link target resolution completed successfully or target was not found; False if an error was encountered while attempting to resolve the link.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is negative or exceeds 65535.</exception>
		public static bool TryResolveLink(string linkPath, int timeout, out string resolvedPath)
		{
			if (timeout > ushort.MaxValue || timeout < ushort.MinValue)
				throw new ArgumentOutOfRangeException("timeout", "Timeout must be a value between 0 and 65535.");
			try
			{
				resolvedPath = ResolveLink(IntPtr.Zero, linkPath, (ushort) timeout);
				return true;
			}
			catch (Exception)
			{
				resolvedPath = linkPath;
				return false;
			}
		}

		/// <summary>
		/// Attempts to resolve the specified file system link to the path of the link target.
		/// </summary>
		/// <remarks>
		/// The Window handle is used to request additional input from the user if the link target does not exist (such as the familiar
		/// animated flashlight search dialog in Windows). If no Window handle is provided, the target resolution will fail if it is
		/// unable to automatically find the target within 3 seconds.
		/// </remarks>
		/// <param name="hWnd">Window handle for any necessary GUI during link resolution. Set to <see cref="IntPtr.Zero"/> to disable all UI interactions.</param>
		/// <param name="linkPath">The full path to the file system link.</param>
		/// <param name="resolvedPath">The resolved path to the link target. If the link target does not exist, simply returns <paramref name="linkPath"/>.</param>
		/// <returns>True if link target resolution completed successfully or target was not found; False if an error was encountered while attempting to resolve the link.</returns>
		public static bool TryResolveLink(IntPtr hWnd, string linkPath, out string resolvedPath)
		{
			try
			{
				resolvedPath = ResolveLink(hWnd, linkPath, 0);
				return true;
			}
			catch (Exception)
			{
				resolvedPath = linkPath;
				return false;
			}
		}

		/// <summary>
		/// Attempts to resolve the specified file system link to the path of the link target.
		/// </summary>
		/// <remarks>
		/// Target resolution will fail if it is unable to automatically find the target within 3000 milliseconds.
		/// </remarks>
		/// <param name="linkPath">The full path to the file system link.</param>
		/// <returns>The resolved path to the link target. If the link target does not exist, simply returns <paramref name="linkPath"/>.</returns>
		/// <exception cref="PathNotFoundException">Thrown if an error was encountered while attempting to resolve the link. Not thrown if the link target does not exist.</exception>
		public static string ResolveLink(string linkPath)
		{
			return ResolveLink(linkPath, 0);
		}

		/// <summary>
		/// Attempts to resolve the specified file system link to the path of the link target.
		/// </summary>
		/// <remarks>
		/// Target resolution will fail if it is unable to automatically find the target within the specified timeout.
		/// </remarks>
		/// <param name="linkPath">The full path to the file system link.</param>
		/// <param name="timeout">Timeout for target resolution in milliseconds. If 0, the system default of 3000 milliseconds is used.</param>
		/// <returns>The resolved path to the link target. If the link target does not exist, simply returns <paramref name="linkPath"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is negative or exceeds 65535.</exception>
		/// <exception cref="PathNotFoundException">Thrown if an error was encountered while attempting to resolve the link. Not thrown if the link target does not exist.</exception>
		public static string ResolveLink(string linkPath, int timeout)
		{
			if (timeout > ushort.MaxValue || timeout < ushort.MinValue)
				throw new ArgumentOutOfRangeException("timeout", "Timeout must be a value between 0 and 65535.");
			return ResolveLink(IntPtr.Zero, linkPath, (ushort) timeout);
		}

		/// <summary>
		/// Attempts to resolve the specified file system link to the path of the link target.
		/// </summary>
		/// <remarks>
		/// The Window handle is used to request additional input from the user if the link target does not exist (such as the familiar
		/// animated flashlight search dialog in Windows). If no Window handle is provided, the target resolution will fail if it is
		/// unable to automatically find the target within 3 seconds.
		/// </remarks>
		/// <param name="hWnd">Window handle for any necessary GUI during link resolution. Set to <see cref="IntPtr.Zero"/> to disable all UI interactions.</param>
		/// <param name="linkPath">The full path to the file system link.</param>
		/// <returns>The resolved path to the link target. If the link target does not exist, simply returns <paramref name="linkPath"/>.</returns>
		/// <exception cref="PathNotFoundException">Thrown if an error was encountered while attempting to resolve the link. Not thrown if the link target does not exist.</exception>
		public static string ResolveLink(IntPtr hWnd, string linkPath)
		{
			return ResolveLink(hWnd, linkPath, 0);
		}

		private static string ResolveLink(IntPtr hWnd, string linkPath, ushort timeout)
		{
			if (!File.Exists(linkPath))
				throw new PathNotFoundException(linkPath, "Specified path does not exist.");

			try
			{
				var instance = Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid(CLSID.CLSID_ShellLink)));
				try
				{
					var punk = Marshal.GetIUnknownForObject(instance);
					var persistFile = (IPersistFile) Marshal.GetTypedObjectForIUnknown(punk, typeof (IPersistFile));
					try
					{
						persistFile.Load(linkPath, (int) STGM.READ);

						var shellLink = (IShellLink) Marshal.GetTypedObjectForIUnknown(punk, typeof (IShellLink));
						try
						{
							shellLink.Resolve(hWnd, hWnd != IntPtr.Zero ? SLR_FLAGS.SLR_UPDATE : (SLR_FLAGS) ((timeout << 16) | (ushort) SLR_FLAGS.SLR_NO_UI));

							WIN32_FIND_DATA findData;
							var path = new StringBuilder(1024);
							shellLink.GetPath(path, path.Capacity, out findData, SLGP_FLAGS.SLGP_UNCPRIORITY);
							return path.ToString();
						}
						finally
						{
							Marshal.ReleaseComObject(shellLink);
						}
					}
					finally
					{
						Marshal.ReleaseComObject(persistFile);
					}
				}
				finally
				{
					Marshal.ReleaseComObject(instance);
				}
			}
			catch (Exception ex)
			{
				throw new PathNotFoundException("Failed to resolve shortcut.", linkPath, ex);
			}
		}

		#endregion
	}

	// ReSharper restore RedundantAssignment
}