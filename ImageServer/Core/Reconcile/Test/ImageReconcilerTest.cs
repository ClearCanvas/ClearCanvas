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

using ClearCanvas.ImageServer.Common.Utilities;
using NUnit.Framework;

namespace ClearCanvas.ImageServer.Core.Reconcile.Test
{
    [TestFixture]
    public class ImageReconcilerTest
    {
        [Test]
        public void Test_LookLikeSameNames()
        {


            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva^Siva", "Selva^Siva"), "These are different. Should return false.");

            // Both names don't have ^
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva Siva", "Selva Siva"), "Identical");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva Siva", "Selva SIVA"), "letter case");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva SIVA", "Selva Siva"), "letter case");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva Siva", "   Selva  Siva  "), "Trailing/Leading Spaces");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("   Selva  Siva  ", "Selva Siva"), "Trailing/Leading Spaces");
            
            // Only one of the names has ^
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva Siva", "Selva^Siva"), "One has ^");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva^Siva", "Selva Siva"), "One has ^");

            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva Siva", "Selva^SIVA"), "letter case");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva^SIVA", "Selva Siva"), "letter case");

            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva Siva", " Selva ^ Siva "), "Trailing/Leading Spaces");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames(" Selva ^ Siva ", "Selva Siva"), "Trailing/Leading Spaces");

            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva Siva", " Selva ^ Siva ^ ^ ^"), "Trailing Empty Components");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames(" Selva ^ Siva ^ ^ ^", "Selva Siva"), "Trailing Empty Components");

            // Both names have ^
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva^Siva", "Selva^SIVA"), "letter case");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva^SIVA", "Selva^Siva"), "letter case");

            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva^Siva", " Selva ^ Siva "), "Trailing/Leading Spaces");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames(" Selva ^ Siva ", "Selva^Siva"), "Trailing/Leading Spaces");

            Assert.IsTrue(DicomNameUtils.LookLikeSameNames("Selva^Siva", " Selva ^ Siva ^ ^ ^"), "Trailing Empty Components");
            Assert.IsTrue(DicomNameUtils.LookLikeSameNames(" Selva ^ Siva ^ ^ ^", "Selva^Siva"), "Trailing Empty Components");

            // Spaces in between... names are not the same
            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva Siva", "Selva Si va"));
            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva Siv a", "Selva Siva"));

            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva^Siva", "Selva Si va"));
            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva Si va", "Selva^Siva"));

            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva^Siva", "Selva^Si va"));
            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva^Si va", "Selva^Siva"));

            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva^Siva", "Selva^^Siva"));

            Assert.IsFalse(DicomNameUtils.LookLikeSameNames("Selva^Siva", "Selva^Siva^Jr"));
            
        }

    }
}
