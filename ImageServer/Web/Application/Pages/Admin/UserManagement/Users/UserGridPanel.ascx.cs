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
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using GridView=ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users
{
    public partial class UserGridPanel : GridViewPanel
    {
        #region Delegates
        public delegate void UserDataSourceCreated(UserDataSource theSource);
        public event UserDataSourceCreated DataSourceCreated;
        public delegate void UserSelectedEventHandler(object sender, UserRowData selectedUser);
        public event UserSelectedEventHandler OnUserSelectionChanged;
        #endregion

        #region Private members
        // list of studies to display
        private UserDataSource _dataSource;
        private IList<UserRowData> _userRows;
        #endregion Private members
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UserGridView.SelectedIndexChanged += UserGridView_SelectedIndexChanged;

            UserGridView.DataSource = UserDataSourceObject;

            TheGrid = UserGridView;
        }

        /// <summary>
        /// Retrieve reference to the grid control being used to display the devices.
        /// </summary>
        public GridView UserGrid
        {
            get { return UserGridView; }
        }

        public int ResultCount
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new UserDataSource();

                    _dataSource.UserFoundSet += delegate(IList<UserRowData> newlist)
                                            {
                                                _userRows = newlist;
                                            };
                    if (DataSourceCreated != null)
                        DataSourceCreated(_dataSource);
                    _dataSource.SelectCount();
                }
                if (_dataSource.ResultCount == 0)
                {
                    if (DataSourceCreated != null)
                        DataSourceCreated(_dataSource);

                    _dataSource.SelectCount();
                }
                return _dataSource.ResultCount;
            }
        }

        private static void CustomizeUserGroupsColumn(GridViewRowEventArgs e)
        {
            TextBox textBox = ((TextBox)e.Row.FindControl("UserGroupTextBox"));
            UserRowData rowData = e.Row.DataItem as UserRowData;
            string groupList = string.Empty;
            foreach (UserGroup userGroup in rowData.UserGroups)
            {
                groupList += userGroup.Name + "\n";
            }
            textBox.Text = groupList;
        }

        public UserRowData SelectedUser
        {
            get
            {
                if(!UserGrid.IsDataBound) UserGrid.DataBind();

                if (Users.Count == 0 || UserGrid.SelectedIndex < 0)
                    return null;

                int index = UserGrid.SelectedIndex;

                if (index < 0 || index > Users.Count - 1)
                    return null;

                return Users[index];
            }
            set
            {                
                UserGrid.SelectedIndex = Users.IndexOf(value);
                if (OnUserSelectionChanged != null)
                    OnUserSelectionChanged(this, value);
            }
        }

        public IList<UserRowData> Users
        {
            get { return _userRows; }
            set
            {
                _userRows = value;
                UserGrid.DataSource = _userRows; // must manually call DataBind() later
            }
        }

        protected void UserGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserRowData userRow = SelectedUser;
            if (userRow != null)
                if (OnUserSelectionChanged != null)
                    OnUserSelectionChanged(this, userRow);

            DataBind();
        }

        protected void UserGridView_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void UserGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            UserGridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected void UserGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (UserGridView.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    CustomizeUserGroupsColumn(e);
                }
            }
        }

        protected void DisposeUserDataSource(object sender, ObjectDataSourceDisposingEventArgs e)
        {
            e.Cancel = true;
        }

        protected void GetUserDataSource(object sender, ObjectDataSourceEventArgs e)
        {
            if (_dataSource == null)
            {
                _dataSource = new UserDataSource();

                _dataSource.UserFoundSet += delegate(IList<UserRowData> newlist)
                                        {
                                            _userRows = newlist;
                                        };
            }

            e.ObjectInstance = _dataSource;

            if (DataSourceCreated != null)
                DataSourceCreated(_dataSource);

        }
    }
}
