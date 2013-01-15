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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="FolderExplorerComponent"/>
    /// </summary>
    public partial class FolderExplorerComponentControl : CustomUserControl
    {
        private readonly FolderExplorerComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public FolderExplorerComponentControl(FolderExplorerComponent component)
        {
            InitializeComponent();
            _component = component;

            _folderTreeView.Tree = _component.FolderTree;
            _folderTreeView.DataBindings.Add("Selection", _component, "SelectedFolderTreeNode", true, DataSourceUpdateMode.OnPropertyChanged);
            _folderTreeView.MenuModel = _component.FoldersContextMenuModel;

            _component.SelectedFolderChanged += _component_SelectedFolderChanged;
        }

        private void _component_SelectedFolderChanged(object sender, EventArgs e)
        {
			if (_folderTreeView.Selection != _component.SelectedFolderTreeNode)
			{
				_folderTreeView.Selection = _component.SelectedFolderTreeNode;

				// Update action model based on the folder selected
				_folderTreeView.MenuModel = _component.FoldersContextMenuModel;
			}
        }
    }
}
