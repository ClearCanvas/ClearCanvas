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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Abstract base-class for worklist filters.
	/// </summary>
	public abstract class WorklistFilter : ValueObject
	{
		private bool _isEnabled;

		/// <summary>
		/// Gets or sets a value indicating whether this filter is enabled or not.
		/// </summary>
		/// <remarks>
		/// This property is significant in that it may allow the worklist broker to make optimizations
		/// when building the query by not loading the filter values for disabled filters.
		/// </remarks>
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set { _isEnabled = value; }
		}
	}

	/// <summary>
	/// Abstract base-class for single-valued worklist filters.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class WorklistSingleValuedFilter<T> : WorklistFilter, IEquatable<WorklistSingleValuedFilter<T>>
	{
		private T _value;

		/// <summary>
		/// Gets or sets the value of this filter.
		/// </summary>
		public T Value
		{
			get { return _value; }
			set { _value = value; }
		}

		#region Object overrides

		public override object Clone()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool Equals(WorklistSingleValuedFilter<T> that)
		{
			if (that == null) return false;
			return Equals(_value, that._value) && Equals(this.IsEnabled, that.IsEnabled);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as WorklistSingleValuedFilter<T>);
		}

		public override int GetHashCode()
		{
			return (_value != null ? _value.GetHashCode() : 0) + 29 * IsEnabled.GetHashCode();
		}

		#endregion
	}

	/// <summary>
	/// Abstract base-class for multi-valued filters.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class WorklistMultiValuedFilter<T> : WorklistFilter, IEquatable<WorklistMultiValuedFilter<T>>
	{
		private Iesi.Collections.Generic.ISet<T> _values;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected WorklistMultiValuedFilter()
		{
			_values = new HashedSet<T>();
		}

		/// <summary>
		/// Gets the set of values for this filter.
		/// </summary>
		public Iesi.Collections.Generic.ISet<T> Values
		{
			get { return _values; }
			// private setter for NHibernate compatibility
			private set { _values = value; }
		}

		#region Object overrides

		public override object Clone()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool Equals(WorklistMultiValuedFilter<T> that)
		{
			if (that == null) return false;
			return Equals(IsEnabled, that.IsEnabled) && Equals(_values, that._values);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as WorklistMultiValuedFilter<T>);
		}

		public override int GetHashCode()
		{
			return _values.GetHashCode() + 29 * IsEnabled.GetHashCode();
		}

		#endregion
	}

	/// <summary>
	/// Defines a filter that limits worklist items to procedures that fall into a specified
	/// set of <see cref="ProcedureType"/>s.
	/// </summary>
	public class WorklistProcedureTypeFilter : WorklistMultiValuedFilter<ProcedureType>
	{
		/// <summary>
		/// Applies this filter to the specified criteria object.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="wqc"></param>
		public void Apply(ProcedureTypeSearchCriteria criteria, IWorklistQueryContext wqc)
		{
			if (!this.IsEnabled)
				return;

			criteria.In(this.Values);
		}
	}

	/// <summary>
	/// Defines a filter that limits worklist items to procedures that fall into a specified
	/// set of <see cref="ProcedureTypeGroup"/>s.
	/// </summary>
	public class WorklistProcedureTypeGroupFilter : WorklistMultiValuedFilter<ProcedureTypeGroup>
	{
		/// <summary>
		/// Applies this filter to the specified criteria object.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="wqc"></param>
		public void Apply(ProcedureTypeSearchCriteria criteria, IWorklistQueryContext wqc)
		{
			if (!this.IsEnabled)
				return;

			// TODO: this is pretty brute force and probably needs to be optimized with a custom broker
			var procedureTypes = new List<ProcedureType>();
			foreach (var procedureTypeGroup in this.Values)
			{
				procedureTypes.AddRange(procedureTypeGroup.ProcedureTypes);
			}

			criteria.In(CollectionUtils.Unique(procedureTypes));
		}
	}

	/// <summary>
	/// Defines a filter that limits worklist items to procedures that are to performed
	/// at specified <see cref="Facility"/>s, or at the current working facility.
	/// </summary>
	public class WorklistFacilityFilter : WorklistMultiValuedFilter<Facility>
	{
		private bool _includeWorkingFacility;

		/// <summary>
		/// Gets or sets a value indicating whether the current working facility should be included
		/// in the worklist query.
		/// </summary>
		public bool IncludeWorkingFacility
		{
			get { return _includeWorkingFacility; }
			set { _includeWorkingFacility = value; }
		}

		/// <summary>
		/// Applies this filter to the specified criteria object.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="wqc"></param>
		public void Apply(FacilitySearchCriteria criteria, IWorklistQueryContext wqc)
		{
			if (!this.IsEnabled)
				return;

			var values = new List<Facility>(this.Values);
			if (this.IncludeWorkingFacility)
			{
				values.Add(wqc.WorkingFacility);
			}
			criteria.In(values);
		}
	}

	/// <summary>
	/// Defines a filter that limits worklist items to procedures that are to performed
	/// at specified <see cref="Department"/>s.
	/// </summary>
	public class WorklistDepartmentFilter : WorklistMultiValuedFilter<Department>
	{
		/// <summary>
		/// Applies this filter to the specified criteria object.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="wqc"></param>
		public void Apply(DepartmentSearchCriteria criteria, IWorklistQueryContext wqc)
		{
			if (!this.IsEnabled)
				return;

			criteria.In(this.Values);
		}
	}

	/// <summary>
	/// Defines a filter that limits worklist items to those visits that fall into a specified
	/// set of <see cref="PatientClassEnum"/> values.
	/// </summary>
	public class WorklistPatientClassFilter : WorklistMultiValuedFilter<PatientClassEnum>
	{
		/// <summary>
		/// Applies this filter to the specified criteria object.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="wqc"></param>
		public void Apply(VisitSearchCriteria criteria, IWorklistQueryContext wqc)
		{
			if (!this.IsEnabled)
				return;

			criteria.PatientClass.In(this.Values);
		}
	}

	/// <summary>
	/// Defines a filter that limits worklist items to those visits where the patient location falls into a specified
	/// set of <see cref="Location"/> values.
	/// </summary>
	public class WorklistPatientLocationFilter : WorklistMultiValuedFilter<Location>
	{
		/// <summary>
		/// Applies this filter to the specified criteria object.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="wqc"></param>
		public void Apply(VisitSearchCriteria criteria, IWorklistQueryContext wqc)
		{
			if (!this.IsEnabled)
				return;

			criteria.CurrentLocation.In(this.Values);
		}
	}

	/// <summary>
	/// Defines a filter that limits worklist items to those orders that fall into a specified
	/// set of <see cref="OrderPriorityEnum"/> values.
	/// </summary>
	public class WorklistOrderPriorityFilter : WorklistMultiValuedFilter<OrderPriorityEnum>
	{
		/// <summary>
		/// Applies this filter to the specified criteria object.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="wqc"></param>
		public void Apply(OrderSearchCriteria criteria, IWorklistQueryContext wqc)
		{
			if (!this.IsEnabled)
				return;

			criteria.Priority.In(
				CollectionUtils.Map(this.Values,
				(OrderPriorityEnum v) => (OrderPriority)Enum.Parse(typeof(OrderPriority), v.Code)));
		}
	}

	/// <summary>
	/// Defines a filter that limits worklist items based on a specified 
	/// set of <see cref="ExternalPractitioner"/> values.
	/// </summary>
	public class WorklistPractitionerFilter : WorklistMultiValuedFilter<ExternalPractitioner>
	{
		/// <summary>
		/// Applies this filter to the specified criteria object.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="wqc"></param>
		public void Apply(ExternalPractitionerSearchCriteria criteria, IWorklistQueryContext wqc)
		{
			if (!this.IsEnabled)
				return;

			criteria.In(this.Values);
		}
	}

	/// <summary>
	/// Defines a filter that limits worklist items to those procedures are either portable
	/// or non-portable, according to the <see cref="Procedure.Portable"/> property.
	/// </summary>
	public class WorklistPortableFilter : WorklistSingleValuedFilter<bool>
	{
		/// <summary>
		/// Applies this filter to the specified criteria object.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="wqc"></param>
		public void Apply(ProcedureSearchCriteria criteria, IWorklistQueryContext wqc)
		{
			if (!this.IsEnabled)
				return;

			criteria.Portable.EqualTo(this.Value);
		}
	}

	/// <summary>
	/// Defines a filter that limits worklist items to those that fall into a specified
	/// time-range.
	/// </summary>
	public class WorklistTimeFilter : WorklistSingleValuedFilter<WorklistTimeRange>
	{
		/// <summary>
		/// Applies this filter to the specified criteria object.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="worklistClass"></param>
		/// <param name="timeDirective"></param>
		/// <param name="wqc"></param>
		public void Apply(WorklistItemSearchCriteria criteria, Type worklistClass, Worklist.TimeDirective timeDirective, IWorklistQueryContext wqc)
		{
			var subCriteria = criteria.GetTimeFieldSubCriteria(timeDirective.TimeField);
			if (!(subCriteria is ISearchCondition))
				throw new ArgumentException("Specified worklist field does not seem to be a time field.");

			var condition = subCriteria as ISearchCondition;

			// apply ordering
			if(timeDirective.HonourOrderPriority)
			{
				criteria.Order.PriorityRank.SortDesc(0);
				ApplyOrdering(timeDirective.Ordering, condition, 1);
			}
			else
			{
				ApplyOrdering(timeDirective.Ordering, condition, 0);
			}


			// apply range filtering, if supported by the worklist class, and not downtime recovery mode
			if (!wqc.DowntimeRecoveryMode)
			{
				var range = this.IsEnabled ? this.Value : timeDirective.DefaultRange;
				if (range != null)
					range.Apply(condition, Platform.Time);
			}
		}

		private static void ApplyOrdering(Worklist.WorklistOrdering ordering, ISearchCondition condition, int index)
		{
			if (ordering == Worklist.WorklistOrdering.PrioritizeOldestItems)
				condition.SortAsc(index);
			else
				condition.SortDesc(index);
		}
	}

	/// <summary>
	/// Defines a filter that limits worklist items based on a specified 
	/// set of <see cref="Staff"/> values.
	/// </summary>
	public class WorklistStaffFilter : WorklistMultiValuedFilter<Staff>
	{
		private bool _includeCurrentStaff;

		/// <summary>
		/// Gets or sets a value indicating whether the current staff should be included
		/// in the worklist query.
		/// </summary>
		public bool IncludeCurrentStaff
		{
			get { return _includeCurrentStaff; }
			set { _includeCurrentStaff = value; }
		}

		/// <summary>
		/// Applies this filter to the specified criteria object.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="wqc"></param>
		public void Apply(StaffSearchCriteria criteria, IWorklistQueryContext wqc)
		{
			if(!this.IsEnabled)
				return;

			var values = new List<Staff>(this.Values);
			if (this.IncludeCurrentStaff)
			{
				values.Add(wqc.ExecutingStaff);
			}
			criteria.In(values);
		}
	}
}
