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
using System.Linq;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Procedure entity
	/// </summary>
	[Validation(HighLevelRulesProviderMethod = "GetValidationRules")]
	public partial class Procedure
	{
		public Procedure(ProcedureType type, string procedureNumber, string studyInstanceUid)
		{
			_type = type;
			_number = procedureNumber;
			_procedureSteps = new HashedSet<ProcedureStep>();
			_procedureCheckIn = new ProcedureCheckIn();
			_reports = new HashedSet<Report>();
			_protocols = new HashedSet<Protocol>();
			_scheduledDuration = type.DefaultDuration;
			_studyInstanceUID = studyInstanceUid;
		}

		#region Public Properties

		/// <summary>
		/// Gets the patient profile associated with the performing facility of this procedure.
		/// </summary>
		public virtual PatientProfile PatientProfile
		{
			get
			{
				return CollectionUtils.SelectFirst(_order.Patient.Profiles,
					profile => Equals(profile.Mrn.AssigningAuthority, _performingFacility.InformationAuthority));
			}
		}

		/// <summary>
		/// Returns true if this procedure is pre check-in (patient has not yet checked-in).
		/// </summary>
		public virtual bool IsPreCheckIn
		{
			get { return _procedureCheckIn.IsPreCheckIn; }
		}

		/// <summary>
		/// Returns true if the patient is currently checked-in for this procedure.
		/// </summary>
		public virtual bool IsCheckedIn
		{
			get { return !IsPreCheckIn && !IsCheckedOut; }
		}

		/// <summary>
		/// Returns true if the patient has checked-out for this procedure.
		/// </summary>
		public virtual bool IsCheckedOut
		{
			get { return _procedureCheckIn.IsCheckedOut; }
		}
		/// <summary>
		/// Gets the modality procedure steps.
		/// </summary>
		public virtual List<ModalityProcedureStep> ModalityProcedureSteps
		{
			get
			{
				return CollectionUtils.Map<ProcedureStep, ModalityProcedureStep>(
					CollectionUtils.Select(this.ProcedureSteps, ps => ps.Is<ModalityProcedureStep>()),
					ps => ps.As<ModalityProcedureStep>());
			}
		}

		/// <summary>
		/// Gets the reporting procedure steps.
		/// </summary>
		public virtual List<ReportingProcedureStep> ReportingProcedureSteps
		{
			get
			{
				return CollectionUtils.Map<ProcedureStep, ReportingProcedureStep>(
					CollectionUtils.Select(this.ProcedureSteps, ps => ps.Is<ReportingProcedureStep>()),
					ps => ps.As<ReportingProcedureStep>());
			}
		}

		/// <summary>
		/// Gets a value indicating whether this procedure is in a terminal state.
		/// </summary>
		public virtual bool IsTerminated
		{
			get
			{
				return _status == ProcedureStatus.CM || _status == ProcedureStatus.CA || _status == ProcedureStatus.DC || _status == ProcedureStatus.GH;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this procedure is in a defunct state - that is, cancelled, discontinued, or ghost.
		/// </summary>
		public virtual bool IsDefunct
		{
			get
			{
				return _status == ProcedureStatus.CA || _status == ProcedureStatus.DC || _status == ProcedureStatus.GH;
			}
		}

		/// <summary>
		/// Gets the documentation procedure step, or null if it does not exist.
		/// </summary>
		public virtual DocumentationProcedureStep DocumentationProcedureStep
		{
			get
			{
				var step = CollectionUtils.SelectFirst(this.ProcedureSteps, ps => ps.Is<DocumentationProcedureStep>());

				return step == null ? null : step.Downcast<DocumentationProcedureStep>();
			}
		}

		/// <summary>
		/// Gets the active <see cref="Report"/> for this procedure, or returns null if there is no active report.
		/// </summary>
		public virtual Report ActiveReport
		{
			get
			{
				return CollectionUtils.SelectFirst(_reports, r => r.Status != ReportStatus.X);
			}
		}

		/// <summary>
		/// Gets the active <see cref="Protocol"/> for this procedure, or returns null if there is no active protocol.
		/// </summary>
		public virtual Protocol ActiveProtocol
		{
			get
			{
				return CollectionUtils.SelectFirst(_protocols, p => p.Status != ProtocolStatus.X);
			}
		}

		/// <summary>
		/// Gets the time at which the procedure can be considered "performed", which corresponds to the maximum
		/// completed modality procedure step end-time.
		/// </summary>
		/// <remarks>
		/// This is a computed property and hence should not be used in summaries.
		/// </remarks>
		public virtual DateTime? PerformedTime
		{
			get
			{
				var completedSteps = CollectionUtils.Select(this.ModalityProcedureSteps, mps => mps.State == ActivityStatus.CM);

				// return the max end-time over all completed MPS
				var maxMps = CollectionUtils.Max(
					completedSteps,
					null,
					(mps1, mps2) => Nullable.Compare(mps1.EndTime, mps2.EndTime));

				return maxMps != null ? maxMps.EndTime : null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this procedure can be considered "performed", meaning all modality procedure
		/// steps have been terminated, and at least one has been completed.
		/// </summary>
		/// <remarks>
		/// This is a computed property and hence should not be used in summaries.
		/// </remarks>
		public virtual bool IsPerformed
		{
			get
			{
				// return true if all MPS are terminated and at least one is completed
				return this.ModalityProcedureSteps.TrueForAll(mps => mps.IsTerminated)
					&& this.ModalityProcedureSteps.Exists(ps => ps.State == ActivityStatus.CM);
			}
		}

		/// <summary>
		/// Gets a value indicating whethe the documentation step for this procedure is complete.
		/// </summary>
		public virtual bool IsDocumented
		{
			get { return this.DocumentationProcedureStep.State == ActivityStatus.CM; }
		}

		#endregion

		#region Public Operations

		/// <summary>
		/// Gets all procedure steps matching the specified predicate.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public virtual List<ProcedureStep> GetProcedureSteps(Predicate<ProcedureStep> predicate)
		{
			return CollectionUtils.Select(_procedureSteps, predicate);
		}

		/// <summary>
		/// Gets the first procedure step matching the specified predicate.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public virtual ProcedureStep GetProcedureStep(Predicate<ProcedureStep> predicate)
		{
			return CollectionUtils.SelectFirst(_procedureSteps, predicate);
		}

		/// <summary>
		/// Creates the procedure steps specified in the procedure plan of the associated
		/// <see cref="ProcedureType"/>.
		/// </summary>
		public virtual void CreateProcedureSteps()
		{
			// TODO: is this the right way to check this condition?  do we need a dedicated flag?
			if (_procedureSteps.Count > 0)
				throw new WorkflowException("Procedure steps have already been created for this Procedure.");

			var builder = new ProcedureBuilder();
			builder.BuildProcedureFromPlan(this);
		}

		/// <summary>
		/// Adds a procedure step.  Use this method rather than adding directly to the <see cref="ProcedureSteps"/>
		/// collection.
		/// </summary>
		/// <param name="step"></param>
		public virtual void AddProcedureStep(ProcedureStep step)
		{
			if ((step.Procedure != null && step.Procedure != this) || step.State != ActivityStatus.SC)
				throw new ArgumentException("Only new ProcedureStep objects may be added to a procedure.");

			step.Procedure = this;
			this.ProcedureSteps.Add(step);
		}

		/// <summary>
		/// Schedules or re-schedules all procedure steps to start at the specified time.
		/// Applicable only if this object is in the SC status.
		/// </summary>
		/// <param name="startTime"></param>
		/// <remarks>This overload does not modify the duration of the procedure.</remarks>
		public virtual void Schedule(DateTime? startTime)
		{
			Schedule(startTime, _scheduledDuration);
		}

		/// <summary>
		/// Schedules or re-schedules all procedure steps to start at the specified time, and for the specified duration.
		/// Applicable only if this object is in the SC status.
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="duration">Duration in minutes.</param>
		public virtual void Schedule(DateTime? startTime, int duration)
		{
			if (_status != ProcedureStatus.SC)
				throw new WorkflowException("Only procedures in the SC status may be scheduled or re-scheduled.");

			// if the procedure steps have not been created, create them now
			if (_procedureSteps.Count == 0)
			{
				this.CreateProcedureSteps();
			}

			// Schedule each step appropriately based on the its SchedulingOffset.
			foreach (var ps in _procedureSteps.Where(ps => ps.State == ActivityStatus.SC))
			{
				if (ps.SchedulingOffset == TimeSpan.MinValue)
				{
					// Make sure the step is scheduled at creation time.
					if (ps.StartTime == null)
						ps.Schedule(ps.CreationTime);
				}
				else if (ps.SchedulingOffset == TimeSpan.MaxValue)
				{
					// ignore.  The step schedule time is not dependent on procedure being scheduled.
				}
				else
				{
					// Schedule the step using its offset
					if (startTime != null)
					{
						// truncate to minute, and add ps offset
						var t = startTime.Value.Truncate(DateTimePrecision.Minute).Add(ps.SchedulingOffset);
						ps.Schedule(t);
					}
					else
					{
						ps.Schedule(null);
					}
				}
			}

			_scheduledDuration = duration > 0 ? duration : ComputeDefaultDuration();
			_scheduledEndTime = _scheduledStartTime + TimeSpan.FromMinutes(_scheduledDuration);
		}

		/// <summary>
		/// Check in the procedure, optionally specifying a check-in time.  If not specified,
		/// the current time is assumed.
		/// </summary>
		public virtual void CheckIn(Staff checkInStaff, DateTime? checkInTime)
		{
			_procedureCheckIn.CheckIn(checkInTime);

			// start the registration step, if not started
			var regStep = GetProcedureStep(ps => ps.Is<RegistrationProcedureStep>());
			if (regStep != null && regStep.State == ActivityStatus.SC)
				regStep.Start(checkInStaff, checkInTime);
		}

		/// <summary>
		/// Check out the procedure, optionally specifying a check-out time.  If not specified,
		/// the current time is assumed.
		/// </summary>
		public virtual void CheckOut(DateTime? checkOutTime)
		{
			_procedureCheckIn.CheckOut(checkOutTime);

			// complete the registration step, if not completed
			var regStep = GetProcedureStep(ps => ps.Is<RegistrationProcedureStep>());
			if (regStep != null && !regStep.IsTerminated)
				regStep.Complete(checkOutTime);
		}

		/// <summary>
		/// Reverts Check-In status if not already checked out
		/// </summary>
		public virtual void RevertCheckIn()
		{
			_procedureCheckIn.RevertCheckIn();

			// JR: technically we should probably discontinue the registration step and schedule a new one here
			// but that would create additional complexity and there is no real need for it right now
		}

		/// <summary>
		/// Discontinue this procedure and any non-terminated procedure steps.
		/// </summary>
		public virtual void Discontinue()
		{
			if (_status != ProcedureStatus.IP)
				throw new WorkflowException("Only procedures in the IP status can be discontinued");

			// update the status prior to cancelling the procedure steps
			// (otherwise cancelling the steps will cause them to try and update the procedure status)
			SetStatus(ProcedureStatus.DC);

			// discontinue any non-terminated procedure steps
			foreach (var ps in _procedureSteps)
			{
				if (!ps.IsTerminated)
					ps.Discontinue();
			}

			// need to update the end-time again, after discontinuing procedure steps
			UpdateEndTime();
		}

		/// <summary>
		/// Cancel this procedure and all procedure steps.
		/// </summary>
		public virtual void Cancel()
		{
			if (_status != ProcedureStatus.SC)
				throw new WorkflowException("Only procedures in the SC status can be cancelled.");

			// update the status prior to cancelling the procedure steps
			// (otherwise cancelling the steps will cause them to try and update the procedure status)
			SetStatus(ProcedureStatus.CA);

			// discontinue all procedure steps (they should all be in the SC status)
			foreach (var ps in _procedureSteps)
			{
				// except PreSteps, which may already be in a terminal state, so ignore them if that's the case.
				// Bug: #1525
				if (ps.IsPreStep && ps.IsTerminated)
					continue;

				ps.Discontinue();
			}

			// need to update the end-time again, after discontinuing procedure steps
			UpdateEndTime();
		}

		/// <summary>
		/// Creates a ghost copy of this procedure.
		/// </summary>
		/// <remarks>
		/// The ghost copy is what remains attached to an order that was merged into another order.
		/// It has no procedure steps, reports, protocols, etc.
		/// </remarks>
		/// <returns></returns> 
		public virtual Procedure CreateGhostCopy()
		{
			return new Procedure(
				_order,
				_type,
				_number,
				new HashedSet<ProcedureStep>(),
				_scheduledStartTime,
				_scheduledDuration,
				_scheduledEndTime,
				_schedulingCode,
				_startTime,
				_endTime,
				ProcedureStatus.GH,
				_performingFacility,
				_performingDepartment,
				_laterality,
				_portable,
				new ProcedureCheckIn(),
				_imageAvailability,
				_downtimeRecoveryMode,
				_studyInstanceUID,
				new HashedSet<Report>(),
				new HashedSet<Protocol>(),
				this);
		}

		/// <summary>
		/// Gets the full history of this procedure, including procedure steps that 
		/// are associated indirectly via linked workflows.
		/// </summary>
		/// <returns></returns>
		public virtual List<ProcedureStep> GetWorkflowHistory()
		{
			var x = new List<ProcedureStep>(_procedureSteps);
			var history = new List<ProcedureStep>(x);
			while (x.Count > 0)
			{
				// obtain all procedure steps that are linked via steps in x
				var y = CollectionUtils.Concat<ProcedureStep>(
						CollectionUtils.Map<ProcedureStep, List<ProcedureStep>>(
							x,
							step => step.IsLinked
								? step.LinkStep.GetRelatedProcedureSteps()
								: new List<ProcedureStep>())
						.ToArray());

				history.AddRange(y);

				// set x = y so that the next time through the loop,
				// we follow the next level of linking
				x = y;
			}
			return history;
		}

		#endregion

		#region Helper methods

		/// <summary>
		/// Called by a child procedure step to complete this procedure.
		/// </summary>
		protected internal virtual void Complete(DateTime completeTime)
		{
			if (_status != ProcedureStatus.IP)
				throw new WorkflowException("Only procedures in the IP status can be completed");

			SetStatus(ProcedureStatus.CM);

			// over-write the end-time with actual completed time
			// TODO: this is a bit of a hack to deal with linked procedures, and the fact that
			// the final ProcedureStep may not exist in our ProcedureSteps collection, 
			// if this procedure was linked to another for reporting.
			// Ideally we should get rid of this at some point, when we build in better tracking
			// of procedure linkages
			_endTime = completeTime;
		}

		/// <summary>
		/// Called by child procedure steps to tell this procedure to update its scheduling information.
		/// </summary>
		protected internal virtual void UpdateScheduling()
		{
			// compute the earliest procedure step scheduled start time (exclude pre-steps)
			_scheduledStartTime = MinMaxHelper.MinValue(
				_procedureSteps,
				ps => !ps.IsPreStep,
				ps => ps.Scheduling == null ? null : ps.Scheduling.StartTime,
				null);

			// the order should never be null, unless this is a brand new instance that has not yet been assigned an order
			if (_order != null)
				_order.UpdateScheduling();
		}

		/// <summary>
		/// Called by a child procedure step to tell the procedure to update its status.  Only
		/// certain status updates can be inferred deterministically from child statuses.  If no
		/// status can be inferred, the status does not change.
		/// </summary>
		protected internal virtual void UpdateStatus()
		{
			// check if the procedure should be auto-discontinued
			if (_status == ProcedureStatus.SC || _status == ProcedureStatus.IP)
			{
				// if all steps are discontinued, this procedure is automatically discontinued
				// Bug: #2471 only consider Modality Procedure Steps for now, although in the long run this is not a good solution
				if (CollectionUtils.TrueForAll(this.ModalityProcedureSteps, step => step.State == ActivityStatus.DC))
				{
					SetStatus(ProcedureStatus.DC);
				}
			}

			// check if the procedure should be auto-started
			if (_status == ProcedureStatus.SC)
			{
				// the condition for auto-starting the procedure is that it has a (non-pre) procedure step that has
				// moved out of the scheduled status but not into the discontinued status
				var anyStepStartedNotDiscontinued = CollectionUtils.Contains(
					_procedureSteps,
					step => !step.IsPreStep && !step.IsInitial && step.State != ActivityStatus.DC);

				if (anyStepStartedNotDiscontinued)
				{
					SetStatus(ProcedureStatus.IP);
				}
			}
		}

		/// <summary>
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.
		/// </remarks>
		/// <param name="minutes"></param>
		protected internal virtual void TimeShift(int minutes)
		{
			_scheduledStartTime = _scheduledStartTime.HasValue ? _scheduledStartTime.Value.AddMinutes(minutes) : _scheduledStartTime;
			_startTime = _startTime.HasValue ? _startTime.Value.AddMinutes(minutes) : _startTime;
			_endTime = _endTime.HasValue ? _endTime.Value.AddMinutes(minutes) : _endTime;

			if (_procedureCheckIn != null)
			{
				_procedureCheckIn.TimeShift(minutes);
			}

			foreach (var protocol in _protocols)
			{
				protocol.TimeShift(minutes);
			}

			foreach (var step in _procedureSteps)
			{
				step.TimeShift(minutes);
			}

			foreach (var report in _reports)
			{
				report.TimeShift(minutes);
			}
		}

		/// <summary>
		/// Helper method to change the status and also notify the parent order to change its status
		/// if necessary.
		/// </summary>
		/// <param name="status"></param>
		private void SetStatus(ProcedureStatus status)
		{
			if (status != _status)
			{
				_status = status;

				if (_status == ProcedureStatus.IP)
					UpdateStartTime();

				if (this.IsTerminated)
					UpdateEndTime();

				// Cancelled/discontinued procedures should not be left in downtime recovery mode.
				if (_status == ProcedureStatus.CA || _status == ProcedureStatus.DC)
					this.DowntimeRecoveryMode = false;

				// the order should never be null, unless this is a brand new instance that has not yet been assigned an order
				if (_order != null)
					_order.UpdateStatus();
			}
		}

		private void UpdateStartTime()
		{
			// compute the earliest procedure step start time
			_startTime = MinMaxHelper.MinValue(
				_procedureSteps,
				ps => !ps.IsPreStep,
				ps => ps.StartTime,
				null);
		}

		private void UpdateEndTime()
		{
			// compute the latest procedure step end time
			_endTime = MinMaxHelper.MaxValue(
				_procedureSteps,
				ps => true,
				ps => ps.EndTime,
				null);
		}

		private int ComputeDefaultDuration()
		{
			if (!_scheduledStartTime.HasValue)
				return 0;

			// todo Yen: I don't think the Scheduling.EndTime field is ever populated
			// hence this will ever return anything other than 0
			var endTime = _procedureSteps.Max(ps => ps.Scheduling.EndTime);
			return endTime.HasValue ? (int)(endTime.Value - _scheduledStartTime.Value).TotalMinutes : 0;
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		#endregion

		private static IValidationRuleSet GetValidationRules()
		{
			var sameInformationAuthorityRule = new ValidationRule<Procedure>(
				procedure => OrderRules.VisitAndPerformingFacilitiesHaveSameInformationAuthority(procedure.Order));

			var samePerformingFacilityRule = new ValidationRule<Procedure>(
				procedure => OrderRules.AllNonDefunctProceduresHaveSamePerformingFacility(procedure.Order));

			var samePerformingDepartmentRule = new ValidationRule<Procedure>(
				procedure => OrderRules.AllNonDefunctProceduresHaveSamePerformingDepartment(procedure.Order));

			// performing department must be associated with performing facility
			var performingDepartmentIsInPerformingFacilityRule = new ValidationRule<Procedure>(
				OrderRules.PerformingDepartmentAlignsWithPerformingFacility);

			// modalities must be associated with performing facility
			var modalitiesAlignWithPerformingFacilityRule = new ValidationRule<Procedure>(
				OrderRules.ModalitiesAlignWithPerformingFacility);

			// patient must have a profile at the performing facility
			var patientProfileExistsForPerformingFacilityRule = new ValidationRule<Procedure>(
				OrderRules.PatientProfileExistsForPerformingFacility);

			return new ValidationRuleSet(new[]
			{
				sameInformationAuthorityRule,
				samePerformingFacilityRule, 
				samePerformingDepartmentRule, 
				performingDepartmentIsInPerformingFacilityRule,
				modalitiesAlignWithPerformingFacilityRule,
				patientProfileExistsForPerformingFacilityRule
			});
		}

	}
}
