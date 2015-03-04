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
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
	[MenuAction("launch", "global-menus/MenuAdmin/MenuFacilities", "Launch")]
	[ActionPermission("launch", Application.Common.AuthorityTokens.Admin.Data.Facility)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class FacilitySummaryTool : Tool<IDesktopToolContext>
	{
		private IWorkspace _workspace;

		public void Launch()
		{
			if (_workspace == null)
			{
				try
				{
					var component = new FacilitySummaryComponent();

					_workspace = ApplicationComponent.LaunchAsWorkspace(
						this.Context.DesktopWindow,
						component,
						SR.TitleFacilities);
					_workspace.Closed += delegate { _workspace = null; };

				}
				catch (Exception e)
				{
					// could not launch component
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
	/// Extension point for views onto <see cref="FacilitySummaryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class FacilitySummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// FacilitySummaryComponent class
	/// </summary>
	public class FacilitySummaryComponent : SummaryComponentBase<FacilitySummary, FacilityTable, ListAllFacilitiesRequest>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public FacilitySummaryComponent()
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

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Facility);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Facility);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Facility);
			model.ToggleActivation.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Facility);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<FacilitySummary> ListItems(ListAllFacilitiesRequest request)
		{
			ListAllFacilitiesResponse listResponse = null;
			Platform.GetService(
				delegate(IFacilityAdminService service)
				{
					listResponse = service.ListAllFacilities(request);
				});

			return listResponse.Facilities;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<FacilitySummary> addedItems)
		{
			addedItems = new List<FacilitySummary>();
			var editor = new FacilityEditorComponent();
			if (ApplicationComponentExitCode.Accepted == LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddFacility))
			{
				addedItems.Add(editor.FacilitySummary);
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
		protected override bool EditItems(IList<FacilitySummary> items, out IList<FacilitySummary> editedItems)
		{
			editedItems = new List<FacilitySummary>();
			var item = CollectionUtils.FirstElement(items);
			var editor = new FacilityEditorComponent(item.FacilityRef);
			if (ApplicationComponentExitCode.Accepted == 
				LaunchAsDialog(this.Host.DesktopWindow, editor, string.Format(SR.FormatTitleSubtitle, SR.TitleUpdateFacility, item.Name)))
			{
				editedItems.Add(editor.FacilitySummary);
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
		protected override bool DeleteItems(IList<FacilitySummary> items, out IList<FacilitySummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<FacilitySummary>();

			foreach (var item in items)
			{
				try
				{
					Platform.GetService<IFacilityAdminService>(
						service => service.DeleteFacility(new DeleteFacilityRequest(item.FacilityRef)));

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
		protected override bool UpdateItemsActivation(IList<FacilitySummary> items, out IList<FacilitySummary> editedItems)
		{
			var results = new List<FacilitySummary>();
			foreach (var item in items)
			{
				Platform.GetService(
					delegate(IFacilityAdminService service)
					{
						var detail = service.LoadFacilityForEdit(new LoadFacilityForEditRequest(item.FacilityRef)).FacilityDetail;
						detail.Deactivated = !detail.Deactivated;
						var summary = service.UpdateFacility(new UpdateFacilityRequest(detail)).Facility;

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
		protected override bool IsSameItem(FacilitySummary x, FacilitySummary y)
		{
			return x.FacilityRef.Equals(y.FacilityRef, true);
		}
	}
}
