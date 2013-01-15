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

using System.Collections.Generic;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Explorer.Local
{
	/// <summary>
	/// Represents a selection of file system paths.
	/// </summary>
	public interface IPathSelection : IEnumerable<string>
	{
		/// <summary>
		/// Gets the selected path at the specified index.
		/// </summary>
		string this[int index] { get; }

		/// <summary>
		/// Gets the number of paths in the selection.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Determines whether or not the specified path is in this selection.
		/// </summary>
		bool Contains(string path);
	}

	/// <summary>
	/// A basic implementation of <see cref="IPathSelection"/>.
	/// </summary>
	public class PathSelection : Selection<string>, IPathSelection
	{
		/// <summary>
		/// Constructs an empty path selection.
		/// </summary>
		public PathSelection() {}

		/// <summary>
		/// Constructs a path selection out of the specified file system paths.
		/// </summary>
		/// <param name="paths">The file system paths in the selection.</param>
		public PathSelection(params string[] paths) : base(paths) {}

		/// <summary>
		/// Constructs a path selection out of the specified file system paths.
		/// </summary>
		/// <param name="paths">The file system paths in the selection.</param>
		public PathSelection(IEnumerable<string> paths) : base(paths) {}

		/// <summary>
		/// Gets the selected path at the specified index.
		/// </summary>
		public string this[int index]
		{
			get { return Items[index]; }
		}
	}
}