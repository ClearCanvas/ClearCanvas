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

namespace ClearCanvas.ImageViewer.Common.Tests
{
    [TestFixture]
    public class DiskspaceTests
    {
        const long _kiloByte = 1024;
        const long _megaByte = _kiloByte * _kiloByte;
        const long _gigaByte = _megaByte * _kiloByte;
        const long _terabyte = _gigaByte * _kiloByte;
        const long _petaByte = _terabyte * _kiloByte;
        const long _halfGig = 500 * _megaByte;

        [Test]
        public void TestUsedSpacePercent()
        {
            var diskSpace = new Diskspace { TotalSpace = _petaByte, FreeSpace = _halfGig };
            Assert.AreEqual(99.999953433871269, diskSpace.UsedSpacePercent);
            Assert.AreEqual(100 - 99.999953433871269, diskSpace.FreeSpacePercent);
        }

        [Test]
        public void TestUsedSpace()
        {
            var diskSpace = new Diskspace { TotalSpace = _petaByte, FreeSpace = _halfGig };
            Assert.AreEqual(_petaByte - _halfGig, diskSpace.UsedSpace);
        }

        [Test]
        public void TestFormat()
        {
            var diskSpace = new Diskspace { TotalSpace = 2*_petaByte, FreeSpace = _halfGig };
            var format = Diskspace.FormatBytes(diskSpace.TotalSpace, "F3");
            Assert.AreEqual("2.000 PB", format);

            format = Diskspace.FormatBytes(diskSpace.TotalSpace);
            Assert.AreEqual("2.0 PB", format);

            diskSpace.TotalSpace = 1024 * _petaByte;
            format = Diskspace.FormatBytes(diskSpace.TotalSpace, "F3");
            Assert.AreEqual("1024.000 PB", format);

            diskSpace.TotalSpace = _petaByte;
            format = Diskspace.FormatBytes(diskSpace.TotalSpace, "F3");
            Assert.AreEqual("1.000 PB", format);

            diskSpace.TotalSpace = 1999 * _terabyte;
            format = Diskspace.FormatBytes(diskSpace.TotalSpace, "F3");
            //1999/1024 (PB)
            Assert.AreEqual("1.952 PB", format);

            diskSpace.TotalSpace = 999542 * _gigaByte;
            format = Diskspace.FormatBytes(diskSpace.TotalSpace, "F3");
            //999542/1024 (TB)
            Assert.AreEqual("976.115 TB", format);

            diskSpace.TotalSpace = 854563 * _gigaByte;
            format = Diskspace.FormatBytes(diskSpace.TotalSpace, "F3");
            //854563/1024 (TB)
            Assert.AreEqual("834.534 TB", format);

            diskSpace.TotalSpace = _terabyte;
            format = Diskspace.FormatBytes(diskSpace.TotalSpace, "F3");
            Assert.AreEqual("1.000 TB", format);

            diskSpace.TotalSpace = _gigaByte;
            format = Diskspace.FormatBytes(diskSpace.TotalSpace, "F3");
            Assert.AreEqual("1.000 GB", format);

            diskSpace.TotalSpace = _megaByte;
            format = Diskspace.FormatBytes(diskSpace.TotalSpace, "F3");
            Assert.AreEqual("1.000 MB", format);

            diskSpace.TotalSpace = _kiloByte;
            format = Diskspace.FormatBytes(diskSpace.TotalSpace, "F3");
            Assert.AreEqual("1.000 KB", format);

            diskSpace.TotalSpace = 1023;
            format = Diskspace.FormatBytes(diskSpace.TotalSpace, "F3");
            Assert.AreEqual("1023.000 B", format);
        }
    }
}

#endif