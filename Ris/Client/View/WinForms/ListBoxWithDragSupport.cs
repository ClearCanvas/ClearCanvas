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
using System.Windows.Forms;
using System.Drawing;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides data for the item dropped events.
    /// </summary>
	public class ListBoxItemDroppedEventArgs : EventArgs
	{
    	private readonly int _draggedIndex;
    	private readonly int _droppedIndex;

		/// <summary>
		/// Constructor.
		/// </summary>
		internal ListBoxItemDroppedEventArgs(int draggedIndex, int droppedIndex)
		{
			_draggedIndex = draggedIndex;
			_droppedIndex = droppedIndex;
		}

		/// <summary>
		/// Gets the index of the item that was dragged.
		/// </summary>
		public int DraggedIndex
		{
			get { return _draggedIndex; }
		}

		/// <summary>
		/// Gets the index of the item being dropped on.
		/// </summary>
    	public int DroppedIndex
    	{
			get { return _droppedIndex; }
    	}
	}

    /// <summary>
	/// Subclass of ListBox - overrides the mouse behaviour to allow items to be "dragged".  This class
	/// is used internally by the <see cref="ListBoxView"/> control and is not intended to be used directly
    /// by application code.
    /// </summary>
    public class ListBoxWithDragSupport : ListBox
	{
		private int _itemIndexFromMouseDown;
		private Rectangle _dragBoxFromMouseDown;

		private event EventHandler<ListBoxItemDroppedEventArgs> _itemDropped;

    	/// <summary>
    	/// Occurs when an item in the collection is dragged and dropped.
    	/// </summary>
		public event EventHandler<ListBoxItemDroppedEventArgs> ItemDropped
		{
			add { _itemDropped += value; }
			remove { _itemDropped -= value; }
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			// We need to store the _itemIndexFromMouseDown here because there is a known problem in .Net
			// calling DoDragDrop in MouseDown will kill the SelectedIndexChanged event
			_itemIndexFromMouseDown = this.IndexFromPoint(e.Location);

			// call the base class, so that the item gets selected, etc.
			base.OnMouseDown(e);

			if (_itemIndexFromMouseDown > -1)
			{
				// Remember the point where the mouse down occurred. 
				// The DragSize indicates the size that the mouse can move 
				// before a drag event should be started.                
				Size dragSize = SystemInformation.DragSize;

				// Create a rectangle using the DragSize, with the mouse position being
				// at the center of the rectangle.
				_dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
			}
			else
			{
				// Reset the rectangle if the mouse is not over an item in the ListBox.
				_dragBoxFromMouseDown = Rectangle.Empty;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (_itemIndexFromMouseDown != -1 && (e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				if (ShouldBeginDrag(e))
				{
					// Proceed with the drag and drop.  
					this.DoDragDrop(_itemIndexFromMouseDown, DragDropEffects.All);

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

		protected override void OnDragOver(DragEventArgs drgevent)
		{
			if (drgevent.Data.GetDataPresent(typeof(int)))
				drgevent.Effect = DragDropEffects.Move;

			base.OnDragOver(drgevent);
		}

		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			if (drgevent.Data.GetDataPresent(typeof(int)))
			{
				int index = (int)drgevent.Data.GetData(typeof(int));
				int newIndex = this.IndexFromPoint(this.PointToClient(new Point(drgevent.X, drgevent.Y)));

				// Notify subscriber that an item is dropped.
				EventsHelper.Fire(_itemDropped, this, new ListBoxItemDroppedEventArgs(index, newIndex));

				// Set selection on the new item.
				this.SetSelected(newIndex, true);
			}

			base.OnDragDrop(drgevent);
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
