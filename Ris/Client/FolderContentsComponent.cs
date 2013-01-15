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
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="FolderContentsComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class FolderContentsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// WorklistExplorerComponent class
	/// </summary>
	[AssociateView(typeof(FolderContentsComponentViewExtensionPoint))]
	public class FolderContentsComponent : ApplicationComponent
	{
		private class FolderContentsPagingModel : SimpleActionModel
		{
			private readonly FolderContentsComponent _owner;

			public FolderContentsPagingModel(FolderContentsComponent owner, IResourceResolver resolver)
				: base(resolver)
			{
				_owner = owner;

				AddAction("Previous", Desktop.SR.TitlePrevious, "Icons.PreviousPageToolSmall.png");
				AddAction("Next", Desktop.SR.TitleNext, "Icons.NextPageToolSmall.png");

				// Set the initial visible and enable state to false
				this.Previous.Visible = false;
				this.Previous.Enabled = false;
				this.Next.Visible = false;
				this.Next.Enabled = false;

				this.Previous.SetClickHandler(OnPrevious);
				this.Next.SetClickHandler(OnNext);

				_owner.PropertyChanged += OnPropertyChanged;
			}

			private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName != "StatusMessage")
					return;

				// Update page tools whenever the status is updated.
				if (_owner.SelectedFolder == null || !_owner.SelectedFolder.SupportsPaging)
				{
					this.Previous.Visible = false;
					this.Next.Visible = false;
				}
				else
				{
					this.Previous.Visible = true;
					this.Next.Visible = true;

					this.Previous.Enabled = _owner.SelectedFolder.HasPrevious;
					this.Next.Enabled = _owner.SelectedFolder.HasNext;
				}
			}

			private void OnPrevious()
			{
				_owner.SelectedFolder.MovePreviousPage();
				_owner.SelectedFolder.Update();
			}

			private void OnNext()
			{
				_owner.SelectedFolder.MoveNextPage();
				_owner.SelectedFolder.Update();
			}

			private ClickAction Previous
			{
				get { return (ClickAction) this["Previous"]; }
			}

			private ClickAction Next
			{
				get { return (ClickAction) this["Next"]; }
			}
		}

		private const bool _multiSelect = true;
		private ISelection _selectedItems = Selection.Empty;

		private event EventHandler _tableChanged;
		private event EventHandler _folderSystemChanged;
		private event EventHandler _selectedItemDoubleClicked;
		private event EventHandler _selectedItemsChanged;

		private IFolderSystem _folderSystem;
		private IFolder _selectedFolder;

		private bool _suppressFolderContentSelectionChanges;

		public IFolderSystem FolderSystem
		{
			get { return _folderSystem; }
			set
			{
				if (_folderSystem == value)
					return;

				// Must set the items and folders to null before chaning folder system, 
				// otherwise the tools that monitors folder and items selected will get out-of-sync with the folder system
				this.SelectedItems = Selection.Empty;
				this.SelectedFolder = null;

				_folderSystem = value;

				EventsHelper.Fire(_folderSystemChanged, this, EventArgs.Empty);
			}
		}

		public IFolder SelectedFolder
		{
			get { return _selectedFolder; }
			set
			{
				if (value == _selectedFolder)
					return;

				if (_selectedFolder != null)
				{
					_selectedFolder.TotalItemCountChanged -= TotalItemCountChangedEventHandler;
					_selectedFolder.ItemsTableChanging -= ItemsTableChangingEventHandler;
					_selectedFolder.ItemsTableChanged -= ItemsTableChangedEventHandler;
					_selectedFolder.Updating -= UpdatingEventHandler;
					_selectedFolder.Updated -= UpdatedEventHandler;
				}

				_selectedFolder = value;
				_selectedItems = new Selection();	// clear selected items

				if (_selectedFolder != null)
				{
					_selectedFolder.TotalItemCountChanged += TotalItemCountChangedEventHandler;
					_selectedFolder.ItemsTableChanging += ItemsTableChangingEventHandler;
					_selectedFolder.ItemsTableChanged += ItemsTableChangedEventHandler;
					_selectedFolder.Updating += UpdatingEventHandler;
					_selectedFolder.Updated += UpdatedEventHandler;
				}

				// ensure that selection changes are not suppressed
				SuppressSelectionChanges(false);

				// notify view
				EventsHelper.Fire(_tableChanged, this, EventArgs.Empty);

				// notify that the selected items have changed (because the folder has changed)
				NotifySelectedItemsChanged();

				NotifyPropertyChanged("IsUpdating");
				NotifyPropertyChanged("StatusMessage");
			}
		}

		#region Application Component overrides

		public override IActionSet ExportedActions
		{
			get 
			{
				return _folderSystem == null || _folderSystem.ItemTools == null
					? new ActionSet() 
					: _folderSystem.ItemTools.Actions; 
			}
		}

		#endregion

		#region Presentation Model

		public bool MultiSelect
		{
			get { return _multiSelect; }
		}

		public ITable FolderContentsTable
		{
			get { return _selectedFolder == null ? null : _selectedFolder.ItemsTable; }
		}

		// this is a bit of a hack to prevent the table view from sending selection changes during folder refreshes
		public bool SuppressFolderContentSelectionChanges
		{
			get { return _suppressFolderContentSelectionChanges; }
		}

		public string StatusMessage
		{
			get
			{
				if (_selectedFolder == null)
					return "";

				if (_selectedFolder.IsUpdating)
					return SR.MessageGettingFolderItems;

				// if no folder selected, or selected folder has 0 items or -1 items (i.e. unknown),
				// don't display a status message
				if (_selectedFolder.TotalItemCount < 1)
					return "";

				if (_selectedFolder.TotalItemCount == _selectedFolder.ItemsTable.Items.Count)
					return string.Format(SR.MessageShowAllItems, _selectedFolder.TotalItemCount);

				if (!_selectedFolder.SupportsPaging)
					return string.Format(SR.MessageShowPartialItems, _selectedFolder.ItemsTable.Items.Count, _selectedFolder.TotalItemCount);

				// Determine status with paging info.
				if (_selectedFolder.PageSize <= 0)
					return SR.MessageShowInvalidPage;

				var currentPage = _selectedFolder.PageNumber + 1;
				var totalPageCount = (int)Math.Ceiling((double)_selectedFolder.TotalItemCount / _selectedFolder.PageSize);

				// In situation where a tool has removed the last item on the page, there are no items to show.  show the maximum of the current page and page count.
				if (_selectedFolder.ItemsTable.Items.Count == 0)
					return string.Format(SR.MessageShowEmptyPage, currentPage, currentPage > totalPageCount ? currentPage : totalPageCount);

				var firstItemNumber = _selectedFolder.PageNumber * _selectedFolder.PageSize + 1;
				var lastItemNumber = firstItemNumber + _selectedFolder.ItemsTable.Items.Count - 1;
				return string.Format(SR.MessageShowPartialItemsWithPageInfo,
					firstItemNumber,
					lastItemNumber,
					_selectedFolder.TotalItemCount,
					currentPage,
					totalPageCount);
			}
		}

		public bool IsUpdating
		{
			get { return _selectedFolder == null ? false : _selectedFolder.IsUpdating; }
		}

		public ISelection SelectedItems
		{
			get { return _selectedItems; }
			set 
			{
				if (_selectedItems.Equals(value))
					return;

				_selectedItems = value;
				NotifySelectedItemsChanged();
			}
		}

		public event EventHandler TableChanged
		{
			add { _tableChanged += value; }
			remove { _tableChanged -= value; }
		}

		public event EventHandler FolderSystemChanged
		{
			add { _folderSystemChanged += value; }
			remove { _folderSystemChanged -= value; }
		}

		public event EventHandler SelectedItemDoubleClicked
		{
			add { _selectedItemDoubleClicked += value; }
			remove { _selectedItemDoubleClicked -= value; }
		}

		public event EventHandler SelectedItemsChanged
		{
			add { _selectedItemsChanged += value; }
			remove { _selectedItemsChanged -= value; }
		}

		public ActionModelRoot ItemsContextMenuModel
		{
			get
			{
				if (_folderSystem == null || _folderSystem.ItemTools == null)
					return null;

				var actionModel = ActionModelRoot.CreateModel(this.GetType().FullName, "folderexplorer-items-contextmenu", _folderSystem.ItemTools.Actions);
				actionModel.InsertSeparator(new Path("folderexplorer-items-contextmenu/separator"));
				actionModel.Merge(new FolderContentsPagingModel(this, new ResourceResolver(this.GetType(), true)));
				return actionModel;
			}
		}

		public ActionModelRoot ItemsToolbarModel
		{
			get
			{
				return _folderSystem == null || _folderSystem.ItemTools == null ? null
					: ActionModelRoot.CreateModel(this.GetType().FullName, "folderexplorer-items-toolbar", _folderSystem.ItemTools.Actions);
			}
		}

		public void DoubleClickSelectedItem()
		{
			EventsHelper.Fire(_selectedItemDoubleClicked, this, EventArgs.Empty);
		}

		#endregion

		private void NotifySelectedItemsChanged()
		{
			OnSelectedItemsChanged(this, EventArgs.Empty);
		}

		private void OnSelectedItemsChanged(object sender, EventArgs args)
		{
			EventsHelper.Fire(_selectedItemsChanged, sender, args);
		}

		private void TotalItemCountChangedEventHandler(object sender, EventArgs e)
		{
			NotifyPropertyChanged("StatusMessage");
		}

		private void ItemsTableChangingEventHandler(object sender, EventArgs e)
		{
			// suppress selection changes while the folder contents are being refreshed
			SuppressSelectionChanges(true);
		}

		private void ItemsTableChangedEventHandler(object sender, EventArgs e)
		{
			// update the selection appropriately - re-select the same items if possible
			// otherwise just select the first item by default

			// note: there are some subtleties here when attempting re-select the "same" items
			// if the items support IVersionedEquatable, then we need to compare them using a version-insensitive comparison,
			// but the new selection must consist of the instances that have the most current version
			_selectedItems = new Selection(CollectionUtils.Select(_selectedFolder.ItemsTable.Items,
				item => CollectionUtils.Contains(_selectedItems.Items,
					delegate(object oldItem)
					{
						return (item is IVersionedEquatable)
								? (item as IVersionedEquatable).Equals(oldItem, true) // ignore version if IVersionedEquatable
								: Equals(item, oldItem);
					})));

			// notify view about the updated selection table to the prior selection
			NotifySelectedItemsChanged();

			// revert back to normal mode where selection changes are not suppressed
			SuppressSelectionChanges(false);
		}

		private void SuppressSelectionChanges(bool suppress)
		{
			_suppressFolderContentSelectionChanges = suppress;
			NotifyPropertyChanged("SuppressFolderContentSelectionChanges");
		}

		private void UpdatingEventHandler(object sender, EventArgs e)
		{
			NotifyPropertyChanged("IsUpdating");
			NotifyPropertyChanged("StatusMessage");
		}

		private void UpdatedEventHandler(object sender, EventArgs e)
		{
			NotifyPropertyChanged("IsUpdating");
			NotifyPropertyChanged("StatusMessage");
		}

	}
}
