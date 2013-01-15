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
using ClearCanvas.Common;
using ClearCanvas.Desktop.Trees;
using CheckState=ClearCanvas.Desktop.Trees.CheckState;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Tree node that knows how to build its subtree on demand from an <see cref="ITree"/> model.  This class
    /// is used internally and is not intended to be used directly by application code.
    /// </summary>
    internal class BindingTreeNode : TreeNode, IDisposable
    {
    	/// <summary>
    	/// need to cache our own reference to the treeView, because during construction the base-class TreeView property is null
    	/// </summary>
		private readonly TreeView _treeView;
    	private readonly BindingTreeView _bindingTreeView;
		private readonly ITree _parentTree;
		private BindingTreeLevelManager _subtreeManager;
        private object _item;
        private bool _isSubTreeBuilt;
    	private bool _isUpdating = false;

		public BindingTreeNode(ITree parentTree, object item, BindingTreeView bindingTreeView)
            : base(parentTree.Binding.GetNodeText(item))
        {
            _item = item;
            _parentTree = parentTree;
			_bindingTreeView = bindingTreeView;
			_treeView = _bindingTreeView.TreeView;

			UpdateDisplay(_treeView.ImageList);
        }

        /// <summary>
        /// The item that this node represents
        /// </summary>
        public object DataBoundItem
        {
            get { return _item; }
            set
            {
                if (value != _item)
                {
                    _item = value;
                    UpdateDisplay();
                }
            }
        }

		/// <summary>
		/// Updates the displayable properties of this node, based on the underlying model
		/// </summary>
		public void UpdateDisplay()
        {
			UpdateDisplay(_treeView.ImageList);
        }

        /// <summary>
        /// Returns true if the sub-tree of this node has been built
        /// </summary>
        public bool IsSubTreeBuilt
        {
            get { return _isSubTreeBuilt; }
        }

        /// <summary>
        /// Forces the sub-tree to be built
        /// </summary>
        public void BuildSubTree()
        {
            if (!_isSubTreeBuilt)
            {
                _isSubTreeBuilt = true;
                RebuildSubTree();
            }
        }

        /// <summary>
        /// Asks the item if it can accept the specifid drop
        /// </summary>
        /// <param name="dropData"></param>
        /// <param name="kind"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public DragDropKind CanAcceptDrop(object dropData, DragDropKind kind, DragDropPosition position)
        {
            return _parentTree.Binding.CanAcceptDrop(_item, dropData, kind, position);
        }

		/// <summary>
		/// Asks if the item text can be changed.
		/// </summary>
		/// <returns></returns>
		public bool CanSetNodeText()
		{
			return _parentTree.Binding.CanSetNodeText(_item);
		}

        /// <summary>
        /// Asks the item to accept the specified drop
        /// </summary>
        /// <param name="dropData"></param>
		/// <param name="kind"></param>
		/// <param name="position"></param>
		public DragDropKind AcceptDrop(object dropData, DragDropKind kind, DragDropPosition position)
        {
			return _parentTree.Binding.AcceptDrop(_item, dropData, kind, position);
        }

		private void UpdateDisplay(ImageList treeViewImageList)
		{
			// Remember the previous update state, and return to it afterward.
			var wasStillUpdating = _isUpdating;
			_isUpdating = true;

			if (this.TreeView != null)
				this.TreeView.BeginUpdate();

			// update all displayable attributes from the binding
			this.Text = _parentTree.Binding.GetNodeText(_item);
			this.ToolTipText = _parentTree.Binding.GetTooltipText(_item);

			CheckState checkState = _parentTree.Binding.GetCheckState(_item);
			if (_bindingTreeView.CheckBoxes)
				this.StateImageKey = checkState.ToString();

			if (_parentTree.Binding.GetIsHighlighted(_item))
				this.BackColor = System.Drawing.Color.FromArgb(124, 177, 221);
			else
				this.BackColor = System.Drawing.Color.Empty;

			if (treeViewImageList != null)
			{
				var resolver = _parentTree.Binding.GetResourceResolver(_item);
				var iconSet = _parentTree.Binding.GetIconSet(_item);
				var imageCollection = treeViewImageList.Images;
				if (iconSet == null)
				{
					this.ImageKey = string.Empty;
				}
				else
				{
					try
					{
						var iconKey = iconSet.GetIconKey(_bindingTreeView.IconResourceSize, resolver);
						if (!imageCollection.ContainsKey(iconKey))
						{
							imageCollection.Add(iconKey, iconSet.CreateIcon(_bindingTreeView.IconResourceSize, resolver));
						}
						this.ImageKey = iconKey;
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Debug, e);
						this.ImageKey = string.Empty;
					}
				}

				this.SelectedImageKey = this.ImageKey;
			}

			// if the subtree was already built, we may need to rebuild it if it has changed
			if (_isSubTreeBuilt)
			{
				// rebuild the subtree only if the binding returns a different reference
				if (_parentTree.Binding.GetSubTree(_item) != _subtreeManager.Tree)
					RebuildSubTree();
			}
			else
			{
				// add a dummy child so that we get a "plus" sign next to the node
				if (_parentTree.Binding.CanHaveSubTree(_item) && this.Nodes.Count == 0)
				{
					this.Nodes.Add(new TreeNode(SR.MessageErrorRetrievingSubtree));
				}
			}

			if (this.TreeView != null)
				this.TreeView.EndUpdate();

			// check whether to start in expanded or collapsed state
			if (_parentTree.Binding.GetExpanded(_item))
				this.Expand();
			else
				this.Collapse();

			// Return the updating flag back to the previous update state, rather than setting it to null.
			_isUpdating = wasStillUpdating;
		}

		/// <summary>
        /// Rebuilds the sub-tree
        /// </summary>
        private void RebuildSubTree()
        {
			// dispose of the old sub-tree manager
			if(_subtreeManager != null)
			{
				_subtreeManager.Dispose();
				_subtreeManager = null;
			}
			
			// get the sub-tree from the binding
			// note: there is no guarantee that successive calls will return the same ITree instance,
			// which is why we create a new BindingTreeLevelManager
            var subTree = _parentTree.Binding.GetSubTree(_item);
            if (subTree != null)
            {
				_subtreeManager = new BindingTreeLevelManager(subTree, this.Nodes, _bindingTreeView);
            }
        }

		public void AfterLabelEdit(string text)
		{
			_parentTree.Binding.SetNodeText(_item, text);
		}

    	public void OnChecked()
    	{
    		if (!_isUpdating)
    		{
    			_isUpdating = true;
    			CheckState checkState = _parentTree.Binding.ToggleCheckState(_item);
				if (_bindingTreeView.CheckBoxes)
					this.StateImageKey = checkState.ToString();
    			_isUpdating = false;
    		}
    	}

		public void OnExpandCollapse()
		{
			_parentTree.Binding.SetExpanded(_item, this.IsExpanded);
		}

		#region IDisposable Members

		public void Dispose()
		{
			if(_subtreeManager != null)
			{
				_subtreeManager.Dispose();
				_subtreeManager = null;
			}
		}

		#endregion
	}
}
