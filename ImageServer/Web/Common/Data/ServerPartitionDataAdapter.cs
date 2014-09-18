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
using System.Threading;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Web.Common.Utilities;
using ClearCanvas.Web.Enterprise.Admin;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Used to create/update/delete server partition entries in the database.
    /// </summary>
    public class ServerPartitionDataAdapter :
        BaseAdaptor
            <ServerPartition, IServerPartitionEntityBroker, ServerPartitionSelectCriteria, ServerPartitionUpdateColumns>
    {
        #region Public methods

        /// <summary>
        /// Gets a server partition entity with the specified AE Title
        /// </summary>
        /// <returns></returns>
        public ServerPartition GetServerPartition(string aeTitle)
        {
            var criteria = new ServerPartitionSelectCriteria();
            criteria.AeTitle.EqualTo(aeTitle);
            return GetFirst(criteria);
        }

        /// <summary>
        /// Gets a list of all server partitions.
        /// </summary>
        /// <returns></returns>
        public IList<ServerPartition> GetServerPartitions()
        {
            return Get();
        }

        public IList<ServerPartition> GetServerPartitions(ServerPartitionSelectCriteria criteria)
        {
            return Get(criteria);
        }

        public ServerPartition GetServerPartition(ServerEntityKey key)
        {
            return Get(key);
        }

        /// <summary>
        /// Creats a new server parition.
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="groupsWithDataAccess"> </param>
        public bool AddServerPartition(ServerPartition partition, List<string> groupsWithDataAccess)
        {
            bool ok;

            using (IUpdateContext ctx = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                var insert = ctx.GetBroker<IInsertServerPartition>();
                var parms = new ServerPartitionInsertParameters
                                {
                                    AeTitle = partition.AeTitle,
                                    Description = partition.Description,
                                    Enabled = partition.Enabled,
                                    PartitionFolder = partition.PartitionFolder,
                                    Port = partition.Port,
                                    DefaultRemotePort = partition.DefaultRemotePort,
                                    AutoInsertDevice = partition.AutoInsertDevice,
                                    AcceptAnyDevice = partition.AcceptAnyDevice,
                                    DuplicateSopPolicyEnum = partition.DuplicateSopPolicyEnum,
                                    MatchPatientsName = partition.MatchPatientsName,
                                    MatchPatientId = partition.MatchPatientId,
                                    MatchPatientsBirthDate = partition.MatchPatientsBirthDate,
                                    MatchAccessionNumber = partition.MatchAccessionNumber,
                                    MatchIssuerOfPatientId = partition.MatchIssuerOfPatientId,
                                    MatchPatientsSex = partition.MatchPatientsSex,
                                    AuditDeleteStudy = partition.AuditDeleteStudy,
                                    AcceptLatestReport = partition.AcceptLatestReport
                                };
                try
                {
                    ServerPartition insertPartition = insert.FindOne(parms);
					ok = insertPartition != null;

                    UpdateDataAccess(ctx, insertPartition, groupsWithDataAccess);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Error while inserting server partition.");
                    ok = false;
                }

                if (ok)
                    ctx.Commit();
            }

            return ok;
        }

        public DataAccessGroup FindDataAccessGroup(string oid)
        {
            var select = new DataAccessGroupSelectCriteria();
            select.AuthorityGroupOID.EqualTo(new ServerEntityKey("AuthorityGroupOID", new Guid(oid)));
			var broker = HttpContext.Current.GetSharedPersistentContext().GetBroker<IDataAccessGroupEntityBroker>();
            return broker.FindOne(select);
        
        }

        private DataAccessGroup AddDataAccessIfNotExists(string oid)
        {
            DataAccessGroup theGroup = FindDataAccessGroup(oid);
            if (theGroup == null)
            {
                using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    var insert = new DataAccessGroupUpdateColumns
                    {
                        AuthorityGroupOID =
                            new ServerEntityKey("AuthorityGroupOID",
                                                new Guid(oid)),
                        Deleted = false
                    };
                    var broker = update.GetBroker<IDataAccessGroupEntityBroker>();
                    theGroup = broker.Insert(insert);
                    update.Commit();
                }
            }
            return theGroup;
         }

        public bool Update(ServerPartition partition)
        {
            var parms = new ServerPartitionUpdateColumns
                            {
                                AeTitle = partition.AeTitle,
                                Description = partition.Description,
                                Enabled = partition.Enabled,
                                PartitionFolder = partition.PartitionFolder,
                                Port = partition.Port,
                                AcceptAnyDevice = partition.AcceptAnyDevice,
                                AutoInsertDevice = partition.AutoInsertDevice,
                                DefaultRemotePort = partition.DefaultRemotePort,
                                DuplicateSopPolicyEnum = partition.DuplicateSopPolicyEnum,
                                MatchPatientsName = partition.MatchPatientsName,
                                MatchPatientId = partition.MatchPatientId,
                                MatchPatientsBirthDate = partition.MatchPatientsBirthDate,
                                MatchAccessionNumber = partition.MatchAccessionNumber,
                                MatchIssuerOfPatientId = partition.MatchIssuerOfPatientId,
                                MatchPatientsSex = partition.MatchPatientsSex,
                                AuditDeleteStudy = partition.AuditDeleteStudy,
                                AcceptLatestReport = partition.AcceptLatestReport,
                            };

            return Update(partition.Key, parms);
        }

        public IEnumerable<ServerPartitionDataAccess> GetServerPartitionDataAccessGroups(ServerPartition partition)
       {
		   var broker = HttpContext.Current.GetSharedPersistentContext().GetBroker<IServerPartitionDataAccessEntityBroker>();
           var criteria = new ServerPartitionDataAccessSelectCriteria();
           criteria.ServerPartitionKey.EqualTo(partition.Key);
           return broker.Find(criteria);
       }
       

       private void UpdateDataAccess(IUpdateContext ctx, ServerPartition partition, List<string> groupsWithDataAccess)
       {
           var broker = ctx.GetBroker<IServerPartitionDataAccessEntityBroker>();
           var criteria = new ServerPartitionDataAccessSelectCriteria();
           criteria.ServerPartitionKey.EqualTo(partition.Key);
           
           var existingGroups = broker.Find(criteria);
           if (existingGroups!=null)
           {
               foreach(var g in existingGroups)
               {
                   if (!groupsWithDataAccess.Contains(g.Key.ToString()))
                   {
                       broker.Delete(g.Key);
                   }
               }
           }

           if (groupsWithDataAccess!=null)
           {
               foreach (var g in groupsWithDataAccess)
               {
                   string g1 = g;
                   if (!CollectionUtils.Contains(existingGroups, group => group.Key.ToString().Equals(g1)))
                   {
                       var dataAccessGroup= AddDataAccessIfNotExists(g);
                       
                       var record = new ServerPartitionDataAccessUpdateColumns
                       {
                           DataAccessGroupKey = dataAccessGroup.Key,
                           ServerPartitionKey = partition.Key
                       };

                       broker.Insert(record);
                   }
               }
           }

          
       }

       public bool Update(ServerPartition partition, List<string> groupsWithDataAccess)
       {
           using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
           {
               var parms = new ServerPartitionUpdateColumns
                               {
                                   AeTitle = partition.AeTitle,
                                   Description = partition.Description,
                                   Enabled = partition.Enabled,
                                   PartitionFolder = partition.PartitionFolder,
                                   Port = partition.Port,
                                   AcceptAnyDevice = partition.AcceptAnyDevice,
                                   AutoInsertDevice = partition.AutoInsertDevice,
                                   DefaultRemotePort = partition.DefaultRemotePort,
                                   DuplicateSopPolicyEnum = partition.DuplicateSopPolicyEnum,
                                   MatchPatientsName = partition.MatchPatientsName,
                                   MatchPatientId = partition.MatchPatientId,
                                   MatchPatientsBirthDate = partition.MatchPatientsBirthDate,
                                   MatchAccessionNumber = partition.MatchAccessionNumber,
                                   MatchIssuerOfPatientId = partition.MatchIssuerOfPatientId,
                                   MatchPatientsSex = partition.MatchPatientsSex,
                                   AuditDeleteStudy = partition.AuditDeleteStudy,
                                   AcceptLatestReport = partition.AcceptLatestReport
                               };

               var broker = context.GetBroker<IServerPartitionEntityBroker>();
               if (!broker.Update(partition.Key, parms))
                   return false;

               UpdateDataAccess(context, partition, groupsWithDataAccess);

               context.Commit();
               return true;
           }
       }

        /// <summary>
        /// Gets a list of authority groups that can access a given partition
        /// </summary>
        /// <param name="partitionKey">The partition</param>
        /// <param name="dataAccessGrupsOnly">True to find data access groups only; False to find all authority groups</param>
        /// <param name="allStudiesGroup">Returns a list of groups that have access to all studies</param>
        /// <returns></returns>
        public IList<AuthorityGroupDetail> GetAuthorityGroupsForPartition(ServerEntityKey partitionKey, bool dataAccessGrupsOnly, out IList<AuthorityGroupDetail> allStudiesGroup )
        {
            using (var service = new AuthorityRead())
            {
                IList<AuthorityGroupDetail> groups = dataAccessGrupsOnly
                                                         ? service.ListDataAccessAuthorityGroupDetails()
                                                         : service.ListAllAuthorityGroupDetails();

                IList<AuthorityGroupDetail> resultGroups = new List<AuthorityGroupDetail>();
                var internalAllStudiesGroup = new List<AuthorityGroupDetail>();

                CollectionUtils.ForEach(
                    groups,
                    delegate(AuthorityGroupDetail group)
                    {
                        bool allPartitions = false;
                        bool allStudies = false;
                        foreach (var token in group.AuthorityTokens)
                        {
                            if (token.Name.Equals(ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllPartitions))
                            {
                                allPartitions = true;                                
                            }
                            else if (token.Name.Equals(ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllStudies))
                            {
                                allStudies = true;
                            }

                            if (allPartitions && allStudies) break;
                        }

                        if (allPartitions && allStudies)
                        {
                            internalAllStudiesGroup.Add(group);
                            return;
                        }

                        if (!allPartitions)
                        {
                            using (IReadContext readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                            {
                                var criteria = new ServerPartitionDataAccessSelectCriteria();
                                criteria.ServerPartitionKey.EqualTo(partitionKey);

                                var dataCriteria = new DataAccessGroupSelectCriteria();
                                dataCriteria.AuthorityGroupOID.EqualTo(new ServerEntityKey("AuthorityGroupOID", new Guid(group.AuthorityGroupRef.ToString(false, false))));
                                dataCriteria.ServerPartitionDataAccessRelatedEntityCondition.Exists(criteria);

                                var broker = readContext.GetBroker<IDataAccessGroupEntityBroker>();
                                if (broker.Count(dataCriteria) == 0)
                                    return;
                            }
                        }

                        if (allStudies)
                        {
                            internalAllStudiesGroup.Add(group);
                            return;
                        }
                        
                        resultGroups.Add(group);
                    });

                allStudiesGroup = internalAllStudiesGroup;
                return resultGroups;
            }
        }

        #endregion Public methods
    }
}

