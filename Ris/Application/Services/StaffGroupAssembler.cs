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
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
	public class StaffGroupAssembler
    {
		private class StaffGroupWorklistCollectionSynchronizeHelper : CollectionSynchronizeHelper<Worklist, WorklistSummary>
		{
			private readonly StaffGroup _group;
			private readonly IPersistenceContext _context;

			public StaffGroupWorklistCollectionSynchronizeHelper(StaffGroup group, IPersistenceContext context)
				: base(false, true)
			{
				_group = group;
				_context = context;
			}

			protected override bool CompareItems(Worklist destItem, WorklistSummary sourceItem)
			{
				return sourceItem.WorklistRef.Equals(destItem.GetRef(), true);
			}

			protected override void AddItem(WorklistSummary sourceItem, ICollection<Worklist> dest)
			{
				Worklist worklist = _context.Load<Worklist>(sourceItem.WorklistRef, EntityLoadFlags.Proxy);
				worklist.GroupSubscribers.Add(_group);
				dest.Add(worklist);
			}

			protected override void RemoveItem(Worklist destItem, ICollection<Worklist> dest)
			{
				destItem.GroupSubscribers.Remove(_group);
				dest.Remove(destItem);
			}
		}

        public StaffGroupSummary CreateSummary(StaffGroup staffGroup)
        {
            return new StaffGroupSummary(
                staffGroup.GetRef(),
                staffGroup.Name,
                staffGroup.Description,
				staffGroup.Elective,
				staffGroup.Deactivated);
        }

        public StaffGroupDetail CreateDetail(StaffGroup staffGroup, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
			WorklistAssembler worklistAssembler = new WorklistAssembler();

        	IList<Worklist> worklists = context.GetBroker<IWorklistBroker>().Find(staffGroup);

        	return new StaffGroupDetail(
        		staffGroup.GetRef(),
        		staffGroup.Name,
        		staffGroup.Description,
        		staffGroup.Elective,
        		CollectionUtils.Map<Staff, StaffSummary>(staffGroup.Members,
        		                                         delegate(Staff staff)
        		                                         	{
        		                                         		return staffAssembler.CreateStaffSummary(staff, context);
        		                                         	}),
        		CollectionUtils.Map<Worklist, WorklistSummary>(worklists,
        		                                         delegate(Worklist worklist)
    		                                               	{
    		                                               		return worklistAssembler.GetWorklistSummary(worklist, context);
    		                                               	}),
				staffGroup.Deactivated
                );
        }

        public void UpdateStaffGroup(StaffGroup group, StaffGroupDetail detail, bool updateWorklist, bool isNewStaffGroup, IPersistenceContext context)
        {
            group.Name = detail.Name;
            group.Description = detail.Description;
        	group.Elective = detail.IsElective;
        	group.Deactivated = detail.Deactivated;

            group.Members.Clear();
            CollectionUtils.ForEach(detail.Members,
                 delegate(StaffSummary summary)
                 {
                     group.AddMember(context.Load<Staff>(summary.StaffRef, EntityLoadFlags.Proxy));
                 });

			if (updateWorklist)
			{
				StaffGroupWorklistCollectionSynchronizeHelper helper = new StaffGroupWorklistCollectionSynchronizeHelper(group, context);
				IList<Worklist> originalWorklists = isNewStaffGroup 
					? new List<Worklist>()
					: context.GetBroker<IWorklistBroker>().Find(group);

				helper.Synchronize(originalWorklists, detail.Worklists);
			}
        }
    }
}