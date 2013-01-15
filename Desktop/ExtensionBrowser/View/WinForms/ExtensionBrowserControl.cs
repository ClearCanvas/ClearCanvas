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

namespace ClearCanvas.Desktop.ExtensionBrowser.View.WinForms
{
    /// <summary>
    /// User-interface for the <see cref="ExtensionBrowserComponent"/>
    /// </summary>
    public partial class ExtensionBrowserControl : UserControl
    {
        private ExtensionBrowserComponent _browser;

        /// <summary>
        /// Default constructor.  Must be initialized with the instance of <see cref="ExtensionBrowserComponent"/>
        /// that it will interact with.
        /// </summary>
        /// <param name="browser"></param>
        public ExtensionBrowserControl(ExtensionBrowserComponent browser)
        {
            InitializeComponent();

            _browser = browser;

            _pluginTreeView.BeforeExpand += new TreeViewCancelEventHandler(TreeView_BeforeExpand);
            _extPointTreeView.BeforeExpand += new TreeViewCancelEventHandler(TreeView_BeforeExpand);

            BuildTreeView(_pluginTreeView, _browser.PluginTree);
            BuildTreeView(_extPointTreeView, _browser.ExtensionPointTree);

        }

        /// <summary>
        /// When the user is about to expand a node, need to build the level beneath it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode expandingNode = e.Node;
            foreach (TreeNode subNode in expandingNode.Nodes)
            {
                // only build the next level if it hasn't been built previously
                if (subNode.Nodes.Count == 0)
                {
                    BuildNextTreeLevel(subNode);
                }
            }
        }

        /// <summary>
        /// Builds the root and first-level of the tree
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="dataRoot"></param>
        private void BuildTreeView(TreeView treeView, IBrowserNode dataRoot)
        {
            treeView.Nodes.Clear();
            if (dataRoot != null)
            {
                TreeNode treeRoot = new TreeNode(dataRoot.DisplayName);
                treeRoot.Tag = dataRoot;
                treeView.Nodes.Add(treeRoot);
                BuildNextTreeLevel(treeRoot);
            }
        }

        /// <summary>
        /// Called to build subsequent levels of the tree as they are expanded
        /// </summary>
        /// <param name="treeNode"></param>
        private void BuildNextTreeLevel(TreeNode treeNode)
        {
            IBrowserNode dataNode = (IBrowserNode)treeNode.Tag;
            foreach (IBrowserNode dataChild in dataNode.ChildNodes)
            {
                TreeNode treeChild = new TreeNode(dataChild.DisplayName);
                treeChild.Tag = dataChild;
                treeChild.ForeColor = dataChild.Enabled ? treeNode.TreeView.ForeColor : Color.DimGray;
                treeNode.Nodes.Add(treeChild);
            }
        }

    }
}
