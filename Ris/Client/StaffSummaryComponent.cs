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
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;

namespace ClearCanvas.Ris.Client
{
    [MenuAction("launch", "global-menus/MenuAdmin/MenuStaff", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Staff)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
    public class StaffSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    StaffSummaryComponent component = new StaffSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleStaff);
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
    /// Extension point for views onto <see cref="StaffSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class StaffSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// StaffSummaryComponent class
    /// </summary>
    [AssociateView(typeof(StaffSummaryComponentViewExtensionPoint))]
    public class StaffSummaryComponent : SummaryComponentBase<StaffSummary, StaffTable, ListStaffRequest>
    {
        private string _firstName;
        private string _lastName;
        private readonly string[] _staffTypesFilter;


        /// <summary>
        /// Constructor
        /// </summary>
        public StaffSummaryComponent()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dialogMode">Indicates whether the component will be shown in a dialog box or not</param>
        public StaffSummaryComponent(bool dialogMode)
            :this(dialogMode, new string[]{})
        {
                
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dialogMode">Indicates whether the component will be shown in a dialog box or not</param>
        /// <param name="staffTypesFilter">Filters the staff list according to the specified staff types.</param>
        public StaffSummaryComponent(bool dialogMode, string[] staffTypesFilter)
			:base(dialogMode)
        {
            _staffTypesFilter = staffTypesFilter;
        }

        #region Presentation Model

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
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

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Staff);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Staff);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Staff);
			model.ToggleActivation.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Staff);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<StaffSummary> ListItems(ListStaffRequest request)
		{
			ListStaffResponse listResponse = null;
			Platform.GetService<IStaffAdminService>(
				delegate(IStaffAdminService service)
				{
					request.StaffTypesFilter = _staffTypesFilter;
					request.FamilyName = _lastName;
					request.GivenName = _firstName;
					listResponse = service.ListStaff(request);
				});

			return listResponse.Staffs;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<StaffSummary> addedItems)
		{
			addedItems = new List<StaffSummary>();
			StaffEditorComponent editor = new StaffEditorComponent();
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddStaff);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.StaffSummary);
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
		protected override bool EditItems(IList<StaffSummary> items, out IList<StaffSummary> editedItems)
		{
			editedItems = new List<StaffSummary>();
			StaffSummary item = CollectionUtils.FirstElement(items);

			StaffEditorComponent editor = new StaffEditorComponent(item.StaffRef);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleUpdateStaff + " - " + item.Name);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.StaffSummary);
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
		protected override bool DeleteItems(IList<StaffSummary> items, out IList<StaffSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<StaffSummary>();

			foreach (StaffSummary item in items)
			{
				try
				{
					Platform.GetService<IStaffAdminService>(
						delegate(IStaffAdminService service)
						{
							// check if staff has associated user account
							StaffDetail detail = service.LoadStaffForEdit(
								new LoadStaffForEditRequest(item.StaffRef)).StaffDetail;
							if (!string.IsNullOrEmpty(detail.UserName))
							{
								// ask if the account should be deleted too
								if(this.Host.ShowMessageBox(
									string.Format(SR.MessageConfirmDeleteAssociatedUserAccount, detail.UserName), MessageBoxActions.YesNo)
									== DialogBoxAction.Yes)
								{
									Platform.GetService<IUserAdminService>(
										delegate(IUserAdminService userAdminService)
										{
											userAdminService.DeleteUser(new DeleteUserRequest(detail.UserName));
										});
								}
								else
								{
									// not deleting user, but we should update user's display name
									Platform.GetService<IUserAdminService>(
										delegate(IUserAdminService userAdminService)
										{
											LoadUserForEditResponse editResponse = userAdminService.LoadUserForEdit(new LoadUserForEditRequest(detail.UserName));
											UserDetail userDetail = editResponse.UserDetail;
											userDetail.DisplayName = null;
											
											userAdminService.UpdateUser(new UpdateUserRequest(userDetail));
										});
								}
							}
							service.DeleteStaff(new DeleteStaffRequest(item.StaffRef));
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
		protected override bool UpdateItemsActivation(IList<StaffSummary> items, out IList<StaffSummary> editedItems)
		{
			List<StaffSummary> results = new List<StaffSummary>();
			foreach (StaffSummary item in items)
			{
				Platform.GetService<IStaffAdminService>(
					delegate(IStaffAdminService service)
					{
						StaffDetail detail = service.LoadStaffForEdit(
							new LoadStaffForEditRequest(item.StaffRef)).StaffDetail;
						detail.Deactivated = !detail.Deactivated;
						StaffSummary summary = service.UpdateStaff(
							new UpdateStaffRequest(detail)).Staff;

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
		protected override bool IsSameItem(StaffSummary x, StaffSummary y)
		{
			return x.StaffRef.Equals(y.StaffRef, true);
		}
	}
}
