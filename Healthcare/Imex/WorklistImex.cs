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
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("Worklist")]
	public class WorklistImex : XmlEntityImex<Worklist, WorklistImex.WorklistData>
	{
		#region Data Contracts

		[DataContract]
		public class WorklistData
		{
			[DataContract]
			public class StaffSubscriberData
			{
				[DataMember]
				public string StaffId;
			}

			[DataContract]
			public class GroupSubscriberData
			{
				[DataMember]
				public string StaffGroupName;
			}

			[DataContract]
			public abstract class FilterData
			{
				[DataMember]
				public bool Enabled;
			}

			[DataContract]
			public class SingleValuedFilterData<TValue> : FilterData
			{
				[DataMember]
				public TValue Value;
			}

			[DataContract]
			public class MultiValuedFilterData<TValue> : FilterData
			{
				public MultiValuedFilterData()
				{
					this.Values = new List<TValue>();
				}

				[DataMember]
				public List<TValue> Values;
			}

			[DataContract]
			public class ProcedureTypeData
			{
				[DataMember]
				public string Id;
			}

			[DataContract]
			public class ProcedureTypeGroupData
			{
				[DataMember]
				public string Name;

				[DataMember]
				public string Class;
			}

			[DataContract]
			public class LocationData
			{
				public LocationData()
				{
				}

				public LocationData(string id)
				{
					this.Id = id;
				}

				[DataMember]
				public string Id;
			}

			[DataContract]
			public class DepartmentData
			{
				public DepartmentData()
				{
				}

				public DepartmentData(string id)
				{
					this.Id = id;
				}

				[DataMember]
				public string Id;
			}

			[DataContract]
			public class PractitionerData
			{
				public PractitionerData()
				{

				}

				public PractitionerData(string familyName, string givenName, string licenseNumber, string billingNumber)
				{
					FamilyName = familyName;
					GivenName = givenName;
					LicenseNumber = licenseNumber;
					BillingNumber = billingNumber;
				}

				[DataMember]
				public string FamilyName;

				[DataMember]
				public string GivenName;

				[DataMember]
				public string LicenseNumber;

				[DataMember]
				public string BillingNumber;
			}

			[DataContract]
			public class EnumValueData
			{
				public EnumValueData()
				{

				}

				public EnumValueData(string code)
				{
					this.Code = code;
				}

				[DataMember]
				public string Code;
			}

			[DataContract]
			public class FacilitiesFilterData : MultiValuedFilterData<EnumValueData>
			{
				public FacilitiesFilterData()
				{

				}

				public FacilitiesFilterData(MultiValuedFilterData<EnumValueData> x)
				{
					this.Enabled = x.Enabled;
					this.Values = x.Values;
				}

				[DataMember]
				public bool IncludeWorkingFacility;
			}

			[DataContract]
			public class StaffFilterData : MultiValuedFilterData<StaffSubscriberData>
			{
				public StaffFilterData()
				{
				}

				public StaffFilterData(MultiValuedFilterData<StaffSubscriberData> s)
				{
					this.Enabled = s.Enabled;
					this.Values = s.Values;
				}

				[DataMember]
				public bool IncludeCurrentStaff;
			}

			[DataContract]
			public class TimeRangeData
			{
				public TimeRangeData()
				{
				}

				public TimeRangeData(WorklistTimeRange tr)
				{
					this.Start = tr.Start == null ? null : new TimePointData(tr.Start);
					this.End = tr.End == null ? null : new TimePointData(tr.End);
				}

				public WorklistTimeRange CreateTimeRange()
				{
					return new WorklistTimeRange(
						this.Start == null ? null : this.Start.CreateTimePoint(),
						this.End == null ? null : this.End.CreateTimePoint());
				}

				[DataMember]
				public TimePointData Start;

				[DataMember]
				public TimePointData End;
			}

			[DataContract]
			public class TimePointData
			{
				public TimePointData()
				{
				}

				public TimePointData(WorklistTimePoint tp)
				{
					this.FixedValue = tp.IsFixed ? tp.FixedValue : null;
					this.RelativeValue = tp.IsRelative ? tp.RelativeValue.Value.Ticks.ToString() : null;
					this.Resolution = tp.Resolution;
				}

				public WorklistTimePoint CreateTimePoint()
				{
					if (this.FixedValue != null)
						return new WorklistTimePoint(this.FixedValue.Value, this.Resolution);
					if (this.RelativeValue != null)
						return new WorklistTimePoint(TimeSpan.FromTicks(Int64.Parse(this.RelativeValue)), this.Resolution);
					return null;
				}

				[DataMember]
				public DateTime? FixedValue;

				[DataMember]
				public string RelativeValue;

				[DataMember]
				public long Resolution;
			}

			[DataContract]
			public class FiltersData
			{
				public FiltersData()
				{
					this.ProcedureTypes = new MultiValuedFilterData<ProcedureTypeData>();
					this.ProcedureTypeGroups = new MultiValuedFilterData<ProcedureTypeGroupData>();
					this.Facilities = new FacilitiesFilterData();
					this.Departments = new MultiValuedFilterData<DepartmentData>();
					this.OrderPriorities = new MultiValuedFilterData<EnumValueData>();
					this.OrderingPractitioners = new MultiValuedFilterData<PractitionerData>();
					this.PatientClasses = new MultiValuedFilterData<EnumValueData>();
					this.PatientLocations = new MultiValuedFilterData<LocationData>();
					this.Portable = new SingleValuedFilterData<bool>();
					this.TimeWindow = new SingleValuedFilterData<TimeRangeData>();
					this.InterpretedByStaff = new StaffFilterData();
					this.TranscribedByStaff = new StaffFilterData();
					this.VerifiedByStaff = new StaffFilterData();
					this.SupervisedByStaff = new StaffFilterData();
				}

				[DataMember]
				public MultiValuedFilterData<ProcedureTypeData> ProcedureTypes;

				[DataMember]
				public MultiValuedFilterData<ProcedureTypeGroupData> ProcedureTypeGroups;

				[DataMember]
				public FacilitiesFilterData Facilities;

				[DataMember]
				public MultiValuedFilterData<DepartmentData> Departments;

				[DataMember]
				public MultiValuedFilterData<EnumValueData> OrderPriorities;

				[DataMember]
				public MultiValuedFilterData<PractitionerData> OrderingPractitioners;

				[DataMember]
				public MultiValuedFilterData<EnumValueData> PatientClasses;

				[DataMember]
				public MultiValuedFilterData<LocationData> PatientLocations;

				[DataMember]
				public SingleValuedFilterData<bool> Portable;

				[DataMember]
				public SingleValuedFilterData<TimeRangeData> TimeWindow;

				[DataMember]
				public StaffFilterData InterpretedByStaff;

				[DataMember]
				public StaffFilterData TranscribedByStaff;

				[DataMember]
				public StaffFilterData VerifiedByStaff;

				[DataMember]
				public StaffFilterData SupervisedByStaff;
			}

			public WorklistData()
			{
				this.Filters = new FiltersData();
			}

			[DataMember]
			public string Name;

			[DataMember]
			public string Class;

			[DataMember]
			public string Description;

			[DataMember]
			public FiltersData Filters;

			[DataMember]
			public List<StaffSubscriberData> StaffSubscribers;

			[DataMember]
			public List<GroupSubscriberData> GroupSubscribers;
		}


		#endregion

		#region Overrides

		protected override IList<Worklist> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			var where = new WorklistSearchCriteria();
			where.Name.SortAsc(0);
			where.FullClassName.SortAsc(1);
			return context.GetBroker<IWorklistBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override WorklistData Export(Worklist worklist, IReadContext context)
		{
			var data = new WorklistData();

			data.Class = worklist.GetClass().FullName;
			data.Name = worklist.Name;
			data.Description = worklist.Description;

			data.StaffSubscribers = CollectionUtils.Map(
				worklist.StaffSubscribers,
				(Staff staff) => new WorklistData.StaffSubscriberData {StaffId = staff.Id});

			data.GroupSubscribers = CollectionUtils.Map(
				worklist.GroupSubscribers,
				(StaffGroup group) => new WorklistData.GroupSubscriberData {StaffGroupName = group.Name});

			// proc type filter
			ExportFilter(
				worklist.ProcedureTypeFilter,
				data.Filters.ProcedureTypes,
				item => new WorklistData.ProcedureTypeData { Id = item.Id });

			// proc type group filter
			ExportFilter(
				worklist.ProcedureTypeGroupFilter,
				data.Filters.ProcedureTypeGroups,
				group => new WorklistData.ProcedureTypeGroupData {Class = group.GetClass().FullName, Name = group.Name});

			// facility filter
			data.Filters.Facilities.IncludeWorkingFacility = worklist.FacilityFilter.IncludeWorkingFacility;
			ExportFilter(
				worklist.FacilityFilter,
				data.Filters.Facilities,
				item => new WorklistData.EnumValueData(item.Code));

			// department filter
			ExportFilter(
				worklist.DepartmentFilter,
				data.Filters.Departments,
				item => new WorklistData.DepartmentData(item.Id));

			// priority filter
			ExportFilter(worklist.OrderPriorityFilter, data.Filters.OrderPriorities,
				item => new WorklistData.EnumValueData(item.Code));

			// ordering prac filter
			ExportFilter(worklist.OrderingPractitionerFilter, data.Filters.OrderingPractitioners,
				item => new WorklistData.PractitionerData(item.Name.FamilyName, item.Name.GivenName, item.LicenseNumber, item.BillingNumber));

			// patient class filter
			ExportFilter(worklist.PatientClassFilter, data.Filters.PatientClasses,
				item => new WorklistData.EnumValueData(item.Code));

			// patient location filter
			ExportFilter(worklist.PatientLocationFilter, data.Filters.PatientLocations,
				item => new WorklistData.LocationData(item.Id));

			// portable filter
			data.Filters.Portable.Enabled = worklist.PortableFilter.IsEnabled;
			data.Filters.Portable.Value = worklist.PortableFilter.Value;

			//Bug #2429: don't forget to include the time filter
			// time filter
			data.Filters.TimeWindow.Enabled = worklist.TimeFilter.IsEnabled;
			data.Filters.TimeWindow.Value = worklist.TimeFilter.Value == null ? null :
				new WorklistData.TimeRangeData(worklist.TimeFilter.Value);

			// reporting worklist filters
			if (Worklist.GetSupportsReportingStaffRoleFilter(worklist.GetClass()))
				ExportReportingWorklistFilters(data, worklist.As<ReportingWorklist>());

			return data;
		}

		private void ExportReportingWorklistFilters(WorklistData data, ReportingWorklist worklist)
		{
			ExportStaffFilter(worklist.InterpretedByStaffFilter, data.Filters.InterpretedByStaff);
			ExportStaffFilter(worklist.TranscribedByStaffFilter, data.Filters.TranscribedByStaff);
			ExportStaffFilter(worklist.VerifiedByStaffFilter, data.Filters.VerifiedByStaff);
			ExportStaffFilter(worklist.SupervisedByStaffFilter, data.Filters.SupervisedByStaff);
		}

		public void ExportStaffFilter(WorklistStaffFilter filter, WorklistData.StaffFilterData data)
		{
			ExportFilter(filter, data,
				staff => new WorklistData.StaffSubscriberData {StaffId = staff.Id});
			data.IncludeCurrentStaff = filter.IncludeCurrentStaff;
		}

		protected override void Import(WorklistData data, IUpdateContext context)
		{
			var worklist = LoadOrCreateWorklist(data.Name, data.Class, context);
			worklist.Description = data.Description;

			if (data.StaffSubscribers != null)
			{
				foreach (var s in data.StaffSubscribers)
				{
					var criteria = new StaffSearchCriteria();
					criteria.Id.EqualTo(s.StaffId);

					var staff = context.GetBroker<IStaffBroker>().Find(criteria);
					if (staff.Count == 1)
						worklist.StaffSubscribers.Add(CollectionUtils.FirstElement(staff));
				}
			}

			if (data.GroupSubscribers != null)
			{
				foreach (var s in data.GroupSubscribers)
				{
					var criteria = new StaffGroupSearchCriteria();
					criteria.Name.EqualTo(s.StaffGroupName);

					var groups = context.GetBroker<IStaffGroupBroker>().Find(criteria);
					if (groups.Count == 1)
						worklist.GroupSubscribers.Add(CollectionUtils.FirstElement(groups));
				}
			}

			// proc type filter
			ImportFilter(
				worklist.ProcedureTypeFilter,
				data.Filters.ProcedureTypes,
				delegate(WorklistData.ProcedureTypeData s)
				{
					var criteria = new ProcedureTypeSearchCriteria();
					criteria.Id.EqualTo(s.Id);

					var broker = context.GetBroker<IProcedureTypeBroker>();
					return CollectionUtils.FirstElement(broker.Find(criteria));
				});

			// proc type group filter
			ImportFilter(
				worklist.ProcedureTypeGroupFilter,
				data.Filters.ProcedureTypeGroups,
				delegate(WorklistData.ProcedureTypeGroupData s)
				{
					var criteria = new ProcedureTypeGroupSearchCriteria();
					criteria.Name.EqualTo(s.Name);

					var broker = context.GetBroker<IProcedureTypeGroupBroker>();
					return CollectionUtils.FirstElement(broker.Find(criteria, ProcedureTypeGroup.GetSubClass(s.Class, context)));
				});

			//Bug #2284: don't forget to set the IncludeWorkingFacility property
			// facility filter
			worklist.FacilityFilter.IncludeWorkingFacility = data.Filters.Facilities.IncludeWorkingFacility;
			ImportFilter(
				worklist.FacilityFilter,
				data.Filters.Facilities,
				delegate(WorklistData.EnumValueData s)
				{
					var criteria = new FacilitySearchCriteria();
					criteria.Code.EqualTo(s.Code);

					var broker = context.GetBroker<IFacilityBroker>();
					return CollectionUtils.FirstElement(broker.Find(criteria));
				});

			// department filter
			ImportFilter(
				worklist.DepartmentFilter,
				data.Filters.Departments,
				delegate(WorklistData.DepartmentData s)
				{
					var criteria = new DepartmentSearchCriteria();
					criteria.Id.EqualTo(s.Id);

					var broker = context.GetBroker<IDepartmentBroker>();
					return CollectionUtils.FirstElement(broker.Find(criteria));
				});

			// priority filter
			ImportFilter(
				worklist.OrderPriorityFilter,
				data.Filters.OrderPriorities,
				delegate(WorklistData.EnumValueData s)
				{
					var broker = context.GetBroker<IEnumBroker>();
					return broker.Find<OrderPriorityEnum>(s.Code);
				});

			// ordering prac filter
			ImportFilter(
				worklist.OrderingPractitionerFilter,
				data.Filters.OrderingPractitioners,
				delegate(WorklistData.PractitionerData s)
				{
					var criteria = new ExternalPractitionerSearchCriteria();
					criteria.Name.FamilyName.EqualTo(s.FamilyName);
					criteria.Name.GivenName.EqualTo(s.GivenName);

					// these criteria may not be provided (the data may not existed when exported),
					// but if available, they help to ensure the correct practitioner is being mapped
					if (!string.IsNullOrEmpty(s.BillingNumber))
						criteria.BillingNumber.EqualTo(s.BillingNumber);
					if (!string.IsNullOrEmpty(s.LicenseNumber))
						criteria.LicenseNumber.EqualTo(s.LicenseNumber);

					var broker = context.GetBroker<IExternalPractitionerBroker>();
					return CollectionUtils.FirstElement(broker.Find(criteria));
				});

			// patient class filter
			ImportFilter(
				worklist.PatientClassFilter,
				data.Filters.PatientClasses,
				delegate(WorklistData.EnumValueData s)
				{
					var broker = context.GetBroker<IEnumBroker>();
					return broker.Find<PatientClassEnum>(s.Code);
				});

			// patient location filter
			ImportFilter(
				worklist.PatientLocationFilter,
				data.Filters.PatientLocations,
				delegate(WorklistData.LocationData s)
				{
					var criteria = new LocationSearchCriteria();
					criteria.Id.EqualTo(s.Id);

					var broker = context.GetBroker<ILocationBroker>();
					return CollectionUtils.FirstElement(broker.Find(criteria));
				});

			// portable filter
			worklist.PortableFilter.IsEnabled = data.Filters.Portable.Enabled;
			worklist.PortableFilter.Value = data.Filters.Portable.Value;

			//Bug #2429: don't forget to include the time filter
			// time filter
			worklist.TimeFilter.IsEnabled = data.Filters.TimeWindow.Enabled;
			worklist.TimeFilter.Value = data.Filters.TimeWindow == null || data.Filters.TimeWindow.Value == null
											? null
											: data.Filters.TimeWindow.Value.CreateTimeRange();

			// reporting filters
			if (Worklist.GetSupportsReportingStaffRoleFilter(worklist.GetClass()))
				ImportReportingWorklistFilters(data, worklist.As<ReportingWorklist>(), context);
		}

		private static void ImportReportingWorklistFilters(WorklistData data, ReportingWorklist worklist, IUpdateContext context)
		{
			ImportStaffFilter(worklist.InterpretedByStaffFilter, data.Filters.InterpretedByStaff, context);
			ImportStaffFilter(worklist.TranscribedByStaffFilter, data.Filters.TranscribedByStaff, context);
			ImportStaffFilter(worklist.VerifiedByStaffFilter, data.Filters.VerifiedByStaff, context);
			ImportStaffFilter(worklist.SupervisedByStaffFilter, data.Filters.SupervisedByStaff, context);
		}

		private static void ImportStaffFilter(WorklistStaffFilter filter, WorklistData.StaffFilterData staff, IUpdateContext context)
		{
			ImportFilter(filter, staff,
				delegate(WorklistData.StaffSubscriberData s)
				{
					var criteria = new StaffSearchCriteria();
					criteria.Id.EqualTo(s.StaffId);

					var broker = context.GetBroker<IStaffBroker>();
					return CollectionUtils.FirstElement(broker.Find(criteria));
				});
			filter.IncludeCurrentStaff = staff.IncludeCurrentStaff;
		}

		#endregion

		#region Helpers

		private static void ExportFilter<TDomain, TData>(WorklistMultiValuedFilter<TDomain> filter, WorklistData.MultiValuedFilterData<TData> data,
			Converter<TDomain, TData> converter)
		{
			data.Enabled = filter.IsEnabled;
			data.Values = CollectionUtils.Map(filter.Values, converter);
		}

		private static void ImportFilter<TDomain, TData>(WorklistMultiValuedFilter<TDomain> filter, WorklistData.MultiValuedFilterData<TData> data,
			Converter<TData, TDomain> converter)
		{
			if (data == null)
				return;

			filter.IsEnabled = data.Enabled;
			foreach (var i in data.Values)
			{
				var value = converter(i);
				if (value != null)
					filter.Values.Add(value);
			}
		}

		private static Worklist LoadOrCreateWorklist(string name, string worklistClassName, IPersistenceContext context)
		{
			Worklist worklist;

			try
			{
				worklist = context.GetBroker<IWorklistBroker>().FindOne(name, worklistClassName);
			}
			catch (EntityNotFoundException)
			{
				worklist = WorklistFactory.Instance.CreateWorklist(worklistClassName);
				worklist.Name = name;

				context.Lock(worklist, DirtyState.New);
			}

			return worklist;
		}

		#endregion

	}
}
