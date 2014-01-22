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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.Web.Enterprise.Admin;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	public class UserManagementController
	{
        public List<UserRowData> GetAllUsers()
        {
            List<UserRowData> data;
            
            using(UserManagement service = new UserManagement())
            {
                data = CollectionUtils.Map(
                    service.FindUsers(new ListUsersRequest()),
                    delegate(UserSummary summary)
                    {
                        UserRowData user = new UserRowData(summary, null);
                        return user;
                    });
            }

            return data;
        }

        public bool AddUser(UserRowData user)
        {
            bool success = false;

            using(UserManagement service = new UserManagement())
            {
                try
                {
                    UserDetail newUser = new UserDetail
                                             {
                                                 UserName = user.UserName,
                                                 DisplayName = user.DisplayName,
                                                 Enabled = user.Enabled,
                                                 CreationTime = Platform.Time,
                                                 PasswordExpiryTime = Platform.Time,
                                                 ResetPassword = true // TODO: Why do we need to reset password here?
                                             };


                    List<AuthorityGroupSummary> groups = new List<AuthorityGroupSummary>();

                    foreach (UserGroup userGroup in user.UserGroups)
                    {
                        groups.Add(new AuthorityGroupSummary(new EntityRef(userGroup.UserGroupRef), userGroup.Name,userGroup.Name, false, false));
                    }

                    newUser.AuthorityGroups = groups;
                    service.AddUser(newUser);
                    success = true;

                }
                catch (Exception ex)
                {
                	Platform.Log(LogLevel.Error, ex, "Unexpected exception adding user: {0}", user.DisplayName);
                }
            }

            return success;
        }

        public bool UpdateUser(UserRowData user)
        {
            bool success = false;

            using(UserManagement service = new UserManagement())
            {
                try
                {
                    UserDetail updateUser = new UserDetail
                                                {
                                                    UserName = user.UserName,
                                                    DisplayName = user.DisplayName,
                                                    EmailAddress = user.EmailAddress,
                                                    Enabled = user.Enabled
                                                };

                    List<AuthorityGroupSummary> groups = new List<AuthorityGroupSummary>();

                    foreach(UserGroup userGroup in user.UserGroups)
                    {
                        groups.Add(new AuthorityGroupSummary(new EntityRef(userGroup.UserGroupRef), userGroup.Name, userGroup.Name, false, false));
                    }

                    updateUser.AuthorityGroups = groups;

                    service.UpdateUserDetail(updateUser);
                    success = true;
                }
                catch (Exception ex)
                {
                	Platform.Log(LogLevel.Error, ex, "Unexpected exception updating user: {0}", user.DisplayName);
                }
            }

            return success;
        }

        public bool ExistsUsername(string username)
        {
            bool exists = false;

            using(UserManagement service = new UserManagement())
            {
                ListUsersRequest filter = new ListUsersRequest();
                filter.ExactMatchOnly = true;
                filter.UserName = username;

                IList<UserSummary> users = service.FindUsers(filter);

                if (users != null && users.Count > 0)
                {
                    exists = true;
                }
            }

            return exists;
        }

        public bool ResetPassword(UserRowData user)
        {
            bool success = false;

            using(UserManagement service = new UserManagement())
            {
                try
                {
                    service.ResetPassword(user.UserName);
                    success = true;
                }
                catch (Exception ex)
                {
                	Platform.Log(LogLevel.Error, ex, "Unexpected exception resetting password for user: {0}",
                	             user.DisplayName);
                }
            }

            return success;
        }

        public bool DeleteUser(UserRowData user)
        {
            bool success = false;

            using(UserManagement service = new UserManagement())
            {
                try
                {
                    service.DeleteUser(user.UserName);
                    success = true;
                }
                catch (Exception ex)
                {
					Platform.Log(LogLevel.Error, ex, "Unexpected exception deleting user: {0}",
								 user.DisplayName);
                }
            }

            return success;
        }

        public bool ExistsUsergroup(string usergroupName)
        {
            bool exists = false;

            using (AuthorityManagement service = new AuthorityManagement())
            {
                IList<AuthorityGroupSummary> list = service.ListAllAuthorityGroups();

            	if (usergroupName != null)
                {
                    foreach (AuthorityGroupSummary group in list)
                    {
                        if (group.Name.ToLower().Equals(usergroupName.ToLower()))
                        {
                            exists = true;
                            break;
                        }
                    }
                }
            }

            return exists;
        }

        public bool AddUserGroup(UserGroupRowData userGroup)
        {
            bool success;

            using(AuthorityManagement service = new AuthorityManagement())
            {
                List<AuthorityTokenSummary> tokens = new List<AuthorityTokenSummary>();

                foreach (TokenSummary token in userGroup.Tokens)
                {
                    tokens.Add(new AuthorityTokenSummary(token.Name, token.Description));
                }

                service.AddAuthorityGroup(userGroup.Name, userGroup.Description, userGroup.DataGroup, tokens);
                success = true;
            }

            //TODO: Catch exception?
            return success;
        }

        public bool UpdateUserGroup(UserGroupRowData userGroup)
        {
            bool success;

            using(AuthorityManagement service = new AuthorityManagement())
        
            {
                AuthorityGroupDetail detail = new AuthorityGroupDetail
                                                  {
                                                      AuthorityGroupRef = new EntityRef(userGroup.Ref),
                                                      Name = userGroup.Name,
                                                      Description = userGroup.Description,
                                                      DataGroup = userGroup.DataGroup
                                                  };

                foreach(TokenSummary token in userGroup.Tokens)
                {
                    detail.AuthorityTokens.Add(new AuthorityTokenSummary(token.Name, token.Description));
                }

                service.UpdateAuthorityGroup(detail, userGroup.Password);
                success = true;
            }

            //TODO: Catch exception?
            return success;
        }

        public void DeleteUserGroup(UserGroupRowData userGroup, bool checkIfGroupIsEmpty)
        {
            using (AuthorityManagement service = new AuthorityManagement())
            {
                try
                {
                    EntityRef entityRef = new EntityRef(userGroup.Ref);
                    service.DeleteAuthorityGroup(entityRef, checkIfGroupIsEmpty);
                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Unexpected exception deleting user group: {0}.", userGroup.Name);
                    throw;
                }
            }
        }

        public bool UpdateTokens(List<TokenRowData> tokens)
        {
            bool success;

            using(AuthorityManagement service = new AuthorityManagement())
            {
                   List<AuthorityTokenSummary> tokenList = new List<AuthorityTokenSummary>();

                   foreach(TokenRowData token in tokens)
                   {
                       tokenList.Add(new AuthorityTokenSummary(token.Name, token.Description));
                   }

                   service.ImportAuthorityTokens(tokenList);
                   success = true;
            }
            
            //TODO: Catch exception?
            return success;
        }
    }
}
