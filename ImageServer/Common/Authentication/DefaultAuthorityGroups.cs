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
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Common.Authentication
{
    class UserGroupDefinitionAttribute : Attribute
    {
        public string Name;
    }

    enum DefaultUserGroup
    {
        [UserGroupDefinition(Name = "PACS Administrators")]
        PACSAdministrators,

        [UserGroupDefinition(Name = "PACS Technologists")]
        PACSTechnologists,

        [UserGroupDefinition(Name = "PACS Read-only Users")]
        PACSUsers
    }

    [ExtensionOf(typeof(DefineAuthorityGroupsExtensionPoint))]
    public class DefaultAuthorityGroups : IDefineAuthorityGroups
    {
        static string GetGroupName(DefaultUserGroup group)
        {
            string fieldName = Enum.GetName(typeof(DefaultUserGroup), group);
            FieldInfo field = typeof(DefaultUserGroup).GetField(fieldName);
            UserGroupDefinitionAttribute definition = AttributeUtils.GetAttribute<UserGroupDefinitionAttribute>(field);
            return definition.Name;
        }

        #region IDefineAuthorityGroups Members

        public AuthorityGroupDefinition[] GetAuthorityGroups()
        {
            //TODO: Load from XML instead

            AuthorityGroupDefinition admins = new AuthorityGroupDefinition(
                GetGroupName(DefaultUserGroup.PACSAdministrators),
                new string[]
                    {
                        #region Tokens
                        ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.User,
                        ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.AuthorityGroup,
                        AuthorityTokens.Admin.Alert.Delete,
                        AuthorityTokens.Admin.Alert.View,
                        AuthorityTokens.Admin.ApplicationLog.Search,
                        AuthorityTokens.Admin.Configuration.Devices,
                        AuthorityTokens.Admin.Configuration.FileSystems,
                        AuthorityTokens.Admin.Configuration.ServerPartitions,
                        AuthorityTokens.Admin.Configuration.ServerRules,
                        AuthorityTokens.Admin.Configuration.ServiceScheduling,
                        AuthorityTokens.Admin.Configuration.PartitionArchive,

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
                        AuthorityTokens.Study.ViewImages,
                        AuthorityTokens.Study.Reprocess,

                        AuthorityTokens.StudyIntegrityQueue.Search,
                        AuthorityTokens.StudyIntegrityQueue.Reconcile,

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
                        #endregion
                    });

            AuthorityGroupDefinition technologists = new AuthorityGroupDefinition(
                GetGroupName(DefaultUserGroup.PACSTechnologists),
                new string[]
                    {
                        #region Tokens
                        //AuthorityTokens.Admin.Alert.Delete,
                        //AuthorityTokens.Admin.Alert.View,
                        //AuthorityTokens.Admin.ApplicationLog.Search,
                        //AuthorityTokens.Admin.Configuration.Devices,
                        //AuthorityTokens.Admin.Configuration.FileSystems,
                        //AuthorityTokens.Admin.Configuration.ServerPartitions,
                        //AuthorityTokens.Admin.Configuration.ServerRules,
                        //AuthorityTokens.Admin.Configuration.ServiceScheduling,

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
                        AuthorityTokens.Study.ViewImages,
                        AuthorityTokens.Study.Reprocess,

                        AuthorityTokens.StudyIntegrityQueue.Search,
                        AuthorityTokens.StudyIntegrityQueue.Reconcile,

                        AuthorityTokens.WorkQueue.Delete,
                        AuthorityTokens.WorkQueue.Reprocess,
                        AuthorityTokens.WorkQueue.Reschedule,
                        AuthorityTokens.WorkQueue.Reset,
                        AuthorityTokens.WorkQueue.Search,
                        AuthorityTokens.WorkQueue.View,

                        ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllStudies,
                        ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllPartitions,
                        #endregion
                    });

            AuthorityGroupDefinition users = new AuthorityGroupDefinition(
                GetGroupName(DefaultUserGroup.PACSUsers),
                new string[]
                    {
                        #region Tokens
                        //AuthorityTokens.Admin.Alert.Delete,
                        //AuthorityTokens.Admin.Alert.View,
                        //AuthorityTokens.Admin.ApplicationLog.Search,
                        //AuthorityTokens.Admin.Configuration.Devices,
                        //AuthorityTokens.Admin.Configuration.FileSystems,
                        //AuthorityTokens.Admin.Configuration.ServerPartitions,
                        //AuthorityTokens.Admin.Configuration.ServerRules,
                        //AuthorityTokens.Admin.Configuration.ServiceScheduling,

                        //AuthorityTokens.ArchiveQueue.Delete,
                        AuthorityTokens.ArchiveQueue.Search,

                        //AuthorityTokens.RestoreQueue.Delete,
                        AuthorityTokens.RestoreQueue.Search,

                        //AuthorityTokens.Study.Delete,
                        //AuthorityTokens.Study.Edit,
                        //AuthorityTokens.Study.Move,
                        //AuthorityTokens.Study.Restore,
                        AuthorityTokens.Study.Search,
                        AuthorityTokens.Study.View,

                        AuthorityTokens.StudyIntegrityQueue.Search,
                        //AuthorityTokens.StudyIntegrityQueue.Reconcile,

                        //AuthorityTokens.WorkQueue.Delete,
                        //AuthorityTokens.WorkQueue.Reprocess,
                        //AuthorityTokens.WorkQueue.Reschedule,
                        //AuthorityTokens.WorkQueue.Reset,
                        AuthorityTokens.WorkQueue.Search,
                        AuthorityTokens.WorkQueue.View,

                        ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllStudies,
                        ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllPartitions,
                        #endregion
                    });


            return new AuthorityGroupDefinition[] { admins, technologists, users };
        }

        #endregion
    }
}