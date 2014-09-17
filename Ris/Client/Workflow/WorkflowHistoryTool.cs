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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	//NOTE: DEBUG'd out because there is no requirement for this feature, however it is a useful debugging tool (JR)
#if DEBUG
	[ButtonAction("view", "folderexplorer-items-toolbar/MenuWorkflowHistory", "View")]
	[MenuAction("view", "folderexplorer-items-contextmenu/MenuWorkflowHistory", "View")]
	[ButtonAction("view", "patientsearch-items-toolbar/MenuWorkflowHistory", "View")]
	[MenuAction("view", "patientsearch-items-contextmenu/MenuWorkflowHistory", "View")]
#endif
	[EnabledStateObserver("view", "Enabled", "EnabledChanged")]
	[Tooltip("view", "TooltipWorkflowHistory")]
	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
	public class WorkflowHistoryTool : Tool<IWorkflowItemToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		public override void Initialize()
		{
			base.Initialize();

			this.Context.SelectionChanged += delegate
			{
				this.Enabled = DetermineEnablement();
			};
		}

		private bool DetermineEnablement()
		{
			return this.Context.Selection != null && this.Context.Selection.Items.Length == 1;
		}

		public bool Enabled
		{
			get
			{
				this.Enabled = DetermineEnablement();
				return _enabled;
			}
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

		public void View()
		{
			WorklistItemSummaryBase item = (WorklistItemSummaryBase) this.Context.Selection.Item;
			Open(item.OrderRef, this.Context.DesktopWindow);
		}

		protected static void Open(EntityRef orderRef, IDesktopWindow window)
		{
			ApplicationComponent.LaunchAsDialog(
				window,
				new WorkflowHistoryComponent(orderRef),
				SR.TitleWorkflowHistory);
		}
	}
}
