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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails
{
	[MenuAction("show", "dicomstudybrowser-contextmenu/MenuShowSeriesDetails", "Show")]
	[ButtonAction("show", "dicomstudybrowser-toolbar/ToolbarShowSeriesDetails", "Show")]

	[Tooltip("show", "TooltipSeriesDetails")]
	[IconSet("show", "Icons.ShowSeriesDetailsToolSmall.png", "Icons.ShowSeriesDetailsToolMedium.png", "Icons.ShowSeriesDetailsToolLarge.png")]

	[EnabledStateObserver("show", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("show", "Visible", "VisibleChanged")]

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class ShowSeriesDetailsTool : StudyBrowserTool
	{
		public override void Initialize()
		{
			base.Initialize();

			UpdateEnabled();
		}

		protected override void OnSelectedServerChanged(object sender, System.EventArgs e)
		{
			UpdateEnabled();
		}

		protected override void OnSelectedStudyChanged(object sender, System.EventArgs e)
		{
			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
            Visible = Context.SelectedServers.AnySupport<IStudyRootQuery>();

			if (Context.SelectedServers == null)
			{
			    Enabled = false;
			}
            else if (Context.SelectedStudy == null || Context.SelectedStudies.Count > 1)
            {
                Enabled = false;
            }
            else
            {
                Enabled = Context.SelectedStudy.Server != null &&
                          Context.SelectedStudy.Server.IsSupported<IStudyRootQuery>();
            }
        }

		public void Show()
		{
			UpdateEnabled();

			if (!Enabled)
				return;

			try
			{
				var component = new SeriesDetailsComponent(Context.SelectedStudy);
				ApplicationComponent.LaunchAsDialog(Context.DesktopWindow, component, SR.TitleSeriesDetails);
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
