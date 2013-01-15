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
using System.Collections.ObjectModel;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Comparers;

namespace ClearCanvas.ImageViewer
{
	public class ImageSetTree : IDisposable
	{
		#region Private Fields

		private readonly string _primaryStudyInstanceUid;
		private readonly ImageSetGroups _imageSetGroups;
		private readonly ImageSetTreeGroupItem _internalTree;
		private ImageSetTreeGroupItem _internalTreeRoot;
		private ISelection _selection;

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="imageSets"></param>
		/// <param name="primaryStudyInstanceUid"></param>
		public ImageSetTree(ObservableList<IImageSet> imageSets, string primaryStudyInstanceUid)
			:this(imageSets, primaryStudyInstanceUid, new ImageSetTreeItemBinding())
		{
		}

		/// <summary>
		/// Constructor that allows a custom binding to be supplied.
		/// </summary>
		/// <param name="imageSets"></param>
		/// <param name="primaryStudyInstanceUid"></param>
		/// <param name="binding"></param>
		public ImageSetTree(ObservableList<IImageSet> imageSets, string primaryStudyInstanceUid, ImageSetTreeItemBinding binding)
		{
			_primaryStudyInstanceUid = primaryStudyInstanceUid;
			_imageSetGroups = new ImageSetGroups(imageSets);
			_internalTree = new ImageSetTreeGroupItem(_imageSetGroups.Root, new StudyDateComparer(), binding);
			UpdateInternalTreeRoot();
			_internalTree.Updated += OnInternalTreeUpdated;
		}

		#region Public Properties / Events

		public ITree TreeRoot
		{
			get { return _internalTreeRoot.Tree; }
		}

		public event EventHandler TreeChanged;
		public event EventHandler TreeUpdated;

		public ISelection Selection
		{
			get
			{
				//we need the actual variable to be able to remain null, so we know when to update it automatically
				return _selection ?? new Selection();
			}
			set
			{
				value = value ?? new Selection();

				if (!Object.Equals(value, _selection))
				{
					_selection = value;
					OnSelectionChanged();
				}
			}
		}

		public event EventHandler SelectionChanged;

		#endregion

		#region Private Methods

		private void OnInternalTreeUpdated(object sender, EventArgs e)
		{
			UpdateInternalTreeRoot();
			OnTreeUpdated();
		}

		private void OnSelectionChanged()
		{
			EventsHelper.Fire(SelectionChanged, this, EventArgs.Empty);
		}

		private void OnTreeChanged()
		{
			EventsHelper.Fire(TreeChanged, this, EventArgs.Empty);
		}

		private void OnTreeUpdated()
		{
			EventsHelper.Fire(TreeUpdated, this, EventArgs.Empty);
		}

		private void UpdateInternalTreeRoot()
		{
			ImageSetTreeGroupItem treeRoot = _internalTree;

			while (treeRoot.GetItems().Count == 0)
			{
				ReadOnlyCollection<ImageSetTreeGroupItem> childGroupItems = treeRoot.GetGroupItems();
				int nonEmptyChildGroupItems = 0;
				foreach (ImageSetTreeGroupItem childGroupItem in childGroupItems)
				{
					if (childGroupItem.GetAllItems().Count > 0)
						++nonEmptyChildGroupItems;
				}

				if (nonEmptyChildGroupItems == 1)
					treeRoot = childGroupItems[0];
				else
					break;
			}

			bool treeChanged = (_internalTreeRoot != treeRoot);

			_internalTreeRoot = treeRoot;
			UpdateSelection(false);
			ExpandToSelection();

			if (treeChanged)
				OnTreeChanged();
		}

		private void UpdateSelection(bool reset)
		{
			if (reset)
				_selection = null;

			if (_selection == null)
			{
				if (!String.IsNullOrEmpty(_primaryStudyInstanceUid))
				{
					foreach (ImageSetTreeItem item in _internalTreeRoot.GetAllItems())
					{
						if (item.ImageSet.Uid == _primaryStudyInstanceUid)
						{
							this.Selection = new Selection(item);
							break;
						}
					}
				}
			}
			else if (_selection.Item != null)
			{
				if (!IsInInternalTreeRoot((IImageSetTreeItem)_selection.Item))
					this.Selection = null;
			}
		}

		private bool IsInInternalTreeRoot(IImageSetTreeItem treeItem)
		{
			while (treeItem != null)
			{
				if (treeItem == _internalTreeRoot)
					return true;

				treeItem = treeItem.Parent;
			}

			return false;
		}

		private void ExpandToSelection()
		{
			if (_selection != null && _selection.Item != null)
				ExpandTo(_selection.Item as IImageSetTreeItem);
		}

		private void ExpandTo(IImageSetTreeItem item)
		{
			ImageSetTreeGroupItem parent = item.Parent;

			while (parent != _internalTreeRoot)
			{
				parent.IsExpanded = true;
				parent = parent.Parent;
			}

			_internalTreeRoot.IsExpanded = true;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			_imageSetGroups.Dispose();

			_internalTree.Updated -= OnInternalTreeUpdated;
			_internalTree.Dispose();
		}

		#endregion
	}

	#region IImageSetTreeItem interface

	public interface IImageSetTreeItem
	{
		ImageSetTreeGroupItem Parent { get; }

		string Name { get; }
		string Description { get; }
		bool IsExpanded { get; set; }
	}

	#endregion

	#region ImageSet Tree Item

	public class ImageSetTreeItem : IImageSetTreeItem
	{
		private readonly IImageSet _imageSet;
		private readonly ImageSetTreeGroupItem _parent;

		internal ImageSetTreeItem(IImageSet imageSet, ImageSetTreeGroupItem parent)
		{
			_imageSet = imageSet;
			_parent = parent;
		}

		#region Public Properties

		public IImageSet ImageSet
		{
			get { return _imageSet; }
		}

		#region IImageSetTreeItem Members

		public ImageSetTreeGroupItem Parent
		{
			get { return _parent; }
		}

		public string Name
		{
			get { return _imageSet.Name; }
		}

		public string Description
		{
			get
			{
				/// TODO (CR Aug 2011): Should put this kind of thing into a centralized helper class of some kind
				/// because the same logic appears in RCCM and here.
				if (_imageSet.Descriptor is IDicomImageSetDescriptor)
				{
					var descriptor = (IDicomImageSetDescriptor)_imageSet.Descriptor;
					if (descriptor.LoadStudyError != null)
					{
						string message;
						if (descriptor.IsOffline)
							message = SR.MessageInfoStudyOffline;
						else if (descriptor.IsNearline)
							message = SR.MessageInfoStudyNearline;
						else if (descriptor.IsInUse)
							message = SR.MessageInfoStudyInUse;
						else if (descriptor.IsNotLoadable)
							message = SR.MessageInfoNoStudyLoader;
						else
							message = SR.MessageInfoStudyCouldNotBeLoaded;

						return String.Format(SR.MessageFormatStudyNotLoadable, Name, message);
					}
				}

				return Name;
			}
		}

		public bool IsExpanded { get; set; }

		#endregion
		#endregion

		public override string ToString()
		{
			return this.Description;
		}
	}

	#endregion

	#region ImageSet Tree Group Item

	public class ImageSetTreeGroupItem : IImageSetTreeItem, IDisposable
	{
		#region Private Fields

		private readonly ImageSetTreeGroupItem _parent;
		private readonly FilteredGroup<IImageSet> _group;
		private readonly Tree<IImageSetTreeItem> _tree;

		private readonly IComparer<IImageSet> _imageSetComparer;

		#endregion

		private ImageSetTreeGroupItem(FilteredGroup<IImageSet> group, ImageSetTreeGroupItem parent, ITreeItemBinding binding)
			: this(group, parent._imageSetComparer, binding)
		{
			_parent = parent;
		}

		internal ImageSetTreeGroupItem(FilteredGroup<IImageSet> group, IComparer<IImageSet> imageSetComparer, ITreeItemBinding binding)
		{
			_group = group;
			_imageSetComparer = imageSetComparer;
			_tree = new Tree<IImageSetTreeItem>(binding);

			_group.ItemAdded += OnItemAdded;
			_group.ItemRemoved += OnItemRemoved;

			_group.ChildGroups.ItemAdded += OnChildGroupAdded;
			_group.ChildGroups.ItemRemoved += OnChildGroupRemoved;
			_group.ChildGroups.ItemChanging += OnChildGroupChanging;
			_group.ChildGroups.ItemChanged += OnChildGroupChanged;

			Initialize();
			IsExpanded = false;
		}

		#region Public Properties/Events

		public bool IsExpanded { get; set; }

		public Tree<IImageSetTreeItem> Tree
		{
			get { return _tree; }
		}

		public event EventHandler Updated;

		#region IImageSetTreeItem Members

		public ImageSetTreeGroupItem Parent
		{
			get { return _parent; }
		}

		public string Name
		{
			get { return _group.Label; }
		}

		public string Description
		{
			get { return _group.Label; }
		}

		#endregion
		#endregion

		private void OnUpdated()
		{
			EventsHelper.Fire(Updated, this, EventArgs.Empty);
			if (Parent != null)
				Parent.OnUpdated();
		}

		private void Initialize()
		{
			List<IImageSet> imageSets = new List<IImageSet>(_group.Items);
			if (_imageSetComparer != null)
				imageSets.Sort(_imageSetComparer);

			foreach (IImageSet imageSet in imageSets)
				_tree.Items.Add(new ImageSetTreeItem(imageSet, this));

			foreach (FilteredGroup<IImageSet> childGroup in _group.ChildGroups)
				_tree.Items.Add(new ImageSetTreeGroupItem(childGroup, this, _tree.Binding));
		}

		#region Private Methods

		#region Group Handlers

		private void OnChildGroupAdded(object sender, ListEventArgs<FilteredGroup<IImageSet>> e)
		{
			ImageSetTreeGroupItem newGroupItem = new ImageSetTreeGroupItem(e.Item, this, _tree.Binding);
			_tree.Items.Add(newGroupItem);
			OnUpdated();
		}

		private void OnChildGroupRemoved(object sender, ListEventArgs<FilteredGroup<IImageSet>> e)
		{
			ImageSetTreeGroupItem groupItem = FindGroupItem(e.Item);
			if (groupItem != null)
			{
				_tree.Items.Remove(groupItem);
				groupItem.Dispose();
				OnUpdated();
			}
		}

		private ImageSetTreeGroupItem _changingGroupItem;

		private void OnChildGroupChanging(object sender, ListEventArgs<FilteredGroup<IImageSet>> e)
		{
			_changingGroupItem = FindGroupItem(e.Item);
		}

		private void OnChildGroupChanged(object sender, ListEventArgs<FilteredGroup<IImageSet>> e)
		{
			if (_changingGroupItem != null)
			{
				int replaceIndex = _tree.Items.IndexOf(_changingGroupItem);
				if (replaceIndex >= 0)
				{
					_tree.Items[replaceIndex] = new ImageSetTreeGroupItem(e.Item, this, _tree.Binding);
					OnUpdated();
				}

				_changingGroupItem.Dispose();
				_changingGroupItem = null;
			}
		}

		#endregion

		#region ImageSet Handlers

		private void OnItemAdded(object sender, ItemEventArgs<IImageSet> e)
		{
			ImageSetTreeItem newItem = new ImageSetTreeItem(e.Item, this);

			ReadOnlyCollection<ImageSetTreeItem> items = GetItems();
			List<IImageSet> imageSets = CollectionUtils.Map<ImageSetTreeItem, IImageSet>(
				items, delegate(ImageSetTreeItem item) { return item.ImageSet; });

			imageSets.Add(e.Item);

			if (_imageSetComparer != null)
				imageSets.Sort(_imageSetComparer);

			int insertIndex = imageSets.IndexOf(e.Item);
			if (insertIndex >= 0)
			{
				_tree.Items.Insert(insertIndex, newItem);
				OnUpdated();
			}
		}

		private void OnItemRemoved(object sender, ItemEventArgs<IImageSet> e)
		{
			foreach (IImageSetTreeItem item in _tree.Items)
			{
				if (item is ImageSetTreeItem)
				{
					ImageSetTreeItem treeItem = item as ImageSetTreeItem;
					if (treeItem.ImageSet == e.Item)
					{
						_tree.Items.Remove(treeItem);
						OnUpdated();
						break;
					}
				}
			}
		}

		#endregion

		private ImageSetTreeGroupItem FindGroupItem(FilteredGroup<IImageSet> filteredGroup)
		{
			ImageSetTreeGroupItem groupItem = CollectionUtils.SelectFirst(_tree.Items,
						delegate(IImageSetTreeItem treeItem)
						{
							if (treeItem is ImageSetTreeGroupItem)
								return ((ImageSetTreeGroupItem)treeItem)._group == filteredGroup;
							else
								return false;
						}) as ImageSetTreeGroupItem;

			return groupItem;
		}

		#endregion

		#region Public Methods

		public ReadOnlyCollection<ImageSetTreeItem> GetAllItems()
		{
			List<ImageSetTreeItem> items = new List<ImageSetTreeItem>();
			items.AddRange(GetItems());

			foreach (ImageSetTreeGroupItem groupItem in GetGroupItems())
				items.AddRange(groupItem.GetAllItems());

			return items.AsReadOnly();
		}

		public ReadOnlyCollection<ImageSetTreeItem> GetItems()
		{
			List<ImageSetTreeItem> items = new List<ImageSetTreeItem>();

			foreach (IImageSetTreeItem item in Tree.Items)
			{
				if (item is ImageSetTreeItem)
					items.Add(item as ImageSetTreeItem);
			}

			return items.AsReadOnly();
		}

		public ReadOnlyCollection<ImageSetTreeGroupItem> GetGroupItems()
		{
			List<ImageSetTreeGroupItem> groups = new List<ImageSetTreeGroupItem>();

			foreach (IImageSetTreeItem item in Tree.Items)
			{
				if (item is ImageSetTreeGroupItem)
					groups.Add(item as ImageSetTreeGroupItem);
			}

			return groups.AsReadOnly();
		}

		public override string ToString()
		{
			return this.Description;
		}

		#region IDisposable Members

		public void Dispose()
		{
			_group.ItemAdded -= OnItemAdded;
			_group.ItemRemoved -= OnItemRemoved;

			_group.ChildGroups.ItemAdded -= OnChildGroupAdded;
			_group.ChildGroups.ItemRemoved -= OnChildGroupRemoved;
			_group.ChildGroups.ItemChanging -= OnChildGroupChanging;
			_group.ChildGroups.ItemChanged -= OnChildGroupChanged;
		}

		#endregion
		#endregion
	}

	#endregion

	#region Tree Binding

	public class ImageSetTreeItemBinding : TreeItemBindingBase
	{
		public override string GetNodeText(object item)
		{
			return ((IImageSetTreeItem)item).Name;
		}

		public override string GetTooltipText(object item)
		{
			return ((IImageSetTreeItem)item).Description;
		}

		public override bool GetExpanded(object item)
		{
			return ((IImageSetTreeItem)item).IsExpanded;
		}

		public override void SetExpanded(object item, bool expanded)
		{
			((IImageSetTreeItem)item).IsExpanded = expanded;
		}

		public override bool CanHaveSubTree(object item)
		{
			return item is ImageSetTreeGroupItem;
		}

		public override ITree GetSubTree(object item)
		{
			if (item is ImageSetTreeGroupItem)
				return ((ImageSetTreeGroupItem)item).Tree;

			return null;
		}
	}

	#endregion
}
