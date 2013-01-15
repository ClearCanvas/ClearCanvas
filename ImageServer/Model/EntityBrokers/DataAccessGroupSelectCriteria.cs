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
    public partial class DataAccessGroupSelectCriteria
    {
        /// <summary>
        /// Used for EXISTS or NOT EXISTS subselects against the <see cref="ServerPartitionDataAccess"/> table.
        /// </summary>
        /// <remarks>
        /// A <see cref="ServerPartitionDataAccessSelectCriteria"/> instance is created with the subselect parameters, 
        /// and assigned to this Sub-Criteria.  Note that the link between the <see cref="ServerPartitionDataAccess"/>
        /// and <see cref="DataAccessGroup"/> tables is automatically added into the <see cref="ServerPartitionDataAccessSelectCriteria"/>
        /// instance by the broker.
        /// </remarks>
        public IRelatedEntityCondition<EntitySelectCriteria> ServerPartitionDataAccessRelatedEntityCondition
        {
            get
            {
                if (!SubCriteria.ContainsKey("ServerPartitionDataAccessRelatedEntityCondition"))
                {
                    SubCriteria["ServerPartitionDataAccessRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("ServerPartitionDataAccessRelatedEntityCondition", "Key", "DataAccessGroupKey");
                }
                return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["ServerPartitionDataAccessRelatedEntityCondition"];
            }
        }
    }
}
