#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU Lesser Public
// License as published by the Free Software Foundation, either version 3 of
// the License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that
// it will be useful, but WITHOUT ANY WARRANTY; without even the implied
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser Public License for more details.
//
// You should have received a copy of the GNU Lesser Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

#if UNIT_TESTS

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Common.Utilities.Imaging.Tests
{
    [TestFixture]
    public class StripPngColourManagementTests
    {
        [Test]
        [ExpectedException(typeof(System.IO.InvalidDataException))]
        public void TestFirstChunkTruncatedAfterLength()
        {
            var bytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 9 };
            PngEncoder.StripColorManagement(new MemoryStream(bytes), new MemoryStream());
        }

        [Test]
        [ExpectedException(typeof(System.IO.InvalidDataException))]
        public void TestBadHeader()
        {
            var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 0x0, 0x0, 0x0, 0x6 };
            PngEncoder.StripColorManagement(new MemoryStream(bytes), new MemoryStream());
        }

        [Test]
        [ExpectedException(typeof(System.IO.InvalidDataException))]
        public void TestFirstChunkTruncatedBeforeCRC()
        {
            //Length = 6, type="IHDR", crc(invalid)
            var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 0,0,0,6, 73, 72, 68, 82, 1,2,3,4,5,6, 1,2,3,4};
            var test = new MemoryStream();
            PngEncoder.StripColorManagement(new MemoryStream(bytes), test);
            var testBytes = test.ToArray();
            Assert.IsTrue(bytes.SequenceEqual(testBytes));
        }

        [Test]
        public void TestStripColorManagement()
        {
            //Create an image with lots of colours
            using (var bitmap = new Bitmap(100, 100))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.Yellow);
                    using (var brush = new LinearGradientBrush(new Rectangle(0, 0, 25, 25), Color.Red, Color.Blue, LinearGradientMode.ForwardDiagonal))
                    {
                        graphics.FillRectangle(brush, new Rectangle(0, 0, 50, 50));
                    }
                    using (var brush = new LinearGradientBrush(new Rectangle(0, 0, 25, 25), Color.Blue, Color.Green, LinearGradientMode.ForwardDiagonal))
                    {
                        graphics.FillRectangle(brush, new Rectangle(0, 50, 50, 50));
                    }
                    using (var brush = new LinearGradientBrush(new Rectangle(0, 0, 25, 25), Color.Red, Color.Green, LinearGradientMode.ForwardDiagonal))
                    {
                        graphics.FillRectangle(brush, new Rectangle(50, 0, 50, 50));
                    }
                    using (var brush = new LinearGradientBrush(new Rectangle(0, 0, 25, 25), Color.Cyan, Color.Purple, LinearGradientMode.ForwardDiagonal))
                    {
                        graphics.FillRectangle(brush, new Rectangle(50, 50, 50, 50));
                    }
                }

                using (var memStream = new MemoryStream())
                {
                    //Save to png
                    bitmap.Save(memStream, ImageFormat.Png);

                    //bitmap.Save(@"c:\stewart\temp\testbitmap.png");
                    var chunkInfo = PngEncoder.GetChunkInfo(memStream);
                    var totalLength = chunkInfo.Sum(c => c.TotalLength) + 8; //8 for header
                    
                    //Parsed chunk info must match exact length of stream
                    Assert.AreEqual(memStream.Length, totalLength);
                    //Has some color management stuff by default
                    Assert.IsTrue(0 < chunkInfo.Count(c => c.IsColorManagement));

                    using (var strippedStream = new MemoryStream())
                    {
                        PngEncoder.StripColorManagement(memStream, strippedStream);
                        chunkInfo = PngEncoder.GetChunkInfo(strippedStream);
                        totalLength = chunkInfo.Sum(c => c.TotalLength) + 8; //8 for header

                        //Parsed chunk info must match exact length of stream
                        Assert.AreEqual(strippedStream.Length, totalLength);
                        //Has no color management stuff left
                        Assert.AreEqual(0, chunkInfo.Count(c => c.IsColorManagement));

                        strippedStream.Position = 0;
                        using (var strippedBitmap = new Bitmap(strippedStream))
                        {
                            for (int x = 0; x < 100; x++)
                            {
                                for (int y = 0; y < 100; y++)
                                {
                                    var pixel1 = strippedBitmap.GetPixel(x, y);
                                    var pixel2 = bitmap.GetPixel(x, y);
                                    Assert.AreEqual(pixel1, pixel2);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

#endif