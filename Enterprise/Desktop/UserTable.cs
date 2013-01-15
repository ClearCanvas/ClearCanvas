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

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;

namespace ClearCanvas.Enterprise.Desktop
{
    public class UserTable : Table<UserSummary>
    {
        public UserTable()
        {
            Columns.Add(new TableColumn<UserSummary, string>(SR.ColumnUserId,
                                                             user => user.UserName,
                                                             0.5f));

            Columns.Add(new TableColumn<UserSummary, string>(SR.ColumnUserName,
                                                             user => user.DisplayName,
                                                             1.0f));

            Columns.Add(new TableColumn<UserSummary, string>(SR.ColumnEmailAddress,
                                                             user => user.EmailAddress,
                                                             1.0f));

            Columns.Add(new DateTimeTableColumn<UserSummary>(SR.ColumnCreatedOn,
                                                             user => user.CreationTime,
                                                             0.75f));

            Columns.Add(new TableColumn<UserSummary, bool>(SR.ColumnEnabled,
                                                           user => user.Enabled,
                                                           0.25f));

            Columns.Add(new DateTimeTableColumn<UserSummary>(SR.ColumnValidFrom,
                                                             user => user.ValidFrom,
                                                             0.75f));

            Columns.Add(new DateTimeTableColumn<UserSummary>(SR.ColumnValidUntil,
                                                             user => user.ValidUntil,
                                                             0.75f));

            Columns.Add(new DateTimeTableColumn<UserSummary>(SR.ColumnPasswordExpiry,
                                                             user => user.PasswordExpiry,
                                                             0.75f));

            Columns.Add(new TableColumn<UserSummary,int>(SR.ColumnSessionCount,
                                                             user => user.SessionCount,
                                                             0.3f));

            Columns.Add(new DateTimeTableColumn<UserSummary>(SR.ColumnLastLoginTime,
                                                             user => user.LastLoginTime,
                                                             0.75f));
        }
    }
}
