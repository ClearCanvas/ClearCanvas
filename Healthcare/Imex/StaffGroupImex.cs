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

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("StaffGroup")]
    public class StaffGroupImex : XmlEntityImex<StaffGroup, StaffGroupImex.StaffGroupData>
    {
        [DataContract]
		public class StaffGroupData : ReferenceEntityDataBase
        {
            [DataMember]
            public string Name;

            [DataMember]
            public string Description;

			[DataMember]
			public bool Elective;

			[DataMember]
            public List<StaffMemberData> Members;

        }

        [DataContract]
        public class StaffMemberData
        {
            [DataMember]
            public string Id;
        }

        #region Overrides

        protected override IList<StaffGroup> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            StaffGroupSearchCriteria where = new StaffGroupSearchCriteria();
            where.Name.SortAsc(0);

            return context.GetBroker<IStaffGroupBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override StaffGroupData Export(StaffGroup entity, IReadContext context)
        {
            StaffGroupData data = new StaffGroupData();
			data.Deactivated = entity.Deactivated;
			data.Name = entity.Name;
            data.Description = entity.Description;
        	data.Elective = entity.Elective;

            data.Members = CollectionUtils.Map<Staff, StaffMemberData>(
                entity.Members,
                delegate(Staff staff)
                {
                    StaffMemberData s = new StaffMemberData();
                    s.Id = staff.Id;
                    return s;
                });

            return data;
        }

        protected override void Import(StaffGroupData data, IUpdateContext context)
        {
            StaffGroup group = LoadOrCreateGroup(data.Name,context);
        	group.Deactivated = data.Deactivated;
            group.Description = data.Description;
        	group.Elective = data.Elective;

            if (data.Members != null)
            {
                foreach (StaffMemberData s in data.Members)
                {
                    StaffSearchCriteria where = new StaffSearchCriteria();
                    where.Id.EqualTo(s.Id);
                    Staff staff = CollectionUtils.FirstElement(context.GetBroker<IStaffBroker>().Find(where));
                    if (staff != null)
                        group.Members.Add(staff);
                }
            }
        }

        #endregion


        private StaffGroup LoadOrCreateGroup(string name, IPersistenceContext context)
        {
            StaffGroup group;
            try
            {
                StaffGroupSearchCriteria where = new StaffGroupSearchCriteria();
                where.Name.EqualTo(name);
                group = context.GetBroker<IStaffGroupBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                group = new StaffGroup();
                group.Name = name;
                context.Lock(group, DirtyState.New);
            }

            return group;
        }
    }
}
