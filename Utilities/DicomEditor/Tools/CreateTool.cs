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
using ClearCanvas.Dicom;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarCreate", "Create")]
	[MenuAction("activate", "dicomeditor-contextmenu/MenuCreate", "Create")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipCreate")]
	[IconSet("activate", "Icons.AddToolSmall.png", "Icons.AddToolSmall.png", "Icons.AddToolSmall.png")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint), FeatureToken = FeatureTokens.DicomEditing)]
	public class CreateTool : DicomEditorTool
	{
		public CreateTool() : base(true) {}

		public void Create()
		{
			if (!LicenseInformation.IsFeatureAuthorized(FeatureTokens.DicomEditing))
				return;

			Activate();
		}

		protected override void ActivateCore()
		{
			var creator = new DicomEditorCreateToolComponent();
			var result = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, creator, SR.TitleCreateTag);
			if (result != ApplicationComponentExitCode.Accepted)
				return;

			try
			{
				//We can only edit tags in the DicomTagDictionary, currently.
				Context.DumpManagement.EditTag(creator.TagId, creator.Value, false);
				Context.UpdateDisplay();
			}
			catch (DicomException)
			{
				Context.DesktopWindow.ShowMessageBox(SR.MessageTagCannotBeCreated, MessageBoxActions.Ok);
			}
		}

		protected override void OnSelectedTagChanged(object sender, EventArgs e)
		{
			if (this.Context.SelectedTags != null && this.Context.SelectedTags.Count > 1)
				this.Enabled = false;
			else
				this.Enabled = true;
		}

		protected override void OnIsLocalFileChanged(object sender, EventArgs e)
		{
			this.Enabled = base.Context.IsLocalFile;
		}
	}
}