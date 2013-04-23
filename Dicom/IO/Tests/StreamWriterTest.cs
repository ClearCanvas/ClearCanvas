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

using System.IO;
using ClearCanvas.Dicom.Tests;
using NUnit.Framework;

namespace ClearCanvas.Dicom.IO.Tests
{
    [TestFixture]
    public class StreamWriterTests : AbstractTest
    {
        [Test]
        public void OBAttributeEvenLengthTests()
        {
            using (var ms = new MemoryStream())
            {
                var writer = new DicomStreamWriter(ms);

                var ds = new DicomAttributeCollection();
                var rawdata = new byte[] { 0x0, 0x1 };

                ds[DicomTags.EncapsulatedDocument].Values = rawdata;
                writer.Write(TransferSyntax.ImplicitVrLittleEndian, ds, DicomWriteOptions.Default);

                ms.Position = 0;
                var output = ms.ToArray();
                var expectedOutput = new byte[]
                                         {
                                             0x42, 0x00, 0x11, 0x00,
                                             0x02, 0x00, 0x00, 0x00,
                                             0x0, 0x1
                                         };

                Assert.AreEqual(output, expectedOutput);
            }

        }

        [Test]
        public void OBAttributeOddLengthTests()
        {
            using(var ms = new MemoryStream())
            {
                var writer = new DicomStreamWriter(ms);

                var ds = new DicomAttributeCollection();
                var rawdata = new byte[] {0x0, 0x1, 0x2};

                ds[DicomTags.EncapsulatedDocument].Values = rawdata;
                writer.Write(TransferSyntax.ImplicitVrLittleEndian, ds, DicomWriteOptions.Default);

                ms.Position = 0;
                var output = ms.ToArray();
                var expectedOutput = new byte[]
                                         {
                                             0x42, 0x00, 0x11, 0x00,
                                             0x04, 0x00, 0x00, 0x00,
                                             0x0, 0x1, 0x2, 0x0 /* pad */
                                         };

                Assert.AreEqual(output, expectedOutput);
            }
            
        }

        [Test]
        public void TextAttributeEvenLengthTests()
        {
            using (var ms = new MemoryStream())
            {
                var writer = new DicomStreamWriter(ms);

                var ds = new DicomAttributeCollection();

                ds[DicomTags.PatientId].Values = "PatientID0";
                writer.Write(TransferSyntax.ImplicitVrLittleEndian, ds, DicomWriteOptions.Default);

                ms.Position = 0;
                var output = ms.ToArray();
                var expectedOutput = new byte[]
                                         {
                                             0x10, 0x00, 0x20, 0x00,
                                             0x0A, 0x00, 0x00, 0x00,
                                             0x50, 0x61,0x74,0x69,0x65,0x6E,0x74,0x49,0x44,0x30 /*PatientID0*/
                                         };

                Assert.AreEqual(output, expectedOutput);
            }

        }

        [Test]
        public void TextAttributeOddLengthTests()
        {
            using (var ms = new MemoryStream())
            {
                var writer = new DicomStreamWriter(ms);

                var ds = new DicomAttributeCollection();

                ds[DicomTags.PatientId].Values = "PatientID01";
                writer.Write(TransferSyntax.ImplicitVrLittleEndian, ds, DicomWriteOptions.Default);

                ms.Position = 0;
                var output = ms.ToArray();
                var expectedOutput = new byte[]
                                         {
                                             0x10, 0x00, 0x20, 0x00,
                                             0x0C, 0x00, 0x00, 0x00,
                                             0x50, 0x61,0x74,0x69,0x65,0x6E,0x74,0x49,0x44,0x30,0x31, /*PatientID01*/
                                             0x20 /*pad*/
                                         };

                Assert.AreEqual(output, expectedOutput);
            }


        }
    }
}

#endif
