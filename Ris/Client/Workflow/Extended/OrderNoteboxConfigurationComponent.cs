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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Extended.Common.OrderNotes;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[MenuAction("apply", "folderexplorer-folders-contextmenu/Configure", "Apply")]
	[ButtonAction("apply", "folderexplorer-folders-toolbar/Configure", "Apply")]
	[Tooltip("apply", "Configure Notebox Groups")]
	[IconSet("apply", "Icons.OptionsToolSmall.png", "Icons.OptionsToolSmall.png", "Icons.OptionsToolSmall.png")]
	//[ExtensionOf(typeof(OrderNoteboxFolderToolExtensionPoint))]
	public class OrderNoteboxConfigurationTool : Tool<IOrderNoteboxFolderToolContext>
	{
		public void Apply()
		{
			OrderNoteboxConfigurationComponent component = new OrderNoteboxConfigurationComponent();
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Context.DesktopWindow, component, "Group Notebox Folder Configuration");

			if(exitCode == ApplicationComponentExitCode.Accepted)
			{
				this.Context.RebuildGroupFolders();
			}

		}
	}

	/// <summary>
	/// Extension point for views onto <see cref="OrderNoteboxConfigurationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class OrderNoteboxConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// OrderNoteboxConfigurationComponent class
	/// </summary>
	[AssociateView(typeof(OrderNoteboxConfigurationComponentViewExtensionPoint))]
	public class OrderNoteboxConfigurationComponent : ApplicationComponent
	{
		class TableItem : Checkable<StaffGroupSummary>
		{
			private readonly bool _isNew;
			public TableItem(StaffGroupSummary group, bool isChecked, bool isNew)
				:base(group, isChecked)
			{
				_isNew = isNew;
			}

			public bool IsNew { get { return _isNew; } }
		}

		private Table<TableItem> _staffGroupTable;
		private TableItem _selectedTableItem;

		private StaffGroupLookupHandler _staffGroupLookupHandler;
		private StaffGroupSummary _staffGroupToAdd;
		private CrudActionModel _actionModel;

		/// <summary>
		/// Constructor
		/// </summary>
		public OrderNoteboxConfigurationComponent()
		{
		}

		public override void Start()
		{
			// create table
			_staffGroupTable = new Table<TableItem>();
			_staffGroupTable.Columns.Add(new TableColumn<TableItem, bool>("Show Folder",
				delegate(TableItem item) { return item.IsChecked; },
				delegate(TableItem item, bool value) { item.IsChecked = value; }));

			_staffGroupTable.Columns.Add(new TableColumn<TableItem, string>("Group Name",
				delegate(TableItem item) { return item.Item.Name; }));
			_staffGroupTable.Columns.Add(new TableColumn<TableItem, string>("Description",
				delegate(TableItem item) { return item.Item.Description; }));

			_actionModel = new CrudActionModel(false, false, true);
			_actionModel.Delete.SetClickHandler(DeleteStaffGroup);
			_actionModel.Delete.Enabled = false;

			// load existing group memberships
			Platform.GetService<IOrderNoteService>(
				delegate(IOrderNoteService service)
				{
					List<string> visibleGroups = OrderNoteboxFolderSystemSettings.Default.GroupFolders.StaffGroupNames;
					List<StaffGroupSummary> groups = service.ListStaffGroups(new ListStaffGroupsRequest()).StaffGroups;
					_staffGroupTable.Items.AddRange(
						CollectionUtils.Map<StaffGroupSummary, TableItem>(groups,
							delegate(StaffGroupSummary g)
							{
								bool visible = CollectionUtils.Contains(visibleGroups,
									delegate (string groupName) { return g.Name == groupName;});
								 return new TableItem(g, visible, false);
							}));
				});

			_staffGroupLookupHandler = new StaffGroupLookupHandler(this.Host.DesktopWindow, true);

			base.Start();
		}

		#region Presentation Model

		public ILookupHandler StaffGroupLookupHandler
		{
			get { return _staffGroupLookupHandler; }
		}

		public StaffGroupSummary StaffGroupToAdd
		{
			get { return _staffGroupToAdd; }
			set
			{
				if(!Equals(value, _staffGroupToAdd))
				{
					_staffGroupToAdd = value;
					NotifyPropertyChanged("StaffGroupToAdd");
				}
			}
		}

		public ITable StaffGroupTable
		{
			get { return _staffGroupTable; }
		}

		public ActionModelNode ActionModel
		{
			get { return _actionModel; }
		}

		public ISelection SelectedTableItem
		{
			get { return new Selection(_selectedTableItem); }
			set
			{
				if(!Equals(new Selection(_selectedTableItem), value))
				{
					_selectedTableItem = (TableItem)value.Item;
					NotifyPropertyChanged("SelectedTableItem");
					UpdateActionModel();
				}
			}
		}

		public void AddStaffGroup()
		{
			if(_staffGroupToAdd == null || _staffGroupTable.Items.Contains(_staffGroupToAdd))
				return;

			_staffGroupTable.Items.Add(new TableItem(_staffGroupToAdd, true, true));
		}

		public void DeleteStaffGroup()
		{
			if(_selectedTableItem == null || !_selectedTableItem.IsNew)
				return;

			TableItem itemToDelete = _selectedTableItem;
			_selectedTableItem = null;
			NotifyPropertyChanged("SelectedTableItem");

			_staffGroupTable.Items.Remove(itemToDelete);
		}

		public void Accept()
		{
			try
			{
				// if the user added any new groups, need to add the user as a member of those groups
				List<TableItem> newItems = CollectionUtils.Select(_staffGroupTable.Items,
					delegate(TableItem item) { return item.IsNew; });

				if (newItems.Count > 0)
				{
					AddStaffGroupsRequest request = new AddStaffGroupsRequest(
						CollectionUtils.Map<TableItem, StaffGroupSummary>(newItems,
							delegate(TableItem item) { return item.Item; }));

					Platform.GetService<IOrderNoteService>(
						delegate(IOrderNoteService service)
						{
							service.AddStaffGroups(request);
						});
				}

				// save the set of folders that should be visible
				List<TableItem> visibleItems = CollectionUtils.Select(_staffGroupTable.Items,
					delegate(TableItem item) { return item.IsChecked; });

				OrderNoteboxFolderSystemSettings.Default.GroupFolders = 
					new OrderNoteboxFolderSystemSettings.GroupFoldersData(
						CollectionUtils.Map<TableItem, string>(visibleItems,
							delegate(TableItem item) { return item.Item.Name; }));

				OrderNoteboxFolderSystemSettings.Default.Save();

				this.Exit(ApplicationComponentExitCode.Accepted);

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, "", this.Host.DesktopWindow,
					delegate
					{
						this.Exit(ApplicationComponentExitCode.Error);
					});
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		private void UpdateActionModel()
		{
			_actionModel.Delete.Enabled = _selectedTableItem != null && _selectedTableItem.IsNew;
		}

	}
}
