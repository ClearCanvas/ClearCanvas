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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Web.Enterprise.Admin;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
    public class UserDataSource : BaseDataSource
    {
        #region Private Members

    	private int _resultCount;

        #endregion Private Members

        #region Public Members
        public delegate void UserFoundSetDelegate(IList<UserRowData> list);
        public UserFoundSetDelegate UserFoundSet;
        #endregion Public Members

        #region Properties
        public int ResultCount
        {
            get { return _resultCount; }
            set { _resultCount = value; }
        }

        public string DisplayName { get; set; }

        public string UserName { get; set; }

        #endregion

        #region Private Methods

        private IList<UserRowData> InternalSelect(int startRowIndex, int maximumRows, out int resultCount)
        {
            if (maximumRows == 0)
            {
                resultCount = 0;
                return new List<UserRowData>();
            }

            List<UserRowData> users;
            using (var service = new UserManagement())
            {
                var filter = new ListUsersRequest
                                 {
                                     UserName = UserName.Replace("*", "%").Replace("?", "_"),
                                     DisplayName = DisplayName.Replace("*", "%").Replace("?", "_"),
                                     Page = {FirstRow = startRowIndex},
                                     ExactMatchOnly = false
                                 };

                users = CollectionUtils.Map(
                    service.FindUsers(filter),
                    (UserSummary summary) => new UserRowData(summary, service.GetUserDetail(summary.UserName)));
            }
            resultCount = users.Count;

            return users;
        }

        #endregion Private Methods

        #region Public Methods
        
        public IEnumerable<UserRowData> Select(int startRowIndex, int maximumRows)
        {
            IList<UserRowData> list = InternalSelect(startRowIndex, maximumRows, out _resultCount);

            if (UserFoundSet != null)
                UserFoundSet(list);

            return list;

        }

        public int SelectCount()
        {
            // Ignore the search results
            InternalSelect(0, 1, out _resultCount);

            return ResultCount;
        }

        #endregion Public Methods
    }

    [Serializable]
    public class UserRowData
    {
        public List<UserGroup> UserGroups { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string EmailAddress
        {
            get; set;
        }

        public bool Enabled { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public UserRowData(UserSummary summary, UserDetail details)
        {
            UserGroups = new List<UserGroup>();
            UserName = summary.UserName;
            DisplayName = summary.DisplayName;
            Enabled = summary.Enabled;
            LastLoginTime = summary.LastLoginTime;
            EmailAddress = summary.EmailAddress;

            if (details!=null)
            {
                foreach (AuthorityGroupSummary authorityGroup in details.AuthorityGroups)
                {
                    UserGroups.Add(new UserGroup(
                            authorityGroup.AuthorityGroupRef.Serialize(), authorityGroup.Name));
                }
            }            
        }

        public UserRowData()
        {
            UserGroups = new List<UserGroup>();
        }
    }

    [Serializable]
    public class UserGroup
    {
        private string _authorityGroupRef;
        private string _name;
        
        public UserGroup(string authorityGroupRef, string name)
        {
            _authorityGroupRef = authorityGroupRef;
            _name = name;
        }

        public string UserGroupRef
        {
            get { return _authorityGroupRef;  }
            set { _authorityGroupRef = value;  }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }       
    }
}