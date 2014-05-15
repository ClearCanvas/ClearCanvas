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
using System.Collections;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.LocationAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/MenuAdmin/MenuLocations", "Launch")]
    [VisibleStateObserver("launch", "Visible")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Location)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
    public class LocationSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

     	public bool Visible
    	{
			get { return new WorkflowConfigurationReader().EnableVisitWorkflow; }
    	}

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    LocationSummaryComponent component = new LocationSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleLocations);
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
    /// Extension point for views onto <see cref="LocationSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class LocationSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// LocationSummaryComponent class
    /// </summary>
	[AssociateView(typeof(LocationSummaryComponentViewExtensionPoint))]
    public class LocationSummaryComponent : SummaryComponentBase<LocationSummary, LocationTable, ListAllLocationsRequest>
    {
		private FacilitySummary _filterNone = new FacilitySummary();
		private ArrayList _facilityList = new ArrayList();
		private FacilitySummary _facility;
		private string _name;

		public override void Start()
		{
			Platform.GetService<ILocationAdminService>(
				delegate(ILocationAdminService service)
				{
					GetLocationEditFormDataResponse response = service.GetLocationEditFormData(new GetLocationEditFormDataRequest());
					_filterNone.Name = SR.DummyItemNone;
					_facilityList.Add(_filterNone);
					response.FacilityChoices.Sort(delegate(FacilitySummary x, FacilitySummary y) { return x.Name.CompareTo(y.Name); });
					_facilityList.AddRange(response.FacilityChoices);
				});
			base.Start();
			_facility = _filterNone;
		}

		# region Presentation Model

		public object NullFilter
		{
			get { return _filterNone; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public object Facility
		{
			get {return _facility; }
			set
			{
				if (value == _filterNone)
					_facility = null;
				else
					_facility = (FacilitySummary)value;

				if (value == _filterNone)
					_facility = _filterNone;
			}
		}

		public IList FacilityChoices
		{
			get
			{
				return _facilityList;
			}
		}

		public string FormatFacilityListItem(object item)
		{
			FacilitySummary summary = (FacilitySummary)item;
			return string.Format(summary.Name);
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

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Location);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Location);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Location);
			model.ToggleActivation.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Location);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<LocationSummary> ListItems(ListAllLocationsRequest request)
		{
			ListAllLocationsResponse listResponse = null;
			Platform.GetService<ILocationAdminService>(
				delegate(ILocationAdminService service)
				{
					if(_facility != _filterNone && _facility != null)
						request.Facility = _facility;
					request.Name = _name;
					listResponse = service.ListAllLocations(request);
				});

			return listResponse.Locations;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<LocationSummary> addedItems)
		{
			addedItems = new List<LocationSummary>();
			LocationEditorComponent editor = new LocationEditorComponent();
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddLocation);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.LocationSummary);
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
		protected override bool EditItems(IList<LocationSummary> items, out IList<LocationSummary> editedItems)
		{
			editedItems = new List<LocationSummary>();
			LocationSummary item = CollectionUtils.FirstElement(items);

			LocationEditorComponent editor = new LocationEditorComponent(item.LocationRef);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, string.Format(SR.FormatTitleCodeSubtitle, SR.TitleUpdateLocation, item.Id, item.Name));
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.LocationSummary);
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
		protected override bool DeleteItems(IList<LocationSummary> items, out IList<LocationSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<LocationSummary>();

			foreach (LocationSummary item in items)
			{
				try
				{
					Platform.GetService<ILocationAdminService>(
						delegate(ILocationAdminService service)
						{
							service.DeleteLocation(new DeleteLocationRequest(item.LocationRef));
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
		protected override bool UpdateItemsActivation(IList<LocationSummary> items, out IList<LocationSummary> editedItems)
		{
			List<LocationSummary> results = new List<LocationSummary>();
			foreach (LocationSummary item in items)
			{
				Platform.GetService<ILocationAdminService>(
					delegate(ILocationAdminService service)
					{
						LocationDetail detail = service.LoadLocationForEdit(
							new LoadLocationForEditRequest(item.LocationRef)).LocationDetail;
						detail.Deactivated = !detail.Deactivated;
						LocationSummary summary = service.UpdateLocation(
							new UpdateLocationRequest(detail)).Location;

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
		protected override bool IsSameItem(LocationSummary x, LocationSummary y)
		{
			return x.LocationRef.Equals(y.LocationRef, true);
		}

    }
}
