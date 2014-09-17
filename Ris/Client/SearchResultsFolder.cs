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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	public abstract class SearchResultsFolder : WorkflowFolder
	{
		protected bool IsValid { get; set; }

		#region Folder overrides

		public override bool SupportsPaging
		{
			get { return false; }
		}

		protected override void InvalidateCore()
		{
			// do nothing.  Search results only become invalid if SearchParams have changed, not if the folder system 
			// is invalidated.
			return;
		}

		protected override bool UpdateCore()
		{
			// only initiate a query if this folder is open (eg selected)
			// this folder does not support updating the count independently of the items
			if (this.IsOpen && !IsValid)
			{
				BeginQueryItems();
				IsValid = true;
				return true;
			}
			return false;
		}

		protected override IconSet OpenIconSet
		{
			get
			{
				return new IconSet("SearchFolderOpenSmall.png", "SearchFolderOpenMedium.png", "SearchFolderOpenLarge.png");
			}
		}

		protected override IconSet ClosedIconSet
		{
			get
			{
				return new IconSet("SearchFolderClosedSmall.png", "SearchFolderClosedMedium.png", "SearchFolderClosedLarge.png");
			}
		}

		#endregion

		protected virtual int SearchCriteriaSpecificityThreshold
		{
			get { return FolderSystemSettings.Default.SearchCriteriaSpecificityThreshold; }
		}

	}

	public abstract class SearchResultsFolder<TSearchParams> : SearchResultsFolder
		where TSearchParams : SearchParams
	{
		private TSearchParams _searchParams;

		/// <summary>
		/// Gets or sets the search arguments.  Setting this property will invalidate this folder.
		/// </summary>
		public TSearchParams SearchParams
		{
			get { return _searchParams; }
			set
			{
				_searchParams = value;
				this.Tooltip = SR.MessageSearchResults;

				// invalidate the folder immediately
				IsValid = false;
			}
		}
	}

	/// <summary>
	/// Base class for folders that display search results.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <typeparam name="TSearchParams"></typeparam>
	public abstract class SearchResultsFolder<TItem, TSearchParams> : SearchResultsFolder<TSearchParams>
		where TItem : DataContractBase
		where TSearchParams : SearchParams
	{
		private readonly Table<TItem> _itemsTable;
		private BackgroundTask _queryItemsTask;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="itemsTable"></param>
		protected SearchResultsFolder(Table<TItem> itemsTable)
		{
			_itemsTable = itemsTable;
		}

		#region Folder overrides

		public override ITable ItemsTable
		{
			get { return _itemsTable; }
		}

		protected override void BeginQueryCount()
		{
			// not supported on search folders
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Called to execute the search query.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="specificityThreshold"></param>
		/// <returns></returns>
		protected abstract TextQueryResponse<TItem> DoQuery(TSearchParams query, int specificityThreshold);

		#endregion

		#region Helpers

		protected override void BeginQueryItems()
		{
			if (_queryItemsTask != null)
			{
				// a search is already in progress, and should be abandoned
				// unsubscribe from its Terminated event, which effectively orphans it
				_queryItemsTask.Terminated -= OnQueryItemsCompleted;
			}

			if (this.SearchParams != null)
			{
				BeginUpdate();

				_queryItemsTask = new BackgroundTask(
					delegate(IBackgroundTaskContext taskContext)
					{
						try
						{
							var response = DoQuery(this.SearchParams, SearchCriteriaSpecificityThreshold);
							if (response.TooManyMatches)
								throw new WeakSearchCriteriaException();
							taskContext.Complete(response.Matches ?? new List<TItem>());
						}
						catch (Exception e)
						{
							taskContext.Error(e);
						}
					},
					false) { ThreadUICulture = Desktop.Application.CurrentUICulture };

				_queryItemsTask.Terminated += OnQueryItemsCompleted;
				_queryItemsTask.Run();
			}
		}

		private void OnQueryItemsCompleted(object sender, BackgroundTaskTerminatedEventArgs args)
		{
			if (args.Reason == BackgroundTaskTerminatedReason.Completed)
			{
				NotifyItemsTableChanging();

				var items = (IList<TItem>)args.Result;
				this.TotalItemCount = items.Count;
				_itemsTable.Items.Clear();
				_itemsTable.Items.AddRange(items);
				_itemsTable.Sort();

				NotifyItemsTableChanged();
			}
			else
			{
				if (args.Exception is WeakSearchCriteriaException)
					ExceptionHandler.Report(args.Exception, this.FolderSystem.DesktopWindow);
				else
					ExceptionHandler.Report(new Exception(SR.ExceptionFailedToFindSearchResults, args.Exception), this.FolderSystem.DesktopWindow);
			}

			// dispose of the task
			_queryItemsTask.Terminated -= OnQueryItemsCompleted;
			_queryItemsTask.Dispose();
			_queryItemsTask = null;

			EndUpdate();
		}

		#endregion
	}

	public abstract class WorklistSearchResultsFolder<TItem, TWorklistService> : SearchResultsFolder<TItem, WorklistSearchParams>
		where TItem : DataContractBase
		where TWorklistService : IWorklistService<TItem>
	{
		protected WorklistSearchResultsFolder(Table<TItem> itemsTable)
			: base(itemsTable)
		{
		}

		protected override TextQueryResponse<TItem> DoQuery(WorklistSearchParams query, int specificityThreshold)
		{
			var options = WorklistItemTextQueryOptions.PatientOrder;
			if(DowntimeRecovery.InDowntimeRecoveryMode)
				options = options | WorklistItemTextQueryOptions.DowntimeRecovery;
			if (FolderSystemSettings.Default.EnablePartialMatchingOnIdentifierSearch)
				options = options | WorklistItemTextQueryOptions.EnablePartialMatchingOnIdentifiers;

			return DoQueryCore(query, specificityThreshold, options, this.ProcedureStepClassName);
		}

		protected static TextQueryResponse<TItem> DoQueryCore(WorklistSearchParams query, int specificityThreshold, WorklistItemTextQueryOptions options, string procedureStepClassName)
		{
			TextQueryResponse<TItem> response = null;

			var request = new WorklistItemTextQueryRequest(
						query.TextSearch, specificityThreshold, procedureStepClassName, options);

			if (query.UseAdvancedSearch)
			{
				request.UseAdvancedSearch = query.UseAdvancedSearch;
				request.SearchFields = query.SearchFields;
			}

			Platform.GetService<TWorklistService>(
				service => response = service.SearchWorklists(request));

			return response;
		}

		protected abstract string ProcedureStepClassName { get; }
	}
}
