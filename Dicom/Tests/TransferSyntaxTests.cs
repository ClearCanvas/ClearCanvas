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

using System.Linq;
#if UNIT_TESTS
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	internal class TransferSyntaxTests
	{
		[Test]
		public void TestGetTransferSyntax()
		{
			const string uid = "1.2.3";
			var retrieved = TransferSyntax.GetTransferSyntax(uid);
			Assert.AreEqual(null, retrieved);
		}

		[Test]
		public void TestRegister()
		{
			const string uid = "1.2.3";
			var registered = new TransferSyntax("a", "1.2.3", true, true, true, false, false, true);

			TransferSyntax.RegisterTransferSyntax(registered);
			Assert.IsTrue(TransferSyntax.TransferSyntaxes.Contains(registered));

			var retrieved = TransferSyntax.GetTransferSyntax(uid);
			Assert.AreEqual(uid, retrieved.UidString);

			TransferSyntax.UnregisterTransferSyntax(registered);
			Assert.IsFalse(TransferSyntax.TransferSyntaxes.Contains(registered));

			TransferSyntax.UnregisterTransferSyntax(registered);
		}

		[Test]
		public void TestEquality()
		{
			// ReSharper disable ExpressionIsAlwaysNull

			var ts0 = (TransferSyntax) null;
			var tsA = new TransferSyntax("a", "1.2.3", true, true, true, false, false, true);
			var tsA2 = new TransferSyntax("a2", "1.2.3", true, true, true, false, false, true);
			var tsA3 = tsA;
			var tsB = new TransferSyntax("b", "1.2.3.4", true, true, true, false, false, true);

			Assert.IsTrue(null == ts0);
			Assert.IsTrue(ts0 == null);
			Assert.IsTrue(Equals(null, ts0));
			Assert.IsTrue(Equals(ts0, null));

			Assert.IsTrue(tsA == tsA2);
			Assert.IsTrue(tsA2 == tsA);
			Assert.IsTrue(Equals(tsA, tsA2));
			Assert.IsTrue(Equals(tsA2, tsA));

			Assert.IsTrue(tsA == tsA3);
			Assert.IsTrue(tsA3 == tsA);
			Assert.IsTrue(Equals(tsA, tsA3));
			Assert.IsTrue(Equals(tsA3, tsA));

			Assert.IsFalse(ts0 == tsA);
			Assert.IsFalse(tsA == ts0);
			Assert.IsFalse(Equals(ts0, tsA));
			Assert.IsFalse(Equals(tsA, ts0));

			Assert.IsFalse(tsA == tsB);
			Assert.IsFalse(tsB == tsA);
			Assert.IsFalse(Equals(tsA, tsB));
			Assert.IsFalse(Equals(tsB, tsA));

			// ReSharper restore ExpressionIsAlwaysNull
		}
	}
}

#endif