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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.CannedTextService;
using Action = ClearCanvas.Desktop.Actions.Action;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("launch", "global-menus/MenuTools/MenuCannedText", "Launch")]
	[ActionPermission("launch", Application.Common.AuthorityTokens.Workflow.CannedText.Personal)]
	[ActionPermission("launch", Application.Common.AuthorityTokens.Workflow.CannedText.Group)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class CannedTextTool : Tool<IDesktopToolContext>
	{
		private IShelf _shelf;

		public void Launch()
		{
			try
			{
				if (_shelf == null)
				{
					var component = new CannedTextSummaryComponent();

					_shelf = ApplicationComponent.LaunchAsShelf(
						this.Context.DesktopWindow,
						component,
						SR.TitleCannedText,
						SR.TitleCannedText,
						ShelfDisplayHint.DockFloat);

					_shelf.Closed += delegate { _shelf = null; };
				}
				else
				{
					_shelf.Activate();
				}
			}
			catch (Exception e)
			{
				// could not launch component
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}

	/// <summary>
	/// Extension point for views onto <see cref="CannedTextSummaryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class CannedTextSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// CannedTextSummaryComponent class
	/// </summary>
	[AssociateView(typeof(CannedTextSummaryComponentViewExtensionPoint))]
	public class CannedTextSummaryComponent : SummaryComponentBase<CannedTextSummary, CannedTextTable, ListCannedTextForUserRequest>
	{
		private readonly string _initialFilterText;

		private CannedTextDetail _selectedCannedTextDetail;
		private EventHandler _copyCannedTextRequested;

		private Action _duplicateCannedTextAction;
		private Action _copyCannedTextToClipboardAction;
		private Action _editCannedTextCategoryAction;

		public CannedTextSummaryComponent()
		{
		}

		public CannedTextSummaryComponent(bool dialogMode, string initialFilterText)
			: base(dialogMode)
		{
			_initialFilterText = initialFilterText;
		}

		protected override void InitializeTable(CannedTextTable table)
		{
			base.InitializeTable(table);

			if (!string.IsNullOrEmpty(_initialFilterText))
			{
				table.Filter(new TableFilterParams(null, _initialFilterText));
			}
		}

		#region Presentation Model

		public string GetFullCannedText()
		{
			if (this.SelectedItems.Count != 1)
				return string.Empty;

			// if the detail object is not null, it means the selection havn't changed
			// no need to hit the server, return the text now
			if (_selectedCannedTextDetail != null)
				return _selectedCannedTextDetail.Text;

			var summary = CollectionUtils.FirstElement(this.SelectedItems);

			try
			{
				Platform.GetService<ICannedTextService>(
					service =>
					{
						var response = service.LoadCannedTextForEdit(new LoadCannedTextForEditRequest(summary.CannedTextRef));
						_selectedCannedTextDetail = response.CannedTextDetail;
					});
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}

			return _selectedCannedTextDetail.Text;
		}

		public event EventHandler CopyCannedTextRequested
		{
			add { _copyCannedTextRequested += value; }
			remove { _copyCannedTextRequested -= value; }
		}

		public void DuplicateAdd()
		{
			try
			{
				var item = CollectionUtils.FirstElement(this.SelectedItems);
				IList<CannedTextSummary> addedItems = new List<CannedTextSummary>();
				var editor = new CannedTextEditorComponent(GetCategoryChoices(), item.CannedTextRef, true);

				var exitCode = LaunchAsDialog(
					this.Host.DesktopWindow, editor, SR.TitleDuplicateCannedText);
				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					addedItems.Add(editor.UpdatedCannedTextSummary);
					this.Table.Items.AddRange(addedItems);
					this.SummarySelection = new Selection(addedItems);
				}
			}
			catch (Exception e)
			{
				// failed to launch editor
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void CopyCannedText()
		{
			EventsHelper.Fire(_copyCannedTextRequested, this, EventArgs.Empty);
		}

		public void EditCategories()
		{
			try
			{
				if (!CanMultiEditCategories())
				{
					this.Host.DesktopWindow.ShowMessageBox(SR.MessageCannotEditCategories, MessageBoxActions.Ok);
					return;
				}

				var items = this.SelectedItems;
				var initialCategory = CollectionUtils.FirstElement(items).Category;
				var editor = new CannedTextCategoryEditorComponent(GetCategoryChoices(), initialCategory);

				var exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleChangeCategory);
				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					var newCategory = editor.Category;

					Platform.GetService<ICannedTextService>(
						service =>
						{
							var response = service.EditCannedTextCategories(
								new EditCannedTextCategoriesRequest(
									CollectionUtils.Map<CannedTextSummary, EntityRef>(items, item => item.CannedTextRef),
									newCategory));

							var table = (Table<CannedTextSummary>)this.SummaryTable;
							foreach (var cannedText in response.CannedTexts)
							{
								table.Items.Replace(
									x => IsSameItem(cannedText, x),
									cannedText);
							}

							this.SummarySelection = new Selection(response.CannedTexts);
						});
				}

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		#endregion

		#region Overrides

		protected override bool SupportsEdit
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether this component supports deletion.  The default is false.
		/// Override this method to support deletion.
		/// </summary>
		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether this component supports paging.  The default is true.
		/// Override this method to change support for paging.
		/// </summary>
		protected override bool SupportsPaging
		{
			get { return false; }
		}

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			_duplicateCannedTextAction = model.AddAction("duplicateCannedText", SR.TitleDuplicate, "Icons.DuplicateSmall.png",
				SR.TitleDuplicate, DuplicateAdd);

			_copyCannedTextToClipboardAction = model.AddAction("copyCannedText", SR.TitleCopy, "Icons.CopyToClipboardToolSmall.png",
				SR.MessageCopyToClipboard, CopyCannedText);

			_editCannedTextCategoryAction = model.AddAction("editCategory", SR.TitleChangeCategory, "Icons.MultiEditToolSmall.png",
				SR.MessageChangeCategoryToolTip, EditCategories);

			_duplicateCannedTextAction.Enabled = false;
			_copyCannedTextToClipboardAction.Enabled = false;
			_editCannedTextCategoryAction.Enabled = false;
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<CannedTextSummary> ListItems(ListCannedTextForUserRequest request)
		{
			ListCannedTextForUserResponse listResponse = null;
			Platform.GetService<ICannedTextService>(
				service => listResponse = service.ListCannedTextForUser(request));
			return listResponse.CannedTexts;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<CannedTextSummary> addedItems)
		{
			addedItems = new List<CannedTextSummary>();
			var editor = new CannedTextEditorComponent(GetCategoryChoices());
			var exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddCannedText);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.UpdatedCannedTextSummary);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "edit" action.
		/// </summary>
		/// <param name="items">A list of items to edit.</param>
		/// <param name="editedItems">The list of items that were edited.</param>
		/// <returns>True if items were edited, false otherwise.</returns>
		protected override bool EditItems(IList<CannedTextSummary> items, out IList<CannedTextSummary> editedItems)
		{
			editedItems = new List<CannedTextSummary>();
			var item = CollectionUtils.FirstElement(items);

			var editor = new CannedTextEditorComponent(GetCategoryChoices(), item.CannedTextRef);
			var exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateCannedText);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.UpdatedCannedTextSummary);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="deletedItems">The list of items that were deleted.</param>
		/// <param name="failureMessage">The message if there any errors that occurs during deletion.</param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<CannedTextSummary> items, out IList<CannedTextSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<CannedTextSummary>();

			foreach (var item in items)
			{
				try
				{
					Platform.GetService<ICannedTextService>(
						service => service.DeleteCannedText(new DeleteCannedTextRequest(item.CannedTextRef)));

					deletedItems.Add(item);
				}
				catch (Exception e)
				{
					failureMessage = e.Message;
				}
			}

			return deletedItems.Count > 0;
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(CannedTextSummary x, CannedTextSummary y)
		{
			return x.CannedTextRef.Equals(y.CannedTextRef, true);
		}

		/// <summary>
		/// Called when the user changes the selected items in the table.
		/// </summary>
		protected override void OnSelectedItemsChanged()
		{
			base.OnSelectedItemsChanged();

			if (this.SelectedItems.Count == 1)
			{
				var selectedItem = this.SelectedItems[0];

				_copyCannedTextToClipboardAction.Enabled = true;

				this.ActionModel.Add.Enabled = HasPersonalAdminAuthority || HasGroupAdminAuthority;
				this.ActionModel.Delete.Enabled =
					_duplicateCannedTextAction.Enabled =
						selectedItem.IsPersonal && HasPersonalAdminAuthority ||
						selectedItem.IsGroup && HasGroupAdminAuthority;
			}
			else
			{
				_duplicateCannedTextAction.Enabled = false;
				_copyCannedTextToClipboardAction.Enabled = false;
			}

			_editCannedTextCategoryAction.Enabled = this.SelectedItems.Count > 1;

			// The detail is only loaded whenever a copy/drag is performed
			// Set this to null, so the view doesn't get wrong text data.
			_selectedCannedTextDetail = null;

			NotifyAllPropertiesChanged();
		}

		#endregion

		private static bool HasPersonalAdminAuthority
		{
			get { return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.CannedText.Personal); }
		}

		private static bool HasGroupAdminAuthority
		{
			get { return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.CannedText.Group); }
		}

		private bool CanMultiEditCategories()
		{
			return (CollectionUtils.Contains(this.SelectedItems, item => item.IsPersonal)
						? HasPersonalAdminAuthority
						: true)
				   && (CollectionUtils.Contains(this.SelectedItems, item => item.IsGroup)
						? HasGroupAdminAuthority
						: true);
		}

		private List<string> GetCategoryChoices()
		{
			var categoryChoices = new List<string>();
			CollectionUtils.ForEach<CannedTextSummary>(this.SummaryTable.Items, c =>
				{
					if (!categoryChoices.Contains(c.Category))
						categoryChoices.Add(c.Category);
				});

			categoryChoices.Sort();

			return categoryChoices;
		}
	}
}
