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
using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Export;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.CoreTools
{
	[DropDownAction("export", DefaultToolbarActionSite + "/ToolbarExport", "DropDownActionModel")]
	[IconSet("export", "Icons.SaveToolSmall.png", "Icons.SaveToolMedium.png", "Icons.SaveToolLarge.png")]
	[EnabledStateObserver("export", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[ViewerActionPermission("export", AuthorityTokens.Study.Anonymize)]
	[ViewerActionPermission("export", AuthorityTokens.Study.Export)]

	[MenuAction("exportAnonymized", DropDownMenuActionSite + "/MenuExportAnonymized", "ExportAnonymized")]
	[MenuAction("exportAnonymized", DefaultContextMenuActionSite + "/MenuExportAnonymized", "ExportAnonymized")]
	[EnabledStateObserver("exportAnonymized", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[ViewerActionPermission("exportCopy", AuthorityTokens.Study.Anonymize)]
	
	[MenuAction("exportCopy", DropDownMenuActionSite + "/MenuExportCopy", "ExportCopy")]
	[MenuAction("exportCopy", DefaultContextMenuActionSite + "/MenuExportCopy", "ExportCopy")]
	[EnabledStateObserver("exportCopy", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[ViewerActionPermission("exportCopy", AuthorityTokens.Study.Export)]
	
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class ExportTool : StudyFilterTool
	{
		public const string DropDownMenuActionSite = "studyfilters-exportdropdown";

		private string _lastExportCopyFolder = string.Empty;
		private string _lastExportAnonymizedFolder = string.Empty;

		public ActionModelNode DropDownActionModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, DropDownMenuActionSite, this.Actions); }
		}

		public void ExportAnonymized()
		{
			if (base.SelectedItems.Count == 0)
				return;

			try
			{
				List<string> files = CollectionUtils.Map(base.SelectedItems, (IStudyItem item) => item.Filename);
				DicomFileExporter exporter = new DicomFileExporter(files)
				                        	{
				                        		DesktopWindow = Context.DesktopWindow,
				                        		OutputPath = _lastExportAnonymizedFolder, 
												Anonymize = true
				                        	};
				
				bool success = exporter.Export();
				_lastExportAnonymizedFolder = exporter.OutputPath;
				
				if (success)
					Process.Start(_lastExportAnonymizedFolder);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, base.DesktopWindow);
			}
		}

		public void ExportCopy()
		{
			if (base.SelectedItems.Count == 0)
				return;

			try
			{
				List<string> files = CollectionUtils.Map(base.SelectedItems, (IStudyItem item) => item.Filename);
				DicomFileExporter exporter = new DicomFileExporter(files) { DesktopWindow = Context.DesktopWindow, OutputPath = _lastExportCopyFolder };

				bool success = exporter.Export();
				_lastExportCopyFolder = exporter.OutputPath;
				if (success)
					Process.Start(_lastExportCopyFolder);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, base.DesktopWindow);
			}
		}
	}
}