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

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using NUnit.Framework;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Tests
{
	[TestFixture]
	public class OrderTests
	{
		private OrderCancelReasonEnum _unmergedCancelReason;

		public OrderTests()
		{
			_unmergedCancelReason = new OrderCancelReasonEnum("UM", "Unmerged", "The order was merged and then unmerged.");

			// set the extension factory to special test factory
			Platform.SetExtensionFactory(new TestExtensionFactory());
		}

		[Test]
		public void Test_merge_order()
		{
			var patient = TestPatientFactory.CreatePatient();
			var visit = TestVisitFactory.CreateVisit(patient);
			var facility = TestFacilityFactory.CreateFacility();
			var order1 = TestOrderFactory.CreateOrder(patient, visit, facility, "101", 2, 1, true, true);
			var order2 = TestOrderFactory.CreateOrder(patient, visit, facility, "102", 2, 1, true, true);
			var staff = TestStaffFactory.CreateStaff();

			// merge order1 into order2
			order1.Merge(new OrderMergeInfo(staff, DateTime.Now, order2));

			// order1 post conditions
			Assert.AreEqual(OrderStatus.MG, order1.Status);
			Assert.IsNotNull(order1.MergeInfo);
			Assert.AreEqual(order2, order1.MergeInfo.MergeDestinationOrder);
			Assert.AreEqual(staff, order1.MergeInfo.MergedBy);
			Assert.AreEqual(2, order1.Procedures.Count);
			Assert.IsTrue(CollectionUtils.TrueForAll(order1.Procedures, p => p.Status == ProcedureStatus.GH));

			// order2 post conditions
			Assert.AreEqual(OrderStatus.SC, order2.Status);
			Assert.IsNull(order2.MergeInfo);
			Assert.AreEqual(4, order2.Procedures.Count);
			Assert.IsTrue(CollectionUtils.TrueForAll(order1.Procedures, p => p.Status == ProcedureStatus.SC));
		}
	}
}

#endif
