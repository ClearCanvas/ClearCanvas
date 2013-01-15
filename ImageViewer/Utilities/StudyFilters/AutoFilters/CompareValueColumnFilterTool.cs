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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters
{
	[ExtensionOf(typeof (AutoFilterToolExtensionPoint))]
	public class CompareValueColumnFilterTool : AutoFilterTool, IFilterMenuActionOwner
	{
		private IActionSet _actions;

		protected override bool IsColumnSupported()
		{
			return typeof (IComparable).IsAssignableFrom(base.Column.GetValueType());
		}

		public CompositeFilterPredicate ParentFilterPredicate
		{
			get
			{
				CompareValueCompositePredicateFilter filter = CompareValueCompositePredicateFilter.Find(base.AutoFilterRoot.Predicates, base.Column);
				if (filter == null)
					base.AutoFilterRoot.Predicates.Add(filter = new CompareValueCompositePredicateFilter(base.Column));
				return filter;
			}
		}

		protected override void Dispose(bool disposing)
		{
			CompareValueCompositePredicateFilter filter = CompareValueCompositePredicateFilter.Find(base.AutoFilterRoot.Predicates, base.Column);
			if (filter != null)
				base.AutoFilterRoot.Predicates.Remove(filter);

			base.Dispose(disposing);
		}

		public override IActionSet Actions
		{
			get
			{
				if (_actions == null)
				{
					List<IAction> list = new List<IAction>();

					if (this.IsColumnSupported())
					{
						ResourceResolver resourceResolver = new ApplicationThemeResourceResolver(this.GetType(), false);

						CompareFilterMenuAction equalsAction = CompareFilterMenuAction.CreateAction(
							this.GetType(), "equals",
							"studyfilters-columnfilters/MenuValueFilters/MenuEquals",
							this, new CompareFilterMode[] {CompareFilterMode.Equals, CompareFilterMode.NotEquals},
							resourceResolver);
						list.Add(equalsAction);

						CompareFilterMenuAction upperBoundAction = CompareFilterMenuAction.CreateAction(
							this.GetType(), "upperBound",
							"studyfilters-columnfilters/MenuValueFilters/MenuUpperBound",
							this, new CompareFilterMode[] {CompareFilterMode.LessThan, CompareFilterMode.LessThenOrEquals},
							resourceResolver);
						list.Add(upperBoundAction);

						CompareFilterMenuAction lowerBoundAction = CompareFilterMenuAction.CreateAction(
							this.GetType(), "lowerBound",
							"studyfilters-columnfilters/MenuValueFilters/MenuLowerBound",
							this, new CompareFilterMode[] {CompareFilterMode.GreaterThan, CompareFilterMode.GreaterThanOrEquals},
							resourceResolver);
						list.Add(lowerBoundAction);
					}

					_actions = base.Actions.Union(new ActionSet(list));
				}
				return _actions;
			}
		}

		private class CompareValueCompositePredicateFilter : CompositeFilterPredicate
		{
			private readonly StudyFilterColumn _column;

			public CompareValueCompositePredicateFilter(StudyFilterColumn column)
				: base()
			{
				_column = column;
			}

			public override bool Evaluate(IStudyItem item)
			{
				foreach (FilterPredicate predicate in base.Predicates)
				{
					if (!predicate.Evaluate(item))
						return false;
				}
				return true;
			}

			public override bool IsActive
			{
				get
				{
					foreach (CompareFilterMenuAction.Predicate predicate in base.Predicates)
						if (predicate.IsActive)
							return true;

					return false;
				}
			}

			public static CompareValueCompositePredicateFilter Find(IEnumerable<FilterPredicate> collection, StudyFilterColumn column)
			{
				return (CompareValueCompositePredicateFilter)
				       CollectionUtils.SelectFirst(collection,
				                                   delegate(FilterPredicate x) { return x is CompareValueCompositePredicateFilter && ((CompareValueCompositePredicateFilter) x)._column == column; });
			}
		}
	}
}