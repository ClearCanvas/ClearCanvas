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
using System.IO;
using System.Security;
using System.Text;
using System.Web.Hosting;
using System.Web.Security;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.Authentication;

namespace ClearCanvas.ImageServer.Web.Common.Security
{
    class DefaultRoleProvider:RoleProvider
    {
        private string[] _allTokens;
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            _allTokens = new string[]
                {
                    AuthorityTokens.Admin.Alert.Delete,
                    AuthorityTokens.Admin.Alert.View,
                    AuthorityTokens.Admin.ApplicationLog.Search,
                    AuthorityTokens.Admin.Configuration.Devices,
                    AuthorityTokens.Admin.Configuration.FileSystems,
                    AuthorityTokens.Admin.Configuration.PartitionArchive,
                    AuthorityTokens.Admin.Configuration.ServerPartitions,
                    AuthorityTokens.Admin.Configuration.ServerRules,
                    AuthorityTokens.Admin.Configuration.ServiceScheduling,
                    AuthorityTokens.Admin.StudyDeleteHistory.Delete,
                    AuthorityTokens.Admin.StudyDeleteHistory.Search,
                    AuthorityTokens.Admin.StudyDeleteHistory.View,
                    AuthorityTokens.Admin.Dashboard.View,
                    AuthorityTokens.ArchiveQueue.Delete,
                    AuthorityTokens.ArchiveQueue.Search,
                    AuthorityTokens.RestoreQueue.Delete,
                    AuthorityTokens.RestoreQueue.Search,
                    AuthorityTokens.Study.Delete,
                    AuthorityTokens.Study.Edit,
                    AuthorityTokens.Study.Move,
                    AuthorityTokens.Study.Restore,
                    AuthorityTokens.Study.Search,
                    AuthorityTokens.Study.View,
                    AuthorityTokens.Study.Reprocess,
                    AuthorityTokens.Study.SaveReason,
                    AuthorityTokens.StudyIntegrityQueue.Reconcile,
                    AuthorityTokens.StudyIntegrityQueue.Search,
                    AuthorityTokens.ExternalRequestQueue.Search,
                    AuthorityTokens.ExternalRequestQueue.Delete,
                    AuthorityTokens.WorkQueue.Delete,
                    AuthorityTokens.WorkQueue.Reprocess,
                    AuthorityTokens.WorkQueue.Reschedule,
                    AuthorityTokens.WorkQueue.Reset,
                    AuthorityTokens.WorkQueue.Search,
                    AuthorityTokens.WorkQueue.View,
                    ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllStudies,
                    ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllPartitions,                    
                };

        }
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string ApplicationName
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string[] GetAllRoles()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string[] GetRolesForUser(string username)
        {
            return _allTokens;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool RoleExists(string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}