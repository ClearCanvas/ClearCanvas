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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuCancelAndReplaceOrder", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuCancelAndReplaceOrder", "Apply")]
	[IconSet("apply", "ReplaceOrderSmall.png", "ReplaceOrderMedium.png", "ReplaceOrderLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", Application.Common.AuthorityTokens.Workflow.Order.Replace)]
	public abstract class ReplaceOrderToolBase<TItem, TContext> : WorkflowItemTool<TItem, TContext>
		where TItem : WorklistItemSummaryBase
		where TContext : IWorkflowItemToolContext<TItem>

	{
		protected ReplaceOrderToolBase()
			: base("ReplaceOrder")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IOrderEntryService));
		}

		protected abstract void InvalidateFolders();

		protected bool ExecuteCore(WorklistItemSummaryBase item)
		{
			// first check for warnings
			var warnings = new List<string>();
			Platform.GetService<IOrderEntryService>(
				service => warnings = service.QueryCancelOrderWarnings(new QueryCancelOrderWarningsRequest(item.OrderRef)).Warnings);

			if(warnings.Count > 0)
			{
				var warn = CollectionUtils.FirstElement(warnings);
				var action = this.Context.DesktopWindow.ShowMessageBox(
					string.Format(SR.FormatMessageConfirmReplaceOrder, warn),
					MessageBoxActions.YesNo);
				if(action == DialogBoxAction.No)
					return false;
			}

			var title = string.Format(SR.FormatTitleReplaceOrder,
				AccessionFormat.Format(item.AccessionNumber),
				PersonNameFormat.Format(item.PatientName),
				MrnFormat.Format(item.Mrn));
			var component = new OrderEditorComponent(new OrderEditorComponent.ReplaceOrderOperatingContext { OrderRef = item.OrderRef });
			var result = ApplicationComponent.LaunchAsDialog(
				this.Context.DesktopWindow,
				component,
				title);

			if(result == ApplicationComponentExitCode.Accepted)
			{
				InvalidateFolders();
				return true;
			}

			return false;
		}
	}

	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	public class RegistrationReplaceOrderTool : ReplaceOrderToolBase<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
	{
		protected override bool Execute(RegistrationWorklistItemSummary item)
		{
			return ExecuteCore(item);
		}

		protected override void InvalidateFolders()
		{
			DocumentManager.InvalidateFolder(typeof(Folders.Registration.ScheduledFolder));
			DocumentManager.InvalidateFolder(typeof(Folders.Registration.CancelledFolder));
		}
	}

	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	public class PerformingReplaceOrderTool : ReplaceOrderToolBase<ModalityWorklistItemSummary, IPerformingWorkflowItemToolContext>
	{
		protected override bool Execute(ModalityWorklistItemSummary item)
		{
			return ExecuteCore(item);
		}

		protected override void InvalidateFolders()
		{
			DocumentManager.InvalidateFolder(typeof(Folders.Performing.ScheduledFolder));
			DocumentManager.InvalidateFolder(typeof(Folders.Performing.CancelledFolder));
		}
	}
}
