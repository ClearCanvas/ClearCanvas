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
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails
{
	[ExtensionPoint]
	public sealed class SeriesDetailsToolExtensionPoint : ExtensionPoint<ITool> {}

	public interface ISeriesDetailsToolContext : IToolContext
	{
	    IDicomServiceNode Server { get; }
        StudyTableItem Study { get; }
        IList<SeriesTableItem> AllSeries { get; }
        IList<SeriesTableItem> SelectedSeries { get; }
		event EventHandler SelectedSeriesChanged;
		void RefreshSeriesTable();

		IDesktopWindow DesktopWindow { get; }
	}

	public abstract class SeriesDetailsTool : Tool<ISeriesDetailsToolContext>
	{
		protected internal const string ToolbarActionSite = "dicomseriesdetails-toolbar";
		protected internal const string ContextMenuActionSite = "dicomseriesdetails-contextmenu";

		private event EventHandler _enabledChanged;
		private bool _enabled;

		protected SeriesDetailsTool() {}

		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					OnEnabledChanged();
				}
			}
		}

		protected StudyTableItem Study
		{
			get { return Context.Study; }
		}

		protected IDicomServiceNode Server
		{
			get { return Study.Server; }
		}

        protected IList<SeriesTableItem> AllSeries
		{
			get { return Context.AllSeries; }
		}

        protected IList<SeriesTableItem> SelectedSeries
		{
			get { return Context.SelectedSeries; }
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Context.SelectedSeriesChanged += OnComponentSelectedSeriesChanged;
		}

		protected override void Dispose(bool disposing)
		{
			this.Context.SelectedSeriesChanged -= OnComponentSelectedSeriesChanged;
			base.Dispose(disposing);
		}

		private void OnComponentSelectedSeriesChanged(object sender, EventArgs e)
		{
			OnSelectedSeriesChanged();
		}

		protected virtual void OnEnabledChanged()
		{
			EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
		}

		protected virtual void OnSelectedSeriesChanged()
		{
			this.Enabled = this.Context.SelectedSeries.Count > 0;
		}
	}
}