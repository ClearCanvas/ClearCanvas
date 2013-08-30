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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarReplicate", "Replicate")]
	[MenuAction("activate", "dicomeditor-contextmenu/MenuReplicate", "Replicate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipReplicate")]
	[IconSet("activate", "Icons.CopyToolSmall.png", "Icons.CopyToolSmall.png", "Icons.CopyToolSmall.png")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint), FeatureToken = FeatureTokens.DicomEditing)]
	public class ReplicateTool : DicomEditorTool
	{
		public ReplicateTool() : base(true) {}

		public void Replicate()
		{
			if (!LicenseInformation.IsFeatureAuthorized(FeatureTokens.DicomEditing))
				return;

			Activate();
		}

		protected override void ActivateCore()
		{
			// if it's not a root level tag, it's part of a sequence. if it's not editable, it's SQ or OB or OW or UN or ?? and thus cannot be set by string value
			if (CollectionUtils.Contains(Context.SelectedTags, t => !t.IsRootLevelTag || !t.IsEditable()))
			{
				Context.DesktopWindow.ShowMessageBox(SR.MessageSequenceBinaryReplicationNotSupported, MessageBoxActions.Ok);
				return;
			}

			if (Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmReplicateTagsInAllFiles, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
			{
				foreach (DicomEditorTag tag in Context.SelectedTags)
				{
					Context.DumpManagement.EditTag(tag.TagId, tag.Value, true);
				}
				Context.UpdateDisplay();
			}
		}

		protected override void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e)
		{
			this.Enabled = !e.IsCurrentTheOnly;
		}

		protected override void OnIsLocalFileChanged(object sender, EventArgs e)
		{
			this.Enabled = base.Context.IsLocalFile;
		}
	}
}