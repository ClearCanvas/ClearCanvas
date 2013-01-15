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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Workflow.Transcription
{
	public class TranscriptionOperations
	{
		public abstract class TranscriptionOperation
		{
			public virtual bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				return false;
			}
		}

		public class StartTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff performingStaff)
			{
				step.Start(performingStaff);
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (!step.IsInitial
					|| step.State == ActivityStatus.IP && !Equals(step.PerformingStaff, currentUserStaff))
					return false;

				if (step.AssignedStaff != null && !Equals(step.AssignedStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class DiscardTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff performingStaff)
			{
				step.Discontinue();
				var transcriptionStep = new TranscriptionStep(step);
				transcriptionStep.Schedule(Platform.Time);
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.IP)
					return false;

				if (!Equals(step.PerformingStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class SaveTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Dictionary<string, string> reportPartExtendedProperties)
			{
				ExtendedPropertyUtils.Update(step.ReportPart.ExtendedProperties, reportPartExtendedProperties);
			}

			public void Execute(TranscriptionStep step, Dictionary<string, string> reportPartExtendedProperties, Staff supervisor)
			{
				this.Execute(step, reportPartExtendedProperties);

				step.ReportPart.TranscriptionSupervisor = supervisor;
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.IP)
					return false;

				if (!Equals(step.PerformingStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class SubmitTranscriptionForReview : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff performingStaff, Staff supervisor)
			{
				step.Complete();

				var transcriptionStep = new TranscriptionStep(step);
				transcriptionStep.Assign(supervisor);
				transcriptionStep.Schedule(Platform.Time);
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.IP)
					return false;

				if (!Equals(step.PerformingStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class CompleteTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff performingStaff)
			{
				if(step.State == ActivityStatus.SC)
					step.Complete(performingStaff);
				else
					step.Complete();

				TranscriptionReviewStep transcriptionReviewStep = new TranscriptionReviewStep(step);
				step.Procedure.AddProcedureStep(transcriptionReviewStep);
				transcriptionReviewStep.Assign(step.ReportPart.Interpreter);
				transcriptionReviewStep.Schedule(Platform.Time);
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.SC && step.State != ActivityStatus.IP)
					return false;

				if (step.State == ActivityStatus.IP && !Equals(step.PerformingStaff, currentUserStaff))
					return false;

				// scheduled items should should look like items submitted for review
				if (step.State == ActivityStatus.SC && (step.ReportPart.Transcriber == null || step.AssignedStaff == null))
					return false;

				// if it looks like a transcription submitted for review, but not to the current user.
				if (step.State == ActivityStatus.SC && step.ReportPart.Transcriber != null && !Equals(step.AssignedStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class RejectTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff rejectedBy, TranscriptionRejectReasonEnum reason)
			{
				if (step.State == ActivityStatus.SC)
					step.Complete(rejectedBy);
				else
					step.Complete();

				step.ReportPart.TranscriptionRejectReason = reason;

				TranscriptionReviewStep transcriptionReviewStep = new TranscriptionReviewStep(step);
				step.Procedure.AddProcedureStep(transcriptionReviewStep);
				transcriptionReviewStep.Assign(step.ReportPart.Interpreter);
				transcriptionReviewStep.Schedule(Platform.Time);
				transcriptionReviewStep.HasErrors = true;
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.SC && step.State != ActivityStatus.IP)
					return false;

				if (step.State == ActivityStatus.IP && !Equals(step.PerformingStaff, currentUserStaff))
					return false;

				// scheduled items should should look like items submitted for review
				if (step.State == ActivityStatus.SC && (step.ReportPart.Transcriber == null || step.AssignedStaff == null))
					return false;

				// if it looks like an transcription submitted for review, but not to the current user.
				if (step.State == ActivityStatus.SC && step.ReportPart.Transcriber != null && !Equals(step.AssignedStaff, currentUserStaff))
					return false;

				return true;
			}
		}
	}
}
