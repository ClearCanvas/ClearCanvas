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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Enumeration for drag and drop actions.
	/// </summary>
	[Flags]
	public enum DragDropOption : int
	{
		/// <summary>
		/// Indicates that no drag and drop interaction is allowed, or that no action has been taken.
		/// </summary>
		None = 0,

		/// <summary>
		/// Indicates that a drag and drop move interaction is allowed, or that a move action was taken.
		/// </summary>
		Move = 1,

		/// <summary>
		/// Indicates that a drag and drop copy interaction is allowed, or that a copy action was taken.
		/// </summary>
		Copy = 2
	}
}