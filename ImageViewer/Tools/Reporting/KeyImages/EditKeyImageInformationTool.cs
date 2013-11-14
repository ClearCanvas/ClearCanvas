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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Clipboard;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[ButtonAction("edit", KeyImageClipboardComponent.ToolbarSite + "/ToolbarEditKeyImageInformation", "Edit")]
	[Tooltip("edit", "TooltipEditKeyImageInformation")]
	[IconSet("edit", "Icons.EditKeyImageInformationToolSmall.png", "Icons.EditKeyImageInformationToolMedium.png", "Icons.EditKeyImageInformationToolLarge.png")]
	[EnabledStateObserver("edit", "Enabled", "EnabledChanged")]
	//
	[ExtensionOf(typeof (KeyImageClipboardComponentToolExtensionPoint))]
	internal class EditKeyImageInformationTool : Tool<IClipboardToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled == value)
					return;

				_enabled = value;
				EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public override void Initialize()
		{
			base.Initialize();

			Context.ClipboardItemsChanged += OnClipboardItemsChanged;
			Context.SelectedClipboardItemsChanged += OnSelectionChanged;
		}

		/// <summary>
		/// Disposes of this object; override this method to do any necessary cleanup.
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized.</param>
		protected override void Dispose(bool disposing)
		{
			Context.ClipboardItemsChanged -= OnClipboardItemsChanged;
			Context.SelectedClipboardItemsChanged -= OnSelectionChanged;

			base.Dispose(disposing);
		}

		private void OnClipboardItemsChanged(object sender, EventArgs e)
		{
			Enabled = Context.ClipboardItems.Count > 0;
		}

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			Enabled = Context.ClipboardItems.Count > 0;
		}

		public void Edit()
		{
			KeyImageInformationEditorComponent.Launch(Context.DesktopWindow);
		}
	}
}