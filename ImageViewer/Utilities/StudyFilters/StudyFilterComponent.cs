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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	[ExtensionPoint]
	public sealed class StudyFilterComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (StudyFilterComponentViewExtensionPoint))]
	public partial class StudyFilterComponent : ApplicationComponent, IStudyFilter
	{
		private event EventHandler _itemAdded;
		private event EventHandler _itemRemoved;
		private event EventHandler _filterPredicatesChanged;
		private event EventHandler _sortPredicatesChanged;
		private event EventHandler _isStaleChanged;
		private event EventHandler _filterPredicatesEnabledChanged;

		private readonly Table<IStudyItem> _table;
		private readonly StudyFilterColumnCollection _columns;
		private readonly StudyItemSelection _selection;
		private readonly StudyFilterSettings _settings;
		private readonly ObservableList<IStudyItem> _masterList;
		private readonly SortPredicateRoot _sortPredicate;
		private readonly FilterPredicateRoot _filterPredicate;
		private StudyFilterToolContext _toolContext;
		private ToolSet _toolset;
		private IActionSet _actions;
		private bool _isStale;
		private bool _bulkOperationsMode;

		public StudyFilterComponent()
		{
			_masterList = new ObservableList<IStudyItem>();
			_masterList.ItemAdded += OnMasterListItemAdded;
			_masterList.ItemChanged += OnMasterListItemAdded;
			_masterList.ItemChanging += OnMasterListItemRemoved;
			_masterList.ItemRemoved += OnMasterListItemRemoved;
			_masterList.EnableEvents = true;

			_selection = new StudyItemSelection(_masterList);
			_table = new Table<IStudyItem>();
			_columns = new StudyFilterColumnCollection(this);
			_settings = StudyFilterSettings.Default;
			_sortPredicate = new SortPredicateRoot(this);
			_filterPredicate = new FilterPredicateRoot(this);
		}

		public StudyFilterComponent(string path) : this()
		{
			this.Load(path, true);
			this.Refresh(true);
		}

		public StudyFilterComponent(IEnumerable<string> paths) : this()
		{
			this.Load(paths, true);
			this.Refresh(true);
		}

		#region Misc

		/// <summary>
		/// Gets or sets a value indicating whether or not the component should operate in bulk operations mode.
		/// </summary>
		/// <remarks>
		/// The component runs special code to speed up operations involving small numbers of changing items.
		/// These optimizations can be detrimental when large numbers of items are changed in one large operation.
		/// Setting the component to run in bulk operations mode will temporarily disable these optimizations.
		/// During and after bulk operations, the displayed table will only update if <see cref="Refresh()"/>
		/// is explicitly called. If <see cref="Refresh()"/> is not called after ending bulk operations, the table
		/// will remain stale.
		/// </remarks>
		public bool BulkOperationsMode
		{
			get { return _bulkOperationsMode; }
			set { _bulkOperationsMode = value; }
		}

		public Table<IStudyItem> Table
		{
			get { return _table; }
		}

		public override IActionSet ExportedActions
		{
			get { return _actions; }
		}

		public override void Start()
		{
			base.Start();

			_toolContext = new StudyFilterToolContext(this);
			_toolset = new ToolSet(new StudyFilterToolExtensionPoint(), _toolContext);
			_actions = _toolset.Actions;

			// restore columns from settings
			_columns.Deserialize(_settings.Columns);
		}

		public override void Stop()
		{
			// save columns to settings
			_settings.Columns = _columns.Serialize();

			_actions = null;
			_toolset.Dispose();
			_toolset = null;
			_toolContext = null;

			_table.Filter();

			base.Stop();

			// dispose any remaining study items now
			if (_masterList.Count > 0)
			{
				foreach (var item in _masterList)
					item.Dispose();
				_masterList.Clear();
			}
		}

		#endregion

		#region Items

		public IList<IStudyItem> Items
		{
			get { return _masterList; }
		}

		public StudyItemSelection Selection
		{
			get { return _selection; }
		}

		public event EventHandler ItemAdded
		{
			add { _itemAdded += value; }
			remove { _itemAdded -= value; }
		}

		public event EventHandler ItemRemoved
		{
			add { _itemRemoved += value; }
			remove { _itemRemoved -= value; }
		}

		private void OnMasterListItemRemoved(object sender, ListEventArgs<IStudyItem> e)
		{
			if (!_bulkOperationsMode)
			{
				// if the item passes the filter, then it's probably being shown in the table right now - remove it!
				if (_filterPredicate.Test(e.Item))
				{
					_table.Items.Remove(e.Item);

					// and now we don't have to flag the filtered table as stale
				}
			}
			else
			{
				this.IsStale = true;
			}

			EventsHelper.Fire(_itemRemoved, this, EventArgs.Empty);
		}

		private void OnMasterListItemAdded(object sender, ListEventArgs<IStudyItem> e)
		{
			if (!_bulkOperationsMode)
			{
				// if the item passes the filter, then immediately add it to the table
				if (_filterPredicate.Test(e.Item))
				{
					_sortPredicate.Insert(_table.Items, e.Item);

					// and now we don't have to flag the filtered table as stale
				}
			}
			else
			{
				this.IsStale = true;
			}

			EventsHelper.Fire(_itemAdded, this, EventArgs.Empty);
		}

		#endregion

		#region Load Methods

		public int Load(IEnumerable<string> paths)
		{
			return this.Load(paths, true);
		}

		public int Load(IEnumerable<string> paths, bool recursive)
		{
			int count = 0;
			foreach (string path in paths)
				count += this.Load(path, recursive);
			return count;
		}

		public int Load(string path)
		{
			return this.Load(path, true);
		}

		public int Load(string path, bool recursive)
		{
			return LoadCore(path, recursive);
		}

		private int LoadCore(string path, bool recursive)
		{
			int count = 0;
			try
			{
				if (File.Exists(path))
				{
					_masterList.Add(new LocalStudyItem(path));
					count++;
				}
				else if (Directory.Exists(path))
				{
					if (recursive)
					{
						foreach (string directory in Directory.GetDirectories(path))
							count += this.LoadCore(directory, true);
					}
					foreach (string filename in Directory.GetFiles(path))
						count += this.LoadCore(filename, false);
				}
			}
			catch (DicomException)
			{
				Platform.Log(LogLevel.Info, string.Format(SR.MessageDicomException, path));
			}
			catch (IOException)
			{
				Platform.Log(LogLevel.Info, string.Format(SR.MessageInvalidPath, path));
			}
			return count;
		}

		#endregion

		#region Columns

		public IStudyFilterColumnCollection Columns
		{
			get { return _columns; }
		}

		private void ColumnInserted(int index, StudyFilterColumn newColumn)
		{
			_table.Columns.Insert(index, newColumn.CreateTableColumn());
		}

		private void ColumnRemoved(int index, StudyFilterColumn oldColumn)
		{
			_table.Columns.RemoveAt(index);
		}

		private void ColumnChanged(int index, StudyFilterColumn oldColumn, StudyFilterColumn newColumn)
		{
			_table.Columns[index] = newColumn.CreateTableColumn();
		}

		private void ColumnsChanged(IEnumerable<StudyFilterColumn> newColumns)
		{
			_table.Columns.Clear();
			foreach (StudyFilterColumn column in newColumns)
				_table.Columns.Add(column.CreateTableColumn());
		}

		#endregion

		#region Tool Context Class

		private class StudyFilterToolContext : IStudyFilterToolContext
		{
			private readonly StudyFilterComponent _component;

			private event EventHandler _activeChanged;

			private IStudyItem _activeItem = null;
			private StudyFilterColumn _activeColumn = null;

			public StudyFilterToolContext(StudyFilterComponent component)
			{
				_component = component;
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public IStudyFilter StudyFilter
			{
				get { return _component; }
			}

			public IStudyItem ActiveItem
			{
				get { return _activeItem; }
			}

			public StudyFilterColumn ActiveColumn
			{
				get { return _activeColumn; }
			}

			public event EventHandler ActiveChanged
			{
				add { _activeChanged += value; }
				remove { _activeChanged -= value; }
			}

			public StudyItemSelection SelectedItems
			{
				get { return _component.Selection; }
			}

			public IList<IStudyItem> Items
			{
				get { return _component.Items; }
			}

			public IStudyFilterColumnCollection Columns
			{
				get { return _component.Columns; }
			}

			public bool BulkOperationsMode
			{
				get { return _component.BulkOperationsMode; }
				set { _component.BulkOperationsMode = value; }
			}

			public bool Load(bool allowCancel, IEnumerable<string> paths, bool recursive)
			{
				return _component.Load(this.DesktopWindow, allowCancel, paths);
			}

			public int Load(IEnumerable<string> paths, bool recursive)
			{
				return _component.Load(paths, recursive);
			}

			public void Refresh()
			{
				_component.Refresh();
			}

			public void Refresh(bool force)
			{
				_component.Refresh(force);
			}

			internal void SetActiveCell(IStudyItem item, StudyFilterColumn column)
			{
				_activeItem = item;
				_activeColumn = column;
				EventsHelper.Fire(_activeChanged, this, EventArgs.Empty);
			}
		}

		#endregion

		#region Context Menu Support

		public ActionModelNode GetContextMenuActionModel(IStudyItem item, StudyFilterColumn column)
		{
			_toolContext.SetActiveCell(item, column);
			return ActionModelRoot.CreateModel(this.GetType().FullName, StudyFilterTool.DefaultContextMenuActionSite, _actions);
		}

		#endregion

		#region Staleness/Caching Support

		public bool IsStale
		{
			get { return _isStale; }
			private set
			{
				if (_isStale != value)
				{
					_isStale = value;
					this.OnIsStaleChanged();
				}
			}
		}

		public event EventHandler IsStaleChanged
		{
			add { _isStaleChanged += value; }
			remove { _isStaleChanged -= value; }
		}

		protected virtual void OnIsStaleChanged()
		{
			EventsHelper.Fire(_isStaleChanged, this, new EventArgs());
		}

		public void Invalidate()
		{
			this.IsStale = true;
		}

		/// <summary>
		/// If the displayed data is stale, reapplies the predicates to the dataset and updates the display.
		/// </summary>
		public void Refresh()
		{
			this.Refresh(false);
		}

		/// <summary>
		/// Reapplies the predicates to the dataset and updates the display.
		/// </summary>
		/// <param name="force">A value indicating whether or not to perform the refresh even if the data is not stale.</param>
		public void Refresh(bool force)
		{
			if (force || this.IsStale)
			{
				try
				{
					_table.Items.Clear();

					IList<IStudyItem> result = _filterPredicate.Filter(_masterList);
					_sortPredicate.Sort(result);

					_table.Items.AddRange(result);

					this.IsStale = false;
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex, "An unexpected error occured evaluating the table's sort and filter predicates.");
				}
			}
		}

		#endregion

		#region Filtering Support

		public IList<FilterPredicate> FilterPredicates
		{
			get { return _filterPredicate.Predicates; }
		}

		public event EventHandler FilterPredicatesChanged
		{
			add { _filterPredicatesChanged += value; }
			remove { _filterPredicatesChanged -= value; }
		}

		public bool FilterPredicatesEnabled
		{
			get { return _filterPredicate.Enabled; }
			set { _filterPredicate.Enabled = value; }
		}

		public event EventHandler FilterPredicatesEnabledChanged
		{
			add { _filterPredicatesEnabledChanged += value; }
			remove { _filterPredicatesEnabledChanged -= value; }
		}

		protected virtual void OnFilterPredicatesEnabledChanged(EventArgs e)
		{
			EventsHelper.Fire(_filterPredicatesEnabledChanged, this, e);
		}

		private class FilterPredicateRoot
		{
			private readonly AndFilterPredicate _predicate = new AndFilterPredicate();
			private readonly StudyFilterComponent _owner;
			private bool _enabled;

			public FilterPredicateRoot(StudyFilterComponent owner)
			{
				_enabled = true;
				_owner = owner;
				_predicate.Changed += Predicates_Changed;
			}

			public IList<FilterPredicate> Predicates
			{
				get { return _predicate.Predicates; }
			}

			public bool Enabled
			{
				get { return _enabled; }
				set
				{
					if (_enabled != value)
					{
						_enabled = value;
						if (_predicate.Predicates.Count > 0)
							_owner.IsStale = true;
						_owner.OnFilterPredicatesEnabledChanged(EventArgs.Empty);
					}
				}
			}

			/// <summary>
			/// Filters a list. O{n}
			/// </summary>
			public IList<IStudyItem> Filter(IList<IStudyItem> list)
			{
				IList<IStudyItem> filtered = new List<IStudyItem>();
				if (_enabled)
				{
					foreach (IStudyItem item in list)
					{
						if (_predicate.Evaluate(item))
							filtered.Add(item);
					}
				}
				else
				{
					((List<IStudyItem>) filtered).AddRange(list);
				}
				return filtered;
			}

			/// <summary>
			/// Tests the filter on the specified item. O{1}
			/// </summary>
			public bool Test(IStudyItem item)
			{
				return !_enabled || _predicate.Evaluate(item);
			}

			private void Predicates_Changed(object sender, EventArgs e)
			{
				_owner.IsStale = _enabled;
				EventsHelper.Fire(_owner._filterPredicatesChanged, _owner, EventArgs.Empty);
			}
		}

		#endregion

		#region Sorting Support

		public IList<SortPredicate> SortPredicates
		{
			get { return _sortPredicate.Predicates; }
		}

		public event EventHandler SortPredicatesChanged
		{
			add { _sortPredicatesChanged += value; }
			remove { _sortPredicatesChanged -= value; }
		}

		private class SortPredicateRoot : IComparer<IStudyItem>
		{
			private readonly ObservableList<SortPredicate> _predicates = new ObservableList<SortPredicate>();
			private readonly StudyFilterComponent _owner;

			public SortPredicateRoot(StudyFilterComponent owner)
			{
				_owner = owner;
				_predicates.ItemAdded += Predicates_Changed;
				_predicates.ItemChanged += Predicates_Changed;
				_predicates.ItemRemoved += Predicates_Changed;
				_predicates.EnableEvents = true;
			}

			public IList<SortPredicate> Predicates
			{
				get { return _predicates; }
			}

			/// <summary>
			/// Sorts a list in place. O{n*log(n)}
			/// </summary>
			public void Sort(IList<IStudyItem> list)
			{
				MergeSort(this, list, 0, list.Count);
			}

			/// <summary>
			/// Inserts the specified item into a list. O{log(n)}
			/// </summary>
			public void Insert(IList<IStudyItem> list, IStudyItem item)
			{
				list.Insert(BinarySearch(this, list, item, 0, list.Count), item);
			}

			private void Predicates_Changed(object sender, ListEventArgs<SortPredicate> e)
			{
				_owner.IsStale = true;
				EventsHelper.Fire(_owner._sortPredicatesChanged, _owner, EventArgs.Empty);
			}

			int IComparer<IStudyItem>.Compare(IStudyItem x, IStudyItem y)
			{
				if (x == y)
					return 0;
				if (x == null)
					return 1;
				if (y == null)
					return -1;

				foreach (SortPredicate predicate in this.Predicates)
				{
					int result = predicate.Compare(x, y);
					if (result != 0)
						return result;
				}
				return 0;
			}

			#region Stable Sort Implementation

			/// <summary>
			/// Performs a stable merge sort on the given <paramref name="list"/> using the given <paramref name="comparer"/>.
			/// The range of items sorted is [<paramref name="rangeStart"/>, <paramref name="rangeStop"/>).
			/// </summary>
			private static void MergeSort(IComparer<IStudyItem> comparer, IList<IStudyItem> list, int rangeStart, int rangeStop)
			{
				int rangeLength = rangeStop - rangeStart;
				if (rangeLength > 1)
				{
					// sort halves
					int rangeMid = rangeStart + rangeLength/2;
					MergeSort(comparer, list, rangeStart, rangeMid);
					MergeSort(comparer, list, rangeMid, rangeStop);

					// merge halves
					List<IStudyItem> merged = new List<IStudyItem>(rangeLength);
					int j = rangeStart;
					int k = rangeMid;

					for (int n = 0; n < rangeLength; n++)
					{
						// if left half has run out of items, add item from right half
						// else if right half has run out of items, add item from left half
						// else if the left item is before or equal to the right item, add left half item
						// else add right half item
						if (k >= rangeStop || (j < rangeMid && comparer.Compare(list[j], list[k]) <= 0))
							merged.Add(list[j++]);
						else
							merged.Add(list[k++]);
					}

					// copy merged to list
					k = rangeStart;
					foreach (IStudyItem item in merged)
						list[k++] = item;
				}
			}

			#endregion

			#region Search Implementation

			/// <summary>
			/// Performs a binary search on the given <paramref name="list"/> using the given <paramref name="comparer"/>
			/// for the expected index of <paramref name="item"/>.
			/// The range of items searched is [<paramref name="rangeStart"/>, <paramref name="rangeStop"/>).
			/// </summary>
			private static int BinarySearch(IComparer<IStudyItem> comparer, IList<IStudyItem> list, IStudyItem item, int rangeStart, int rangeStop)
			{
				int rangeLength = rangeStop - rangeStart;

				int result;
				if (rangeLength == 1)
				{
					if (comparer.Compare(item, list[rangeStart]) >= 0)
						result = rangeStop;
					else
						result = rangeStart;
				}
				else if (rangeLength == 0)
				{
					result = rangeStop;
				}
				else
				{
					rangeLength = rangeStart + rangeLength/2;
					if (comparer.Compare(item, list[rangeLength]) >= 0)
						result = BinarySearch(comparer, list, item, rangeLength, rangeStop);
					else
						result = BinarySearch(comparer, list, item, rangeStart, rangeLength);
				}
				return result;
			}

			#endregion
		}

		#endregion
	}
}