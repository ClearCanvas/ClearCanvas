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
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.CopyToClipboard
{
	[MenuAction("copyImage", "imageviewer-contextmenu/MenuClipboard/MenuCopyImageToClipboard", "CopyImage")]
	[MenuAction("copyImage", ShowClipboardTool.ClipboardToolbarDropdownSite + "/MenuCopyImageToClipboard", "CopyImage")]
	[IconSet("copyImage", "Icons.CopyToClipboardToolSmall.png", "Icons.CopyToClipboardToolMedium.png", "Icons.CopyToClipboardToolLarge.png")]
	[EnabledStateObserver("copyImage", "CopyImageEnabled", "CopyImageEnabledChanged")]

	[MenuAction("copyDisplaySet", "imageviewer-contextmenu/MenuClipboard/MenuCopyDisplaySetToClipboard", "CopyDisplaySet")]
	[MenuAction("copyDisplaySet", ShowClipboardTool.ClipboardToolbarDropdownSite + "/MenuCopyDisplaySetToClipboard", "CopyDisplaySet")]
	[IconSet("copyDisplaySet", "Icons.CopyToClipboardToolSmall.png", "Icons.CopyToClipboardToolMedium.png", "Icons.CopyToClipboardToolLarge.png")]
	[EnabledStateObserver("copyDisplaySet", "CopyDisplaySetEnabled", "CopyDisplaySetEnabledChanged")]

	[MenuAction("copySubset", "imageviewer-contextmenu/MenuClipboard/MenuCopySubsetToClipboard", "CopySubset")]
	[MenuAction("copySubset", ShowClipboardTool.ClipboardToolbarDropdownSite + "/MenuCopySubsetToClipboard", "CopySubset")]
	[IconSet("copySubset", "Icons.CopyToClipboardToolSmall.png", "Icons.CopyToClipboardToolMedium.png", "Icons.CopyToClipboardToolLarge.png")]
	[EnabledStateObserver("copySubset", "CopySubsetEnabled", "CopySubsetEnabledChanged")]

	[ExtensionOf(typeof(ClipboardToolbarToolExtensionPoint))]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class CopyToClipboardTool : Tool<IImageViewerToolContext>
	{
		private static IShelf _copySubsetShelf;

		private bool _copyDisplaySetEnabled;
		private bool _copyImageEnabled;
		private bool _copySubsetEnabled;

		private event EventHandler _copyDisplaySetEnabledChanged;
		private event EventHandler _copyImageEnabledChanged;
		private event EventHandler _copySubsetEnabledChanged;

		public CopyToClipboardTool()
		{
			_copyDisplaySetEnabled = false;
			_copyImageEnabled = false;
			_copySubsetEnabled = false;
		}

		public bool CopyDisplaySetEnabled
		{
			get { return _copyDisplaySetEnabled; }	
			set
			{
				if (value == _copyDisplaySetEnabled)
					return;

				_copyDisplaySetEnabled = value;
				EventsHelper.Fire(_copyDisplaySetEnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler CopyDisplaySetEnabledChanged
		{
			add { _copyDisplaySetEnabledChanged += value; }
			remove { _copyDisplaySetEnabledChanged -= value; }
		}

		public bool CopyImageEnabled
		{
			get { return _copyImageEnabled; }
			set
			{
				if (value == _copyImageEnabled)
					return;

				_copyImageEnabled = value;
				EventsHelper.Fire(_copyImageEnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler CopyImageEnabledChanged
		{
			add { _copyImageEnabledChanged += value; }
			remove { _copyImageEnabledChanged -= value; }
		}

		public bool CopySubsetEnabled
		{
			get { return _copySubsetEnabled; }
			set
			{
				if (value == _copySubsetEnabled)
					return;

				_copySubsetEnabled = value;
				EventsHelper.Fire(_copySubsetEnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler CopySubsetEnabledChanged
		{
			add { _copySubsetEnabledChanged += value; }
			remove { _copySubsetEnabledChanged -= value; }
		}

		public override void Initialize()
		{
			base.Initialize();

			base.Context.Viewer.EventBroker.ImageBoxSelected += OnImageBoxSelected;
			base.Context.Viewer.EventBroker.DisplaySetSelected += OnDisplaySetSelected;
		}

		protected override void Dispose(bool disposing)
		{
			base.Context.Viewer.EventBroker.ImageBoxSelected -= OnImageBoxSelected;
			base.Context.Viewer.EventBroker.DisplaySetSelected -= OnDisplaySetSelected;

			base.Dispose(disposing);
		}

		public void CopyImage()
		{
			try
			{
				BlockingOperation.Run(
					delegate
						{
							Clipboard.Add(this.Context.Viewer.SelectedPresentationImage);
						});
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageClipboardCopyFailed, Context.DesktopWindow);
			}
		}

		public void CopyDisplaySet()
		{
			try
			{
				BlockingOperation.Run(
					delegate
						{
							Clipboard.Add(this.Context.Viewer.SelectedPresentationImage.ParentDisplaySet);
						});
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageClipboardCopyFailed, Context.DesktopWindow);
			}
		}

		public void CopySubset()
		{
			try
			{
				CopySubsetToClipboardComponent component;

				if (_copySubsetShelf != null)
				{
					component = (CopySubsetToClipboardComponent)_copySubsetShelf.Component;
					if (component.DesktopWindow != this.Context.DesktopWindow)
					{
						component.Close();
					}
					else
					{
						_copySubsetShelf.Activate();
						return;
					}
				}

				IDesktopWindow desktopWindow = this.Context.DesktopWindow;

				component = new CopySubsetToClipboardComponent(desktopWindow);

				_copySubsetShelf = ApplicationComponent.LaunchAsShelf(
					desktopWindow,
					component,
					SR.TitleCopySubsetToClipboard,
					ShelfDisplayHint.ShowNearMouse);

				_copySubsetShelf.Closed += delegate { _copySubsetShelf = null; };
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageClipboardCopyFailed, Context.DesktopWindow);
			}
		}

		private void OnImageBoxSelected(object sender, ImageBoxSelectedEventArgs e)
		{
			if (e.SelectedImageBox.DisplaySet == null)
				UpdateEnabled(null);
		}

		private void OnDisplaySetSelected(object sender, DisplaySetSelectedEventArgs e)
		{
			UpdateEnabled(e.SelectedDisplaySet);
		}

		private void UpdateEnabled(IDisplaySet selectedDisplaySet)
		{
			if (selectedDisplaySet == null || selectedDisplaySet.PresentationImages.Count < 1)
			{
				CopyDisplaySetEnabled = false;
				CopySubsetEnabled = false;
				CopyImageEnabled = false;
			}
			else if (selectedDisplaySet.PresentationImages.Count == 1)
			{
				CopyDisplaySetEnabled = false;
				CopySubsetEnabled = false;
				CopyImageEnabled = true;
			}
			else
			{
				CopyDisplaySetEnabled = true;
				CopySubsetEnabled = true;
				CopyImageEnabled = true;
			}
		}
	}
}
