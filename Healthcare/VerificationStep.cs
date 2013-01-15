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
using System.Linq;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Workflow;
using System.Xml.Linq;

namespace ClearCanvas.Healthcare
{


    /// <summary>
    /// VerificationStep entity
    /// </summary>
	[Validation(HighLevelRulesProviderMethod = "GetValidationRules")]
	public partial class VerificationStep : ReportingProcedureStep
	{

        public VerificationStep(ReportingProcedureStep previousStep)
			: base(previousStep)
        {
        }
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        public override string Name
        {
            get { return "Verification"; }
        }

        protected override void OnStateChanged(ActivityStatus previousState, ActivityStatus newState)
        {
            if (newState == ActivityStatus.CM)
                this.ReportPart.Verifier = this.PerformingStaff;

            base.OnStateChanged(previousState, newState);
        }

		public override ProcedureStep Reassign(Staff performer)
		{
			var reassign = base.Reassign(performer).Downcast<VerificationStep>();

			// When reassigning a verification step to another staff, we should reassign the supervisor as well
			// so the report part will be reviewed by the appropriate staff radiologist
			if (reassign.ReportPart != null && reassign.ReportPart.Supervisor != null)
				reassign.ReportPart.Supervisor = performer;

			return reassign;
		}

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new VerificationStep(this);
		}

		private static IValidationRuleSet GetValidationRules()
		{
			return new ValidationRuleSet(new[]
			{
				new ValidationRule<VerificationStep>(ValidateReportTextNotBlank)
			});
		}

		private static TestResult ValidateReportTextNotBlank(VerificationStep step)
    	{
			// none of this applies unless we're transitioning into the completed state
			if (step.State != ActivityStatus.CM)
				return new TestResult(true);

			// check for a non-empty ReportContent property
    		string content;
    		if(!step.ReportPart.ExtendedProperties.TryGetValue("ReportContent", out content) || string.IsNullOrEmpty(content))
				return new TestResult(false, SR.MessageValidateVerifiedReportIsNotBlank);

			// attempt to parse reportContent property as XML, to check for content
			try
			{
				var report = XDocument.Parse(content).Elements("Report").FirstOrDefault();
				if (report == null || string.IsNullOrEmpty(report.Value))
					return new TestResult(false, SR.MessageValidateVerifiedReportIsNotBlank);
			}
			catch(Exception)
			{
				// if we can't parse it, there isn't much we can do but assume it is valid
			}
			return new TestResult(true);
		}
	}
}