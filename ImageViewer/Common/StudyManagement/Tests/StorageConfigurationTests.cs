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

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Tests
{
    [TestFixture]
    public class StorageConfigurationTests
    {
        const long _kiloByte = 1024;
        const long _megaByte = _kiloByte * _kiloByte;
        const long _gigaByte = _megaByte * _kiloByte;
        const long _terabyte = _gigaByte * _kiloByte;
        const long _petaByte = _terabyte * _kiloByte;
        const long _halfGig = 500 * _megaByte;

        [Test]
        public void TestMinimumFreeSpaceBytes()
        {
            var diskSpace = new Diskspace { TotalSpace = _petaByte, FreeSpace = _halfGig };
            var configuration = new StorageConfiguration
                                                     {
                                                         FileStoreDirectory = @"c:\filestore",
                                                         FileStoreDiskSpace = diskSpace
                                                     };

            configuration.MinimumFreeSpacePercent = 90;
            Assert.AreEqual(1013309916158361, configuration.MinimumFreeSpaceBytes);
        }

        [Test]
        public void TestMaximumUsedSpace()
        {
            var diskSpace = new Diskspace { TotalSpace = _petaByte, FreeSpace = _halfGig };
            var configuration = new StorageConfiguration
            {
                FileStoreDirectory = @"c:\filestore",
                FileStoreDiskSpace = diskSpace
            };

            configuration.MinimumFreeSpacePercent = 90;
            Assert.AreEqual(10, configuration.MaximumUsedSpacePercent);
            Assert.AreEqual(_petaByte - 1013309916158361, configuration.MaximumUsedSpaceBytes);
        }

        [Test]
        public void TestSetMinUsedSpace_Auto()
        {
            var diskSpace = new Diskspace { TotalSpace = _petaByte, FreeSpace = _halfGig };
            var configuration = new StorageConfiguration
            {
                FileStoreDirectory = @"c:\filestore",
                FileStoreDiskSpace = diskSpace
            };

            configuration.MinimumFreeSpacePercent = -10;
            Assert.AreEqual(configuration.MinimumFreeSpacePercent, StorageConfiguration.AutoMinimumFreeSpace);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSetMinUsedSpace_InvalidPercent()
        {
            var diskSpace = new Diskspace { TotalSpace = _petaByte, FreeSpace = _halfGig };
            var configuration = new StorageConfiguration
            {
                FileStoreDirectory = @"c:\filestore",
                FileStoreDiskSpace = diskSpace
            };

            configuration.MinimumFreeSpacePercent = 110;
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSetMaxUsedSpace_InvalidPercent()
        {
            var diskSpace = new Diskspace { TotalSpace = _petaByte, FreeSpace = _halfGig };
            var configuration = new StorageConfiguration
            {
                FileStoreDirectory = @"c:\filestore",
                FileStoreDiskSpace = diskSpace
            };

            configuration.MaximumUsedSpacePercent = -1;
        }
        
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDriveNotSet()
        {
            var configuration = new StorageConfiguration {FileStoreDirectory = @"c:\filestore"};
            var maxUsedSpace = configuration.MaximumUsedSpaceBytes;
        }

        [Test]
        public void TestFileStoreDirectoryValid()
        {
            var configuration = new StorageConfiguration { FileStoreDirectory = @"c:\filestore" };
            Assert.IsTrue(configuration.IsFileStoreDriveValid);

            configuration.FileStoreDirectory = @"A:\\";
            Assert.IsTrue(configuration.IsFileStoreDriveValid);

            configuration.FileStoreDirectory = @"\";
            Assert.IsFalse(configuration.IsFileStoreDriveValid);

            configuration.FileStoreDirectory = @"\\";
            Assert.IsFalse(configuration.IsFileStoreDriveValid);

            configuration.FileStoreDirectory = @"\\test\testing";
            Assert.IsFalse(configuration.IsFileStoreDriveValid);
        }
    }
}

#endif