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
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Local
{
	[MenuAction("Import", "explorerlocal-contextmenu/ImportDicomFiles", "Import")]
    [ActionFormerly("Import", "ClearCanvas.ImageViewer.Services.Tools.DicomFileImportTool:Import")]
    [Tooltip("Import", "TooltipImportDicomFiles")]
	[IconSet("Import", "Icons.DicomFileImportToolSmall.png", "Icons.DicomFileImportToolMedium.png", "Icons.DicomFileImportToolLarge.png")]
	[EnabledStateObserver("Import", "Enabled", "EnabledChanged")]
	[ViewerActionPermission("Import", ImageViewer.AuthorityTokens.Study.Import)]

	[ExtensionOf(typeof(LocalImageExplorerToolExtensionPoint))]
	public class DicomFileImportTool : Tool<ILocalImageExplorerToolContext>
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

		public void Import()
		{
			var filePaths = new List<string>();

			foreach (string path in this.Context.SelectedPaths)
			{
				if (string.IsNullOrEmpty(path))
					continue;

				filePaths.Add(path);
			}

			if (filePaths.Count == 0)
				return;
	
			try
			{
			    string linkText = SR.LinkOpenActivityMonitor;   
                var importClient = new DicomFileImportBridge();
                importClient.ImportFileList(filePaths, BadFileBehaviourEnum.Ignore, FileImportBehaviourEnum.Copy);
                Context.DesktopWindow.ShowAlert(AlertLevel.Info, string.Format(filePaths.Count > 1 ? SR.MessageFormatImportingFilesPlural : SR.MessageFormatImportingFiles, filePaths.Count), linkText, ActivityMonitorManager.Show, true);
			}
			catch (EndpointNotFoundException)
			{
			    var message = String.Format(SR.FormatMessageImportWorkItemServiceNotRunning, LocalServiceProcess.Name);
                Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageFailedToImportSelection, this.Context.DesktopWindow);
			}
		}
	}
}
