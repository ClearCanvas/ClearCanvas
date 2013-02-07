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
using System.Text;
using ClearCanvas.Controls.WinForms.Native;

namespace ClearCanvas.Controls.WinForms
{
	/// <summary>
	/// A managed wrapper type for shell PIDLs (pointers to ITEMIDLISTs).
	/// </summary>
	public sealed class Pidl : IDisposable, ICloneable, IEquatable<Pidl>
	{
		private IntPtr _pidl;
		private bool _disposed = false;

		private string _path;
		private string _displayName;
		private string _virtualPath;
		private bool? _isRoot;
		private bool? _isFolder;
		private bool? _isLink;

		public Pidl(IntPtr pidl) : this(pidl, false) {}

		public Pidl(Environment.SpecialFolder specialFolder) : this(CreateSpecialFolderPidl(specialFolder), true) {}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// Use this override with great care when specifying <paramref name="isPreallocatedPidl"/> = True.
		/// </remarks>
		/// <param name="pidl"></param>
		/// <param name="isPreallocatedPidl"></param>
		private Pidl(IntPtr pidl, bool isPreallocatedPidl)
		{
			if (IntPtr.Zero.Equals(pidl))
				throw new ArgumentOutOfRangeException("pidl", "PIDL cannot be NULL.");

			if (isPreallocatedPidl)
				_pidl = pidl;
			else
				_pidl = ILClone(pidl);

#if DEBUG
			_instanceCount++;
#endif
		}

		public Pidl(Pidl parentPidl, Pidl childPidl) : this(parentPidl.GetPidl(), childPidl.GetPidl()) {}

		public Pidl(IntPtr parentPidl, IntPtr childPidl)
		{
			if (IntPtr.Zero.Equals(parentPidl) || IntPtr.Zero.Equals(childPidl))
				throw new ArgumentOutOfRangeException("childPidl", "PIDL cannot be NULL.");
			_pidl = ILCombine(parentPidl, childPidl);

#if DEBUG
			_instanceCount++;
#endif
		}

		~Pidl()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
#if DEBUG
			--_instanceCount;
#endif
			if (!_disposed)
			{
				if (!IntPtr.Zero.Equals(_pidl))
				{
					Marshal.FreeCoTaskMem(_pidl);
					_pidl = IntPtr.Zero;
				}

				_disposed = true;
			}
		}

		public string Path
		{
			get
			{
				if (_path == null)
				{
					// The maximum component length (i.e. for any given filename or directory name) is 256.
					// The maximum path length (i.e. the concatenation of all components in the path) is 260.
					const int MAX_PATH = 260;

					var strBuffer = new StringBuilder(MAX_PATH);
					Shell32.SHGetPathFromIDList(GetPidl(), strBuffer);
					_path = strBuffer.ToString();
				}
				return _path;
			}
		}

		public string DisplayName
		{
			get
			{
				if (_displayName == null)
					GetShellFileInfo();
				return _displayName;
			}
		}

		public string VirtualPath
		{
			get
			{
				if (_virtualPath == null)
				{
					using (var pidl = Clone())
					{
						var stack = new Stack<string>();
						var displayName = DisplayName;
						var countChars = displayName.Length;

						stack.Push(displayName);
						while (ILRemoveLastID((IntPtr) pidl))
						{
							displayName = pidl.DisplayName;
							countChars += displayName.Length;
							stack.Push(displayName);
						}

						var sb = new StringBuilder(countChars + stack.Count - 1);
						while (stack.Count > 0)
						{
							sb.Append(stack.Pop());
							if (stack.Count > 0)
								sb.Append(System.IO.Path.DirectorySeparatorChar);
						}
						_virtualPath = sb.ToString();
					}
				}
				return _virtualPath;
			}
		}

		public bool IsRoot
		{
			get
			{
				if (!_isRoot.HasValue)
				{
					var pidl = ILClone(GetPidl());
					try
					{
						_isRoot = !ILRemoveLastID(pidl);
					}
					finally
					{
						Marshal.FreeCoTaskMem(pidl);
					}
				}
				return _isRoot.Value;
			}
		}

		public bool IsFolder
		{
			get
			{
				if (!_isFolder.HasValue)
					GetShellFileInfo();
				return _isFolder.Value;
			}
		}

		public bool IsLink
		{
			get
			{
				if (!_isLink.HasValue)
					GetShellFileInfo();
				return _isLink.Value;
			}
		}

		public void Refresh()
		{
			_path = null;
			_displayName = null;
			_virtualPath = null;
			_isRoot = null;
			_isFolder = null;
			_isLink = null;
		}

		public Pidl Clone()
		{
			return new Pidl(this.GetPidl());
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public bool Equals(Pidl otherPidl)
		{
			if (otherPidl == null)
				return false;
			return ILIsEqual(this.GetPidl(), otherPidl.GetPidl());
		}

		public override bool Equals(object obj)
		{
			if (obj is Pidl)
				return this.Equals((Pidl) obj);
			if (obj is IntPtr) // if we're comparing against an IntPtr, don't throw any disposed object exceptions
				return ILIsEqual(_pidl, (IntPtr) obj);
			return false;
		}

		public override int GetHashCode()
		{
			// don't throw any disposed object exceptions
			return _pidl.GetHashCode();
		}

		public override string ToString()
		{
			// don't throw any disposed object exceptions
			return _pidl.ToString();
		}

		public string ToString(string format)
		{
			// don't throw any disposed object exceptions
			return _pidl.ToString(format);
		}

		public bool IsParentOf(Pidl testPidl)
		{
			return ILIsParent(this.GetPidl(), testPidl.GetPidl(), true);
		}

		public bool IsAncestorOf(Pidl testPidl)
		{
			return ILIsParent(this.GetPidl(), testPidl.GetPidl(), false);
		}

		public Pidl GetParent()
		{
			IntPtr pidl = this.GetPidl();
			pidl = ILClone(pidl);
			if (ILRemoveLastID(pidl))
				return new Pidl(pidl, true);
			Marshal.FreeCoTaskMem(pidl);
			return null;
		}

		/// <summary>
		/// Updates the values of <see cref="_displayName"/>, <see cref="_isFolder"/> and <see cref="_isLink"/>.
		/// </summary>
		private void GetShellFileInfo()
		{
			var pidl = GetPidl();
			var shInfo = new SHFILEINFO();
			Shell32.SHGetFileInfo(pidl, 0, out shInfo, (uint) Marshal.SizeOf(shInfo), SHGFI.SHGFI_PIDL | SHGFI.SHGFI_ATTRIBUTES | SHGFI.SHGFI_DISPLAYNAME);

			_displayName = shInfo.szDisplayName;
			_isFolder = (((SFGAO) shInfo.dwAttributes) & SFGAO.SFGAO_FOLDER) != 0;
			_isLink = (((SFGAO) shInfo.dwAttributes) & SFGAO.SFGAO_LINK) != 0;
		}

		/// <summary>
		/// Gets the pointer to current PIDL instance.
		/// </summary>
		/// <exception cref="ObjectDisposedException">If the current PIDL instance has already been disposed.</exception>
		private IntPtr GetPidl()
		{
			if (IntPtr.Zero.Equals(_pidl))
				throw new ObjectDisposedException("PIDL");
			return _pidl;
		}

		public static Pidl Parse(string path)
		{
			Exception exception;
			Pidl result;
			if (!TryParse(path, out result, out exception))
				throw new PathNotFoundException(path, exception);
			return result;
		}

		public static bool TryParse(string path, out Pidl result)
		{
			Exception exception;
			return TryParse(path, out result, out exception);
		}

		private static bool TryParse(string path, out Pidl result, out Exception exception)
		{
			exception = null;
			result = null;
			if (Native.Shell32.IsSHParseDisplayNameSupported)
			{
				IntPtr pidl = IntPtr.Zero;
				Native.SFGAO flags = 0;
				int hResult = Native.Shell32.SHParseDisplayName(path, IntPtr.Zero, out pidl, 0, out flags);
				if (hResult == 0)
				{
					result = new Pidl(pidl, true);
					return true;
				}
				else
				{
					exception = Marshal.GetExceptionForHR(hResult);
				}
			}
			else
			{
				if (System.IO.Path.IsPathRooted(path))
				{
					if (Directory.Exists(path) || File.Exists(path))
					{
						if (Directory.Exists(path) && !path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
							path += System.IO.Path.DirectorySeparatorChar;
						IntPtr pidl = ILCreateFromPath(path);
						if (!IntPtr.Zero.Equals(pidl))
						{
							result = new Pidl(pidl, true);
							return true;
						}
						else
						{
							exception = new IOException("ILCreateFromPath failed to return a proper PIDL.");
						}
					}
					else
					{
						exception = new FileNotFoundException("The specified path does not exist.", path);
					}
				}
				else
				{
					exception = new ArgumentException("The specified path must be absolute.", "path");
				}
			}
			return false;
		}

		public static Pidl operator +(Pidl parentPidl, Pidl childPidl)
		{
			return new Pidl(parentPidl, childPidl);
		}

		public static bool operator ==(Pidl pidl1, Pidl pidl2)
		{
			if (ReferenceEquals(pidl1, pidl2))
				return true;
			if (!ReferenceEquals(pidl1, null))
				return pidl1.Equals(pidl2);
			return pidl2.Equals(pidl1);
		}

		public static bool operator !=(Pidl pidl1, Pidl pidl2)
		{
			if (ReferenceEquals(pidl1, pidl2))
				return false;
			if (!ReferenceEquals(pidl1, null))
				return !pidl1.Equals(pidl2);
			return !pidl2.Equals(pidl1);
		}

		public static explicit operator IntPtr(Pidl pidl)
		{
			return pidl.GetPidl();
		}

		internal static IEnumerable<Pidl> ConvertPidlEnumeration(Native.IEnumIDList pEnum)
		{
			List<Pidl> children = new List<Pidl>();

			IntPtr pidl = IntPtr.Zero;
			int count = 0;

			// get the first value in the enumeration (the args in the native method signature are more "ref" rather than "out")
			pEnum.Next(1, out pidl, out count);

			// check and loop if value is valid
			while (!IntPtr.Zero.Equals(pidl) && count == 1)
			{
				children.Add(new Pidl(pidl, true));

				// reset counters (required, see note above)
				pidl = IntPtr.Zero;
				count = 0;

				// get the next value in the enumeration
				pEnum.Next(1, out pidl, out count);
			}

			return children;
		}

		private static IntPtr CreateSpecialFolderPidl(Environment.SpecialFolder folder)
		{
			Native.CSIDL csidl;
			switch (folder)
			{
				case Environment.SpecialFolder.MyComputer:
					csidl = Native.CSIDL.CSIDL_DRIVES;
					break;
				case Environment.SpecialFolder.MyDocuments:
					csidl = Native.CSIDL.CSIDL_PERSONAL;
					break;
				case Environment.SpecialFolder.MyMusic:
					csidl = Native.CSIDL.CSIDL_MYMUSIC;
					break;
				case Environment.SpecialFolder.MyPictures:
					csidl = Native.CSIDL.CSIDL_MYPICTURES;
					break;
				case Environment.SpecialFolder.Desktop:
					csidl = Native.CSIDL.CSIDL_DESKTOP;
					break;
				default:
					throw new NotSupportedException(string.Format("The specified SpecialFolder '{0}' is not supported.", folder));
			}

			IntPtr pidl;
			int hResult = Native.Shell32.SHGetFolderLocation(IntPtr.Zero, csidl, IntPtr.Zero, 0, out pidl);
			if (hResult != 0)
				throw new Exception(string.Format("SHGetFolderLocation failed to return the PIDL of the specified SpecialFolder '{0}'.", folder), Marshal.GetExceptionForHR(hResult));
			return pidl;
		}

		#region ITEMIDLIST Shell32 Functions

		// ReSharper disable InconsistentNaming

		/// <summary>
		/// Clones an ITEMIDLIST structure.
		/// </summary>
		/// <remarks>
		/// <para>When you are finished with the cloned ITEMIDLIST structure, release it with <see cref="ILFree"/> to avoid memory leaks.</para>
		/// <para>Requires at least Windows 2000.</para>
		/// </remarks>
		/// <param name="pidl">A pointer to the ITEMIDLIST structure to be cloned.</param>
		/// <returns>Returns a pointer to a copy of the ITEMIDLIST structure pointed to by pidl.</returns>
		[DllImport("shell32.dll")]
		private static extern IntPtr ILClone(IntPtr pidl);

		/// <summary>
		/// Combines two ITEMIDLIST structures.
		/// </summary>
		/// <remarks>
		/// <para>Requires at least Windows 2000.</para>
		/// </remarks>
		/// <param name="pIDLParent">A pointer to the first ITEMIDLIST structure.</param>
		/// <param name="pIDLChild">A pointer to the second ITEMIDLIST structure. This structure is appended to the structure pointed to by pidl1.</param>
		/// <returns>Returns an ITEMIDLIST containing the combined structures. If you set either pidl1 or pidl2 to <see cref="IntPtr.Zero"/>, the returned ITEMIDLIST structure is a clone of the non-<see cref="IntPtr.Zero"/> parameter. Returns <see cref="IntPtr.Zero"/> if pidl1 and pidl2 are both set to <see cref="IntPtr.Zero"/>.</returns>
		[DllImport("shell32.dll")]
		private static extern IntPtr ILCombine(IntPtr pIDLParent, IntPtr pIDLChild);

		/// <summary>
		/// Returns the ITEMIDLIST structure associated with a specified file path.
		/// </summary>
		/// <remarks>
		/// <para>Call ILFree to release the ITEMIDLIST when you are finished with it.</para>
		/// <para>Requires at least Windows 2000.</para>
		/// </remarks>
		/// <param name="pszPath">A NULL-terminated Unicode string that contains the path. This string should be no more than MAX_PATH characters in length, including the terminating NULL character.</param>
		/// <returns>Returns a pointer to an ITEMIDLIST structure that corresponds to the path.</returns>
		[DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = false)]
		private static extern IntPtr ILCreateFromPath([MarshalAs(UnmanagedType.LPWStr)] string pszPath);

		/// <summary>
		/// Tests whether two ITEMIDLIST structures are equal in a binary comparison.
		/// </summary>
		/// <remarks>
		/// <para>ILIsEqual performs a binary comparison of the item data. It is possible for two ITEMIDLIST structures to differ at the binary level while referring to the same item. IShellFolder::CompareIDs should be used to perform a non-binary comparison.</para>
		/// <para>Requires at least Windows 2000.</para>
		/// </remarks>
		/// <param name="pidl1">The first ITEMIDLIST structure.</param>
		/// <param name="pidl2">The second ITEMIDLIST structure.</param>
		/// <returns>Returns TRUE if the two structures are equal, FALSE otherwise.</returns>
		[DllImport("shell32.dll")]
		[return : MarshalAs(UnmanagedType.Bool)]
		private static extern bool ILIsEqual(IntPtr pidl1, IntPtr pidl2);

		/// <summary>
		/// Frees an ITEMIDLIST structure allocated by the Shell.
		/// </summary>
		/// <remarks>
		/// <para>ILFree is often used with ITEMIDLIST structures allocated by one of the other IL functions, but it can be used to free any such structure returned by the Shell—for example, the ITEMIDLIST structure returned by SHBrowseForFolder or used in a call to SHGetFolderLocation.</para>
		/// <para>Requires at least Windows 2000.</para>
		/// </remarks>
		/// <param name="pidl">A pointer to the ITEMIDLIST structure to be freed. This parameter can be NULL.</param>
		[DllImport("shell32.dll")]
		[Obsolete("When using Microsoft Windows 2000 or later, use CoTaskMemFree rather than ILFree. ITEMIDLIST structures are always allocated with the Component Object Model (COM) task allocator on those platforms.")]
		private static extern void ILFree(IntPtr pidl);

		/// <summary>
		/// Removes the last SHITEMID structure from an ITEMIDLIST structure.
		/// </summary>
		/// <remarks>
		/// <para>Requires at least Windows 2000.</para>
		/// </remarks>
		/// <param name="fullPidl">A pointer to the ITEMIDLIST structure to be shortened. When the function returns, this variable points to the shortened structure.</param>
		/// <returns>Returns TRUE if successful, FALSE otherwise.</returns>
		[DllImport("shell32.dll")]
		[return : MarshalAs(UnmanagedType.Bool)]
		private static extern bool ILRemoveLastID(IntPtr fullPidl);

		/// <summary>
		/// Tests whether an ITEMIDLIST structure is the parent of another ITEMIDLIST structure.
		/// </summary>
		/// <param name="pidl_absolute1">A pointer to an ITEMIDLIST (PIDL) structure that specifies the parent. This must be an absolute PIDL.</param>
		/// <param name="pidl_absolute2">A pointer to an ITEMIDLIST (PIDL) structure that specifies the child. This must be an absolute PIDL.</param>
		/// <param name="immediateParentOnly">A Boolean value that is set to TRUE to test for immediate parents of pidl2, or FALSE to test for any parents of pidl2.</param>
		/// <returns>Returns TRUE if pidl1 is a parent of pidl2. If fImmediate is set to TRUE, the function only returns TRUE if pidl1 is the immediate parent of pidl2. Otherwise, the function returns FALSE.</returns>
		[DllImport("shell32.dll")]
		[return : MarshalAs(UnmanagedType.Bool)]
		private static extern bool ILIsParent(IntPtr pidl_absolute1, IntPtr pidl_absolute2, [MarshalAs(UnmanagedType.Bool)] bool immediateParentOnly);

		// ReSharper restore InconsistentNaming

		#endregion

		#region Debugging Code

#if DEBUG
		private static int _instanceCount = 0;

		internal static int InstanceCount
		{
			get { return _instanceCount; }
		}
#endif

		#endregion
	}
}