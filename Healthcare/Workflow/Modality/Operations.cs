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
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Modality
{
	public abstract class ModalityOperation
	{
		/// <summary>
		/// Helper method implements some fuzzy logic to try and determine whether the procedure
		/// should be checked-out, and check it out if necessary.
		/// </summary>
		/// <param name="rp"></param>
		/// <param name="timestamp"></param>
		protected void TryAutoCheckOut(Procedure rp, DateTime? timestamp)
		{
			if (rp.IsCheckedIn)
			{
				var allMpsTerminated = rp.ModalityProcedureSteps.TrueForAll(mps => mps.IsTerminated);
				if (allMpsTerminated)
				{
					// auto check-out
					rp.CheckOut(timestamp);
				}
			}
		}

		protected void TryAutoTerminateProcedureSteps(Procedure procedure, DateTime? time, IWorkflow workflow)
		{
			foreach (var mps in procedure.ModalityProcedureSteps)
			{
				// if the MPS is not terminated and has some MPPS
				if(!mps.IsTerminated && !mps.PerformedSteps.IsEmpty)
				{
					var allMppsDiscontinued = CollectionUtils.TrueForAll(mps.PerformedSteps,
						(PerformedProcedureStep pps) => pps.State == PerformedStepStatus.DC);
					var allMppsTerminated = CollectionUtils.TrueForAll(mps.PerformedSteps,
						(PerformedProcedureStep pps) => pps.IsTerminated);

					if (allMppsDiscontinued)
					{
						// discontinue MPS, since all MPPS are discontinued
						mps.Discontinue(time);
					}
					else if (allMppsTerminated)
					{
						// all MPPS are terminated, and at least one MPPS must be completed, so complete MPS
						mps.Complete(time);
					}
				}
			}
		}
	}

	public class StartModalityProcedureStepsOperation : ModalityOperation
	{
		public ModalityPerformedProcedureStep Execute(IList<ModalityProcedureStep> modalitySteps, DateTime? startTime, Staff technologist, IWorkflow workflow)
		{
			if (modalitySteps.Count == 0)
				throw new WorkflowException("At least one procedure step is required.");

			// validate that each mps being started is being performed on the same modality
			if (!CollectionUtils.TrueForAll(modalitySteps, step => step.Modality.Equals(modalitySteps[0].Modality)))
			{
				throw new WorkflowException("Procedure steps cannot be started together because they are not on the same modality.");
			}

			// create an mpps
			var mpps = new ModalityPerformedProcedureStep(technologist, startTime);
			workflow.AddEntity(mpps);

			foreach (var mps in modalitySteps)
			{
				mps.Start(technologist, startTime);
				mps.AddPerformedStep(mpps);

				//note: this feature was disabled by request (see #2138) - they want to enforce explicit check-in
				//AutoCheckIn(mps.Procedure, startTime);
			}

			// Create Documentation Step for each RP that has an MPS started by this service call
			foreach (var step in modalitySteps)
			{
				if (step.Procedure.DocumentationProcedureStep == null)
				{
					ProcedureStep docStep = new DocumentationProcedureStep(step.Procedure);
					docStep.Start(technologist, startTime);
					workflow.AddEntity(docStep);
				}
			}

			return mpps;
		}
	}

	public class DiscontinueModalityProcedureStepOperation : ModalityOperation
	{
		public void Execute(ModalityProcedureStep mps, DateTime? discontinueTime, IWorkflow workflow)
		{
			mps.Discontinue(discontinueTime);
			TryAutoCheckOut(mps.Procedure, discontinueTime);
		}
	}

	public abstract class TerminateModalityPerformedProcedureStepOperation : ModalityOperation
	{
		public void Execute(ModalityPerformedProcedureStep mpps, DateTime? time, IWorkflow workflow)
		{
			TerminatePerformedProcedureStep(mpps, time);

			var oneMps = CollectionUtils.FirstElement<ProcedureStep>(mpps.Activities).As<ModalityProcedureStep>();
			var order = oneMps.Procedure.Order;

			// try to complete any mps that have all mpps completed
			foreach (var rp in order.Procedures)
			{
				if (rp.Status != ProcedureStatus.IP)
					continue;

				TryAutoTerminateProcedureSteps(rp, time, workflow);
				TryAutoCheckOut(rp, time);
			}
		}

		protected abstract void TerminatePerformedProcedureStep(ModalityPerformedProcedureStep mpps, DateTime? time);

	}

	public class CompleteModalityPerformedProcedureStepOperation : TerminateModalityPerformedProcedureStepOperation
	{
		protected override void TerminatePerformedProcedureStep(ModalityPerformedProcedureStep mpps, DateTime? time)
		{
			mpps.Complete(time);
		}
	}

	public class DiscontinueModalityPerformedProcedureStepOperation : TerminateModalityPerformedProcedureStepOperation
	{
		protected override void TerminatePerformedProcedureStep(ModalityPerformedProcedureStep mpps, DateTime? time)
		{
			mpps.Discontinue(time);
		}
	}
}
