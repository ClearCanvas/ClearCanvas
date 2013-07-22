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
	[ButtonAction("activate", "dicomeditor-toolbar/ToolbarSaveAs", "SaveAs")]
	[Tooltip("activate", "TooltipSaveAs")]
	[IconSet("activate", "Icons.SaveAsToolSmall.png", "Icons.SaveAsToolSmall.png", "Icons.SaveAsToolSmall.png")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof (DicomEditorToolExtensionPoint), FeatureToken = FeatureTokens.DicomEditing)]
	public class SaveAsTool : DicomEditorTool
	{
		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public SaveAsTool() : base(true) {}

		/// <summary>
		/// Called by the framework when the user clicks the "apply" menu item or toolbar button.
		/// </summary>
		public void SaveAs()
		{
			if (!LicenseInformation.IsFeatureAuthorized(FeatureTokens.DicomEditing))
				return;

			Activate();
		}

		protected override void ActivateCore()
		{
			if (Context.DumpManagement.SaveAll(true))
				Context.UpdateDisplay();
		}

		protected override void OnIsLocalFileChanged(object sender, EventArgs e)
		{
			Enabled = Context.IsLocalFile;
		}
	}
}