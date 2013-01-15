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
using System.Linq;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	public abstract class StudyBrowserTool : Tool<IStudyBrowserToolContext>
	{
		private bool _enabled = true;
		private event EventHandler _enabledChangedEvent;

		private bool _visible = true;
		private event EventHandler _visibleChangedEvent;


		public override void Initialize()
		{
			base.Initialize();
			Context.SelectedStudyChanged += new EventHandler(OnSelectedStudyChanged);
			Context.SelectedServerChanged += new EventHandler(OnSelectedServerChanged);
		}

		protected virtual void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			if (Context.SelectedStudy != null)
				Enabled = true;
			else
				Enabled = false;
		}

		protected abstract void OnSelectedServerChanged(object sender, EventArgs e);

		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChangedEvent += value; }
			remove { _enabledChangedEvent -= value; }
		}

		public bool Visible
		{
			get { return _visible; }
			protected set
			{
				if (_visible != value)
				{
					_visible = value;
					EventsHelper.Fire(_visibleChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler VisibleChanged
		{
			add { _visibleChangedEvent += value; }
			remove { _visibleChangedEvent -= value; }
		}
		
		protected int ProcessItemsAsync<T>(IEnumerable<T> items, Action<T> processAction, bool cancelable)
		{
			var itemsToProcess = items.ToList();
			return ProgressDialog.Show(this.Context.DesktopWindow,
				itemsToProcess,
				(item, i) =>
				{
					processAction(item);
					return string.Format(SR.MessageProcessedItemsProgress, i + 1, itemsToProcess.Count);
				},
				cancelable);
		}
	}
}
