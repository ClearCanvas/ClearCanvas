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

using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{


	/// <summary>
	/// TranscriptionStep entity
	/// </summary>
	public partial class TranscriptionStep : ReportingProcedureStep
	{

		public TranscriptionStep(ReportingProcedureStep previousStep)
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
			get { return "Transcription"; }
		}

		protected override void OnStateChanged(ActivityStatus previousState, ActivityStatus newState)
		{
			if (newState == ActivityStatus.CM)
			{
				if (this.ReportPart == null)
					throw new WorkflowException("This ReportingStep does not have an associated ReportPart.");

				// When a supervisor completes a submitted transcription, do not overwrite the original transcriber.
				if (this.ReportPart.Transcriber == null)
					this.ReportPart.Transcriber = this.PerformingStaff;
			}

			base.OnStateChanged(previousState, newState);
		}

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new TranscriptionStep(this);
		}
	}
}
