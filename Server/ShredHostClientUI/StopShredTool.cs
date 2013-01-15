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

namespace ClearCanvas.Server.ShredHostClientUI
{
    [MenuAction("apply", "shredhostclient-contextmenu/Stop")]
    [ButtonAction("apply", "shredhostclient-toolbar/Stop")]
    [Tooltip("apply", "Stop the running shred")]
    [IconSet("apply", IconScheme.Colour, "Icons.StopShredToolSmall.png", "Icons.StopShredToolSmall.png", "Icons.StopShredToolSmall.png")]
    [ClickHandler("apply", "StopShred")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(ShredHostClientToolExtensionPoint))]
    public class StopShredTool : Tool<IShredHostClientToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public StopShredTool()
        {
            _enabled = true;
        }

        /// <summary>
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // TODO: add any significant initialization code here rather than in the constructor
        }

        /// <summary>
        /// Called to determine whether this tool is enabled/disabled in the UI.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            protected set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Notifies that the Enabled state of this tool has changed.
        /// </summary>
        public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }

        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void StopShred()
        {
            // TODO
            // Add code here to implement the functionality of the tool
            // If this tool is associated with a workspace, you can access the workspace
            // using the Workspace property

            if (null != this.Context.SelectedShred)
            {
                this.Context.StopShred(this.Context.SelectedShred);
            }
        }
    }
}
