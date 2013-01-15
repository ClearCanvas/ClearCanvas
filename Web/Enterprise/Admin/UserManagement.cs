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
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;

namespace ClearCanvas.Web.Enterprise.Admin
{
    /// <summary>
    /// Wrapper for <see cref="IUserAdminService"/> service.
    /// </summary>
    /// 
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public sealed class UserManagement : IDisposable
    {
        private IUserAdminService _service;

        public UserManagement()
        {
            _service = Platform.GetService<IUserAdminService>();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_service!=null && _service is IDisposable)
            {
                (_service as IDisposable).Dispose();
                _service = null;
            }
        }

        #endregion

        public UserDetail GetUserDetail(string name)
        {
            return _service.LoadUserForEdit(new LoadUserForEditRequest(name)).UserDetail;
        }


        public void AddUser(UserDetail user)
        {
            _service.AddUser(new AddUserRequest(user));
        }

        public void UpdateUserDetail(UserDetail user)
        {
            _service.UpdateUser(new UpdateUserRequest(user));
        }

        public void ResetPassword(string name)
        {
            _service.ResetUserPassword(new ResetUserPasswordRequest(name));
        }

        public void DeleteUser(string name)
        {
            _service.DeleteUser(new DeleteUserRequest(name));
        }

        public IList<UserSummary> FindUsers(ListUsersRequest filter)
        {
            List<UserSummary> users = _service.ListUsers(filter).Users;

            return users;
        }
    }
}