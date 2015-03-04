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
using System.Web;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.Web.Enterprise.Admin;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class StudyDataAccessController
    {

        #region Public Methods

        public DataAccessGroup AddDataAccessIfNotExists(string oid)
        {
            DataAccessGroup theGroup = FindDataAccessGroup(oid);
            if (theGroup == null)
            {
                using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    DataAccessGroupUpdateColumns insert = new DataAccessGroupUpdateColumns
                                                              {
                                                                  AuthorityGroupOID =
                                                                      new ServerEntityKey("AuthorityGroupOID",
                                                                                          new Guid(oid)),
                                                                  Deleted = false
                                                              };
                    IDataAccessGroupEntityBroker broker = update.GetBroker<IDataAccessGroupEntityBroker>();
                    theGroup = broker.Insert(insert);
                    update.Commit();
                }
            }
            return theGroup;
        }

        /// <summary>
        /// Returns all data-access groups that have access to the study not through direct data access assignment. 
        /// <remarks>Use <seealso cref="ListAuthorityGroupsForStudyViaToken"/> to get groups which can access the study through other means</remarks>
        /// </summary>
        /// <param name="studyStorageKey"></param>
        /// <returns></returns>
        public IList<AuthorityGroupStudyAccessInfo> ListDataAccessGroupsForStudy(ServerEntityKey studyStorageKey)
        {
            List<AuthorityGroupDetail> nonAddedAuthorityGroups;
            Dictionary<ServerEntityKey, AuthorityGroupDetail> dictionary = LoadAuthorityGroups(out nonAddedAuthorityGroups);
            return ListDataAccessGroupsForStudy(dictionary, studyStorageKey);
        }

        /// <summary>
        /// Returns all authority groups (data-access or not) 
        /// that have access to the study not through direct data access assignment but through other means, such as administrative tokens
        /// </summary>
        /// <param name="studyStorage"></param>
        /// <returns></returns>
        public IList<AuthorityGroupStudyAccessInfo> ListAuthorityGroupsForStudyViaToken(StudyStorage studyStorage)
        {
            // list all Authority Groups (data or non data-access) with permission to access all studies on the same partition
            var adapter = new ServerPartitionDataAdapter();
            IList<AuthorityGroupDetail> groupWithAccessToAllStudies;
            adapter.GetAuthorityGroupsForPartition(studyStorage.ServerPartitionKey, false, out groupWithAccessToAllStudies);

            // Convert into AuthorityGroupStudyAccessInfo objects for rendering
            var result = new List<AuthorityGroupStudyAccessInfo>();
            foreach (var groupDetail in groupWithAccessToAllStudies)
            {
                result.Add(new AuthorityGroupStudyAccessInfo(groupDetail));
            }


            return result;
        }

        public bool UpdateStudyAuthorityGroups(string studyInstanceUid, string accessionNumber, ServerEntityKey studyStorageKey, IList<string> assignedGroupOids)
        {
            List<AuthorityGroupDetail> nonAddedAuthorityGroups;
            Dictionary<ServerEntityKey, AuthorityGroupDetail> dic = LoadAuthorityGroups(out nonAddedAuthorityGroups);
            IList<AuthorityGroupStudyAccessInfo> assignedList = ListDataAccessGroupsForStudy(dic, studyStorageKey);

            List<string> groupList = new List<string>();
            foreach (AuthorityGroupStudyAccessInfo group in assignedList)
            {
                bool found = false;
                foreach (var oid in assignedGroupOids)
                {
                    if (group.AuthorityOID.Equals(oid))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        IStudyDataAccessEntityBroker broker = update.GetBroker<IStudyDataAccessEntityBroker>();
                        broker.Delete(group.StudyDataAccess.Key);
                        update.Commit();
                    }

                    groupList.Add(group.Description);
                }
            }

            if (groupList.Count > 0)
            {
                ServerAuditHelper.RemoveAuthorityGroupAccess(studyInstanceUid, accessionNumber, groupList);
                groupList.Clear();
            }

            foreach (var oid in assignedGroupOids)
            {
                bool found = false;
                foreach (AuthorityGroupStudyAccessInfo group in assignedList)
                {
                    if (group.AuthorityOID.Equals(oid))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    DataAccessGroup accessGroup = AddDataAccessIfNotExists(oid);

                    using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        StudyDataAccessUpdateColumns insertColumns = new StudyDataAccessUpdateColumns
                                                                         {
                                                                             DataAccessGroupKey = accessGroup.Key,
                                                                             StudyStorageKey = studyStorageKey
                                                                         };

                        IStudyDataAccessEntityBroker insert = updateContext.GetBroker<IStudyDataAccessEntityBroker>();
                        insert.Insert(insertColumns);
                        updateContext.Commit();
                    }

                    foreach (AuthorityGroupDetail group in nonAddedAuthorityGroups)
                    {
                        if (group.AuthorityGroupRef.ToString(false,false).Equals(accessGroup.AuthorityGroupOID.Key.ToString()))
                            groupList.Add(group.Name);
                    }
                }
            }

            if (groupList.Count > 0)
                ServerAuditHelper.AddAuthorityGroupAccess(studyInstanceUid, accessionNumber, groupList);

            return true;
        }

        public bool AddStudyAuthorityGroups(string studyInstanceUid, string accessionNumber, ServerEntityKey studyStorageKey, IList<string> assignedGroupOids)
        {
            List<AuthorityGroupDetail> nonAddedSummaries;
            Dictionary<ServerEntityKey, AuthorityGroupDetail> dic = LoadAuthorityGroups(out nonAddedSummaries);
            IList<AuthorityGroupStudyAccessInfo> assignedList = ListDataAccessGroupsForStudy(dic, studyStorageKey);

            List<string> assignedGroups = new List<string>();
            foreach (var oid in assignedGroupOids)
            {
                bool found = false;
                foreach (AuthorityGroupStudyAccessInfo group in assignedList)
                {
                    if (group.AuthorityOID.Equals(oid))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    DataAccessGroup accessGroup = AddDataAccessIfNotExists(oid);

                    using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        StudyDataAccessUpdateColumns insertColumns = new StudyDataAccessUpdateColumns
                        {
                            DataAccessGroupKey = accessGroup.Key,
                            StudyStorageKey = studyStorageKey
                        };

                        IStudyDataAccessEntityBroker insert = updateContext.GetBroker<IStudyDataAccessEntityBroker>();
                        insert.Insert(insertColumns);
                        updateContext.Commit();
                    }

                    AuthorityGroupDetail detail;
                    if (dic.TryGetValue(accessGroup.Key, out detail))
                    {
                      assignedGroups.Add(detail.Name);
                    }
                }
            }
            if (assignedGroups.Count > 0)
                ServerAuditHelper.AddAuthorityGroupAccess(studyInstanceUid, accessionNumber, assignedGroups);
            return true;
        }

        #endregion

        #region Private Methods

        private static Dictionary<ServerEntityKey, AuthorityGroupDetail> LoadAuthorityGroups(out List<AuthorityGroupDetail> otherList)
        {
            Dictionary<ServerEntityKey, AuthorityGroupDetail> dic = new Dictionary<ServerEntityKey, AuthorityGroupDetail>();
            var list = new List<AuthorityGroupDetail>();

            using (var service = new AuthorityRead())
            {
                IList<AuthorityGroupDetail> tokens = service.ListDataAccessAuthorityGroupDetails();
                CollectionUtils.ForEach(tokens, delegate(AuthorityGroupDetail group)
                {
                    DataAccessGroupSelectCriteria select = new DataAccessGroupSelectCriteria();
                    select.AuthorityGroupOID.EqualTo(new ServerEntityKey("AuthorityGroupOID", new Guid(group.AuthorityGroupRef.ToString(false, false))));
                    IDataAccessGroupEntityBroker broker = HttpContext.Current.GetSharedPersistentContext().GetBroker<IDataAccessGroupEntityBroker>();
                    DataAccessGroup accessGroup = broker.FindOne(select);
                    if (accessGroup != null)
                    {
                        dic.Add(accessGroup.Key, group);
                    }
                    else
                    {
                        list.Add(group);
                    }
                });
            }

            otherList = list;

            return dic;
        }
       
        private IList<AuthorityGroupStudyAccessInfo> ListDataAccessGroupsForStudy(Dictionary<ServerEntityKey, AuthorityGroupDetail> dictionary, ServerEntityKey studyStorageKey)
        {
            List<AuthorityGroupStudyAccessInfo> list = new List<AuthorityGroupStudyAccessInfo>();

            StudyDataAccessSelectCriteria select = new StudyDataAccessSelectCriteria();
            select.StudyStorageKey.EqualTo(studyStorageKey);
            IStudyDataAccessEntityBroker broker = HttpContext.Current.GetSharedPersistentContext().GetBroker<IStudyDataAccessEntityBroker>();

            broker.Find(select, delegate(StudyDataAccess dataAccess)
            {
                AuthorityGroupDetail detail;
                if (dictionary.TryGetValue(dataAccess.DataAccessGroupKey, out detail))
                {
                    list.Add(new AuthorityGroupStudyAccessInfo(detail) { StudyDataAccess = dataAccess });
                }
            });

            return list;
        }

        private DataAccessGroup FindDataAccessGroup(string oid)
        {
            DataAccessGroupSelectCriteria select = new DataAccessGroupSelectCriteria();
            select.AuthorityGroupOID.EqualTo(new ServerEntityKey("AuthorityGroupOID", new Guid(oid)));
            IDataAccessGroupEntityBroker broker = HttpContext.Current.GetSharedPersistentContext().GetBroker<IDataAccessGroupEntityBroker>();
            return broker.FindOne(select);
        }

        #endregion
    }
}
