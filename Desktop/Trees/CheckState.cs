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

namespace ClearCanvas.Desktop.Trees
{
	/// <summary>
	/// Enumeration of values representing the check state of nodes in an <see cref="ITree"/>.
	/// </summary>
	public enum CheckState
	{
		/// <summary>
		/// Indicates the unchecked state. <see cref="int">Integer</see> value of -1.
		/// </summary>
		Unchecked = -1,

		/// <summary>
		/// Indicates the unknown/indeterminate/partial check state. <see cref="int">Integer</see> value of 0.
		/// </summary>
		Partial = 0,

		/// <summary>
		/// Indicates the checked state. <see cref="int">Integer</see> value of 1.
		/// </summary>
		Checked = 1
	}
}