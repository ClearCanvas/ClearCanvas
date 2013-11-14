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
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Clipboard;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[ButtonAction("revert", KeyImageClipboardComponent.ToolbarSite + "/ToolbarRevertKeyImage", "Revert")]
	[ButtonAction("revert", KeyImageClipboardComponent.MenuSite + "/MenuRevertKeyImage", "Revert")]
	[IconSet("revert", "Icons.RevertToolSmall.png", "Icons.RevertToolMedium.png", "Icons.RevertToolLarge.png")]
	[EnabledStateObserver("revert", "CanRevert", "ActionsChanged")]
	//
	[ButtonAction("revertAll", KeyImageClipboardComponent.ToolbarSite + "/ToolbarRevertAllKeyImages", "RevertAll")]
	[IconSet("revertAll", "Icons.RevertAllToolSmall.png", "Icons.RevertAllToolMedium.png", "Icons.RevertAllToolLarge.png")]
	[EnabledStateObserver("revertAll", "CanRevertAll", "ActionsChanged")]
	//
	[ExtensionOf(typeof (KeyImageClipboardComponentToolExtensionPoint))]
	internal class RevertKeyImageClipboardTool : Tool<IClipboardToolContext>
	{
		public bool CanRevert { get; set; }
		public bool CanRevertAll { get; set; }

		public event EventHandler ActionsChanged;

		public override void Initialize()
		{
			base.Initialize();

			Context.ClipboardItemsChanged += OnSelectionChanged;
			Context.SelectedClipboardItemsChanged += OnSelectionChanged;
		}

		protected override void Dispose(bool disposing)
		{
			Context.ClipboardItemsChanged -= OnSelectionChanged;
			Context.SelectedClipboardItemsChanged -= OnSelectionChanged;

			base.Dispose(disposing);
		}

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			CanRevert = Context.ClipboardItems.Count > 0 && Context.SelectedClipboardItems.Any(CanRevertItem);
			CanRevertAll = Context.ClipboardItems.Count > 0 && Context.ClipboardItems.Any(CanRevertItem);
			EventsHelper.Fire(ActionsChanged, this, EventArgs.Empty);
		}

		public void Revert()
		{
			if (CanRevert)
				Revert(Context.SelectedClipboardItems.Where(item => item.HasChanges()));
		}

		public void RevertAll()
		{
			if (CanRevertAll)
				Revert(Context.ClipboardItems.Where(item => item.HasChanges()));
		}

		private void Revert(IEnumerable<IClipboardItem> items)
		{
			try
			{
				foreach (var item in items.ToList())
				{
					var clipboard = KeyImageClipboard.GetKeyImageClipboard(Context.DesktopWindow);
					var selectionDocumentInstanceUid = item.GetSelectionDocumentInstanceUid();
					var context = clipboard.AvailableContexts.FirstOrDefault(c => c.DocumentInstanceUid == selectionDocumentInstanceUid);
					if (context != null)
					{
						if (item.RevertKeyImage(context))
						{
							clipboard.CurrentContext = context;
						}
					}
				}
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, Context.DesktopWindow);
			}
		}

		private static bool CanRevertItem(IClipboardItem item)
		{
			return item.IsSerialized() && item.HasChanges();
		}
	}
}