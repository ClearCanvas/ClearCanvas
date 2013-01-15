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

using ClearCanvas.Common.Utilities;
using System;

namespace ClearCanvas.Desktop.Trees
{
	/// <summary>
	/// A delegate that allows setting whether or not <paramref name="item"/> is checked in a tree.
	/// </summary>
	public delegate void SetterDelegate<T, TValue>(T item, TValue value);
	/// <summary>
	/// A delegate that determines whether or not <paramref name="item"/> can accept a dropped item.
	/// </summary>
	public delegate DragDropKind CanAcceptDropDelegate<T>(T item, object dropData, DragDropKind kind);
	/// <summary>
	/// A delegate that finalizes a drop operation.
	/// </summary>
	public delegate DragDropKind AcceptDropDelegate<T>(T item, object dropData, DragDropKind kind);

    /// <summary>
    /// A useful generic implementation of <see cref="ITreeItemBinding"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of item being bound to a tree.</typeparam>
    public class TreeItemBinding<TItem> : TreeItemBindingBase
    {
        private Converter<TItem, string> _nodeTextProvider;
    	private SetterDelegate<TItem, string> _nodeTextSetter;
		private Converter<TItem, bool> _canSetNodeTextHandler;
        private Converter<TItem, bool> _isCheckedGetter;
        private SetterDelegate<TItem, bool> _isCheckedSetter;
        private Converter<TItem, string> _tooltipTextProvider;
        private Converter<TItem, IconSet> _iconSetIndexProvider;
        private Converter<TItem, IResourceResolver> _resourceResolverProvider;
        private Converter<TItem, bool> _canHaveSubTreeHandler;
        private Converter<TItem, bool> _isExpandedGetter;
    	private SetterDelegate<TItem, bool> _isExpandedSetter;
        private Converter<TItem, ITree> _subTreeProvider;
        private CanAcceptDropDelegate<TItem> _canAcceptDropHandler;
        private AcceptDropDelegate<TItem> _acceptDropHandler;
    	private Converter<TItem, bool> _isHighlightedProvider;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nodeTextProvider">A delegate providing text for the node in the tree.</param>
        /// <param name="subTreeProvider">A delegate providing the sub-tree for a node in the tree.</param>
        public TreeItemBinding(Converter<TItem, string> nodeTextProvider, Converter<TItem, ITree> subTreeProvider)
        {
            _nodeTextProvider = nodeTextProvider;
            _subTreeProvider = subTreeProvider;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="nodeTextProvider">A delegate providing text for the node in the tree.</param>
		public TreeItemBinding(Converter<TItem, string> nodeTextProvider)
            : this(nodeTextProvider, null)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TreeItemBinding()
            :this(null, null)
        {
        }

        /// <summary>
        /// Gets or sets the node text provider for this binding.
        /// </summary>
		public Converter<TItem, string> NodeTextProvider
        {
            get { return _nodeTextProvider; }
            set { _nodeTextProvider = value; }
        }

		/// <summary>
		/// Gets or sets the node text setter for this binding.
		/// </summary>
		public SetterDelegate<TItem, string> NodeTextSetter
		{
			get { return _nodeTextSetter; }
			set { _nodeTextSetter = value; }
		}
		
		/// <summary>
		/// Gets or sets the handler that determines whether or not the text of this item can change.
		/// </summary>
    	public Converter<TItem, bool> CanSetNodeTextHandler
    	{
			get { return _canSetNodeTextHandler; }
			set { _canSetNodeTextHandler = value; }
    	}

        /// <summary>
        /// Gets or sets the node checked status provider for this binding.
        /// </summary>
		public Converter<TItem, bool> IsCheckedGetter
        {
            get { return _isCheckedGetter; }
            set { _isCheckedGetter = value; }
        }

		/// <summary>
		/// Gets or sets the node checked setter for this binding.
		/// </summary>
		public SetterDelegate<TItem, bool> IsCheckedSetter
        {
            get { return _isCheckedSetter; }
            set { _isCheckedSetter = value; }
        }

        /// <summary>
        /// Gets or sets the tooltip text provider for this binding.
        /// </summary>
		public Converter<TItem, string> TooltipTextProvider
        {
            get { return _tooltipTextProvider; }
            set { _tooltipTextProvider = value; }
        }

        /// <summary>
        /// Gets or sets the iconset provider for this binding.
        /// </summary>
		public Converter<TItem, IconSet> IconSetProvider
        {
            get { return _iconSetIndexProvider; }
            set { _iconSetIndexProvider = value; }
        }

        /// <summary>
        /// Gets or sets the resource resolver provider for this binding.
        /// </summary>
		public Converter<TItem, IResourceResolver> ResourceResolverProvider
        {
            get { return _resourceResolverProvider; }
            set { _resourceResolverProvider = value; }
        }

		/// <summary>
		/// Gets or sets the handler that determines whether or not this item can have a sub-tree.
		/// </summary>
		public Converter<TItem, bool> CanHaveSubTreeHandler
        {
            get { return _canHaveSubTreeHandler; }
            set { _canHaveSubTreeHandler = value; }
        }

        /// <summary>
        /// Gets or sets the subtree expansion state getter for this binding.
        /// </summary>
		public Converter<TItem, bool> IsExpandedGetter
        {
            get { return _isExpandedGetter; }
            set { _isExpandedGetter = value; }
        }

		/// <summary>
		/// Gets or sets the subtree expansion state getter for this binding.
		/// </summary>
		public SetterDelegate<TItem, bool> IsExpandedSetter
		{
			get { return _isExpandedSetter; }
			set { _isExpandedSetter = value; }
		}

        /// <summary>
        /// Gets or sets the subtree provider for this binding.
        /// </summary>
		public Converter<TItem, ITree> SubTreeProvider
        {
            get { return _subTreeProvider; }
            set { _subTreeProvider = value; }
        }

		/// <summary>
		/// Gets or sets the handler that decides whether or not an item can be dropped on this node in the tree.
		/// </summary>
        public CanAcceptDropDelegate<TItem> CanAcceptDropHandler
        {
            get { return _canAcceptDropHandler; }
            set { _canAcceptDropHandler = value; }
        }

		/// <summary>
		/// Gets or sets the handler that accepts dropped items onto this node in the tree.
		/// </summary>
		public AcceptDropDelegate<TItem> AcceptDropHandler
        {
            get { return _acceptDropHandler; }
            set { _acceptDropHandler = value; }
        }

		/// <summary>
		/// Gets or sets the handler that indicate whether the node should be highlighted in the tree.
		/// </summary>
    	public Converter<TItem, bool> IsHighlightedProvider
    	{
    		get { return _isHighlightedProvider; }
			set { _isHighlightedProvider = value; }
    	}

		/// <summary>
		/// Gets whether the specified item should be highlighted.
		/// </summary>
		public override bool GetIsHighlighted(object item)
		{
			return _isHighlightedProvider == null ? false : _isHighlightedProvider((TItem)item);
		}

    	///<summary>
    	/// Gets the text to display for the node representing the specified item.
    	///</summary>
    	public override string GetNodeText(object item)
        {
            return _nodeTextProvider((TItem)item);
        }

		/// <summary>
		/// Sets the text to display for the node representing the specified item.
		/// </summary>
		public override void SetNodeText(object item, string text)
		{
			if (_nodeTextSetter != null && CanSetNodeText(item))
			{
				_nodeTextSetter((TItem) item, text);
			}
		}

		/// <summary>
		/// Asks if the item text can be changed.
		/// </summary>
		public override bool CanSetNodeText(object item)
		{
			return _canSetNodeTextHandler == null 
				? _nodeTextSetter != null
				: _canSetNodeTextHandler((TItem)item);
		}

    	///<summary>
    	/// Gets whether or not <paramref name="item" /> is checked.
    	///</summary>
    	public override bool GetIsChecked(object item)
        {
            return _isCheckedGetter == null ? base.GetIsChecked(((TItem) item)) : _isCheckedGetter((TItem) item);
        }

    	///<summary>
    	/// Sets whether or not <paramref name="item" /> is checked.
    	///</summary>
    	public override void SetIsChecked(object item, bool value)
        {
            if(_isCheckedSetter != null)
            {
                _isCheckedSetter((TItem) item, value);
            }
        }

    	///<summary>
    	/// Asks if the item can have a subtree.
    	///</summary>
    	///<remarks>
    	/// Note that this method should return true to inidicate that it
    	/// is possible that the item might have a subtree.  This allows the view to determine whether to display
    	/// a "plus" sign next to the node, without having to actually call <see cref="M:ClearCanvas.Desktop.Trees.ITreeItemBinding.GetSubTree(System.Object)" />.
    	///</remarks>
    	public override bool CanHaveSubTree(object item)
        {
            return _canHaveSubTreeHandler == null ? base.CanHaveSubTree(item) : _canHaveSubTreeHandler((TItem)item);
        }

    	///<summary>
    	/// Gets the <see cref="T:ClearCanvas.Desktop.Trees.ITree" /> that represents the subtree for the specified item,
    	/// or null if the item does not have a subtree.
    	///</summary>
    	///<remarks>
    	/// Note that <see cref="M:ClearCanvas.Desktop.Trees.ITreeItemBinding.CanHaveSubTree(System.Object)" /> is called first,
    	/// and this method will be called only if <see cref="M:ClearCanvas.Desktop.Trees.ITreeItemBinding.CanHaveSubTree(System.Object)" /> returns true.
    	///</remarks>
    	public override ITree GetSubTree(object item)
        {
            return _subTreeProvider == null ? base.GetSubTree(item) : _subTreeProvider((TItem)item);
        }

    	///<summary>
    	/// Gets a value indicating if the item should be expanded when the tree is initially loaded.
    	///</summary>
    	public override bool GetExpanded(object item)
        {
             return _isExpandedGetter == null ? base.GetExpanded(item) : _isExpandedGetter((TItem)item);
        }

    	/// <summary>
    	/// Sets a value indicating whether the specified item is currently expanded.
    	/// </summary>
    	/// <param name="item"></param>
    	/// <param name="expanded"></param>
    	/// <returns></returns>
    	public override void SetExpanded(object item, bool expanded)
		{
			if (_isExpandedSetter != null)
				_isExpandedSetter((TItem)item, expanded);
		}

    	///<summary>
    	/// Gets the tooltip to display for the specified item.
    	///</summary>
    	public override string GetTooltipText(object item)
        {
            return _tooltipTextProvider == null ? base.GetTooltipText(item) : _tooltipTextProvider((TItem)item);
        }

    	///<summary>
    	/// Gets the image iconset to display for the specified item.
    	///</summary>
    	public override IconSet GetIconSet(object item)
        {
            return _iconSetIndexProvider == null ? base.GetIconSet(item) : _iconSetIndexProvider((TItem)item);
        }

    	///<summary>
    	/// Gets the resource resolver used to resolve the icon(s).
    	///</summary>
    	public override IResourceResolver GetResourceResolver(object item)
        {
            return _resourceResolverProvider == null ? base.GetResourceResolver(item) : _resourceResolverProvider((TItem)item);
        }

    	///<summary>
    	/// Asks the specified item if it can accept the specified drop data in a drag-drop operation.
    	///</summary>
    	///<param name="item">The item being drag-dropped.</param>
    	///<param name="dropData">Information about the item drag-dropped.</param>
    	///<param name="kind">The drop kind being performed.</param>
    	///<returns>
    	///The drop kind that will be accepted.
    	///</returns>
    	public override DragDropKind CanAcceptDrop(object item, object dropData, DragDropKind kind)
        {
            return _canAcceptDropHandler == null ? base.CanAcceptDrop(item, dropData, kind) : _canAcceptDropHandler((TItem)item, dropData, kind);
        }

    	///<summary>
    	/// Informs the specified item that it should accept a drop of the specified data, completing a drag-drop operation.
    	///</summary>
    	///<param name="item">The item being drag-dropped.</param>
    	///<param name="dropData">Information about the item being drag-dropped.</param>
    	///<param name="kind">The drop kind being performed.</param>
    	///<returns>
    	///The drop kind that will be accepted.
    	///</returns>
    	public override DragDropKind AcceptDrop(object item, object dropData, DragDropKind kind)
        {
            return _acceptDropHandler == null ? base.AcceptDrop(item, dropData, kind) : _acceptDropHandler((TItem)item, dropData, kind);
        }
    }
}
