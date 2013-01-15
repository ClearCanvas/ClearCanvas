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

#if	UNIT_TESTS

#pragma warning disable 1591

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;

namespace ClearCanvas.Utilities.Manifest.Tests
{
	[TestFixture]
    public class SerializationTest
	{
	    [TestFixtureSetUp]
		public void Init()
		{
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void ProductTest()
		{
            Product theProduct = new Product
                                     {
                                         Manifest = "Manifest.xml",
                                         Name = "ClearCanvas Test",
                                         Suffix = "SP1",
                                         Version = "1.2.12011.33333"
                                     };

		    ProductManifest theManfest = new ProductManifest
		                                     {
		                                         Product = theProduct
		                                     };

		    ManifestFile theFile = new ManifestFile
		                               {
		                                   Checksum = "111",
		                                   Filename = "Test.dll",
		                                   Timestamp = DateTime.Now
		                               };

		    theManfest.Files.Add(theFile);

            ClearCanvasManifest manifest = new ClearCanvasManifest
                                               {
                                                   ProductManifest = theManfest
                                               };

		    XmlSerializer theSerializer = new XmlSerializer(typeof(ClearCanvasManifest));

            using (FileStream fs = new FileStream("ProductTest.xml", FileMode.Create))
            {
                XmlWriter writer = XmlWriter.Create(fs);
                if (writer != null)
                    theSerializer.Serialize(writer, manifest);
                fs.Flush();
                fs.Close();
            }
		}

        [Test]
        public void PackageTest()
        {
            Package package = new Package
            {
                Manifest = "PackageManifest.xml",
                Name = "ClearCanvas Test",
                Product = new Product
                               {
                                   Name = "ClearCanvas Test",
                                   Suffix = "SP1",
                                   Version = "1.2.12011.33333"
                               },
            };

            PackageManifest packageManifest = new PackageManifest
            {
                Package = package
            };

            ManifestFile theFile = new ManifestFile
            {
                Checksum = "111",
                Filename = "Test.dll",
                Timestamp = DateTime.Now
            };

            packageManifest.Files.Add(theFile);

            ClearCanvasManifest manifest = new ClearCanvasManifest
                                               {
                                                   PackageManifest = packageManifest
                                               };

            XmlSerializer theSerializer = new XmlSerializer(typeof(ClearCanvasManifest));

            using (FileStream fs = new FileStream("PackageTest.xml", FileMode.Create))
            {
                XmlWriter writer = XmlWriter.Create(fs);
                if (writer != null)
                    theSerializer.Serialize(writer, manifest);
                fs.Flush();
                fs.Close();
            }
        }
    }
}
#endif