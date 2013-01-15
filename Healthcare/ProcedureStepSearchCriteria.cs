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

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// Search criteria for <see cref="ProcedureStep"/> entity
    /// This file is machine generated - changes will be lost.
    /// </summary>
	public partial class ProcedureStepSearchCriteria : ActivitySearchCriteria
	{
		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public ProcedureStepSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public ProcedureStepSearchCriteria(string key)
			:base(key)
		{
		}

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected ProcedureStepSearchCriteria(ProcedureStepSearchCriteria other)
            : base(other)
        {
        }

        public override object Clone()
        {
            return new ProcedureStepSearchCriteria(this);
        }

		
        public new ProcedureStepSchedulingSearchCriteria Scheduling
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Scheduling"))
                {
                    this.SubCriteria["Scheduling"] = new ProcedureStepSchedulingSearchCriteria("Scheduling");
                }
                return (ProcedureStepSchedulingSearchCriteria)this.SubCriteria["Scheduling"];
            }
        }

        public ProcedureSearchCriteria Procedure
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("Procedure"))
	  			{
                    this.SubCriteria["Procedure"] = new ProcedureSearchCriteria("Procedure");
	  			}
                return (ProcedureSearchCriteria)this.SubCriteria["Procedure"];
	  		}
	  	}

        public ProcedureStepPerformerSearchCriteria Performer
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Performer"))
                {
                    this.SubCriteria["Performer"] = new ProcedureStepPerformerSearchCriteria("Performer");
                }
                return (ProcedureStepPerformerSearchCriteria)this.SubCriteria["Performer"];
            }
        }

	}
}
