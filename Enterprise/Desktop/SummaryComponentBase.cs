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
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Desktop
{
	[ExtensionPoint]
	public class SummaryComponentBaseViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}


	/// <summary>
	/// Abstract base class for admin summary components.
	/// </summary>
	[AssociateView(typeof(SummaryComponentBaseViewExtensionPoint))]
	public abstract class SummaryComponentBase : ApplicationComponent, ISummaryComponent
	{
		#region Presentation Model

		/// <summary>
		/// Gets the summary table action model.
		/// </summary>
		public abstract ActionModelNode SummaryTableActionModel { get; }

		/// <summary>
		/// Gets the summary table <see cref="ITable"/>.
		/// </summary>
		public abstract ITable SummaryTable { get; }

		/// <summary>
		/// Gets the summary table selection as an <see cref="ISelection"/>.
		/// </summary>
		public abstract ISelection SummarySelection { get; set; }

		/// <summary>
		/// Handles the "search" action if supported.
		/// </summary>
		public abstract void Search();

		/// <summary>
		/// Handles the "add" action.
		/// </summary>
		public abstract void AddItems();

		/// <summary>
		/// Handles the "edit" action.
		/// </summary>
		public abstract void EditSelectedItems();

		/// <summary>
		/// Handles the "delete" action.
		/// </summary>
		public abstract void DeleteSelectedItems();

		/// <summary>
		/// Handles the "toggle activation" action.
		/// </summary>
		public abstract void ToggleSelectedItemsActivation();

		/// <summary>
		/// Gets a value indicating whether dialog accept/cancel buttons are visible.
		/// </summary>
		public abstract bool ShowAcceptCancelButtons { get; }

		/// <summary>
		/// Gets a value indicating whether accept button is enabled.
		/// </summary>
		public abstract bool AcceptEnabled { get; }

		/// <summary>
		/// Handles double-clicking on a list item.
		/// </summary>
		public abstract void DoubleClickSelectedItem();

		/// <summary>
		/// Handles the accept button.
		/// </summary>
		public abstract void Accept();

		/// <summary>
		/// Handles the cancel button.
		/// </summary>
		public abstract void Cancel();

		#endregion
	}

	/// <summary>
	/// Abstract base class for admin summary components.
	/// </summary>
	/// <typeparam name="TSummary"></typeparam>
	/// <typeparam name="TTable"></typeparam>
	public abstract class SummaryComponentBase<TSummary, TTable> : SummaryComponentBase, ISummaryComponent<TSummary, TTable>
		where TSummary : class
		where TTable : Table<TSummary>, new()
	{
		public class AdminActionModel : CrudActionModel
		{
			private static readonly object ToggleActivationKey = new object();

			public AdminActionModel(bool add, bool edit, bool delete, bool toggleActivation, IResourceResolver resolver)
				:base(add, edit, delete, resolver)
			{
				if (toggleActivation)
				{
					this.AddAction(ToggleActivationKey, SR.LabelToggleActivation, "Icons.ToggleActivationSmall.png");
				}
			}

			/// <summary>
			/// Gets the ToggleActivation action.
			/// </summary>
			public ClickAction ToggleActivation
			{
				get { return (ClickAction)this[ToggleActivationKey]; }
			}
		}



		private IList<TSummary> _selectedItems;
		private readonly TTable _summaryTable;

		private AdminActionModel _actionModel;
		private PagingController<TSummary> _pagingController;

		private readonly bool _dialogMode;
		private bool _hostedMode;
		private bool _setModifiedOnListChange;

		private event EventHandler _summarySelectionChanged;
		private event EventHandler _itemDoubleClicked;

		protected SummaryComponentBase()
			: this(false)
		{
		}

		protected SummaryComponentBase(bool dialogMode)
		{
			_dialogMode = dialogMode;
			_summaryTable = new TTable();
			_selectedItems = new List<TSummary>();

			// by default, we do not include de-activated items in dialog mode
			this.IncludeDeactivatedItems = !_dialogMode;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this component will set <see cref="ApplicationComponent.Modified"/> to true
		/// when the summary list is changed.
		/// </summary>
		public bool SetModifiedOnListChange
		{
			get { return _setModifiedOnListChange; }
			set { _setModifiedOnListChange = value; }
		}

		/// <summary>
		/// Gets or sets whether the component is being hosted by another component.  Hosted mode overrides dialog mode.
		/// When being hosted, the Accept/Cancel and double click actions are disabled.  It is up to the hosting component
		/// to decide what to do.
		/// </summary>
		public bool HostedMode
		{
			get { return _hostedMode; }
			set { _hostedMode = value; }
		}

		/// <summary>
		/// Gets or sets whether this component is in a read-only mode.  If true, the defaul add, edit, delete, and toggle activation
		/// actions will be hidden.
		/// </summary>
		public bool ReadOnly { get; set; }

		/// <summary>
		/// Gets or sets whether this component includes de-activated items in the list.
		/// </summary>
		public bool IncludeDeactivatedItems { get; set; }

		#region ApplicationComponent overrides

		public override void Start()
		{
			InitializeTable(_summaryTable);

			// add the "Active" column if needed
			if (SupportsDeactivation && IncludeDeactivatedItems)
			{
				_summaryTable.Columns.Add(new TableColumn<TSummary, bool>(SR.ColumnActive, item => !GetItemDeactivated(item), 0.15f));
			}

			// build the action model
			_actionModel = new AdminActionModel(
				this.SupportsAdd,
				this.SupportsEdit,
				this.SupportsDelete,
				this.SupportsDeactivation,
				new ApplicationThemeResourceResolver(this.GetType(), true));

			if (SupportsAdd)
			{
				_actionModel.Add.SetClickHandler(AddItems);
				_actionModel.Add.Visible = !this.ReadOnly;
			}

			if (SupportsEdit)
			{
				_actionModel.Edit.SetClickHandler(EditSelectedItems);
				_actionModel.Edit.Enabled = false;
				_actionModel.Edit.Visible = !this.ReadOnly;
			}

			if (SupportsDelete)
			{
				_actionModel.Delete.SetClickHandler(DeleteSelectedItems);
				_actionModel.Delete.Enabled = false;
				_actionModel.Delete.Visible = !this.ReadOnly;
			}

			if (SupportsDeactivation)
			{
				_actionModel.ToggleActivation.SetClickHandler(ToggleSelectedItemsActivation);
				_actionModel.ToggleActivation.Enabled = false;
				_actionModel.ToggleActivation.Visible = !this.ReadOnly && IncludeDeactivatedItems;
			}

			InitializeActionModel(_actionModel);

			// setup paging
			if (SupportsPaging)
			{
				_pagingController = new PagingController<TSummary>(ListItems);
				_pagingController.PageChanged += PagingControllerPageChangedEventHandler;
				_actionModel.Merge(new PagingActionModel<TSummary>(_pagingController, Host.DesktopWindow));

				// load first page of items
				_pagingController.GetFirst();
			}
			else
			{
				// load the one and only page of items
				_summaryTable.Items.AddRange(ListItems(0, -1));
			}

			// Apply existing sort parameters
			_summaryTable.Sort(_summaryTable.SortParams);

			base.Start();
		}

		private void PagingControllerPageChangedEventHandler(object sender, PageChangedEventArgs<TSummary> e)
		{
			this.Table.Items.Clear();
			this.Table.Items.AddRange(e.Items);
		}

		#endregion

		#region Presentation Model

		/// <summary>
		/// Gets the summary table action model.
		/// </summary>
		public override ActionModelNode SummaryTableActionModel
		{
			get { return _actionModel; }
		}

		/// <summary>
		/// Gets the summary table <see cref="ITable"/>.
		/// </summary>
		public override ITable SummaryTable
		{
			get { return _summaryTable; }
		}

		/// <summary>
		/// Gets the summary table selection as an <see cref="ISelection"/>.
		/// </summary>
		public override ISelection SummarySelection
		{
			get
			{
				return new Selection(_selectedItems);
			}
			set
			{
				var previousSelection = new Selection(_selectedItems);
				if (!previousSelection.Equals(value))
				{
					_selectedItems = new TypeSafeListWrapper<TSummary>(value.Items);
					OnSelectedItemsChanged();
					NotifyPropertyChanged("SummarySelection");
					EventsHelper.Fire(_summarySelectionChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler SummarySelectionChanged
		{
			add { _summarySelectionChanged += value; }
			remove { _summarySelectionChanged -= value; }
		}

		public event EventHandler ItemDoubleClicked
		{
			add { _itemDoubleClicked += value; }
			remove { _itemDoubleClicked -= value; }
		}

		public override void Search()
		{
			try
			{
				 _pagingController.GetFirst();
			}
			catch (Exception e)
			{
				// search failed
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		/// <summary>
		/// Handles the "add" action.
		/// </summary>
		public override void AddItems()
		{
			try
			{
				IList<TSummary> addedItems;
				if(AddItems(out addedItems))
				{
					_summaryTable.Items.AddRange(addedItems);
					this.SummarySelection = new Selection(addedItems);
					if (_setModifiedOnListChange)
						this.Modified = true;
				}
			}
			catch (Exception e)
			{
				// failed to launch editor
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		/// <summary>
		/// Handles the "edit" action.
		/// </summary>
		public override void EditSelectedItems()
		{
			try
			{
				if (_selectedItems.Count != 1) return;

				IList<TSummary> editedItems;
				if (EditItems(_selectedItems, out editedItems))
				{
					foreach (var item in editedItems)
					{
						_summaryTable.Items.Replace(x => IsSameItem(item, x), item);
					}

					this.SummarySelection = new Selection(editedItems);
					if (_setModifiedOnListChange)
						this.Modified = true;
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		/// <summary>
		/// Handles the "delete" action.
		/// </summary>
		public override void DeleteSelectedItems()
		{
			try
			{
				if (_selectedItems.Count == 0) return;

				string confirmationMessage;
				var doConfirmation = GetDeleteConfirmationMessage(_selectedItems, out confirmationMessage);
				if (!doConfirmation || this.Host.ShowMessageBox(confirmationMessage, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
				{
					string failureMessage;
					IList<TSummary> deletedItems;

					if (DeleteItems(_selectedItems, out deletedItems, out failureMessage))
					{
						var notDeletedItems = new List<TSummary>(_selectedItems);

						// remove from table
						CollectionUtils.ForEach(deletedItems, 
							delegate(TSummary item)
								{
									notDeletedItems.Remove(item);
									_summaryTable.Items.Remove(item);
								});

						// clear selection
						this.SummarySelection = new Selection(notDeletedItems);
						if (_setModifiedOnListChange)
							this.Modified = true;
					}

					if (!string.IsNullOrEmpty(failureMessage))
						this.Host.ShowMessageBox(failureMessage, MessageBoxActions.Ok);
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public override void ToggleSelectedItemsActivation()
		{
			try
			{
				if (_selectedItems.Count == 0) return;

				IList<TSummary> editedItems;
				if (UpdateItemsActivation(_selectedItems, out editedItems))
				{
					foreach (var item in editedItems)
					{
						_summaryTable.Items.Replace(x => IsSameItem(item, x), item);
					}

					this.SummarySelection = new Selection(editedItems);
					if (_setModifiedOnListChange)
						this.Modified = true;
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public override bool ShowAcceptCancelButtons
		{
			get { return _hostedMode ? false : _dialogMode; }
		}

		public override bool AcceptEnabled
		{
			get { return _selectedItems.Count > 0; }
		}

		public override void DoubleClickSelectedItem()
		{
			if (!_hostedMode)
			{
				// double-click behaviour is different depending on whether we're running as a dialog box or not
				if (_dialogMode)
					Accept();
				else if (SupportsEdit)
					EditSelectedItems();
			}

			EventsHelper.Fire(_itemDoubleClicked, this, EventArgs.Empty);
		}

		public override void Accept()
		{
			if (!_hostedMode)
			{
				if (this.HasValidationErrors)
				{
					ShowValidation(true);
					return;
				}
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
		}

		public override void Cancel()
		{
			if (!_hostedMode)
				this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		#region Abstract/overridable members
		
		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// If <see cref="SupportsPaging"/> is false, then this method should ignore the first and max items
		/// parameters and return all items.
		/// </summary>
		/// <returns></returns>
		protected abstract IList<TSummary> ListItems(int firstRow, int maxRows);

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected abstract bool AddItems(out IList<TSummary> addedItems);

		/// <summary>
		/// Called to handle the "edit" action, if supported
		/// </summary>
		/// <param name="items">A list of items to edit.</param>
		/// <param name="editedItems">The list of items that were edited.</param>
		/// <returns>True if items were edited, false otherwise.</returns>
		protected abstract bool EditItems(IList<TSummary> items, out IList<TSummary> editedItems);


		/// <summary>
		/// Called to handle the "toggle activation" action, if supported
		/// </summary>
		/// <param name="items">A list of items to edit.</param>
		/// <param name="editedItems">The list of items that were edited.</param>
		/// <returns>True if items were edited, false otherwise.</returns>
		protected virtual bool UpdateItemsActivation(IList<TSummary> items, out IList<TSummary> editedItems)
		{
			editedItems = new List<TSummary>();
			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="deletedItems">The list of items that were deleted.</param>
		/// <param name="failureMessage">The message if there any errors that occurs during deletion.</param>
		/// <returns>True if any items were deleted, false otherwise.</returns>
		protected abstract bool DeleteItems(IList<TSummary> items, out IList<TSummary> deletedItems, out string failureMessage);

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected abstract bool IsSameItem(TSummary x, TSummary y);

		/// <summary>
		/// Override this method to perform custom initialization of the table,
		/// such as adding columns or setting other properties that control the table behaviour.
		/// </summary>
		/// <param name="table"></param>
		protected virtual void InitializeTable(TTable table)
		{

		}

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected virtual void InitializeActionModel(AdminActionModel model)
		{
		}

		/// <summary>
		/// Called when the user changes the selected items in the table.
		/// </summary>
		protected virtual void OnSelectedItemsChanged()
		{
			if(SupportsEdit)
				_actionModel.Edit.Enabled = _selectedItems.Count == 1;

			if(SupportsDelete)
				_actionModel.Delete.Enabled = _selectedItems.Count > 0;

			if (SupportsDeactivation)
				_actionModel.ToggleActivation.Enabled = _selectedItems.Count > 0;
		}

		/// <summary>
		/// Gets a value indicating whether this component supports add.  The default is true.
		/// Override this method to change support for edit.
		/// </summary>
		protected virtual bool SupportsAdd
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether this component supports edit.  The default is true.
		/// Override this method to change support for edit.
		/// </summary>
		protected virtual bool SupportsEdit
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether this component supports deletion.  The default is false.
		/// Override this method to change support for deletion.
		/// </summary>
		protected virtual bool SupportsDelete
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the message to be presented to the user to confirm the deletion of items, or returns
		/// false to specify that no confirmation is required.
		/// </summary>
		protected virtual bool GetDeleteConfirmationMessage(IList<TSummary> itemsToBeDeleted, out string message)
		{
			message= SR.MessageConfirmDeleteSelectedItems;
			return true;
		}

		/// <summary>
		/// Gets a value indicating whether this component supports paging.  The default is true.
		/// Override this method to change support for paging.
		/// </summary>
		protected virtual bool SupportsPaging
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether the items listed by this component support de-activation.
		/// The default implementation looks for a boolean "Deactivated" field on the summary item class,
		/// and assumes true if this field is present.
		/// </summary>
		protected virtual bool SupportsDeactivation
		{
			get
			{
				var f = GetDeactivatedField();
				return f != null && f.FieldType.Equals(typeof (bool));
			}
		}

		/// <summary>
		/// Called to determine whether the specified item is deactivated, assuming <see cref="SupportsDeactivation"/> is true.
		/// The default implementation queries the "Deactivated" field on the summary item class.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual bool GetItemDeactivated(TSummary item)
		{
			var field = GetDeactivatedField();
			return field == null ? false : (bool)field.GetValue(item);
		}

		#endregion 
		
		/// <summary>
		/// Gets the action model.
		/// </summary>
		protected AdminActionModel ActionModel
		{
			get { return _actionModel; }
		}

		/// <summary>
		/// Gets the table.
		/// </summary>
		protected TTable Table
		{
			get { return _summaryTable; }
		}

		/// <summary>
		/// Gets the paging controller.
		/// </summary>
		protected IPagingController<TSummary> PagingController
		{
			get { return _pagingController; }
		}

		/// <summary>
		/// Gets the selected items.
		/// </summary>
		protected IList<TSummary> SelectedItems
		{
			get { return _selectedItems; }
		}

		/// <summary>
		/// Gets a value indicating whether the component is running in dialog mode.
		/// </summary>
		protected bool DialogMode
		{
			get { return _dialogMode; }
		}

		private static FieldInfo GetDeactivatedField()
		{
			return typeof(TSummary).GetField("Deactivated");
		}

		private void ListItems(int firstRow, int maxRows, Action<IList<TSummary>> resultHandler)
		{
			IList<TSummary> results = null;
			//TODO (CR April 2011): Not sure about this idea.  Inheritors may not realize
			//the call to ListItems is being made on a worker thread, and may try to catch exceptions
			//and show errors to the user on the worker thread ... that's actually how I found this issue.
			Async.Invoke(this, 
				() => results = ListItems(firstRow, maxRows), 
				() => resultHandler(results), 
				delegate(Exception e)
					{
						resultHandler.Invoke(new List<TSummary>());
						ExceptionHandler.Report(e, Host.DesktopWindow);
					});
		}
	}

	public abstract class SummaryComponentBase<TSummary, TTable, TListRequest> : SummaryComponentBase<TSummary, TTable>
		where TSummary : class
		where TTable : Table<TSummary>, new()
		where TListRequest : ListRequestBase, new()
	{
		protected SummaryComponentBase()
		{
		}

		protected SummaryComponentBase(bool dialogMode)
			:base(dialogMode)
		{
		}

		protected override IList<TSummary> ListItems(int firstRow, int maxRows)
		{
			var request = new TListRequest
			              	{
			              		Page = {FirstRow = firstRow, MaxRows = maxRows},
			              		IncludeDeactivated = this.IncludeDeactivatedItems
			              	};

			return ListItems(request);
		}

		protected abstract IList<TSummary> ListItems(TListRequest request);
	}

}
