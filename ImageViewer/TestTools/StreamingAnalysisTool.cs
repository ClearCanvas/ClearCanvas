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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.TestTools
{
	[MenuAction("activate", "dicomstudybrowser-contextmenu/StreamingAnalysis", "OpenAnalysisTool")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class StreamingAnalysisTool : StudyBrowserTool
	{
		private IShelf _shelf;

		public StreamingAnalysisTool()
		{
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			Enabled = true;
		}

		public override void Initialize()
		{
			base.Initialize();
			Enabled = true;
		}

		public void OpenAnalysisTool()
		{
			if (_shelf != null)
			{
				_shelf.Activate();
			}
			else
			{
				StreamingAnalysisComponent component = new StreamingAnalysisComponent(base.Context);
				_shelf = ApplicationComponent.LaunchAsShelf(base.Context.DesktopWindow, component, 
					"Streaming Analysis", ShelfDisplayHint.DockFloat | ShelfDisplayHint.ShowNearMouse);

				_shelf.Closed += delegate { _shelf = null; };
			}
		}
	}
}
