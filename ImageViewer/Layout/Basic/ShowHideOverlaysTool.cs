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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Tools.Standard;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    [ActionFormerly("dropdown", "ClearCanvas.ImageViewer.Tools.Standard.ShowHideOverlaysTool:dropdown")]
    [DropDownButtonAction("dropdown", "global-toolbars/ToolbarStandard/ToolbarShowHideOverlays", "ToggleAll", "DropDownActionModel", KeyStroke = XKeys.O)]
    [TooltipValueObserver("dropdown", "Tooltip", "TooltipChanged")]
	[GroupHint("dropdown", "Tools.Image.Overlays.Text.ShowHide")]
    [IconSetObserver("dropdown", "IconSet", "IconSetChanged")]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ShowHideOverlaysTool : ImageViewerTool
	{
        private enum ShowHideOption
        {
            ShowSelected,
            HideAll,
            HideUnimportant
        }

        private readonly IconSet _selectedOverlaysVisibleIconSet;
        private readonly IconSet _selectedOverlaysHiddenIconSet;

        private ActionModelRoot _legacyDropDownActionModel;

        static ShowHideOverlaysTool()
        {
            try
            {
                new SelectOverlaysActionViewExtensionPoint().CreateExtension();
                LegacyMode = false;
            }
            catch (NotSupportedException)
            {
                LegacyMode = true;
            }
        }

        public ShowHideOverlaysTool()
        {
            _selectedOverlaysVisibleIconSet = new IconSet("Icons.ShowHideOverlaysToolSmall.png", "Icons.ShowHideOverlaysToolMedium.png", "Icons.ShowHideOverlaysToolLarge.png");
            _selectedOverlaysHiddenIconSet = new UnavailableActionIconSet(_selectedOverlaysVisibleIconSet){GrayMode = true};
            IconSet = _selectedOverlaysVisibleIconSet;
		}

        internal static bool LegacyMode { get; set; }

        internal IImageViewer Viewer { get { return Context.Viewer; } }

		public ActionModelNode DropDownActionModel
		{
			get
			{
                if (LegacyMode)
                    return LegacyDropDownActionModel;
                
                //Take advantage of the fact that the drop-down model is requested each time it's going to be shown.
                var resolver = new ActionResourceResolver(typeof(SelectOverlaysAction));
			    var actionId = typeof (ShowHideOverlaysTool).Namespace + ":selectOverlays";
                var action = new SelectOverlaysAction(this, actionId, new ActionPath("overlays-dropdown/SelectOverlays", resolver), resolver);
                var actionSet = new ActionSet(new[] { action });
				return ActionModelRoot.CreateModel(typeof (ShowHideOverlaysTool).Namespace, "overlays-dropdown", actionSet);
			}
		}


        private ActionModelNode LegacyDropDownActionModel
        {
            get
            {
                if (_legacyDropDownActionModel == null)
                {
                    //Leave the drop-down action model namespace the same so that old action models will continue to work.
                    _legacyDropDownActionModel = ActionModelRoot.CreateModel("ClearCanvas.ImageViewer.Tools.Standard", "overlays-dropdown", this.ImageViewer.ExportedActions);
                }
                return _legacyDropDownActionModel;
            }    
        }

        public IconSet IconSet { get; private set; }

        public event EventHandler IconSetChanged
        {
            add { SelectedOverlaysVisibleChanged += value; }
            remove { SelectedOverlaysVisibleChanged -= value; }
        }

        public string Tooltip
        {
            get
            {
                if (LegacyMode)
                    return SR.TooltipShowHideOverlays;

                return SelectedOverlaysVisible ? SR.TooltipHideOverlays : SR.TooltipShowOverlays;
            }
        }

        public event EventHandler TooltipChanged
        {
            add { SelectedOverlaysVisibleChanged += value; }
            remove { SelectedOverlaysVisibleChanged -= value; }
        }

        public bool SelectedOverlaysVisible
        {
            get { return ReferenceEquals(IconSet, _selectedOverlaysVisibleIconSet); }
        }

        public event EventHandler SelectedOverlaysVisibleChanged;

        public override void Initialize()
        {
            base.Initialize();

            if (!LegacyMode)
            {
                //Don't apply the settings when there's no view, as it conflicts with the behaviour of the "legacy" tool.
                DisplaySetCreationSettings.DefaultInstance.PropertyChanged += SettingsChanged;
                Context.Viewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!LegacyMode)
            {
                DisplaySetCreationSettings.DefaultInstance.PropertyChanged -= SettingsChanged;
                Context.Viewer.EventBroker.DisplaySetChanged -= OnDisplaySetChanged;
            }

            base.Dispose(disposing);
        }

        public void ToggleAll()
        {
            if (LegacyMode)
            {
                LegacyToggleAll();
                return;
            }

            IconSet = SelectedOverlaysVisible ? _selectedOverlaysHiddenIconSet : _selectedOverlaysVisibleIconSet;
            EventsHelper.Fire(SelectedOverlaysVisibleChanged, this, EventArgs.Empty);

            var selectedOverlaysVisible = SelectedOverlaysVisible;
            foreach (var imageBox in base.Context.Viewer.PhysicalWorkspace.ImageBoxes.Where(i => i.DisplaySet != null))
                UpdateVisibility(imageBox.DisplaySet, selectedOverlaysVisible ? ShowHideOption.ShowSelected : ShowHideOption.HideAll);

			Context.Viewer.PhysicalWorkspace.Draw();
		}

        //Taken from the old Show/Hide overlays implementation.
        private void LegacyToggleAll()
        {
			// get the current check state
			bool currentCheckState = true;
			foreach (OverlayToolBase tool in OverlayToolBase.EnumerateTools(this.ImageViewer))
			{
				if (!tool.Checked)
				{
					currentCheckState = false;
					break;
				}
			}

			// invert the check state
			currentCheckState = !currentCheckState;

			// apply new check state to all
			foreach (OverlayToolBase tool in OverlayToolBase.EnumerateTools(this.ImageViewer))
				tool.Checked = currentCheckState;

			this.Context.Viewer.PhysicalWorkspace.Draw();
        }

        private void OnDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
        {
            if (e.NewDisplaySet != null)
                UpdateVisibility(e.NewDisplaySet, SelectedOverlaysVisible ? ShowHideOption.ShowSelected : ShowHideOption.HideUnimportant);
        }

        private static void UpdateVisibility(IDisplaySet displaySet, ShowHideOption option)
        {
            if (displaySet == null)
                return;

            //Have to update all images each time so that even ones that haven't been drawn are correct.
            //That way, even ones that are exported to the clipboard look right.
            switch (option)
            {
                case ShowHideOption.HideUnimportant:
                    foreach (var image in displaySet.PresentationImages)
                        image.GetOverlays().HideUnimportant(false);
                    break;
                case ShowHideOption.ShowSelected:
                    foreach (var image in displaySet.PresentationImages)
                        image.GetOverlays().ShowSelected(false);
                    break;
                case ShowHideOption.HideAll:
                    foreach (var image in displaySet.PresentationImages)
                        image.GetOverlays().HideAll(false);
                    break;
            }
        }

        private void SettingsChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            //OverlayHelper caches the modality defaults in the viewer extension data for efficiency.
            OverlayHelper.OverlaySettingsChanged(Context.Viewer);
        }
	}
}