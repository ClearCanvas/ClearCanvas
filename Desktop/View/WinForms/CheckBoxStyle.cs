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

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Enumeration for specifying the style of check boxes displayed on a <see cref="BindingTreeView"/> control.
	/// </summary>
	public enum CheckBoxStyle
	{
		/// <summary>
		/// Indicates that no check boxes should be displayed at all.
		/// </summary>
		None,

		/// <summary>
		/// Indicates that standard true/false check boxes should be displayed.
		/// </summary>
		Standard,

		/// <summary>
		/// Indicates that tri-state (true/false/unknown) check boxes should be displayed.
		/// </summary>
		TriState
	}
}