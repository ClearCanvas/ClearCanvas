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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("launch", "folderexplorer-folders-contextmenu/MenuProperties", "Launch")]
	[Tooltip("launch", "TooltipFolderProperties")]
	[EnabledStateObserver("launch", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(FolderExplorerGroupToolExtensionPoint))]
	public class WorklistPropertiesTool : Tool<IFolderExplorerGroupToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		#region API Methods and Properties

		public override void Initialize()
		{
			base.Initialize();

			this.Context.SelectedFolderChanged += delegate
				{
					this.Enabled = this.Context.SelectedFolder != null && this.Context.SelectedFolder is IWorklistFolder;
				};
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		#endregion

		public void Launch()
		{
			try
			{
				WorklistSummaryComponent component;
				if (this.Context.SelectedFolder is FolderTreeNode.ContainerFolder)
				{
					var worklistDetail = new WorklistAdminDetail(null, this.Context.SelectedFolder.Name, "Container folder", null);
					component = new WorklistSummaryComponent(worklistDetail, false);
				}
				else
				{
					var folder = (IWorklistFolder)this.Context.SelectedFolder;

					if (folder.WorklistRef == null)
					{
						var description = folder.Tooltip ?? SR.TitleStaticFolder;
						var worklistDetail = new WorklistAdminDetail(null, folder.Name, description, null);
						component = new WorklistSummaryComponent(worklistDetail, false);
					}
					else
					{
						WorklistAdminDetail worklistDetail = null;
						Platform.GetService<IWorklistAdminService>(service =>
							{
								var response = service.LoadWorklistForEdit(new LoadWorklistForEditRequest(folder.WorklistRef));
								worklistDetail = response.Detail;
							});

						component = new WorklistSummaryComponent(worklistDetail, worklistDetail.IsUserWorklist == false);
					}
				}

				ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, component, SR.TitleWorklistProperties);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
