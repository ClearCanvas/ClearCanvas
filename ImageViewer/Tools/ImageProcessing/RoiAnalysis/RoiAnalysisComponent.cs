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
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis
{
	public abstract class RoiAnalysisComponent : ImageViewerToolComponent
	{
		private bool _enabled = false;
		private RoiAnalysisComponentContainer _container;

		protected RoiAnalysisComponent(IImageViewerToolContext imageViewerToolContext)
			: base(imageViewerToolContext.DesktopWindow)
		{
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				_enabled = value;
				NotifyPropertyChanged("Enabled");
			}
		}

		internal RoiAnalysisComponentContainer Container
		{
			get { return _container; }
			set { _container = value; }
		}

		public override void Start()
		{
			// If there's an ROI selected already when 
			WatchRoiGraphic(GetSelectedRoi());

			base.Start();
		}

		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		public RoiGraphic GetSelectedRoi()
		{
			if (this.ImageViewer == null)
				return null;

			if (this.ImageViewer.SelectedPresentationImage == null)
				return null;

			if (this.ImageViewer.SelectedPresentationImage.SelectedGraphic == null)
				return null;

			RoiGraphic graphic =
				this.ImageViewer.SelectedPresentationImage.SelectedGraphic as RoiGraphic;

			return graphic;
		}

		protected abstract bool CanAnalyzeSelectedRoi();

		protected void OnAllPropertiesChanged()
		{
			if (CanAnalyzeSelectedRoi())
			{
				if (this.Container != null)
					this.Container.SelectedComponent = this;
			}

			base.NotifyAllPropertiesChanged();
		}

		internal void Initialize()
		{
			OnAllPropertiesChanged();
		}

		protected override void OnActiveImageViewerChanged(ActiveImageViewerChangedEventArgs e)
		{
			if (e.DeactivatedImageViewer != null)
				e.DeactivatedImageViewer.EventBroker.GraphicSelectionChanged -= new EventHandler<GraphicSelectionChangedEventArgs>(OnGraphicSelectionChanged);

			if (e.ActivatedImageViewer != null)
				e.ActivatedImageViewer.EventBroker.GraphicSelectionChanged += new EventHandler<GraphicSelectionChangedEventArgs>(OnGraphicSelectionChanged);

			OnAllPropertiesChanged();
		}

		private void OnGraphicSelectionChanged(object sender, GraphicSelectionChangedEventArgs e)
		{
			RoiGraphic deselectedGraphic = e.DeselectedGraphic as RoiGraphic;
			RoiGraphic selectedGraphic = e.SelectedGraphic as RoiGraphic;

			UnwatchRoiGraphic(deselectedGraphic);
			WatchRoiGraphic(selectedGraphic);

			OnAllPropertiesChanged();
		}

		private void UnwatchRoiGraphic(RoiGraphic roiGraphic)
		{
			if (roiGraphic != null)
				roiGraphic.RoiChanged -= OnRoiChanged;
		}

		private void WatchRoiGraphic(RoiGraphic roiGraphic)
		{
			if (roiGraphic != null)
				roiGraphic.RoiChanged += OnRoiChanged;
		}

		private void OnRoiChanged(object sender, EventArgs e)
		{
			OnAllPropertiesChanged();
		}
	}
}
