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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/FilterDuplicateStudies", "Toggle", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/FilterDuplicateStudies", "Toggle", Flags = ClickActionFlags.CheckAction)]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("activate", "Visible", "VisibleChanged")]
	[CheckedStateObserver("activate", "Checked", "CheckedChanged")]
	[TooltipValueObserver("activate", "Tooltip", "CheckedChanged")]
	[LabelValueObserver("activate", "Label", "CheckedChanged")]
	[IconSet("activate", "Icons.FilterDuplicateStudiesToolSmall.png", "Icons.FilterDuplicateStudiesToolMedium.png", "Icons.FilterDuplicateStudiesToolLarge.png")]
	internal class FilterDuplicateStudiesTool : StudyBrowserTool
	{
		private readonly StudyBrowserComponent _parent;
		private bool _checked;
		private event EventHandler _checkedChanged;

		public FilterDuplicateStudiesTool(StudyBrowserComponent parent)
		{
			_parent = parent;
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			UpdateState();
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			UpdateState();
		}

		private void UpdateState()
		{
			if (!_parent.SelectedServers.IsLocalServer && _parent.SelectedServers.Count > 1)
			{
				this.Enabled = _parent.CurrentSearchResult != null && _parent.CurrentSearchResult.HasDuplicates;
				this.Visible = true;
			}
			else
			{
				this.Visible = false;
				this.Enabled = false;
			}

			this.Checked = _parent.FilterDuplicateStudies;
		}

		public bool Checked
		{
			get { return _checked; }
			set
			{
				if (_checked != value)
				{
					_checked = value;
					EventsHelper.Fire(_checkedChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler CheckedChanged
		{
			add { this._checkedChanged += value; }
			remove { this._checkedChanged -= value; }
		}

		public string Label
		{
			get { return Tooltip; }
		}
		public string Tooltip
		{
			get
			{
				if (Checked)
					return SR.TooltipShowAllStudies;
				else
					return SR.TooltipHideDuplicateStudies;
			}
		}

		public void Toggle()
		{
			if (Visible && Enabled)
			{
				this._parent.FilterDuplicateStudies = !this._parent.FilterDuplicateStudies;
				UpdateState();
			}
		}
	}
}