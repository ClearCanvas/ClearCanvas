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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis
{
	/// <summary>
	/// This tool runs an instance of <see cref="LayoutComponent"/> in a shelf, and coordinates
	/// it so that it reflects the state of the active workspace.
	/// </summary>
	[MenuAction("show", "global-menus/MenuTools/MenuStandard/MenuRoiAnalysis", "Show")]
	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarRoiAnalysis", "Show")]
	[IconSet("show", "Icons.RoiHistogramToolSmall.png", "Icons.RoiHistogramToolMedium.png", "Icons.RoiHistogramToolLarge.png")]
	[Tooltip("show", "TooltipRoiAnalysis")]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class RoiAnalysisTool : ImageViewerTool
	{
		private static RoiAnalysisComponentContainer _roiAnalysisComponent;
		private static IShelf _roiAnalysisShelf;

		/// <summary>
		/// Constructor
		/// </summary>
		public RoiAnalysisTool() {}

		/// <summary>
		/// Overridden to subscribe to workspace activation events
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
		}

		/// <summary>
		/// Shows the ROI Histogram component in a shelf.  Only one ROI Histogram component will ever be shown
		/// at a time, so if there is already an ROI Histogram component showing, this method does nothing
		/// </summary>
		public void Show()
		{
			// check if a layout component is already displayed
			if (_roiAnalysisComponent == null)
			{
				// create and initialize the layout component
				_roiAnalysisComponent = new RoiAnalysisComponentContainer(this.Context);

				// launch the layout component in a shelf
				_roiAnalysisShelf = ApplicationComponent.LaunchAsShelf(
					this.Context.DesktopWindow,
					_roiAnalysisComponent,
					SR.Title,
					ShelfDisplayHint.DockLeft);

				_roiAnalysisShelf.Closed += RoiAnalysisShelf_Closed;
			}
		}

		private static void RoiAnalysisShelf_Closed(object sender, ClosedEventArgs e)
		{
			// note that the component is thrown away when the shelf is closed by the user
			_roiAnalysisShelf.Closed -= RoiAnalysisShelf_Closed;
			_roiAnalysisShelf = null;
			_roiAnalysisComponent = null;
		}
	}
}