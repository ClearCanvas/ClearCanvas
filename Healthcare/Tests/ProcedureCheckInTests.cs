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
using NUnit.Framework;
using ClearCanvas.Workflow;


namespace ClearCanvas.Healthcare.Tests
{
	[TestFixture]
	public class ProcedureCheckInTests
	{
		# region Public Operations Tests

		[Test]
		public void Test_CheckIn()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();

			Assert.IsTrue(pc.IsPreCheckIn);
			Assert.IsFalse(pc.IsCheckedIn);
			Assert.IsFalse(pc.IsCheckedOut);

			pc.CheckIn(now);

			Assert.IsFalse(pc.IsPreCheckIn);
			Assert.IsTrue(pc.IsCheckedIn);
			Assert.IsFalse(pc.IsCheckedOut);
			Assert.AreEqual(now, pc.CheckInTime);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_CheckIn_AlreadyCheckedIn()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckIn(now);
			pc.CheckIn(now);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_CheckOut_NeverCheckedIn()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckOut(now);
		}

		[Test]
		public void Test_CheckOut_Typical()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckIn(now);

			Assert.IsFalse(pc.IsPreCheckIn);
			Assert.IsTrue(pc.IsCheckedIn);
			Assert.IsFalse(pc.IsCheckedOut);

			pc.CheckOut(now);

			Assert.IsFalse(pc.IsPreCheckIn);
			Assert.IsFalse(pc.IsCheckedIn);
			Assert.IsTrue(pc.IsCheckedOut);
			Assert.AreEqual(now, pc.CheckOutTime);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_CheckOut_AlreadyCheckedOut()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckIn(now);
			pc.CheckOut(now);

			pc.CheckOut(now);
		}

		[Test]
		public void Test_RevertCheckIn()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckIn(now);

			Assert.IsFalse(pc.IsPreCheckIn);
			Assert.IsTrue(pc.IsCheckedIn);
			Assert.IsFalse(pc.IsCheckedOut);
			Assert.AreEqual(now, pc.CheckInTime);

			pc.RevertCheckIn();

			Assert.IsTrue(pc.IsPreCheckIn);
			Assert.IsFalse(pc.IsCheckedIn);
			Assert.IsFalse(pc.IsCheckedOut);
			Assert.IsNull(pc.CheckInTime);
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_RevertCheckIn_NeverCheckedIn()
		{
			var pc = new ProcedureCheckIn();
			pc.RevertCheckIn();
		}

		[Test]
		[ExpectedException(typeof(WorkflowException))]
		public void Test_RevertCheckIn_AlreadyCheckedOut()
		{
			DateTime? now = DateTime.Now;
			var pc = new ProcedureCheckIn();
			pc.CheckIn(now);
			pc.CheckOut(now + TimeSpan.FromSeconds(100));
			pc.RevertCheckIn();
		}

		#endregion
	}
}

#endif