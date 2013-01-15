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
using ClearCanvas.Workflow;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
    public abstract class ReportingProcedureStep : ProcedureStep
    {
        private ReportPart _reportPart;

        public ReportingProcedureStep()
        {
        }

        public ReportingProcedureStep(Procedure procedure, ReportPart reportPart)
            :base(procedure)
        {
            _reportPart = reportPart;
        }

        public ReportingProcedureStep(ReportingProcedureStep previousStep)
            : this(previousStep.Procedure, previousStep.ReportPart)
        {
        }

		public override bool CreateInDowntimeMode
		{
			get { return true; }
		}

		public override bool IsPreStep
		{
			get { return false; }
		}

		public override TimeSpan SchedulingOffset
		{
			get { return TimeSpan.MaxValue; }
		}

		public override List<Procedure> GetLinkedProcedures()
		{
			if(_reportPart != null && _reportPart.Report != null)
			{
				return CollectionUtils.Select(_reportPart.Report.Procedures,
					delegate(Procedure p) { return !Equals(p, this.Procedure); });
			}
			else
			{
				return new List<Procedure>();
			}
		}

		/// <summary>
		/// Gets the <see cref="ReportPart"/> that this step targets, or null if there is no associated report part.
		/// </summary>
        public virtual ReportPart ReportPart
        {
            get { return _reportPart; }
            set { _reportPart = value; }
        }

		/// <summary>
		/// Gets the <see cref="Report"/> that this step is associated with, or null if not associated.
		/// </summary>
    	public virtual Report Report
    	{
			get { return _reportPart == null ? null : _reportPart.Report; }
    	}

		protected override bool IsRelatedStep(ProcedureStep step)
		{
			// can't have relatives if no report
			if(this.Report == null)
				return false;

			// relatives must be reporting steps
			if (!step.Is<ReportingProcedureStep>())
				return false;

			// check if tied to same report
			ReportingProcedureStep that = step.As<ReportingProcedureStep>();
			return that.Report != null && Equals(this.Report, that.Report);
		}
	}
}
