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
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
	internal class SimpleSpecification<T> : ISpecification where T : class
	{
		private readonly Predicate<T> _test;

		public SimpleSpecification(Predicate<T> test)
		{
			Platform.CheckForNullReference(test, "test");
			_test = test;
		}

		#region ISpecification Members

		public TestResult Test(object obj)
		{
			if (obj is T && _test(obj as T))
				return new TestResult(true);
			else
				return new TestResult(false);
		}

		#endregion
	}

	internal class SimpleSpecification : SimpleSpecification<object>
	{
		public SimpleSpecification(Predicate<object> test)
			: base(test)
		{
		}
	}

	/// <summary>
	/// An <see cref="ObservableList{TItem}"/> of <see cref="FilteredGroup{T}"/>s.
	/// </summary>
	public class FilteredGroupList<T> : ObservableList<FilteredGroup<T>> where T : class
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public FilteredGroupList()
		{
		}
	}

	/// <summary>
	/// The root <see cref="FilteredGroup{T}"/> class.
	/// </summary>
	/// <remarks>
	/// You must use a <see cref="RootFilteredGroup{T}"/> as the top-level object
	/// in order to be able to effectively use the <see cref="FilteredGroup{T}"/> functionality.
	/// The <see cref="FilteredGroup{T}"/> class is purposely closed off and all
	/// the functionality for adding items must occur at the root level so that
	/// the items can be processed by all the filters.
	/// </remarks>
	public class RootFilteredGroup<T> : FilteredGroup<T> where T : class
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public RootFilteredGroup()
			: this("Root", "All Items")
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">A name for the group.</param>
		/// <param name="label">A description for the group.</param>
		public RootFilteredGroup(string name, string label)
			: this(name, label, ReturnTrue)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">A name for the group.</param>
		/// <param name="label">A description for the group.</param>
		/// <param name="test">A delegate that tests for membership of an item in this group.</param>
		public RootFilteredGroup(string name, string label, Predicate<T> test)
			: this(name, label, test, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">A name for the group.</param>
		/// <param name="label">A description for the group.</param>
		/// <param name="specification">An <see cref="ISpecification"/> that tests for membership of an item in this group.</param>
		public RootFilteredGroup(string name, string label, ISpecification specification)
			: this(name, label, specification, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">A name for the group.</param>
		/// <param name="label">A description for the group.</param>
		/// <param name="childGroupFactory">A factory that is capable of dynamically creating a new <see cref="FilteredGroup{T}"/>
		/// when an item is added that does not belong to any of the other groups.</param>
		public RootFilteredGroup(string name, string label, IFilteredGroupFactory<T> childGroupFactory)
			: this(name, label, new SimpleSpecification(ReturnTrue), childGroupFactory)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">A name for the group.</param>
		/// <param name="label">A description for the group.</param>
		/// <param name="test">A delegate that tests for membership of an item in this group.</param>
		/// <param name="childGroupFactory">A factory that is capable of dynamically creating a new <see cref="FilteredGroup{T}"/>
		/// when an item is added that does not belong to any of the other groups.</param>
		public RootFilteredGroup(string name, string label, Predicate<T> test, IFilteredGroupFactory<T> childGroupFactory)
			: base(name, label, test, childGroupFactory)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">A name for the group.</param>
		/// <param name="label">A description for the group.</param>
		/// <param name="specification">An <see cref="ISpecification"/> that tests for membership of an item in this group.</param>
		/// <param name="childGroupFactory">A factory that is capable of dynamically creating a new <see cref="FilteredGroup{T}"/>
		/// when an item is added that does not belong to any of the other groups.</param>
		public RootFilteredGroup(string name, string label, ISpecification specification, IFilteredGroupFactory<T> childGroupFactory)
			: base(name, label, specification, childGroupFactory)
		{
		}

		private static bool ReturnTrue<U>(U item)
		{
			return true;
		}

		/// <summary>
		/// Adds an item to the tree of <see cref="FilteredGroup{T}"/>s.
		/// </summary>
		public void Add(T item)
		{
			base.AddItem(item);
		}

		/// <summary>
		/// Adds multiple items to the tree of <see cref="FilteredGroup{T}"/>s.
		/// </summary>
		public void Add(IEnumerable<T> items)
		{
			base.AddItems(items);
		}

		/// <summary>
		/// Removes an item from the tree of <see cref="FilteredGroup{T}"/>s.
		/// </summary>
		public void Remove(T item)
		{
			base.RemoveItem(item);
		}

		/// <summary>
		/// Clears the entire tree of <see cref="FilteredGroup{T}"/>s.
		/// </summary>
		public new void Clear()
		{
			base.Clear();
		}
	}

	/// <summary>
	/// An interface defining a factory for <see cref="FilteredGroup{T}"/>s based on a given item.
	/// </summary>
	public interface IFilteredGroupFactory<T> where T: class
	{
		/// <summary>
		/// Creates a new <see cref="FilteredGroup{T}"/> based on the properties of the given item.
		/// </summary>
		/// <returns>A new <see cref="FilteredGroup{T}"/>, or null if not applicable.</returns>
		FilteredGroup<T> Create(T item);
	}

	/// <summary>
	/// A filter node in a tree of <see cref="FilteredGroup{T}"/>s.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This utility class is very useful for automatically filtering a
	/// flat list of objects into a tree of related groups.
	/// </para>
	/// <para>
	/// The functionality of this class is difficult to define in words, but
	/// it can be thought of as an (albeit inefficient) simple in-memory database,
	/// except that the entire database and query results are always in memory.
	/// The <typeparam name="T">items</typeparam> belonging to each <see cref="FilteredGroup{T}">node</see>
	/// in the tree are essentially the results of a query that is the combination
	/// of itself and all its parent <see cref="FilteredGroup{T}">nodes</see> evaluated with logical AND.
	/// </para>
	/// <para>
	/// As a simple example, imagine a set of words where you want to be able to
	/// easily identify ones that start with a certain letter combination, but you don't want
	/// to be constantly performing the search through a linear list over and over again.
	/// </para>
	/// <para>
	/// Notice also that the <see cref="FilteredGroup{T}"/>s are fully dynamic.  You can create self-managing
	/// <see cref="FilteredGroup{T}">groups</see> by using the <see cref="IFilteredGroupFactory{T}"/> and you can
	/// also add/remove <see cref="FilteredGroup{T}">groups</see> and the affected items will be re-evaluated.
	/// An example would be removing <b>groupWordsBeginningWithAB</b> from the groups in the previous code example.
	/// Upon removal, all the items belonging to that group would simply be moved up to the parent group (<b>groupWordsBeginningWithA</b>).
	/// </para>
	/// </remarks>
	/// <example>
	/// An example using the <see cref="FilteredGroup{T}"/>s would be:
	/// <code>
	/// [C#]
	///	RootFilteredGroup{string} root = new RootFilteredGroup{string}("Words", "Database of words");
	/// FilteredGroup{string} groupWordsBeginningWithA = new FilteredGroup{string}("A", "Words beginning with A",
	///		delegate(string word) { return word.StartsWith("a", true, null); });
	/// 
	///	FilteredGroup{string} groupWordsBeginningWithAB = new FilteredGroup{string}("AB", "Words beginning with AB",
	///		delegate(string word) { return word.StartsWith("ab", true, null); });
	/// //... up to zz
	/// 
	/// root.ChildGroups.Add(groupWordsBeginningWithA);
	///	root.ChildGroups.Add(groupWordsBeginningWithAB);
	///	root.Add("ant");
	///	root.Add("abdomen");
	///	//...
	///	root.Add("zebra");
	///	root.Add("zoo");
	///
	///	List{string} wordsBeginningWithA = groupWordsBeginningWithA.GetAllItems();
	///	List{string} wordsBeginningWithAB = groupWordsBeginningWithAB.GetAllItems();
	/// </code>
	/// At the end of the example, each call to <see cref="GetAllItems"/> returns the items that matched
	/// that group's filter as well as all it's child filters.  It's easy to see how this class would 
	/// be both useful and efficient if you had to keep a large list of objects organized in memory.  The
	/// performance overhead of inserting the items is high, but once it's done, looking an item up is very efficient.
	/// </example>
	public class FilteredGroup<T> where T : class 
	{
		private FilteredGroup<T> _parentGroup;
		private readonly string _name;
		private readonly string _label;
		private readonly ISpecification _specification;
		private readonly IFilteredGroupFactory<T> _childGroupFactory;
		private readonly ObservableList<T> _items;
		private readonly ReadOnlyCollection<T> _readOnlyItems;
		private readonly FilteredGroupList<T> _childGroups;

		//purposely not ListEventArgs
		private event EventHandler<ItemEventArgs<T>> _itemAdded;
		private event EventHandler<ItemEventArgs<T>> _itemRemoved;


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">A name for the group.</param>
		/// <param name="label">A description for the group.</param>
		/// <param name="test">A delegate that tests for membership of an item in this group.</param>
		public FilteredGroup(string name, string label, Predicate<T> test)
			: this(name, label, test, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">A name for the group.</param>
		/// <param name="label">A description for the group.</param>
		/// <param name="specification">An <see cref="ISpecification"/> that tests for membership of an item in this group.</param>
		public FilteredGroup(string name, string label, ISpecification specification)
			: this(name, label, specification, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">A name for the group.</param>
		/// <param name="label">A description for the group.</param>
		/// <param name="test">A delegate that tests for membership of an item in this group.</param>
		/// <param name="childGroupFactory">A factory that is capable of dynamically creating a new <see cref="FilteredGroup{T}"/>
		/// when an item is added that does not belong to any of the other groups.</param>
		public FilteredGroup(string name, string label, Predicate<T> test, IFilteredGroupFactory<T> childGroupFactory)
			: this(name, label, new SimpleSpecification<T>(test), childGroupFactory)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">A name for the group.</param>
		/// <param name="label">A description for the group.</param>
		/// <param name="specification">An <see cref="ISpecification"/> that tests for membership of an item in this group.</param>
		/// <param name="childGroupFactory">A factory that is capable of dynamically creating a new <see cref="FilteredGroup{T}"/>
		/// when an item is added that does not belong to any of the other groups.</param>
		public FilteredGroup(string name, string label, ISpecification specification, IFilteredGroupFactory<T> childGroupFactory)
		{
			Platform.CheckForNullReference(specification, "specification");

			_name = name;
			_label = label;
			_specification = specification;
			_childGroupFactory = childGroupFactory;

			_childGroups = new FilteredGroupList<T>();
			_items = new ObservableList<T>();
			_readOnlyItems = new ReadOnlyCollection<T>(_items);

			_childGroups.ItemAdded += OnChildGroupAdded;
			_childGroups.ItemChanging += OnChildGroupChanging;
			_childGroups.ItemChanged += OnChildGroupChanged;
			_childGroups.ItemRemoved += OnChildGroupRemoved;

			_items.ItemAdded += OnItemAdded;
			_items.ItemRemoved += OnItemRemoved;
		}

		#region Public Events

		/// <summary>
		/// Occurs when an item is added to the group.
		/// </summary>
		public event EventHandler<ItemEventArgs<T>> ItemAdded
		{
			add { _itemAdded += value; }
			remove { _itemAdded -= value; }
		}

		/// <summary>
		/// Occurs when an item is removed from the group.
		/// </summary>
		public event EventHandler<ItemEventArgs<T>> ItemRemoved
		{
			add { _itemRemoved += value; }
			remove { _itemRemoved -= value; }
		}

		#endregion

		#region Private Methods

		private void OnChildGroupAdded(object sender, ListEventArgs<FilteredGroup<T>> e)
		{
			e.Item.ParentGroup = this;
		}

		private void OnChildGroupChanging(object sender, ListEventArgs<FilteredGroup<T>> e)
		{
			e.Item.ParentGroup = null;
		}

		private void OnChildGroupChanged(object sender, ListEventArgs<FilteredGroup<T>> e)
		{
			e.Item.ParentGroup = this;
		}

		private void OnChildGroupRemoved(object sender, ListEventArgs<FilteredGroup<T>> e)
		{
			e.Item.ParentGroup = null;
		}

		private void OnEmpty()
		{
			if (ParentGroup != null)
				ParentGroup.OnChildGroupEmpty(this);
		}

		private void OnChildGroupEmpty(FilteredGroup<T> childGroup)
		{
			bool remove;
			OnChildGroupEmpty(childGroup, out remove);
			if (remove)
				ChildGroups.Remove(childGroup);
		}

		private void OnItemAdded(object sender, ListEventArgs<T> e)
		{
			OnItemAdded(e.Item);

			if (ParentGroup != null)
				ParentGroup.OnChildItemAdded(e.Item);
		}

		private void OnItemRemoved(object sender, ListEventArgs<T> e)
		{
			OnItemRemoved(e.Item);
			
			if (ParentGroup != null)
				ParentGroup.OnChildItemRemoved(e.Item);
		}

		private void OnChildItemAdded(T item)
		{
			_items.Remove(item);
		}

		private void OnChildItemRemoved(T item)
		{
			bool found = false;
			foreach (T childItem in GetAllChildItems())
			{
				if (childItem.Equals(item))
				{
					found = true;
					break;
				}
			}

			//when it no longer exists in any children, add it back to our list.
			if (!found)
				_items.Add(item);
		}

		/// <summary>
		/// Called when a child group becomes empty.
		/// </summary>
		/// <param name="childGroup">The child group that has just become empty.</param>
		/// <param name="remove">Out parameter indicates whether or not the child group should be removed.  By default,
		/// this value depends on whether there is an <see cref="IFilteredGroupFactory{T}"/> in this node that is capable
		/// of automatically recreating an appropriate child group; if there is such a factory, then the value is true,
		/// otherwise it is false.</param>
		protected virtual void OnChildGroupEmpty(FilteredGroup<T> childGroup, out bool remove)
		{
			//TODO (CR May09): make it so if there is a factory, you can't add your own groups.
			//NOTE: in the interest of time, deferring actually doing this since it involves no API changes.
			remove = _childGroupFactory != null;
		}

		private FilteredGroup<T> CreateNewGroup(T item)
		{
			if (_childGroupFactory != null)
				return _childGroupFactory.Create(item);
			else
				return null;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the parent <see cref="FilteredGroup{T}"/>.
		/// </summary>
		public FilteredGroup<T> ParentGroup
		{
			get { return _parentGroup; }
			private set
			{
				Clear();
				_parentGroup = value;
				Refresh();
			}
		}

		/// <summary>
		/// Gets the name of this node.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets the label/description of this node.
		/// </summary>
		public string Label
		{
			get { return _label; }
		}

		/// <summary>
		/// Gets whether or not this node has any items, not including the ones
		/// that belong to it's child groups.
		/// </summary>
		public bool HasItems
		{
			get { return _items.Count > 0; }	
		}

		/// <summary>
		/// Gets the items that belong only to this group.
		/// </summary>
		/// <remarks>
		/// An item's membership in this <see cref="FilteredGroup{T}"/> node implicitly
		/// means that it is not a match for <b>any</b> of the child groups.  Items can
		/// belong to multiple groups at the same level.
		/// </remarks>
		public ReadOnlyCollection<T> Items
		{
			get { return _readOnlyItems; }	
		}

		/// <summary>
		/// Gets the list of <see cref="FilteredGroupList{T}">child groups</see>.
		/// </summary>
		public FilteredGroupList<T> ChildGroups
		{
			get { return _childGroups; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets all the items that belong to this group directly.
		/// </summary>
		/// <remarks>
		/// An item's membership in this <see cref="FilteredGroup{T}"/> node implicitly
		/// means that it is not a match for <b>any</b> of the child groups.  Items can
		/// belong to multiple groups at the same level.
		/// </remarks>
		public List<T> GetItems()
		{
			return new List<T>(_items);
		}

		/// <summary>
		/// Gets a list of all items belonging to this node and all it's child nodes.
		/// </summary>
		public List<T> GetAllItems()
		{
			List<T> items = new List<T>();
			foreach (T item in Items)
			{
				if (!items.Contains(item))
					items.Add(item);
			}

			foreach (T item in GetAllChildItems())
			{
				if (!items.Contains(item))
					items.Add(item);
			}

			return items;
		}

		/// <summary>
		/// Gets a list of items belonging only to the children of this node, excluding
		/// the items belonging directly to this node.
		/// </summary>
		public List<T> GetAllChildItems()
		{
			List<T> items = new List<T>();
			foreach (FilteredGroup<T> child in ChildGroups)
			{
				foreach (T item in child.GetAllItems())
				{
					if (!items.Contains(item))
						items.Add(item);
				}
			}
			
			return items;
		}

		/// <summary>
		/// Returns <see cref="Label"/>.
		/// </summary>
		public override string ToString()
		{
			return this.Label;
		}

		#endregion

		#region Protected Methods

		#region Overridable

		/// <summary>
		/// Called when an item is added to this node.
		/// </summary>
		protected virtual void OnItemAdded(T item)
		{
			EventsHelper.Fire(_itemAdded, this, new ItemEventArgs<T>(item));
		}

		/// <summary>
		/// Called when an item is removed from this node.
		/// </summary>
		protected virtual void OnItemRemoved(T item)
		{
			EventsHelper.Fire(_itemRemoved, this, new ItemEventArgs<T>(item));
		}

		#endregion

		/// <summary>
		/// Clears items directly belonging to this node, but not ones belonging to child nodes.
		/// </summary>
		protected virtual void Clear()
		{
			foreach (T item in GetItems())
				RemoveItem(item);
		}

		/// <summary>
		/// Refreshes this node by first calling <see cref="Clear"/>, then reconsidering all items
		/// belonging to the parent node for membership in this node.
		/// </summary>
		protected virtual void Refresh()
		{
			Clear();
			if (ParentGroup != null)
				AddItems(ParentGroup.GetItems());
		}

		/// <summary>
		/// Adds multiple items to this node.
		/// </summary>
		protected virtual void AddItems(IEnumerable<T> items)
		{
			foreach (T item in items)
				AddItem(item);
		}

		/// <summary>
		/// Adds a single item to this node.
		/// </summary>
		protected virtual bool AddItem(T item)
		{
			if (!_specification.Test(item).Success)
				return false;

			if (!AddItemToChildren(item))
			{
				if (!_items.Contains(item))
					_items.Add(item);

				return true;
			}

			return false;
		}

		/// <summary>
		/// Tests the item for membership in all child groups and adds it if appropriate.
		/// </summary>
		/// <returns>True if the item was added to at least one child group.</returns>
		protected virtual bool AddItemToChildren(T item)
		{
			if (!_specification.Test(item).Success)
				return false;

			bool addedToChild = false;
			foreach (FilteredGroup<T> childGroup in _childGroups)
			{
				if (childGroup.AddItem(item))
					addedToChild = true;
			}

			if (!addedToChild)
			{
				FilteredGroup<T> newGroup = CreateNewGroup(item);
				if (newGroup != null)
				{
					ChildGroups.Add(newGroup);
					addedToChild = AddItemToChild(item, newGroup);
					Platform.CheckTrue(addedToChild, "Item should be guaranteed to have been inserted.");
				}
			}

			return addedToChild;
		}

		/// <summary>
		/// Tests the item for membership in <paramref name="child"/> and adds it if appropriate.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <param name="child">The child group.</param>
		/// <returns>True if the item was added, otherwise false.</returns>
		protected virtual bool AddItemToChild(T item, FilteredGroup<T> child)
		{
			Platform.CheckTrue(ChildGroups.Contains(child), "Group is not a child of this group.");

			if (!_specification.Test(item).Success)
				return false;

			return child.AddItem(item);
		}

		/// <summary>
		/// Removes the specified item from this group and all child groups.
		/// </summary>
		protected virtual void RemoveItem(T item)
		{
			foreach (FilteredGroup<T> group in new List<FilteredGroup<T>>(ChildGroups))
				group.RemoveItem(item);

			_items.Remove(item);
			if (GetAllItems().Count == 0)
				OnEmpty();
		}

		#endregion
	}
}