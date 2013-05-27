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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[MenuAction("create", "imageviewer-contextmenu/MenuCreateKeyImage", "Create")]
	[ButtonAction("create", "global-toolbars/ToolbarStandard/ToolbarCreateKeyImage", "Create", KeyStroke = XKeys.Space)]
	[Tooltip("create", "TooltipCreateKeyImage")]
	[IconSet("create", "Icons.CreateKeyImageToolSmall.png", "Icons.CreateKeyImageToolMedium.png", "Icons.CreateKeyImageToolLarge.png")]
	[EnabledStateObserver("create", "Enabled", "EnabledChanged")]
	// TODO (CR Phoenix5 - Med): Clinical as well
	[ViewerActionPermission("create", AuthorityTokens.KeyImages)]

	[ButtonAction("show", "global-toolbars/ToolbarStandard/ToolbarShowKeyImages", "Show")]
	[Tooltip("show", "TooltipShowKeyImages")]
	[IconSet("show", "Icons.ShowKeyImagesToolSmall.png", "Icons.ShowKeyImagesToolMedium.png", "Icons.ShowKeyImagesToolLarge.png")]
	[EnabledStateObserver("show", "ShowEnabled", "ShowEnabledChanged")]
    // TODO (CR Phoenix5 - Med): Clinical as well
    [ViewerActionPermission("show", AuthorityTokens.KeyImages)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	internal class KeyImageTool : ImageViewerTool
	{
		#region Private Fields

		private readonly FlashOverlayController _flashOverlayController;
		private bool _showEnabled;
		private bool _firstKeyImageCreation = true;
		private event EventHandler _showEnabledChanged;
		private IWorkItemActivityMonitor _workItemActivityMonitor;

		#endregion

		public KeyImageTool()
		{
			_flashOverlayController = new FlashOverlayController("Icons.CreateKeyImageToolLarge.png", new ApplicationThemeResourceResolver(this.GetType(), false));
		}

		public bool ShowEnabled
		{
			get { return _showEnabled; }
			set
			{
				if (_showEnabled == value)
					return;

				_showEnabled = value;
				EventsHelper.Fire(_showEnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler ShowEnabledChanged
		{
			add { _showEnabledChanged += value; }
			remove { _showEnabledChanged -= value; }
		}
	
		#region Overrides

		public override void Initialize()
		{
			base.Initialize();
			KeyImageClipboard.OnViewerOpened(base.Context.Viewer);

			UpdateEnabled();

            if (WorkItemActivityMonitor.IsSupported)
            {
                _workItemActivityMonitor = WorkItemActivityMonitor.Create();
                _workItemActivityMonitor.IsConnectedChanged += OnIsConnectedChanged;
            }

            if (!KeyImageClipboard.HasViewPlugin)
            {
                foreach (var a in Actions)
                {
                    // TODO (CR Phoenix5 - High): use the ID, which doesn't change; this will change with language.
                    if (a.Path.LocalizedPath == "global-toolbars/ToolbarStandard/Show Key Images")
                    {
                        var buttonAction = a as ButtonAction;
                        if (buttonAction != null)
                            buttonAction.Visible = false;
                    }
                }
            }
		}

	    protected override void Dispose(bool disposing)
		{
            if (_workItemActivityMonitor != null)
            {
                _workItemActivityMonitor.IsConnectedChanged -= OnIsConnectedChanged;
                _workItemActivityMonitor.Dispose();
                _workItemActivityMonitor = null;
            }

	        if (base.Context != null)
	        {
	            KeyImageClipboard.OnViewerClosed(base.Context.Viewer);
	        }

	        base.Dispose(disposing);
		}

		private void UpdateEnabled()
		{
            // TODO  Better way to address Webstation usage?
			base.Enabled = KeyImagePublisher.IsSupportedImage(base.SelectedPresentationImage) &&
                // TODO (CR Phoenix5 - Med): Clinical as well	  
                        PermissionsHelper.IsInRole(AuthorityTokens.KeyImages) &&
			               // TODO (CR Phoenix5 - Low): KeyImagePublisher.IsSupportedImage?
                      !(SelectedPresentationImage.ParentDisplaySet.Descriptor is KeyImageDisplaySetDescriptor) ;

            // TODO (CR Phoenix5 - Med): Clinical as well
            this.ShowEnabled = 
					  PermissionsHelper.IsInRole(AuthorityTokens.KeyImages);
		}

        private void OnIsConnectedChanged(object sender, EventArgs eventArgs)
        {
            UpdateEnabled();
        }

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			UpdateEnabled();

            /*
            if (!KeyImageClipboard.HasViewPlugin())
            {
                if (SelectedPresentationImage.ParentDisplaySet.Descriptor is KeyImageDisplaySetDescriptor)
                {
                    foreach (ClearCanvas.Desktop.Actions.Action a in this.Actions)
                    {
                        // TODO (CR Phoenix5 - High): use the ID, which doesn't change; this will change with language.
                        if (a.Path.LocalizedPath.Equals("imageviewer-contextmenu/MenuCreateKeyImage")
                          | a.Path.LocalizedPath.Equals("global-toolbars/ToolbarStandard/Create Key Image"))
                        {
                            a.IconSet = new IconSet("Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png");
                        }
                    }
                }
                else
                {
                    foreach (ClearCanvas.Desktop.Actions.Action a in this.Actions)
                    {
                        // TODO (CR Phoenix5 - High): use the ID, which doesn't change; this will change with language.
                        if (a.Path.LocalizedPath.Equals("imageviewer-contextmenu/MenuCreateKeyImage")
                         || a.Path.LocalizedPath.Equals("global-toolbars/ToolbarStandard/Create Key Image"))
                        {
                            a.IconSet = new IconSet("Icons.CreateKeyImageToolSmall.png", "Icons.CreateKeyImageToolMedium.png", "Icons.CreateKeyImageToolLarge.png");
                        }
                    }
                }
            }
            */
        }

		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			UpdateEnabled();
		}

		#endregion

		#region Methods

		public void Show()
		{
			if (this.ShowEnabled)
				KeyImageClipboard.Show(Context.DesktopWindow);
		}

		public void Create()
		{
			if (!base.Enabled)
				return;

		    if (KeyImageClipboard.HasViewPlugin)
		    {
		        try
		        {
		            IPresentationImage image = base.Context.Viewer.SelectedPresentationImage;
		            if (image != null)
		            {
		                KeyImageClipboard.Add(image);
		                _flashOverlayController.Flash(image);
		            }

		            if (_firstKeyImageCreation && this.ShowEnabled)
		            {
		                KeyImageClipboard.Show(Context.DesktopWindow,
		                                       ShelfDisplayHint.DockAutoHide | ShelfDisplayHint.DockLeft);
		                _firstKeyImageCreation = false;
		            }
		        }
		        catch (Exception ex)
		        {
		            Platform.Log(LogLevel.Error, ex, "Failed to add item to the key image clipboard.");
		            ExceptionHandler.Report(ex, SR.MessageCreateKeyImageFailed, base.Context.DesktopWindow);
		        }
		    }
		    else
		    {
                try
                {
                    IPresentationImage image = base.Context.Viewer.SelectedPresentationImage;
                    if (image != null)
                    {
                        // New Virtual Display Set
                        // TODO (9-JAN-13) As per Phoenx5, Sprint 4 review, disable creation of the virtual display set in Webstation.
                        //KeyImageDisplaySet.AddKeyImage(image);

                        // Still save to clipboard, makes publishing easier later
                        KeyImageClipboard.Add(image);
		     
                        _flashOverlayController.Flash(image);
                    }
                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Failed to create virtual display set for key image.");
                    ExceptionHandler.Report(ex, SR.MessageCreateKeyImageFailed, base.Context.DesktopWindow);
                }
            }
		}
		
		#endregion
	}
}
