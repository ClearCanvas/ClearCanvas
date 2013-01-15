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

using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    internal class ServerRuleAdaptor :
        BaseAdaptor
            <ServerRule, IServerRuleEntityBroker, ServerRuleSelectCriteria, ServerRuleUpdateColumns>
    {
    }

    public class ServerRuleController : BaseController
    {
        #region Private Members

        private readonly ServerRuleAdaptor _adaptor = new ServerRuleAdaptor();

        #endregion

        #region Public Methods

        public IList<ServerRule> GetServerRules(ServerRuleSelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }

        public bool DeleteServerRule(ServerRule rule)
        {
            return _adaptor.Delete(rule.Key);
        }

        public ServerRule AddServerRule(ServerRule rule)
        {
            ServerRuleUpdateColumns parms = new ServerRuleUpdateColumns();

            parms.DefaultRule = rule.DefaultRule;
            parms.Enabled = rule.Enabled;
            parms.RuleName = rule.RuleName;
            parms.RuleXml = rule.RuleXml;
            parms.ServerPartitionKey = rule.ServerPartitionKey;
            parms.ServerRuleApplyTimeEnum = rule.ServerRuleApplyTimeEnum;
            parms.ServerRuleTypeEnum = rule.ServerRuleTypeEnum;
        	parms.ExemptRule = rule.ExemptRule;

            return _adaptor.Add(parms);
        }

        public bool UpdateServerRule(ServerRule rule)
        {
            ServerRuleUpdateColumns parms = new ServerRuleUpdateColumns();

            parms.DefaultRule = rule.DefaultRule;
            parms.Enabled = rule.Enabled;
            parms.RuleName = rule.RuleName;
            parms.RuleXml = rule.RuleXml;
            parms.ServerPartitionKey = rule.ServerPartitionKey;
            parms.ServerRuleApplyTimeEnum = rule.ServerRuleApplyTimeEnum;
            parms.ServerRuleTypeEnum = rule.ServerRuleTypeEnum;
			parms.ExemptRule = rule.ExemptRule;

            return _adaptor.Update(rule.Key, parms);
        }

        #endregion
    }
}
