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

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop;
using Crownwood.DotNetMagic.Controls;
using ClearCanvas.Common.Utilities;
using CheckState=Crownwood.DotNetMagic.Controls.CheckState;

namespace ClearCanvas.Enterprise.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="AuthorityGroupEditorComponent"/>
    /// </summary>
    public partial class AuthorityGroupEditorComponentControl : ApplicationComponentUserControl
    {
		#region IgnoreCheckStateEventsScope class

		class IgnoreCheckStateEventsScope : IDisposable
		{
			[ThreadStatic]
			private static bool _ignore;

			public static bool Ignore
			{
				get { return _ignore; }
			}

			public IgnoreCheckStateEventsScope()
			{
				_ignore = true;
			}

			void IDisposable.Dispose()
			{
				_ignore = false;
			}
		}

		#endregion

    	private AuthorityGroupEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthorityGroupEditorComponentControl(AuthorityGroupEditorComponent component)
			:base(component)
        {
            InitializeComponent();

            _component = component;

            _authorityGroupName.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
            _authorityGroupDescription.DataBindings.Add("Value", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);
            _authorityGroupDataGroup.DataBindings.Add("Checked", _component, "DataGroup", true,
                                                      DataSourceUpdateMode.OnPropertyChanged);

			// build the authority token tree

			// ignore check state change events while building the tree 
			using(new IgnoreCheckStateEventsScope())
			{
				BuildTree(_component.AuthorityTokens);

				// initially expand only the mixed nodes
				ExpandMixedNodes(_tokenTreeView.Nodes);

				// make sure a node is selected prior to displaying the tree ctrl, otherwise it does some annoying things
				Node firstNode = CollectionUtils.FirstElement<Node>(_tokenTreeView.Nodes);
				if (firstNode != null)
					firstNode.Select();
			}
		}

		private void BuildTree(IEnumerable<AuthorityTokenTableEntry> tokens)
		{
			foreach (AuthorityTokenTableEntry token in tokens)
			{
				InsertToken(_tokenTreeView.Nodes, token, 0);
			}
		}

		private void InsertToken(NodeCollection nodeCollection, AuthorityTokenTableEntry token, int depth)
		{
			// check if a node for this path segment already exists in the tree
			PathSegment segment = token.Path.Segments[depth];
			Node node = CollectionUtils.SelectFirst<Node>(nodeCollection,
			                            delegate(Node n)
			                            {
			                            	return n.Text == segment.LocalizedText;
			                            });
			// if not, create the node
			if(node == null)
			{
				node = new Node(segment.LocalizedText);
				node.CheckStateChanged += new EventHandler(node_CheckStateChanged);
				nodeCollection.Add(node);
			}

			// if this is a leaf node, set the properties of the tree node accordingly
			if (segment == token.Path.LastSegment)
			{
				node.Tag = token;
				node.Tooltip = token.Description;
				node.CheckState = token.Selected ? CheckState.Checked : CheckState.Unchecked;

				// addition of this leaf may need to alter the check state of the parent
				if (node.Parent != null)
				{
					PropagateCheckStateUp(node.Parent);
				}
			}
			else
			{
				// otherwise, recur until we hit the leaf
				InsertToken(node.Nodes, token, depth + 1);
			}
		}

		private void ExpandMixedNodes(NodeCollection nodes)
		{
			foreach (Node node in nodes)
			{
				if(node.CheckState == CheckState.Mixed)
				{
					node.Expand();
					ExpandMixedNodes(node.Nodes);
				}
			}
		}

		private void node_CheckStateChanged(object sender, EventArgs e)
		{
			Node node = (Node)sender;

			// for leaf nodes, update the underlying item
			AuthorityTokenTableEntry token = (AuthorityTokenTableEntry) node.Tag;
			bool nodeChecked = node.CheckState == CheckState.Checked;
			if (token != null && token.Selected != nodeChecked)
				token.Selected = nodeChecked;

			// check if this event should be ignored because it was not generated by a user action
			if(IgnoreCheckStateEventsScope.Ignore)
				return;

			if(node.CheckState == CheckState.Mixed)
			{
				// don't allow the user to set the state to mixed - force it to unchecked
				node.CheckState = CheckState.Unchecked;
			}
			else
			{
				// otherwise, update child and parent nodes appropriately
				using(new IgnoreCheckStateEventsScope())
				{
					PropagateCheckStateDown(node);
					if (node.Parent != null)
					{
						PropagateCheckStateUp(node.Parent);
					}
				}
			}
		}

		private void PropagateCheckStateUp(Node parent)
		{
			bool allChecked =
				CollectionUtils.TrueForAll<Node>(parent.Nodes, delegate(Node n) { return n.CheckState == CheckState.Checked; });
			bool noneChecked =
				CollectionUtils.TrueForAll<Node>(parent.Nodes, delegate(Node n) { return n.CheckState == CheckState.Unchecked; });

			if (allChecked)
				parent.CheckState = CheckState.Checked;
			else if (noneChecked)
				parent.CheckState = CheckState.Unchecked;
			else
				parent.CheckState = CheckState.Mixed;

			if(parent.Parent != null)
				PropagateCheckStateUp(parent.Parent);
		}

		private void PropagateCheckStateDown(Node parent)
		{
			if(parent.CheckState != CheckState.Mixed)
			{
				foreach (Node child in parent.Nodes)
				{
					child.CheckState = parent.CheckState;
					PropagateCheckStateDown(child);
				}
			}
		}

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

		private void _tokenTreeView_AfterSelect(TreeControl tc, NodeEventArgs e)
		{
			Node node = e.Node;
			if(node != null && node.Tag != null)
			{
				_description.Value = ((AuthorityTokenTableEntry) node.Tag).Description;
			}
			else
			{
				_description.Value = "";
			}
		}
    }
}
