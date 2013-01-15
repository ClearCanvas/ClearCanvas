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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="CancelOrderComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class CancelOrderComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// CancelOrderComponent class
	/// </summary>
	[AssociateView(typeof(CancelOrderComponentViewExtensionPoint))]
	public class CancelOrderComponent : ApplicationComponent
	{
		private readonly EntityRef _orderRef;
		private readonly ProcedureRequisitionTable _proceduresTable;
		private EnumValueInfo _selectedCancelReason;
		private List<EnumValueInfo> _cancelReasonChoices;

		/// <summary>
		/// Constructor
		/// </summary>
		public CancelOrderComponent(EntityRef orderRef)
		{
			_orderRef = orderRef;
			_proceduresTable = new ProcedureRequisitionTable();
			_proceduresTable.ScheduledDurationColumn.Visible = false;
			_proceduresTable.ModalityColumn.Visible = false;
		}

		public override void Start()
		{
			Platform.GetService<IOrderEntryService>(
				service =>
				{
					_cancelReasonChoices = service.GetCancelOrderFormData(new GetCancelOrderFormDataRequest()).CancelReasonChoices;
					var orderReq = service.GetOrderRequisitionForEdit(new GetOrderRequisitionForEditRequest() {OrderRef = _orderRef}).Requisition;
					_proceduresTable.Items.AddRange(orderReq.Procedures.Where(pr => !pr.Cancelled));
				});

			base.Start();
		}

		#region Presentation Model

		public ITable ProceduresTable
		{
			get { return _proceduresTable; }
		}

		public IList CancelReasonChoices
		{
			get { return _cancelReasonChoices; }
		}

		public EnumValueInfo SelectedCancelReason
		{
			get { return _selectedCancelReason; }
			set { _selectedCancelReason = value; }
		}

		#endregion

		public void Accept()
		{
			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		public bool AcceptEnabled
		{
			get { return _selectedCancelReason != null; }
		}
	}
}
