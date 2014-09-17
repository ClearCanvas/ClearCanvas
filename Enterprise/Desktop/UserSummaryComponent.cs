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
using Action=ClearCanvas.Desktop.Actions.Action;

namespace ClearCanvas.Enterprise.Desktop
{
    [MenuAction("launch", "global-menus/MenuAdmin/MenuUsers", "Launch")]
    [ActionPermission("launch", ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.User)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class UserSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
					if (Application.SessionStatus != SessionStatus.Online)
					{
						Context.DesktopWindow.ShowMessageBox(SR.MessageServerOffline, MessageBoxActions.Ok);
						return;
					}

                    UserSummaryComponent component = new UserSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleUser);
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
	/// Extension point for views onto <see cref="UserSummaryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class UserSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

    /// <summary>
    /// UserSummaryComponent class
    /// </summary>
	[AssociateView(typeof(UserSummaryComponentViewExtensionPoint))]
    public class UserSummaryComponent : SummaryComponentBase<UserSummary, UserTable>
    {
        private Action _resetPasswordAction;
        private Action _viewSessionsAction;
		private string _id;
		private string _name;

		public UserSummaryComponent()
			: base(false)
		{
		}

		public UserSummaryComponent(bool dialogMode)
			: base(dialogMode)
		{
		}

        #region Presentation Model

        public void ViewUserSessions()
        {
            try
            {
                UserSummary selectedUser = CollectionUtils.FirstElement(this.SelectedItems);
                if (selectedUser == null) return;

                var component = new UserSessionManagmentComponent(selectedUser);
                LaunchAsDialog(this.Host.DesktopWindow, component, SR.TitleUserSessions);
            }
            catch (Exception e)
            {
                // failed
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void ResetUserPassword()
        {
            try
            {
            	UserSummary selectedUser = CollectionUtils.FirstElement(this.SelectedItems);
				if (selectedUser == null) return;

                // confirm this action
				if (this.Host.ShowMessageBox(string.Format(SR.FormatMessageResetPassword, selectedUser.UserName), MessageBoxActions.OkCancel) == DialogBoxAction.Cancel)
                    return;

                Platform.GetService<IUserAdminService>(
                    delegate(IUserAdminService service)
                    {
						ResetUserPasswordResponse response = service.ResetUserPassword(new ResetUserPasswordRequest(selectedUser.UserName));
                        this.Table.Items.Replace(delegate(UserSummary u) { return u.UserName == response.UserSummary.UserName; },
                           response.UserSummary);
                    });
            }
            catch (Exception e)
            {
                // failed
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

		public string ID
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

			_resetPasswordAction = model.AddAction("resetPassword", SR.TitleResetPassword, "Icons.ResetToolSmall.png",
				SR.TitleResetPassword, ResetUserPassword, ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.User);

            _viewSessionsAction = model.AddAction("viewSessions", SR.LabelViewSessions, "Icons.ViewUserSessionsSmall.png",
               SR.TitleViewUserSessions, ViewUserSessions, ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.User);

			model.Add.SetPermissibility(ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.User);
			model.Edit.SetPermissibility(ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.User);
			model.Delete.SetPermissibility(ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.User);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<UserSummary> ListItems(int firstRow, int maxRows)
		{
			ListUsersRequest request = new ListUsersRequest();
			request.Page.FirstRow = firstRow;
			request.Page.MaxRows = maxRows;

			ListUsersResponse listResponse = null;
			Platform.GetService<IUserAdminService>(
				delegate(IUserAdminService service)
				{
					request.UserName = _id;
					request.DisplayName = _name;
					listResponse = service.ListUsers(request);
				});

			return listResponse.Users;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<UserSummary> addedItems)
		{
			addedItems = new List<UserSummary>();
			UserEditorComponent editor = new UserEditorComponent();
			ApplicationComponentExitCode exitCode = LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddUser);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.UserSummary);
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
		protected override bool EditItems(IList<UserSummary> items, out IList<UserSummary> editedItems)
		{
			editedItems = new List<UserSummary>();
			UserSummary item = CollectionUtils.FirstElement(items);

			UserEditorComponent editor = new UserEditorComponent(item.UserName);
			ApplicationComponentExitCode exitCode = LaunchAsDialog(
				this.Host.DesktopWindow, editor, string.Format(SR.FormatTitleSubtitle, SR.TitleUpdateUser, item.UserName));
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.UserSummary);
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
		protected override bool DeleteItems(IList<UserSummary> items, out IList<UserSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<UserSummary>();

			foreach (UserSummary item in items)
			{
				try
				{

					// delete user account
					Platform.GetService<IUserAdminService>(
						delegate(IUserAdminService service)
						{
							service.DeleteUser(new DeleteUserRequest(item.UserName));
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
		/// Compares two items to see if they represent the same item.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override bool IsSameItem(UserSummary x, UserSummary y)
		{
			return x.UserName.Equals(y.UserName);
		}

    	/// <summary>
    	/// Called when the user changes the selected items in the table.
    	/// </summary>
    	protected override void OnSelectedItemsChanged()
		{
			base.OnSelectedItemsChanged();

			_resetPasswordAction.Enabled = this.SelectedItems.Count == 1;
		}
	}
}
