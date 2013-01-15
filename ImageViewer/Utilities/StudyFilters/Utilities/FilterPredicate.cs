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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities
{
	public abstract class FilterPredicate
	{
		private event EventHandler _changed;

		protected FilterPredicate() {}

		public event EventHandler Changed
		{
			add { _changed += value; }
			remove { _changed -= value; }
		}

		public abstract bool Evaluate(IStudyItem item);

		protected virtual void OnChanged()
		{
			EventsHelper.Fire(_changed, this, EventArgs.Empty);
		}
	}

	public sealed class NotFilterPredicate : FilterPredicate
	{
		private FilterPredicate _predicate;

		public NotFilterPredicate() : base() {}

		public NotFilterPredicate(FilterPredicate predicate)
			: base()
		{
			_predicate = predicate;
		}

		public FilterPredicate Predicate
		{
			get { return _predicate; }
			set { _predicate = value; }
		}

		public override bool Evaluate(IStudyItem item)
		{
			if (_predicate == null)
				return true;
			return !_predicate.Evaluate(item);
		}
	}

	public abstract class CompositeFilterPredicate : FilterPredicate
	{
		private readonly ObservableList<FilterPredicate> _predicates;

		protected CompositeFilterPredicate()
			: base()
		{
			_predicates = new ObservableList<FilterPredicate>();
			_predicates.ItemAdded += Predicates_Added;
			_predicates.ItemChanged += Predicates_Changed;
			_predicates.ItemChanging += Predicates_Changing;
			_predicates.ItemRemoved += Predicates_Removed;
			_predicates.EnableEvents = true;
		}

		protected CompositeFilterPredicate(IEnumerable<FilterPredicate> predicates)
			: base()
		{
			_predicates = new ObservableList<FilterPredicate>(predicates);
		}

		public IList<FilterPredicate> Predicates
		{
			get { return _predicates; }
		}

		public virtual bool IsActive
		{
			get
			{
				foreach (FilterPredicate predicate in _predicates)
				{
					if (predicate is CompositeFilterPredicate && !((CompositeFilterPredicate)predicate).IsActive)
						continue;
					return true;
				}
				return false;
			}
		}

		private void Predicates_Added(object sender, ListEventArgs<FilterPredicate> e)
		{
			e.Item.Changed += FilterPredicate_Changed;
			base.OnChanged();
		}

		private void Predicates_Changing(object sender, ListEventArgs<FilterPredicate> e)
		{
			e.Item.Changed -= FilterPredicate_Changed;
		}

		private void Predicates_Changed(object sender, ListEventArgs<FilterPredicate> e)
		{
			e.Item.Changed += FilterPredicate_Changed;
			base.OnChanged();
		}

		private void Predicates_Removed(object sender, ListEventArgs<FilterPredicate> e)
		{
			e.Item.Changed -= FilterPredicate_Changed;
			base.OnChanged();
		}

		private void FilterPredicate_Changed(object sender, EventArgs e)
		{
			base.OnChanged();
		}
	}

	public sealed class AndFilterPredicate : CompositeFilterPredicate
	{
		public AndFilterPredicate() : base() {}

		public AndFilterPredicate(IEnumerable<FilterPredicate> predicates) : base(predicates) {}

		public override bool Evaluate(IStudyItem item)
		{
			foreach (FilterPredicate predicate in this.Predicates)
			{
				if (!predicate.Evaluate(item))
					return false;
			}
			return true;
		}
	}

	public sealed class OrFilterPredicate : CompositeFilterPredicate
	{
		public OrFilterPredicate() : base() {}

		public OrFilterPredicate(IEnumerable<FilterPredicate> predicates) : base(predicates) {}

		public override bool Evaluate(IStudyItem item)
		{
			foreach (FilterPredicate predicate in this.Predicates)
			{
				if (predicate.Evaluate(item))
					return true;
			}
			return false;
		}
	}
}