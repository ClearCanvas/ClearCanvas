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

namespace ClearCanvas.Desktop.Applets.WebBrowser
{
	[MenuAction("apply", "global-menus/MenuTools/MenuStandard/MenuWebBrowser", "Apply")]
	[ButtonAction("apply", "global-toolbars/ToolbarStandard/ToolbarWebBrowser", "Apply")]
	[Tooltip("apply", "TooltipWebBrowser")]
	[IconSet("apply", "Icons.WebBrowserToolSmall.png", "Icons.WebBrowserToolMedium.png", "Icons.WebBrowserToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	//
	[ExtensionOf(typeof (DesktopToolExtensionPoint))]
	public class WebBrowserTool : Tool<IDesktopToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public WebBrowserTool()
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
		public void Apply()
		{
			// TODO
			// Add code here to implement the functionality of the tool
			// If this tool is associated with a workspace, you can access the workspace
			// using the Workspace property

			WebBrowserComponent component = new WebBrowserComponent();

			ApplicationComponent.LaunchAsWorkspace(this.Context.DesktopWindow, component, SR.WorkspaceName);
		}
	}
}