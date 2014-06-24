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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer
{
	internal sealed class ViewerShortcutManager : IViewerShortcutManager
	{
		private readonly Dictionary<ITool, ITool> _tools;
		private readonly ImageViewerComponent _viewer;

        private const string _globalMenusActionSite = "global-menus";
        private const string _globalToolbarActionSite = "global-toolbars";

	    private static readonly string[] _defaultSites =
	    {
	        _globalMenusActionSite, _globalToolbarActionSite,
	        ImageViewerComponent.KeyboardSite, ImageViewerComponent.ContextMenuSite
	    };

		public ViewerShortcutManager(ImageViewerComponent viewer)
		{
			_tools = new Dictionary<ITool, ITool>();
			_viewer = viewer;
		}

		private void RegisterMouseTool(MouseImageViewerTool mouseTool)
		{
			if (mouseTool.Active)
				ActivateMouseTool(mouseTool);

			mouseTool.MouseButtonChanged += OnMouseToolMouseButtonChanged;
			mouseTool.ActivationChanged += OnMouseToolActivationChanged;
		}

        private IEnumerable<MouseImageViewerTool> GetMouseTools()
        {
            return _tools.Keys.OfType<MouseImageViewerTool>();
        }

        private MouseImageViewerTool GetDefaultMouseTool(XMouseButtons mouseButton)
        {
            return GetMouseTools().FirstOrDefault(t => t.InitiallyActive && t.MouseButton == mouseButton);
        }
        
        private MouseImageViewerTool GetActiveMouseTool(XMouseButtons mouseButton)
		{
            return GetMouseTools().FirstOrDefault(t => t.Active && t.MouseButton == mouseButton);
		}

        private IMouseButtonHandler GetActiveMouseTool(MouseButtonShortcut shortcut)
        {
            if (shortcut == null)
                return null;

            return (from mouseTool in GetMouseTools()
                    where mouseTool.Active && shortcut.Equals(mouseTool.MouseButton)
                    select mouseTool).FirstOrDefault();
        }

        private IEnumerable<IMouseButtonHandler> GetDefaultMouseTools(MouseButtonShortcut shortcut)
        {
            return (from mouseTool in GetMouseTools()
                    where shortcut.Equals(mouseTool.DefaultMouseButtonShortcut)
                    select mouseTool).Cast<IMouseButtonHandler>();
        }
        
        private void DeactivateMouseTools(MouseImageViewerTool activating)
		{
		    var others = from tool in GetMouseTools()
		                     where tool != activating && tool.MouseButton == activating.MouseButton
		                     select tool;

            foreach (var otherTool in others)
                otherTool.Active = false;
		}

		private void ActivateMouseTool(MouseImageViewerTool mouseTool)
		{
			if (mouseTool.MouseButton == XMouseButtons.None)
			{
				Platform.Log(LogLevel.Debug, String.Format(SR.FormatMouseToolHasNoAssignment, mouseTool.GetType().FullName));
				mouseTool.Active = false;
			}
            else
			{
			    DeactivateMouseTools(mouseTool);
                mouseTool.Active = true;
            }
		}

		private void OnMouseToolActivationChanged(object sender, EventArgs e)
		{
			var mouseTool = (MouseImageViewerTool)sender;

			if (mouseTool.Active)
			{
			    ActivateMouseTool(mouseTool);
			}
            else
			{
			    var activeMouseTool = GetActiveMouseTool(mouseTool.MouseButton);
			    if (activeMouseTool != null)
                    return;

			    var defaultMouseTool = GetDefaultMouseTool(mouseTool.MouseButton);
			    if (defaultMouseTool != null)
			        ActivateMouseTool(defaultMouseTool);
			}
		}

		private void OnMouseToolMouseButtonChanged(object sender, EventArgs e)
		{
			var mouseTool = (MouseImageViewerTool)sender;

			if (mouseTool.Active)
				ActivateMouseTool(mouseTool);
		}

		/// <summary>
		/// Registers the tool with the viewer shortcut manager.
		/// </summary>
		/// <param name="tool">the tool to register.</param>
		public void RegisterImageViewerTool(ITool tool)
		{
			Platform.CheckForNullReference(tool, "tool");

            _tools[tool] = tool;

			if (tool is MouseImageViewerTool)
				RegisterMouseTool((MouseImageViewerTool)tool);
		}

	    #region IViewerShortcutManager Members

		public IEnumerable<IMouseButtonHandler> GetMouseButtonHandlers(MouseButtonShortcut shortcut)
		{
            if (shortcut == null)
                yield break;
		    
			var activeMouseTool = GetActiveMouseTool(shortcut);
            if (activeMouseTool != null)
                yield return activeMouseTool;

            foreach (var defaultMouseTool in GetDefaultMouseTools(shortcut))
                yield return defaultMouseTool;
		}

		/// <summary>
		/// Gets the <see cref="IMouseWheelHandler"/> associated with a shortcut.
		/// </summary>
		/// <param name="shortcut">The shortcut for which an <see cref="IMouseWheelHandler"/> is to be found.</param>
		/// <returns>An <see cref="IMouseWheelHandler"/> or null.</returns>
		public IMouseWheelHandler GetMouseWheelHandler(MouseWheelShortcut shortcut)
		{
			if (shortcut == null)
				return null;

            return (from mouseTool in GetMouseTools()
                        where shortcut.Equals(mouseTool.MouseWheelShortcut)
                    select mouseTool).FirstOrDefault();
        }

	    /// <summary>
	    /// Gets the <see cref="IClickAction"/> associated with a shortcut.
	    /// </summary>
	    /// <param name="shortcut">The shortcut for which an <see cref="IClickAction"/> is to be found.</param>
	    /// <returns>An <see cref="IClickAction"/> or null.</returns>
	    public IClickAction GetKeyboardAction(KeyboardButtonShortcut shortcut)
	    {
	        if (shortcut == null || shortcut.IsEmpty)
	            return null;

	        var actions = (IActionSet) new ActionSet();
	        foreach (var tool in _tools.Keys)
	            actions = actions.Union(tool.Actions);

	        var clickActions = actions.OfType<IClickAction>().ToList();
	        // precedence is given to actions on the viewer keyboard site
	        return ((FindClickAction(shortcut, ImageViewerComponent.KeyboardSite, clickActions)
	                    // if not defined, give consideration to the viewer context menu
                        ?? FindClickAction(shortcut, ImageViewerComponent.ContextMenuSite, clickActions))
	                    // if still not found, search the global toolbars
	                    ?? FindClickAction(shortcut, _globalToolbarActionSite, clickActions))
	                    // then the global menus
	                    ?? FindClickAction(shortcut, _globalMenusActionSite, clickActions)
	                    // and finally any other site in our toolset that hasn't already been covered
	                    //TODO (CR Orion): do we even have to order by site? Free-for-all is probably ok at this point.
	                    ?? clickActions.Where(a => !_defaultSites.Contains(a.Path.Site))
                            .OrderBy(a => a.Path.Site)
	                        .FirstOrDefault(a => shortcut.Equals(a.KeyStroke));
	    }

	    private static IClickAction FindClickAction(KeyboardButtonShortcut shortcut, string site, IEnumerable<IClickAction> actionSet)
		{
            //This used to use the ActionModel to create them in the right order, but a properly configured action model
            //shouldn't have repeated keyboard shortcuts, at least not within the same site.
            return actionSet.FirstOrDefault(a => a.Path.Site == site && shortcut.Equals(a.KeyStroke));
		}

	    #endregion
	}
}
