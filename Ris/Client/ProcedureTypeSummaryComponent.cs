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
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("launch", "global-menus/MenuAdmin/MenuProcedureTypes", "Launch")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureType)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class ProcedureTypeAdminTool : Tool<IDesktopToolContext>
	{
		private IWorkspace _workspace;

		public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    ProcedureTypeSummaryComponent component = new ProcedureTypeSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleProcedureTypes);
                    _workspace.Closed += delegate { _workspace = null; };

                }
                catch (Exception e)
                {
                    // could not launch editor
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
	/// Extension point for views onto <see cref="ProcedureTypeSummaryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ProcedureTypeSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ProcedureTypeSummaryComponent class.
	/// </summary>
	[AssociateView(typeof(ProcedureTypeSummaryComponentViewExtensionPoint))]
	public class ProcedureTypeSummaryComponent : SummaryComponentBase<ProcedureTypeSummary, ProcedureTypeSummaryTable, ListProcedureTypesRequest>
	{
		private string _id;
		private string _name;

		public ProcedureTypeSummaryComponent()
		{

		}

		public ProcedureTypeSummaryComponent(bool dialogMode)
			: base(dialogMode)
		{
		}

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureType);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureType);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureType);
			model.ToggleActivation.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureType);
		}

		protected override bool SupportsDelete
		{
			get
			{
				return true;
			}
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
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<ProcedureTypeSummary> ListItems(ListProcedureTypesRequest request)
		{
			ListProcedureTypesResponse _response = null;
			Platform.GetService<IProcedureTypeAdminService>(
				delegate(IProcedureTypeAdminService service)
				{
					request.Id = _id;
					request.Name = _name;
					_response = service.ListProcedureTypes(request);
				});
			return _response.ProcedureTypes;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<ProcedureTypeSummary> addedItems)
		{
			addedItems = new List<ProcedureTypeSummary>();
			ProcedureTypeEditorComponent editor = new ProcedureTypeEditorComponent();
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddProcedureType);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.ProcedureTypeSummary);
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
		protected override bool EditItems(IList<ProcedureTypeSummary> items, out IList<ProcedureTypeSummary> editedItems)
		{
			editedItems = new List<ProcedureTypeSummary>();
			ProcedureTypeSummary item = CollectionUtils.FirstElement(items);

			ProcedureTypeEditorComponent editor = new ProcedureTypeEditorComponent(item.ProcedureTypeRef);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, string.Format("{0} - ({1}) {2}", SR.TitleUpdateProcedureType, item.Id, item.Name));
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.ProcedureTypeSummary);
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
		protected override bool DeleteItems(IList<ProcedureTypeSummary> items, out IList<ProcedureTypeSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<ProcedureTypeSummary>();

			foreach (ProcedureTypeSummary item in items)
			{
				try
				{
					Platform.GetService<IProcedureTypeAdminService>(
						delegate(IProcedureTypeAdminService service)
						{
							service.DeleteProcedureType(new DeleteProcedureTypeRequest(item.ProcedureTypeRef));
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
		protected override bool UpdateItemsActivation(IList<ProcedureTypeSummary> items, out IList<ProcedureTypeSummary> editedItems)
		{
			List<ProcedureTypeSummary> results = new List<ProcedureTypeSummary>();
			foreach (ProcedureTypeSummary item in items)
			{
				Platform.GetService<IProcedureTypeAdminService>(
					delegate(IProcedureTypeAdminService service)
					{
						ProcedureTypeDetail detail = service.LoadProcedureTypeForEdit(
							new LoadProcedureTypeForEditRequest(item.ProcedureTypeRef)).ProcedureType;
						detail.Deactivated = !detail.Deactivated;
						ProcedureTypeSummary summary = service.UpdateProcedureType(
							new UpdateProcedureTypeRequest(detail)).ProcedureType;

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
		protected override bool IsSameItem(ProcedureTypeSummary x, ProcedureTypeSummary y)
		{
			if (x != null && y != null)
			{
				return x.ProcedureTypeRef.Equals(y.ProcedureTypeRef, true);
			}
			return false;
		}
	}
}
