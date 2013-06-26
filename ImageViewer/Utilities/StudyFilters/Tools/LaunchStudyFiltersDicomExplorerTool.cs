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
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	[ButtonAction("Open", "dicomstudybrowser-toolbar/ToolbarFilterStudy", "Open")]
	[MenuAction("Open", "dicomstudybrowser-contextmenu/MenuFilterStudy", "Open")]
	[EnabledStateObserver("Open", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("Open", "Visible", "VisibleChanged")]
	[Tooltip("Open", "TooltipFilterStudy")]
	[IconSet("Open", "Icons.StudyFilterToolSmall.png", "Icons.StudyFilterToolMedium.png", "Icons.StudyFilterToolLarge.png")]
	[ViewerActionPermission("Open", AuthorityTokens.StudyFilters)]
	[ExtensionOf(typeof (StudyBrowserToolExtensionPoint))]
	public class LaunchStudyFiltersDicomExplorerTool : StudyBrowserTool
	{
		public void Open()
		{
			var studyLoaders = new List<IStudyLoader>();
			int sopCount = 0;
			foreach (var studyItem in base.Context.SelectedStudies)
			{
				if (!studyItem.Server.IsSupported<IStudyLoader>())
					return;

				var loader = studyItem.Server.GetService<IStudyLoader>();
				studyLoaders.Add(loader);
				sopCount += loader.Start(new StudyLoaderArgs(studyItem.StudyInstanceUid, studyItem.Server, new StudyLoaderOptions(true)));
			}

			bool success = false;
			var component = new StudyFilterComponent {BulkOperationsMode = true};
			var task = new BackgroundTask(c =>
			                              	{
			                              		c.ReportProgress(new BackgroundTaskProgress(0, sopCount, SR.MessageLoading));
			                              		if (c.CancelRequested)
			                              			c.Cancel();

			                              		int progress = 0;
			                              		var studyItems = new List<IStudyItem>();
			                              		foreach (IStudyLoader localStudyLoader in studyLoaders)
			                              		{
			                              			Sop sop;
			                              			while ((sop = localStudyLoader.LoadNextSop()) != null)
			                              			{
			                              				try
			                              				{
			                              					studyItems.Add(new SopDataSourceStudyItem(sop));
			                              					c.ReportProgress(new BackgroundTaskProgress(Math.Min(sopCount, ++progress) - 1, sopCount, SR.MessageLoading));
			                              					if (c.CancelRequested)
			                              						c.Cancel();
			                              				}
			                              				finally
			                              				{
			                              					sop.Dispose();
			                              				}
			                              			}
			                              		}

			                              		try
			                              		{
			                              			foreach (var item in studyItems)
			                              				component.Items.Add(item);
			                              			component.Refresh(true);
			                              		}
			                              		catch (Exception)
			                              		{
			                              			foreach (var item in studyItems)
			                              				item.Dispose();
			                              		}

			                              		success = true;
			                              		c.Complete();
			                              	}, true);
			ProgressDialog.Show(task, this.Context.DesktopWindow, true, ProgressBarStyle.Continuous);

			if (success)
			{
				component.BulkOperationsMode = false;
				base.Context.DesktopWindow.Workspaces.AddNew(component, SR.TitleStudyFilters);
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			this.UpdateEnabled();
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			this.UpdateEnabled();
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			base.OnSelectedStudyChanged(sender, e);
			this.UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			// N.B.: Must restrict to local files only, because study filters (and some of its tools) require file paths
			base.Enabled = Context.SelectedStudies.Count > 0
			               && base.Context.SelectedServers.IsLocalServer
			               && base.Context.SelectedServers.AllSupport<IStudyLoader>();

			Visible = Context.SelectedServers.IsLocalServer && Context.SelectedServers.AllSupport<IStudyLoader>();
		}
	}
}