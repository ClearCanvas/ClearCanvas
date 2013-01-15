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
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Configuration
{
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class CustomizeViewerActionModelTool : ImageViewerTool
    {
        private class ContextMenuAction : MenuAction
        {
            private bool _realAvailable;
            private MenuAction _mainMenuAction;

            public ContextMenuAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resourceResolver) 
                : base(actionID, path, flags, resourceResolver)
            {
            }

            public override bool Available
            {
                get { return base.Available; }
                set
                {
                    //This property is only set from outside, so this is the "real" value.
                    _realAvailable = value;
                    UpdateAvailable();
                }
            }

            private bool MainMenuActionShowing
            {
                get { return _mainMenuAction.Available && _mainMenuAction.Visible; }
            }

            public void Initialize(MenuAction mainMenuAction)
            {
                _mainMenuAction = mainMenuAction;
                base.Available = _realAvailable = false;

                mainMenuAction.AvailableChanged += UpdateAvailable;
                mainMenuAction.VisibleChanged += UpdateAvailable;
            }

            private void UpdateAvailable(object sender, EventArgs eventArgs)
            {
                UpdateAvailable();
            }

            private void UpdateAvailable()
            {
                //Set Available to whatever it actually should be, which
                //is the "real" value, except when the main menu item isn't showing.
                base.Available = Visible = _realAvailable || !MainMenuActionShowing;
            }
        }

        private const string _groupHint = "Application.Options.Customize";

	    internal const string _mainMenuCustomizeId = "customize";
        internal const string _contextMenuCustomizeId = "customizeContextMenu";

        public override IActionSet Actions
        {
            get
            {
                var baseActions = base.Actions;
                if (baseActions.Count == 0)
                    base.Actions = CreateActions();
                return base.Actions;
            }
            protected set
            {
                base.Actions = value;
            }
        }

        private IActionSet CreateActions()
        {
            var toolType = typeof (CustomizeViewerActionModelTool);
            var resolver = new ActionResourceResolver(toolType);

            var idPrefix = toolType.FullName + ":";
            var mainMenuAction = new MenuAction(idPrefix + _mainMenuCustomizeId,
                                                new ActionPath("global-menus/MenuTools/MenuCustomizeActionModels", resolver),
                                                ClickActionFlags.None, resolver)
                                     {
                                         GroupHint = new GroupHint(_groupHint),
                                         Label = SR.MenuCustomizeActionModels,
                                         Persistent = true
                                     };
            mainMenuAction.SetClickHandler(Customize);

            var contextMenuAction = new ContextMenuAction(idPrefix + _contextMenuCustomizeId,
                                                   new ActionPath(ImageViewerComponent.ContextMenuSite +"/MenuCustomizeActionModels", resolver),
                                                   ClickActionFlags.None, resolver)
                                        {
                                            GroupHint = new GroupHint(_groupHint),
                                            Label = SR.MenuCustomizeActionModels,
                                            Persistent = true
                                        };

            contextMenuAction.SetClickHandler(Customize);
            contextMenuAction.Initialize(mainMenuAction);

            return new ActionSet(new[] {mainMenuAction, contextMenuAction});
        }

		public void Customize()
		{
			try
			{
				if (SettingsStore.IsSupported && !SettingsStore.IsStoreOnline)
				{
					base.Context.DesktopWindow.ShowMessageBox(Desktop.SR.MessageSettingsStoreOffline, MessageBoxActions.Ok);
					return;
				}

				CustomizeViewerActionModelsComponent component = new CustomizeViewerActionModelsComponent(this.ImageViewer);

				DialogBoxCreationArgs args = new DialogBoxCreationArgs(component, SR.TitleCustomizeActionModels, "CustomizeActionModels")
				                             	{
				                             		AllowUserResize = true
				                             	};
				ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, args);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.MessageActionModelUpdateFailure, Context.DesktopWindow);
			}
		}
	}
}
