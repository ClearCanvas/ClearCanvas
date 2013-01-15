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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
	public partial class StudyIntegrityQueueSelectCriteria
	{
		/// <summary>
		/// Used for EXISTS or NOT EXISTS subselects against the StudyIntegrityQueueUID table.
		/// </summary>
		/// <remarks>
		/// A <see cref="StudyIntegrityQueueUidSelectCriteria"/> instance is created with the subselect parameters, 
		/// and assigned to this Sub-Criteria.  Note that the link between the <see cref="StudyIntegrityQueue"/>
		/// and <see cref="StudyIntegrityQueueUid"/> tables is automatically added into the <see cref="StudyIntegrityQueueSelectCriteria"/>
		/// instance by the broker.
		/// </remarks>
		public IRelatedEntityCondition<EntitySelectCriteria> StudyIntegrityQueueUidRelatedEntityCondition
		{
			get
			{
				if (!SubCriteria.ContainsKey("StudyIntegrityQueueUidRelatedEntityCondition"))
				{
					SubCriteria["StudyIntegrityQueueUidRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("StudyIntegrityQueueUidRelatedEntityCondition",
						"Key", "StudyIntegrityQueueKey");
				}
				return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["StudyIntegrityQueueUidRelatedEntityCondition"];
			}
		}

        /// <summary>
        /// Used for EXISTS or NOT EXISTS subselects against the StudyDataAccess table.
        /// </summary>
        /// <remarks>
        /// A <see cref="StudyDataAccessSelectCriteria"/> instance is created with the subselect parameters, 
        /// and assigned to this Sub-Criteria.  Note that the link between the <see cref="Study"/>
        /// and <see cref="StudyDataAccess"/> tables is automatically added into the <see cref="StudyDataAccessSelectCriteria"/>
        /// instance by the broker.
        /// </remarks>
        public IRelatedEntityCondition<EntitySelectCriteria> StudyDataAccessRelatedEntityCondition
        {
            get
            {
                if (!SubCriteria.ContainsKey("StudyDataAccessRelatedEntityCondition"))
                {
                    SubCriteria["StudyDataAccessRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("StudyDataAccessRelatedEntityCondition", "StudyStorageKey", "StudyStorageKey");
                }
                return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["StudyDataAccessRelatedEntityCondition"];
            }
        }
	}
}
