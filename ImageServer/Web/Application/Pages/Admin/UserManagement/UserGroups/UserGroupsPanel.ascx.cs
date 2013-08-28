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

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups
{

    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel", ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.js")]
    public partial class UserGroupsPanel : AJAXScriptControl
    {
        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteUserGroupButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string EditButtonClientID
        {
            get { return EditUserGroupButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("UserGroupsListClientID")]
        public string UserGroupsListClientID
        {
            get { return UserGroupsGridPanel.UserGroupGrid.ClientID; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GridPagerTop.InitializeGridPager(SR.GridPagerUserGroupsSingleItemFound,
                                             SR.GridPagerUserGroupsMultipleItemsFound,
                                             UserGroupsGridPanel.UserGroupGrid, 
                                             delegate
                                                 {
                                                     return UserGroupsGridPanel.ResultCount;
                                                 },
                                             ImageServerConstants.GridViewPagerPosition.Top);
            UserGroupsGridPanel.Pager = GridPagerTop;
            GridPagerTop.Reset();

            UserGroupsGridPanel.DataSourceCreated += delegate(UserGroupDataSource source)
                            {
                                source.GroupName = GroupName.TrimText;
                            };

        }

        public void UpdateUI()
        {
            UserGroupRowData userGroupRow = UserGroupsGridPanel.SelectedUserGroup;

            if (userGroupRow == null)
            {
                // no device being selected
                EditUserGroupButton.Enabled = false;
                DeleteUserGroupButton.Enabled = false;
            }
            else
            {
                EditUserGroupButton.Enabled = true;
                DeleteUserGroupButton.Enabled = true;
            }
            
            UserGroupsGridPanel.RefreshCurrentPage();
            SearchUpdatePanel.Update();
        }

        protected void AddUserGroupButton_Click(object sender, ImageClickEventArgs e)
        {
            ((Default)Page).OnAddUserGroup();
        }

        protected void EditUserGroupButton_Click(object sender, ImageClickEventArgs e)
        {
            UserGroupRowData userGroup = UserGroupsGridPanel.SelectedUserGroup;
            if (userGroup != null) ((Default)Page).OnEditUserGroup(userGroup);
        }

        protected void DeleteUserGroupButton_Click(object sender, ImageClickEventArgs e)
        {
            UserGroupRowData userGroup = UserGroupsGridPanel.SelectedUserGroup;
            if (userGroup != null) ((Default)Page).OnDeleteUserGroup(userGroup);
        }

    	protected void SearchButton_Click(object sender, ImageClickEventArgs e)
    	{
    		UserGroupsGridPanel.Refresh();
    	}
    }
}