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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;
using ClearCanvas.Healthcare.Workflow.Reporting;

namespace ClearCanvas.Healthcare
{
	[WorklistProcedureTypeGroupClass(typeof(ReadingGroup))]
	[WorklistCategory("WorklistCategoryReporting")]
	public abstract class ReportingWorklist : Worklist
	{
		private WorklistStaffFilter _interpretedByStaffFilter;
		private WorklistStaffFilter _transcribedByStaffFilter;
		private WorklistStaffFilter _verifiedByStaffFilter;
		private WorklistStaffFilter _supervisedByStaffFilter;

		protected ReportingWorklist()
		{
			_interpretedByStaffFilter = new WorklistStaffFilter();
			_transcribedByStaffFilter = new WorklistStaffFilter();
			_verifiedByStaffFilter = new WorklistStaffFilter();
			_supervisedByStaffFilter = new WorklistStaffFilter();
		}

		#region Public Properties

		/// <summary>
		/// Gets or sets the <see cref="WorklistStaffFilter"/> for Interpreted By staff.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
		public virtual WorklistStaffFilter InterpretedByStaffFilter
		{
			get { return _interpretedByStaffFilter; }
			protected set { _interpretedByStaffFilter = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistStaffFilter"/> for Transcribed By staff.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
		public virtual WorklistStaffFilter TranscribedByStaffFilter
		{
			get { return _transcribedByStaffFilter; }
			protected set { _transcribedByStaffFilter = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistStaffFilter"/> for Verified By staff.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
		public virtual WorklistStaffFilter VerifiedByStaffFilter
		{
			get { return _verifiedByStaffFilter; }
			protected set { _verifiedByStaffFilter = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistStaffFilter"/> for Supervisor staff.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
		public virtual WorklistStaffFilter SupervisedByStaffFilter
		{
			get { return _supervisedByStaffFilter; }
			protected set { _supervisedByStaffFilter = value; }
		}

		#endregion

		public override IList GetWorklistItems(IWorklistQueryContext wqc)
		{
			return (IList)wqc.GetBroker<IReportingWorklistItemBroker>().GetWorklistItems<ReportingWorklistItem>(this, wqc);
		}

		public override string GetWorklistItemsHql(IWorklistQueryContext wqc)
		{
			return wqc.GetBroker<IReportingWorklistItemBroker>().GetWorklistItemsHql(this, wqc);
		}

		public override int GetWorklistItemCount(IWorklistQueryContext wqc)
		{
			return wqc.GetBroker<IReportingWorklistItemBroker>().CountWorklistItems(this, wqc);
		}

		protected override WorklistItemProjection GetProjectionCore(WorklistItemField timeField)
		{
			return WorklistItemProjection.GetReportingProjection(timeField);
		}

		protected override WorklistItemSearchCriteria[] GetFilterCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();

			// apply base filters
			ApplyFilterCriteria(criteria, wqc);

			// add reporting-specific filters
			if (GetSupportsReportingStaffRoleFilter(GetClass()))
			{
				this.InterpretedByStaffFilter.Apply(criteria.ReportPart.Interpreter, wqc);
				this.TranscribedByStaffFilter.Apply(criteria.ReportPart.Transcriber, wqc);
				this.VerifiedByStaffFilter.Apply(criteria.ReportPart.Verifier, wqc);
				this.SupervisedByStaffFilter.Apply(criteria.ReportPart.Supervisor, wqc);
			}

			return new[] { criteria };
		}

	}

	/// <summary>
	/// ReportingToBeReportedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("ReportingToBeReportedWorklistDescription")]
	public class ReportingToBeReportedWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
			criteria.ProcedureStep.Scheduling.StartTime.IsNotNull();
			return new [] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(InterpretationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepScheduledStartTime,
				null,
				WorklistOrdering.PrioritizeOldestItems,
				true);
		}
	}

	/// <summary>
	/// ReportingAssignedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingAssignedWorklistDescription")]
	public class ReportingAssignedWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.ExecutingStaff);
			criteria.ProcedureStep.Scheduling.StartTime.IsNotNull();
			return new[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(InterpretationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepScheduledStartTime,
				null,
				WorklistOrdering.PrioritizeOldestItems,
				true);
		}
	}


	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingDraftWorklistDescription")]
	public class ReportingDraftWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.IP });
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.ExecutingStaff);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(InterpretationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepStartTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingInTranscriptionWorklistDescription")]
	public class ReportingInTranscriptionWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.ReportPart.Interpreter.EqualTo(wqc.ExecutingStaff);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(TranscriptionStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingReviewTranscriptionWorklistDescription")]
	public class ReportingReviewTranscriptionWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.ExecutingStaff);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(TranscriptionReviewStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems,
				true);
		}

		protected override WorklistItemProjection GetProjectionCore(WorklistItemField timeField)
		{
			// add the "HasErrors" flag for this worklist
			return base.GetProjectionCore(timeField).AddFields(new[] { WorklistItemField.ReportPartHasErrors });
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingVerifiedWorklistDescription")]
	public class ReportingVerifiedWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var unsupervised = new ReportingWorklistItemSearchCriteria();
			unsupervised.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.CM });
			unsupervised.ReportPart.Verifier.EqualTo(wqc.ExecutingStaff);

			var supervised = new ReportingWorklistItemSearchCriteria();
			supervised.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.CM });
			supervised.ReportPart.Verifier.NotEqualTo(wqc.ExecutingStaff);
			supervised.ReportPart.Interpreter.EqualTo(wqc.ExecutingStaff);

			return new WorklistItemSearchCriteria[] { unsupervised, supervised };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(PublicationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeNewestItems);
		}
	}
}
