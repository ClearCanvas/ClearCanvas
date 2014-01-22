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

using NUnit.Framework;

#if UNIT_TESTS

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	public class SopClassGeneratorTests
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
			var uid = "1.2.3";
			var sopClass = new SopClass("Bleeding Edge Image", uid, SopClassCategory.Image);

			var registered = SopClass.RegisterSopClass(sopClass);
			Assert.AreEqual(uid, registered.Uid);

			var retrieved = SopClass.GetSopClass(uid);
			Assert.AreEqual(uid, retrieved.Uid);

			var reregistered = new SopClass("Bleeding Edge Image", uid, SopClassCategory.Image);
			reregistered = SopClass.RegisterSopClass(reregistered);
			Assert.IsTrue(ReferenceEquals(registered, reregistered));
		}

		[Test]
		public void TestGet()
		{
			var uid = "1.2.3";
			var retrieved = SopClass.GetSopClass(uid);
			Assert.AreEqual(uid, retrieved.Uid);
		}
	}
}

#endif
