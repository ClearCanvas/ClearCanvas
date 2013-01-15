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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
	public class Operations
	{
		public abstract class ReportingOperation
		{
			public abstract bool CanExecute(ReportingProcedureStep step, Staff executingStaff);

			protected PublicationStep CreateScheduledPublicationStep(Staff executingStaff, VerificationStep verification)
			{
				var settings = new ReportingWorkflowSettings();

				var publication = new PublicationStep(verification);
				publication.Assign(executingStaff);
				publication.Schedule(Platform.Time.AddSeconds(settings.PublicationDelay));

				return publication;
			}
		}

		public class SaveReport : ReportingOperation
		{
			public void Execute(ReportingProcedureStep step, Dictionary<string, string> reportPartExtendedProperties, Staff supervisor)
			{
				step.ReportPart.Supervisor = supervisor;

				// if this value is null, it means we don't want to update the report content
				if (reportPartExtendedProperties == null)
					return;

				foreach (var pair in reportPartExtendedProperties)
				{
					step.ReportPart.ExtendedProperties[pair.Key] = pair.Value;
				}
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				// cannot save if no report part
				if (step.ReportPart == null)
					return false;

				// cannot save if the report part is no longer modifiable
				if (!step.ReportPart.IsModifiable)
					return false;

				// only the owner of this step can save
				if (step.PerformingStaff != null && !Equals(step.PerformingStaff, executingStaff))
					return false;

				return true;
			}
		}

		public class StartInterpretation : ReportingOperation
		{
			public void Execute(InterpretationStep step, Staff executingStaff, List<InterpretationStep> linkInterpretations, IWorkflow workflow)
			{
				// if not assigned, assign
				if (step.AssignedStaff == null)
				{
					step.Assign(executingStaff);
				}

				// put in-progress
				step.Start(executingStaff);

				// if a report has not yet been created for this step, create now
				if (step.ReportPart == null)
				{
					var report = new Report(step.Procedure);
					var reportPart = report.ActivePart;

					workflow.AddEntity(report);

					step.ReportPart = reportPart;
					step.ReportPart.Interpreter = executingStaff;
				}

				// attach linked interpretations to this report
				foreach (var interpretation in linkInterpretations)
				{
					interpretation.LinkTo(step);
				}
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				// must be an interpretation step
				if (step.Is<InterpretationStep>() == false)
					return false;

				// must be scheduled
				if (step.State != ActivityStatus.SC)
					return false;

				// must not be assigned to another staff
				if (step.AssignedStaff != null && !Equals(step.AssignedStaff, executingStaff))
					return false;

				return true;
			}
		}

		public class StartTranscriptionReview : ReportingOperation
		{
			public void Execute(TranscriptionReviewStep step, Staff executingStaff)
			{
				step.Start(executingStaff);
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				// must be an interpretation step
				if (step.Is<TranscriptionReviewStep>() == false)
					return false;

				// must be scheduled
				if (step.State != ActivityStatus.SC)
					return false;

				// must not be assigned to another staff
				if (!Equals(step.AssignedStaff, executingStaff))
					return false;

				return true;
			}
		}

		public abstract class CompleteInterpretationBase : ReportingOperation
		{
			protected void UpdateStep(ReportingProcedureStep step, Staff executingStaff)
			{
				if (step.PerformingStaff == null)
					step.Complete(executingStaff);
				else
					step.Complete();

				// move draft report to prelim status
				if (step.ReportPart.Status == ReportPartStatus.D)
					step.ReportPart.MarkPreliminary();
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				if (!step.Is<InterpretationStep>() && !step.Is<TranscriptionReviewStep>())
					return false;

				// interpretation steps must be in progress
				if (step.Is<InterpretationStep>() && step.State != ActivityStatus.IP)
					return false;

				// transcription review step must not be terminated
				if (step.Is<TranscriptionReviewStep>() && step.IsTerminated)
					return false;

				// must not be assigned to someone else
				if (!Equals(step.AssignedStaff, executingStaff))
					return false;

				return true;
			}
		}

		public class CompleteInterpretationForTranscription : CompleteInterpretationBase
		{
			public ReportingProcedureStep Execute(ReportingProcedureStep step, Staff executingStaff, IWorkflow workflow)
			{
				UpdateStep(step, executingStaff);

				// Ensure Supervisor and Reject reason from previous transcriptions of the same report part are
				// removed.
				step.ReportPart.ResetTranscription();

				var transcriptionStep = new TranscriptionStep(step);
				transcriptionStep.Schedule(Platform.Time);
				workflow.AddEntity(transcriptionStep);
				return transcriptionStep;
			}
		}

		public class CompleteInterpretationForVerification : CompleteInterpretationBase
		{
			public ReportingProcedureStep Execute(ReportingProcedureStep step, Staff executingStaff, IWorkflow workflow)
			{
				UpdateStep(step, executingStaff);

				var verificationStep = new VerificationStep(step);

				// supervisor can be null, in which case the verification step is unassigned.
				verificationStep.Assign(step.ReportPart.Supervisor);

				workflow.AddEntity(verificationStep);
				return verificationStep;
			}
		}

		public class CompleteInterpretationAndVerify : CompleteInterpretationBase
		{
			public ReportingProcedureStep Execute(ReportingProcedureStep step, Staff executingStaff, IWorkflow workflow)
			{
				UpdateStep(step, executingStaff);

				var verificationStep = new VerificationStep(step);
				verificationStep.Assign(executingStaff);
				verificationStep.Complete(executingStaff);
				workflow.AddEntity(verificationStep);

				var publicationStep = CreateScheduledPublicationStep(executingStaff, verificationStep);
				workflow.AddEntity(publicationStep);

				return publicationStep;
			}
		}

		public class CancelReportingStep : ReportingOperation
		{
			public List<InterpretationStep> Execute(ReportingProcedureStep step, Staff executingStaff, Staff assignStaff, IWorkflow workflow)
			{
				step.Discontinue();

				// cancel the report part if exists
				if (step.ReportPart != null)
					step.ReportPart.Cancel();

				var interpretationSteps = new List<InterpretationStep>();
				if (!IsAddendumStep(step))
				{
					var procedures = new HashedSet<Procedure> { step.Procedure };

					// if there are linked procedures, schedule a new interpretation for each procedure being reported
					if (step.ReportPart != null)
					{
						procedures.AddAll(step.ReportPart.Report.Procedures);
					}

					// schedule new interpretations
					foreach (var procedure in procedures)
					{
						var interpretationStep = new InterpretationStep(procedure);

						// Bug: #5128 - if the procedure is not document, do not schedule the replacement interpretation step,
						// since interpretation steps aren't scheduled until documentation is complete.
						if (procedure.IsDocumented)
							interpretationStep.Schedule(procedure.PerformedTime);

						if (assignStaff != null)
							interpretationStep.Assign(assignStaff);

						interpretationSteps.Add(interpretationStep);
						workflow.AddEntity(interpretationStep);
					}
				}
				return interpretationSteps;
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				// Publication steps cannot be cancelled after starting
				if (step.Is<PublicationStep>() && step.State != ActivityStatus.SC)
					return false;

				// cannot cancel an unclaimed interpretation step
				if (step.Is<InterpretationStep>() && step.AssignedStaff == null)
					return false;

				// cannot cancel once transcription has begun
				if (step.Is<TranscriptionStep>() && step.State != ActivityStatus.SC)
					return false;

				// cannot cancel a step that is already completed or cancelled
				if (step.IsTerminated)
					return false;

				return true;
			}

			private bool IsAddendumStep(ReportingProcedureStep step)
			{
				return step.ReportPart != null && step.ReportPart.IsAddendum;
			}
		}

		/// <summary>
		/// Residents are not allowed to edit a report that has been scheduled for verification.
		/// This operation cancels the scheduled verification and creates a new interpretation step for the resident to edit.
		/// </summary>
		public class ReviseResidentReport : ReportingOperation
		{
			public InterpretationStep Execute(VerificationStep step, Staff executingStaff, IWorkflow workflow)
			{
				// Cancel the current step
				step.Discontinue();

				// Create a new interpreatation step that uses the same report part
				var interpretationStep = new InterpretationStep(step);

				// Reset the interpretator
				interpretationStep.ReportPart.Interpreter = executingStaff;

				// Assign the new step to the resident
				interpretationStep.Assign(executingStaff);
				interpretationStep.Start(executingStaff);

				workflow.AddEntity(interpretationStep);
				return interpretationStep;
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				if (step.Is<VerificationStep>() == false)
					return false;

				// must be scheduled
				if (step.State != ActivityStatus.SC)
					return false;

				// cannot revise a report that was read by someone else
				if (!Equals(step.ReportPart.Interpreter, executingStaff))
					return false;

				return true;
			}
		}

		/// <summary>
		/// Residents are not allowed to edit a report that has been scheduled for verification or pending publication.
		/// This operation allows radiologist to cancel a step (interpretation step, in progress verification or scheduled publication) 
		/// and creates a new interpretation step for the original interpreter to edit.
		/// </summary>
		public class ReturnToInterpreter : ReportingOperation
		{
			public InterpretationStep Execute(ReportingProcedureStep step, IWorkflow workflow)
			{
				// Cancel the current step
				step.Discontinue();

				// Create a new interpreatation step that uses the same report part
				var interpretationStep = new InterpretationStep(step);

				// Reset the verifier
				interpretationStep.ReportPart.Verifier = null;

				// Assign the new step to the interpreter
				var interpreter = interpretationStep.ReportPart.Interpreter;
				interpretationStep.Assign(interpreter);
				interpretationStep.Schedule(Platform.Time);

				workflow.AddEntity(interpretationStep);
				return interpretationStep;
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				// must be scheduled or in progress
				if (step.State != ActivityStatus.SC && step.State != ActivityStatus.IP)
					return false;

				// Cannot send back a report if it does not exist!
				if (step.ReportPart == null)
					return false;

				// cannot send back a report that was interpreted by the current staff
				if (Equals(step.ReportPart.Interpreter, executingStaff))
					return false;

				return true;
			}
		}

		public class StartVerification : ReportingOperation
		{
			public void Execute(VerificationStep step, Staff executingStaff, IWorkflow workflow)
			{
				// if not assigned, assign
				if (step.AssignedStaff == null)
				{
					step.Assign(executingStaff);
				}

				// put in-progress
				step.Start(executingStaff);
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				if (!step.Is<VerificationStep>())
					return false;

				// must be scheduled
				if (step.State != ActivityStatus.SC)
					return false;

				if (step.AssignedStaff != null && !Equals(executingStaff, step.AssignedStaff))
					return false;

				if (step.ReportPart.Supervisor != null && !Equals(executingStaff, step.ReportPart.Supervisor))
					return false;

				return true;
			}
		}

		public class CompleteVerification : ReportingOperation
		{
			public PublicationStep Execute(VerificationStep step, Staff executingStaff, IWorkflow workflow)
			{
				// this operation is legal even if the step was never started, therefore need to supply the performer
				step.Complete(executingStaff);

				var publicationStep = CreateScheduledPublicationStep(executingStaff, step);
				workflow.AddEntity(publicationStep);

				return publicationStep;
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				if (!step.Is<VerificationStep>())
					return false;

				// must not be already completed or cancelled
				if (step.IsTerminated)
					return false;

				if (step.AssignedStaff != null && !Equals(executingStaff, step.AssignedStaff))
					return false;

				if (step.ReportPart.Supervisor != null && !Equals(executingStaff, step.ReportPart.Supervisor))
					return false;

				return true;
			}
		}

		public class CreateAddendum : ReportingOperation
		{
			public InterpretationStep Execute(Procedure procedure, Staff executingStaff, IWorkflow workflow)
			{
				// the procedure passed in may be any one of the procedures that this report covers
				// ideally, the new interpretation step should be created on the procedure that the
				// publication step was linked to (and only one of the reported procedures should have a publication step)
				procedure = CollectionUtils.SelectFirst(
					procedure.ActiveReport.Procedures,
					p => CollectionUtils.Contains(
						p.ReportingProcedureSteps,
						ps => ps.Is<PublicationStep>() && ps.State == ActivityStatus.CM))

					// but if there are no publication steps (i.e. imported data), then just use the procedure that was provided.
					// See bug #3450
					?? procedure;

				var interpretation = new InterpretationStep(procedure);
				interpretation.Assign(executingStaff);
				interpretation.ReportPart = procedure.ActiveReport.AddAddendum();
				interpretation.ReportPart.Interpreter = executingStaff;
				workflow.AddEntity(interpretation);
				return interpretation;
			}

			public bool CanExecute(Procedure procedure, Staff executingStaff)
			{
				// a procedure must have an active report in order to create an addendum
				if (procedure.ActiveReport == null)
					return false;

				// cannot add a new addendum while there is still an active part
				if (procedure.ActiveReport.ActivePart != null)
					return false;

				// can only create an addendum if all reporting steps for the procedure are terminated
				if (!CollectionUtils.TrueForAll(procedure.ReportingProcedureSteps, ps => ps.IsTerminated))
					return false;

				return true;
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				return CanExecute(step.Procedure, executingStaff);
			}
		}

		public class ReviseUnpublishedReport : ReportingOperation
		{
			public InterpretationStep Execute(PublicationStep step, Staff executingStaff, IWorkflow workflow)
			{
				// Discontinue the publication step
				step.Discontinue();

				// Create a new interpreatation step that uses the same report part
				var interpretationStep = new InterpretationStep(step);

				// Reset the verifier
				interpretationStep.ReportPart.Verifier = null;

				// Assign the new step back to me
				interpretationStep.Assign(executingStaff);
				interpretationStep.Schedule(Platform.Time);

				workflow.AddEntity(interpretationStep);
				return interpretationStep;
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				if (!step.Is<PublicationStep>())
					return false;

				// must be a scheduled publication
				if (step.State != ActivityStatus.SC)
					return false;

				// can only revise reports verified by the same staff
				if (!Equals(step.ReportPart.Verifier, executingStaff))
					return false;

				return true;
			}
		}

		/// <summary>
		/// Publishes the report. 
		/// </summary>
		public class PublishReport : ReportingOperation
		{
			public void Execute(PublicationStep step, Staff executingStaff, IWorkflow workflow)
			{
				if (step.AssignedStaff != null)
					step.Complete(executingStaff);
				else
					step.Complete();
			}

			public override bool CanExecute(ReportingProcedureStep step, Staff executingStaff)
			{
				if (!step.Is<PublicationStep>())
					return false;

				// must be a scheduled publication step
				if (step.State != ActivityStatus.SC)
					return false;

				// can only publish reports verified by the same staff
				if (!Equals(step.ReportPart.Verifier, executingStaff))
					return false;

				return true;
			}
		}
	}
}
