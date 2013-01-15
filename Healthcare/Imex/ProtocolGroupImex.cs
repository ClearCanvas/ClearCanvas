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
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;
using System.Xml;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("ProtocolGroup")]
    public class ProtocolGroupImex : XmlEntityImex<ProtocolGroup, ProtocolGroupImex.ProtocolGroupData>
    {
        [DataContract]
        public class ProtocolGroupData
        {
            [DataMember]
            public string Name;

            [DataMember]
            public string Description;

            [DataMember]
            public List<ProtocolCodeData> Codes;

            [DataMember]
            public List<ReadingGroupData> ReadingGroups;
        }

        [DataContract]
        public class ProtocolCodeData
        {
            [DataMember]
            public string Name;

            [DataMember]
            public string Description;
        }

        [DataContract]
        public class ReadingGroupData
        {
            [DataMember]
            public string Name;
        }

        #region Overrides

        protected override IList<ProtocolGroup> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            ProtocolGroupSearchCriteria where = new ProtocolGroupSearchCriteria();
            where.Name.SortAsc(0);

            return context.GetBroker<IProtocolGroupBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override ProtocolGroupData Export(ProtocolGroup group, IReadContext context)
        {
            ProtocolGroupData data = new ProtocolGroupData();
            data.Name = group.Name;
            data.Description = group.Description;

            data.Codes = CollectionUtils.Map<ProtocolCode, ProtocolCodeData>(group.Codes,
                delegate(ProtocolCode code)
                {
                    ProtocolCodeData s = new ProtocolCodeData();
                    s.Name = code.Name;
                    s.Description = code.Description;
                    return s;
                });

            data.ReadingGroups = CollectionUtils.Map<ReadingGroup, ReadingGroupData>(group.ReadingGroups,
                delegate(ReadingGroup rg)
                {
                    ReadingGroupData s = new ReadingGroupData();
                    s.Name = rg.Name;
                    return s;
                });

            return data;
        }

        protected override void Import(ProtocolGroupData data, IUpdateContext context)
        {
            ProtocolGroup group = LoadOrCreateProtocolGroup(data.Name, context);
            group.Description = data.Description;

            if (data.Codes != null)
            {
                foreach (ProtocolCodeData s in data.Codes)
                {
                    ProtocolCode code = LoadOrCreateProtocolCode(s.Name, context);
                    code.Description = s.Description;

                    group.Codes.Add(code);
                }
            }

            if (data.ReadingGroups != null)
            {
                foreach (ReadingGroupData s in data.ReadingGroups)
                {
                    ReadingGroupSearchCriteria criteria = new ReadingGroupSearchCriteria();
                    criteria.Name.EqualTo(s.Name);

                    IReadingGroupBroker broker = context.GetBroker<IReadingGroupBroker>();
                    ReadingGroup rg = CollectionUtils.FirstElement(broker.Find(criteria));
                    if (rg != null)
                        group.ReadingGroups.Add(rg);
                }
            }


        }

        #endregion

        private ProtocolGroup LoadOrCreateProtocolGroup(string name, IPersistenceContext context)
        {
            ProtocolGroup group;
            try
            {
                // see if already exists in db
                ProtocolGroupSearchCriteria where = new ProtocolGroupSearchCriteria();
                where.Name.EqualTo(name);
                group = context.GetBroker<IProtocolGroupBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                // create it
                group = new ProtocolGroup();
                group.Name = name;
                context.Lock(group, DirtyState.New);
            }

            return group;
        }

        private ProtocolCode LoadOrCreateProtocolCode(string name, IPersistenceContext context)
        {
            ProtocolCode code;
            try
            {
                // see if already exists in db
                ProtocolCodeSearchCriteria where = new ProtocolCodeSearchCriteria();
                where.Name.EqualTo(name);
                code = context.GetBroker<IProtocolCodeBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                // create it
                code = new ProtocolCode();
                code.Name = name;
                context.Lock(code, DirtyState.New);
            }

            return code;
        }
    }
}
