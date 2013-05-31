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

namespace ClearCanvas.ImageServer.Common.Utilities.Test
{
    [TestFixture]
    public class DicomNameUtilsTest
    {
        [Test]
        public void Test_TrimSpacesOption()
        {
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C  ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize(" Ab C  ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("  Ab       C  ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C");
            

            Assert.AreEqual(DicomNameUtils.Normalize("Ab C^Ab C", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C ^Ab C ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C  ^Ab C  ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize(" Ab C  ^ Ab C  ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("  Ab C  ^  Ab C  ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("  Ab       C  ^  Ab  C  ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C");

            Assert.AreEqual(DicomNameUtils.Normalize("Ab C^Ab C ^ ^ ^ ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C^^^");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab   C^Ab   C ^ ^ ^ ", DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^Ab C^^^");
            
        }

        [Test]
        public void Test_TrimEmptyEndingComponentsOption()
        {
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C", DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents), "Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C^", DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents), "Ab C");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C^ABC", DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents), "Ab C^ABC");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C^ABC^^", DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents), "Ab C^ABC");
            Assert.AreEqual(DicomNameUtils.Normalize("Ab C^ABC^ ^ ^ ", DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents), "Ab C^ABC");
            
        }

        [Test]
        public void Test_CombinedOptions()
        {
            Assert.AreEqual(DicomNameUtils.Normalize("   Ab    C           ^    ABC     ^         ^     ^     ", DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents | DicomNameUtils.NormalizeOptions.TrimSpaces), "Ab C^ABC");

        }
    }
}

#endif