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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using NUnit.Framework;

#if UNIT_TESTS

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Tests
{
    [TestFixture]
    public class StudyStoreTests
    {
        [TestFixtureSetUp]
        public void Initialize1()
        {
            StudyStoreTestServiceProvider.Reset();

            Platform.SetExtensionFactory(new UnitTestExtensionFactory
                                             {
                                                 { typeof(ServiceProviderExtensionPoint), typeof(StudyStoreTestServiceProvider) }
                                             });

            //Force IsSupported to be re-evaluated.
            StudyStore.InitializeIsSupported();
        }

        public void Initialize2()
        {
            StudyStoreTestServiceProvider.Reset();

            Platform.SetExtensionFactory(new NullExtensionFactory());
            //Force IsSupported to be re-evaluated.
            StudyStore.InitializeIsSupported();
        }

        [Test]
        public void TestIsSupported()
        {
            Initialize1();
            Assert.IsTrue(StudyStore.IsSupported);

            Initialize2();
            Assert.IsFalse(StudyStore.IsSupported);
        }
    }
}


#endif