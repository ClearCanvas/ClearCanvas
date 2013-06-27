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
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarDelete", "Delete")]
	[MenuAction("activate", "dicomeditor-contextmenu/MenuDelete", "Delete")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipDelete")]
	[IconSet("activate", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint), FeatureToken = FeatureTokens.DicomEditing)]
	public class DeleteTool : DicomEditorTool
	{
		private bool _promptForAll;

		public DeleteTool() : base(true) {}

		public void Delete()
		{
			if (!LicenseInformation.IsFeatureAuthorized(FeatureTokens.DicomEditing))
				return;

			Activate();
		}

		protected override void ActivateCore()
		{
			if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmDeleteSelectedTags, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
			{
				bool tagDeleted = false;
				bool applyToAll = false;

				if (_promptForAll)
				{
					if (this.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmDeleteSelectedTagsFromAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
						applyToAll = true;
				}

				foreach (DicomEditorTag tag in this.Context.SelectedTags)
				{
					if (tag.TagId != 0)
					{
						this.Context.DumpManagement.DeleteTag(tag.TagId, applyToAll);
						tagDeleted = true;
					}
				}

				if (tagDeleted)
					this.Context.UpdateDisplay();
				else
					this.Context.DesktopWindow.ShowMessageBox(SR.MessageNoTagsWereDeleted, MessageBoxActions.Ok);
			}
		}

		protected override void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
		{
			_promptForAll = !e.IsCurrentTheOnly;
		}

		protected override void OnIsLocalFileChanged(object sender, EventArgs e)
		{
			this.Enabled = base.Context.IsLocalFile;
		}
	}
}