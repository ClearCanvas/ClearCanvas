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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Imex;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication.Brokers;

namespace ClearCanvas.Enterprise.Authentication.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("AuthorityGroup")]
    public class AuthorityGroupImex : XmlEntityImex<AuthorityGroup, AuthorityGroupImex.AuthorityGroupData>
    {
        [DataContract]
        public class AuthorityGroupData
        {
            [DataMember]
            public string Name;

			[DataMember]
			public string Description;

			[DataMember]
			public bool DataGroup;

            [DataMember]
            public List<string> Tokens;
        }


        #region Overrides

        protected override IList<AuthorityGroup> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            AuthorityGroupSearchCriteria where = new AuthorityGroupSearchCriteria();
            where.Name.SortAsc(0);
            return context.GetBroker<IAuthorityGroupBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override AuthorityGroupData Export(AuthorityGroup group, IReadContext context)
        {
            AuthorityGroupData data = new AuthorityGroupData();
            data.Name = group.Name;
        	data.Description = group.Description;
        	data.DataGroup = group.DataGroup;
            data.Tokens = CollectionUtils.Map<AuthorityToken, string>(
                group.AuthorityTokens,
                delegate(AuthorityToken token)
                {
                    return token.Name;
                });

            return data;
        }

        protected override void Import(AuthorityGroupData data, IUpdateContext context)
        {
            AuthorityGroup group = LoadOrCreateGroup(data.Name, context);
        	group.Description = data.Description;
        	group.DataGroup = data.DataGroup;
            if (data.Tokens != null)
            {
                foreach (string token in data.Tokens)
                {
                    AuthorityTokenSearchCriteria where = new AuthorityTokenSearchCriteria();
                    where.Name.EqualTo(token);

                    AuthorityToken authToken = CollectionUtils.FirstElement(context.GetBroker<IAuthorityTokenBroker>().Find(where));
                    if (authToken != null)
                        group.AuthorityTokens.Add(authToken);
                }
            }
        }

        #endregion


        private AuthorityGroup LoadOrCreateGroup(string name, IPersistenceContext context)
        {
            AuthorityGroup group = null;

            try
            {
                AuthorityGroupSearchCriteria criteria = new AuthorityGroupSearchCriteria();
                criteria.Name.EqualTo(name);

                IAuthorityGroupBroker broker = context.GetBroker<IAuthorityGroupBroker>();
                group = broker.FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
                group = new AuthorityGroup();
                group.Name = name;
                context.Lock(group, DirtyState.New);
            }
            return group;
        }
    }
}

