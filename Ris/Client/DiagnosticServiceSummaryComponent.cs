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
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("launch", "global-menus/MenuAdmin/MenuImagingServices", "Launch")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.DiagnosticService)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class DiagnosticServiceAdminTool : Tool<IDesktopToolContext>
	{
		private IWorkspace _workspace;

		public void Launch()
		{
			if (_workspace == null)
			{
				try
				{
					DiagnosticServiceSummaryComponent component = new DiagnosticServiceSummaryComponent();

					_workspace = ApplicationComponent.LaunchAsWorkspace(
						this.Context.DesktopWindow,
						component,
						SR.TitleImagingServices);
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

	/// <summary>
	/// Extension point for views onto <see cref="DiagnosticServiceSummaryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class DiagnosticServiceSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public class DiagnosticServiceSummaryTable : Table<DiagnosticServiceSummary>
	{
		private readonly int columnSortIndex = 0;

		public DiagnosticServiceSummaryTable()
		{
			this.Columns.Add(new TableColumn<DiagnosticServiceSummary, string>(SR.ColumnID,
			                                                                   delegate(DiagnosticServiceSummary rpt) { return rpt.Id; },
			                                                                   0.2f));

			this.Columns.Add(new TableColumn<DiagnosticServiceSummary, string>(SR.ColumnDiagnosticServiceName,
			                                                                   delegate(DiagnosticServiceSummary rpt) { return rpt.Name; },
			                                                                   1.0f));

			this.Sort(new TableSortParams(this.Columns[columnSortIndex], true));
		}
	}

	/// <summary>
	/// DiagnosticServiceSummaryComponent class.
	/// </summary>
	[AssociateView(typeof(DiagnosticServiceSummaryComponentViewExtensionPoint))]
	public class DiagnosticServiceSummaryComponent : SummaryComponentBase<DiagnosticServiceSummary, DiagnosticServiceSummaryTable, ListDiagnosticServicesRequest>
	{
		private string _id;
		private string _name;

		public DiagnosticServiceSummaryComponent()
		{

		}

		public DiagnosticServiceSummaryComponent(bool dialogMode)
			: base(dialogMode)
		{
		}

		#region Presentation Model

		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		#endregion


		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.DiagnosticService);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.DiagnosticService);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.DiagnosticService);
			model.ToggleActivation.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.DiagnosticService);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<DiagnosticServiceSummary> ListItems(ListDiagnosticServicesRequest request)
		{
			ListDiagnosticServicesResponse listResponse = null;
			Platform.GetService<IDiagnosticServiceAdminService>(
				delegate(IDiagnosticServiceAdminService service)
				{
					request.Id = _id;
					request.Name = _name;
					listResponse = service.ListDiagnosticServices(request);
				});
			return listResponse.DiagnosticServices;
		}


		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<DiagnosticServiceSummary> addedItems)
		{
			addedItems = new List<DiagnosticServiceSummary>();
			DiagnosticServiceEditorComponent editor = new DiagnosticServiceEditorComponent();
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddDiagnosticService);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.DiagnosticServiceSummary);
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
		protected override bool EditItems(IList<DiagnosticServiceSummary> items, out IList<DiagnosticServiceSummary> editedItems)
		{
			editedItems = new List<DiagnosticServiceSummary>();
			DiagnosticServiceSummary item = CollectionUtils.FirstElement(items);

			DiagnosticServiceEditorComponent editor = new DiagnosticServiceEditorComponent(item.DiagnosticServiceRef);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(
				this.Host.DesktopWindow, editor, string.Format("{0} - ({1}) {2}", SR.TitleUpdateDiagnosticService, item.Id, item.Name));
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.DiagnosticServiceSummary);
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
		protected override bool DeleteItems(IList<DiagnosticServiceSummary> items, out IList<DiagnosticServiceSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<DiagnosticServiceSummary>();

			foreach (DiagnosticServiceSummary item in items)
			{
				try
				{
					Platform.GetService<IDiagnosticServiceAdminService>(
						delegate(IDiagnosticServiceAdminService service)
						{
							service.DeleteDiagnosticService(new DeleteDiagnosticServiceRequest(item.DiagnosticServiceRef));
						});

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
		protected override bool UpdateItemsActivation(IList<DiagnosticServiceSummary> items, out IList<DiagnosticServiceSummary> editedItems)
		{
			List<DiagnosticServiceSummary> results = new List<DiagnosticServiceSummary>();
			foreach (DiagnosticServiceSummary item in items)
			{
				Platform.GetService<IDiagnosticServiceAdminService>(
					delegate(IDiagnosticServiceAdminService service)
					{
						DiagnosticServiceDetail detail = service.LoadDiagnosticServiceForEdit(
							new LoadDiagnosticServiceForEditRequest(item.DiagnosticServiceRef)).DiagnosticService;
						detail.Deactivated = !detail.Deactivated;
						DiagnosticServiceSummary summary = service.UpdateDiagnosticService(
							new UpdateDiagnosticServiceRequest(detail)).DiagnosticService;

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
		protected override bool IsSameItem(DiagnosticServiceSummary x, DiagnosticServiceSummary y)
		{
			if (x != null && y != null)
			{
				return x.DiagnosticServiceRef.Equals(y.DiagnosticServiceRef, true);
			}
			return false;
		}
	}
}