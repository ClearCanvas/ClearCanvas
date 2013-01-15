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
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Manages a single level of a tree view, listening for changes to the underlying model and updating the tree view
	/// as required.  This class is used internally and is not intended to be used directly by application code.
	/// </summary>
	internal class BindingTreeLevelManager : IDisposable
    {
        private readonly ITree _tree;
        private readonly TreeNodeCollection _nodeCollection;
		private readonly TreeView _treeView;
		private readonly BindingTreeView _bindingTreeView;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="nodeCollection"></param>
		/// <param name="bindingTreeView"></param>
        public BindingTreeLevelManager(ITree tree, TreeNodeCollection nodeCollection, BindingTreeView bindingTreeView)
        {
            _tree = tree;
            _tree.Items.ItemsChanged += TreeItemsChangedEventHandler;
            _nodeCollection = nodeCollection;
        	_bindingTreeView = bindingTreeView;
        	_treeView = _bindingTreeView.TreeView;

            BuildLevel();
        }

		/// <summary>
		/// Gets the tree that is being managed.
		/// </summary>
		public ITree Tree
		{
			get { return _tree; }
		}

		public void Dispose()
		{
			_tree.Items.ItemsChanged -= TreeItemsChangedEventHandler;
		}

        /// <summary>
        /// Handles changes to the tree's items collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeItemsChangedEventHandler(object sender, ItemChangedEventArgs e)
        {
            switch (e.ChangeType)
            {
                case ItemChangeType.ItemAdded:
                    AddNode(_tree.Items[e.ItemIndex]);
                    break;
				case ItemChangeType.ItemInserted:
            		InsertNode(e.ItemIndex, _tree.Items[e.ItemIndex]);
            		break;
                case ItemChangeType.ItemChanged:
                    UpdateNode(e.ItemIndex, _tree.Items[e.ItemIndex]);
                    break;
                case ItemChangeType.ItemRemoved:
                    RemoveNode(e.ItemIndex);
                    break;
                case ItemChangeType.Reset:
                    BuildLevel();
                    break;
            }
        }

        /// <summary>
        /// Adds a node for the specified item
        /// </summary>
        /// <param name="item"></param>
        private void AddNode(object item)
        {
            var node = new BindingTreeNode(_tree, item, _bindingTreeView);
            _nodeCollection.Add(node);
            node.UpdateDisplay();
        }

        /// <summary>
        /// Updates the node at the specified index, with the specified item
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        private void UpdateNode(int index, object item)
        {
            BindingTreeNode node = (BindingTreeNode)_nodeCollection[index];
            node.DataBoundItem = item;
            node.UpdateDisplay();   // force update, even if it is the same item, because its properties may have changed
        }

		private void InsertNode(int index, object item)
		{
			BindingTreeNode node = new BindingTreeNode(_tree, item, _bindingTreeView);
			_nodeCollection.Insert(index, node);
		}

        /// <summary>
        /// Removes the node at the specified index
        /// </summary>
        /// <param name="index"></param>
        private void RemoveNode(int index)
        {
			BindingTreeNode node = (BindingTreeNode)_nodeCollection[index];
            _nodeCollection.RemoveAt(index);
			node.Dispose();
        }

        /// <summary>
        /// Builds or rebuilds the entire level
        /// </summary>
        private void BuildLevel()
        {
			// dispose of all existing tree nodes before clearing the collection
        	foreach (TreeNode node in _nodeCollection)
        	{
        		if(node is IDisposable)
					(node as IDisposable).Dispose();
        	}

            _nodeCollection.Clear();

			// create new node for each item
            foreach (object item in _tree.Items)
            {
				BindingTreeNode node = new BindingTreeNode(_tree, item, _bindingTreeView);
                _nodeCollection.Add(node);
                node.UpdateDisplay();
            }
        }

    }
}
