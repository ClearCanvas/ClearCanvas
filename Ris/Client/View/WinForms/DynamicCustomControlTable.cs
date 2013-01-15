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
using System.Windows.Forms;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	public abstract class DynamicCustomControlTable<TRow> : TableLayoutPanel
	{
		/// <summary>
		/// Defines a callback that get entity reference of an object.
		/// </summary>
		/// <param name="item"></param>
		/// <returns>The entity reference of an object.</returns>
		public delegate EntityRef GetEntityRefCallback(object item);

		private ILookupHandler _staffLookupHandler;
		private event GetEntityRefCallback _getStaffEntityRefCallback;

		private ILookupHandler _practitionerLookupHandler;
		private event GetEntityRefCallback _getPractitionerEntityRefCallback;

		private event EventHandler _selectionChanged;

		private readonly ItemCollection<TRow> _items;
		private ISelection _selection;
		private bool _hasHeaderRow;

		protected DynamicCustomControlTable()
		{
			_selection = Desktop.Selection.Empty;
			_items = new ItemCollection<TRow>();
			_items.ItemsChanged += OnItemsChanged;

			this.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
			this.AutoScroll = true;
		}

		#region Public Properties and Methods

		/// <summary>
		/// Gets the collection of items in the table.
		/// </summary>
		/// <remarks>
		/// Do not yet supports ItemRemoved, ItemInserted and ItemChanged.
		/// </remarks>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ItemCollection<TRow> Items
		{
			get { return _items; }
		}

		/// <summary>
		/// Gets/sets the current selection
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ISelection Selection
		{
			get { return _selection; }
			set
			{
				_selection = value;
				EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler SelectionChanged
		{
			add { _selectionChanged += value; }
			remove { _selectionChanged -= value; }
		}

		/// <summary>
		/// Gets and sets the lookup handler for the Staff lookup field.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ILookupHandler StaffLookupHandler
		{
			get { return _staffLookupHandler; }
			set { _staffLookupHandler = value; }
		}

		/// <summary>
		/// Fires when the table need to get the value of a Staff object.
		/// </summary>
		/// <remark>
		/// The return object is not for displayed, but as a returned object in the Arguments property.
		/// </remark>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event GetEntityRefCallback GetStaffEntityRefCallback
		{
			add { _getStaffEntityRefCallback += value; }
			remove { _getStaffEntityRefCallback -= value; }
		}

		/// <summary>
		/// Gets and sets the lookup handler for the Practitioner lookup field.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ILookupHandler PractitionerLookupHandler
		{
			get { return _practitionerLookupHandler; }
			set { _practitionerLookupHandler = value; }
		}

		/// <summary>
		/// Fires when the table need to format a Practitioner object for return argument
		/// </summary>
		/// <remark>
		/// The formatted return object is not for displayed, but as a returned object in the Arguments property.
		/// </remark>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event GetEntityRefCallback GetPractitionerEntityRefCallback
		{
			add { _getPractitionerEntityRefCallback += value; }
			remove { _getPractitionerEntityRefCallback -= value; }
		}

		#endregion

		#region Overridables

		/// <summary>
		/// Clear data specific to the inherit class.
		/// </summary>
		protected abstract void ClearCustomData();

		/// <summary>
		/// Gets a list of control based on the rowData.  Each control must have a unique name.
		/// </summary>
		protected abstract List<Control> GetRowControls(TRow rowData);

		/// <summary>
		/// Gets a list of control for the header row.  Each control must have a unique name.
		/// Override this to insert a header row.
		/// </summary>
		protected virtual List<Control> GetHeaderRowControls()
		{
			return new List<Control>();
		}

		protected EntityRef GetStaffEntityRef(object item)
		{
			return _getStaffEntityRefCallback == null ? null : _getStaffEntityRefCallback(item);
		}

		protected EntityRef GetPractitionerEntityRef(object item)
		{
			return _getPractitionerEntityRefCallback == null ? null : _getPractitionerEntityRefCallback(item);
		}

		#endregion

		/// <summary>
		/// Do not yet supports ItemRemoved, ItemInserted and ItemChanged.
		/// </summary>
		private void OnItemsChanged(object sender, Desktop.ItemChangedEventArgs e)
		{
			// Redrawing controls one at a time will take time.  Set it to invisible first
			this.Visible = false;

			switch (e.ChangeType)
			{
				case ItemChangeType.ItemAdded:
				case ItemChangeType.ItemInserted:
					AddRow((TRow)e.Item);
					break;
				case ItemChangeType.ItemChanged:
				case ItemChangeType.ItemRemoved:
					break;
				case ItemChangeType.Reset:
				default:
					ClearRows();
					foreach (var item in _items)
						AddRow(item);

					break;
			}

			// Make this table visible again.
			this.Visible = true;
		}
		
		private void ClearRows()
		{
			// Clear existing table
			this.RowCount = 1;
			this.Controls.Clear();

			ClearCustomData();

			AddHeaderRow();

			this.RowStyles[0].SizeType = SizeType.AutoSize;
		}

		private void AddHeaderRow()
		{
			var controls = GetHeaderRowControls();
			if (controls.Count > 0)
				_hasHeaderRow = true;

			AddControls(0, controls);
		}

		private void AddRow(TRow rowData)
		{
			var controls = GetRowControls(rowData);
			AddControls(this.RowCount - 1, controls);
		}

		private void AddControls(int rowNumber, IList<Control> controls)
		{
			if (controls.Count == 0)
				return;

			if (this.ColumnCount < 1 || controls.Count != this.ColumnCount)
				throw new ArgumentException("Specified number of columns is incorrect."); 

			for (var c = 0; c < controls.Count; c++)
			{
				var control = controls[c];
				control.Click += OnControlClicked;
				this.Controls.Add(control, c, rowNumber);
			}

			this.RowCount++;
		}

		private void OnControlClicked(object sender, System.EventArgs e)
		{
			var controlIndex = this.Controls.GetChildIndex((Control)sender);
			var rowIndex = controlIndex/this.ColumnCount;
			if (_hasHeaderRow)
				rowIndex -= 1;

			this.Selection = rowIndex < 0 ? Desktop.Selection.Empty : new Selection(_items[rowIndex]);
		}
	}
}
