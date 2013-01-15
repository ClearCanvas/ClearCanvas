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
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	[ExtensionPoint]
	public sealed class StudyFilterToolExtensionPoint : ExtensionPoint<ITool> {}

	public interface IStudyFilterToolContext : IToolContext
	{
		IDesktopWindow DesktopWindow { get; }
		IStudyFilter StudyFilter { get; }

		IStudyItem ActiveItem { get; }
		StudyFilterColumn ActiveColumn { get; }
		event EventHandler ActiveChanged;

		StudyItemSelection SelectedItems { get; }

		IList<IStudyItem> Items { get; }
		IStudyFilterColumnCollection Columns { get; }

		bool BulkOperationsMode { get; set; }

		bool Load(bool allowCancel, IEnumerable<string> paths, bool recursive);
		int Load(IEnumerable<string> paths, bool recursive);
		void Refresh();
		void Refresh(bool force);
	}

	public abstract class StudyFilterTool : Tool<IStudyFilterToolContext>
	{
		public const string DefaultToolbarActionSite = "studyfilter-toolbar";
		public const string DefaultContextMenuActionSite = "studyfilter-context";

		protected IStudyFilterColumnCollection Columns
		{
			get { return base.Context.Columns; }
		}

		protected IList<IStudyItem> Items
		{
			get { return base.Context.Items; }
		}

		protected StudyItemSelection SelectedItems
		{
			get { return base.Context.SelectedItems; }
		}

		protected IStudyItem ActiveItem
		{
			get { return base.Context.ActiveItem; }
		}

		protected StudyFilterColumn ActiveColumn
		{
			get { return base.Context.ActiveColumn; }
		}

		protected IDesktopWindow DesktopWindow
		{
			get { return base.Context.DesktopWindow; }
		}

		protected IStudyFilter StudyFilter
		{
			get { return base.Context.StudyFilter; }
		}

		public override void Initialize()
		{
			base.Initialize();
			this.SelectedItems.SelectionChanged += SelectionChangedEventHandler;
			this.Context.ActiveChanged += ActiveChangedEventHandler;
		}

		protected override void Dispose(bool disposing)
		{
			this.Context.ActiveChanged -= ActiveChangedEventHandler;
			this.SelectedItems.SelectionChanged -= SelectionChangedEventHandler;
			base.Dispose(disposing);
		}

		private void SelectionChangedEventHandler(object sender, EventArgs e)
		{
			this.OnSelectionChanged();
		}

		protected virtual void OnSelectionChanged()
		{
			this.AtLeastOneSelected = this.SelectedItems.Count > 0;
		}

		private void ActiveChangedEventHandler(object sender, EventArgs e)
		{
			this.OnActiveChanged();
		}

		protected virtual void OnActiveChanged() {}

		private bool _atLeastOneSelected;

		public event EventHandler AtLeastOneSelectedChanged;

		protected virtual void OnAtLeastOneSelectedChanged()
		{
			EventsHelper.Fire(this.AtLeastOneSelectedChanged, this, EventArgs.Empty);
		}

		public bool AtLeastOneSelected
		{
			get { return _atLeastOneSelected; }
			private set
			{
				if (_atLeastOneSelected != value)
				{
					_atLeastOneSelected = value;
					this.OnAtLeastOneSelectedChanged();
				}
			}
		}

		protected void RefreshTable()
		{
			this.Context.Refresh();
		}

		protected void RefreshTable(bool forceRefresh)
		{
			this.Context.Refresh(forceRefresh);
		}

		protected bool Load(params string[] paths)
		{
			return Load((IEnumerable<string>) paths);
		}

		protected bool Load(IEnumerable<string> paths)
		{
			return Load(true, paths);
		}

		protected bool Load(bool allowCancel, params string[] paths)
		{
			return Load(allowCancel, (IEnumerable<string>) paths);
		}

		protected bool Load(bool allowCancel, IEnumerable<string> paths)
		{
			return this.Context.Load(allowCancel, paths, true);
		}
	}
}