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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class GalleryComponentControl : UserControl
	{
		private readonly GalleryComponent _component;
		private IBindingList _gallery;
		private bool _selectionEventsEnabled = true;

		public GalleryComponentControl()
		{
			InitializeComponent();

			_listView.LargeImageList = new ImageList();
			_listView.LargeImageList.ImageSize = new Size(100, 100);
			_listView.View = System.Windows.Forms.View.LargeIcon;
			_listView.BackColor = Color.Black;
			_listView.ForeColor = Color.WhiteSmoke;
			_listView.ListViewItemSorter = new ListViewIndexComparer();
			_listView.ItemSelectionChanged += OnSelectionChanged;
			_listView.ItemDrag += OnItemDrag;
			_listView.DragEnter += OnItemDragEnter;
			_listView.DragOver += OnItemDragOver;
			_listView.DragLeave += OnItemDragLeave;
			_listView.DragDrop += OnItemDragDrop;

			_toolStrip.Visible = false;
		}

		public GalleryComponentControl(GalleryComponent component)
			: this()
		{
			_component = component;

			_listView.AllowDrop = component.AllowsDropAtIndex || component.AllowsDropOnItem;
			_listView.MultiSelect = _component.MultiSelect;
			_listView.View = _component.ShowDescription ? System.Windows.Forms.View.Tile : System.Windows.Forms.View.LargeIcon;
			_listView.HideSelection = _component.HideSelection;
			_listView.LargeImageList.ImageSize = _component.ImageSize;
			_listView.LabelEdit = _component.AllowRenaming;

			if (_component.MaxDescriptionLines >= 0)
			{
				// add and remove columns as necessary to allow the correct number of description lines to show (first column is always the label/name)
				while (_listView.Columns.Count - 1 > _component.MaxDescriptionLines)
				{
					_listView.Columns.RemoveAt(1);
				}
				while (_listView.Columns.Count - 1 < _component.MaxDescriptionLines)
				{
					_listView.Columns.Add("");
				}
			}

			InitializeToolStrip();
			InitializeContextMenu();

			this.DataSource = _component.DataSource;
			_component.DataSourceChanged += OnDataSourceChanged;
			_component.SelectionChanged += OnDataSourceSelectionChanged;
		}

		private void InitializeContextMenu()
		{
			ToolStripBuilder.Clear(_contextMenu.Items);
			if (_component != null && _component.MenuModel != null)
			{
				ToolStripBuilder.BuildMenu(_contextMenu.Items, _component.MenuModel.ChildNodes);
			}
		}

		private void InitializeToolStrip()
		{
			ToolStripBuilder.Clear(_toolStrip.Items);

			if (_component != null && _component.ToolbarModel != null)
			{
				ToolStripBuilder.BuildToolbar(_toolStrip.Items, _component.ToolbarModel.ChildNodes);
				if (_toolStrip.Items.Count > 0)
					_toolStrip.Visible = true;
			}
		}

		public IBindingList DataSource
		{
			get { return _gallery; }
			set
			{
				if (_gallery != value)
				{
					if (_gallery != null)
					{
						_gallery.ListChanged -= OnListChanged;

						_listView.Items.Clear();
					}

					_gallery = value;

					if (_gallery != null)
					{
						_gallery.ListChanged += OnListChanged;

						foreach (object item in _gallery)
							AddItem(item);
					}
				}
			}
		}

		private void OnDataSourceChanged(object sender, EventArgs e)
		{
			this.DataSource = _component.DataSource;
		}

		#region Handling Changes in the Data Source

		private delegate void OnListChangedDelegate(object sender, ListChangedEventArgs e);

		private void OnListChanged(object sender, ListChangedEventArgs e)
		{
			if(_listView.InvokeRequired)
			{
				_listView.Invoke(new OnListChangedDelegate(OnListChanged), sender, e);
			}
			else
			{
				_listView.SuspendLayout();
				switch (e.ListChangedType)
				{
					case ListChangedType.ItemAdded:
						InsertItem(e.NewIndex, _gallery[e.NewIndex]);
						break;
					case ListChangedType.ItemDeleted:
						RemoveItem(e.NewIndex);
						break;
					case ListChangedType.ItemChanged:
						UpdateItem(e.NewIndex);
						break;
					case ListChangedType.ItemMoved:
						MoveItem(e.NewIndex, e.OldIndex);
						break;
					case ListChangedType.Reset:
						ResetList();
						break;
				}
				_listView.ResumeLayout();
			}
		}

		private void UpdateItem(int index)
		{
			ListViewItem lvi = _listView.Items[index];
			IGalleryItem item = (IGalleryItem) _gallery[index];

			lvi.SubItems.Clear();
			AddSubItems(lvi, item);

			lvi.Text = item.Name;

			int keyIndex = _listView.LargeImageList.Images.IndexOfKey(lvi.ImageKey);
            var existing = _listView.LargeImageList.Images[keyIndex];
            var @new = (Image)item.Image;

            if (existing != @new)
            {
                if (@new != null)
                    _listView.LargeImageList.Images[keyIndex] = @new;
                else
                    _listView.LargeImageList.Images.RemoveAt(keyIndex);

                _listView.RedrawItems(index, index, true);
            }
		}

		private void AddItem(object item)
		{
			InsertItem(_listView.Items.Count, item);
		}

		private void InsertItem(int index, object item)
		{
			IGalleryItem galleryItem = CastToGalleryItem(item);

			string imageKey = Guid.NewGuid().ToString();
			_listView.LargeImageList.Images.Add(imageKey, (Image)galleryItem.Image);
			ListViewItem lvi = new ListViewItem(galleryItem.Name, imageKey);

			AddSubItems(lvi, galleryItem);

			lvi.ToolTipText = galleryItem.Description;
			lvi.Tag = galleryItem;
			_listView.Items.Insert(index, lvi);
		}

		private void AddSubItems(ListViewItem lvi, IGalleryItem galleryItem)
		{
			string[] galleryDescriptionLines = galleryItem.Description.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			for (var i = 0; i < galleryDescriptionLines.Length; i++)
			{
				if (galleryDescriptionLines.Length > _component.MaxDescriptionLines &&
					i == _component.MaxDescriptionLines - 1)
				{
					// There are more descriptions then number of lines allowed
					// and this is pass the last line, add ellipsis instead
					lvi.SubItems.Add("...", Color.Gray, Color.Transparent, _listView.Font);
				}
				else
				{
					// Add regular description line
					lvi.SubItems.Add(galleryDescriptionLines[i], Color.Gray, Color.Transparent, _listView.Font);
				}
			}	
		}

		private void RemoveItem(int index)
		{
			ListViewItem lvi = _listView.Items[index];
			_listView.LargeImageList.Images.RemoveByKey(lvi.ImageKey);
			_listView.Items.RemoveAt(index);

			if (_listView.Items.Count > 0)
			{
				int i = Math.Min(index, _listView.Items.Count - 1);
				_listView.Items[i].Selected = true;
			}
		}

		private void MoveItem(int index, int oldIndex)
		{
			ListViewItem lvi = _listView.Items[oldIndex];
			_listView.Items.RemoveAt(oldIndex);
			_listView.Items.Insert(index, lvi);
			_listView.Items[index].Selected = true;
		}

		private void ResetList()
		{
			_listView.Items.Clear();
			_listView.LargeImageList.Images.Clear();

			foreach (object item in _gallery)
				AddItem(item);
		}

		private static IGalleryItem CastToGalleryItem(object item)
		{
			IGalleryItem galleryItem = item as IGalleryItem;

			if (galleryItem == null)
				throw new InvalidCastException("DataSource must be an IBindingList of IGalleryItem objects.");

			return galleryItem;
		}

		#endregion

		#region Selection and Activation Business

		private void OnDataSourceSelectionChanged(object sender, EventArgs e)
		{
			//_selectionEventsEnabled = false;
			//foreach (ListViewItem lvi in _listView.Items)
			//{
			//    if (_component.Selection.Contains(lvi.Tag))
			//    {
			//        lvi.Selected = true;
			//    }
			//    else
			//    {
			//        lvi.Selected = false;
			//    }
			//}
			//_selectionEventsEnabled = true;
		}

		private void OnSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (!_selectionEventsEnabled)
				return;

			List<IGalleryItem> selectedItems = new List<IGalleryItem>();

			// the focused item should always be the first item in the selection
			if (_listView.FocusedItem != null)
				selectedItems.Add((IGalleryItem)_listView.FocusedItem.Tag);

			//when an item is removed from the list view, the 'selection changed' event fires
			//before the item is removed, so the indices are out of sync and we can't rely on them
			foreach (int index in _listView.SelectedIndices)
			{
				ListViewItem lvi = _listView.Items[index];
				if(lvi != _listView.FocusedItem)
				{
					object item = lvi.Tag;
					selectedItems.Add((IGalleryItem) item);
				}
			}

			_component.Select(selectedItems);
		}

		private void OnItemActivate(object sender, EventArgs e) {
			if(_listView.FocusedItem!=null)
			{
				IGalleryItem item = _listView.FocusedItem.Tag as IGalleryItem;
				_component.Activate(item);
			}
		}

		#endregion

		#region Display Issues

		private void OnAfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			if(e.Label != null)
			{
				try
				{
					((IGalleryItem) _gallery[e.Item]).Name = e.Label;
					return;
				}
				catch (Exception) {
					// if editing the name on the item fails, abort the label change
				}
			}

			e.CancelEdit = true;
		}

		private void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
			if (e.KeyCode == Keys.F2) {
				e.IsInputKey = true;
			}
		}

		private void OnKeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.F2 && _listView.LabelEdit) {
				if (_listView.FocusedItem != null) {
					_listView.FocusedItem.BeginEdit();
					e.Handled = true;
					e.SuppressKeyPress = true;
				}
			}
		}

		// Sorts ListViewItem objects by index.
		private class ListViewIndexComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				return ((ListViewItem) x).Index - ((ListViewItem) y).Index;
			}
		}

		#endregion

		#region Drag and Drop Code

		private void OnItemDrag(object sender, ItemDragEventArgs e)
		{
			List<IGalleryItem> list = new List<IGalleryItem>();
			List<object> objectList = new List<object>();
			ListViewItem specificLvi = (ListViewItem)e.Item;
			IGalleryItem specificDraggedItem = (IGalleryItem)specificLvi.Tag;

			// save selection and focus
			IList savedSelection = new List<IGalleryItem>();
			object savedFocus = _listView.FocusedItem.Tag;

			if(_listView.SelectedItems.Count > 0)
			{
				foreach (ListViewItem lvi in _listView.SelectedItems)
				{
					IGalleryItem item = (IGalleryItem)lvi.Tag;
					list.Add(item);
					objectList.Add(item.Item);
					savedSelection.Add(item);
				}
			}
			else {
				list.Add(specificDraggedItem);
				objectList.Add(specificDraggedItem.Item);
			}

			_selectionEventsEnabled = false;

			IList<IGalleryItem> draggedItems = list.AsReadOnly();
			DragDropOption allowedActions = _component.BeginDrag(draggedItems);
			DragDropOption actualAction = DragDropOption.None;
			if (allowedActions != DragDropOption.None)
			{
				DataObject data = new DataObject();
				data.SetData(draggedItems); // for GalleryComponent consumers, provide a list of the items that were dragged
				data.SetData(objectList.ToArray()); // for foreign consumers, provide an array of the objects wrapped by the dragged/selected items
				data.SetData(specificDraggedItem); // for foreign consumers, provide the actual item that was dragged, not the selected ones

				DragDropEffects allowedEffects = ConvertEnum.GetDragDropEffects(allowedActions);
				DragDropEffects actualEffect = _listView.DoDragDrop(data, allowedEffects);
				actualAction = ConvertEnum.GetDragDropAction(actualEffect);
			}
			_component.EndDrag(draggedItems, actualAction);

			// restore selection and focus
			_listView.SelectedIndices.Clear();
			for(int n = 0; n < _listView.Items.Count; n++)
			{
				ListViewItem lvi = _listView.Items[n];
				if (savedSelection.Contains(lvi.Tag))
				{
					_listView.SelectedIndices.Add(n);
					savedSelection.Remove(lvi.Tag);
				}

				if (lvi.Tag == savedFocus)
					lvi.Focused = true;
			}

			_selectionEventsEnabled = true;

			// if any previously-selected items are left in the list, then the selection has changed overall and we need to update the component selection
			if(savedSelection.Count > 0)
				OnSelectionChanged(null, null);
		}

		private void OnItemDragEnter(object sender, DragEventArgs e)
		{
			// we don't need to do anything on enter... the over event will fire shortly and draw stuff for us
		}

		private void OnItemDragLeave(object sender, EventArgs e)
		{
			DrawInsertionMark(-1, false, Point.Empty);
		}

		private void OnItemDragOver(object sender, DragEventArgs e)
		{
			DragDropObject data = new DragDropObject(e.Data);
			DragDropOption action = DragDropOption.None;
			DragDropOption allowedActions = ConvertEnum.GetDragDropAction(e.AllowedEffect);
			ModifierFlags modifiers = ConvertEnum.GetModifierFlags(e);
			Point clientPoint = _listView.PointToClient(new Point(e.X, e.Y));
			IGalleryItem targetItem;
			bool skipNearestItem = false;

			if (_component.AllowsDropOnItem)
			{
				targetItem = GetTargetItemAt(clientPoint, false);
				if (targetItem != null)
				{
					action = _component.CheckDrop(data, targetItem, allowedActions, modifiers);
					skipNearestItem = true;
					if (action != DragDropOption.None)
						DrawInsertionMark(_gallery.IndexOf(targetItem), true, clientPoint);
				}
			}
			if (_component.AllowsDropAtIndex && action == DragDropOption.None)
			{
				int targetIndex = GetNearestTargetIndexAt(clientPoint);
				if (targetIndex >= 0)
				{
					action = _component.CheckDrop(data, targetIndex, allowedActions, modifiers);
					if (action != DragDropOption.None)
						DrawInsertionMark(targetIndex, false, clientPoint);
				}
			}
			if (!skipNearestItem && _component.AllowsDropOnItem && action == DragDropOption.None)
			{
				targetItem = GetTargetItemAt(clientPoint, true);
				if (targetItem != null)
				{
					action = _component.CheckDrop(data, targetItem, allowedActions, modifiers);
					if (action != DragDropOption.None)
						DrawInsertionMark(_gallery.IndexOf(targetItem), true, clientPoint);
				}
			}

			e.Effect = ConvertEnum.GetDragDropEffects(action);
		}

		private void OnItemDragDrop(object sender, DragEventArgs e)
		{
			DragDropObject data = new DragDropObject(e.Data);
			DragDropOption action = DragDropOption.None;
			DragDropOption allowedActions = ConvertEnum.GetDragDropAction(e.AllowedEffect);
			ModifierFlags modifiers = ConvertEnum.GetModifierFlags(e);
			Point clientPoint = _listView.PointToClient(new Point(e.X, e.Y));
			IGalleryItem targetItem;
			bool skipNearestItem = false;

			if (_component.AllowsDropOnItem)
			{
				targetItem = GetTargetItemAt(clientPoint, false);
				if (targetItem != null)
				{
					action = _component.PerformDrop(data, targetItem, allowedActions, modifiers);
					skipNearestItem = true;
					if (action != DragDropOption.None)
						DrawInsertionMark(_gallery.IndexOf(targetItem), true, clientPoint);
				}
			}
			if (_component.AllowsDropAtIndex && action == DragDropOption.None)
			{
				int targetIndex = GetNearestTargetIndexAt(clientPoint);
				if (targetIndex >= 0)
				{
					action = _component.PerformDrop(data, targetIndex, allowedActions, modifiers);
					if (action != DragDropOption.None)
						DrawInsertionMark(targetIndex, false, clientPoint);
				}
			}
			if (!skipNearestItem && _component.AllowsDropOnItem && action == DragDropOption.None)
			{
				targetItem = GetTargetItemAt(clientPoint, true);
				if (targetItem != null)
				{
					action = _component.PerformDrop(data, targetItem, allowedActions, modifiers);
					if (action != DragDropOption.None)
						DrawInsertionMark(_gallery.IndexOf(targetItem), true, clientPoint);
				}
			}

			e.Effect = ConvertEnum.GetDragDropEffects(action);

			DrawInsertionMark(-1, false, Point.Empty);
		}

		private void DrawInsertionMark(int index, bool drawBoxInsteadOfLine, Point cursorHint)
		{
			// because of various nuances about how the index getter methods work and how the insertion mark is drawn,
			// we ignore the index parameter and recompute where the index should go based on the cursor hint
			int nearestIndex = _listView.InsertionMark.NearestIndex(cursorHint);
			ListViewItem lvi = _listView.GetItemAt(cursorHint.X, cursorHint.Y);
			if (lvi != null)
				nearestIndex = _listView.Items.IndexOf(lvi);

			if (drawBoxInsteadOfLine || index < 0)
			{
				// hide line
				_listView.InsertionMark.Index = -1;
			}
			else
			{
				// draw line
				if (nearestIndex >= 0)
				{
					Rectangle itemRect = _listView.GetItemRect(nearestIndex, ItemBoundsPortion.Entire);
					_listView.InsertionMark.AppearsAfterItem = (cursorHint.X > itemRect.Left + itemRect.Width/2);
					_listView.InsertionMark.Index = nearestIndex;
				}
			}

			if (!drawBoxInsteadOfLine || index < 0)
			{
				// hide box
				_listView.InsertionBoxIndex = -1;
			}
			else
			{
				// draw box
				_listView.InsertionBoxIndex = nearestIndex;
			}
		}

		private IGalleryItem GetTargetItemAt(Point clientPoint, bool allowNearestMatch)
		{
			ListViewItem lvi = _listView.GetItemAt(clientPoint.X, clientPoint.Y);
			if (lvi != null)
				return lvi.Tag as IGalleryItem;
			if (allowNearestMatch)
			{
				int nearestIndex = _listView.InsertionMark.NearestIndex(clientPoint);
				if (nearestIndex >= 0 && nearestIndex < _listView.Items.Count)
					return _listView.Items[nearestIndex].Tag as IGalleryItem;
			}
			return null;
		}

		private int GetNearestTargetIndexAt(Point clientPoint)
		{
			int nearestIndex = _listView.InsertionMark.NearestIndex(clientPoint);
			ListViewItem lvi = _listView.GetItemAt(clientPoint.X, clientPoint.Y);
			if (lvi != null)
				nearestIndex = _listView.Items.IndexOf(lvi);
			if (nearestIndex >= 0 && nearestIndex < _listView.Items.Count)
			{
				Rectangle itemRect = _listView.GetItemRect(nearestIndex, ItemBoundsPortion.Entire);
				if (clientPoint.X > itemRect.Left + itemRect.Width/2)
					nearestIndex+=1;
			}
			else if (nearestIndex < 0 && _listView.Items.Count == 0)
			{
				nearestIndex = 0;
			}
			return nearestIndex;
		}

		#endregion
	}
}
