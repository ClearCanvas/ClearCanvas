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

using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="FolderExplorerConfigurationComponent"/>.
	/// </summary>
	public partial class FolderExplorerConfigurationComponentControl : ApplicationComponentUserControl
	{
		private readonly FolderExplorerConfigurationComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public FolderExplorerConfigurationComponentControl(FolderExplorerConfigurationComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			_folderSystems.DataBindings.Add("SelectedIndex", _component, "SelectedFolderSystemIndex", true, DataSourceUpdateMode.OnPropertyChanged);
			_folderSystems.Format += _folderSystems_Format;
			_folderSystems.ItemDropped += _folderSystems_ItemDropped;
			_folderSystems.MenuModel = _component.FolderSystemsActionModel;
			_folderSystems.ToolbarModel = _component.FolderSystemsActionModel;
			_folderSystems.DataSource = _component.FolderSystems;

			_folders.DataBindings.Add("Selection", _component, "SelectedFolderNode", true, DataSourceUpdateMode.OnPropertyChanged);
			_folders.MenuModel = _component.FoldersActionModel;
			_folders.ToolbarModel = _component.FoldersActionModel;
			_folders.Tree = _component.FolderTree;
			_folders.DataBindings.Add("Enabled", _component, "FolderEditorEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_component.OnEditFolder += delegate { _folders.EditSelectedNode(); };
		}

		private void _folderSystems_Format(object sender, ListControlConvertEventArgs e)
		{
			e.Value = _component.FormatFolderSystem(e.ListItem);
		}

		private void _folderSystems_ItemDropped(object sender, ListBoxItemDroppedEventArgs e)
		{
			// There is no ItemDragged event, and the dragged object does not fire selection change event either
			// So we must change the selected folder system first, before attempting to move folder system
			_component.SelectedFolderSystemIndex = e.DraggedIndex;
			_component.MoveSelectedFolderSystem(e.DraggedIndex, e.DroppedIndex);
		}

		private void _folders_ItemDrag(object sender, ItemDragEventArgs e)
		{
			// allow dragging of nodes
			var selection = (ISelection)e.Item;

			// send the node
			if (selection.Item != null)
				_folders.DoDragDrop(selection.Item, DragDropEffects.Move);
		}

		private void _folders_ItemDropped(object sender, BindingTreeView.ItemDroppedEventArgs e)
		{
			if (e.Item != null && e.Kind != DragDropKind.None)
				_component.OnItemDropped(e.Item, e.Kind);
		}
	}
}
