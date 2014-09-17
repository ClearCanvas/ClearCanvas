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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuMergeOrders", "Apply")]
	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuMergeOrders", "Apply")]
	[Tooltip("apply", "TooltipMergeOrders")]
	[IconSet("apply", "MergeOrdersSmall.png", "MergeOrdersMedium.png", "MergeOrdersLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", Application.Common.AuthorityTokens.Workflow.Order.Merge)]
	public abstract class MergeOrdersToolBase<TItem, TContext> : WorkflowItemTool<TItem, TContext>
		where TItem : WorklistItemSummaryBase
		where TContext : IWorkflowItemToolContext<TItem>
	{
		protected MergeOrdersToolBase()
			: base("MergeOrder")
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
				if(DowntimeRecovery.InDowntimeRecoveryMode)
					return false;

				if (this.Context.SelectedItems.Count < 2)
					return false;

				var list = new List<TItem>(this.Context.SelectedItems);

				// Obvious cases where merging should not be allowed.
				// Cannot merge the same order.  If Unique list has the same number as the original list, every item in the list is unique.
				var sameOrder = CollectionUtils.TrueForAll(list, item => item.AccessionNumber.Equals(list[0].AccessionNumber));
				if (sameOrder)
					return false;

				// Cannot merge orders from different patient
				var differentPatient = CollectionUtils.Contains(list, item => !item.PatientRef.Equals(list[0].PatientRef, true));
				if (differentPatient)
					return false;

				// Return true, let the server decide how to inform user of more complicated error.
				return true;
			}
		}

		protected bool ExecuteCore(WorklistItemSummaryBase item)
		{
			var list = new List<TItem>(this.Context.SelectedItems);
			var orderRefs = CollectionUtils.Map<TItem, EntityRef>(list, x => x.OrderRef);
			var component = new MergeOrdersComponent(orderRefs);

			string failureReason;
			if (!ValidateMergeRequest(orderRefs, out failureReason))
			{
				this.Context.DesktopWindow.ShowMessageBox(failureReason, MessageBoxActions.Ok);
				return false;
			}

			var args = new DialogBoxCreationArgs(component, SR.TitleMergeOrders, null) {AllowUserResize = true};
			if (ApplicationComponentExitCode.Accepted != ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, args))
				return false;

			InvalidateFolders();

			return true;
		}

		private static bool ValidateMergeRequest(IList<EntityRef> orderRefs, out string failureReason)
		{
			var destinationOrderRef = orderRefs[0];
			var sourceOrderRefs = new List<EntityRef>(orderRefs);
			sourceOrderRefs.Remove(destinationOrderRef);

			try
			{
				Platform.GetService(
					delegate(IOrderEntryService service)
					{
						var request = new MergeOrderRequest(sourceOrderRefs, destinationOrderRef) { ValidationOnly = true };
						service.MergeOrder(request);
					});
				failureReason = null;
				return true;
			}
			catch (RequestValidationException e)
			{
				failureReason = e.Message;
				return false;
			}
		}
	}

	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	public class RegistrationMergeOrdersTool : MergeOrdersToolBase<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
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
	public class PerformingMergeOrdersTool : MergeOrdersToolBase<ModalityWorklistItemSummary, IPerformingWorkflowItemToolContext>
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
