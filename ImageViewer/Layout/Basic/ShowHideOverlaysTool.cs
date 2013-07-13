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
using Action = ClearCanvas.Desktop.Actions.Action;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    public class SelectOverlaysActionViewExtensionPoint : ExtensionPoint<IActionView>
    {
    }

    [AssociateView(typeof(SelectOverlaysActionViewExtensionPoint))]
    public class SelectOverlaysAction : Action
    {
        public SelectOverlaysAction(IImageViewer viewer, string actionID, ActionPath path, IResourceResolver resourceResolver)
            : base(actionID, path, resourceResolver)
        {

            Overlays = viewer.SelectedImageBox.GetOverlays();
        }

        public IOverlays Overlays { get; private set; }
    }

    [DropDownButtonAction("dropdown", "global-toolbars/ToolbarStandard/ToolbarShowHideOverlays", "ToggleAll", "DropDownActionModel", KeyStroke = XKeys.O)]
    [Tooltip("dropdown", "TooltipShowHideOverlays")]
	[GroupHint("dropdown", "Tools.Image.Overlays.Text.ShowHide")]
    [IconSetObserver("dropdown", "IconSet", "IconSetChanged")]
    [ActionFormerly("dropdown", "ClearCanvas.ImageViewer.Tools.Standard.ShowHideOverlaysTool")]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ShowHideOverlaysTool : ImageViewerTool
	{
	    private ActionModelNode _mainDropDownActionModel;
        private IconSet _selectedOverlaysVisible = new IconSet("Icons.ShowHideOverlaysToolSmall.png", "Icons.ShowHideOverlaysToolMedium.png", "Icons.ShowHideOverlaysToolLarge.png");
        private IconSet _selectedOverlaysHidden = new IconSet("Icons.ShowHideOverlaysToolSmall.png", "Icons.ShowHideOverlaysToolMedium.png", "Icons.ShowHideOverlaysToolLarge.png");

		public ShowHideOverlaysTool()
		{
		    IconSet = _selectedOverlaysVisible;
		}

		public ActionModelNode DropDownActionModel
		{
			get
			{
				if (_mainDropDownActionModel == null)
				{
                    var resolver = new ActionResourceResolver(typeof(SelectOverlaysAction));
				    IAction action = new SelectOverlaysAction(Context.Viewer, "selectOverlays", new ActionPath("overlays-dropdown/SelectOverlays", resolver), resolver);
				    var actionSet = new ActionSet(new[] {action});
				    return ActionModelRoot.CreateModel(typeof (ShowHideOverlaysTool).Namespace, "overlays-dropdown", actionSet);
				}

			    return _mainDropDownActionModel;
			}
		}

        public IconSet IconSet { get; private set; }
        public event EventHandler IconSetChanged;

        public bool SelectedOverlaysVisible
        {
            get { return ReferenceEquals(IconSet, _selectedOverlaysVisible); }
        }

        public override void Initialize()
        {
            base.Initialize();
            Context.Viewer.EventBroker.ImageDrawing += OnImageDrawing;
        }

        protected override void Dispose(bool disposing)
        {
            Context.Viewer.EventBroker.ImageDrawing -= OnImageDrawing;
            base.Dispose(disposing);
        }

        public void ToggleAll()
        {
            IconSet = SelectedOverlaysVisible ? _selectedOverlaysHidden : _selectedOverlaysVisible;
            EventsHelper.Fire(IconSetChanged, this, EventArgs.Empty);

            var show = SelectedOverlaysVisible;
            foreach (var imageBox in base.Context.Viewer.PhysicalWorkspace.ImageBoxes)
            {
                var tile = imageBox.SelectedTile;
                if (tile == null || tile.PresentationImage == null)continue;

                var overlays = tile.PresentationImage.GetOverlays();
                if (show)
                    overlays.ShowSelected(false);
                else
                    overlays.Hide(false);
            }

			Context.Viewer.PhysicalWorkspace.Draw();
		}

        private void OnImageDrawing(object sender, ImageDrawingEventArgs args)
        {
            if (SelectedOverlaysVisible)
                args.PresentationImage.GetOverlays().ShowSelected(false);
            else
                args.PresentationImage.GetOverlays().Hide(false);
        }
	}
}