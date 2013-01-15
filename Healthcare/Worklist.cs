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
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Worklist entity.  Represents a worklist.
	/// </summary>
	[UniqueKey("WorklistUniqueKey", new [] { "Name", "FullClassName", "Owner.Staff", "Owner.Group" })]
	[Validation(HighLevelRulesProviderMethod="GetValidationRules")]
	public abstract class Worklist : Entity, IWorklist
	{
		/// <summary>
		/// Defines values for how a worklist is ordered in terms of which items are given priority
		/// when the full worklist cannot be displayed.
		/// </summary>
		public enum WorklistOrdering
		{
			/// <summary>
			/// Ensure the oldest items are given priority.
			/// </summary>
			PrioritizeOldestItems,

			/// <summary>
			/// Ensure the newest items are given priority.
			/// </summary>
			PrioritizeNewestItems
		}

		/// <summary>
		/// Describes how a worklist will behave wrt time.
		/// </summary>
		public class TimeDirective
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="timeField"></param>
			/// <param name="defaultRange"></param>
			/// <param name="ordering"></param>
			public TimeDirective(WorklistItemField timeField, WorklistTimeRange defaultRange, WorklistOrdering ordering)
				:this(timeField, defaultRange, ordering, false)
			{
			}

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="timeField"></param>
			/// <param name="defaultRange"></param>
			/// <param name="ordering"></param>
			/// <param name="honourOrderPriority"> </param>
			public TimeDirective(WorklistItemField timeField, WorklistTimeRange defaultRange, WorklistOrdering ordering, bool honourOrderPriority)
			{
				this.TimeField = timeField;
				this.Ordering = ordering;
				this.DefaultRange = defaultRange;
				this.HonourOrderPriority = honourOrderPriority;
			}

			/// <summary>
			/// Gets the time field used by the worklist for filtering, ordering and population of the "time" column in the worklist item.
			/// </summary>
			public WorklistItemField TimeField { get; private set; }

			/// <summary>
			/// Gets the default time window to be used when the time filter is not enabled.
			/// </summary>
			public WorklistTimeRange DefaultRange { get; private set; }

			/// <summary>
			/// Gets a value indicating whether oldest or newest items are prioritized in result sets.
			/// </summary>
			public WorklistOrdering Ordering { get; private set; }

			/// <summary>
			/// Gets a value indicating whether the priority of the Order is honoured. If true, items corresponding
			/// to orders with higher priority will take precedence.
			/// </summary>
			public bool HonourOrderPriority { get; private set; }
		}




		#region Static members

		/// <summary>
		/// Gets the logical class name for the specified worklist class, which may or may not be identical
		/// to its .NET class name.
		/// </summary>
		/// <param name="worklistClass"></param>
		/// <returns></returns>
		public static string GetClassName(Type worklistClass)
		{
			// could use a logical class name here to decouple from the actual .NET class name,
			// but is it worth the added complexity?
			return worklistClass.Name;
		}

		/// <summary>
		/// Gets the procedure-type group class for the specified worklist class, as specified by
		/// the <see cref="WorklistProcedureTypeGroupClassAttribute"/>.
		/// </summary>
		/// <param name="worklistClass"></param>
		/// <returns></returns>
		public static Type GetProcedureTypeGroupClass(Type worklistClass)
		{
			var a = AttributeUtils.GetAttribute<WorklistProcedureTypeGroupClassAttribute>(worklistClass, true);
			return a == null ? null : a.ProcedureTypeGroupClass;
		}

		/// <summary>
		/// Gets the category for the specified worklist class, as specified by
		/// the <see cref="WorklistCategoryAttribute"/>, or null if not specified.
		/// </summary>
		/// <param name="worklistClass"></param>
		/// <returns></returns>
		public static string GetCategory(Type worklistClass)
		{
			var a = AttributeUtils.GetAttribute<WorklistCategoryAttribute>(worklistClass, true);
			if (a == null)
				return null;

			var resolver = new ResourceResolver(worklistClass, true);
			return resolver.LocalizeString(a.Category);
		}

		/// <summary>
		/// Gets the display name for the specified worklist class, obtained either from
		/// the <see cref="WorklistClassDisplayNameAttribute"/>, otherwise via the
		/// <see cref="TerminologyTranslator"/> class.
		/// </summary>
		/// <param name="worklistClass"></param>
		/// <returns></returns>
		public static string GetDisplayName(Type worklistClass)
		{
			var a = AttributeUtils.GetAttribute<WorklistClassDisplayNameAttribute>(worklistClass, true);

			if (a == null)
				return TerminologyTranslator.Translate(worklistClass);

			var resolver = new ResourceResolver(worklistClass, true);
			return resolver.LocalizeString(a.DisplayName);
		}

		/// <summary>
		/// Gets the description for the specified worklist class, as specified by
		/// the <see cref="WorklistCategoryAttribute"/>, or null if not specified.
		/// </summary>
		/// <param name="worklistClass"></param>
		/// <returns></returns>
		public static string GetDescription(Type worklistClass)
		{
			var a = AttributeUtils.GetAttribute<WorklistClassDescriptionAttribute>(worklistClass, true);
			if (a == null)
				return null;

			var resolver = new ResourceResolver(worklistClass, true);
			return resolver.LocalizeString(a.Description);
		}

		/// <summary>
		/// Gets a value indicating whether the specified worklist class supports time-filters, as specified by
		/// the <see cref="WorklistSupportsReportingStaffRoleFilterAttribute"/>.
		/// </summary>
		/// <param name="worklistClass"></param>
		/// <returns></returns>
		public static bool GetSupportsReportingStaffRoleFilter(Type worklistClass)
		{
			var a = AttributeUtils.GetAttribute<WorklistSupportsReportingStaffRoleFilterAttribute>(worklistClass, true);
			return a == null ? false : a.SupportsReportingStaffRoleFilter;
		}

		/// <summary>
		/// Gets a value indicating whether the specified worklist class behaves as a singleton, as specified by
		/// the <see cref="StaticWorklistAttribute"/>.
		/// </summary>
		/// <param name="worklistClass"></param>
		/// <returns></returns>
		public static bool GetIsStatic(Type worklistClass)
		{
			var a = AttributeUtils.GetAttribute<StaticWorklistAttribute>(worklistClass, true);
			return a == null ? false : a.IsSingleton;
		}

		#endregion

		private string _name;
		private string _description;
		private WorklistProcedureTypeFilter _procedureTypeFilter;
		private WorklistProcedureTypeGroupFilter _procedureTypeGroupFilter;
		private WorklistFacilityFilter _facilityFilter;
		private WorklistDepartmentFilter _departmentFilter;
		private WorklistPatientClassFilter _patientClassFilter;
		private WorklistPatientLocationFilter _patientLocationFilter;
		private WorklistOrderPriorityFilter _orderPriorityFilter;
		private WorklistPractitionerFilter _orderingPractitionerFilter;
		private WorklistPortableFilter _portableFilter;
		private WorklistTimeFilter _timeFilter;

		private ISet<Staff> _staffSubscribers;
		private ISet<StaffGroup> _groupSubscribers;

		private WorklistOwner _owner;

		/// <summary>
		/// No-args constructor required by NHibernate.
		/// </summary>
		protected Worklist()
		{
			_owner = WorklistOwner.Admin;   // admin owned by default
			_staffSubscribers = new HashedSet<Staff>();
			_groupSubscribers = new HashedSet<StaffGroup>();

			_procedureTypeFilter = new WorklistProcedureTypeFilter();
			_procedureTypeGroupFilter = new WorklistProcedureTypeGroupFilter();
			_facilityFilter = new WorklistFacilityFilter();
			_departmentFilter = new WorklistDepartmentFilter();
			_patientClassFilter = new WorklistPatientClassFilter();
			_patientLocationFilter = new WorklistPatientLocationFilter();
			_orderPriorityFilter = new WorklistOrderPriorityFilter();
			_orderingPractitionerFilter = new WorklistPractitionerFilter();
			_portableFilter = new WorklistPortableFilter();
			_timeFilter = new WorklistTimeFilter();
		}

		#region Public Properties

		/// <summary>
		/// Gets the logical name of this class, which may or may not be the same as its .NET class name.
		/// </summary>
		public virtual string ClassName
		{
			get { return GetClassName(this.GetClass()); }
		}

		/// <summary>
		/// Gets the fully-qualified .NET class name of this worklist instance.
		/// </summary>
		//NOTE: this property is persistent only for the purpose of enforcing the unique-key across Name and ClassName
		//there is no need to persist the class name otherwise
		[PersistentProperty]
		[Required]
		public virtual string FullClassName
		{
			get { return this.GetClass().FullName; }
			protected set
			{
				// do nothing - setter required for NH
			}
		}

		/// <summary>
		/// Gets or sets the name of the worklist instance.
		/// </summary>
		[PersistentProperty]
		[Required]
		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets the description of the worklist instance.
		/// </summary>
		[PersistentProperty]
		public virtual string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		/// Gets or sets the owner of the worklist.
		/// </summary>
		[PersistentProperty]
		public virtual WorklistOwner Owner
		{
			get { return _owner ?? WorklistOwner.Admin; }
			set
			{
				if (!Equals(_owner, value))
				{
					_owner = value;

					// if the owner is other than Admin, then the subscribers collections
					// should contain only 1 value between them (the owner)
					if (_owner != null && !_owner.IsAdminOwner)
					{
						_staffSubscribers.Clear();
						_groupSubscribers.Clear();

						if (_owner.IsStaffOwner)
							_staffSubscribers.Add(_owner.Staff);
						else if (_owner.IsGroupOwner)
							_groupSubscribers.Add(_owner.Group);
					}
				}
			}
		}

		/// <summary>
		/// Gets the set of <see cref="Staff"/> that subscribe to this worklist.
		/// </summary>
		[PersistentProperty]
		public virtual ISet<Staff> StaffSubscribers
		{
			get { return _staffSubscribers; }
			protected set { _staffSubscribers = value; }
		}

		/// <summary>
		/// Gets the set of <see cref="StaffGroup"/>s that subscribe to this worklist.
		/// </summary>
		[PersistentProperty]
		public virtual ISet<StaffGroup> GroupSubscribers
		{
			get { return _groupSubscribers; }
			protected set { _groupSubscribers = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistProcedureTypeFilter"/>.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
		public virtual WorklistProcedureTypeFilter ProcedureTypeFilter
		{
			get { return _procedureTypeFilter; }
			protected set { _procedureTypeFilter = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistProcedureTypeGroupFilter"/>.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
		public virtual WorklistProcedureTypeGroupFilter ProcedureTypeGroupFilter
		{
			get { return _procedureTypeGroupFilter; }
			protected set { _procedureTypeGroupFilter = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistFacilityFilter"/>.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
		public virtual WorklistFacilityFilter FacilityFilter
		{
			get { return _facilityFilter; }
			protected set { _facilityFilter = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistDepartmentFilter"/>.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
		public virtual WorklistDepartmentFilter DepartmentFilter
		{
			get { return _departmentFilter; }
			protected set { _departmentFilter = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistPatientClassFilter"/>.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
		public virtual WorklistPatientClassFilter PatientClassFilter
		{
			get { return _patientClassFilter; }
			protected set { _patientClassFilter = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistPatientLocationFilter"/>.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
		public virtual WorklistPatientLocationFilter PatientLocationFilter
		{
			get { return _patientLocationFilter; }
			protected set { _patientLocationFilter = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistOrderPriorityFilter"/>.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
		public virtual WorklistOrderPriorityFilter OrderPriorityFilter
		{
			get { return _orderPriorityFilter; }
			protected set { _orderPriorityFilter = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistPractitionerFilter"/> for the ordering practitioner.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
		public virtual WorklistPractitionerFilter OrderingPractitionerFilter
		{
			get { return _orderingPractitionerFilter; }
			protected set { _orderingPractitionerFilter = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistPortableFilter"/>.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
		public virtual WorklistPortableFilter PortableFilter
		{
			get { return _portableFilter; }
			protected set { _portableFilter = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistTimeFilter"/>.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
		public virtual WorklistTimeFilter TimeFilter
		{
			get { return _timeFilter; }
			protected set { _timeFilter = value; }
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Gets the list of worklist items in this worklist.
		/// </summary>
		/// <param name="wqc"></param>
		/// <returns></returns>
		public abstract IList GetWorklistItems(IWorklistQueryContext wqc);

		/// <summary>
		/// Gets the number of worklist items in this worklist.
		/// </summary>
		/// <param name="wqc"></param>
		/// <returns></returns>
		public abstract int GetWorklistItemCount(IWorklistQueryContext wqc);

		/// <summary>
		/// Gets the criteria that define the invariant aspects of this worklist.
		/// </summary>
		/// <param name="wqc"></param>
		/// <remarks>
		/// This method is called by the worklist brokers and is not intended for use by application code.
		/// </remarks>
		/// <returns></returns>
		public WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
		{
			var criteria = GetInvariantCriteriaCore(wqc);
			foreach (var criterion in criteria)
			{
				// augment criteria with time directive
				_timeFilter.Apply(criterion, this.GetClass(), GetTimeDirective(), wqc);

				// augment the criteria with the downtime flag
				criterion.Procedure.DowntimeRecoveryMode.EqualTo(wqc.DowntimeRecoveryMode);
			}
			return criteria;
		}

		/// <summary>
		/// Gets the criteria established by the worklist filters.
		/// </summary>
		/// <param name="wqc"></param>
		/// <returns></returns>
		public WorklistItemSearchCriteria[] GetFilterCriteria(IWorklistQueryContext wqc)
		{
			return GetFilterCriteriaCore(wqc);
		}

		/// <summary>
		/// Gets the projection for this worklist.
		/// </summary>
		/// <returns></returns>
		public WorklistItemProjection GetProjection()
		{
			return GetProjectionCore(GetTimeDirective().TimeField);
		}

		/// <summary>
		/// Gets the Procedure Step subclass(es) that this worklist is based on.
		/// </summary>
		public abstract Type[] GetProcedureStepSubclasses();

		/// <summary>
		/// Gets the worklist items HQL query, for debugging purposes only. 
		/// </summary>
		/// <remarks>
		/// HQL should not leak through the abstraction layer, but it is useful for debugging to sometimes be able to see the query.
		/// </remarks>
		/// <param name="wqc"></param>
		/// <returns></returns>
		public abstract string GetWorklistItemsHql(IWorklistQueryContext wqc);

		public static bool CurrentServerConfigurationRequiresTimeFilter()
		{
			var settings = new WorklistSettings();
			return settings.TimeWindowRequired;
		}

		public static int CurrentServerConfigurationMaxSpanDays()
		{
			var settings = new WorklistSettings();
			return settings.TimeWindowMaxSpanDays;
		}

		#endregion

		#region Protected API
		/// <summary>
		/// Gets the worklist item projection required to populate worklist items for this worklist.
		/// </summary>
		/// <param name="timeField"></param>
		/// <returns></returns>
		protected abstract WorklistItemProjection GetProjectionCore(WorklistItemField timeField);

		/// <summary>
		/// Gets the criteria that define the invariant aspects of this worklist.
		/// </summary>
		/// <param name="wqc"></param>
		/// <returns></returns>
		protected abstract WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc);

		/// <summary>
		/// Gets the criteria established by the worklist filters.
		/// </summary>
		/// <param name="wqc"></param>
		/// <returns></returns>
		protected virtual WorklistItemSearchCriteria[] GetFilterCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new WorklistItemSearchCriteria();

			ApplyFilterCriteria(criteria, wqc);

			return new[] { criteria };
		}

		/// <summary>
		/// Gets the directive that specifies how items in this worklist are handled wrt time.
		/// </summary>
		/// <returns></returns>
		protected abstract TimeDirective GetTimeDirective();

		#endregion

		#region Helpers

		/// <summary>
		/// Modifies the specified criteria to represent the filters that are set for this worklist.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="wqc"></param>
		protected void ApplyFilterCriteria(WorklistItemSearchCriteria criteria, IWorklistQueryContext wqc)
		{
			this.ProcedureTypeFilter.Apply(criteria.Procedure.Type, wqc);
			this.ProcedureTypeGroupFilter.Apply(criteria.Procedure.Type, wqc);
			this.FacilityFilter.Apply(criteria.Procedure.PerformingFacility, wqc);
			this.DepartmentFilter.Apply(criteria.Procedure.PerformingDepartment, wqc);
			this.OrderPriorityFilter.Apply(criteria.Order, wqc);
			this.PatientClassFilter.Apply(criteria.Visit, wqc);
			this.PatientLocationFilter.Apply(criteria.Visit, wqc);
			this.OrderingPractitionerFilter.Apply(criteria.Order.OrderingPractitioner, wqc);
			this.PortableFilter.Apply(criteria.Procedure, wqc);

			// note: the TimeFilter is treated as part of the invariant criteria, so it is not processed here
		}

		/// <summary>
		/// Checks if the time filter is enabled and spans less than the specified maximum time span.
		/// </summary>
		/// <param name="maxSpan"></param>
		/// <returns></returns>
		private bool CheckTimeFilterSpan(TimeSpan maxSpan)
		{
			if (maxSpan == TimeSpan.Zero || maxSpan == TimeSpan.MaxValue)
				return true;

			// ensure filter is enabled
			if(!_timeFilter.IsEnabled)
				return false;

			// ensure filter is constrained to be up to maxSpan
			return _timeFilter.Value.IsConstrained(TimeSpan.Zero, maxSpan);
		}

		private static IValidationRuleSet GetValidationRules()
		{
			// ensure that not both the procedure type and procedure type groups filters are being applied
			var procedureTypeRule = new ValidationRule<Worklist>(
				delegate(Worklist w)
				{
					var filterByBothProcedureTypeAndProcedureTypeGroup = w.ProcedureTypeFilter.IsEnabled &&
																		 w.ProcedureTypeGroupFilter.IsEnabled;

					return new TestResult(!filterByBothProcedureTypeAndProcedureTypeGroup, SR.MessageValidateWorklistProcedureTypeAndGroupFilters);
				});

			// ensure time filter meets constraints specified in settings
			var timeFilterRule = new ValidationRule<Worklist>(
				delegate(Worklist w)
				{
					var settings = new WorklistSettings();
					var maxDays = settings.TimeWindowMaxSpanDays;
					if (settings.TimeWindowRequired && maxDays > 0)
					{
						return new TestResult(w.CheckTimeFilterSpan(TimeSpan.FromDays(maxDays)),
							string.Format(SR.MessageValidateWorklistTimeFilter, maxDays));
					}
					return new TestResult(true);
				});

			return new ValidationRuleSet(new[] { procedureTypeRule, timeFilterRule });
		}

		#endregion
	}
}