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
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class NavigatorComponentContainerControl : CustomUserControl
    {
        #region TreeUpdater helper class

        /// <summary>
        /// Builds or updates the tree from a collection of navigator pages. Performs a
        /// non-destructive update (e.g. does not remove nodes that no longer have pages).
        /// </summary>
        class TreeUpdater
        {
            private IEnumerable<NavigatorPage> _pages;
            private TreeNodeCollection _rootNodes;
            private List<TreeNode> _visitedNodes;

            public TreeUpdater(IEnumerable<NavigatorPage> pages, TreeNodeCollection rootNodes)
            {
                _pages = pages;
                _rootNodes = rootNodes;
                _visitedNodes = new List<TreeNode>();
            }

            public Dictionary<NavigatorPage, TreeNode> UpdateTree()
            {
                // insert any new pages into tree
                Dictionary<NavigatorPage, TreeNode> pageMap = new Dictionary<NavigatorPage, TreeNode>();
                foreach (NavigatorPage page in _pages)
                {
                    InsertTreeNode(page, _rootNodes, 0, pageMap);
                }
                return pageMap;
            }

            private void InsertTreeNode(NavigatorPage page, TreeNodeCollection treeNodes, int depth, Dictionary<NavigatorPage, TreeNode> pageMap)
            {
                PathSegment segment = page.Path.Segments[depth];

                // see if node for this segment already exists
                TreeNode treeNode = CollectionUtils.FirstElement(treeNodes.Find(segment.LocalizedText, false));
                if (treeNode == null)
                {
                    // need to create the node, however, we can't just add it to the end of the child collection,
                    // we need to insert it at the appropriate place, which is just after the last "visited" node
                    // find first unvisited node, which indicates the insertion point
                    int i = 0;
                    for (; i < treeNodes.Count; i++)
                    {
                        if (!_visitedNodes.Contains(treeNodes[i]))
                            break;
                    }

                    // insert new node
                    treeNode = treeNodes.Insert(i, segment.LocalizedText, segment.LocalizedText);
                }

                if (depth < page.Path.Segments.Count - 1)
                {
                    // recur on next path segment
                    InsertTreeNode(page, treeNode.Nodes, depth + 1, pageMap);
                }
                else
                {
                    // this is the last path segment
                    treeNode.Tag = page;
                    pageMap.Add(page, treeNode);
                }

                // remember that this node has now been "visited"
                _visitedNodes.Add(treeNode);

            }
        }

        #endregion


        private readonly NavigatorComponentContainer _component;
        private Dictionary<NavigatorPage, TreeNode> _nodeMap;

        #region Constructor

        public NavigatorComponentContainerControl(NavigatorComponentContainer component)
		{
            InitializeComponent();

            _nodeMap = new Dictionary<NavigatorPage, TreeNode>();

            _component = component;
            _component.CurrentPageChanged += _component_CurrentNodeChanged;
			_component.Pages.ItemAdded += Pages_ItemAdded;
			_component.Pages.ItemRemoved += Pages_ItemRemoved;

			if (!_component.ShowApply)
			{
				_applyButton.Dispose();
				_applyButton = null;
			}
			else
			{
				_applyButton.Click += delegate { _component.Apply(); };
				_applyButton.DataBindings.Add("Enabled", _component, "ApplyEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			}

    		base.AcceptButton = this._okButton;
			base.CancelButton = this._cancelButton;

        	splitContainer1.Panel1Collapsed = _component.ShowTree == false;
            _nextButton.DataBindings.Add("Enabled", _component, "ForwardEnabled");
            _backButton.DataBindings.Add("Enabled", _component, "BackEnabled");
            _okButton.DataBindings.Add("Enabled", _component, "AcceptEnabled");

            // build the tree
            UpdateTree();

			if(_component.StartFullyExpanded)
			{
				// expand entire tree
				_treeView.ExpandAll();
			}
			else
			{
				// expand first-level of tree
				foreach (TreeNode treeNode in _treeView.Nodes)
				{
					treeNode.Expand();
				}
			}

            ShowPage(_component.CurrentPage);
        }

        #endregion

        #region Component event handlers

        private void Pages_ItemAdded(object sender, ListEventArgs<NavigatorPage> e)
		{
            UpdateTree();
		}

		private void Pages_ItemRemoved(object sender, ListEventArgs<NavigatorPage> e)
		{
			RemoveTreeNode(e.Item);
		}

        private void _component_CurrentNodeChanged(object sender, EventArgs e)
        {
            ShowPage(_component.CurrentPage);
        }

        #endregion

        #region GUI event handlers

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Accept();
            }
        }

        private void _nextButton_Click(object sender, EventArgs e)
        {
            _component.Forward();
        }

        private void _backButton_Click(object sender, EventArgs e)
        {
            _component.Back();
        }

        private void _treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _component.CurrentPage = (NavigatorPage)e.Node.Tag;
        }

        private void _treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            NavigatorPage page = (NavigatorPage)e.Node.Tag;
            if (page == null)
            {
                // no page associated with this node, so cancel this selection
                e.Cancel = true;

                // attempt to select the next selectable node
                TreeNode nextNode = FindNextNode(e.Node, delegate(TreeNode n) { return n.Tag != null; });
                if (nextNode != null)
                    _treeView.SelectedNode = nextNode;
            }
        }

        #endregion

        #region Helpers

        private void UpdateTree()
        {
            TreeUpdater updater = new TreeUpdater(_component.Pages, _treeView.Nodes);
            _nodeMap = updater.UpdateTree();
        }

		private void RemoveTreeNode(NavigatorPage page)
		{
			// find node in map and remove it
			TreeNode node = _nodeMap[page];
			_nodeMap.Remove(page);

			// remove node from tree, recursively removing parent nodes if empty
			RemoveNode(node);
		}

        private void ShowPage(NavigatorPage page)
        {
            // get the control to show
            Control toShow = (Control)_component.GetPageView(page).GuiElement;

            // hide all others
            foreach (Control c in _contentPanel.Controls)
            {
                if (c != toShow)
                    c.Visible = false;
            }

            // if the control has not been added to the content panel, add it now
            if (!_contentPanel.Controls.Contains(toShow))
            {
                toShow.Dock = DockStyle.Fill;
                _contentPanel.Controls.Add(toShow);
            }

            toShow.Visible = true;

            // HACK: for some reason the error provider symbols don't show up the first time the control is shown
            // therefore we need to force it
            if (toShow is ApplicationComponentUserControl)
            {
                (toShow as ApplicationComponentUserControl).ErrorProvider.UpdateBinding();
            }

            // set the title and selected tree node
            _titleBar.Text = page.Path.LastSegment.LocalizedText;
            _treeView.SelectedNode = _nodeMap[page];
        }


		/// <summary>
		/// Performs in-order traversal, returns the next node that matches the specified condition.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="condition"></param>
		/// <returns></returns>
		private static TreeNode FindNextNode(TreeNode node, Predicate<TreeNode> condition)
		{
			if (condition(node))
				return node;

			foreach (TreeNode child in node.Nodes)
			{
				TreeNode n = FindNextNode(child, condition);
				if (n != null)
					return n;
			}
			return null;
		}

        /// <summary>
        /// Removes the specified node and all childless antecedants.
        /// </summary>
        /// <param name="node"></param>
        private static void RemoveNode(TreeNode node)
        {
            TreeNode parent = node.Parent;
            node.Remove();
            if (parent.Nodes.Count == 0)
                RemoveNode(parent);
        }

        #endregion
    }
}
