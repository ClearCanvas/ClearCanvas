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

namespace ClearCanvas.Desktop.Tables
{
	/// <summary>
	/// Defines an interface to a table cell editor.
	/// </summary>
	public interface ITableCellEditor
	{
		/// <summary>
		/// Called by the framework to associate this editor with the specified column.
		/// </summary>
		/// <param name="column"></param>
		void SetColumn(ITableColumn column);

		/// <summary>
		/// Informs the editor that it is going to begin an edit on the specified item.
		/// </summary>
		/// <param name="item"></param>
		void BeginEdit(object item);

		/// <summary>
		/// Gets or sets the value (e.g. content) of the editor.
		/// </summary>
		object Value { get; set; }

		/// <summary>
		/// Occurs when the <see cref="Value"/> property is modified.
		/// </summary>
		event EventHandler ValueChanged;
	}
}
