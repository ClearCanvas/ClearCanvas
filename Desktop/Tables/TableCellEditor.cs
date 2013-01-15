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
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Tables
{
	/// <summary>
	/// Abstract base implementation of <see cref="ITableCellEditor"/>, providing common functionality.
	/// </summary>
	public abstract class TableCellEditor : ITableCellEditor
	{
		private ITableColumn _column;
		private event EventHandler _valueChanged;
		private object _item;

		#region ITableCellEditor members

		/// <summary>
		/// Called by the framework to associate this editor with the specified column.
		/// </summary>
		/// <param name="column"></param>
		public void SetColumn(ITableColumn column)
		{
			_column = column;
		}

		/// <summary>
		/// Informs the editor that it is going to begin an edit on the specified item.
		/// </summary>
		/// <param name="item"></param>
		public void BeginEdit(object item)
		{
			_item = item;
		}

		/// <summary>
		/// Gets or sets the value (e.g. content) of the editor.
		/// </summary>
		public object Value
		{
			get { return _item == null ? null : _column.GetValue(_item); }
			set
			{
				if (_item != null)
				{
					// check for no-op
					if(Equals(_column.GetValue(_item), value))
						return;

					_column.SetValue(_item, value);
					NotifyValueChanged();
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="ITableCellEditor.Value"/> property is modified.
		/// </summary>
		public event EventHandler ValueChanged
		{
			add { _valueChanged += value; }
			remove { _valueChanged -= value; }
		}

		#endregion


		private void NotifyValueChanged()
		{
			EventsHelper.Fire(_valueChanged, this, EventArgs.Empty);
		}
	}
}
