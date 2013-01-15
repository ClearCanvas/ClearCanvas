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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	#region FolderForWorklistClassAttribute

	/// <summary>
	/// Associates a folder class with a worklist class.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class FolderForWorklistClassAttribute : Attribute
	{
		private readonly string _worklistClassName;

		public FolderForWorklistClassAttribute(string worklistClassName)
		{
			_worklistClassName = worklistClassName;
		}

		public string WorklistClassName
		{
			get { return _worklistClassName; }
		}
	}

	#endregion


	/// <summary>
	/// Abstract base class for workflow folders.  A workflow folder is characterized by the fact
	/// that it contains "work items".
	/// </summary>
	public abstract class WorkflowFolder : Folder
	{
		private bool _isCountValid;
		private bool _isItemsValid;

		private static readonly IconSet _closedRefreshingIconSet = new IconSet("FolderClosedRefreshMedium.gif");
		private static readonly IconSet _openRefreshingIconSet = new IconSet("FolderOpenRefreshMedium.gif");



		#region Folder overrides

		protected override IconSet ClosedIconSet
		{
			get { return IsUpdating ? _closedRefreshingIconSet : base.ClosedIconSet; }
		}

		protected override IconSet OpenIconSet
		{
			get { return IsUpdating ? _openRefreshingIconSet : base.OpenIconSet; }
		}

		protected override void InvalidateCore()
		{
			_isCountValid = false;
			_isItemsValid = false;
		}

		protected void MarkItemsValid()
		{
			_isItemsValid = true;
			_isCountValid = true;
		}

		protected void MarkCountValid()
		{
			_isCountValid = true;
		}

		protected override bool UpdateCore()
		{
			if(this.IsOpen)
			{
				// if the folder is open, the actual contents need to be valid
				if (!_isItemsValid)
				{
					// an items query validates the count as well as the items
					BeginQueryItems();
					return true;
				}
			}
			else
			{
				// otherwise, only the count needs to be valid
				if (!_isCountValid)
				{
					BeginQueryCount();
					return true;
				}
			}

			return false;	// nothing updated
		}

		#endregion

		#region Paging overrides

		public override bool SupportsPaging
		{
			get { return true; }
		}

		public override int PageSize
		{
			get { return FolderSystemSettings.Default.ItemsPerPage; }
		}

		#endregion

		#region Protected API

		protected abstract void BeginQueryItems();

		protected abstract void BeginQueryCount();

		#endregion
	}

	/// <summary>
	/// Abstract base class for folders that display the contents of worklists.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public abstract class WorkflowFolder<TItem> : WorkflowFolder
	{
		#region QueryItemsResult class

		protected class QueryItemsResult
		{
			private readonly IList _items;
			private readonly int _totalItemCount;

			public QueryItemsResult(IList items, int totalItemCount)
			{
				_items = items;
				_totalItemCount = totalItemCount;
			}

			public IList Items
			{
				get { return _items; }
			}

			public int TotalItemCount
			{
				get { return _totalItemCount; }
			}
		}

		#endregion

		private readonly Table<TItem> _itemsTable;
		private IDropHandler<TItem> _currentDropHandler;

		private readonly AsyncTask _queryItemsTask;
		private readonly AsyncTask _queryCountTask;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="itemsTable"></param>
		protected WorkflowFolder(Table<TItem> itemsTable)
		{
			_itemsTable = itemsTable;
			_queryItemsTask = new AsyncTask();
			_queryCountTask = new AsyncTask();
		}

		#region Folder overrides

		/// <summary>
		/// Gets a table of the items that are contained in this folder
		/// </summary>
		public override ITable ItemsTable
		{
			get { return _itemsTable; }
		}

		protected override void InvalidateCore()
		{
			base.InvalidateCore();

			// Stops item querying task when the folder becomes invalidated.
			_queryItemsTask.Cancel();
			_queryCountTask.Cancel();
		}

		/// <summary>
		/// Informs the folder that the specified items were dragged from it.  It is up to the implementation
		/// of the folder to determine the appropriate response (e.g. whether the items should be removed or not).
		/// </summary>
		/// <param name="items"></param>
		/// <param name="kind"></param>
		public override void DragComplete(object[] items, DragDropKind kind)
		{
			if (kind == DragDropKind.Move)
			{
				// items have been "moved" out of this folder
			}
		}

		/// <summary>
		/// Asks the folder if it can accept a drop of the specified items
		/// </summary>
		/// <param name="items"></param>
		/// <param name="kind"></param>
		/// <returns></returns>
		public override DragDropKind CanAcceptDrop(object[] items, DragDropKind kind)
		{
			// this cast is not terribly safe, but in practice it should always succeed
			var fs = (WorkflowFolderSystem) this.FolderSystem;
			_currentDropHandler = (IDropHandler<TItem>)fs.GetDropHandler(this, items);

			// if the items are acceptable, return Move (never Copy, which would make no sense for a workflow folder)
			return _currentDropHandler != null ? DragDropKind.Move : DragDropKind.None;
		}

		/// <summary>
		/// Instructs the folder to accept the specified items
		/// </summary>
		/// <param name="items"></param>
		/// <param name="kind"></param>
		public override DragDropKind AcceptDrop(object[] items, DragDropKind kind)
		{
			if (_currentDropHandler == null)
				return DragDropKind.None;

			// cast items to type safe collection
			var dropItems = CollectionUtils.Map(items, (object item) => (TItem) item);
			return _currentDropHandler.ProcessDrop(dropItems) ? DragDropKind.Move : DragDropKind.None;
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Asks the folder to refresh its contents starting from the first page.  The implementation may be asynchronous.
		/// </summary>
		protected override void BeginQueryItems()
		{
			// notify the framework that an update is beginning
			BeginUpdate();

			QueryItemsResult result = null;
			_queryItemsTask.Run(
				delegate
				{
					var currentPageRowNumber = this.PageNumber * this.PageSize;
					result = QueryItems(currentPageRowNumber, this.PageSize);
				},
				delegate
				{
					NotifyItemsTableChanging();

					var items = CollectionUtils.Map<object, TItem>(result.Items, item => (TItem)item);
					_itemsTable.Items.Clear();
					_itemsTable.Items.AddRange(items);
					_itemsTable.Sort();
					InErrorState = false;

					NotifyItemsTableChanged();

					this.TotalItemCount = result.TotalItemCount;
					MarkItemsValid();

					EndUpdate();
					NotifyIconChanged();
				},
				delegate(Exception e)
				{
					// since this was running in the background, we can't report the exception to the user
					// because they would have no context for it, and it would be annoying
					// therefore, just log it
					InErrorState = true;
					Platform.Log(LogLevel.Error, e);

					EndUpdate();
					NotifyIconChanged();
				});

			NotifyIconChanged();
		}

		/// <summary>
		/// Asks the folder to refresh the count of its contents, without actually refreshing the contents.
		/// The implementation may be asynchronous.
		/// </summary>
		protected override void BeginQueryCount()
		{
			// notify the framework that an update is beginning
			BeginUpdate();

			var count = -1;
			_queryCountTask.Run(
				delegate
					{
						count = QueryCount();
					},
				delegate
					{
						InErrorState = false;
						this.TotalItemCount = count;
						MarkCountValid();

						EndUpdate();
						NotifyIconChanged();
					},
				delegate(Exception e)
					{
						// since this was running in the background, we can't report the exception to the user
						// because they would have no context for it, and it would be annoying
						// therefore, just log it
						InErrorState = true;
						Platform.Log(LogLevel.Error, e);

						EndUpdate();
						NotifyIconChanged();
					});

			NotifyIconChanged();
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Called to obtain the set of items in the folder.
		/// </summary>
		/// <returns></returns>
		protected abstract QueryItemsResult QueryItems(int firstRow, int maxRows);

		/// <summary>
		/// Called to obtain a count of the logical total number of items in the folder (which may be more than the number in memory).
		/// </summary>
		/// <returns></returns>
		protected abstract int QueryCount();

		#endregion

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_queryItemsTask != null)
					_queryItemsTask.Dispose();

				if (_queryCountTask != null)
					_queryCountTask.Dispose();
			}

			base.Dispose(disposing);
		}
	}
}
