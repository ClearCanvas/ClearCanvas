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
using System.Collections.Generic;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class OperationEnablementAttribute : Attribute
	{
		private readonly string _enablementMethodName;

		public OperationEnablementAttribute(string enablementMethodName)
		{
			_enablementMethodName = enablementMethodName;
		}

		public string EnablementMethodName
		{
			get { return _enablementMethodName; }
		}
	}


	public abstract class WorkflowServiceBase : ApplicationServiceBase, IWorkflowService
	{
		#region ProbingWorklistQueryContext

		class ProbingWorklistQueryContext : IWorklistQueryContext
		{
			private readonly WorklistQueryContext _wqc;

			/// <summary>
			/// Gets a value indicating if the worklist depends on the executing staff.
			/// </summary>
			public bool DependsOnExecutingStaff { get; private set; }

			public ProbingWorklistQueryContext(ApplicationServiceBase service, Facility workingFacility, SearchResultPage page, bool downtimeRecoveryMode)
			{
				_wqc = new WorklistQueryContext(service, workingFacility, page, downtimeRecoveryMode);
			}

			public Staff ExecutingStaff
			{
				get
				{
					DependsOnExecutingStaff = true;
					return _wqc.ExecutingStaff;
				}
			}

			public Facility WorkingFacility
			{
				get { return _wqc.WorkingFacility; }
			}

			public bool DowntimeRecoveryMode
			{
				get { return _wqc.DowntimeRecoveryMode; }
			}

			public SearchResultPage Page
			{
				get { return _wqc.Page; }
			}

			public TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker
			{
				return _wqc.GetBroker<TBrokerInterface>();
			}
		}

		#endregion

		#region IWorkflowService implementation

		/// <summary>
		/// Obtain the list of worklists for the current user.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[ReadOperation]
		public ListWorklistsForUserResponse ListWorklistsForUser(ListWorklistsForUserRequest request)
		{
			var assembler = new WorklistAssembler();
			return new ListWorklistsForUserResponse(
				CollectionUtils.Map<Worklist, WorklistSummary>(
					PersistenceContext.GetBroker<IWorklistBroker>().Find(CurrentUserStaff, request.WorklistTokens),
					worklist => assembler.GetWorklistSummary(worklist, PersistenceContext)));
		}

		[ReadOperation]
		public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
		{
			//TODO: is this appropriate? or should we throw an exception?
			if (request.WorkItem == null)
				return new GetOperationEnablementResponse(new Dictionary<string, bool>());

			return new GetOperationEnablementResponse(GetOperationEnablement(GetWorkItemKey(request.WorkItem)));
		}


		#endregion

		#region Protected API

		/// <summary>
		/// Extracts a work-item key from the specified work-item, or returns null if the item cannot be converted to a key.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected abstract object GetWorkItemKey(object item);


		/// <summary>
		/// Helper method to query a worklist.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <typeparam name="TSummary"></typeparam>
		/// <param name="request"></param>
		/// <param name="mapCallback"></param>
		/// <returns></returns>
		protected QueryWorklistResponse<TSummary> QueryWorklistHelper<TItem, TSummary>(QueryWorklistRequest request,
			Converter<TItem, TSummary> mapCallback)
		{
			IWorklist worklist = GetWorklist(request);

			IList results = null;
			var page = new SearchResultPage(request.Page.FirstRow, request.Page.MaxRows);
			var workingFacility = GetWorkingFacility(request);
			if (request.QueryItems)
			{
				// get the first page, up to the default max number of items per page
				results = worklist.GetWorklistItems(new WorklistQueryContext(this, workingFacility, page, request.DowntimeRecoveryMode));
			}

			var count = -1;
			if (request.QueryCount)
			{
				// if the items were already queried, and the number returned is less than the max per page, and this is the first page
				// then there is no need to do a separate count query
				if (results != null && results.Count < page.MaxRows && page.FirstRow == 0)
					count = results.Count;
				else
					count = worklist.GetWorklistItemCount(new WorklistQueryContext(this, workingFacility, null, request.DowntimeRecoveryMode));
			}

			return new QueryWorklistResponse<TSummary>(
				request.QueryItems ? CollectionUtils.Map(results, mapCallback) : null, count);
		}

		protected ResponseCachingDirective GetQueryWorklistCacheDirective(object request, object response)
		{
			var req = (QueryWorklistRequest)request;

			// items queries are never cached
			if (req.QueryItems)
				return ResponseCachingDirective.DoNotCacheDirective;

			var settings = new WorklistSettings();
			if(IsWorklistUserDependent(req))
			{
				// return user-dependent cache directive according to settings
				// cache on client, since it is user-dependent
				return new ResponseCachingDirective(
					settings.UserDependentWorklistItemCountCachingEnabled,
					TimeSpan.FromSeconds(settings.UserDependentWorklistItemCountCachingTimeToLiveSeconds),
					ResponseCachingSite.Client);
			}
			else
			{
				// return user-independent cache directive according to settings
				// cache on server, to benefit from sharing among all users
				return new ResponseCachingDirective(
					settings.UserIndependentWorklistItemCountCachingEnabled,
					TimeSpan.FromSeconds(settings.UserIndependentWorklistItemCountCachingTimeToLiveSeconds),
					ResponseCachingSite.Server);
			}
		}


		/// <summary>
		/// Helper method that implements the logic for performing searches on worklists.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <typeparam name="TSummary"></typeparam>
		/// <param name="request"></param>
		/// <param name="broker"></param>
		/// <param name="projection"></param>
		/// <param name="mapCallback"></param>
		/// <returns></returns>
		protected TextQueryResponse<TSummary> SearchHelper<TItem, TSummary>(
			WorklistItemTextQueryRequest request,
			IWorklistItemBroker broker,
			WorklistItemProjection projection,
			Converter<TItem, TSummary> mapCallback)
			where TSummary : DataContractBase
			where TItem : WorklistItem
		{
			var procedureStepClass = request.ProcedureStepClassName == null ? null
				: ProcedureStep.GetSubClass(request.ProcedureStepClassName, PersistenceContext);

			var helper = new WorklistItemTextQueryHelper<TItem, TSummary>(broker, mapCallback, procedureStepClass, projection, request.Options, PersistenceContext);

			return helper.Query(request);
		}

		#endregion

		#region Private

		private Worklist GetWorklist(QueryWorklistRequest request)
		{
			return request.WorklistRef != null ?
				this.PersistenceContext.Load<Worklist>(request.WorklistRef) :
				WorklistFactory.Instance.CreateWorklist(request.WorklistClass);
		}

		private Facility GetWorkingFacility(QueryWorklistRequest request)
		{
			return request.WorkingFacilityRef == null ? null :
				PersistenceContext.Load<Facility>(request.WorkingFacilityRef, EntityLoadFlags.Proxy);
		}


		private bool IsWorklistUserDependent(QueryWorklistRequest req)
		{
			// check if the worklist has a dependency on the executing staff (eg current user)
			var worklist = GetWorklist(req);
			var workingFacility = GetWorkingFacility(req);
			var probingWqc = new ProbingWorklistQueryContext(this, workingFacility, req.Page, req.DowntimeRecoveryMode);

			// get the worklist to apply all of its criteria
			worklist.GetInvariantCriteria(probingWqc);
			worklist.GetFilterCriteria(probingWqc);

			// return value indicating dependency on executing staff
			return probingWqc.DependsOnExecutingStaff;
		}

		/// <summary>
		/// Helper method to determine operation enablement for.
		/// </summary>
		/// <param name="itemKey"></param>
		/// <returns></returns>
		private Dictionary<string, bool> GetOperationEnablement(object itemKey)
		{
			var results = new Dictionary<string, bool>();
			if (itemKey == null)
				return results;

			var serviceContractType = this.GetType();
			foreach (var info in serviceContractType.GetMethods())
			{
				var attribs = info.GetCustomAttributes(typeof(OperationEnablementAttribute), true);
				if (attribs.Length < 1)
					continue;

				// Evaluate the list of enablement method in the OperationEnablementAttribute

				var enablement = true;
				foreach (var obj in attribs)
				{
					var attrib = (OperationEnablementAttribute)obj;

					var enablementHelper = serviceContractType.GetMethod(attrib.EnablementMethodName);
					if (enablementHelper == null)
						throw new EnablementMethodNotFoundException(attrib.EnablementMethodName, info.Name);

					var test = (bool)enablementHelper.Invoke(this, new[] { itemKey });
					if (test == false)
					{
						// No need to continue after any evaluation failed
						enablement = false;
						break;
					}
				}

				results.Add(info.Name, enablement);
			}

			return results;
		}

		#endregion

	}
}
