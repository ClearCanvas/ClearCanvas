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

using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.OrderEntry
{
	public class CancelOrDiscontinueOrderOperation
	{
		/// <summary>
		/// Executes Cancel Order operation.
		/// Checks if order is in scheduling state, then executes if it is.
		/// Otherwise, throws a WorkflowException.
		/// </summary>
		/// <param name="order"></param>
		/// <param name="info"></param>
		public void Execute(Order order, OrderCancelInfo info)
		{
			if (order.Status == OrderStatus.SC)
				order.Cancel(info);
			else if (order.Status == OrderStatus.IP)
				order.Discontinue(info);
			else
				throw new WorkflowException(string.Format("Order with status {0} cannot be cancelled/discontinued.", order.Status));
		}

		/// <summary>
		/// Determines if cancelling an order is possible.
		/// The order needs to currently be in scheduling.
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
		public bool CanExecute(Order order)
		{
			return order.Status == OrderStatus.SC || order.Status == OrderStatus.IP;
		}
	}

	public class AssignPlacerOrderNumberOperation
	{
		public void Execute(Order order, string placerOrderNumber)
		{
			if(!string.IsNullOrEmpty(order.PlacerNumber))
				throw new WorkflowException("Order already has a placer order number");

			order.PlacerNumber = placerOrderNumber;
		}

		public bool CanExecute(Order order)
		{
			return string.IsNullOrEmpty(order.PlacerNumber);
		}
	}
}
