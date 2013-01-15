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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters
{
	[ExtensionOf(typeof (AutoFilterToolExtensionPoint))]
	public class QuickValueColumnFilterTool : AutoFilterTool, IListFilterDataSource
	{
		protected override bool IsColumnSupported()
		{
			return base.Column is IValueIndexedColumn;
		}

		private QuickValuePredicateFilter QuickValueFilter
		{
			get
			{
				QuickValuePredicateFilter filter = QuickValuePredicateFilter.Find(base.AutoFilterRoot.Predicates, base.Column);
				if (filter == null)
				{
					base.AutoFilterRoot.Predicates.Add(filter = new QuickValuePredicateFilter(this.Column));
					foreach (object value in ((IValueIndexedColumn) base.Column).UniqueValues)
						filter.InstallFilter(value);
				}
				return filter;
			}
		}

		private void ClearQuickValueFilter()
		{
			QuickValuePredicateFilter filter = QuickValuePredicateFilter.Find(base.AutoFilterRoot.Predicates, base.Column);
			if (filter != null)
				this.AutoFilterRoot.Predicates.Remove(filter);
		}

		public override IActionSet Actions
		{
			get
			{
				List<IAction> list = new List<IAction>();

				if (this.IsColumnSupported())
				{
					ResourceResolver resourceResolver = new ApplicationThemeResourceResolver(this.GetType(), false);
					ListFilterMenuAction action = ListFilterMenuAction.CreateAction(
						this.GetType(), "selectValue",
						"studyfilters-columnfilters/MenuExactValue",
						this, resourceResolver);
					list.Add(action);
				}

				return base.Actions.Union(new ActionSet(list));
			}
		}

		protected override void Dispose(bool disposing)
		{
			ClearQuickValueFilter();

			base.Dispose(disposing);
		}

		#region IListFilterDataSource Members

		IEnumerable<object> IListFilterDataSource.Values
		{
			get
			{
				foreach (object value in ((IValueIndexedColumn) base.Column).UniqueValues)
					yield return value;
			}
		}

		bool IListFilterDataSource.GetSelectedState(object value)
		{
			if (QuickValuePredicateFilter.Find(base.AutoFilterRoot.Predicates, base.Column) == null)
				return true;

			ValueFilterPredicate result;
			return this.QuickValueFilter.FindFilter(value, out result);
		}

		void IListFilterDataSource.SetSelectedState(object value, bool selected)
		{
			if (selected)
				this.QuickValueFilter.InstallFilter(value);
			else
				this.QuickValueFilter.UninstallFilter(value);
		}

		void IListFilterDataSource.SetAllSelectedState(bool selected)
		{
			if (selected)
				this.ClearQuickValueFilter();
			else
				this.QuickValueFilter.ClearFilters();
		}

		#endregion

		private class QuickValuePredicateFilter : FilterPredicate
		{
			private readonly List<ValueFilterPredicate> _predicates = new List<ValueFilterPredicate>();
			private readonly StudyFilterColumn _column;

			public QuickValuePredicateFilter(StudyFilterColumn column) : base()
			{
				_column = column;
			}

			public bool FindFilter(object value, out ValueFilterPredicate filterPredicate)
			{
				foreach (ValueFilterPredicate predicate in _predicates)
				{
					if ((predicate.Value == null && value == null) || (predicate.Value != null && predicate.Value.Equals(value)))
					{
						filterPredicate = predicate;
						return true;
					}
				}
				filterPredicate = null;
				return false;
			}

			public void ClearFilters()
			{
				_predicates.Clear();
				base.OnChanged();
			}

			public void InstallFilter(object value)
			{
				ValueFilterPredicate existingPredicate;
				if (!this.FindFilter(value, out existingPredicate))
				{
					_predicates.Add(new TextValueFilterPredicate(_column, value));
					base.OnChanged();
				}
			}

			public void UninstallFilter(object value)
			{
				ValueFilterPredicate existingPredicate;
				if (this.FindFilter(value, out existingPredicate))
				{
					_predicates.Remove(existingPredicate);
					base.OnChanged();
				}
			}

			public override bool Evaluate(IStudyItem item)
			{
				foreach (ValueFilterPredicate predicate in _predicates)
				{
					if (predicate.Evaluate(item))
						return true;
				}
				return false;
			}

			public static QuickValuePredicateFilter Find(IEnumerable<FilterPredicate> collection, StudyFilterColumn column)
			{
				return (QuickValuePredicateFilter)
				       CollectionUtils.SelectFirst(collection,
				                                   delegate(FilterPredicate x) { return x is QuickValuePredicateFilter && ((QuickValuePredicateFilter) x)._column == column; });
			}

			private class TextValueFilterPredicate : ValueFilterPredicate
			{
				public TextValueFilterPredicate(StudyFilterColumn column, object value) : base(column, value) {}

				public override bool Evaluate(IStudyItem item)
				{
					return this.Column.GetText(item).Equals(this.Value);
				}
			}
		}
	}
}