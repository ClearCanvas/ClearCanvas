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

using System.Linq;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using System;

namespace ClearCanvas.ImageServer.Model
{
    public class StudyCompareOptions
    {
        private bool _matchIssuerOfPatientId;
        private bool _matchPatientId;
        private bool _matchPatientsName;
        private bool _matchPatientsBirthDate;
        private bool _matchPatientsSex;
        private bool _matchAccessionNumber;

        public bool MatchIssuerOfPatientId
        {
            get { return _matchIssuerOfPatientId; }
            set { _matchIssuerOfPatientId = value; }
        }

        public bool MatchPatientId
        {
            get { return _matchPatientId; }
            set { _matchPatientId = value; }
        }

        public bool MatchPatientsName
        {
            get { return _matchPatientsName; }
            set { _matchPatientsName = value; }
        }

        public bool MatchPatientsBirthDate
        {
            get { return _matchPatientsBirthDate; }
            set { _matchPatientsBirthDate = value; }
        }

        public bool MatchPatientsSex
        {
            get { return _matchPatientsSex; }
            set { _matchPatientsSex = value; }
        }

        public bool MatchAccessionNumber
        {
            get { return _matchAccessionNumber; }
            set { _matchAccessionNumber = value; }
        }
    }

    public partial class ServerPartition
    {
        private readonly object _syncLock = new object();
        bool _dataAccessInfoloaded = false;
        bool _rulesLoaded = false;
        bool _archiveLoaded = false;
        private IList<ServerRule> _rules = null;
        private IList<PartitionArchive> _archives = null;

        private Dictionary<DataAccessGroup, ServerEntityKey> _mapDataAccessGroupsAuthorityGroups = null;

        IEnumerable<ServerPartitionDataAccess> _dataAccessGroups = null;

        /// <summary>
        /// Indicates the server partition contains at least one delete rule which is enabled
        /// </summary>
        public bool HasEnabledDeleteRules
        {
            get
            {
                LoadServerRules();

                return _rules != null && _rules.Any(r => r.ServerRuleTypeEnum == ServerRuleTypeEnum.StudyDelete && r.Enabled);
            }
        }

        /// <summary>
        /// Indicates the server partition currently has an archive configured (which may/may not be active)
        /// </summary>
        public bool ArchiveExists
        {
            get
            {
                LoadArchiveInfo();

                return _archives != null && _archives.Count > 0;
            }
        }



        public StudyCompareOptions GetComparisonOptions()
        {
            StudyCompareOptions options = new StudyCompareOptions();
            options.MatchAccessionNumber = MatchAccessionNumber;
            options.MatchIssuerOfPatientId = MatchIssuerOfPatientId;
            options.MatchPatientId = MatchPatientId;
            options.MatchPatientsBirthDate = MatchPatientsBirthDate;
            options.MatchPatientsName = MatchPatientsName;
            options.MatchPatientsSex = MatchPatientsSex;

            return options;
        }

        public IEnumerable<ServerPartitionDataAccess> DataAccessGroups
        {
            get
            {
                if (_dataAccessGroups == null)
                {
                    using (var ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                    {
                        LoadDataAcessInformation(ctx);
                    }

                }
                return _dataAccessGroups;
            }
        }

        private Dictionary<DataAccessGroup, ServerEntityKey> MapDataAccessGroupsAuthorityGroups
        {
            get
            {
                if (_mapDataAccessGroupsAuthorityGroups == null)
                {
                    using (var ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                    {

                        LoadAuthorityGroup(ctx);
                    }

                }

                return _mapDataAccessGroupsAuthorityGroups;
            }
        }

        /// <summary>
        /// Checks if users in the specified data access authority group have access to this partition
        /// </summary>
        /// <param name="authorityGroupOID"></param>
        /// <returns></returns>
        public bool IsAuthorityGroupAllowed(string authorityGroupOID)
        {
            var dataAccessGroup = FindDataAccessGroup(authorityGroupOID);
            if (dataAccessGroup == null)
                return false;

            var existingGroups = DataAccessGroups;
            if (existingGroups == null)
                return false;

            return CollectionUtils.Contains(existingGroups, g => g.DataAccessGroupKey.Equals(dataAccessGroup.Key));
        }

        public void LoadDataAcessInformation(IPersistenceContext context)
        {
            lock (_syncLock)
            {
                if (_dataAccessInfoloaded)
                    return;

                IServerPartitionDataAccessEntityBroker broker = context.GetBroker<IServerPartitionDataAccessEntityBroker>();
                ServerPartitionDataAccessSelectCriteria criteria = new ServerPartitionDataAccessSelectCriteria();
                criteria.ServerPartitionKey.EqualTo(this.Key);
                _dataAccessGroups = broker.Find(criteria);

                _dataAccessInfoloaded = true;
            }


        }

        private void LoadServerRules()
        {
            if (!_rulesLoaded)
            {
                lock (_syncLock)
                {
                    if (!_rulesLoaded)
                    {
                        using (var ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                        {
                            var broker = ctx.GetBroker<IServerRuleEntityBroker>();
                            var criteria = new ServerRuleSelectCriteria();
                            criteria.ServerPartitionKey.EqualTo(this.Key);
                            _rules = broker.Find(criteria);
                        }

                        _rulesLoaded = true;
                    }
                }
            }

        }

        private void LoadArchiveInfo()
        {
            if (!_archiveLoaded)
            {
                lock (_syncLock)
                {
                    if (!_archiveLoaded)
                    {
                        using (var ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                        {
                            var broker = ctx.GetBroker<IPartitionArchiveEntityBroker>();
                            var criteria = new PartitionArchiveSelectCriteria();
                            criteria.ServerPartitionKey.EqualTo(this.Key);
                            _archives = broker.Find(criteria);
                        }

                        _archiveLoaded = true;
                    }
                }
            }
        }

        private DataAccessGroup FindDataAccessGroup(string authorityGroupOID)
        {
            foreach (var entry in MapDataAccessGroupsAuthorityGroups)
            {
                if (entry.Value.Key.ToString().Equals(authorityGroupOID))
                    return entry.Key;
            }

            return null;
        }

        private void LoadAuthorityGroup(IPersistenceContext context)
        {
            lock (_syncLock)
            {
                _mapDataAccessGroupsAuthorityGroups = new Dictionary<DataAccessGroup, ServerEntityKey>();

                IDataAccessGroupEntityBroker dataAccessBroker = context.GetBroker<IDataAccessGroupEntityBroker>();
                DataAccessGroupSelectCriteria all = new DataAccessGroupSelectCriteria();
                var dataAccessGroups = dataAccessBroker.Find(all);

                foreach (var group in dataAccessGroups)
                {
                    _mapDataAccessGroupsAuthorityGroups.Add(group, group.AuthorityGroupOID);
                }
            }
        }
    }
}
