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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Extension point for views of the <see cref="GalleryComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public class GalleryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	/// <summary>
	/// A component to show an interactive gallery of items.
	/// </summary>
	[AssociateView(typeof (GalleryComponentViewExtensionPoint))]
	public class GalleryComponent : ApplicationComponent
	{
		private const string _msgNullDataSource = "No data source is specified.";
		private const string _msgItemNotInDataSource = "The item is not in the data source.";
		private const string _msgItemsNotInDataSource = "One or more items are not in the data source.";

		private readonly IToolSet _toolSet;
		private IBindingList _dataSource;
		private ActionModelNode _menuModel;
		private ActionModelNode _toolbarModel;

		private GalleryItemEventHandler _itemActivated;
		private EventHandler _selectionChanged;
		private EventHandler _dataSourceChanged;

		private ISelection _selection = new Selection();
		private ISelection _dataSelection = new Selection();
		private bool _multiSelect = true;
		private bool _showDescription = false;
		private bool _hideSelection = true;
		private Size _imageSize = new Size(100, 100);
		private int _maxDescriptionLines = 0;
		private bool _allowRenaming = false;

		/// <summary>
		/// Constructs an empty <see cref="GalleryComponent"/> without any tool actions.
		/// </summary>
		public GalleryComponent() : this(null, null, null) {}

		/// <summary>
		/// Constructs a <see cref="GalleryComponent"/> with the specified data source and without any tool actions.
		/// </summary>
		/// <param name="dataSource">An <see cref="IBindingList"/> of <see cref="IGalleryItem"/>s.</param>
		public GalleryComponent(IBindingList dataSource) : this(dataSource, null, null) {}

		/// <summary>
		/// Constructs an empty <see cref="GalleryComponent"/>, automatically adding the actions of
		/// <see cref="GalleryToolExtensionPoint"/>s at the specified action sites.
		/// </summary>
		/// <param name="toolbarSite">The site for toolbar actions.</param>
		/// <param name="contextMenuSite">The site for context menu actions.</param>
		public GalleryComponent(string toolbarSite, string contextMenuSite) : this(null, toolbarSite, contextMenuSite) {}

		/// <summary>
		/// Constructs a <see cref="GalleryComponent"/> with the specified data source, automatically adding the actions of
		/// <see cref="GalleryToolExtensionPoint"/>s at the specified action sites.
		/// </summary>
		/// <param name="dataSource">An <see cref="IBindingList"/> of <see cref="IGalleryItem"/>s.</param>
		/// <param name="toolbarSite">The site for toolbar actions.</param>
		/// <param name="contextMenuSite">The site for context menu actions.</param>
		public GalleryComponent(IBindingList dataSource, string toolbarSite, string contextMenuSite)
		{
			_dataSource = dataSource;

			if (toolbarSite != null || contextMenuSite != null)
			{
				GalleryToolExtensionPoint xp = new GalleryToolExtensionPoint();
				ToolContext context = new ToolContext(this);
				ToolSet ts = new ToolSet(xp, context);

				if (contextMenuSite != null)
					_menuModel = ActionModelRoot.CreateModel(typeof (GalleryComponent).FullName, contextMenuSite, ts.Actions);
				if (toolbarSite != null)
					_toolbarModel = ActionModelRoot.CreateModel(typeof (GalleryComponent).FullName, toolbarSite, ts.Actions);

				_toolSet = ts;
			}
		}

		#region Tools, Actions, and Models

		#region Context Class

		private class ToolContext : IGalleryToolContext
		{
			private GalleryComponent _component;
			private event EventHandler _selectionChanged;
			private event GalleryItemEventHandler _itemActivated;

			public ToolContext(GalleryComponent component)
			{
				_component = component;
				_component.SelectionChanged += FireSelectionChanged;
				_component.ItemActivated += FireItemActivated;
			}

			private void FireItemActivated(object sender, GalleryItemEventArgs e)
			{
				if (_itemActivated != null)
					_itemActivated(this, new GalleryItemEventArgs(e.Item));
			}

			private void FireSelectionChanged(object sender, EventArgs e)
			{
				if (_selectionChanged != null)
					_selectionChanged(this, new EventArgs());
			}

			public event EventHandler SelectionChanged
			{
				add { _selectionChanged += value; }
				remove { _selectionChanged -= value; }
			}

			public event GalleryItemEventHandler ItemActivated
			{
				add { _itemActivated += value; }
				remove { _itemActivated -= value; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public IBindingList DataSource
			{
				get { return _component.DataSource; }
			}

			public ISelection Selection
			{
				get { return _component.Selection; }
			}

			public ISelection SelectedData
			{
				get { return _component.SelectedData; }
			}

			public void Activate(IGalleryItem item)
			{
				_component.Activate(item);
			}

			public void Select(IEnumerable<IGalleryItem> selection)
			{
				_component.Select(selection);
			}

			public void Select(IGalleryItem item)
			{
				_component.Select(item);
			}
		}

		#endregion

		/// <summary>
		/// Gets a <see cref="IToolSet"/> of <see cref="GalleryToolExtensionPoint"/> tools.
		/// </summary>
		protected IToolSet ToolSet
		{
			get { return _toolSet; }
		}

		/// <summary>
		/// Gets or sets the context menu action model.
		/// </summary>
		/// <remarks>
		/// The action model must be set before the view is created. Any changes to the action model are not propagated to the view afterwards.
		/// </remarks>
		public ActionModelNode MenuModel
		{
			get { return _menuModel; }
			set { _menuModel = value; }
		}

		/// <summary>
		/// Gets or sets the toolbar action model.
		/// </summary>
		/// <remarks>
		/// The action model must be set before the view is created. Any changes to the action model are not propagated to the view afterwards.
		/// </remarks>
		public ActionModelNode ToolbarModel
		{
			get { return _toolbarModel; }
			set { _toolbarModel = value; }
		}

		#endregion

		#region Data Source

		/// <summary>
		/// Gets or sets the underlying <see cref="IBindingList"/> of <see cref="IGalleryItem"/>s.
		/// </summary>
		public IBindingList DataSource
		{
			get { return _dataSource; }
			set
			{
				if (_dataSource != value)
				{
					_dataSource = value;
					EventsHelper.Fire(_dataSourceChanged, this, new EventArgs());
					base.Modified = true;
					base.NotifyPropertyChanged("DataSource");
				}
			}
		}

		/// <summary>
		/// Indicates that the underlying <see cref="IBindingList"/> of <see cref="IGalleryItem"/>s has changed.
		/// </summary>
		public event EventHandler DataSourceChanged
		{
			add { _dataSourceChanged += value; }
			remove { _dataSourceChanged -= value; }
		}

		#endregion

		#region Selection

		/// <summary>
		/// Gets or sets if this <see cref="GalleryComponent"/> supports selection of multiple <see cref="IGalleryItem"/>s simultaneously.
		/// </summary>
		public virtual bool MultiSelect
		{
			get { return _multiSelect; }
			set { _multiSelect = value; }
		}

		/// <summary>
		/// Gets the current selection of <see cref="IGalleryItem"/>s.
		/// </summary>
		public ISelection Selection
		{
			get { return _selection; }
		}

		/// <summary>
		/// Gets the data objects of the current selection of <see cref="IGalleryItem"/>s.
		/// </summary>
		public ISelection SelectedData
		{
			get { return _dataSelection; }
		}

		/// <summary>
		/// Indicates that the current selection of <see cref="IGalleryItem"/>s in the gallery has changed.
		/// </summary>
		public event EventHandler SelectionChanged
		{
			add { _selectionChanged += value; }
			remove { _selectionChanged -= value; }
		}

		/// <summary>
		/// Indicates that an <see cref="IGalleryItem"/> in the gallery has been activated
		/// </summary>
		public event GalleryItemEventHandler ItemActivated
		{
			add { _itemActivated += value; }
			remove { _itemActivated -= value; }
		}

		/// <summary>
		/// Unselects all <see cref="IGalleryItem"/>s.
		/// </summary>
		public void UnselectAll()
		{
			Select(new List<IGalleryItem>());
		}

		/// <summary>
		/// Selects the specified <see cref="IGalleryItem"/>s.
		/// </summary>
		/// <remarks>
		/// Unselection of all items can be accomplished by passing an empty enumeration to <see cref="Select(IEnumerable{IGalleryItem})"/>.
		/// </remarks>
		/// <param name="selection">The items to select.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="selection"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if the selected items are not in the <see cref="DataSource"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if <see cref="DataSource"/> is null.</exception>
		public void Select(IEnumerable<IGalleryItem> selection)
		{
			Platform.CheckForNullReference(selection, "selection");
			if (_dataSource == null)
				throw new InvalidOperationException(_msgNullDataSource);

			List<IGalleryItem> list = new List<IGalleryItem>();
			List<object> data = new List<object>();
			foreach (IGalleryItem item in selection)
			{
				if (!_dataSource.Contains(item))
					continue; // throw new ArgumentException(_msgItemsNotInDataSource, "selection");
				list.Add(item);
				data.Add(item.Item);
			}
			_selection = new Selection(list);
			_dataSelection = new Selection(data);
			NotifySelectionChanged();
		}

		/// <summary>
		/// Selects the specified <see cref="IGalleryItem"/>.
		/// </summary>
		/// <remarks>
		/// Unselection of all items can be accomplished by passing an empty enumeration to <see cref="Select(IEnumerable{IGalleryItem})"/>.
		/// </remarks>
		/// <param name="item">The item to select.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="item"/> is not in the <see cref="DataSource"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if <see cref="DataSource"/> is null.</exception>
		public void Select(IGalleryItem item)
		{
			Platform.CheckForNullReference(item, "item");
			if (_dataSource == null)
				throw new InvalidOperationException(_msgNullDataSource);
			if (!_dataSource.Contains(item))
				return; // throw new ArgumentException(_msgItemNotInDataSource, "item");

			_selection = new Selection(item);
			_dataSelection = new Selection(item.Item);
			NotifySelectionChanged();
		}

		/// <summary>
		/// Activates the specified <see cref="IGalleryItem"/>.
		/// </summary>
		/// <param name="item">The item to activate.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="item"/> is not in the <see cref="DataSource"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if <see cref="DataSource"/> is null.</exception>
		public void Activate(IGalleryItem item)
		{
			Platform.CheckForNullReference(item, "item");
			if (_dataSource == null)
				throw new InvalidOperationException(_msgNullDataSource);
			if (!_dataSource.Contains(item))
				throw new ArgumentException(_msgItemNotInDataSource, "item");

			NotifyItemActivated(item);
		}

		private void NotifySelectionChanged()
		{
			EventsHelper.Fire(_selectionChanged, this, new EventArgs());
		}

		private void NotifyItemActivated(IGalleryItem item)
		{
			EventsHelper.Fire(_itemActivated, this, new GalleryItemEventArgs(item));
		}

		#endregion

		#region Display Options

		/// <summary>
		/// Gets or sets if the gallery should show selection accents when the control is not in focus.
		/// </summary>
		public virtual bool HideSelection
		{
			get { return _hideSelection; }
			set { _hideSelection = value; }
		}

		/// <summary>
		/// Gets or sets if the gallery should show the <see cref="IGalleryItem.Description"/> of each <see cref="IGalleryItem"/>.
		/// </summary>
		public virtual bool ShowDescription
		{
			get { return _showDescription; }
			set { _showDescription = value; }
		}

		/// <summary>
		/// Gets or sets the maximum lines of <see cref="IGalleryItem.Description"/> that should be displayed.
		/// </summary>
		public virtual int MaxDescriptionLines
		{
			get { return _maxDescriptionLines; }
			set { _maxDescriptionLines = value; }
		}

		/// <summary>
		/// Gets or sets the size of the icons that the gallery should show.
		/// </summary>
		public virtual Size ImageSize
		{
			get { return _imageSize; }
			set { _imageSize = value; }
		}

		/// <summary>
		/// Gets or sets if the gallery should allow renaming of <see cref="IGalleryItem"/>s.
		/// </summary>
		public virtual bool AllowRenaming
		{
			get { return _allowRenaming; }
			set { _allowRenaming = value; }
		}

		#endregion

		#region Drag Drop Functionality

		/// <summary>
		/// Gets if the gallery supports any drag and drop interaction on top of items.
		/// </summary>
		public virtual bool AllowsDropOnItem
		{
			get { return false; }
		}

		/// <summary>
		/// Gets if the gallery supports any drag and drop interaction in between items.
		/// </summary>
		public virtual bool AllowsDropAtIndex
		{
			get { return false; }
		}

		/// <summary>
		/// Signals the component that a drag &amp; drop operation involving the specified
		/// <see cref="IGalleryItem"/>s has started on the associated view.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="draggedItems">The <see cref="IGalleryItem"/>s being dragged.</param>
		public virtual DragDropOption BeginDrag(IList<IGalleryItem> draggedItems)
		{
			return DragDropOption.Copy;
		}

		/// <summary>
		/// Signals the component that a drag &amp; drop operation involving the specified
		/// <see cref="IGalleryItem"/>s has ended with the given action being taken on the items by the drop target.
		/// </summary>
		/// <param name="draggedItems">The <see cref="IGalleryItem"/>s that were dragged.</param>
		/// <param name="action">The <see cref="DragDropOption"/> action that was taken on the items by the drop target.</param>
		public virtual void EndDrag(IList<IGalleryItem> draggedItems, DragDropOption action) {}

		/// <summary>
		/// Checks for allowed drag &amp; drop actions involving the specified foreign data and the given target on this component.
		/// </summary>
		/// <param name="droppingData">The <see cref="IDragDropObject"/> object that encapsulates all forms of the foreign data.</param>
		/// <param name="targetIndex">The target index that the user is trying to drop at.</param>
		/// <param name="actions"></param>
		/// <param name="modifiers">The modifier keys that are being held by the user.</param>
		/// <returns>The allowed <see cref="DragDropKind"/> actions for this attempted drag &amp; drop operation.</returns>
		public virtual DragDropOption CheckDrop(IDragDropObject droppingData, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		/// <summary>
		/// Checks for allowed drag &amp; drop actions involving the specified foreign data and the given target on this component.
		/// </summary>
		/// <param name="droppingData">The <see cref="IDragDropObject"/> object that encapsulates all forms of the foreign data.</param>
		/// <param name="targetItem">The target item that the user is trying to drop on to.</param>
		/// <param name="actions"></param>
		/// <param name="modifiers">The modifier keys that are being held by the user.</param>
		/// <returns>The allowed <see cref="DragDropKind"/> action for this attempted drag &amp; drop operation.</returns>
		public virtual DragDropOption CheckDrop(IDragDropObject droppingData, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// This method or <see cref="PerformDrop(IDragDropObject,IGalleryItem,DragDropOption,ModifierFlags)"/> may be called
		/// additional times if the returned action is <see cref="DragDropOption.None"/> in order to attempt other ways to drop the item in
		/// an acceptable manner. It is thus very important that the result be set properly if the drop was accepted and no further attempts
		/// should be made.
		/// </remarks>
		/// <param name="droppedData"></param>
		/// <param name="targetIndex"></param>
		/// <param name="action"></param>
		/// <param name="modifiers"></param>
		/// <returns></returns>
		public virtual DragDropOption PerformDrop(IDragDropObject droppedData, int targetIndex, DragDropOption action, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="droppedData"></param>
		/// <param name="targetItem"></param>
		/// <param name="action"></param>
		/// <param name="modifiers"></param>
		/// <returns></returns>
		public virtual DragDropOption PerformDrop(IDragDropObject droppedData, IGalleryItem targetItem, DragDropOption action, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		#endregion
	}
}