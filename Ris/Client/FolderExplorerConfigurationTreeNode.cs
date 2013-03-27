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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Abstract base class for tree nodes in the folder explorer configuration page.
	/// </summary>
	public abstract class FolderConfigurationNodeBase
	{
		public class ContainerNode : FolderConfigurationNodeBase
		{
			private string _text;
			private readonly IResourceResolver _resourceResolver;

			public ContainerNode(string text)
				: base(false)
			{
				_text = text;
				_resourceResolver = new ResourceResolver(typeof(ContainerNode).Assembly);
			}

			#region FolderConfigurationNodeBase Overrides

			public override string Text
			{
				get { return _text; }
				set
				{
					if (_text == value)
						return;

					_text = value;
					this.Modified = true;
				}
			}

			public override string ToolTip
			{
				get { return _text; }
			}

			public override bool CanEdit
			{
				get { return true; }
			}

			public override bool CanDelete
			{
				get { return _subTree == null || _subTree.Items.Count == 0; }
			}

			public override IconSet IconSet
			{
				get { return new IconSet("ContainerFolderOpenSmall.png", "ContainerFolderOpenSmall.png", "ContainerFolderOpenSmall.png"); }
			}

			public override IResourceResolver ResourceResolver
			{
				get { return _resourceResolver; }
			}

			#endregion
		}

		private bool _isChecked;
		private bool _isExpanded;
		private FolderConfigurationNodeBase _parent;
		private Tree<FolderConfigurationNodeBase> _subTree;

		private bool _modified;
		private bool _modifiedEnabled;
		private event EventHandler _modifiedChanged;

		protected FolderConfigurationNodeBase()
			: this(true)
		{
		}

		protected FolderConfigurationNodeBase(bool isChecked)
		{
			_isExpanded = true;
			_isChecked = isChecked;
		}

		#region Abstract and Virtual Properties

		/// <summary>
		/// Gets or sets the display text of this node.
		/// </summary>
		public abstract string Text { get; set; }

		/// <summary>
		/// Gets or sets the tooltip of the node.
		/// </summary>
		public abstract string ToolTip { get; }

		/// <summary>
		/// Gets whether this node can be edited.
		/// </summary>
		public abstract bool CanEdit { get; }

		/// <summary>
		/// Gets whether this node can be deleted.
		/// </summary>
		public abstract bool CanDelete { get; }

		/// <summary>
		/// Gets or sets whether the node is checked.
		/// </summary>
		public virtual bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				if (_isChecked == value)
					return;

				SetCheckStateInternal(value);

				if (!this.CheckStateChained)
					return;

				PropagateCheckStateUp(_parent);
				PropagateCheckStateDown(this);
			}
		}

		/// <summary>
		/// Gets or sets whether un/checking will affect check state of parent and children.  Default is true.
		/// </summary>
		/// <remarks>
		/// If true, the parent will be checked if this node is checked.  The descendents will be unchecked if this node is unchecked.
		/// If false, checking or unchecking this node will not affect the check state of parent or children.
		/// </remarks>
		public virtual bool CheckStateChained
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the iconset that should be displayed for the node.
		/// </summary>
		public virtual IconSet IconSet
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the resource resolver that is used to resolve the Icon
		/// </summary>
		public virtual IResourceResolver ResourceResolver
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the path of this node.
		/// </summary>
		public virtual Path Path
		{
			get
			{
				var thisPath = new PathSegment(this.Text);

				if (_parent == null || _parent.Path == null)
					return new Path(thisPath);

				return _parent.Path.Append(thisPath);
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets whether this node has been modified.
		/// </summary>
		public bool Modified
		{
			get { return _modified; }
			protected set
			{
				if (!_modifiedEnabled || value == _modified)
					return;

				_modified = value;

				if (_parent != null)
					_parent.Modified = true;

				EventsHelper.Fire(_modifiedChanged, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets whether the Modified property is in effect or not.  It should be set to false when initializing the tree, where nodes
		/// are being inserted but the Modified property should remain false.
		/// </summary>
		public bool ModifiedEnabled
		{
			get { return _modifiedEnabled; }
			set
			{
				_modifiedEnabled = value;

				if (_subTree != null)
				{
					CollectionUtils.ForEach(_subTree.Items, child => child.ModifiedEnabled = value);
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="Modified"/> property has changed.
		/// </summary>
		public event EventHandler ModifiedChanged
		{
			add { _modifiedChanged += value; }
			remove { _modifiedChanged -= value; }
		}
		
		/// <summary>
		/// Gets or sets whether the subtree of this node is expanded.
		/// </summary>
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set { _isExpanded = value; }
		}

		/// <summary>
		/// Gets the previous sibling of this node, null if none.
		/// </summary>
		public FolderConfigurationNodeBase PreviousSibling
		{
			get
			{
				if (_parent == null)
					return null;

				// Find index of current node
				var index = _parent.SubTree.Items.IndexOf(this);

				// has older sibling
				return index <= 0 ? null : _parent.SubTree.Items[index - 1];
			}
		}

		/// <summary>
		/// Gets the next sibling of this node, null if none.
		/// </summary>
		public FolderConfigurationNodeBase NextSibling
		{
			get
			{
				if (_parent == null)
					return null;

				// Find index of current node
				var index = _parent.SubTree.Items.IndexOf(this);

				// has younger sibling
				return index == _parent.SubTree.Items.Count - 1 ? null : _parent.SubTree.Items[index + 1];
			}
		}

		/// <summary>
		/// Gets the parent of this node, null if none.
		/// </summary>
		public FolderConfigurationNodeBase Parent
		{
			get { return _parent; }
			private set { _parent = value; }
		}

		/// <summary>
		/// Gets a list of all descendent nodes, by in-order traversal.
		/// </summary>
		public List<FolderConfigurationNodeBase> Descendents
		{
			get
			{
				var descendents = new List<FolderConfigurationNodeBase>();

				if (_subTree != null)
				{
					CollectionUtils.ForEach(_subTree.Items,
						delegate(FolderConfigurationNodeBase child)
						{
							descendents.Add(child);
							descendents.AddRange(child.Descendents);
						});
				}

				return descendents;
				
			}
		}

		/// <summary>
		/// Gets the subtree of this node, null if none.
		/// </summary>
		public Tree<FolderConfigurationNodeBase> SubTree
		{
			get { return _subTree; }
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Clear all the children from this node.
		/// </summary>
		public void ClearSubTree()
		{
			if (_subTree == null)
				return;

			_subTree.Items.Clear();
			_subTree = null;
		}

		/// <summary>
		/// Add a node to the sub tree.
		/// </summary>
		public void AddChildNode(FolderConfigurationNodeBase node)
		{
			BuildSubTree();

			node.Parent = this;
			node.ModifiedEnabled = this.ModifiedEnabled;
			_subTree.Items.Add(node);
			this.Modified = true;

			// expand the tree right away
			this.ExpandSubTree();
		}

		/// <summary>
		/// Remove a child node and return the node's next sibling, previous sibling or the parent node.
		/// </summary>
		public FolderConfigurationNodeBase RemoveChildNode(FolderConfigurationNodeBase node)
		{
			var nextSelectedNode = node.NextSibling ?? node.PreviousSibling ?? this;
			this.SubTree.Items.Remove(node);
			this.Modified = true;
			return nextSelectedNode;
		}

		/// <summary>
		/// Replace a child node with a new node.
		/// </summary>
		public void ReplaceChildNode(FolderConfigurationNodeBase oldChildNode, FolderConfigurationNodeBase newChildNode)
		{
			// Move the subtree of the old node to the new node.
			foreach (var node in oldChildNode.SubTree.Items)
			{
				newChildNode.AddChildNode(node);
			}
			oldChildNode.ClearSubTree();
			newChildNode.Parent = oldChildNode.Parent;

			// replace the nodes
			var index = this.SubTree.Items.IndexOf(oldChildNode);
			this.SubTree.Items.Insert(index, newChildNode);
			this.SubTree.Items.Remove(oldChildNode);
			this.Modified = true;
		}

		/// <summary>
		/// Insert a node to the proper depth using the path.
		/// </summary>
		public void InsertNode(FolderConfigurationNodeBase node, Path path)
		{
			// There is no recommended path.  Add it immediately
			if (path == null || path.Segments.Count == 0)
			{
				AddChildNode(node);
				return;
			}

			var text  = CollectionUtils.FirstElement(path.Segments).LocalizedText;
			var childWithMatchingText = _subTree == null ? null : CollectionUtils.SelectFirst(_subTree.Items, child => child.Text == text);
			
			if (childWithMatchingText == null)
			{
				if (path.Segments.Count == 1)
				{
					// There are no more depth to the path, add child now.
					AddChildNode(node);
					PropagateCheckStateUp(node.Parent);
				}
				else
				{
					// create a container node and insert into the container node's subtree
					var containerNode = new ContainerNode(text);
					AddChildNode(containerNode);
					containerNode.InsertNode(node, path.SubPath(1, path.Segments.Count - 1));
				}
			}
			else
			{
				if (path.Segments.Count == 1)
				{
					// There are no more depth to the path, add child now.
					if (childWithMatchingText is ContainerNode)
						ReplaceChildNode(childWithMatchingText, node);
					else
						AddChildNode(node);

					PropagateCheckStateUp(node.Parent);
				}
				else
				{
					// insert this node child's subtree
					childWithMatchingText.InsertNode(node, path.SubPath(1, path.Segments.Count - 1));
				}
			}
		}

		/// <summary>
		/// Move the node up by swapping with the previous sibling.
		/// </summary>
		public void MoveUp()
		{
			var previousSibling = this.PreviousSibling;
			if (_parent == null || previousSibling == null)
				return;

			var index = _parent.SubTree.Items.IndexOf(this);

			// remove and re-insert this node at the prior index
			_parent.SubTree.Items.Remove(previousSibling);
			_parent.SubTree.Items.Insert(index, previousSibling);

			this.Modified = true;
		}

		/// <summary>
		/// Move the node down by swapping with the next sibling.
		/// </summary>
		public void MoveDown()
		{
			var nextSibling = this.NextSibling;
			if (_parent == null || nextSibling == null)
				return;

			// Find index of current node
			var index = _parent.SubTree.Items.IndexOf(this);

			// remove and re-insert this node at the next index
			_parent.SubTree.Items.Remove(nextSibling);
			_parent.SubTree.Items.Insert(index, nextSibling);

			this.Modified = true;
		}

		#endregion

		#region Drag & Drop supports

		public DragDropKind CanAcceptDrop(FolderConfigurationNodeBase dropData, DragDropKind kind)
		{
			if (dropData == null || this == dropData || this == dropData.Parent || this.IsDescendentOf(dropData))
				return DragDropKind.None;

			return DragDropKind.Move;
		}

		public DragDropKind AcceptDrop(FolderConfigurationNodeBase dropData, DragDropKind kind)
		{
			if (dropData.Parent != null)
				dropData.Parent.SubTree.Items.Remove(dropData);

			AddChildNode(dropData);
			PropagateCheckStateUp(this);

			this.Modified = true;

			return DragDropKind.Move;
		}

		#endregion

		#region Private helpers

		private void BuildSubTree()
		{
			if (_subTree != null)
				return;

			_subTree = BuildTree();

			ExpandSubTree();
		}

		private void ExpandSubTree()
		{
			this.IsExpanded = true;
			NotifyItemUpdated();
		}

		private void NotifyItemUpdated()
		{
			if (_parent == null)
				return;

			_parent.SubTree.Items.NotifyItemUpdated(this);
		}

		/// <summary>
		/// Determine whether this node is a descendent of the ancestorNode.
		/// </summary>
		/// <param name="ancestorNode">The unknown ancestorNode.</param>
		/// <returns>True if this node is the descendent of the ancestorNode.</returns>
		private bool IsDescendentOf(FolderConfigurationNodeBase ancestorNode)
		{
			// testNode has no children
			if (ancestorNode.SubTree == null)
				return false;

			var isDescendentOfAncestorNode = CollectionUtils.Contains(ancestorNode.SubTree.Items,
				childOfTestNode => this == childOfTestNode || this.IsDescendentOf(childOfTestNode));

			return isDescendentOfAncestorNode;
		}

		/// <summary>
		/// Sets the checked state without propagating.
		/// </summary>
		/// <param name="value"></param>
		protected virtual void SetCheckStateInternal(bool value)
		{
			if (_isChecked == value)
				return;

			_isChecked = value;
			this.Modified = true;
			NotifyItemUpdated();
		}

		/// <summary>
		/// Propagates check state up to parent.
		/// </summary>
		/// <param name="parent"></param>
		private static void PropagateCheckStateUp(FolderConfigurationNodeBase parent)
		{
			if (parent == null)
				return;

			var b = CollectionUtils.Contains(parent.SubTree.Items, n => n.IsChecked);
			parent.SetCheckStateInternal(b);

			PropagateCheckStateUp(parent.Parent);
		}

		/// <summary>
		/// Propagates check state down to children.
		/// </summary>
		/// <param name="parent"></param>
		private static void PropagateCheckStateDown(FolderConfigurationNodeBase parent)
		{
			if (parent == null || parent.SubTree == null)
				return;

			foreach (var child in parent.SubTree.Items)
			{
				child.SetCheckStateInternal(parent.IsChecked);
				PropagateCheckStateDown(child);
			}
		}

		#endregion

		public static Tree<FolderConfigurationNodeBase> BuildTree()
		{
			var binding = new TreeItemBinding<FolderConfigurationNodeBase>(node => node.Text, node => node.SubTree)
				{
					NodeTextSetter = (node, text) => node.Text = text,
					CanSetNodeTextHandler = node => node.CanEdit,
					CanHaveSubTreeHandler = node => node.SubTree != null,
					IsCheckedGetter = node => node.IsChecked,
					IsCheckedSetter = (node, isChecked) => node.IsChecked = isChecked,
					TooltipTextProvider = node => node.ToolTip,
					IsExpandedGetter = node => node.IsExpanded,
					IsExpandedSetter = (node, isExpanded) => node.IsExpanded = isExpanded,
					CanAcceptDropHandler = (node, dropData, kind) => node.CanAcceptDrop((dropData as FolderConfigurationNodeBase), kind),
					AcceptDropHandler = (node, dropData, kind) => node.AcceptDrop((dropData as FolderConfigurationNodeBase), kind),
					IconSetProvider = node => node.IconSet,
					ResourceResolverProvider = node => node.ResourceResolver
				};
			return new Tree<FolderConfigurationNodeBase>(binding);
		}
	}

	/// <summary>
	/// Represents a folder-system in the folder explorer configuration page.
	/// </summary>
	public class FolderSystemConfigurationNode : FolderConfigurationNodeBase
	{
		private readonly IFolderSystem _folderSystem;
		private readonly bool _readonly;
		private bool _initialized;

		public FolderSystemConfigurationNode(IFolderSystem folderSystem, bool isReadonly)
		{
			_folderSystem = folderSystem;
			_readonly = isReadonly;
		}

		public void InitializeFolderSystemOnce()
		{
			if (_initialized) return;

			_folderSystem.Initialize();
			_initialized = true;
		}

		public IFolderSystem FolderSystem
		{
			get { return _folderSystem; }
		}

		public bool Readonly
		{
			get { return _readonly; }
		}

		/// <summary>
		/// Gets a list of all folders for this folder system.
		/// </summary>
		public List<IFolder> Folders
		{
			get
			{
				var folders = new List<IFolder>();
				CollectionUtils.ForEach(this.Descendents,
					delegate(FolderConfigurationNodeBase node)
						{
							if (node is FolderConfigurationNode)
							{
								var folderNode = (FolderConfigurationNode)node;
								folders.Add(folderNode.Folder);
							}
						});

				return folders;
			}
		}

		/// <summary>
		/// Update IFolder with the current path of the folder node.
		/// </summary>
		public void UpdateFolderPath()
		{
			CollectionUtils.ForEach(this.Descendents,
				delegate(FolderConfigurationNodeBase node)
				{
					if (node is FolderConfigurationNode)
					{
						var folderNode = (FolderConfigurationNode) node;
						folderNode.Folder.FolderPath = folderNode.Path;
					}
				});
		}

		#region FolderConfigurationNodeBase Overrides

		public override string Text
		{
			get { return _folderSystem.Title; }
			set { }
		}

		public override string ToolTip
		{
			get { return _folderSystem.Title; }
		}

		public override bool CanEdit
		{
			get { return false; }
		}

		public override bool CanDelete
		{
			get { return false; }
		}

		public override bool CheckStateChained
		{
			get { return false; }
		}

		public override Path Path
		{
			get
			{
				// This is overwritten because we don't want folder system name to be part of the path
				return null;
			}
		}

		#endregion
	}

	/// <summary>
	/// Represents a folder in the folder explorer configuration page.
	/// </summary>
	public class FolderConfigurationNode : FolderConfigurationNodeBase
	{
		private readonly IFolder _folder;
		private string _text;
		private readonly IResourceResolver _resourceResolver;

		public FolderConfigurationNode(IFolder folder)
			: base(folder.Visible)
		{
			_folder = folder;
			_text = folder.Name;
			_resourceResolver = new ResourceResolver(typeof(ContainerNode).Assembly);
		}

		public IFolder Folder
		{
			get { return _folder; }
		}

		#region FolderConfigurationNodeBase Overrides

		public override string Text
		{
			get { return _text; }
			set
			{
				if (_text == value)
					return;

				_text = value;
				this.Modified = true;
			}
		}

		public override string ToolTip
		{
			get { return _folder.Tooltip; }
		}

		public override bool CanEdit
		{
			get { return true; }
		}

		public override bool CanDelete
		{
			get { return false; }
		}

		public override IconSet IconSet
		{
			get { return _folder.IconSet; }
		}

		public override IResourceResolver ResourceResolver
		{
			get { return _resourceResolver; }
		}

		protected override void SetCheckStateInternal(bool value)
		{
			base.SetCheckStateInternal(value);
			_folder.Visible = value;
		}

		#endregion
	}
}
