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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Explorer.Local;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	[MenuAction("Open", "explorerlocal-contextmenu/MenuFilterStudy", "Open")]
	[Tooltip("Open", "TooltipFilterStudy")]
	[IconSet("Open", "Icons.StudyFilterToolSmall.png", "Icons.StudyFilterToolMedium.png", "Icons.StudyFilterToolLarge.png")]
	[EnabledStateObserver("Open", "Enabled", "EnabledChanged")]
	[ViewerActionPermission("Open", AuthorityTokens.StudyFilters)]
	[ExtensionOf(typeof (LocalImageExplorerToolExtensionPoint))]
	public class LaunchStudyFiltersLocalExplorerTool : Tool<ILocalImageExplorerToolContext>
	{
		public event EventHandler EnabledChanged;
		private bool _enabled = true;

		public bool Enabled
		{
			get { return _enabled; }
			private set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(EnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			Context.SelectedPathsChanged += OnContextSelectedPathsChanged;
		}

		protected override void Dispose(bool disposing)
		{
			Context.SelectedPathsChanged -= OnContextSelectedPathsChanged;
			base.Dispose(disposing);
		}

		private void OnContextSelectedPathsChanged(object sender, EventArgs e)
		{
			Enabled = Context.SelectedPaths.Count > 0;
		}

		public void Open()
		{
			List<string> paths = new List<string>(base.Context.SelectedPaths);
			if (paths.Count == 0)
				return;

			StudyFilterComponent component = new StudyFilterComponent();
			component.BulkOperationsMode = true;

			if (component.Load(base.Context.DesktopWindow, true, paths))
			{
				component.Refresh(true);
				base.Context.DesktopWindow.Workspaces.AddNew(component, SR.TitleStudyFilters);
			}

			component.BulkOperationsMode = false;
		}
	}
}