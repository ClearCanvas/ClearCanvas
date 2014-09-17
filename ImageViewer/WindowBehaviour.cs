#region License

// Copyright (c) 2014, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Specifies window launch options for the <see cref="ImageViewerComponent"/>.
	/// </summary>
	public enum WindowBehaviour
	{
		/// <summary>
		/// Same as <see cref="Single"/> currently.
		/// </summary>
		Auto,

		/// <summary>
		/// Specifies that the <see cref="ImageViewerComponent"/> should be launched
		/// in a single (e.g. active) desktop window.
		/// </summary>
		Single,

		/// <summary>
		/// Specifies that the <see cref="ImageViewerComponent"/> should be launched
		/// in a separate desktop window.
		/// </summary>
		Separate
	}
}