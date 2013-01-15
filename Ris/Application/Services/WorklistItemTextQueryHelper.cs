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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using System;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Application.Services
{

	public class WorklistItemTextQueryHelper<TDomainItem, TSummary>
		: TextQueryHelper<TDomainItem, WorklistItemSearchCriteria, TSummary>
		where TDomainItem : WorklistItem
		where TSummary : DataContractBase
	{
		/// <summary>
		/// This class is needed as a hacky way to get some boolean flags passed around,
		/// without having to modify the <see cref="TextQueryHelper"/> super-class.
		/// </summary>
		class TextQueryCriteria : WorklistItemSearchCriteria
		{
			private readonly bool _includeDegeneratePatientItems;
			private readonly bool _includeDegenerateProcedureItems;

			public TextQueryCriteria(WorklistItemSearchCriteria that, bool includeDegeneratePatientItems, bool includeDegenerateProcedureItems)
				: base(that)
			{
				_includeDegeneratePatientItems = includeDegeneratePatientItems;
				_includeDegenerateProcedureItems = includeDegenerateProcedureItems;
			}

			public bool IncludeDegeneratePatientItems
			{
				get { return _includeDegeneratePatientItems; }
			}

			public bool IncludeDegenerateProcedureItems
			{
				get { return _includeDegenerateProcedureItems; }
			}
		}


		private readonly Type[] _procedureStepClasses;
		private readonly IWorklistItemBroker _broker;
		private readonly WorklistItemProjection _projection;
		private readonly WorklistItemTextQueryOptions _options;
		private readonly IPersistenceContext _context;

		/// <summary>
		/// Constructor.
		/// </summary>
		public WorklistItemTextQueryHelper(
			IWorklistItemBroker broker,
			Converter<TDomainItem, TSummary> summaryAssembler,
			Type procedureStepClass,
			WorklistItemProjection projection,
			WorklistItemTextQueryOptions options,
			IPersistenceContext context)
			: this(broker, summaryAssembler, new []{procedureStepClass}, projection, options, context)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public WorklistItemTextQueryHelper(
			IWorklistItemBroker broker,
			Converter<TDomainItem, TSummary> summaryAssembler,
			Type[] procedureStepClasses,
			WorklistItemProjection projection,
			WorklistItemTextQueryOptions options,
			IPersistenceContext context)
			: base(null, summaryAssembler, null, null)
		{
			_broker = broker;
			_procedureStepClasses = procedureStepClasses;
			_projection = projection;
			_options = options;
			_context = context;
		}

		#region Overrides

		protected override bool ValidateRequest(TextQueryRequest request)
		{
			// if the UseAdvancedSearch flag is set, check if the Search fields are empty
			var req = (WorklistItemTextQueryRequest)request;
			if (req.UseAdvancedSearch)
			{
				return req.SearchFields != null && !req.SearchFields.IsEmpty();
			}

			// otherwise, do base behaviour (check text query)
			return base.ValidateRequest(request);
		}

		protected override WorklistItemSearchCriteria[] BuildCriteria(TextQueryRequest request)
		{
			var req = (WorklistItemTextQueryRequest)request;
			var criteria = new List<WorklistItemSearchCriteria>();

			if ((_options & WorklistItemTextQueryOptions.PatientOrder) == WorklistItemTextQueryOptions.PatientOrder)
			{
				criteria.AddRange(BuildProcedureSearchCriteria(req));
			}

			if ((_options & WorklistItemTextQueryOptions.ProcedureStepStaff) == WorklistItemTextQueryOptions.ProcedureStepStaff)
			{
				criteria.AddRange(BuildStaffSearchCriteria(req));
			}

			// add constraint for downtime vs live procedures
			var downtimeRecoveryMode = (_options & WorklistItemTextQueryOptions.DowntimeRecovery) ==
										WorklistItemTextQueryOptions.DowntimeRecovery;
			criteria.ForEach(c => c.Procedure.DowntimeRecoveryMode.EqualTo(downtimeRecoveryMode));

			// this is a silly hack to append additional information (degenerate flags) into the criteria so that we can
			// pass them on to the TestSpecificity and DoQuery methods (didn't want to refactor the superclass)
			var augmented = CollectionUtils.Map<WorklistItemSearchCriteria, WorklistItemSearchCriteria>(
				criteria,
				c => new TextQueryCriteria(c, ShouldIncludeDegeneratePatientItems(req), ShouldIncludeDegenerateProcedureItems(req)));

			return augmented.ToArray();
		}

		protected override bool TestSpecificity(WorklistItemSearchCriteria[] where, int threshold)
		{
			var c = (TextQueryCriteria)CollectionUtils.FirstElement(where);
			var searchArgs = new WorklistItemSearchArgs(
				_procedureStepClasses,
				where,
				_projection,
				c.IncludeDegeneratePatientItems,
				c.IncludeDegenerateProcedureItems,
				threshold);

			int count;
			return _broker.EstimateSearchResultsCount(searchArgs, out count);
		}

		protected override IList<TDomainItem> DoQuery(WorklistItemSearchCriteria[] where, SearchResultPage page)
		{
			var c = (TextQueryCriteria)CollectionUtils.FirstElement(where);
			var searchArgs = new WorklistItemSearchArgs(
				_procedureStepClasses,
				where,
				_projection,
				c.IncludeDegeneratePatientItems,
				c.IncludeDegenerateProcedureItems);

			return _broker.GetSearchResults<TDomainItem>(searchArgs);
		}

		#endregion

		#region Patient Criteria builders

		private List<WorklistItemSearchCriteria> BuildProcedureSearchCriteria(WorklistItemTextQueryRequest request)
		{
			return request.UseAdvancedSearch ? 
				BuildAdvancedProcedureSearchCriteria(request) : BuildTextQueryProcedureSearchCriteria(request);
		}

		private List<WorklistItemSearchCriteria> BuildAdvancedProcedureSearchCriteria(WorklistItemTextQueryRequest request)
		{
			Platform.CheckMemberIsSet(request.SearchFields, "SearchFields");

			var searchParams = request.SearchFields;


			var wheres = new List<WorklistItemSearchCriteria>();

			// construct a base criteria object from the request values
			var criteria = new WorklistItemSearchCriteria();

			ApplyStringCriteria(criteria.PatientProfile.Mrn.Id, searchParams.Mrn, ShouldUseExactMatchingOnIdentifiers(request));
			ApplyStringCriteria(criteria.PatientProfile.Name.FamilyName, searchParams.FamilyName);
			ApplyStringCriteria(criteria.PatientProfile.Name.GivenName, searchParams.GivenName);
			ApplyStringCriteria(criteria.PatientProfile.Healthcard.Id, searchParams.HealthcardNumber, ShouldUseExactMatchingOnIdentifiers(request));
			ApplyStringCriteria(criteria.Order.AccessionNumber, searchParams.AccessionNumber, ShouldUseExactMatchingOnIdentifiers(request));

			if (searchParams.OrderingPractitionerRef != null)
			{
				var orderedBy = _context.Load<ExternalPractitioner>(searchParams.OrderingPractitionerRef, EntityLoadFlags.Proxy);
				criteria.Order.OrderingPractitioner.EqualTo(orderedBy);
			}

			if (searchParams.DiagnosticServiceRef != null)
			{
				var ds = _context.Load<DiagnosticService>(searchParams.DiagnosticServiceRef, EntityLoadFlags.Proxy);
				criteria.Order.DiagnosticService.EqualTo(ds);
			}

			if (searchParams.ProcedureTypeRef != null)
			{
				var pt = _context.Load<ProcedureType>(searchParams.ProcedureTypeRef, EntityLoadFlags.Proxy);
				criteria.Procedure.Type.EqualTo(pt);
			}

			if (searchParams.FromDate != null || searchParams.UntilDate != null)
			{
				// the goal here is to use the date-range in an approximate fashion, to search for procedures
				// that were performed "around" that time-frame
				// therefore, the date-range is applied to muliple dates, and these are OR'd

				// use "day" resolution on the start and end times, because we don't care about time
				var start = searchParams.FromDate == null ? null
					: new WorklistTimePoint(searchParams.FromDate.Value, WorklistTimePoint.Resolutions.Day);
				var end = searchParams.UntilDate == null ? null
					: new WorklistTimePoint(searchParams.UntilDate.Value, WorklistTimePoint.Resolutions.Day);

				var dateRange = new WorklistTimeRange(start, end);
				var now = Platform.Time;

				var procSchedDateCriteria = (WorklistItemSearchCriteria)criteria.Clone();
				dateRange.Apply((ISearchCondition)procSchedDateCriteria.Procedure.ScheduledStartTime, now);
				wheres.Add(procSchedDateCriteria);

				var procStartDateCriteria = (WorklistItemSearchCriteria)criteria.Clone();
				dateRange.Apply((ISearchCondition)procStartDateCriteria.Procedure.StartTime, now);
				wheres.Add(procStartDateCriteria);

				var procEndDateCriteria = (WorklistItemSearchCriteria)criteria.Clone();
				dateRange.Apply((ISearchCondition)procEndDateCriteria.Procedure.EndTime, now);
				wheres.Add(procEndDateCriteria);

			}
			else
			{
				// no date range, so just need a single criteria
				wheres.Add(criteria);
			}

			return wheres;
		}

		private List<WorklistItemSearchCriteria> BuildTextQueryProcedureSearchCriteria(WorklistItemTextQueryRequest request)
		{
			var query = request.TextQuery;

			// this will hold all criteria
			var criteria = new List<WorklistItemSearchCriteria>();

			// build criteria against names
			var names = ParsePersonNames(query);
			criteria.AddRange(CollectionUtils.Map(names,
				delegate(PersonName n)
				{
					var sc = new WorklistItemSearchCriteria();
					ApplyStringCriteria(sc.PatientProfile.Name.FamilyName, n.FamilyName);
					ApplyStringCriteria(sc.PatientProfile.Name.GivenName, n.GivenName);
					return sc;
				}));

			// build criteria against Mrn identifiers
			var ids = ParseIdentifiers(query);
			criteria.AddRange(CollectionUtils.Map(ids,
				delegate(string word)
				{
					var c = new WorklistItemSearchCriteria();
					ApplyStringCriteria(c.PatientProfile.Mrn.Id, word, ShouldUseExactMatchingOnIdentifiers(request));
					return c;
				}));

			// build criteria against Healthcard identifiers
			criteria.AddRange(CollectionUtils.Map(ids,
				delegate(string word)
				{
					var c = new WorklistItemSearchCriteria();
					ApplyStringCriteria(c.PatientProfile.Healthcard.Id, word, ShouldUseExactMatchingOnIdentifiers(request));
					return c;
				}));

			// build criteria against Accession Number
			criteria.AddRange(CollectionUtils.Map(ids,
				delegate(string word)
				{
					var c = new WorklistItemSearchCriteria();
					ApplyStringCriteria(c.Order.AccessionNumber, word, ShouldUseExactMatchingOnIdentifiers(request));
					return c;
				}));

			return criteria;
		}

		#endregion

		#region Staff Criteria builders

		private IEnumerable<WorklistItemSearchCriteria> BuildStaffSearchCriteria(WorklistItemTextQueryRequest request)
		{
			if (request.UseAdvancedSearch)
			{
				// advanced search not supported here
				throw new NotSupportedException();
			}

			return BuildTextQueryStaffSearchCriteria(request);
		}

		private List<WorklistItemSearchCriteria> BuildTextQueryStaffSearchCriteria(WorklistItemTextQueryRequest request)
		{
			var query = request.TextQuery;

			// this will hold all criteria
			var criteria = new List<WorklistItemSearchCriteria>();

			// build criteria against names
			var names = ParsePersonNames(query);

			// scheduled performer
			criteria.AddRange(CollectionUtils.Map(names,
				delegate(PersonName n)
				{
					var sc = new WorklistItemSearchCriteria();

					var scheduledPerformerNameCriteria = sc.ProcedureStep.Scheduling.Performer.Staff.Name;
					ApplyStringCriteria(scheduledPerformerNameCriteria.FamilyName, n.FamilyName);
					ApplyStringCriteria(scheduledPerformerNameCriteria.GivenName, n.GivenName);
					return sc;
				}));

			// actual performer
			criteria.AddRange(CollectionUtils.Map(names,
				delegate(PersonName n)
				{
					var sc = new WorklistItemSearchCriteria();

					var performerNameCriteria = sc.ProcedureStep.Performer.Staff.Name;
					ApplyStringCriteria(performerNameCriteria.FamilyName, n.FamilyName);
					ApplyStringCriteria(performerNameCriteria.GivenName, n.GivenName);
					return sc;
				}));

			// build criteria against Staff ID identifiers
			// bug #3952: use ParseTerms instead of ParseIdentifiers, because a Staff ID might only contain letters
			var ids = ParseTerms(query);

			// scheduled performer
			criteria.AddRange(CollectionUtils.Map(ids,
				delegate(string id)
				{
					var sc = new WorklistItemSearchCriteria();
					ApplyStringCriteria(sc.ProcedureStep.Scheduling.Performer.Staff.Id, id, ShouldUseExactMatchingOnIdentifiers(request));
					return sc;
				}));

			// actual performer
			criteria.AddRange(CollectionUtils.Map(ids,
				delegate(string id)
				{
					var sc = new WorklistItemSearchCriteria();
					ApplyStringCriteria(sc.ProcedureStep.Performer.Staff.Id, id, ShouldUseExactMatchingOnIdentifiers(request));
					return sc;
				}));

			return criteria;
		}

		#endregion

		private bool ShouldIncludeDegenerateProcedureItems(WorklistItemTextQueryRequest request)
		{
			// generally, if the search query is being used on patients/orders, then it makes sense to include
			// degenerate procedure items
			// conversely, if this flag is not present, then including degenerate items could result in an open query
			// on the entire database which would obviously not be desirable
			return (_options & WorklistItemTextQueryOptions.PatientOrder) == WorklistItemTextQueryOptions.PatientOrder;
		}

		private bool ShouldIncludeDegeneratePatientItems(WorklistItemTextQueryRequest request)
		{
			// include degenerate patient items iff
			// 1) degen procedure items are being included, and
			// 2) advanced search is not being used, or it is being used and all non-patient search criteria are empty
			return ShouldIncludeDegenerateProcedureItems(request)
				   && (!request.UseAdvancedSearch || request.SearchFields.IsNonPatientFieldsEmpty());
		}

		private bool ShouldUseExactMatchingOnIdentifiers(WorklistItemTextQueryRequest request)
		{
			// use exact matching if the option to enable partial matching is not specified
			return !((_options & WorklistItemTextQueryOptions.EnablePartialMatchingOnIdentifiers)
				== WorklistItemTextQueryOptions.EnablePartialMatchingOnIdentifiers);
		}
	}
}