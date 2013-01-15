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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Subclass of DataGridView - overrides the mouse behaviour to allow rows to be "dragged".  This class
	/// is used internally by the <see cref="TableView"/> control and is not intended to be used directly
	/// by application code.
	/// </summary>
	internal class DataGridViewWithDragSupport : DataGridView
	{
		private Rectangle _dragBoxFromMouseDown;
		private bool _suppressSelectionChangedEvent = false;
		private int _anchorRowIndex = -1;
		private int _rowIndexFromMouseDown;
		private event EventHandler<ItemDragEventArgs> _itemDrag;

        public DataGridViewWithDragSupport()
        {
            base.DoubleBuffered = true;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	    public new bool DoubleBuffered
	    {
            get { return base.DoubleBuffered; }
            set { base.DoubleBuffered = value; }
	    }

	    public event EventHandler<ItemDragEventArgs> ItemDrag
		{
			add { _itemDrag += value; }
			remove { _itemDrag -= value; }
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			// get the index of the clicked item
			_rowIndexFromMouseDown = HitTest(e.X, e.Y).RowIndex;

			// basic mouse handling has right clicks show context menu without changing selection, so we handle it manually here
			if (e.Button == MouseButtons.Right)
				HandleRightMouseDown(e);

			// call the base class, so that the row gets selected, etc.
			base.OnMouseDown(e);

			if (_rowIndexFromMouseDown > -1)
			{
				// Remember the point where the mouse down occurred. 
				// The DragSize indicates the size that the mouse can move 
				// before a drag event should be started.                
				Size dragSize = SystemInformation.DragSize;

				// Create a rectangle using the DragSize, with the mouse position being
				// at the center of the rectangle.
				_dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width/2), e.Y - (dragSize.Height/2)), dragSize);
			}
			else
			{
				// Reset the rectangle if the mouse is not over an item in the ListBox.
				_dragBoxFromMouseDown = Rectangle.Empty;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (_rowIndexFromMouseDown != -1 &&
			    (e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				if (ShouldBeginDrag(e))
				{
					// Proceed with the drag and drop, passing in the list item.  
					ItemDragEventArgs args = new ItemDragEventArgs(MouseButtons.Left, null);
					EventsHelper.Fire(_itemDrag, this, args);

					// reset the drag box to empty so that the event is not fired repeatedly
					_dragBoxFromMouseDown = Rectangle.Empty;
				}
			}
			else
			{
				// allow the base class to handle it only if the left mouse button was not pressed
				base.OnMouseMove(e);
			}
		}

		protected override bool SetCurrentCellAddressCore(int columnIndex, int rowIndex, bool setAnchorCellAddress, bool validateCurrentCell, bool throughMouseClick)
		{
			// the base control doesn't make the current anchor cell address available, so we have to keep track of it ourselves
			var result = base.SetCurrentCellAddressCore(columnIndex, rowIndex, setAnchorCellAddress, validateCurrentCell, throughMouseClick);
			if (result && setAnchorCellAddress)
				_anchorRowIndex = rowIndex;
			return result;
		}

		protected override void OnSelectionChanged(EventArgs e)
		{
			if (!_suppressSelectionChangedEvent)
				base.OnSelectionChanged(e);
		}

		/// <summary>
		/// Handles right mouse button down events in a similar fashion as the base implementation's left mouse button down.
		/// </summary>
		/// <remarks>
		/// The base implementation's left mouse button down handling includes support for updating the row selection, but
		/// does not do so for the right mouse button. This function handles all that manually, including support for using
		/// the CTRL and SHIFT modifier keys while selecting. One single <see cref="DataGridView.SelectionChanged"/> event
		/// is fired afterwards, making the operation appear atomic to <see cref="DataGridView"/> consumers. Since this occurs
		/// on mouse down, it should allow drag-drop and context menu operations to see the updated selections if applicable.
		/// </remarks>
		/// <param name="e">The <see cref="MouseEventArgs"/> from the <see cref="OnMouseDown"/> event.</param>
		private void HandleRightMouseDown(MouseEventArgs e)
		{
			// identify the target cell that was clicked
			var targetCell = HitTest(e.X, e.Y);

			var selectionChanged = false;
			var anchorChanged = false;

			// suppress transient selection change events
			_suppressSelectionChangedEvent = true;
			try
			{
				if ((ModifierKeys & Keys.Control) == Keys.Control) // ctrl right click
				{
					// toggle select the clicked row
					if (targetCell.RowIndex >= 0)
						Rows[targetCell.RowIndex].Selected = !Rows[targetCell.RowIndex].Selected;

					// set flag so that selection changed event will fire
					selectionChanged = true;

					// set flag so that the anchor row will update
					anchorChanged = true;
				}
				else if ((ModifierKeys & Keys.Shift) == Keys.Shift) // shift right click
				{
					// if the clicked row forms a valid range, replace selection with new range
					if (_anchorRowIndex >= 0 && targetCell.RowIndex >= 0)
					{
						// deselect all previously selected rows
						while (SelectedRows.Count > 0)
							SelectedRows[0].Selected = false;

						// select everything between the anchor row and the clicked row
						var first = Math.Min(targetCell.RowIndex, _anchorRowIndex);
						var last = Math.Max(targetCell.RowIndex, _anchorRowIndex);
						for (int n = first; n <= last; n++)
							Rows[n].Selected = true;

						// set flag so that selection changed event will fire
						selectionChanged = true;

						// N.B. anchor row does not update when shift clicking the end of the range!
					}
				}
				else // plain right click
				{
					// if the clicked row is not already selected, replace selection with new row
					if (!CollectionUtils.Contains<DataGridViewRow>(SelectedRows, r => r.Index == targetCell.RowIndex))
					{
						// deselect all previously selected rows
						while (SelectedRows.Count > 0)
							SelectedRows[0].Selected = false;

						// select the new row
						if (targetCell.RowIndex >= 0)
							Rows[targetCell.RowIndex].Selected = true;

						// set flag so that selection changed event will fire
						selectionChanged = true;

						// set flag so that the anchor row will update
						anchorChanged = true;
					}
				}
			}
			finally
			{
				// stop suppressing transient selection change events
				_suppressSelectionChangedEvent = false;

				// update the core current cell, otherwise we'll break shift+click range select
				if (targetCell.RowIndex >= 0 && targetCell.ColumnIndex >= 0)
					SetCurrentCellAddressCore(targetCell.ColumnIndex, targetCell.RowIndex, anchorChanged, true, true);

				// fire one single selection change now that all transients have settled
				if (selectionChanged)
					OnSelectionChanged(EventArgs.Empty);
			}
		}

		private bool ShouldBeginDrag(MouseEventArgs e)
		{
			// If the mouse moves outside the rectangle, start the drag.
			return ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			       && (_dragBoxFromMouseDown != Rectangle.Empty)
			       && !_dragBoxFromMouseDown.Contains(e.X, e.Y);
		}
	}
}