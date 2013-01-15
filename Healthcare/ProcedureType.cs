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

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// ProcedureType entity
    /// </summary>
	public partial class ProcedureType
    {
    	private ProcedurePlan _plan;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public ProcedureType(string id, string name)
            :this(id, name, null, null, 0)
        {
        }

        /// <summary>
        /// Sets the plan for this procedure type from the specified prototype procedure.
        /// </summary>
        /// <param name="prototype"></param>
        public virtual void SetPlanFromPrototype(Procedure prototype)
        {
			this.Plan = ProcedurePlan.CreateFromProcedure(prototype);
        }

		/// <summary>
		/// Gets or sets the procedure plan.
		/// </summary>
    	public virtual ProcedurePlan Plan
    	{
    		get
    		{
				if (_plan == null)
				{
					_plan = new ProcedurePlan(_planXml);
				}
    			return _plan;
    		}
			set
			{
				if(value == null)
					throw new InvalidOperationException("Value must not be null.");

				_plan = value;
				_planXml = value.ToString();
			}
    	}

        /// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}