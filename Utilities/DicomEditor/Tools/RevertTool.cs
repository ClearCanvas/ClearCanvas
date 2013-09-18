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

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarRevert", "Revert")]
	[MenuAction("activate", "dicomeditor-contextmenu/MenuRevert", "Revert")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipRevert")]
	[IconSet("activate", "Icons.UndoToolSmall.png", "Icons.UndoToolSmall.png", "Icons.UndoToolSmall.png")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint), FeatureToken = FeatureTokens.DicomEditing)]
	public class RevertTool : DicomEditorTool
	{
		private bool _promptForAll;

		public RevertTool() : base(true) {}

		public void Revert()
		{
			if (!LicenseInformation.IsFeatureAuthorized(FeatureTokens.DicomEditing))
				return;

			Activate();
		}

		protected override void ActivateCore()
		{
			if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmRevert, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
			{
				bool revertAll = false;

				if (_promptForAll)
				{
					if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmRevertAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
						revertAll = true;
				}

				this.Context.DumpManagement.RevertEdits(revertAll);
				this.Context.UpdateDisplay();
				this.Enabled = false;
			}
		}

		protected override void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
		{
			_promptForAll = !e.IsCurrentTheOnly;
			this.Enabled = e.HasCurrentBeenEdited;
		}

		protected override void OnTagEdited(object sender, EventArgs e)
		{
			this.Enabled = true;
		}

		protected override void OnIsLocalFileChanged(object sender, EventArgs e)
		{
			this.Enabled = base.Context.IsLocalFile;
		}
	}
}