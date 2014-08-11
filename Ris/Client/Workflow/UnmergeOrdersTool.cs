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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuUndoMergeOrders", "Apply")]
	[Tooltip("apply", "TooltipUndoMergeOrders")]
	[IconSet("apply", "UnmergeOrdersSmall.png", "UnmergeOrdersMedium.png", "UnmergeOrdersLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", Application.Common.AuthorityTokens.Workflow.Order.Unmerge)]
	public abstract class UnmergeOrdersToolBase<TItem, TContext> : WorkflowItemTool<TItem, TContext>
		where TItem : WorklistItemSummaryBase
		where TContext : IWorkflowItemToolContext<TItem>
	{
		protected UnmergeOrdersToolBase()
			: base("UnmergeOrder")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IOrderEntryService));
		}

		protected abstract void InvalidateFolders();

		public override bool Enabled
		{
			get
			{
				if (DowntimeRecovery.InDowntimeRecoveryMode)
					return false;

				// we can tolerate a multi-select, as long as all selected items have the same accession number
				var accNumbers = CollectionUtils.Unique(CollectionUtils.Map(this.Context.SelectedItems, (TItem item) => item.AccessionNumber));
				if(accNumbers.Count != 1)
					return false;

				return this.Context.GetOperationEnablement("UnmergeOrder");
			}
		}

		protected bool ExecuteCore(WorklistItemSummaryBase item)
		{
			EnumValueInfo reason;
			var reasonCode = OrderMergeSettings.Default.UnmergeDefaultReasonCode;
			if(string.IsNullOrEmpty(reasonCode))
			{
				var unmergeComponent = new UnmergeOrderComponent();
				var exitCode = ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					unmergeComponent,
					string.Format(SR.MessageUndoMergeOrder, AccessionFormat.Format(item.AccessionNumber)));

				if (exitCode != ApplicationComponentExitCode.Accepted)
					return false;

				reason = unmergeComponent.SelectedReason;
			}
			else
			{
				// confirm
				var message = string.Format(SR.MessageUnmergeOrderConfirmation, item.AccessionNumber);
				if (DialogBoxAction.No == this.Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo))
					return false;
				reason = new EnumValueInfo(reasonCode, null, null);
			}

			Platform.GetService(
				delegate(IOrderEntryService service)
				{
					var request = new UnmergeOrderRequest(item.OrderRef) {UnmergeReason = reason};
					service.UnmergeOrder(request);
				});

			InvalidateFolders();
			return true;
		}
	}

	// [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))] // Disabled due to bug #12488
	public class RegistrationUnmergeOrdersTool : UnmergeOrdersToolBase<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
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

	// [ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))] // Disabled due to bug #12488
	public class PerformingUnmergeOrdersTool : UnmergeOrdersToolBase<ModalityWorklistItemSummary, IPerformingWorkflowItemToolContext>
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
