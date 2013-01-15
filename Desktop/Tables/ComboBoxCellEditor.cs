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

using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Tables
{
	/// <summary>
	/// Defines an extension point for views onto a <see cref="ComboBoxCellEditor"/>.
	/// </summary>
	[ExtensionPoint]
	public class ComboBoxCellEditorViewExtensionPoint : ExtensionPoint<ITableCellEditorView>
	{
	}

	/// <summary>
	/// Implements a <see cref="ITableCellEditor"/> to show a list of choices in a combox box.
	/// </summary>
	[AssociateView(typeof(ComboBoxCellEditorViewExtensionPoint))]
	public class ComboBoxCellEditor : TableCellEditor
	{
		/// <summary>
		/// Delegate to return a list of items to the user-interface.
		/// </summary>
		public delegate IList GetChoicesDelegate();

		/// <summary>
		/// Delegate to formats an item.
		/// </summary>
		public delegate string FormatItemDelegate(object item);

		private readonly GetChoicesDelegate _getChoicesCallback;
		private readonly FormatItemDelegate _formatItemCallback;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="getChoicesCallback"></param>
		/// <param name="formatItemCallback"></param>
		public ComboBoxCellEditor(GetChoicesDelegate getChoicesCallback, FormatItemDelegate formatItemCallback)
		{
			_getChoicesCallback = getChoicesCallback;
			_formatItemCallback = formatItemCallback;
		}

		/// <summary>
		/// Gets the call back to get a list of choices.
		/// </summary>
		public GetChoicesDelegate GetChoices
		{
			get { return _getChoicesCallback; }
		}

		/// <summary>
		/// Gets the call back to format an item.
		/// </summary>
		public FormatItemDelegate FormatItem
		{
			get { return _formatItemCallback; }
		}
	}
}
