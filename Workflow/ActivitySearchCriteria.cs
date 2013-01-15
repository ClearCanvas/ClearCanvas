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
    public abstract class ActivitySearchCriteria : EntitySearchCriteria<Activity>
    {
 		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public ActivitySearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public ActivitySearchCriteria(string key)
			:base(key)
		{
		}

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected ActivitySearchCriteria(ActivitySearchCriteria other)
            : base(other)
        {
        }

		
	  	public ActivitySchedulingSearchCriteria Scheduling
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("Scheduling"))
	  			{
	  				this.SubCriteria["Scheduling"] = new ActivitySchedulingSearchCriteria("Scheduling");
	  			}
	  			return (ActivitySchedulingSearchCriteria)this.SubCriteria["Scheduling"];
	  		}
	  	}
	  	
	  	public ISearchCondition<ActivityStatus> State
	  	{
	  		get
	  		{
                if (!this.SubCriteria.ContainsKey("State"))
	  			{
                    this.SubCriteria["State"] = new SearchCondition<ActivityStatus>("State");
	  			}
                return (ISearchCondition<ActivityStatus>)this.SubCriteria["State"];
	  		}
	  	}

        public ISearchCondition<DateTime> CreationTime
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("CreationTime"))
                {
                    this.SubCriteria["CreationTime"] = new SearchCondition<DateTime>("CreationTime");
                }
                return (ISearchCondition<DateTime>)this.SubCriteria["CreationTime"];
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
