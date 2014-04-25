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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.TestTools
{
	[ButtonAction("apply", "global-toolbars/Performance Analysis", "Apply")]
	[IconSet("apply", "PerformanceAnalysisToolSmall.png", "PerformanceAnalysisToolMedium.png", "PerformanceAnalysisToolLarge.png")]

	[ExtensionOf(typeof(DesktopToolExtensionPoint), Enabled=false)]
	public class PerformanceAnalysisTool : Tool<IDesktopToolContext>
	{
		private IShelf _shelf;

		public PerformanceAnalysisTool()
		{
		}

		public void Apply()
		{
			if (_shelf != null)
			{
				_shelf.Activate();
				return;
			}

			PerformanceAnalysisComponent component = new PerformanceAnalysisComponent();
			_shelf = ApplicationComponent.LaunchAsShelf(this.Context.DesktopWindow, component, "Performance Analysis",
			                                   ShelfDisplayHint.DockFloat);

			_shelf.Closing += delegate { _shelf = null; };
		}
	}
}
