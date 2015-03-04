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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.DepartmentAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
	[MenuAction("launch", "global-menus/MenuAdmin/MenuDepartments", "Launch")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Department)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class DepartmentAdminTool : Tool<IDesktopToolContext>
	{
		private IWorkspace _workspace;

		public void Launch()
		{
			if (_workspace == null)
			{
				try
				{
					var component = new DepartmentSummaryComponent();

					_workspace = ApplicationComponent.LaunchAsWorkspace(
						this.Context.DesktopWindow,
						component,
						SR.TitleDepartments);
					_workspace.Closed += delegate { _workspace = null; };

				}
				catch (Exception e)
				{
					// failed to launch component
					ExceptionHandler.Report(e, this.Context.DesktopWindow);
				}
			}
			else
			{
				_workspace.Activate();
			}
		}
	}

	public class DepartmentSummaryTable : Table<DepartmentSummary>
	{
		public DepartmentSummaryTable()
		{
			this.Columns.Add(new TableColumn<DepartmentSummary, string>(SR.ColumnID, dept => dept.Id, 0.2f));
			this.Columns.Add(new TableColumn<DepartmentSummary, string>(SR.ColumnName, dept => dept.Name, 1.0f));
			this.Columns.Add(new TableColumn<DepartmentSummary, string>(SR.ColumnFacility, dept => dept.FacilityName, 1.0f));
		}
	}



	/// <summary>
	/// DepartmentSummaryComponent class.
	/// </summary>
	public class DepartmentSummaryComponent : SummaryComponentBase<DepartmentSummary, DepartmentSummaryTable, ListDepartmentsRequest>
	{
		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Department);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Department);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Department);
			model.ToggleActivation.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Department);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<DepartmentSummary> ListItems(ListDepartmentsRequest request)
		{
			ListDepartmentsResponse listResponse = null;
			Platform.GetService<IDepartmentAdminService>(
				service => listResponse = service.ListDepartments(request));

			return listResponse.Departments;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<DepartmentSummary> addedItems)
		{
			addedItems = new List<DepartmentSummary>();
			var editor = new DepartmentEditorComponent();
			var exitCode = LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddDepartment);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.DepartmentSummary);
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
		protected override bool EditItems(IList<DepartmentSummary> items, out IList<DepartmentSummary> editedItems)
		{
			editedItems = new List<DepartmentSummary>();
			var item = CollectionUtils.FirstElement(items);

			var editor = new DepartmentEditorComponent(item.DepartmentRef);
			var exitCode = LaunchAsDialog(
				this.Host.DesktopWindow, editor, string.Format(SR.FormatTitleCodeSubtitle, SR.TitleUpdateDepartment, item.Id, item.Name));
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.DepartmentSummary);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="deletedItems"></param>
		/// <param name="failureMessage"></param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<DepartmentSummary> items, out IList<DepartmentSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<DepartmentSummary>();

			foreach (var item in items)
			{
				try
				{
					Platform.GetService<IDepartmentAdminService>(
						service => service.DeleteDepartment(new DeleteDepartmentRequest(item.DepartmentRef)));

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
		/// Called to handle the "toggle activation" action, if supported
		/// </summary>
		/// <param name="items">A list of items to edit.</param>
		/// <param name="editedItems">The list of items that were edited.</param>
		/// <returns>True if items were edited, false otherwise.</returns>
		protected override bool UpdateItemsActivation(IList<DepartmentSummary> items, out IList<DepartmentSummary> editedItems)
		{
			var results = new List<DepartmentSummary>();
			foreach (var item in items)
			{
				Platform.GetService<IDepartmentAdminService>(
					service =>
					{
						var detail = service.LoadDepartmentForEdit(
							new LoadDepartmentForEditRequest(item.DepartmentRef)).Department;
						detail.Deactivated = !detail.Deactivated;
						var summary = service.UpdateDepartment(
							new UpdateDepartmentRequest(detail)).Department;

						results.Add(summary);
					});
			}

			editedItems = results;
			return true;
		}

		/// <summary>
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(DepartmentSummary x, DepartmentSummary y)
		{
			return x.DepartmentRef.Equals(y.DepartmentRef, true);
		}
	}
}
