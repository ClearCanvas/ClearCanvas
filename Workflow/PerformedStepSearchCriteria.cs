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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Workflow
{
    public class PerformedStepSearchCriteria : EntitySearchCriteria<PerformedStep>
    {
 		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public PerformedStepSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
        public PerformedStepSearchCriteria(string key)
			:base(key)
		{
		}

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected PerformedStepSearchCriteria(PerformedStepSearchCriteria other)
            : base(other)
        {
        }

        public override object Clone()
        {
            return new PerformedStepSearchCriteria(this);
        }

        public ISearchCondition<PerformedStepStatus> Status
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Status"))
                {
                    this.SubCriteria["Status"] = new SearchCondition<PerformedStepStatus>("Status");
                }
                return (ISearchCondition<PerformedStepStatus>)this.SubCriteria["Status"];
            }
        }

        public ISearchCondition<DateTime?> StartTime
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StartTime"))
                {
                    this.SubCriteria["StartTime"] = new SearchCondition<DateTime?>("StartTime");
                }
                return (ISearchCondition<DateTime?>)this.SubCriteria["StartTime"];
            }
        }

        public ISearchCondition<DateTime?> EndTime
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("EndTime"))
                {
                    this.SubCriteria["EndTime"] = new SearchCondition<DateTime?>("EndTime");
                }
                return (ISearchCondition<DateTime?>)this.SubCriteria["EndTime"];
            }
        }
    }
}
