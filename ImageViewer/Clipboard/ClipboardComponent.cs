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
using System.ComponentModel;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard
{
	[ExtensionPoint()]
	public sealed class ClipboardToolExtensionPoint : ExtensionPoint<ITool> {}

	/// <summary>
	/// Extension point for views onto <see cref="ClipboardComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ClipboardComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	/// <summary>
	/// ClipboardComponent class.
	/// </summary>
	[AssociateView(typeof (ClipboardComponentViewExtensionPoint))]
	public class ClipboardComponent : ApplicationComponent
	{
		#region Component

		#region ClipboardToolContext class

		protected class ClipboardToolContext : ToolContext, IClipboardToolContext
		{
			private readonly ClipboardComponent _component;

			public ClipboardToolContext(ClipboardComponent component)
			{
				Platform.CheckForNullReference(component, "component");
				_component = component;
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public ClipboardComponent Component
			{
				get { return _component; }
			}

			public IList<IClipboardItem> ClipboardItems
			{
				get { return _component._items; }
			}

			public ReadOnlyCollection<IClipboardItem> SelectedClipboardItems
			{
				get { return _component.SelectedItems; }
			}

			public event EventHandler ClipboardItemsChanged
			{
				add { _component._itemsChanged += value; }
				remove { _component._itemsChanged -= value; }
			}

			public event EventHandler SelectedClipboardItemsChanged
			{
				add { _component._selectedItemsChanged += value; }
				remove { _component._selectedItemsChanged -= value; }
			}
		}

		#endregion

		#region Private Fields

		private readonly string _toolbarSite;
		private readonly string _menuSite;
		private IToolSet _toolSet;
		private MenuAction _deleteAllMenuAction;
		private MenuAction _deleteMenuAction;
		private ButtonAction _deleteAllButtonAction;
		private ButtonAction _deleteButtonAction;
		private IResourceResolver _resolver;
		private ActionModelRoot _toolbarModel;
		private ActionModelRoot _contextMenuModel;

		private readonly bool _disposeItemsOnClose = true;
		private ClipboardItemList _items;
		private event EventHandler _dataSourceChanged;

		private ISelection _selection;
		private event EventHandler _selectedItemsChanged;
		private event EventHandler _itemsChanged;

		private Clipboard _clipboard;
		private event EventHandler _clipboardChanged;

		#endregion

		public ClipboardComponent(string toolbarSite, string menuSite, bool disposeItemsOnClose = true)
			: this(toolbarSite, menuSite, null, disposeItemsOnClose) {}

		public ClipboardComponent(string toolbarSite, string menuSite, Clipboard clipboard, bool disposeItemsOnClose)
		{
			Platform.CheckForEmptyString(toolbarSite, "toolbarSite");
			Platform.CheckForEmptyString(menuSite, "menuSite");

			_toolbarSite = toolbarSite;
			_menuSite = menuSite;
			_clipboard = clipboard;
			_items = new ClipboardItemList(clipboard != null ? clipboard.Items : new BindingList<IClipboardItem>());
			_items.BindingList.ListChanged += OnBindingListChanged;
			_disposeItemsOnClose = disposeItemsOnClose;
		}

		#region Presentation Model

		public Clipboard Clipboard
		{
			get { return _clipboard; }
			set
			{
				if (!ReferenceEquals(_clipboard, value))
				{
					_clipboard = value;

					DataSource = _clipboard != null ? _clipboard.Items : new BindingList<IClipboardItem>();

					EventsHelper.Fire(_clipboardChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler ClipboardChanged
		{
			add { _clipboardChanged += value; }
			remove { _clipboardChanged -= value; }
		}

		public BindingList<IClipboardItem> DataSource
		{
			get { return _items.BindingList; }
			private set
			{
				Platform.CheckForNullReference(value, "value");

				CheckForLockedItems();
				if (_items != null)
					_items.BindingList.ListChanged -= OnBindingListChanged;

				_items = new ClipboardItemList(value);
				_items.BindingList.ListChanged += OnBindingListChanged;

				EventsHelper.Fire(_dataSourceChanged, this, EventArgs.Empty);

				SetSelection(new Selection());
			}
		}

		public ReadOnlyCollection<IClipboardItem> SelectedItems
		{
			get
			{
				List<IClipboardItem> selectedItems = new List<IClipboardItem>();

				if (_selection != null)
				{
					foreach (IClipboardItem item in _selection.Items)
						selectedItems.Add(item);
				}

				return selectedItems.AsReadOnly();
			}
		}

		public ActionModelRoot ToolbarModel
		{
			get { return _toolbarModel; }
		}

		public ActionModelRoot ContextMenuModel
		{
			get { return _contextMenuModel; }
		}

		public void SetSelection(ISelection selection)
		{
			if (_selection != selection)
			{
				_selection = selection;
				OnSelectionChanged();
			}
		}

		public event EventHandler DataSourceChanged
		{
			add { _dataSourceChanged += value; }
			remove { _dataSourceChanged -= value; }
		}

		#endregion

		private bool DeleteAllEnabled
		{
			get { return _items.Count > 0; }
		}

		private bool DeleteEnabled
		{
			get { return SelectedItems.Count > 0; }
		}

		protected virtual string ToolActionsNamespace
		{
			get { return typeof (ClipboardComponent).FullName; }
		}

		protected virtual IActionSet CreateToolActions()
		{
			if (_toolSet == null)
				_toolSet = new ToolSet(new ClipboardToolExtensionPoint(), new ClipboardToolContext(this));
			return new ActionSet(_toolSet.Actions);
		}

		#region Overrides

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			base.Start();

			_resolver = new ApplicationThemeResourceResolver(GetType(), true);
			ActionSet deleteToolActions = new ActionSet(GetDeleteActions());
			var toolActions = CreateToolActions();
			IActionSet allActions = toolActions != null ? toolActions.Union(deleteToolActions) : deleteToolActions;

			_toolbarModel = ActionModelRoot.CreateModel(ToolActionsNamespace, _toolbarSite, allActions);
			_contextMenuModel = ActionModelRoot.CreateModel(ToolActionsNamespace, _menuSite, allActions);

			_items.BindingList.ListChanged += OnBindingListChanged;
		}

		/// <summary>
		/// Called by the host when the application component is being terminated.
		/// </summary>
		public override void Stop()
		{
			_items.BindingList.ListChanged -= OnBindingListChanged;

			if (_disposeItemsOnClose)
			{
				foreach (IClipboardItem item in _items)
				{
					if (item is IDisposable)
						((IDisposable) item).Dispose();
				}
			}

			if (_toolSet != null)
			{
				_toolSet.Dispose();
				_toolSet = null;
			}

			base.Stop();
		}

		#endregion

		#region Delete Actions

		private IEnumerable<IAction> GetDeleteActions()
		{
			CreateDeleteActions();
			UpdateDeleteActionEnablement();

			yield return _deleteAllButtonAction;
			yield return _deleteAllMenuAction;
			yield return _deleteButtonAction;
			yield return _deleteMenuAction;
		}

		private void CreateDeleteActions()
		{
			_deleteAllMenuAction = CreateMenuAction("deleteAll", String.Format("{0}/MenuDeleteAllClipboardItems", _menuSite), SR.TooltipDeleteAllClipboardItems, CreateDeleteAllIconSet(), DeleteAll);
			_deleteMenuAction = CreateMenuAction("delete", String.Format("{0}/MenuDeleteClipboardItem", _menuSite), SR.TooltipDeleteClipboardItem, CreateDeleteIconSet(), DeleteSelected);
			_deleteAllButtonAction = CreateToolbarAction("deleteAll", String.Format("{0}/ToolbarDeleteAllClipboardItems", _toolbarSite), SR.TooltipDeleteAllClipboardItems, CreateDeleteAllIconSet(), DeleteAll);
			_deleteButtonAction = CreateToolbarAction("delete", String.Format("{0}/ToolbarDeleteClipboardItem", _toolbarSite), SR.TooltipDeleteClipboardItem, CreateDeleteIconSet(), DeleteSelected);
		}

		private MenuAction CreateMenuAction(string id, string path, string tooltip, IconSet iconSet, ClickHandlerDelegate clickHandler)
		{
			id = String.Format("{0}:{1}", typeof (ClipboardComponent).FullName, id);
			MenuAction action = new MenuAction(id, new ActionPath(path, _resolver), ClickActionFlags.None, _resolver);
			action.IconSet = iconSet;
			action.Tooltip = tooltip;
			action.Label = action.Path.LastSegment.LocalizedText;
			action.SetClickHandler(clickHandler);
			return action;
		}

		private ButtonAction CreateToolbarAction(string id, string path, string tooltip, IconSet iconSet, ClickHandlerDelegate clickHandler)
		{
			id = String.Format("{0}:{1}", typeof (ClipboardComponent).FullName, id);
			ButtonAction action = new ButtonAction(id, new ActionPath(path, _resolver), ClickActionFlags.None, _resolver);
			action.IconSet = iconSet;
			action.Tooltip = tooltip;
			action.Label = action.Path.LastSegment.LocalizedText;
			action.SetClickHandler(clickHandler);
			return action;
		}

		private static IconSet CreateDeleteAllIconSet()
		{
			return new IconSet("Icons.DeleteAllClipboardItemsToolSmall.png",
			                   "Icons.DeleteAllClipboardItemsToolSmall.png", "Icons.DeleteClipboardItemToolSmall.png");
		}

		private static IconSet CreateDeleteIconSet()
		{
			return new IconSet("Icons.DeleteClipboardItemToolSmall.png",
			                   "Icons.DeleteClipboardItemToolSmall.png", "Icons.DeleteClipboardItemToolSmall.png");
		}

		private void UpdateDeleteActionEnablement()
		{
			_deleteAllButtonAction.Enabled = DeleteAllEnabled;
			_deleteAllMenuAction.Enabled = DeleteAllEnabled;
			_deleteButtonAction.Enabled = DeleteEnabled;
			_deleteMenuAction.Enabled = DeleteEnabled;
		}

		#endregion

		private void OnBindingListChanged(object sender, ListChangedEventArgs e)
		{
			OnItemsChanged();
		}

		private void CheckForLockedItems()
		{
			if (_items.Any(item => item.IsLocked))
				throw new InvalidOperationException("At least one item is currently locked.");
		}

		#region Protected Methods

		protected virtual void OnItemsChanged()
		{
			UpdateDeleteActionEnablement();
			EventsHelper.Fire(_itemsChanged, this, EventArgs.Empty);
		}

		protected virtual void OnSelectionChanged()
		{
			UpdateDeleteActionEnablement();
			EventsHelper.Fire(_selectedItemsChanged, this, EventArgs.Empty);
		}

		#endregion

		#region Public Methods

		public void DeleteAll()
		{
			bool anyLocked = false;

			List<IClipboardItem> items = new List<IClipboardItem>(_items);
			foreach (ClipboardItem item in items)
			{
				if (item.IsLocked)
				{
					anyLocked = true;
				}
				else
				{
					((IDisposable) item).Dispose();
					_items.Remove(item);
				}
			}

			if (anyLocked)
				this.Host.DesktopWindow.ShowMessageBox(SR.MessageUnableToClearClipboardItems, MessageBoxActions.Ok);
		}

		public void DeleteSelected()
		{
			bool anyLocked = false;

			//Shouldn't be possible for selected items to not be in the current list, but check anyway,
			//because the user's selections can come from the Webstation, where the UI changes are not going
			//to be totally in sync with the "current" state on the server.
			foreach (ClipboardItem item in this.SelectedItems.Where(s => _items.Contains(s)))
			{
				if (item.IsLocked)
				{
					anyLocked = true;
				}
				else
				{
					((IDisposable) item).Dispose();
					_items.Remove(item);
				}
			}

			if (anyLocked)
				this.Host.DesktopWindow.ShowMessageBox(SR.MessageUnableToClearClipboardItems, MessageBoxActions.Ok);
		}

		public virtual void AddPresentationImage(IPresentationImage presentationImage)
		{
			_items.Add(_clipboard.CreatePresentationImageItem(presentationImage));
		}

		public virtual void AddDisplaySet(DisplaySet displaySet)
		{
			_items.Add(_clipboard.CreateDisplaySetItem(displaySet));
		}

		public virtual void AddDisplaySet(DisplaySet displaySet, IImageSelectionStrategy selectionStrategy)
		{
			_items.Add(_clipboard.CreateDisplaySetItem(displaySet, selectionStrategy));
		}

		#endregion

		#endregion
	}
}