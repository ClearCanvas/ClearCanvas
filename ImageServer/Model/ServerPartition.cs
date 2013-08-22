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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model
{
    public class StudyCompareOptions
    {
        public bool MatchIssuerOfPatientId { get; set; }

        public bool MatchPatientId { get; set; }

        public bool MatchPatientsName { get; set; }

        public bool MatchPatientsBirthDate { get; set; }

        public bool MatchPatientsSex { get; set; }

        public bool MatchAccessionNumber { get; set; }
    }

    public partial class ServerPartition
    {
        private readonly object _syncLock = new object();
        private bool _dataAccessInfoloaded = false;
        private bool _rulesLoaded = false;
        private bool _archiveLoaded = false;
        private bool _aeTitlesLoaded = false;
        private IList<ServerRule> _rules = null;
        private IList<PartitionArchive> _archives = null;
        private IList<ServerPartitionAlternateAeTitle> _alternateAeTitles = null;

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
            var options = new StudyCompareOptions
                {
                    MatchAccessionNumber = MatchAccessionNumber,
                    MatchIssuerOfPatientId = MatchIssuerOfPatientId,
                    MatchPatientId = MatchPatientId,
                    MatchPatientsBirthDate = MatchPatientsBirthDate,
                    MatchPatientsName = MatchPatientsName,
                    MatchPatientsSex = MatchPatientsSex
                };

            return options;
        }

        public IEnumerable<ServerPartitionDataAccess> DataAccessGroups
        {
            get
            {
                if (_dataAccessGroups == null)
                {
                    using (var context = new ServerExecutionContext())
                    {
                        LoadDataAcessInformation(context.ReadContext);
                    }

                }
                return _dataAccessGroups;
            }
        }

        public IEnumerable<ServerPartitionAlternateAeTitle> RelatedServerPartitionAlternateAeTitles
        {
            get
            {
                LoadAlternateAes();
                return _alternateAeTitles;
            }
        }

        private Dictionary<DataAccessGroup, ServerEntityKey> MapDataAccessGroupsAuthorityGroups
        {
            get
            {
                if (_mapDataAccessGroupsAuthorityGroups == null)
                {
                    using (var context = new ServerExecutionContext())
                    {
                        LoadAuthorityGroup(context.ReadContext);
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

                var broker = context.GetBroker<IServerPartitionDataAccessEntityBroker>();
                var criteria = new ServerPartitionDataAccessSelectCriteria();
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
                        using (var context = new ServerExecutionContext())
                        {
                            var broker = context.ReadContext.GetBroker<IServerRuleEntityBroker>();
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
                        using (var context = new ServerExecutionContext())
                        {
                            var broker = context.ReadContext.GetBroker<IPartitionArchiveEntityBroker>();
                            var criteria = new PartitionArchiveSelectCriteria();
                            criteria.ServerPartitionKey.EqualTo(this.Key);
                            _archives = broker.Find(criteria);
                        }

                        _archiveLoaded = true;
                    }
                }
            }
        }

        private void LoadAlternateAes()
        {
            if (!_aeTitlesLoaded)
            {
                lock (_syncLock)
                {
                    if (!_aeTitlesLoaded)
                    {
                        using (var context = new ServerExecutionContext())
                        {
                            var broker = context.ReadContext.GetBroker<IServerPartitionAlternateAeTitleEntityBroker>();
                            var criteria = new ServerPartitionAlternateAeTitleSelectCriteria();
                            criteria.ServerPartitionKey.EqualTo(this.Key);
                            _alternateAeTitles = broker.Find(criteria);
                        }

                        _aeTitlesLoaded = true;
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

                var dataAccessBroker = context.GetBroker<IDataAccessGroupEntityBroker>();
                var all = new DataAccessGroupSelectCriteria();
                var dataAccessGroups = dataAccessBroker.Find(all);

                foreach (var group in dataAccessGroups)
                {
                    _mapDataAccessGroupsAuthorityGroups.Add(group, group.AuthorityGroupOID);
                }
            }
        }
    }
}
