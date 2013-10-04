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

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarNext", "Next")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipNext")]
	[IconSet("activate", "Icons.NextToolSmall.png", "Icons.NextToolSmall.png", "Icons.NextToolSmall.png")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint))]
	public class NextTool : DicomEditorTool
	{
		public NextTool() {}

		public void Next()
		{
			Activate();
		}

		protected override void ActivateCore()
		{
			this.Context.DumpManagement.LoadedFileDumpIndex += 1;
			this.Context.UpdateDisplay();
		}

		protected override void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
		{
			this.Enabled = !(e.IsCurrentTheOnly || e.IsCurrentLast);
		}
	}
}