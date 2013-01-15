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
    public partial class StudySelectCriteria
    {
        /// <summary>
        /// Used for EXISTS or NOT EXISTS subselects against the Series table.
        /// </summary>
        /// <remarks>
        /// A <see cref="SeriesSelectCriteria"/> instance is created with the subselect parameters, 
        /// and assigned to this Sub-Criteria.  Note that the link between the <see cref="Study"/>
        /// and <see cref="Series"/> tables is automatically added into the <see cref="SeriesSelectCriteria"/>
        /// instance by the broker.
        /// </remarks>
        public IRelatedEntityCondition<EntitySelectCriteria> SeriesRelatedEntityCondition
        {
            get
            {
                if (!SubCriteria.ContainsKey("SeriesRelatedEntityCondition"))
                {
                    SubCriteria["SeriesRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("SeriesRelatedEntityCondition", "Key", "StudyKey");
                }
                return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["SeriesRelatedEntityCondition"];
            }
        }

		/// <summary>
		/// Used for EXISTS or NOT EXISTS subselects against the StudyStorage table.
		/// </summary>
		/// <remarks>
		/// A <see cref="StudyStorageSelectCriteria"/> instance is created with the subselect parameters, 
		/// and assigned to this Sub-Criteria.  Note that the link between the <see cref="Study"/>
		/// and <see cref="StudyStorage"/> tables is automatically added into the <see cref="StudyStorageSelectCriteria"/>
		/// instance by the broker.
		/// </remarks>
		public IRelatedEntityCondition<EntitySelectCriteria> StudyStorageRelatedEntityCondition
		{
			get
			{
				if (!SubCriteria.ContainsKey("StudyStorageRelatedEntityCondition"))
				{
					SubCriteria["StudyStorageRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("StudyStorageRelatedEntityCondition", "StudyStorageKey", "Key");
				}
				return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["StudyStorageRelatedEntityCondition"];
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
