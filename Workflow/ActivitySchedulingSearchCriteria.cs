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

namespace ClearCanvas.Workflow {

    /// <summary>
    /// Search criteria for <see cref="ActivityScheduling"/> entity
    /// This file is machine generated - changes will be lost.
    /// </summary>
	public partial class ActivitySchedulingSearchCriteria : SearchCriteria
	{
		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public ActivitySchedulingSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public ActivitySchedulingSearchCriteria(string key)
			:base(key)
		{
		}

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected ActivitySchedulingSearchCriteria(ActivitySchedulingSearchCriteria other)
            : base(other)
        {
        }

        public override object Clone()
        {
            return new ActivitySchedulingSearchCriteria(this);
        }
 
        public ISearchCondition<DateTime?> StartTime
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("StartTime"))
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
	  			if(!this.SubCriteria.ContainsKey("EndTime"))
	  			{
	  				this.SubCriteria["EndTime"] = new SearchCondition<DateTime?>("EndTime");
	  			}
	  			return (ISearchCondition<DateTime?>)this.SubCriteria["EndTime"];
	  		}
	  	}
	}
}
