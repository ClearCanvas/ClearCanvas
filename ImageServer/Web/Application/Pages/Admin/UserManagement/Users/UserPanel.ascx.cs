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
using System.Web.UI;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using Resources;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users
{
    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel", ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel.js")]
    public partial class UserPanel : AJAXScriptControl
    {
        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteUserButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string EditButtonClientID
        {
            get { return EditUserButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ResetPasswordButtonClientID")]
        public string ResetPasswordButtonClientID
        {
            get { return ResetPasswordButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("UserListClientID")]
        public string UserListClientID
        {
            get { return UserGridPanel.UserGrid.ClientID; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GridPagerTop.InitializeGridPager(SR.GridPagerUserSingleItemFound,
                                             SR.GridPagerUserMultipleItemsFound,
                                             UserGridPanel.UserGrid, 
                                             delegate
                                                 {
                                                     return UserGridPanel.ResultCount;
                                                 },
                                             ImageServerConstants.GridViewPagerPosition.Top);
            UserGridPanel.Pager = GridPagerTop;
            GridPagerTop.Reset();

            UserGridPanel.DataSourceCreated += delegate(UserDataSource source)
                            {
                                source.UserName = UserNameTextBox.TrimText;
                                source.DisplayName = DisplayNameTextBox.TrimText;
                            };

            
        }

        public void UpdateUI()
        {
            UserRowData userRow = UserGridPanel.SelectedUser;

            if (userRow == null)
            {
                // no device being selected
                EditUserButton.Enabled = false;
                DeleteUserButton.Enabled = false;
                ResetPasswordButton.Enabled = false;
            }
            else
            {
                EditUserButton.Enabled = true;
                DeleteUserButton.Enabled = true;
                ResetPasswordButton.Enabled = true;
            }

            // UpdatePanel UpdateMode must be set to "conditional"
            // Calling UpdatePanel.Update() will force the client to refresh the screen
            UserGridPanel.RefreshCurrentPage();
            SearchUpdatePanel.Update();
        }

        protected void AddUserButton_Click(object sender, ImageClickEventArgs e)
        {             
            ((Default)Page).OnAddUser();
        }

        protected void EditUserButton_Click(object sender, ImageClickEventArgs e)
        {
            UserRowData user = UserGridPanel.SelectedUser;
            if (user != null) ((Default)Page).OnEditUser(user);
        }

        protected void DeleteUserButton_Click(object sender, ImageClickEventArgs e)
        {            
            UserRowData user = UserGridPanel.SelectedUser;
            if (user != null) ((Default)Page).OnDeleteUser(user);
        }

        protected void ResetPasswordButton_Click(object sender, ImageClickEventArgs e)
        {
            UserRowData user = UserGridPanel.SelectedUser;
            if (user != null) ((Default)Page).OnResetPassword(user);
        }

    	protected void SearchButton_Click(object sender, ImageClickEventArgs e)
    	{
    		UserGridPanel.Refresh();
    	}
    }
}