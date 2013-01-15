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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// ListBox-view that binds to an instance of an <see cref="IBindingList"/>, which acts as data-source.
	/// Also has built-in drag & drop support, delegating drop decisions to the underlying <see cref="ListBoxWithDragSupport"/>.
	/// </summary>
	public partial class ListBoxView : UserControl
	{
		private ActionModelNode _toolbarModel;
		private ActionModelNode _menuModel;
		private event ListControlConvertEventHandler _format;

		private bool _isLoaded = false;

		public ListBoxView()
		{
			InitializeComponent();
			_listBox.Format += _listBox_Format;
		}

		#region Public Members

		/// <summary>
		/// Gets or sets the data source for the underlying <see cref="ListBoxWithDragSupport"/>.
		/// </summary>
		public IBindingList DataSource
		{
			get { return (IBindingList) _listBox.DataSource; }
			set { _listBox.DataSource = value; }
		}

		/// <summary>
		/// Gets or sets the display member of the ListBox item.
		/// </summary>
		public string DisplayMember
		{
			get { return _listBox.DisplayMember; }
			set { _listBox.DisplayMember = value; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ActionModelNode ToolbarModel
		{
			get { return _toolbarModel; }
			set
			{
				_toolbarModel = value;
				if (_isLoaded) 
					InitializeToolStrip();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ActionModelNode MenuModel
		{
			get { return _menuModel; }
			set
			{
				_menuModel = value;
				if (_isLoaded) 
					InitializeMenu();
			}
		}

		/// <summary>
		/// Gets or sets the current selected index.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedIndex
		{
			get { return _listBox.SelectedIndex; }
			set { _listBox.SelectedIndex = value; }
		}

		/// <summary>
		/// Notifies that the selection has changed
		/// </summary>
		public event EventHandler SelectedIndexChanged
		{
			add { _listBox.SelectedIndexChanged += value; }
			remove { _listBox.SelectedIndexChanged -= value; }
		}

		/// <summary>
		/// Notifies that an item is dragged and dropped on another item.
		/// </summary>
		public event EventHandler<ListBoxItemDroppedEventArgs> ItemDropped
		{
			add { _listBox.ItemDropped += value; }
			remove { _listBox.ItemDropped -= value; }
		}

		/// <summary>
		/// Occurs to allow formatting of the item for display in the user-interface.
		/// </summary>
		public event ListControlConvertEventHandler Format
		{
			add { _format += value; }
			remove { _format -= value; }
		}

		#endregion

		#region Design Time Properties and Events

		[DefaultValue(true)]
		public bool ShowToolbar
		{
			get { return _toolStrip.Visible; }
			set { _toolStrip.Visible = value; }
		}

		#endregion

		private void ListBoxView_Load(object sender, EventArgs e)
		{
			InitializeMenu();
			InitializeToolStrip();
			_isLoaded = true;
		}

		private void _contextMenu_Opening(object sender, CancelEventArgs e)
		{
			Point pt = _listBox.PointToClient(ListBox.MousePosition);
			int itemIndex = _listBox.IndexFromPoint(pt);
			if (itemIndex != -1)
				_listBox.SetSelected(itemIndex, true);
		}

		private void InitializeToolStrip()
		{
			ToolStripBuilder.Clear(_toolStrip.Items);
			if (_toolbarModel != null)
			{
				ToolStripBuilder.BuildToolbar(_toolStrip.Items, _toolbarModel.ChildNodes);
			}
		}

		private void InitializeMenu()
		{
			ToolStripBuilder.Clear(_contextMenu.Items);
			if (_menuModel != null)
			{
				ToolStripBuilder.BuildMenu(_contextMenu.Items, _menuModel.ChildNodes);
			}
		}

		private void _listBox_Format(object sender, ListControlConvertEventArgs e)
		{
			e.Value = FormatItem(e.ListItem);
		}

		private string FormatItem(object item)
		{
			ListControlConvertEventArgs args = new ListControlConvertEventArgs(item, typeof(string), item);
			EventsHelper.Fire(_format, this, args);

			return args.Value.ToString();
		}
	}
}
