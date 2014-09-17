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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Defines an interface for providing custom pages to be displayed in the merge orders component.
	/// </summary>
	public interface IMergeOrdersPageProvider : IExtensionPageProvider<IMergeOrdersPage, IMergeOrdersContext>
	{
	}

	/// <summary>
	/// Defines an interface to a custom merge orders page.
	/// </summary>
	public interface IMergeOrdersPage : IExtensionPage
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom page with access to the merge orders context.
	/// </summary>
	public interface IMergeOrdersContext
	{
		event EventHandler DryRunMergedOrderChanged;

		OrderDetail DryRunMergedOrder { get; }
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the merge orders component.
	/// </summary>
	[ExtensionPoint]
	public class MergeOrdersPageProviderExtensionPoint : ExtensionPoint<IMergeOrdersPageProvider>
	{
	}

	/// <summary>
	/// Extension point for views onto <see cref="MergeOrdersComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class MergeOrdersComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// MergeOrdersComponent class.
	/// </summary>
	[AssociateView(typeof(MergeOrdersComponentViewExtensionPoint))]
	public class MergeOrdersComponent : ApplicationComponent
	{
		class MergeOrdersTable : Table<OrderDetail>
		{
			public MergeOrdersTable()
			{
				ITableColumn accesionNumberColumn;
				this.Columns.Add(accesionNumberColumn = new TableColumn<OrderDetail, string>(SR.ColumnAccessionNumber, o => AccessionFormat.Format(o.AccessionNumber), 0.25f));
				this.Columns.Add(new TableColumn<OrderDetail, string>(SR.ColumnImagingService, o => o.DiagnosticService.Name, 0.75f));

				this.Sort(new TableSortParams(accesionNumberColumn, true));
			}
		}

		class MergeOrdersContext : IMergeOrdersContext
		{
			private readonly MergeOrdersComponent _owner;

			public MergeOrdersContext(MergeOrdersComponent owner)
			{
				_owner = owner;
			}

			public event EventHandler DryRunMergedOrderChanged;

			public OrderDetail DryRunMergedOrder
			{
				get { return _owner._dryRunMergedOrder; }
			}

			internal void NotifyDryRunMergedOrderChanged()
			{
				EventsHelper.Fire(DryRunMergedOrderChanged, this, EventArgs.Empty);
			}
		}

		private readonly List<EntityRef> _orderRefs;
		private readonly MergeOrdersTable _ordersTable;
		private OrderDetail _selectedOrder;
		private OrderDetail _dryRunMergedOrder;

		private TabComponentContainer _mergedOrderViewComponentContainer;
		private ChildComponentHost _mergedOrderPreviewComponentHost;

		private MergedOrderDetailViewComponent _orderPreviewComponent;
		private AttachedDocumentPreviewComponent _attachmentSummaryComponent;

		private readonly List<IMergeOrdersPage> _extensionPages = new List<IMergeOrdersPage>();
		private readonly MergeOrdersContext _extensionPageContext;

		public MergeOrdersComponent(List<EntityRef> orderRefs)
		{
			_orderRefs = orderRefs;
			_ordersTable = new MergeOrdersTable();
			_extensionPageContext = new MergeOrdersContext(this);
		}

		public override void Start()
		{
			_mergedOrderViewComponentContainer = new TabComponentContainer();
			_mergedOrderPreviewComponentHost = new ChildComponentHost(this.Host, _mergedOrderViewComponentContainer);
			_mergedOrderPreviewComponentHost.StartComponent();

			_mergedOrderViewComponentContainer.Pages.Add(new TabPage(SR.TitleOrder, _orderPreviewComponent = new MergedOrderDetailViewComponent()));
			_mergedOrderViewComponentContainer.Pages.Add(new TabPage(SR.TitleOrderAttachments, _attachmentSummaryComponent = new AttachedDocumentPreviewComponent(true, AttachmentSite.Order)));

			// instantiate all extension pages
			foreach (IMergeOrdersPageProvider pageProvider in new MergeOrdersPageProviderExtensionPoint().CreateExtensions())
			{
				_extensionPages.AddRange(pageProvider.GetPages(_extensionPageContext));
			}

			// add extension pages to container and set initial context
			// the container will start those components if the user goes to that page
			foreach (var page in _extensionPages)
			{
				_mergedOrderViewComponentContainer.Pages.Add(new TabPage(page.Path, page.GetComponent()));
			}

			// Load form data
			Platform.GetService(
				delegate(IBrowsePatientDataService service)
				{
					var request = new GetDataRequest { GetOrderDetailRequest = new GetOrderDetailRequest() };

					foreach (var orderRef in _orderRefs)
					{
						request.GetOrderDetailRequest.OrderRef = orderRef;
						var response = service.GetData(request);
						_ordersTable.Items.Add(response.GetOrderDetailResponse.Order);
					}
				});

			_ordersTable.Sort();

			// Re-populate orderRef list by sorted accession number
			_orderRefs.Clear();
			_orderRefs.AddRange(CollectionUtils.Map<OrderDetail, EntityRef>(_ordersTable.Items, item => item.OrderRef));

			_selectedOrder = CollectionUtils.FirstElement(_ordersTable.Items);
			DryRunForSelectedOrder();

			base.Start();
		}

		public override void Stop()
		{
			if (_mergedOrderPreviewComponentHost != null)
			{
				_mergedOrderPreviewComponentHost.StopComponent();
				_mergedOrderPreviewComponentHost = null;
			}

			base.Stop();
		}

		#region Presentation Model

		public ITable OrdersTable
		{
			get { return _ordersTable; }
		}

		public ISelection OrdersTableSelection
		{
			get
			{
				return new Selection(_selectedOrder);
			}
			set
			{
				var previousSelection = new Selection(_selectedOrder);
				if (previousSelection.Equals(value))
					return;

				_selectedOrder = (OrderDetail) value.Item;
				DryRunForSelectedOrder();
				NotifyPropertyChanged("SummarySelection");
			}
		}

		public bool AcceptEnabled
		{
			get
			{
				return _ordersTable.Items.Count > 0
					&& _selectedOrder != null
					&& _dryRunMergedOrder != null;
			}
		}

		public ApplicationComponentHost MergedOrderPreviewComponentHost
		{
			get { return _mergedOrderPreviewComponentHost; }
		}

		public void Accept()
		{
			try
			{
				var destAccNumber = _selectedOrder.AccessionNumber;
				var sourceAccNumbers = CollectionUtils.Map(_ordersTable.Items, (OrderDetail o) => o.AccessionNumber);
				sourceAccNumbers.Remove(destAccNumber);

				var message = string.Format(SR.MessageMergeOrderConfirmation, StringUtilities.Combine(sourceAccNumbers, ","), destAccNumber);
				if (DialogBoxAction.No == this.Host.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo))
					return;

				var destinationOrderRef = _selectedOrder.OrderRef;
				var sourceOrderRefs = new List<EntityRef>(_orderRefs);
				sourceOrderRefs.Remove(_selectedOrder.OrderRef);
				Platform.GetService(
					delegate(IOrderEntryService service)
					{
						var request = new MergeOrderRequest(sourceOrderRefs, destinationOrderRef) { DryRun = false };
						service.MergeOrder(request);
					});
				
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionMergeOrdersTool, this.Host.DesktopWindow,
					() => this.Exit(ApplicationComponentExitCode.Error));
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		private void DryRunForSelectedOrder()
		{
			string failureReason;
			MergeOrderDryRun(out _dryRunMergedOrder, out failureReason);
			if (!string.IsNullOrEmpty(failureReason))
				this.Host.ShowMessageBox(failureReason, MessageBoxActions.Ok);

			// Update order preview components
			if (_dryRunMergedOrder == null)
			{
				_orderPreviewComponent.Context = null;
				_attachmentSummaryComponent.Attachments = new List<AttachmentSummary>();
			}
			else
			{
				_orderPreviewComponent.Context = _dryRunMergedOrder;
				_attachmentSummaryComponent.Attachments = _dryRunMergedOrder.Attachments;
			}

			_extensionPageContext.NotifyDryRunMergedOrderChanged();
		}

		private void MergeOrderDryRun(out OrderDetail mergedOrder, out string failureReason)
		{
			if (_selectedOrder == null)
			{
				failureReason = null;
				mergedOrder = null;
				return;
			}

			var destinationOrderRef = _selectedOrder.OrderRef;
			var sourceOrderRefs = new List<EntityRef>(_orderRefs);
			sourceOrderRefs.Remove(_selectedOrder.OrderRef);

			try
			{
				MergeOrderResponse response = null;
				Platform.GetService(
					delegate(IOrderEntryService service)
					{
						var request = new MergeOrderRequest(sourceOrderRefs, destinationOrderRef) { DryRun = true };
						response = service.MergeOrder(request);
					});

				mergedOrder = response.DryRunMergedOrder;
				failureReason = null;
			}
			catch (RequestValidationException e)
			{
				failureReason = e.Message;
				mergedOrder = null;
			}
		}
	}
}
