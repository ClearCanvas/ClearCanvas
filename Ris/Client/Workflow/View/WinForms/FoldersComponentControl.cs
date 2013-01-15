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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="FoldersComponent"/>
    /// </summary>
    public partial class FoldersComponentControl : CustomUserControl
    {
        private FoldersComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public FoldersComponentControl(FoldersComponent component)
        {
            InitializeComponent();

            _component = component;

            ToolStripBuilder.BuildToolbar(_toolbar.Items, _component.ToolbarModel.ChildNodes, ToolStripItemDisplayStyle.ImageAndText);

            foreach (IFolder folder in _component.Folders)
            {
                TreeNode node = new TreeNode(folder.DisplayName);
                node.Tag = folder;

                _folderTree.Nodes.Add(node);
            }

            _component.SelectedFolderChanged += new EventHandler(_component_SelectedFolderChanged);

            // TODO add .NET databindings to _component
        }

        private void _component_SelectedFolderChanged(object sender, EventArgs e)
        {
            if (_component.SelectedFolder != _folderTree.SelectedNode.Tag)
            {
                SelectTreeNodeForFolder(_component.SelectedFolder);
            }
        }

        private void _folderTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _component.SelectedFolder = (IFolder)_folderTree.SelectedNode.Tag;
        }

        private void SelectTreeNodeForFolder(IFolder folder)
        {
            foreach(TreeNode node in _folderTree.Nodes)
            {
                if (node.Tag == folder)
                {
                    _folderTree.SelectedNode = node;
                    break;
                }
            }
        }
    }
}
