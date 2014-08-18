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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	public enum WorklistItemCompletedResult
	{
		Completed,
		Skipped,
		Invalid
	}

	public interface IContinuousWorkflowComponentMode
	{
		bool ShouldUnclaim { get; }
		bool ShowStatusText { get; }
		bool CanContinue { get; }
	}

	public abstract class ContinuousWorkflowComponentMode : IContinuousWorkflowComponentMode
	{
		private readonly bool _shouldUnclaim;
		private readonly bool _showStatusText;
		private readonly bool _canContinue;

		protected ContinuousWorkflowComponentMode(bool shouldUnclaim, bool showStatusText, bool canContinue)
		{
			_shouldUnclaim = shouldUnclaim;
			_showStatusText = showStatusText;
			_canContinue = canContinue;
		}

		#region IContinuousWorkflowComponentMode Members

		public bool ShouldUnclaim
		{
			get { return _shouldUnclaim; }
		}

		public bool ShowStatusText
		{
			get { return _showStatusText; }
		}

		public bool CanContinue
		{
			get { return _canContinue; }
		}

		#endregion
	}

	public interface IWorklistItemManager<TWorklistItem>
	{
		/// <summary>
		/// The current <see cref="TWorklistItem"/>
		/// </summary>
		TWorklistItem WorklistItem { get; }

		/// <summary>
		/// Used to request the next <see cref="TWorklistItem"/> to be loaded.
		/// </summary>
		/// <param name="result">Indicates whether previous item was completed or skipped.</param>
		void ProceedToNextWorklistItem(WorklistItemCompletedResult result);

		/// <summary>
		/// Used to request the next <see cref="TWorklistItem"/> to be loaded.
		/// </summary>
		/// <param name="result">Indicates whether previous item was completed or skipped.</param>
		/// <param name="overrideDoNotPerformNextItem">Override the default behaviour.  Complete the current item and do not proceed to next item.</param>
		void ProceedToNextWorklistItem(WorklistItemCompletedResult result, bool overrideDoNotPerformNextItem);

		/// <summary>
		/// Specify a list of <see cref="TWorklistItem"/> that should be excluded from <see mref="ProceedToNextWorklistItem"/>
		/// </summary>
		/// <param name="worklistItems"></param>
		void IgnoreWorklistItems(List<TWorklistItem> worklistItems);

		/// <summary>
		/// Fired when the next worklist item is available.
		/// </summary>
		event EventHandler WorklistItemChanged;

		bool ShouldUnclaim { get; }

		/// <summary>
		/// A string indicating the name of the source folder system and counts of available, completed and skipped items.
		/// </summary>
		string StatusText { get; }

		bool StatusTextVisible { get; }

		/// <summary>
		/// Specifies if the next <see cref="TWorklistItem"/> should be reported
		/// </summary>
		bool ReportNextItem { get; set; }

		/// <summary>
		/// 
		/// </summary>
		bool ReportNextItemEnabled { get; }

		/// <summary>
		/// Specifies if a "Skip" button should be enabled based on mode and value of <see cref="ReportNextItem"/>
		/// </summary>
		bool CanSkipItem { get; }
	}

	public abstract class WorklistItemManager<TWorklistItem, TWorkflowService> : IWorklistItemManager<TWorklistItem>
		where TWorklistItem : WorklistItemSummaryBase
		where TWorkflowService : IWorklistService<TWorklistItem>
	{
		#region Private fields

		private TWorklistItem _worklistItem;
		private event EventHandler _worklistItemChanged;

		private IContinuousWorkflowComponentMode _componentMode;
		private readonly string _folderName;
		private readonly EntityRef _worklistRef;
		private readonly string _worklistClassName;
		private int _allAvailableItemsCount;
		private int _completedItemsCount;
		private int _skippedItemsCount;
		private bool _isInitialItem = true;

		private readonly List<TWorklistItem> _visitedItems;
		private readonly Queue<TWorklistItem> _worklistLocalItemQueue;
		private readonly int _worklistItemLocalQueueSize;

		private bool _reportNextItem;

		private bool _isInitialized;

		#endregion

		protected abstract IContinuousWorkflowComponentMode GetMode(TWorklistItem worklistItem);
		protected abstract string TaskName { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>
		/// Only one of worklistRef or worklistClassName should be specified.  worklistRef will take precedence if both are provided.
		/// </remarks>
		/// <param name="folderName">Folder system name, displayed in status text</param>
		/// <param name="worklistRef">An <see cref="EntityRef"/> for the folder from which additional worklist items should be loaded.</param>
		/// <param name="worklistClassName">A name for the folder class from which additional worklist items should be loaded.</param>
		/// <param name="worklistItemLocalQueueSize">
		/// Number of worklist items to queue locally.  The higher this number is, the fewer trips
		/// to the server will need to be made.  The tradeoff however is that the client will not be aware of higher priority items that may
		/// have appeared on the server since the local queue was populated, until the local queue is refreshed.
		/// </param>
		protected WorklistItemManager(string folderName, EntityRef worklistRef, string worklistClassName, int worklistItemLocalQueueSize)
		{
			_folderName = folderName;
			_worklistRef = worklistRef;
			_worklistClassName = worklistClassName;

			_visitedItems = new List<TWorklistItem>();
			_worklistLocalItemQueue = new Queue<TWorklistItem>();
			_worklistItemLocalQueueSize = worklistItemLocalQueueSize;
		}

		public void Initialize(TWorklistItem worklistItem)
		{
			this.Initialize(worklistItem, GetMode(worklistItem));
		}

		public void Initialize(TWorklistItem worklistItem, IContinuousWorkflowComponentMode mode)
		{
			_worklistItem = worklistItem;
			_componentMode = mode;
			_reportNextItem = WorklistItemManagerSettings.Default.ShouldProceedToNextItem
				&& this.ReportNextItemEnabled;

			_isInitialized = true;
		}

		public TWorklistItem WorklistItem
		{
			get
			{
				if (!_isInitialized)
					throw new Exception("Not initialized.");

				return _worklistItem;
			}
		}

		// This function is used for swapping primary reporting step with a linkable step before any of the UI components are updated.
		// Use of this function in other places may have unintended effect.
		public void SwapCurentItem(TWorklistItem newCurrentItem)
		{
			if (Equals(_worklistItem, newCurrentItem))
				return;

			CollectionUtils.Remove(_visitedItems as IList, item => Equals(item, newCurrentItem));
			_worklistLocalItemQueue.Enqueue(_worklistItem);
			_worklistItem = newCurrentItem;
		}

		public void ProceedToNextWorklistItem(WorklistItemCompletedResult result)
		{
			ProceedToNextWorklistItem(result, false);
		}

		public void ProceedToNextWorklistItem(WorklistItemCompletedResult result, bool overrideDoNotPerformNextItem)
		{
			if (result == WorklistItemCompletedResult.Completed)
			{
				_completedItemsCount++;
				_visitedItems.Add(_worklistItem);
			}
			else if (result == WorklistItemCompletedResult.Skipped)
			{
				_skippedItemsCount++;
				_visitedItems.Add(_worklistItem);
			}

			_isInitialItem = false;

			if (_reportNextItem && _componentMode.CanContinue && overrideDoNotPerformNextItem == false)
			{
				if (_worklistLocalItemQueue.Count == 0)
				{
					RefreshWorklistItemLocalQueue();
				}

				_worklistItem = _worklistLocalItemQueue.Count > 0 ? _worklistLocalItemQueue.Dequeue() : null;
				_allAvailableItemsCount--;
			}
			else
			{
				_worklistItem = null;
			}

			EventsHelper.Fire(_worklistItemChanged, this, EventArgs.Empty);
		}

		public void IgnoreWorklistItems(List<TWorklistItem> interpretations)
		{
			if (interpretations.Count == 0)
				return;

			_visitedItems.AddRange(interpretations);
			RefreshWorklistItemLocalQueue();
		}

		public event EventHandler WorklistItemChanged
		{
			add { _worklistItemChanged += value; }
			remove { _worklistItemChanged -= value; }
		}

		public bool ShouldUnclaim
		{
			get { return _componentMode.ShouldUnclaim; }
		}

		#region Presentation Model

		public string StatusText
		{
			get
			{
				var status = string.Format(SR.FormatContinuousWorkflowDescription, this.TaskName, _folderName);
				if (!_isInitialItem)
				{
					status = status + string.Format(SR.FormatReportingStatusText, _allAvailableItemsCount, _completedItemsCount, _skippedItemsCount);
				}

				return status;
			}
		}

		public bool StatusTextVisible
		{
			get { return _componentMode.ShowStatusText && this.HasValidWorklistContext; }
		}

		public bool ReportNextItem
		{
			get { return _reportNextItem; }
			set
			{
				_reportNextItem = value;
				WorklistItemManagerSettings.Default.ShouldProceedToNextItem = value;
				WorklistItemManagerSettings.Default.Save();
			}
		}

		public bool ReportNextItemEnabled
		{
			get { return _componentMode.CanContinue && this.HasValidWorklistContext; }
		}

		public bool CanSkipItem
		{
			get { return _reportNextItem && this.ReportNextItemEnabled; }
		}

		#endregion

		#region Private methods

		private bool HasValidWorklistContext
		{
			get { return _worklistRef != null || _worklistClassName != null; }
		}

		private void RefreshWorklistItemLocalQueue()
		{
			if (!this.HasValidWorklistContext)
				return;

			// refresh the count of available items
			_allAvailableItemsCount = GetAvailableItemCount();

			// populate the queue
			_worklistLocalItemQueue.Clear();
			foreach (var item in GetAvailableItemStream())
			{
				if (WorklistItemWasPreviouslyVisited(item) == false)
				{
					_worklistLocalItemQueue.Enqueue(item);
				}
				else
				{
					// If any excluded items are still in the worklist, don't include them in the available items count
					_allAvailableItemsCount--;
				}

				// stop when we've reached the desired amount
				if (_worklistLocalItemQueue.Count == _worklistItemLocalQueueSize)
					break;
			}
		}

		private IEnumerable<TWorklistItem> GetAvailableItemStream()
		{
			var workingFacilityRef = LoginSession.Current.WorkingFacility.FacilityRef;
			var request = _worklistRef != null
				? new QueryWorklistRequest(_worklistRef, true, false, DowntimeRecovery.InDowntimeRecoveryMode, workingFacilityRef)
				: new QueryWorklistRequest(_worklistClassName, true, false, DowntimeRecovery.InDowntimeRecoveryMode, workingFacilityRef);

			const int pageSize = 25;
			var more = true;
			QueryWorklistResponse<TWorklistItem> response = null;
			for (var p = 0; more; p += pageSize)
			{
				request.Page = new SearchResultPage(p, p + pageSize);
				Platform.GetService<TWorkflowService>(service => response = service.QueryWorklist(request));

				more = response.WorklistItems.Count > 0;

				foreach (var item in response.WorklistItems)
				{
					yield return item;
				}
			}
		}

		private int GetAvailableItemCount()
		{
			var workingFacilityRef = LoginSession.Current.WorkingFacility.FacilityRef;
			var request = _worklistRef != null
				? new QueryWorklistRequest(_worklistRef, false, true, DowntimeRecovery.InDowntimeRecoveryMode, workingFacilityRef)
				: new QueryWorklistRequest(_worklistClassName, false, true, DowntimeRecovery.InDowntimeRecoveryMode, workingFacilityRef);

			QueryWorklistResponse<TWorklistItem> response = null;
			Platform.GetService<TWorkflowService>(service => response = service.QueryWorklist(request));
			return response.ItemCount;
		}

		private bool WorklistItemWasPreviouslyVisited(TWorklistItem item)
		{
			return CollectionUtils.Contains(_visitedItems, skippedItem => skippedItem.ProcedureRef.Equals(item.ProcedureRef, true));
		}

		#endregion
	}
}