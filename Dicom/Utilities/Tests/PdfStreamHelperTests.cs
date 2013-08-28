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

namespace ClearCanvas.Dicom.Utilities.Tests
{
    [TestFixture]
    public class PdfStreamHelperTests
    {
        [Test]
        public void TestGoodPdf()
        {

            TestNoStripMade(PdfTestSamples.PdfSample_CRLF); 
            TestNoStripMade(PdfTestSamples.PdfSample_LF);  
            TestNoStripMade(PdfTestSamples.PdfSample_NoEOL); 
            TestNoStripMade(PdfTestSamples.PdfSample_IncrementalChanges); 
        }

        [Test]
        public void TestBadPdf()
        {
            TestNoStripMade(PdfTestSamples.PdfSample_Corrupted);

        }

        [Test]
        public void TestGoodPdfPaddedWithNull()
        {
            TestStripMade(PdfTestSamples.PdfSample_CRLFNull);
            TestStripMade(PdfTestSamples.PdfSample_CRNull);
            TestStripMade(PdfTestSamples.PdfSample_LFNull);
            TestStripMade(PdfTestSamples.PdfSamepl_NoLFNull);
        }

        [Test]
        public void TestPdfUnknownPadding()
        {
            TestNoStripMade(PdfTestSamples.PdfSample_CRLFNullSpace);
            TestNoStripMade(PdfTestSamples.PdfSample_CRLFSpace);
            TestNoStripMade(PdfTestSamples.PdfSample_CRLFSpaceNull);

            TestNoStripMade(PdfTestSamples.PdfSample_LFNullSpace);
            TestNoStripMade(PdfTestSamples.PdfSample_LFSpace);
            TestNoStripMade(PdfTestSamples.PdfSample_LFSpaceNull);

        }


        private void TestStripMade(string rawContent)
        {
            var raw = Convert.FromBase64String(rawContent);
            var stripped = PdfStreamHelper.StripDicomPaddingBytes(raw);
            Assert.AreEqual(stripped.Length, raw.Length - 1);

            var expected = new byte[raw.Length - 1];
            Array.Copy(raw, expected, raw.Length - 1);

            Assert.AreEqual(stripped, expected);
        }

        private void TestNoStripMade(string rawContent)
        {
            var raw = Convert.FromBase64String(rawContent);
            var stripped = PdfStreamHelper.StripDicomPaddingBytes(raw);
            Assert.AreEqual(stripped.Length, raw.Length);
            Assert.AreEqual(stripped,raw);
        }

        
    }
}
#endif