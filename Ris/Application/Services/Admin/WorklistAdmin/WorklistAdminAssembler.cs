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
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using Iesi.Collections.Generic;

namespace ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin
{
	internal class WorklistAdminAssembler
	{
		/// <summary>
		/// Create worklist class summary.
		/// </summary>
		/// <param name="worklistClass"></param>
		/// <returns></returns>
		public WorklistClassSummary CreateClassSummary(Type worklistClass)
		{
			var ptgClass = Worklist.GetProcedureTypeGroupClass(worklistClass);

			return new WorklistClassSummary(
				Worklist.GetClassName(worklistClass),
				Worklist.GetDisplayName(worklistClass),
				Worklist.GetCategory(worklistClass),
				Worklist.GetDescription(worklistClass),
				ptgClass == null ? null : ptgClass.Name,
				ptgClass == null ? null : TerminologyTranslator.Translate(ptgClass),
				Worklist.GetSupportsReportingStaffRoleFilter(worklistClass));
		}

		/// <summary>
		/// Create worklist summary.
		/// </summary>
		/// <param name="worklist"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public WorklistAdminSummary CreateWorklistSummary(Worklist worklist, IPersistenceContext context)
		{
			var isStatic = Worklist.GetIsStatic(worklist.GetClass());

			var staffAssembler = new StaffAssembler();
			var groupAssembler = new StaffGroupAssembler();
			return new WorklistAdminSummary(
				isStatic ? null : worklist.GetRef(),
				isStatic ? Worklist.GetDisplayName(worklist.GetClass()) : worklist.Name,
				isStatic ? Worklist.GetDescription(worklist.GetClass()) : worklist.Description,
				CreateClassSummary(worklist.GetClass()),
				worklist.Owner.IsStaffOwner ? staffAssembler.CreateStaffSummary(worklist.Owner.Staff, context) : null,
				worklist.Owner.IsGroupOwner ? groupAssembler.CreateSummary(worklist.Owner.Group) : null);
		}

		/// <summary>
		/// Create worklist detail.
		/// </summary>
		/// <param name="worklist"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public WorklistAdminDetail CreateWorklistDetail(Worklist worklist, IPersistenceContext context)
		{
			var detail = new WorklistAdminDetail(worklist.GetRef(), worklist.Name, worklist.Description,
				CreateClassSummary(worklist.GetClass()));

			var staffAssembler = new StaffAssembler();
			var staffGroupAssembler = new StaffGroupAssembler();
			detail.OwnerStaff = worklist.Owner.IsStaffOwner ?
				staffAssembler.CreateStaffSummary(worklist.Owner.Staff, context) : null;
			detail.OwnerGroup = worklist.Owner.IsGroupOwner ?
				staffGroupAssembler.CreateSummary(worklist.Owner.Group) : null;

			// proc types
			var ptAssembler = new ProcedureTypeAssembler();
			detail.ProcedureTypes = GetFilterSummary(worklist.ProcedureTypeFilter,
				item => ptAssembler.CreateSummary(item));

			// proc type groups
			var ptgAssembler = new ProcedureTypeGroupAssembler();
			detail.ProcedureTypeGroups = GetFilterSummary(worklist.ProcedureTypeGroupFilter,
				item => ptgAssembler.GetProcedureTypeGroupSummary(item, context));

			// facilities
			var facilityAssembler = new FacilityAssembler();
			detail.Facilities = GetFilterSummary(worklist.FacilityFilter,
				item => facilityAssembler.CreateFacilitySummary(item));
			detail.FilterByWorkingFacility = worklist.FacilityFilter.IsEnabled && worklist.FacilityFilter.IncludeWorkingFacility;

			// departments
			var departmentAssembler = new DepartmentAssembler();
			detail.Departments = GetFilterSummary(worklist.DepartmentFilter,
				item => departmentAssembler.CreateSummary(item, context));

			// patient class
			detail.PatientClasses = GetFilterSummary(worklist.PatientClassFilter,
				item => EnumUtils.GetEnumValueInfo(item));

			// location
			var locationAssembler = new LocationAssembler();
			detail.PatientLocations = GetFilterSummary(worklist.PatientLocationFilter,
				item => locationAssembler.CreateLocationSummary(item));

			// order priority
			detail.OrderPriorities = GetFilterSummary(worklist.OrderPriorityFilter,
				item => EnumUtils.GetEnumValueInfo(item));

			// ordering prac
			var practitionerAssembler = new ExternalPractitionerAssembler();
			detail.OrderingPractitioners = GetFilterSummary(worklist.OrderingPractitionerFilter,
				item => practitionerAssembler.CreateExternalPractitionerSummary(item, context));

			// portable
			if (worklist.PortableFilter.IsEnabled)
			{
				detail.Portabilities = new List<bool> { worklist.PortableFilter.Value };
			}

			// time window
			if (worklist.TimeFilter.IsEnabled && worklist.TimeFilter.Value != null)
			{
				if (worklist.TimeFilter.Value.Start != null)
					detail.StartTime = CreateTimePointContract(worklist.TimeFilter.Value.Start);
				if (worklist.TimeFilter.Value.End != null)
					detail.EndTime = CreateTimePointContract(worklist.TimeFilter.Value.End);
			}

			detail.StaffSubscribers = CollectionUtils.Map(worklist.StaffSubscribers,
				(Staff staff) => staffAssembler.CreateStaffSummary(staff, context));

			detail.GroupSubscribers = CollectionUtils.Map(worklist.GroupSubscribers,
				(StaffGroup group) => staffGroupAssembler.CreateSummary(group));

			// Some ReportingWorklists can support staff role filters, if that is true for this worklist,
			// add those filters to the WorklistAdminDetail
			if (Worklist.GetSupportsReportingStaffRoleFilter(worklist.GetClass()))
			{
				var reportingWorklist = worklist.As<ReportingWorklist>();
				detail.InterpretedByStaff = GetFilterSummary(reportingWorklist.InterpretedByStaffFilter, context);
				detail.TranscribedByStaff = GetFilterSummary(reportingWorklist.TranscribedByStaffFilter, context);
				detail.VerifiedByStaff = GetFilterSummary(reportingWorklist.VerifiedByStaffFilter, context);
				detail.SupervisedByStaff = GetFilterSummary(reportingWorklist.SupervisedByStaffFilter, context);
			}

			return detail;
		}

		/// <summary>
		/// Update specified worklist from detail.
		/// </summary>
		/// <param name="worklist"></param>
		/// <param name="detail"></param>
		/// <param name="updateSubscribers"></param>
		/// <param name="context"></param>
		public void UpdateWorklist(Worklist worklist, WorklistAdminDetail detail,
			bool updateSubscribers, IPersistenceContext context)
		{
			worklist.Name = detail.Name;
			worklist.Description = detail.Description;

			// do not update the worklist.Owner here!!! - once set, it should never be updated

			// procedure types
			UpdateFilter(worklist.ProcedureTypeFilter, detail.ProcedureTypes,
				summary => context.Load<ProcedureType>(summary.ProcedureTypeRef, EntityLoadFlags.Proxy));

			// procedure groups
			UpdateFilter(worklist.ProcedureTypeGroupFilter, detail.ProcedureTypeGroups,
				summary => context.Load<ProcedureTypeGroup>(summary.ProcedureTypeGroupRef, EntityLoadFlags.Proxy));

			// facilities
			UpdateFilter(worklist.FacilityFilter, detail.Facilities,
				summary => context.Load<Facility>(summary.FacilityRef, EntityLoadFlags.Proxy));
			worklist.FacilityFilter.IncludeWorkingFacility = detail.FilterByWorkingFacility;
			worklist.FacilityFilter.IsEnabled = worklist.FacilityFilter.Values.Count > 0 || worklist.FacilityFilter.IncludeWorkingFacility;

			// departments
			UpdateFilter(worklist.DepartmentFilter, detail.Departments,
				summary => context.Load<Department>(summary.DepartmentRef, EntityLoadFlags.Proxy));

			// patient classes
			UpdateFilter(worklist.PatientClassFilter, detail.PatientClasses,
				summary => EnumUtils.GetEnumValue<PatientClassEnum>(summary, context));

			// patient locations
			UpdateFilter(worklist.PatientLocationFilter, detail.PatientLocations,
				summary => context.Load<Location>(summary.LocationRef, EntityLoadFlags.Proxy));

			// order priorities
			UpdateFilter(worklist.OrderPriorityFilter, detail.OrderPriorities,
				summary => EnumUtils.GetEnumValue<OrderPriorityEnum>(summary, context));

			// ordering practitioners
			UpdateFilter(worklist.OrderingPractitionerFilter, detail.OrderingPractitioners,
				summary => context.Load<ExternalPractitioner>(summary.PractitionerRef, EntityLoadFlags.Proxy));

			// portable
			if (detail.Portabilities != null)
			{
				// put them into a set to guarantee uniqueness, in case the client sent a non-unique list
				var set = new HashedSet<bool>(detail.Portabilities);

				// it only makes sense to enable this filter if the set contains exactly one value (true or false, but not both)
				worklist.PortableFilter.IsEnabled = set.Count == 1;
				worklist.PortableFilter.Value = CollectionUtils.FirstElement(set, false);
			}

			var start = CreateTimePoint(detail.StartTime);
			var end = CreateTimePoint(detail.EndTime);
			if (start != null || end != null)
			{
				worklist.TimeFilter.Value = new WorklistTimeRange(start, end);
				worklist.TimeFilter.IsEnabled = true;
			}
			else
				worklist.TimeFilter.IsEnabled = false;

			// process subscriptions
			if (updateSubscribers)
			{
				worklist.StaffSubscribers.Clear();
				worklist.StaffSubscribers.AddAll(
					CollectionUtils.Map(detail.StaffSubscribers,
						(StaffSummary summary) => context.Load<Staff>(summary.StaffRef, EntityLoadFlags.Proxy)));

				worklist.GroupSubscribers.Clear();
				worklist.GroupSubscribers.AddAll(
					CollectionUtils.Map(detail.GroupSubscribers,
						(StaffGroupSummary summary) => context.Load<StaffGroup>(summary.StaffGroupRef, EntityLoadFlags.Proxy)));
			}

			// If the worklist supports staff role filters, process the filters provided.
			if (Worklist.GetSupportsReportingStaffRoleFilter(worklist.GetClass()))
			{
				var reportingWorklist = worklist.As<ReportingWorklist>();
				UpdateFilter(reportingWorklist.InterpretedByStaffFilter, detail.InterpretedByStaff, context);
				UpdateFilter(reportingWorklist.TranscribedByStaffFilter, detail.TranscribedByStaff, context);
				UpdateFilter(reportingWorklist.VerifiedByStaffFilter, detail.VerifiedByStaff, context);
				UpdateFilter(reportingWorklist.SupervisedByStaffFilter, detail.SupervisedByStaff, context);
			}
		}

		#region Helpers

		private static WorklistAdminDetail.TimePoint CreateTimePointContract(WorklistTimePoint tp)
		{
			return tp.IsFixed ? new WorklistAdminDetail.TimePoint(tp.FixedValue, tp.Resolution) :
				new WorklistAdminDetail.TimePoint(tp.RelativeValue, tp.Resolution);
		}

		private static WorklistTimePoint CreateTimePoint(WorklistAdminDetail.TimePoint contract)
		{
			if (contract != null && (contract.FixedTime.HasValue || contract.RelativeTime.HasValue))
			{
				return contract.FixedTime.HasValue ?
					new WorklistTimePoint(contract.FixedTime.Value, contract.Resolution) :
					new WorklistTimePoint(contract.RelativeTime.Value, contract.Resolution);
			}
			return null;
		}

		private static List<TSummary> GetFilterSummary<TItem, TSummary>(WorklistMultiValuedFilter<TItem> filter, Converter<TItem, TSummary> converter)
		{
			return !filter.IsEnabled ? new List<TSummary>() : CollectionUtils.Map(filter.Values, converter);
		}

		private static WorklistAdminDetail.StaffList GetFilterSummary(WorklistStaffFilter filter, IPersistenceContext context)
		{
			if (!filter.IsEnabled)
				return new WorklistAdminDetail.StaffList();

			var assembler = new StaffAssembler();
			return new WorklistAdminDetail.StaffList
								{
									Staff = CollectionUtils.Map(filter.Values,
										(Staff staff) => assembler.CreateStaffSummary(staff, context)),
									IncludeCurrentUser = filter.IncludeCurrentStaff
								};
		}

		private static void UpdateFilter<TItem, TSummary>(WorklistMultiValuedFilter<TItem> filter, List<TSummary> summaries, Converter<TSummary, TItem> converter)
		{
			filter.Values.Clear();
			if (summaries != null)
			{
				filter.Values.AddAll(CollectionUtils.Map(summaries, converter));
			}
			filter.IsEnabled = filter.Values.Count > 0;
		}

		private static void UpdateFilter(WorklistStaffFilter staffFilter, WorklistAdminDetail.StaffList stafflist, IPersistenceContext context)
		{
			staffFilter.Values.Clear();
			if (stafflist != null)
			{
				staffFilter.Values.AddAll(CollectionUtils.Map(
					stafflist.Staff,
					(StaffSummary s) => context.Load<Staff>(s.StaffRef, EntityLoadFlags.Proxy)));

				staffFilter.IncludeCurrentStaff = stafflist.IncludeCurrentUser;
			}
			staffFilter.IsEnabled = staffFilter.Values.Count > 0 || staffFilter.IncludeCurrentStaff;
		}

		#endregion

	}
}