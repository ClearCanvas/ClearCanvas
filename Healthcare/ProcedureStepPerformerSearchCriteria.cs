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
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// Search criteria for <see cref="ProcedureStepPerformer"/> entity
    /// This file is machine generated - changes will be lost.
    /// </summary>
	public partial class ProcedureStepPerformerSearchCriteria : SearchCriteria
	{
		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public ProcedureStepPerformerSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public ProcedureStepPerformerSearchCriteria(string key)
			:base(key)
		{
		}

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected ProcedureStepPerformerSearchCriteria(ProcedureStepPerformerSearchCriteria other)
            : base(other)
        {
        }

        public override object Clone()
        {
            return new ProcedureStepPerformerSearchCriteria(this);
        }


	  	public StaffSearchCriteria Staff
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("Staff"))
	  			{
                    this.SubCriteria["Staff"] = new StaffSearchCriteria("Staff");
	  			}
                return (StaffSearchCriteria)this.SubCriteria["Staff"];
	  		}
	  	}
	  	
	}
}
