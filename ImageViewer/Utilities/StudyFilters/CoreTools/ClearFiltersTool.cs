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

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.CoreTools
{
	[ButtonAction("clear", DefaultToolbarActionSite + "/ToolbarClearFilters", "ClearAll")]
	[IconSet("clear", "Icons.ClearFiltersToolSmall.png", "Icons.ClearFiltersToolMedium.png", "Icons.ClearFiltersToolLarge.png")]
	[Tooltip("clear", "TooltipClearFilters")]
	[EnabledStateObserver("clear", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class ClearFiltersTool : StudyFilterTool
	{
		public event EventHandler EnabledChanged;

		private bool _enabled;

		public bool Enabled
		{
			get { return _enabled; }
			private set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(this.EnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public void ClearAll()
		{
			if (this.AtLeastOneFilter)
			{
				if (base.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmClearAllFilters, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
				{
					foreach (StudyFilterColumn column in (IEnumerable<StudyFilterColumn>) base.Columns)
					{
						column.AutoFilterRoot.Predicates.Clear();
					}
					base.RefreshTable(true);
				}
			}
		}

		private bool AtLeastOneFilter
		{
			get
			{
				foreach (StudyFilterColumn column in (IEnumerable<StudyFilterColumn>) base.Columns)
				{
					if (column.IsColumnFiltered)
						return true;
				}
				return false;
			}
		}

		private void StudyFilter_FilterPredicatesChanged(object sender, EventArgs e)
		{
			this.Enabled = this.AtLeastOneFilter;
		}

		public override void Initialize()
		{
			base.Initialize();
			base.StudyFilter.FilterPredicatesChanged += StudyFilter_FilterPredicatesChanged;
		}

		protected override void Dispose(bool disposing)
		{
			base.StudyFilter.FilterPredicatesChanged -= StudyFilter_FilterPredicatesChanged;
			base.Dispose(disposing);
		}
	}
}