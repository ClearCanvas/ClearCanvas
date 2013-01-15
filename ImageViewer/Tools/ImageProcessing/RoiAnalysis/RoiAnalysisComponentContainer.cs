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
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis
{
	public class RoiAnalysisComponentContainer : TabComponentContainer
	{
		private TabPage _roiHistogramPage;
		private TabPage _pathProfilePage;

		public RoiAnalysisComponentContainer(IImageViewerToolContext imageViewerToolContext)
		{
			RoiHistogramComponent roiHistogramComponent = new RoiHistogramComponent(imageViewerToolContext);
			roiHistogramComponent.Container = this;
			_roiHistogramPage = new TabPage("Roi", roiHistogramComponent);
			this.Pages.Add(_roiHistogramPage);

			PathProfileComponent pathProfileComponent = new PathProfileComponent(imageViewerToolContext);
			pathProfileComponent.Container = this;
			_pathProfilePage = new TabPage("Path", pathProfileComponent);
			this.Pages.Add(_pathProfilePage);
		}

		public override void Start()
		{
			base.Start();

			((RoiHistogramComponent)_roiHistogramPage.Component).Initialize();
			((PathProfileComponent)_pathProfilePage.Component).Initialize();
		}

		internal RoiAnalysisComponent SelectedComponent
		{
			get { return this.CurrentPage.Component as RoiAnalysisComponent; }
			set
			{
				if (value is RoiHistogramComponent)
					this.CurrentPage = _roiHistogramPage;
				else if (value is PathProfileComponent)
					this.CurrentPage = _pathProfilePage;
			}
		}

	}
}
