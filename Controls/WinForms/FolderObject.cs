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

namespace ClearCanvas.Controls.WinForms
{
	public abstract class FolderObject : IEquatable<FolderObject>
	{
		private readonly string _displayName;
		private readonly string _path;
		private readonly string _virtualPath;
		private readonly bool _isFolder;
		private readonly bool _isLink;

		protected FolderObject(string path, string virtualPath, string displayName, bool isFolder, bool isLink)
		{
			_path = path;
			_virtualPath = virtualPath;
			_displayName = displayName;
			_isFolder = isFolder;
			_isLink = isLink;
		}

		public string DisplayName
		{
			get { return _displayName; }
		}

		public string Path
		{
			get { return _path; }
		}

		public string VirtualPath
		{
			get { return _virtualPath; }
		}

		public bool IsFolder
		{
			get { return _isFolder; }
		}

		public bool IsLink
		{
			get { return _isLink; }
		}

		public string ResolveLink()
		{
			return ResolveLink(IntPtr.Zero);
		}

		public string ResolveLink(IntPtr hWnd)
		{
			if (!IsLink)
				throw new InvalidOperationException("Object is not a file system link.");
			return ShellItem.ResolveLink(hWnd, Path);
		}

		public string GetPath()
		{
			return GetPath(false);
		}

		public string GetPath(bool resolveLinks)
		{
			return GetPath(IntPtr.Zero, resolveLinks);
		}

		public string GetPath(IntPtr hWnd, bool resolveLinks)
		{
			if (resolveLinks && IsLink)
			{
				string resolvedPath;
				if (ShellItem.TryResolveLink(hWnd, Path, out resolvedPath))
					return resolvedPath;
			}
			return Path;
		}

		public bool Equals(FolderObject other)
		{
			if (other != null)
				return _path == other._path && _virtualPath == other._virtualPath;
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is FolderObject)
				return this.Equals((FolderObject) obj);
			return false;
		}

		public override int GetHashCode()
		{
			return 0x7B3F7EEF ^ _path.GetHashCode() ^ _virtualPath.GetHashCode();
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(_path))
				return _displayName;
			return _path;
		}
	}
}