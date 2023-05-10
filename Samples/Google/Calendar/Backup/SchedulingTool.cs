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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Samples.Google.Calendar
{
    [MenuAction("apply", "global-menus/MenuTools/MenuToolsMyTools/SchedulingTool", "Apply")]
    [ButtonAction("apply", "global-toolbars/ToolbarMyTools/SchedulingTool", "Apply")]
    [Tooltip("apply", "SchedulingToolTooltip")]
    [IconSet("apply", IconScheme.Colour, "Icons.SchedulingToolSmall.png", "Icons.SchedulingToolMedium.png", "Icons.SchedulingToolLarge.png")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class SchedulingTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {
        private Shelf _shelf;

        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public SchedulingTool()
        {
        }

        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
            // check if the shelf already exists
            if (_shelf == null)
            {
                // create a new shelf that hosts the SchedulingComponent
                _shelf = ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
                    new SchedulingComponent(),
                    SR.SchedulingTool,
                    ShelfDisplayHint.DockRight|ShelfDisplayHint.DockAutoHide,
                    delegate(IApplicationComponent c)
                    {
                        _shelf = null;  // destroy the shelf when the user closes it
                    });
            }
            else
            {
                // activate existing shelf
                _shelf.Activate();
            }
        }
    }
}
