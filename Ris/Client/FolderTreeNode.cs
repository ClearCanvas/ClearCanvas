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
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Represents a node in a folder explorer tree.
	/// </summary>
	internal class FolderTreeNode
	{
		#region ContainerFolder

		/// <summary>
		/// A folder that acts strictly as a parent for other folders, and does not itself contain any items.
		/// </summary>
		internal class ContainerFolder : Folder
		{
			private readonly Table<object> _itemsTable;

			public ContainerFolder(Path path, bool startExpanded)
				: base(path, startExpanded)
			{
				_itemsTable = new Table<object>();
			}

			protected override bool IsItemCountKnown
			{
				get { return true; }
			}

			public override string Text
			{
				get { return this.FolderPath.LastSegment.LocalizedText; }
			}

			protected override bool UpdateCore()
			{
				return false;
			}

			protected override void InvalidateCore()
			{
			}

			public override ITable ItemsTable
			{
				get { return _itemsTable; }
			}

			public override DragDropKind CanAcceptDrop(object[] items, DragDropKind kind)
			{
				// can't drop items into a container folder, since it contains only other folders
				return DragDropKind.None;
			}

			public override DragDropKind AcceptDrop(object[] items, DragDropKind kind)
			{
				// can't drop items into a container folder, since it contains only other folders
				return DragDropKind.None;
			}

			protected override IconSet OpenIconSet
			{
				get { return new IconSet("ContainerFolderOpenSmall.png", "ContainerFolderOpenMedium.png", "ContainerFolderOpenMedium.png"); }
			}

			protected override IconSet ClosedIconSet
			{
				get { return new IconSet("ContainerFolderClosedSmall.png", "ContainerFolderClosedMedium.png", "ContainerFolderClosedMedium.png"); }
			}
		}

		#endregion

		private readonly FolderExplorerComponent _explorer;
		private readonly Tree<FolderTreeNode> _subTree;
		private readonly FolderTreeNode _parent;
		private bool _expanded;
		private IFolder _folder;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="explorer"></param>
		/// <param name="parent"></param>
		/// <param name="path"></param>
		public FolderTreeNode(FolderExplorerComponent explorer, FolderTreeNode parent, Path path)
		{
			_explorer = explorer;
			_parent = parent;
			_subTree = new Tree<FolderTreeNode>(GetBinding(_explorer));

			// always start with container folder
			SetFolder(new ContainerFolder(path, true));
		}

		#region Public API

		/// <summary>
		/// Gets the folder at this node.
		/// </summary>
		public IFolder Folder
		{
			get { return _folder; }
		}

		/// <summary>
		/// Gets the subtree at this node.
		/// </summary>
		/// <returns></returns>
		public Tree<FolderTreeNode> GetSubTree()
		{
			return _subTree;
		}

		/// <summary>
		/// Finds a descendant node (not necessarily an immediate child) associated with the specified folder,
		/// or returns null if no such node exists.
		/// </summary>
		/// <param name="folder"></param>
		/// <returns></returns>
		public FolderTreeNode FindNode(IFolder folder)
		{
			if (_folder == folder)
				return this;

			foreach (var child in _subTree.Items)
			{
				var node = child.FindNode(folder);
				if (node != null)
					return node;
			}

			return null;
		}

		/// <summary>
		/// Ensures that this node is up to date with respect to the folder count and/or contents.
		/// Applies recursively to all descendants of this node.
		/// </summary>
		public void Update()
		{
			// update this node
			_folder.Update();

			if (!_expanded)
				return;

			// only update the child nodes if this node is expanded
			foreach (var child in _subTree.Items)
			{
				child.Update();
			}
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Removes the specified node from the subtree of this node, assuming the specified node is
		/// a descendant (not necessarily an immediate child) of this node.  Also removes any empty
		/// parent container nodes of the specified node.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected bool RemoveNode(FolderTreeNode node)
		{
			if (_subTree.Items.Contains(node))
			{
				// important to null out the folder, to unsubscribe from events, etc. before removing from the collection
				node.SetFolder(null);
				_subTree.Items.Remove(node);
				return true;
			}

			foreach (var child in _subTree.Items)
			{
				if (!child.RemoveNode(node))
					continue;

				if (child.IsEmptyContainer())
					RemoveNode(child);

				return true;
			}

			return false;
		}

		/// <summary>
		/// Inserts the specified folder into the tree, based on its path, recursively creating
		/// container nodes where necessary.
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="depth"></param>
		/// <param name="alphabetical"></param>
		protected void InsertFolder(IFolder folder, int depth, bool alphabetical)
		{
			if (depth == folder.FolderPath.Segments.Count)
			{
				SetFolder(folder);
			}
			else
			{
				var segment = folder.FolderPath.Segments[depth];

				// find an existing node at this path point
				var node = CollectionUtils.SelectFirst(_subTree.Items,
					n => Equals(n.Folder.FolderPath.Segments[depth], segment));

				var isAddingLeafNode = (depth == folder.FolderPath.Segments.Count - 1);
				var existingNodeIsAContainer = node != null && node.Folder is ContainerFolder;

				if (node == null || (isAddingLeafNode && !existingNodeIsAContainer))
				{
					// create the node if it doesn't exist, or if this is the leaf node
					node = new FolderTreeNode(_explorer, this, folder.FolderPath.SubPath(0, depth + 1));
					if(alphabetical)
						InsertChildAlphabetical(node, depth);
					else
						_subTree.Items.Add(node);
				}

				node.InsertFolder(folder, depth + 1, alphabetical);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this node is currently expanded.
		/// </summary>
		protected bool Expanded
		{
			get { return _expanded; }
			set
			{
				_expanded = value;
				if(_expanded)
				{
					Update();
				}
			}
		}

		/// <summary>
		/// Notifies the tree to update the display.
		/// </summary>
		protected internal void NotifyItemUpdated()
		{
			// parent may be null iff this is a root node
			// parent item may not yet contain this node, if this is node has not yet been added to the parent's
			// item collection (a transient state that occurs while the tree is being built)
			if (_parent != null && _parent.GetSubTree().Items.Contains(this))
				_parent.GetSubTree().Items.NotifyItemUpdated(this);
		}

		#endregion

		#region Private Helpers

		/// <summary>
		/// Gets a value indicating whether this node has a sub-tree.
		/// </summary>
		private bool CanHaveSubTree
		{
			get
			{
				if (this.Folder is ContainerFolder)
					return true;

				return this.GetSubTree().Items.Count > 0;
			}
		}

		/// <summary>
		/// Sets the folder associatd with this node.
		/// </summary>
		/// <param name="folder"></param>
		private void SetFolder(IFolder folder)
		{
			if(_folder != null)
			{
				_folder.TextChanged -= FolderTextOrIconChangedEventHandler;
				_folder.IconChanged -= FolderTextOrIconChangedEventHandler;
			}

			_folder = folder;

			if (_folder == null)
				return;

			_folder.TextChanged += FolderTextOrIconChangedEventHandler;
			_folder.IconChanged += FolderTextOrIconChangedEventHandler;
			_expanded = _folder.StartExpanded;

			// since the folder has changed, need to immediately notify the tree that this item is updated.
			NotifyItemUpdated();
		}

		/// <summary>
		/// Gets a value indicating whether this node is an empty container.
		/// </summary>
		/// <returns></returns>
		private bool IsEmptyContainer()
		{
			return _folder is ContainerFolder && _subTree.Items.Count == 0;
		}

		/// <summary>
		/// Listens for changes from the folder. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FolderTextOrIconChangedEventHandler(object sender, EventArgs e)
		{
			NotifyItemUpdated();
		}

		/// <summary>
		/// Inserts the specified node alphabetically as a child of this node.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="pathDepth"></param>
		private void InsertChildAlphabetical(FolderTreeNode node, int pathDepth)
		{
			var segment = node.Folder.FolderPath.Segments[pathDepth];

			// find the insertion point - the first node greater/equalto the node to be inserted
			var insertPoint = _subTree.Items.FindIndex(
				n => n.Folder.FolderPath.Segments[pathDepth].LocalizedText.CompareTo(segment.LocalizedText) >= 0);

			if (insertPoint > -1)
				_subTree.Items.Insert(insertPoint, node);
			else
				_subTree.Items.Add(node);
		}

		/// <summary>
		/// Constructs a tree item binding.
		/// </summary>
		/// <param name="explorer"></param>
		/// <returns></returns>
		private static TreeItemBinding<FolderTreeNode> GetBinding(FolderExplorerComponent explorer)
		{
			var binding = new TreeItemBinding<FolderTreeNode>
				{
					NodeTextProvider = node => node.Folder.Text,
					IconSetProvider = node => node.Folder.IconSet,
					TooltipTextProvider = node => node.Folder.Tooltip,
					ResourceResolverProvider = node => node.Folder.ResourceResolver,
					CanAcceptDropHandler = explorer.CanFolderAcceptDrop,
					AcceptDropHandler = explorer.FolderAcceptDrop,
					CanHaveSubTreeHandler = node => node.CanHaveSubTree,
					IsExpandedGetter = node => node.Expanded,
					IsExpandedSetter = (node, expanded) => node.Expanded = expanded,
					SubTreeProvider = node => node.GetSubTree()
				};

			return binding;
		}

		#endregion

	}

	/// <summary>
	/// Represents the root node in a folder explorer tree.
	/// </summary>
	internal class FolderTreeRoot : FolderTreeNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="explorer"></param>
		public FolderTreeRoot(FolderExplorerComponent explorer)
			: base(explorer, null, new Path("", null))
		{
		}

		/// <summary>
		/// Inserts the specified folders into the tree.
		/// </summary>
		/// <param name="folders"></param>
		/// <param name="alphabetical"></param>
		public void InsertFolders(IEnumerable<IFolder> folders, bool alphabetical)
		{
			foreach (var folder in folders)
			{
				InsertFolder(folder, 0, alphabetical);
			}
		}


		/// <summary>
		/// Inserts the specified folder into the tree.
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="alphabetical"></param>
		public void InsertFolder(IFolder folder, bool alphabetical)
		{
			InsertFolder(folder, 0, alphabetical);
		}

		/// <summary>
		/// Removes the specified folder from the tree.
		/// </summary>
		/// <param name="folder"></param>
		public void RemoveFolder(IFolder folder)
		{
			var node = FindNode(folder);
			if (node != null)
			{
				RemoveNode(node);
			}
		}

		/// <summary>
		/// Notifies the view that properties of the folder, such as its text or icon, have changed.
		/// </summary>
		/// <param name="folder"></param>
		public void NotifyFolderPropertiesUpdated(IFolder folder)
		{
			var node = FindNode(folder);
			if (node != null)
			{
				node.NotifyItemUpdated();
			}
		}
	}
}
