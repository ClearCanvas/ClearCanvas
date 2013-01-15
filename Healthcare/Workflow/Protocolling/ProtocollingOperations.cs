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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Workflow.Protocolling
{
	public class ProtocollingOperations
	{
		public abstract class ProtocollingOperation
		{
			public virtual bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
			{
				return false;
			}

			public virtual bool CanExecute(Order order, Staff currentUserStaff)
			{
				return false;
			}

			protected static T InprogressProcedureStep<T>(Procedure rp) where T : ProcedureStep
			{
				return CurrentProcedureStep<T>(rp, ActivityStatus.IP);
			}

			protected static T ScheduledProcedureStep<T>(Procedure rp) where T : ProcedureStep
			{
				return CurrentProcedureStep<T>(rp, ActivityStatus.SC);
			}

			protected static T CurrentProcedureStep<T>(Procedure rp, ActivityStatus status) where T : ProcedureStep
			{
				ProcedureStep uncastProcedureStep = CollectionUtils.SelectFirst(
					rp.ProcedureSteps,
					delegate(ProcedureStep ps) { return ps.Is<T>() && ps.State == status; });

				return uncastProcedureStep != null ? uncastProcedureStep.Downcast<T>() : null;
			}
		}

		public class StartProtocolOperation : ProtocollingOperation
		{
			public void Execute(ProtocolAssignmentStep assignmentStep, List<ProtocolAssignmentStep> linkedSteps, Staff protocolPerformer, bool canPerformerAcceptProtocols, out bool protocolClaimed, out Staff assignedStaff)
			{
				protocolClaimed = false;
				assignedStaff = null;

				if (assignmentStep.IsInitial)
				{
					assignedStaff = assignmentStep.AssignedStaff;

					// Scheduled assignment step exists (i.e. Protocol has not been claimed), so claim it
					assignmentStep.Start(protocolPerformer);

					// Current user should be set as the supervisor if the protocol is awaiting approval 
					// and the user is able to accept protocols
					if (assignmentStep.Protocol.Status == ProtocolStatus.AA)
					{
						if (!canPerformerAcceptProtocols)
						{
							// User is unable to approve
							//throw new RequestValidationException(SR.ExceptionNoProtocolAssignmentStep);
							throw new Exception("TODO: user unable to approve");
						}
					}
					// otherwise, it's just a new protocol
					else
					{
						assignmentStep.Protocol.Author = protocolPerformer;
					}

					protocolClaimed = true;

					if (linkedSteps != null)
					{
						foreach (ProtocolAssignmentStep step in linkedSteps)
						{
							step.LinkTo(assignmentStep);
						}
					}
				}
				else
				{
					// In-progress assignment step started by someone else
					if (assignmentStep.PerformingStaff != protocolPerformer)
					{
						// So not available to this user to start
						//throw new RequestValidationException(SR.ExceptionNoProtocolAssignmentStep);
						throw new Exception("TODO: protocol already started");
					}

					// otherwise, it's been claimed by this user, so do nothing
				}
			}

			public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.SC)
					return false;

				if (step.Protocol.Status == ProtocolStatus.AA && Equals(step.Protocol.Author, currentUserStaff))
					return false;

				// If supervisor is specified, it must be this user
				if (step.Protocol.Status == ProtocolStatus.AA && step.Protocol.Supervisor != null && !Equals(step.Protocol.Supervisor, currentUserStaff))
					return false;

				return true;
			}
		}

		public class DiscardProtocolOperation : ProtocollingOperation
		{
			public void Execute(ProtocolAssignmentStep assignmentStep, Staff reassignToStaff)
			{
				assignmentStep.Discontinue();

				// if it's an approval step, replace the assignment step but keep the existing protocol
				if (assignmentStep.Protocol.Status == ProtocolStatus.AA)
				{
					// replace the step and unclaim the protocol
					ReplaceAssignmentStep(assignmentStep.Procedure, assignmentStep.Protocol, reassignToStaff);
				}
				// other wise, create a new assignment step with its own new protocol for each procedure in the old protocol
				else
				{
					assignmentStep.Protocol.Cancel();

					List<Procedure> procedures = new List<Procedure>(assignmentStep.Protocol.Procedures);
					foreach (Procedure procedure in procedures)
					{
						Protocol protocol = new Protocol(procedure);
						ReplaceAssignmentStep(procedure, protocol, reassignToStaff);
						PersistenceScope.CurrentContext.Lock(protocol, DirtyState.New);
					}
				}
			}

			private void ReplaceAssignmentStep(Procedure procedure, Protocol protocol, Staff reassignToStaff)
			{
				ProtocolAssignmentStep replacementAssignmentStep = new ProtocolAssignmentStep(protocol);
				procedure.AddProcedureStep(replacementAssignmentStep);
				if (reassignToStaff != null)
				{
					replacementAssignmentStep.Assign(reassignToStaff);
				}

				replacementAssignmentStep.Schedule(Platform.Time);
			}

			public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
			{
				// cannot cancel a step that is already completed or cancelled
				if (step.IsTerminated)
					return false;

				// can only cancel scheduled pending and approval steps
				if (step.State == ActivityStatus.SC && 
					step.Protocol.Status != ProtocolStatus.AA &&
					step.Protocol.Status != ProtocolStatus.PN)
					return false;

				// no need to cancel unassigned pending or approval steps
				if (step.State == ActivityStatus.SC 
					&& (step.Protocol.Status == ProtocolStatus.PN || step.Protocol.Status == ProtocolStatus.AA)
					&& step.AssignedStaff == null)
					return false;

				return true;
			}
		}

		public class AcceptProtocolOperation : ProtocollingOperation
		{
			public void Execute(ProtocolAssignmentStep assignmentStep, Staff acceptedBy)
			{
				if (assignmentStep.State == ActivityStatus.IP)
					assignmentStep.Complete();
				else
					assignmentStep.Complete(acceptedBy);
				assignmentStep.Protocol.Accept();
			}

			public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
			{
				if (step.PerformingStaff != null && !Equals(step.PerformingStaff, currentUserStaff))
					return false;

				if (step.Protocol.Status == ProtocolStatus.PR || step.Protocol.Status == ProtocolStatus.RJ)
					return false;

				if (step.Protocol.Status == ProtocolStatus.PN && step.State != ActivityStatus.IP)
					return false;

				if (step.Protocol.Status == ProtocolStatus.AA && (step.State != ActivityStatus.SC && step.State != ActivityStatus.IP))
					return false;

				// Cannot approve own submitted items
				if (step.Protocol.Status == ProtocolStatus.AA && step.Protocol.Supervisor == null && Equals(step.Protocol.Author, currentUserStaff))
					return false;

				// If supervisor is specified, it must be this user
				if (step.Protocol.Status == ProtocolStatus.AA && step.Protocol.Supervisor != null && !Equals(step.Protocol.Supervisor, currentUserStaff))
					return false;

				return true;
			}
		}

		public class RejectProtocolOperation : ProtocollingOperation
		{
			public void Execute(ProtocolAssignmentStep assignmentStep, Staff rejectedBy, ProtocolRejectReasonEnum reason)
			{
				if (assignmentStep.State == ActivityStatus.SC)
				{
					assignmentStep.Start(rejectedBy);
				}
				assignmentStep.Discontinue();
				assignmentStep.Protocol.Reject(reason);

				// TODO: one resolution step or one per procedure?
				ProtocolResolutionStep resolutionStep = new ProtocolResolutionStep(assignmentStep.Protocol);
				assignmentStep.Procedure.AddProcedureStep(resolutionStep);
			}

			public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
			{
				if (step.PerformingStaff != null && !Equals(step.PerformingStaff, currentUserStaff))
					return false;

				if (step.Protocol.Status == ProtocolStatus.PR || step.Protocol.Status == ProtocolStatus.RJ)
					return false;

				if (step.Protocol.Status == ProtocolStatus.PN && step.State != ActivityStatus.IP)
					return false;

				if (step.Protocol.Status == ProtocolStatus.AA && (step.State != ActivityStatus.SC && step.State != ActivityStatus.IP))
					return false;

				// Cannot approve own submitted items
				if (step.Protocol.Status == ProtocolStatus.AA && step.Protocol.Supervisor == null && Equals(step.Protocol.Author, currentUserStaff))
					return false;

				// If supervisor is specified, it must be this user
				if (step.Protocol.Status == ProtocolStatus.AA && step.Protocol.Supervisor != null && !Equals(step.Protocol.Supervisor, currentUserStaff))
					return false;

				return true;
			}
		}

		public class ResubmitProtocolOperation : ProtocollingOperation
		{
			public void Execute(Order order, Staff resolvingStaff)
			{
				foreach (Procedure rp in order.Procedures)
				{
					ProtocolResolutionStep resolutionStep = ScheduledProcedureStep<ProtocolResolutionStep>(rp);

					if (resolutionStep != null)
					{
						resolutionStep.Complete(resolvingStaff);
						resolutionStep.Protocol.Resolve();
						ProtocolAssignmentStep assignmentStep = new ProtocolAssignmentStep(resolutionStep.Protocol);
						rp.AddProcedureStep(assignmentStep);
					}
				}
			}

			public override bool CanExecute(Order order, Staff currentUserStaff)
			{
				foreach (Procedure rp in order.Procedures)
				{
					ProtocolResolutionStep resolutionStep = ScheduledProcedureStep<ProtocolResolutionStep>(rp);

					if (resolutionStep != null)
					{
						if (resolutionStep.State == ActivityStatus.SC)
							return true;
					}
				}

				return false;
			}
		}

		public class SubmitForApprovalOperation : ProtocollingOperation
		{
			public void Execute(ProtocolAssignmentStep assignmentStep)
			{
				assignmentStep.Complete();
				assignmentStep.Protocol.SubmitForApproval();

				// Replace with new step scheduled step
				ProtocolAssignmentStep approvalStep = new ProtocolAssignmentStep(assignmentStep.Protocol);
				assignmentStep.Procedure.AddProcedureStep(approvalStep);

				approvalStep.Schedule(Platform.Time);
				approvalStep.Assign(assignmentStep.Protocol.Supervisor);
			}

			public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
			{
				if (step.PerformingStaff != null && !Equals(step.PerformingStaff, currentUserStaff))
					return false;

				if (step.State != ActivityStatus.IP)
					return false;

				if (step.Protocol.Status != ProtocolStatus.PN)
					return false;

				return true;
			}
		}

		public class ReviseSubmittedProtocolOperation : ProtocollingOperation
		{
			public ProtocolAssignmentStep Execute(ProtocolAssignmentStep assignmentStep, Staff author)
			{
				assignmentStep.Protocol.Status = ProtocolStatus.PN;
				assignmentStep.Protocol.Supervisor = null;
				assignmentStep.Discontinue();

				// Replace with new step scheduled step
				ProtocolAssignmentStep replacementStep = new ProtocolAssignmentStep(assignmentStep.Protocol);
				assignmentStep.Procedure.AddProcedureStep(replacementStep);
				replacementStep.Assign(author);
				replacementStep.Start(author);

				return replacementStep;
			}

			public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.SC)
					return false;

				if (step.Protocol.Status != ProtocolStatus.AA)
					return false;

				if (!Equals(step.Protocol.Author, currentUserStaff))
					return false;

				return true;
			}
		}
	}
}