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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public abstract partial class StudyFilterColumn : IComparer<IStudyItem>
	{
		private IStudyFilter _owner;

		protected StudyFilterColumn() {}

		public IStudyFilter Owner
		{
			get { return _owner; }
			internal set
			{
				if (_owner != value)
				{
					if (_owner != null)
					{
						_owner.ItemAdded -= Owner_ItemAdded;
						_owner.ItemRemoved -= Owner_ItemRemoved;

						// dispose filter root after disposing tools, since some tools might hold references to the filter root
						this.DisposeTools();
						this.Owner.FilterPredicates.Remove(_autoFilterRoot);
						_autoFilterRoot = null;
					}

					_owner = value;

					if (_owner != null)
					{
						_autoFilterRoot = new AutoFilterRootPredicate();
						this.Owner.FilterPredicates.Add(_autoFilterRoot);

						_owner.ItemAdded += Owner_ItemAdded;
						_owner.ItemRemoved += Owner_ItemRemoved;
					}

					this.OnOwnerChanged();
				}
			}
		}

		public abstract string Name { get; }

		public abstract string Key { get; }

		public virtual string GetText(IStudyItem item)
		{
			object value = this.GetValue(item);
			if (value == null)
				return string.Empty;
			return value.ToString();
		}

		public abstract object GetValue(IStudyItem item);

		public abstract Type GetValueType();

		public virtual bool Parse(string input, out object output)
		{
			output = null;
			return false;
		}

		public virtual int Compare(IStudyItem x, IStudyItem y)
		{
			return 0;
		}

		public override sealed string ToString()
		{
			return this.Name;
		}

		internal abstract TableColumnBase<IStudyItem> CreateTableColumn();

		#region Event Handling

		private void Owner_ItemAdded(object sender, EventArgs e)
		{
			this.OnOwnerItemAdded();
		}

		private void Owner_ItemRemoved(object sender, EventArgs e)
		{
			this.OnOwnerItemRemoved();
		}

		protected virtual void OnOwnerItemAdded() {}

		protected virtual void OnOwnerItemRemoved() {}

		protected virtual void OnOwnerChanged() {}

		#endregion

		#region Tools and Actions

		private ToolSet _tools;
		private ActionModelNode _actionModel;

		public ToolSet Tools
		{
			get
			{
				if (_tools == null)
					_tools = new ToolSet(new AutoFilterToolExtensionPoint(), new ToolContext(this));

				return _tools;
			}
		}

		public ActionModelNode FilterMenuModel
		{
			get
			{
				if (_actionModel == null)
					_actionModel = ActionModelRoot.CreateModel(this.GetType().Namespace, "studyfilters-columnfilters", this.Tools.Actions);

				return _actionModel;
			}
		}

		private void DisposeTools()
		{
			// if column is removed from an owner, dispose any tools which are hanging on
			if (_actionModel != null)
				_actionModel = null;

			if (_tools != null)
			{
				_tools.Dispose();
				_tools = null;
			}
		}

		private class ToolContext : IAutoFilterToolContext
		{
			private readonly StudyFilterColumn _column;

			public ToolContext(StudyFilterColumn column)
			{
				_column = column;
			}

			public StudyFilterColumn Column
			{
				get { return _column; }
			}
		}

		#endregion

		#region Column Filter

		private AutoFilterRootPredicate _autoFilterRoot;

		public CompositeFilterPredicate AutoFilterRoot
		{
			get { return _autoFilterRoot; }
		}

		public bool IsColumnFiltered
		{
			get { return _autoFilterRoot != null && _autoFilterRoot.IsActive; }
		}

		private class AutoFilterRootPredicate : CompositeFilterPredicate
		{
			public override bool Evaluate(IStudyItem item)
			{
				foreach (FilterPredicate predicate in base.Predicates)
				{
					if (!predicate.Evaluate(item))
						return false;
				}
				return true;
			}
		}

		#endregion
	}
}