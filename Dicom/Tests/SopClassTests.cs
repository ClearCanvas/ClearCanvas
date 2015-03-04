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

using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	internal class SopClassTests
	{
		[Test(Description = "Sanity check on generated SopClass(es)")]
		public void TestCategories()
		{
			foreach (var sopClass in SopClass.GetRegisteredSopClasses())
			{
				//In DICOM 2011:
				// a) all meta SOP class names contain "Meta"
				// b) all Image Storage SOP class names contain "Image Storage", except "Enhanced US Volume Storage"
				// c) all Storage SOP class names contain "Storage", excluding "Storage Commitment"

				if (sopClass.Name.ToLower().Contains("meta"))
				{
					Assert.IsTrue(sopClass.Meta);
					Assert.IsFalse(sopClass.IsStorage);
					Assert.IsFalse(sopClass.IsImage);
				}
				else if (sopClass.Name.ToLower().Contains("image storage") || sopClass.Name == "Enhanced US Volume Storage")
				{
					Assert.IsTrue(sopClass.IsImage);
					Assert.IsTrue(sopClass.IsStorage);
					Assert.IsFalse(sopClass.Meta);
				}
				else if (sopClass.Name.ToLower().Contains("storage") && !sopClass.Name.ToLower().Contains("storage commitment"))
				{
					Assert.IsTrue(sopClass.IsStorage);
					Assert.IsFalse(sopClass.IsImage);
					Assert.IsFalse(sopClass.Meta);
				}
				else
				{
					Assert.IsFalse(sopClass.IsImage);
					Assert.IsFalse(sopClass.IsStorage);
					Assert.IsFalse(sopClass.Meta);
				}
			}
		}

		[Test]
		public void TestRegister()
		{
			const string uid = "1.2.3";
			var sopClass = new SopClass("Bleeding Edge Image", uid, SopClassCategory.Image);

			var registered = SopClass.RegisterSopClass(sopClass);
			Assert.AreEqual(uid, registered.Uid);

			var retrieved = SopClass.GetSopClass(uid);
			Assert.AreEqual(uid, retrieved.Uid);

			var reregistered = new SopClass("Bleeding Edge Image", uid, SopClassCategory.Image);
			Assert.IsTrue(SopClass.GetRegisteredSopClasses().Contains(reregistered));

			reregistered = SopClass.RegisterSopClass(reregistered);
			Assert.IsTrue(ReferenceEquals(registered, reregistered));

			var unregistered = SopClass.UnregisterSopClass(registered);
			Assert.IsTrue(ReferenceEquals(registered, unregistered));
			Assert.IsFalse(SopClass.GetRegisteredSopClasses().Contains(registered));

			unregistered = SopClass.UnregisterSopClass(registered);
			Assert.IsNull(unregistered);
		}

		[Test]
		public void TestGetSopClass()
		{
			const string uid = "1.2.3";
			var retrieved = SopClass.GetSopClass(uid);
			Assert.AreEqual(uid, retrieved.Uid);
			Assert.IsTrue(!SopClass.GetRegisteredSopClasses().Contains(retrieved));
		}

		[Test]
		public void TestEquality()
		{
			// ReSharper disable ExpressionIsAlwaysNull

			var sop0 = (SopClass) null;
			var sopA = new SopClass("a", "1.2.3", SopClassCategory.Image);
			var sopA2 = new SopClass("a2", "1.2.3", SopClassCategory.Meta);
			var sopA3 = sopA;
			var sopB = new SopClass("b", "1.2.3.4", SopClassCategory.Image);

			Assert.IsTrue(null == sop0);
			Assert.IsTrue(sop0 == null);
			Assert.IsTrue(Equals(null, sop0));
			Assert.IsTrue(Equals(sop0, null));

			Assert.IsTrue(sopA == sopA2);
			Assert.IsTrue(sopA2 == sopA);
			Assert.IsTrue(Equals(sopA, sopA2));
			Assert.IsTrue(Equals(sopA2, sopA));

			Assert.IsTrue(sopA == sopA3);
			Assert.IsTrue(sopA3 == sopA);
			Assert.IsTrue(Equals(sopA, sopA3));
			Assert.IsTrue(Equals(sopA3, sopA));

			Assert.IsFalse(sop0 == sopA);
			Assert.IsFalse(sopA == sop0);
			Assert.IsFalse(Equals(sop0, sopA));
			Assert.IsFalse(Equals(sopA, sop0));

			Assert.IsFalse(sopA == sopB);
			Assert.IsFalse(sopB == sopA);
			Assert.IsFalse(Equals(sopA, sopB));
			Assert.IsFalse(Equals(sopB, sopA));

			// ReSharper restore ExpressionIsAlwaysNull
		}
	}
}

#endif