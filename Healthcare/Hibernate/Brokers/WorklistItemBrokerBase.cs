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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Abstract base class for brokers that evaluate worklists.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class provides the basis functionality for worklist brokers.
	/// </para>
	/// </remarks>
	public abstract class WorklistItemBrokerBase : Broker, IWorklistItemBroker
	{
		#region IWorklistItemSearchContext implementation

		class SearchContext : IWorklistItemSearchContext
		{
			private readonly WorklistItemBrokerBase _owner;
			private readonly WorklistItemSearchArgs _args;
			private readonly Type _worklistItemClass;

			public SearchContext(WorklistItemBrokerBase owner, WorklistItemSearchArgs args, Type worklistItemClass)
			{
				_owner = owner;
				_args = args;
				_worklistItemClass = worklistItemClass;
			}

			public bool IncludeDegenerateProcedureItems
			{
				get { return _args.IncludeDegenerateProcedureItems; }
			}

			public bool IncludeDegeneratePatientItems
			{
				get { return _args.IncludeDegeneratePatientItems; }
			}

			public WorklistItemSearchCriteria[] SearchCriteria
			{
				get { return _args.SearchCriteria; }
			}

			public int Threshold
			{
				get { return _args.Threshold; }
			}

			public IList<WorklistItem> FindWorklistItems(WorklistItemSearchCriteria[] where)
			{
				var qbArgs = new SearchQueryArgs(_args.ProcedureStepClasses, where, _args.Projection);
				var query = _owner.BuildWorklistSearchQuery(qbArgs);

				// query may be null to signal that it should not be performed
				return query == null ? new List<WorklistItem>() :
					_owner.DoQuery(query, _worklistItemClass, _owner.WorklistItemQueryBuilder, qbArgs);
			}

			public int CountWorklistItems(WorklistItemSearchCriteria[] where)
			{
				var query = _owner.BuildWorklistSearchQuery(new SearchQueryArgs(_args.ProcedureStepClasses, where, null));
				return _owner.DoQueryCount(query);
			}

			public IList<WorklistItem> FindProcedures(WorklistItemSearchCriteria[] where)
			{
				var p = FilterProjection(_args.Projection, WorklistItemFieldLevel.Procedure);
				var qbArgs = new SearchQueryArgs((Type[])null, where, p);
				var query = _owner.BuildProcedureSearchQuery(qbArgs);
				return _owner.DoQuery(query, _worklistItemClass, _owner._procedureQueryBuilder, qbArgs);
			}

			public int CountProcedures(WorklistItemSearchCriteria[] where)
			{
				var query = _owner.BuildProcedureSearchQuery(new SearchQueryArgs((Type[])null, where, null));
				return _owner.DoQueryCount(query);
			}

			public IList<WorklistItem> FindPatients(WorklistItemSearchCriteria[] where)
			{
				var p = FilterProjection(_args.Projection, WorklistItemFieldLevel.Patient);
				var w = GetPatientCriteria(where);
				var qbArgs = new SearchQueryArgs((Type[]) null, w, p);
				var query = _owner.BuildPatientSearchQuery(qbArgs);
				return _owner.DoQuery(query, _worklistItemClass, _owner._patientQueryBuilder, qbArgs);
			}

			public int CountPatients(WorklistItemSearchCriteria[] where)
			{
				var w = GetPatientCriteria(where);
				var query = _owner.BuildPatientSearchQuery(new SearchQueryArgs((Type[])null, w, null));
				return _owner.DoQueryCount(query);
			}
		}

		#endregion

		private readonly IQueryBuilder _patientQueryBuilder;
		private readonly IQueryBuilder _procedureQueryBuilder;

		/// <summary>
		/// Constructor that uses defaults for procedure/patient search query builders.
		/// </summary>
		/// <param name="worklistItemQueryBuilder"></param>
		protected WorklistItemBrokerBase(IWorklistItemQueryBuilder worklistItemQueryBuilder)
			: this(worklistItemQueryBuilder, new ProcedureQueryBuilder(), new PatientQueryBuilder())
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="worklistItemQueryBuilder"></param>
		/// <param name="procedureSearchQueryBuilder"></param>
		/// <param name="patientSearchQueryBuilder"></param>
		protected WorklistItemBrokerBase(IWorklistItemQueryBuilder worklistItemQueryBuilder,
			IQueryBuilder procedureSearchQueryBuilder, IQueryBuilder patientSearchQueryBuilder)
		{
			this.WorklistItemQueryBuilder = worklistItemQueryBuilder;

			_patientQueryBuilder = patientSearchQueryBuilder;
			_procedureQueryBuilder = procedureSearchQueryBuilder;
		}


		#region Public API

		/// <summary>
		/// Gets the set of worklist items in the specified worklist.
		/// </summary>
		/// <remarks>
		/// Subclasses may override this method but in most cases this should not be necessary.
		/// </remarks>
		public virtual IList<TItem> GetWorklistItems<TItem>(Worklist worklist, IWorklistQueryContext wqc)
			where TItem : WorklistItem
		{
			var args = new WorklistQueryArgs(worklist, wqc, false);
			var query = BuildWorklistQuery(args);
			return DoQuery<TItem>(query, this.WorklistItemQueryBuilder, args);
		}

		/// <summary>
		/// Gets the set of items matching the specified criteria, returned as tuples shaped by the specified projection.
		/// </summary>
		public IList<object[]> GetWorklistItems(Type[] procedureStepClasses, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection, SearchResultPage page)
		{
			var args = new QueryBuilderArgs(procedureStepClasses, criteria, projection, page);
			var query = BuildWorklistQuery(args);
			return CollectionUtils.Map(ExecuteHql<object[]>(query), (object[] tuple) => this.WorklistItemQueryBuilder.PreProcessResult(tuple, args));
		}

		/// <summary>
		/// Performs a search using the specified criteria.
		/// </summary>
		public IList<TItem> GetSearchResults<TItem>(WorklistItemSearchArgs args)
			where TItem : WorklistItem
		{
			var wisc = new SearchContext(this, args, typeof(TItem));
			IWorklistItemSearchExecutionStrategy strategy = new OptimizedWorklistItemSearchExecutionStrategy();
			var results = strategy.GetSearchResults(wisc);
			return CollectionUtils.Map(results, (WorklistItem r) => (TItem)r);
		}

		/// <summary>
		/// Gets an approximate count of the results that a search using the specified criteria would return.
		/// </summary>
		public bool EstimateSearchResultsCount(WorklistItemSearchArgs args, out int count)
		{
			var wisc = new SearchContext(this, args, null);
			IWorklistItemSearchExecutionStrategy strategy = new OptimizedWorklistItemSearchExecutionStrategy();
			return strategy.EstimateSearchResultsCount(wisc, out count);
		}

		/// <summary>
		/// Gets the HQL for debugging purposes only.
		/// </summary>
		/// <param name="procedureStepClasses"></param>
		/// <param name="criteria"></param>
		/// <param name="projection"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		public string GetWorklistItemsHql(Type[] procedureStepClasses, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection, SearchResultPage page)
		{
			var args = new QueryBuilderArgs(procedureStepClasses, criteria, projection, page);
			var query = BuildWorklistQuery(args);
			return query.Hql;
		}

		/// <summary>
		/// Gets the HQL for debugging purposes only.
		/// </summary>
		/// <param name="worklist"></param>
		/// <param name="wqc"></param>
		/// <returns></returns>
		public virtual string GetWorklistItemsHql(Worklist worklist, IWorklistQueryContext wqc)
		{
			var args = new WorklistQueryArgs(worklist, wqc, false);
			var query = BuildWorklistQuery(args);
			return query.Hql;
		}


		/// <summary>
		/// Gets a count of the number of worklist items in the specified worklist.
		/// </summary>
		/// <param name="worklist"></param>
		/// <param name="wqc"></param>
		/// <returns></returns>
		/// <remarks>
		/// Subclasses may override this method but in most cases this should not be necessary.
		/// </remarks>
		public virtual int CountWorklistItems(Worklist worklist, IWorklistQueryContext wqc)
		{
			var query = BuildWorklistQuery(new WorklistQueryArgs(worklist, wqc, true));
			return DoQueryCount(query);
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Gets the query builder for worklist items.
		/// </summary>
		protected IWorklistItemQueryBuilder WorklistItemQueryBuilder { get; private set; }

		/// <summary>
		/// Gets the query builder for patient searches.
		/// </summary>
		protected IQueryBuilder PatientQueryBuilder { get { return _patientQueryBuilder; } }

		/// <summary>
		/// Gets the query builder for procedure searches.
		/// </summary>
		protected IQueryBuilder ProcedureQueryBuilder { get { return _procedureQueryBuilder; } }

		/// <summary>
		/// Executes the specified query, using the specified query-builder to pre-process the results.
		/// </summary>
		/// <typeparam name="TItem">Class of worklist item that will hold the results.</typeparam>
		/// <param name="query"></param>
		/// <param name="queryBuilder"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		protected List<TItem> DoQuery<TItem>(HqlQuery query, IQueryBuilder queryBuilder, QueryBuilderArgs args)
			where TItem : WorklistItem
		{
			var results = DoQuery(query, typeof(TItem), queryBuilder, args);
			return CollectionUtils.Map(results, (WorklistItem r) => (TItem)r);
		}

		/// <summary>
		/// Executes the specified query, using the specified query-builder to pre-process the results.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="worklistItemClass"></param>
		/// <param name="queryBuilder"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		protected IList<WorklistItem> DoQuery(HqlQuery query, Type worklistItemClass, IQueryBuilder queryBuilder, QueryBuilderArgs args)
		{
			// execute query
			var tuples = ExecuteHql<object[]>(query);

			// create a dummy worklist item, so we can get the tuple mapping exactly once, outside of the loop
			var tupleMapping = ((WorklistItem) Activator.CreateInstance(worklistItemClass)).GetTupleMapping(args.Projection);

			// use tuple mapping to create worklist items
			return CollectionUtils.Map(tuples,
				(object[] tuple) => CreateWorklistItem(worklistItemClass, queryBuilder.PreProcessResult(tuple, args), tupleMapping));
		}

		/// <summary>
		/// Executes the specified count query.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		protected int DoQueryCount(HqlQuery query)
		{
			return (int)ExecuteHqlUnique<long>(query);
		}

		/// <summary>
		/// Builds a query that searches for worklist items.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		protected HqlProjectionQuery BuildWorklistSearchQuery(QueryBuilderArgs args)
		{
			var query = new HqlProjectionQuery();
			this.WorklistItemQueryBuilder.AddRootQuery(query, args);
			this.WorklistItemQueryBuilder.AddConstrainPatientProfile(query, args);
			this.WorklistItemQueryBuilder.AddCriteria(query, args);
			this.WorklistItemQueryBuilder.AddActiveProcedureStepConstraint(query, args);

			if (args.CountQuery)
			{
				this.WorklistItemQueryBuilder.AddCountProjection(query, args);
			}
			else
			{
				this.WorklistItemQueryBuilder.AddItemProjection(query, args);
			}

			return query;
		}


		/// <summary>
		/// Builds a query that searches for patient items.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private HqlProjectionQuery BuildPatientSearchQuery(QueryBuilderArgs args)
		{
			var query = new HqlProjectionQuery();
			_patientQueryBuilder.AddRootQuery(query, null);
			_patientQueryBuilder.AddCriteria(query, args);

			if (args.CountQuery)
			{
				_patientQueryBuilder.AddCountProjection(query, args);
			}
			else
			{
				_patientQueryBuilder.AddItemProjection(query, args);
			}
			return query;
		}

		/// <summary>
		/// Builds a query that searches for procedure items.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private HqlProjectionQuery BuildProcedureSearchQuery(QueryBuilderArgs args)
		{
			var query = new HqlProjectionQuery();
			_procedureQueryBuilder.AddRootQuery(query, null);
			_procedureQueryBuilder.AddConstrainPatientProfile(query, args);
			_procedureQueryBuilder.AddCriteria(query, args);

			if (args.CountQuery)
			{
				_procedureQueryBuilder.AddCountProjection(query, args);
			}
			else
			{
				_procedureQueryBuilder.AddItemProjection(query, args);
			}
			return query;
		}

		/// <summary>
		/// Builds a worklist item query, including the ordering and paging directives.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private HqlProjectionQuery BuildWorklistQuery(QueryBuilderArgs args)
		{
			var query = new HqlProjectionQuery();
			this.WorklistItemQueryBuilder.AddRootQuery(query, args);
			this.WorklistItemQueryBuilder.AddConstrainPatientProfile(query, args);
			this.WorklistItemQueryBuilder.AddCriteria(query, args);

			if(args is WorklistQueryArgs)
			{
				this.WorklistItemQueryBuilder.AddFilters(query, (WorklistQueryArgs)args);
			}

			if (args.CountQuery)
			{
				this.WorklistItemQueryBuilder.AddCountProjection(query, args);
			}
			else
			{
				this.WorklistItemQueryBuilder.AddOrdering(query, args);
				this.WorklistItemQueryBuilder.AddItemProjection(query, args);
				this.WorklistItemQueryBuilder.AddPagingRestriction(query, args);
			}

			return query;
		}

		#endregion

		/// <summary>
		/// Creates a worklist item from the specified tuple and projection.
		/// </summary>
		/// <param name="worklistItemClass"></param>
		/// <param name="tuple"></param>
		/// <param name="mapping"></param>
		/// <returns></returns>
		private static WorklistItem CreateWorklistItem(Type worklistItemClass, object[] tuple, WorklistItem.WorklistItemFieldSetterDelegate[] mapping)
		{
			var item = (WorklistItem)Activator.CreateInstance(worklistItemClass);
			item.InitializeFromTuple(tuple, mapping);
			return item;
		}

		/// <summary>
		/// Gets a new projection that represents the specified projection filtered to the specified level.
		/// </summary>
		/// <param name="projection"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		private static WorklistItemProjection FilterProjection(WorklistItemProjection projection, WorklistItemFieldLevel level)
		{
			return projection.Filter(f => level.Includes(f.Level));
		}

		/// <summary>
		/// Gets a copy of the specified criteria, filtering out all but patient-related criteria.
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		private static WorklistItemSearchCriteria[] GetPatientCriteria(WorklistItemSearchCriteria[] where)
		{
			// create a copy of the criteria that contains only the patient profile criteria
			var filteredCopy = CollectionUtils.Map(where,
				(WorklistItemSearchCriteria criteria) =>
				(WorklistItemSearchCriteria)criteria.ClonePatientCriteriaOnly());

			return filteredCopy.FindAll(sc => !sc.IsEmpty) // remove any empties!
				.ToArray();
		}

	}
}
