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
using System.Collections;
using System.Text;
using ClearCanvas.Enterprise.Core.Modelling;
using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// InterpretationStep entity
    /// </summary>
	public partial class InterpretationStep : ReportingProcedureStep
	{
        public InterpretationStep(Procedure procedure)
            :base(procedure, null)
        {
		}

        public InterpretationStep(ReportingProcedureStep previousStep)
            :base(previousStep)
        {
			CustomInitialize();
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		protected override void LinkProcedure(Procedure procedure)
		{
			if(this.Report == null)
				throw new WorkflowException("This step must be associated with a Report before procedures can be linked.");

			this.Report.LinkProcedure(procedure);
		}

        public override string Name
        {
            get { return "Interpretation"; }
        }

        protected override void OnStateChanged(ActivityStatus previousState, ActivityStatus newState)
        {
            if(newState == ActivityStatus.CM)
            {
                if (this.ReportPart == null)
                    throw new WorkflowException("This ReportingStep does not have an associated ReportPart.");
            }

            base.OnStateChanged(previousState, newState);
        }

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new InterpretationStep(this);
		}
	}
}
