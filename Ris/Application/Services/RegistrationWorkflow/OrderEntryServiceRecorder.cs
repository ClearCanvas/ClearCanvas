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
using System.Linq;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
	class OrderEntryServiceRecorder
	{
		static class Operations
		{
			public const string New = "Order:New";
			public const string OpenForModifyOrReplace = "Order:OpenForModifyOrReplace";
			public const string Modify = "Order:Modify";
			public const string Replace = "Order:Replace";
			public const string Cancel = "Order:Cancel";
			public const string Merge = "Order:Merge";
			public const string Unmerge = "Order:Unmerge";
		}

		internal class PlaceOrder : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (PlaceOrderRequest)recorderContext.Request;
				var profile = persistenceContext.Load<PatientProfile>(request.Requisition.Patient.PatientProfileRef, EntityLoadFlags.None);
				
				var response = (PlaceOrderResponse) recorderContext.Response;
				var order = persistenceContext.Load<Order>(response.Order.OrderRef, EntityLoadFlags.None);

				return new OperationData(Operations.New, profile, order);
			}
		}

		internal class ReplaceOrder : RisServiceOperationRecorderBase
		{
			[DataContract]
			public class ReplaceOrderOperationData : OperationData
			{
				public ReplaceOrderOperationData(string operation, PatientProfile patientProfile, Order cancelledOrder, Order newOrder)
					: base(operation, patientProfile)
				{
					this.CancelledOrder = new OrderData(cancelledOrder);
					this.NewOrder = new OrderData(newOrder);
				}

				[DataMember]
				public OrderData CancelledOrder;

				[DataMember]
				public OrderData NewOrder;
			}

			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (ReplaceOrderRequest)recorderContext.Request;
				var profile = persistenceContext.Load<PatientProfile>(request.Requisition.Patient.PatientProfileRef, EntityLoadFlags.None);
				var cancelledOrder = persistenceContext.Load<Order>(request.OrderRef, EntityLoadFlags.None);

				var response = (ReplaceOrderResponse)recorderContext.Response;
				var newOrder = persistenceContext.Load<Order>(response.Order.OrderRef, EntityLoadFlags.None);

				return new ReplaceOrderOperationData(Operations.Replace, profile, cancelledOrder, newOrder);
			}
		}

		internal class LoadOrderForEdit : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var response = (GetOrderRequisitionForEditResponse)recorderContext.Response;
				var patientProfile = persistenceContext.Load<PatientProfile>(response.Requisition.Patient.PatientProfileRef, EntityLoadFlags.None);
				var order = persistenceContext.Load<Order>(response.Requisition.OrderRef, EntityLoadFlags.None);

				return new OperationData(Operations.OpenForModifyOrReplace, patientProfile, order);
			}
		}

		internal class ModifyOrder : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (ModifyOrderRequest)recorderContext.Request;
				var profile = persistenceContext.Load<PatientProfile>(request.Requisition.Patient.PatientProfileRef, EntityLoadFlags.None);
				var order = persistenceContext.Load<Order>(request.Requisition.OrderRef, EntityLoadFlags.None);

				IncludeChangeSetFor(order);
				IncludeChangeSetFor(typeof(Procedure));

				return new OperationData(Operations.Modify, profile, order);
			}
		}

		internal class CancelOrder : RisServiceOperationRecorderBase
		{
			[DataContract]
			public class CancelOrderOperationData : OperationData
			{
				public CancelOrderOperationData(string operation, PatientProfile patientProfile, Order order)
					: base(operation, patientProfile, order)
				{
					this.CancelReason = EnumUtils.GetEnumValueInfo(order.CancelInfo.Reason);
				}

				[DataMember]
				public EnumValueInfo CancelReason;
			}


			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (CancelOrderRequest)recorderContext.Request;
				var order = persistenceContext.Load<Order>(request.OrderRef, EntityLoadFlags.None);
				var patientProfile = order.Procedures.First().PatientProfile;	// choose patient profile from one procedure?

				return new CancelOrderOperationData(Operations.Cancel, patientProfile, order);
			}
		}

		internal class MergeOrder : RisServiceOperationRecorderBase
		{
			[DataContract]
			public class MergeOrderOperationData : OperationData
			{
				public MergeOrderOperationData(string operation, PatientProfile patientProfile, Order destOrder, IEnumerable<Order> mergedOrders)
					: base(operation, patientProfile)
				{
					this.MergedIntoOrder = new OrderData(destOrder);
					this.MergedOrders = mergedOrders.Select(x => new OrderData(x)).ToList();
				}

				[DataMember]
				public OrderData MergedIntoOrder;

				[DataMember]
				public List<OrderData> MergedOrders;
			}

			protected override bool ShouldCapture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (MergeOrderRequest)recorderContext.Request;
				return !(request.DryRun || request.ValidationOnly);
			}

			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (MergeOrderRequest)recorderContext.Request;
				var destOrder = persistenceContext.Load<Order>(request.DestinationOrderRef, EntityLoadFlags.None);
				var patientProfile = destOrder.Procedures.First().PatientProfile;	// choose patient profile from one procedure?
				var sourceOrders = request.SourceOrderRefs.Select(r => persistenceContext.Load<Order>(r, EntityLoadFlags.None));

				return new MergeOrderOperationData(Operations.Merge, patientProfile, destOrder, sourceOrders);
			}
		}

		internal class UnmergeOrder : RisServiceOperationRecorderBase
		{
			protected override bool ShouldCapture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (UnmergeOrderRequest)recorderContext.Request;
				return !request.DryRun;
			}

			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (UnmergeOrderRequest)recorderContext.Request;
				var order = persistenceContext.Load<Order>(request.OrderRef, EntityLoadFlags.None);
				var patientProfile = order.Procedures.First().PatientProfile;	// choose patient profile from one procedure?
				return new OperationData(Operations.Unmerge, patientProfile, order);
			}
		}

	}
}
