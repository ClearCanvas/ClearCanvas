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
#pragma warning disable 1591,0419,1574,1587

using System.Drawing;
using ClearCanvas.Dicom.Tests;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Iod.Modules.Tests
{
	[TestFixture]
	public class OverlayPlaneTests
	{
		#region Misc Tests

		[Test]
		public void TestIsValidMultiFrameOverlay()
		{
			const int size = 3;

			var dataset = new DicomAttributeCollection();
			SetOverlay(dataset, 0, new bool[size*size], OverlayType.R, new Point(0, 0), size, size, null, false, false);
			SetOverlay(dataset, 1, new bool[size*size], OverlayType.R, new Point(0, 0), size, size, 1, false, false);
			SetOverlay(dataset, 2, new bool[size*size*5], OverlayType.R, new Point(0, 0), size, size, 5, false, false);
			SetOverlay(dataset, 3, new bool[size*size], OverlayType.R, new Point(0, 0), size, size, 1, 3, false, false);
			SetOverlay(dataset, 4, new bool[size*size*5], OverlayType.R, new Point(0, 0), size, size, 5, 2, false, false);

			var module = new OverlayPlaneModuleIod(dataset);

			Assert.IsTrue(module[0].IsValidMultiFrameOverlay(1), "Single Frame Overlay / 1 Frame in Image");
			Assert.IsTrue(module[0].IsValidMultiFrameOverlay(2), "Single Frame Overlay / 2 Frames in Image");

			Assert.IsTrue(module[1].IsValidMultiFrameOverlay(1), "1 Frame in Overlay (origin null==1) / 1 Frame in Image");
			Assert.IsTrue(module[1].IsValidMultiFrameOverlay(2), "1 Frame in Overlay (origin null==1) / 2 Frames in Image");

			Assert.IsFalse(module[2].IsValidMultiFrameOverlay(4), "5 Frames in Overlay (origin null==1) / 4 Frames in Image");
			Assert.IsTrue(module[2].IsValidMultiFrameOverlay(5), "5 Frames in Overlay (origin null==1) / 5 Frames in Image");
			Assert.IsTrue(module[2].IsValidMultiFrameOverlay(6), "5 Frames in Overlay (origin null==1) / 6 Frames in Image");

			Assert.IsFalse(module[3].IsValidMultiFrameOverlay(2), "1 Frames in Overlay (origin 3) / 2 Frames in Image");
			Assert.IsTrue(module[3].IsValidMultiFrameOverlay(3), "1 Frames in Overlay (origin 3) / 3 Frames in Image");
			Assert.IsTrue(module[3].IsValidMultiFrameOverlay(4), "1 Frames in Overlay (origin 3) / 4 Frames in Image");

			Assert.IsFalse(module[4].IsValidMultiFrameOverlay(5), "5 Frames in Overlay (origin 2) / 5 Frames in Image");
			Assert.IsTrue(module[4].IsValidMultiFrameOverlay(6), "5 Frames in Overlay (origin 2) / 6 Frames in Image");
			Assert.IsTrue(module[4].IsValidMultiFrameOverlay(7), "5 Frames in Overlay (origin 2) / 7 Frames in Image");
		}

		#endregion

		#region Basic Tests

		[Test]
		public void TestOverlayPlaneModuleIod_SingleFrame()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int imageRows = 97;
			const int imageColumns = 101;
			const int overlay1Rows = 103;
			const int overlay1Columns = 89;
			const int overlay2Rows = 67;
			const int overlay2Columns = 71;

			var dataset = new DicomAttributeCollection();
			SetImage(dataset, new byte[imageRows*imageColumns*2], imageRows, imageColumns, 1, 16, 13, 12, false);
			SetOverlay(dataset, 0, new bool[overlay1Rows*overlay1Columns], OverlayType.G, new Point(1, 1), overlay1Rows, overlay1Columns, bigEndian, false);
			SetOverlay(dataset, 1, new bool[imageRows*imageColumns], OverlayType.G, new Point(2, 3), 15, bigEndian);
			SetOverlay(dataset, 2, new bool[overlay2Rows*overlay2Columns], OverlayType.R, new Point(4, 5), overlay2Rows, overlay2Columns, bigEndian, true);
			SetOverlay(dataset, 13, new bool[imageRows*imageColumns], OverlayType.R, new Point(6, 7), 14, bigEndian);

			var module = new OverlayPlaneModuleIod(dataset);

			// test overlay group 6000
			{
				const int overlayIndex = 0;
				var overlayPlane = module[overlayIndex];

				// assert overlay plane identification
				Assert.AreEqual(0x6000, overlayPlane.Group, "Wrong overlay group for plane #{0}", overlayIndex);
				Assert.AreEqual(0, overlayPlane.Index, "Wrong overlay index for plane #{0}", overlayIndex);
				Assert.AreEqual(0x000000, overlayPlane.TagOffset, "Wrong tag offset for plane #{0}", overlayIndex);

				// assert overlay plane encoding detection
				Assert.AreEqual(true, overlayPlane.HasOverlayData, "Incorrect OverlayData detection for plane #{0}", overlayIndex);
				Assert.AreEqual(false, overlayPlane.IsBigEndianOW, "Incorrect OW/OB detection for plane #{0}", overlayIndex);
				Assert.AreEqual(false, overlayPlane.IsMultiFrame, "Incorrect multiframe detection for plane #{0}", overlayIndex);
				Assert.AreEqual(103*89, overlayPlane.GetOverlayFrameLength(), "Incorrect frame size computation for plane #{0}", overlayIndex);

				// assert overlay plane basic attribute values
				Assert.AreEqual(103, overlayPlane.OverlayRows, "Wrong overlay rows for plane #{0}", overlayIndex);
				Assert.AreEqual(89, overlayPlane.OverlayColumns, "Wrong overlay columns for plane #{0}", overlayIndex);
				Assert.AreEqual(0, overlayPlane.OverlayBitPosition, "Wrong overlay bit position for plane #{0}", overlayIndex);
				Assert.AreEqual(1, overlayPlane.OverlayBitsAllocated, "Wrong overlay bits allocated for plane #{0}", overlayIndex);
				Assert.AreEqual(OverlayType.G, overlayPlane.OverlayType, "Wrong overlay type for plane #{0}", overlayIndex);
				Assert.AreEqual(new Point(1, 1), overlayPlane.OverlayOrigin, "Wrong overlay origin for plane #{0}", overlayIndex);

				// assert overlay plane multiframe attribute values
				Assert.AreEqual(null, overlayPlane.ImageFrameOrigin, "Wrong image frame origin for plane #{0}", overlayIndex);
				Assert.AreEqual(null, overlayPlane.NumberOfFramesInOverlay, "Wrong number of frames in overlay for plane #{0}", overlayIndex);
			}

			// test overlay group 6002
			{
				const int overlayIndex = 1;
				var overlayPlane = module[overlayIndex];

				// assert overlay plane identification
				Assert.AreEqual(0x6002, overlayPlane.Group, "Wrong overlay group for plane #{0}", overlayIndex);
				Assert.AreEqual(1, overlayPlane.Index, "Wrong overlay index for plane #{0}", overlayIndex);
				Assert.AreEqual(0x020000, overlayPlane.TagOffset, "Wrong tag offset for plane #{0}", overlayIndex);

				// assert overlay plane encoding detection
				Assert.AreEqual(false, overlayPlane.HasOverlayData, "Incorrect OverlayData detection for plane #{0}", overlayIndex);
				Assert.AreEqual(false, overlayPlane.IsBigEndianOW, "Incorrect OW/OB detection for plane #{0}", overlayIndex);
				Assert.AreEqual(false, overlayPlane.IsMultiFrame, "Incorrect multiframe detection for plane #{0}", overlayIndex);
				Assert.AreEqual(97*101, overlayPlane.GetOverlayFrameLength(), "Incorrect frame size computation for plane #{0}", overlayIndex);

				// assert overlay plane basic attribute values
				Assert.AreEqual(97, overlayPlane.OverlayRows, "Wrong overlay rows for plane #{0}", overlayIndex);
				Assert.AreEqual(101, overlayPlane.OverlayColumns, "Wrong overlay columns for plane #{0}", overlayIndex);
				Assert.AreEqual(15, overlayPlane.OverlayBitPosition, "Wrong overlay bit position for plane #{0}", overlayIndex);
				Assert.AreEqual(16, overlayPlane.OverlayBitsAllocated, "Wrong overlay bits allocated for plane #{0}", overlayIndex);
				Assert.AreEqual(OverlayType.G, overlayPlane.OverlayType, "Wrong overlay type for plane #{0}", overlayIndex);
				Assert.AreEqual(new Point(2, 3), overlayPlane.OverlayOrigin, "Wrong overlay origin for plane #{0}", overlayIndex);

				// assert overlay plane multiframe attribute values
				Assert.AreEqual(null, overlayPlane.ImageFrameOrigin, "Wrong image frame origin for plane #{0}", overlayIndex);
				Assert.AreEqual(null, overlayPlane.NumberOfFramesInOverlay, "Wrong number of frames in overlay for plane #{0}", overlayIndex);
			}

			// test overlay group 6004
			{
				const int overlayIndex = 2;
				var overlayPlane = module[overlayIndex];

				// assert overlay plane identification
				Assert.AreEqual(0x6004, overlayPlane.Group, "Wrong overlay group for plane #{0}", overlayIndex);
				Assert.AreEqual(2, overlayPlane.Index, "Wrong overlay index for plane #{0}", overlayIndex);
				Assert.AreEqual(0x040000, overlayPlane.TagOffset, "Wrong tag offset for plane #{0}", overlayIndex);

				// assert overlay plane encoding detection
				Assert.AreEqual(true, overlayPlane.HasOverlayData, "Incorrect OverlayData detection for plane #{0}", overlayIndex);
				Assert.AreEqual(false, overlayPlane.IsBigEndianOW, "Incorrect OW/OB detection for plane #{0}", overlayIndex);
				Assert.AreEqual(false, overlayPlane.IsMultiFrame, "Incorrect multiframe detection for plane #{0}", overlayIndex);
				Assert.AreEqual(67*71, overlayPlane.GetOverlayFrameLength(), "Incorrect frame size computation for plane #{0}", overlayIndex);

				// assert overlay plane basic attribute values
				Assert.AreEqual(67, overlayPlane.OverlayRows, "Wrong overlay rows for plane #{0}", overlayIndex);
				Assert.AreEqual(71, overlayPlane.OverlayColumns, "Wrong overlay columns for plane #{0}", overlayIndex);
				Assert.AreEqual(0, overlayPlane.OverlayBitPosition, "Wrong overlay bit position for plane #{0}", overlayIndex);
				Assert.AreEqual(1, overlayPlane.OverlayBitsAllocated, "Wrong overlay bits allocated for plane #{0}", overlayIndex);
				Assert.AreEqual(OverlayType.R, overlayPlane.OverlayType, "Wrong overlay type for plane #{0}", overlayIndex);
				Assert.AreEqual(new Point(4, 5), overlayPlane.OverlayOrigin, "Wrong overlay origin for plane #{0}", overlayIndex);

				// assert overlay plane multiframe attribute values
				Assert.AreEqual(null, overlayPlane.ImageFrameOrigin, "Wrong image frame origin for plane #{0}", overlayIndex);
				Assert.AreEqual(null, overlayPlane.NumberOfFramesInOverlay, "Wrong number of frames in overlay for plane #{0}", overlayIndex);
			}

			// test overlay group 601A
			{
				const int overlayIndex = 13;
				var overlayPlane = module[overlayIndex];

				// assert overlay plane identification
				Assert.AreEqual(0x601A, overlayPlane.Group, "Wrong overlay group for plane #{0}", overlayIndex);
				Assert.AreEqual(13, overlayPlane.Index, "Wrong overlay index for plane #{0}", overlayIndex);
				Assert.AreEqual(0x1A0000, overlayPlane.TagOffset, "Wrong tag offset for plane #{0}", overlayIndex);

				// assert overlay plane encoding detection
				Assert.AreEqual(false, overlayPlane.HasOverlayData, "Incorrect OverlayData detection for plane #{0}", overlayIndex);
				Assert.AreEqual(false, overlayPlane.IsBigEndianOW, "Incorrect OW/OB detection for plane #{0}", overlayIndex);
				Assert.AreEqual(false, overlayPlane.IsMultiFrame, "Incorrect multiframe detection for plane #{0}", overlayIndex);
				Assert.AreEqual(97*101, overlayPlane.GetOverlayFrameLength(), "Incorrect frame size computation for plane #{0}", overlayIndex);

				// assert overlay plane basic attribute values
				Assert.AreEqual(97, overlayPlane.OverlayRows, "Wrong overlay rows for plane #{0}", overlayIndex);
				Assert.AreEqual(101, overlayPlane.OverlayColumns, "Wrong overlay columns for plane #{0}", overlayIndex);
				Assert.AreEqual(14, overlayPlane.OverlayBitPosition, "Wrong overlay bit position for plane #{0}", overlayIndex);
				Assert.AreEqual(16, overlayPlane.OverlayBitsAllocated, "Wrong overlay bits allocated for plane #{0}", overlayIndex);
				Assert.AreEqual(OverlayType.R, overlayPlane.OverlayType, "Wrong overlay type for plane #{0}", overlayIndex);
				Assert.AreEqual(new Point(6, 7), overlayPlane.OverlayOrigin, "Wrong overlay origin for plane #{0}", overlayIndex);

				// assert overlay plane multiframe attribute values
				Assert.AreEqual(null, overlayPlane.ImageFrameOrigin, "Wrong image frame origin for plane #{0}", overlayIndex);
				Assert.AreEqual(null, overlayPlane.NumberOfFramesInOverlay, "Wrong number of frames in overlay for plane #{0}", overlayIndex);
			}
		}

		[Test]
		public void TestOverlayPlaneModuleIod_Multiframe()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int imageRows = 97;
			const int imageColumns = 101;
			const int imageFrames = 11;
			const int overlay1Rows = 103;
			const int overlay1Columns = 89;
			const int overlay1Frames = 1;
			const int overlay2Rows = 67;
			const int overlay2Columns = 71;
			const int overlay2Frames = 7;

			var dataset = new DicomAttributeCollection();
			SetImage(dataset, new byte[imageRows*imageColumns*imageFrames*2], imageRows, imageColumns, imageFrames, 16, 13, 12, false);
			SetOverlay(dataset, 0, new bool[overlay1Rows*overlay1Columns*overlay1Frames], OverlayType.G, new Point(1, 1), overlay1Rows, overlay1Columns, overlay1Frames, 3, bigEndian, false);
			SetOverlay(dataset, 1, new bool[imageRows*imageColumns*imageFrames], OverlayType.G, new Point(2, 3), 15, bigEndian);
			SetOverlay(dataset, 2, new bool[overlay2Rows*overlay2Columns*overlay2Frames], OverlayType.R, new Point(4, 5), overlay2Rows, overlay2Columns, overlay2Frames, bigEndian, true);
			SetOverlay(dataset, 13, new bool[imageRows*imageColumns*imageFrames], OverlayType.R, new Point(6, 7), 14, bigEndian);

			var module = new OverlayPlaneModuleIod(dataset);

			// test overlay group 6000
			{
				const int overlayIndex = 0;
				var overlayPlane = module[overlayIndex];

				// assert overlay plane identification
				Assert.AreEqual(0x6000, overlayPlane.Group, "Wrong overlay group for plane #{0}", overlayIndex);
				Assert.AreEqual(0, overlayPlane.Index, "Wrong overlay index for plane #{0}", overlayIndex);
				Assert.AreEqual(0x000000, overlayPlane.TagOffset, "Wrong tag offset for plane #{0}", overlayIndex);

				// assert overlay plane encoding detection
				Assert.AreEqual(true, overlayPlane.HasOverlayData, "Incorrect OverlayData detection for plane #{0}", overlayIndex);
				Assert.AreEqual(false, overlayPlane.IsBigEndianOW, "Incorrect OW/OB detection for plane #{0}", overlayIndex);
				Assert.AreEqual(true, overlayPlane.IsMultiFrame, "Incorrect multiframe detection for plane #{0}", overlayIndex);
				Assert.AreEqual(103*89, overlayPlane.GetOverlayFrameLength(), "Incorrect frame size computation for plane #{0}", overlayIndex);

				// assert overlay plane basic attribute values
				Assert.AreEqual(103, overlayPlane.OverlayRows, "Wrong overlay rows for plane #{0}", overlayIndex);
				Assert.AreEqual(89, overlayPlane.OverlayColumns, "Wrong overlay columns for plane #{0}", overlayIndex);
				Assert.AreEqual(0, overlayPlane.OverlayBitPosition, "Wrong overlay bit position for plane #{0}", overlayIndex);
				Assert.AreEqual(1, overlayPlane.OverlayBitsAllocated, "Wrong overlay bits allocated for plane #{0}", overlayIndex);
				Assert.AreEqual(OverlayType.G, overlayPlane.OverlayType, "Wrong overlay type for plane #{0}", overlayIndex);
				Assert.AreEqual(new Point(1, 1), overlayPlane.OverlayOrigin, "Wrong overlay origin for plane #{0}", overlayIndex);

				// assert overlay plane multiframe attribute values
				Assert.AreEqual(3, overlayPlane.ImageFrameOrigin, "Wrong image frame origin for plane #{0}", overlayIndex);
				Assert.AreEqual(1, overlayPlane.NumberOfFramesInOverlay, "Wrong number of frames in overlay for plane #{0}", overlayIndex);
			}

			// test overlay group 6002
			{
				const int overlayIndex = 1;
				var overlayPlane = module[overlayIndex];

				// assert overlay plane identification
				Assert.AreEqual(0x6002, overlayPlane.Group, "Wrong overlay group for plane #{0}", overlayIndex);
				Assert.AreEqual(1, overlayPlane.Index, "Wrong overlay index for plane #{0}", overlayIndex);
				Assert.AreEqual(0x020000, overlayPlane.TagOffset, "Wrong tag offset for plane #{0}", overlayIndex);

				// assert overlay plane encoding detection
				Assert.AreEqual(false, overlayPlane.HasOverlayData, "Incorrect OverlayData detection for plane #{0}", overlayIndex);
				Assert.AreEqual(false, overlayPlane.IsBigEndianOW, "Incorrect OW/OB detection for plane #{0}", overlayIndex);
				Assert.AreEqual(true, overlayPlane.IsMultiFrame, "Incorrect multiframe detection for plane #{0}", overlayIndex);
				Assert.AreEqual(97*101, overlayPlane.GetOverlayFrameLength(), "Incorrect frame size computation for plane #{0}", overlayIndex);

				// assert overlay plane basic attribute values
				Assert.AreEqual(97, overlayPlane.OverlayRows, "Wrong overlay rows for plane #{0}", overlayIndex);
				Assert.AreEqual(101, overlayPlane.OverlayColumns, "Wrong overlay columns for plane #{0}", overlayIndex);
				Assert.AreEqual(15, overlayPlane.OverlayBitPosition, "Wrong overlay bit position for plane #{0}", overlayIndex);
				Assert.AreEqual(16, overlayPlane.OverlayBitsAllocated, "Wrong overlay bits allocated for plane #{0}", overlayIndex);
				Assert.AreEqual(OverlayType.G, overlayPlane.OverlayType, "Wrong overlay type for plane #{0}", overlayIndex);
				Assert.AreEqual(new Point(2, 3), overlayPlane.OverlayOrigin, "Wrong overlay origin for plane #{0}", overlayIndex);

				// assert overlay plane multiframe attribute values
				Assert.AreEqual(1, overlayPlane.ImageFrameOrigin.GetValueOrDefault(1), "Wrong image frame origin for plane #{0}", overlayIndex);
				Assert.AreEqual(11, overlayPlane.NumberOfFramesInOverlay, "Wrong number of frames in overlay for plane #{0}", overlayIndex);
			}

			// test overlay group 6004
			{
				const int overlayIndex = 2;
				var overlayPlane = module[overlayIndex];

				// assert overlay plane identification
				Assert.AreEqual(0x6004, overlayPlane.Group, "Wrong overlay group for plane #{0}", overlayIndex);
				Assert.AreEqual(2, overlayPlane.Index, "Wrong overlay index for plane #{0}", overlayIndex);
				Assert.AreEqual(0x040000, overlayPlane.TagOffset, "Wrong tag offset for plane #{0}", overlayIndex);

				// assert overlay plane encoding detection
				Assert.AreEqual(true, overlayPlane.HasOverlayData, "Incorrect OverlayData detection for plane #{0}", overlayIndex);
				Assert.AreEqual(false, overlayPlane.IsBigEndianOW, "Incorrect OW/OB detection for plane #{0}", overlayIndex);
				Assert.AreEqual(true, overlayPlane.IsMultiFrame, "Incorrect multiframe detection for plane #{0}", overlayIndex);
				Assert.AreEqual(67*71, overlayPlane.GetOverlayFrameLength(), "Incorrect frame size computation for plane #{0}", overlayIndex);

				// assert overlay plane basic attribute values
				Assert.AreEqual(67, overlayPlane.OverlayRows, "Wrong overlay rows for plane #{0}", overlayIndex);
				Assert.AreEqual(71, overlayPlane.OverlayColumns, "Wrong overlay columns for plane #{0}", overlayIndex);
				Assert.AreEqual(0, overlayPlane.OverlayBitPosition, "Wrong overlay bit position for plane #{0}", overlayIndex);
				Assert.AreEqual(1, overlayPlane.OverlayBitsAllocated, "Wrong overlay bits allocated for plane #{0}", overlayIndex);
				Assert.AreEqual(OverlayType.R, overlayPlane.OverlayType, "Wrong overlay type for plane #{0}", overlayIndex);
				Assert.AreEqual(new Point(4, 5), overlayPlane.OverlayOrigin, "Wrong overlay origin for plane #{0}", overlayIndex);

				// assert overlay plane multiframe attribute values
				Assert.AreEqual(null, overlayPlane.ImageFrameOrigin, "Wrong image frame origin for plane #{0}", overlayIndex);
				Assert.AreEqual(7, overlayPlane.NumberOfFramesInOverlay, "Wrong number of frames in overlay for plane #{0}", overlayIndex);
			}

			// test overlay group 601A
			{
				const int overlayIndex = 13;
				var overlayPlane = module[overlayIndex];

				// assert overlay plane identification
				Assert.AreEqual(0x601A, overlayPlane.Group, "Wrong overlay group for plane #{0}", overlayIndex);
				Assert.AreEqual(13, overlayPlane.Index, "Wrong overlay index for plane #{0}", overlayIndex);
				Assert.AreEqual(0x1A0000, overlayPlane.TagOffset, "Wrong tag offset for plane #{0}", overlayIndex);

				// assert overlay plane encoding detection
				Assert.AreEqual(false, overlayPlane.HasOverlayData, "Incorrect OverlayData detection for plane #{0}", overlayIndex);
				Assert.AreEqual(false, overlayPlane.IsBigEndianOW, "Incorrect OW/OB detection for plane #{0}", overlayIndex);
				Assert.AreEqual(true, overlayPlane.IsMultiFrame, "Incorrect multiframe detection for plane #{0}", overlayIndex);
				Assert.AreEqual(97*101, overlayPlane.GetOverlayFrameLength(), "Incorrect frame size computation for plane #{0}", overlayIndex);

				// assert overlay plane basic attribute values
				Assert.AreEqual(97, overlayPlane.OverlayRows, "Wrong overlay rows for plane #{0}", overlayIndex);
				Assert.AreEqual(101, overlayPlane.OverlayColumns, "Wrong overlay columns for plane #{0}", overlayIndex);
				Assert.AreEqual(14, overlayPlane.OverlayBitPosition, "Wrong overlay bit position for plane #{0}", overlayIndex);
				Assert.AreEqual(16, overlayPlane.OverlayBitsAllocated, "Wrong overlay bits allocated for plane #{0}", overlayIndex);
				Assert.AreEqual(OverlayType.R, overlayPlane.OverlayType, "Wrong overlay type for plane #{0}", overlayIndex);
				Assert.AreEqual(new Point(6, 7), overlayPlane.OverlayOrigin, "Wrong overlay origin for plane #{0}", overlayIndex);

				// assert overlay plane multiframe attribute values
				Assert.AreEqual(1, overlayPlane.ImageFrameOrigin.GetValueOrDefault(1), "Wrong image frame origin for plane #{0}", overlayIndex);
				Assert.AreEqual(11, overlayPlane.NumberOfFramesInOverlay, "Wrong number of frames in overlay for plane #{0}", overlayIndex);
			}
		}

		[Test]
		public void TestOverlayPlaneModuleIod_MultiframeImageSingleFrameOverlay()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int imageRows = 97;
			const int imageColumns = 101;
			const int imageFrames = 11;
			const int overlay2Rows = 67;
			const int overlay2Columns = 71;

			var dataset = new DicomAttributeCollection();
			SetImage(dataset, new byte[imageRows*imageColumns*imageFrames*2], imageRows, imageColumns, imageFrames, 16, 13, 12, false);
			SetOverlay(dataset, 2, new bool[overlay2Rows*overlay2Columns], OverlayType.R, new Point(4, 5), overlay2Rows, overlay2Columns, bigEndian, true);

			var module = new OverlayPlaneModuleIod(dataset);

			// test overlay group 6004

			const int overlayIndex = 2;
			var overlayPlane = module[overlayIndex];

			// assert overlay plane identification
			Assert.AreEqual(0x6004, overlayPlane.Group, "Wrong overlay group for plane #{0}", overlayIndex);
			Assert.AreEqual(2, overlayPlane.Index, "Wrong overlay index for plane #{0}", overlayIndex);
			Assert.AreEqual(0x040000, overlayPlane.TagOffset, "Wrong tag offset for plane #{0}", overlayIndex);

			// assert overlay plane encoding detection
			Assert.AreEqual(true, overlayPlane.HasOverlayData, "Incorrect OverlayData detection for plane #{0}", overlayIndex);
			Assert.AreEqual(false, overlayPlane.IsBigEndianOW, "Incorrect OW/OB detection for plane #{0}", overlayIndex);
			Assert.AreEqual(false, overlayPlane.IsMultiFrame, "Incorrect multiframe detection for plane #{0}", overlayIndex);
			Assert.AreEqual(67*71, overlayPlane.GetOverlayFrameLength(), "Incorrect frame size computation for plane #{0}", overlayIndex);

			// assert overlay plane basic attribute values
			Assert.AreEqual(67, overlayPlane.OverlayRows, "Wrong overlay rows for plane #{0}", overlayIndex);
			Assert.AreEqual(71, overlayPlane.OverlayColumns, "Wrong overlay columns for plane #{0}", overlayIndex);
			Assert.AreEqual(0, overlayPlane.OverlayBitPosition, "Wrong overlay bit position for plane #{0}", overlayIndex);
			Assert.AreEqual(1, overlayPlane.OverlayBitsAllocated, "Wrong overlay bits allocated for plane #{0}", overlayIndex);
			Assert.AreEqual(OverlayType.R, overlayPlane.OverlayType, "Wrong overlay type for plane #{0}", overlayIndex);
			Assert.AreEqual(new Point(4, 5), overlayPlane.OverlayOrigin, "Wrong overlay origin for plane #{0}", overlayIndex);

			// assert overlay plane multiframe attribute values
			Assert.AreEqual(null, overlayPlane.ImageFrameOrigin, "Wrong image frame origin for plane #{0}", overlayIndex);
			Assert.AreEqual(null, overlayPlane.NumberOfFramesInOverlay, "Wrong number of frames in overlay for plane #{0}", overlayIndex);
		}

		#endregion

		#region GetRelevantOverlayFrame Tests

		[Test]
		public void TestGetRelevantOverlayFrame_MultiframeEmbedded()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int rows = 97;
			const int columns = 101;
			const int frames = 7;

			const int overlayIndex = 0;
			var dataset = new DicomAttributeCollection();
			SetImage(dataset, new byte[rows*columns*2*frames], rows, columns, frames, 16, 12, 11, false);
			SetOverlay(dataset, overlayIndex, new bool[rows*columns*frames], OverlayType.G, new Point(1, 1), 12, bigEndian);

			var module = new OverlayPlaneModuleIod(dataset);
			var overlayPlane = module[overlayIndex];

			int actualOverlayFrame;

			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(-1, frames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #-1");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(0, frames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #0");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(8, frames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #8");

			// all valid image frame inputs should map 1-to-1 with the same numbered overlay frame

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(1, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #1");
			Assert.AreEqual(1, actualOverlayFrame, "Wrong overlay frame matched to image frame #1");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(2, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #2");
			Assert.AreEqual(2, actualOverlayFrame, "Wrong overlay frame matched to image frame #2");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(3, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #3");
			Assert.AreEqual(3, actualOverlayFrame, "Wrong overlay frame matched to image frame #3");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(4, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #4");
			Assert.AreEqual(4, actualOverlayFrame, "Wrong overlay frame matched to image frame #4");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(5, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #5");
			Assert.AreEqual(5, actualOverlayFrame, "Wrong overlay frame matched to image frame #5");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(6, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #6");
			Assert.AreEqual(6, actualOverlayFrame, "Wrong overlay frame matched to image frame #6");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(7, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #7");
			Assert.AreEqual(7, actualOverlayFrame, "Wrong overlay frame matched to image frame #7");
		}

		[Test]
		public void TestGetRelevantOverlayFrame_SingleFrameEmbedded()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int rows = 97;
			const int columns = 101;

			const int overlayIndex = 0;
			var dataset = new DicomAttributeCollection();
			SetImage(dataset, new byte[rows*columns*2], rows, columns, 1, 16, 12, 11, false);
			SetOverlay(dataset, overlayIndex, new bool[rows*columns], OverlayType.G, new Point(1, 1), 12, bigEndian);

			var module = new OverlayPlaneModuleIod(dataset);
			var overlayPlane = module[overlayIndex];

			int actualOverlayFrame;

			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(-1, 1, out actualOverlayFrame), "Should not any matching overlay frame for image frame #-1");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(0, 1, out actualOverlayFrame), "Should not any matching overlay frame for image frame #0");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(8, 1, out actualOverlayFrame), "Should not any matching overlay frame for image frame #8");

			// in the single frame case, there's only one pairing

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(1, 1, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #1");
			Assert.AreEqual(1, actualOverlayFrame, "Wrong overlay frame matched to image frame #1");
		}

		[Test]
		public void TestGetRelevantOverlayFrame_MultiframeOverlayData()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int rows = 97;
			const int columns = 101;
			const int frames = 7;

			const int overlayIndex = 0;
			var dataset = new DicomAttributeCollection();
			SetOverlay(dataset, overlayIndex, new bool[rows*columns*frames], OverlayType.G, new Point(1, 1), rows, columns, frames, null, bigEndian, false);

			var module = new OverlayPlaneModuleIod(dataset);
			var overlayPlane = module[overlayIndex];

			int actualOverlayFrame;

			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(-1, frames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #-1");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(0, frames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #0");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(8, frames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #8");

			// all valid image frame inputs should map 1-to-1 with the same numbered overlay frame

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(1, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #1");
			Assert.AreEqual(1, actualOverlayFrame, "Wrong overlay frame matched to image frame #1");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(2, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #2");
			Assert.AreEqual(2, actualOverlayFrame, "Wrong overlay frame matched to image frame #2");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(3, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #3");
			Assert.AreEqual(3, actualOverlayFrame, "Wrong overlay frame matched to image frame #3");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(4, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #4");
			Assert.AreEqual(4, actualOverlayFrame, "Wrong overlay frame matched to image frame #4");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(5, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #5");
			Assert.AreEqual(5, actualOverlayFrame, "Wrong overlay frame matched to image frame #5");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(6, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #6");
			Assert.AreEqual(6, actualOverlayFrame, "Wrong overlay frame matched to image frame #6");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(7, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #7");
			Assert.AreEqual(7, actualOverlayFrame, "Wrong overlay frame matched to image frame #7");
		}

		[Test]
		public void TestGetRelevantOverlayFrame_MultiframeOverlayDataWithOrigin()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int rows = 97;
			const int columns = 101;
			const int frames = 5;

			const int imageFrames = 7;
			const int imageFrameOrigin = 2;

			const int overlayIndex = 0;
			var dataset = new DicomAttributeCollection();
			SetOverlay(dataset, overlayIndex, new bool[rows*columns*frames], OverlayType.G, new Point(1, 1), rows, columns, frames, imageFrameOrigin, bigEndian, false);

			var module = new OverlayPlaneModuleIod(dataset);
			var overlayPlane = module[overlayIndex];

			int actualOverlayFrame;

			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(-1, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #-1");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(0, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #0");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(8, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #8");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(1, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #1");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(7, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #7");

			// all valid image frame inputs should map 1-to-1 with an overlay frame starting with image frame #2 and overlay frame #1

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(2, imageFrames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #2");
			Assert.AreEqual(1, actualOverlayFrame, "Wrong overlay frame matched to image frame #2");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(3, imageFrames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #3");
			Assert.AreEqual(2, actualOverlayFrame, "Wrong overlay frame matched to image frame #3");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(4, imageFrames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #4");
			Assert.AreEqual(3, actualOverlayFrame, "Wrong overlay frame matched to image frame #4");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(5, imageFrames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #5");
			Assert.AreEqual(4, actualOverlayFrame, "Wrong overlay frame matched to image frame #5");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(6, imageFrames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #6");
			Assert.AreEqual(5, actualOverlayFrame, "Wrong overlay frame matched to image frame #6");
		}

		[Test]
		public void TestGetRelevantOverlayFrame_SingleMultiframeOverlayData()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int rows = 97;
			const int columns = 101;

			const int imageFrames = 7;

			const int overlayIndex = 0;
			var dataset = new DicomAttributeCollection();
			SetOverlay(dataset, overlayIndex, new bool[rows*columns], OverlayType.G, new Point(1, 1), rows, columns, 1, null, bigEndian, false);

			var module = new OverlayPlaneModuleIod(dataset);
			var overlayPlane = module[overlayIndex];

			int actualOverlayFrame;

			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(-1, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #-1");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(0, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #0");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(2, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #2");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(5, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #5");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(8, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #8");

			// the only valid mapping is for the image frame identified by the origin

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(1, imageFrames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #1");
			Assert.AreEqual(1, actualOverlayFrame, "Wrong overlay frame matched to image frame #1");
		}

		[Test]
		public void TestGetRelevantOverlayFrame_SingleMultiframeOverlayDataWithOrigin()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int rows = 97;
			const int columns = 101;

			const int imageFrames = 7;

			const int overlayIndex = 0;
			var dataset = new DicomAttributeCollection();
			SetOverlay(dataset, overlayIndex, new bool[rows*columns], OverlayType.G, new Point(1, 1), rows, columns, 1, 5, bigEndian, false);

			var module = new OverlayPlaneModuleIod(dataset);
			var overlayPlane = module[overlayIndex];

			int actualOverlayFrame;

			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(-1, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #-1");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(0, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #0");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(1, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #1");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(2, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #2");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(8, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #8");

			// the only valid mapping is for the image frame identified by the origin

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(5, imageFrames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #5");
			Assert.AreEqual(1, actualOverlayFrame, "Wrong overlay frame matched to image frame #5");
		}

		[Test]
		public void TestGetRelevantOverlayFrame_SingleFrameOverlayData()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int rows = 97;
			const int columns = 101;

			const int imageFrames = 7;

			const int overlayIndex = 0;
			var dataset = new DicomAttributeCollection();
			SetOverlay(dataset, overlayIndex, new bool[rows*columns], OverlayType.G, new Point(1, 1), rows, columns, null, null, bigEndian, false);

			var module = new OverlayPlaneModuleIod(dataset);
			var overlayPlane = module[overlayIndex];

			int actualOverlayFrame;

			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(-1, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #-1");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(0, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #0");
			Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(8, imageFrames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #8");

			// in the single frame case, all valid image frame inputs should map to overlay frame #1

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(1, imageFrames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #1");
			Assert.AreEqual(1, actualOverlayFrame, "Wrong overlay frame matched to image frame #1");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(2, imageFrames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #2");
			Assert.AreEqual(1, actualOverlayFrame, "Wrong overlay frame matched to image frame #2");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(3, imageFrames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #3");
			Assert.AreEqual(1, actualOverlayFrame, "Wrong overlay frame matched to image frame #3");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(4, imageFrames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #4");
			Assert.AreEqual(1, actualOverlayFrame, "Wrong overlay frame matched to image frame #4");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(5, imageFrames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #5");
			Assert.AreEqual(1, actualOverlayFrame, "Wrong overlay frame matched to image frame #5");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(6, imageFrames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #6");
			Assert.AreEqual(1, actualOverlayFrame, "Wrong overlay frame matched to image frame #6");

			Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(7, imageFrames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #7");
			Assert.AreEqual(1, actualOverlayFrame, "Wrong overlay frame matched to image frame #7");
		}

		#endregion

		#region ComputeOverlayDataBitOffset Tests

		[Test]
		public void TestComputeOverlayDataBitOffset_MultiframeOverlayData()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int rows = 97;
			const int columns = 101;
			const int frames = 7;

			const int overlayIndex = 0;
			var dataset = new DicomAttributeCollection();
			SetOverlay(dataset, overlayIndex, new bool[rows*columns*frames], OverlayType.G, new Point(1, 1), rows, columns, frames, null, bigEndian, false);

			var module = new OverlayPlaneModuleIod(dataset);
			var overlayPlane = module[overlayIndex];

			int actualBitOffset;

			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(-1, out actualBitOffset), "Should not be able to compute bit offset for frame number -1");
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(0, out actualBitOffset), "Should not be able to compute bit offset for frame number 0");
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(8, out actualBitOffset), "Should not be able to compute bit offset for frame number 8");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(1, out actualBitOffset), "Should be able to compute bit offset for frame number 1");
			Assert.AreEqual(0*rows*columns, actualBitOffset, "Wrong offset computed for frame number 1");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(2, out actualBitOffset), "Should be able to compute bit offset for frame number 2");
			Assert.AreEqual(1*rows*columns, actualBitOffset, "Wrong offset computed for frame number 2");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(3, out actualBitOffset), "Should be able to compute bit offset for frame number 3");
			Assert.AreEqual(2*rows*columns, actualBitOffset, "Wrong offset computed for frame number 3");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(4, out actualBitOffset), "Should be able to compute bit offset for frame number 4");
			Assert.AreEqual(3*rows*columns, actualBitOffset, "Wrong offset computed for frame number 4");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(5, out actualBitOffset), "Should be able to compute bit offset for frame number 5");
			Assert.AreEqual(4*rows*columns, actualBitOffset, "Wrong offset computed for frame number 5");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(6, out actualBitOffset), "Should be able to compute bit offset for frame number 6");
			Assert.AreEqual(5*rows*columns, actualBitOffset, "Wrong offset computed for frame number 6");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(7, out actualBitOffset), "Should be able to compute bit offset for frame number 7");
			Assert.AreEqual(6*rows*columns, actualBitOffset, "Wrong offset computed for frame number 7");
		}

		[Test]
		public void TestComputeOverlayDataBitOffset_MultiframeOverlayDataWithOrigin()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int rows = 97;
			const int columns = 101;
			const int frames = 7;

			// just because you're getting overlay frame #2 for image frame #4 doesn't change the offset where overlay frame #2 can be found!!
			const int imageFrameOrigin = 3;

			const int overlayIndex = 0;
			var dataset = new DicomAttributeCollection();
			SetOverlay(dataset, overlayIndex, new bool[rows*columns*frames], OverlayType.G, new Point(1, 1), rows, columns, frames, imageFrameOrigin, bigEndian, false);

			var module = new OverlayPlaneModuleIod(dataset);
			var overlayPlane = module[overlayIndex];

			int actualBitOffset;

			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(-1, out actualBitOffset), "Should not be able to compute bit offset for frame number -1");
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(0, out actualBitOffset), "Should not be able to compute bit offset for frame number 0");
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(8, out actualBitOffset), "Should not be able to compute bit offset for frame number 8");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(1, out actualBitOffset), "Should be able to compute bit offset for frame number 1");
			Assert.AreEqual(0*rows*columns, actualBitOffset, "Wrong offset computed for frame number 1");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(2, out actualBitOffset), "Should be able to compute bit offset for frame number 2");
			Assert.AreEqual(1*rows*columns, actualBitOffset, "Wrong offset computed for frame number 2");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(3, out actualBitOffset), "Should be able to compute bit offset for frame number 3");
			Assert.AreEqual(2*rows*columns, actualBitOffset, "Wrong offset computed for frame number 3");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(4, out actualBitOffset), "Should be able to compute bit offset for frame number 4");
			Assert.AreEqual(3*rows*columns, actualBitOffset, "Wrong offset computed for frame number 4");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(5, out actualBitOffset), "Should be able to compute bit offset for frame number 5");
			Assert.AreEqual(4*rows*columns, actualBitOffset, "Wrong offset computed for frame number 5");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(6, out actualBitOffset), "Should be able to compute bit offset for frame number 6");
			Assert.AreEqual(5*rows*columns, actualBitOffset, "Wrong offset computed for frame number 6");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(7, out actualBitOffset), "Should be able to compute bit offset for frame number 7");
			Assert.AreEqual(6*rows*columns, actualBitOffset, "Wrong offset computed for frame number 7");
		}

		[Test]
		public void TestComputeOverlayDataBitOffset_SingleMultiframeOverlayData()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int rows = 97;
			const int columns = 101;

			const int overlayIndex = 0;
			var dataset = new DicomAttributeCollection();
			SetOverlay(dataset, overlayIndex, new bool[rows*columns], OverlayType.G, new Point(1, 1), rows, columns, 1, null, bigEndian, false);

			var module = new OverlayPlaneModuleIod(dataset);
			var overlayPlane = module[overlayIndex];

			int actualBitOffset;
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(-1, out actualBitOffset), "Should not be able to compute bit offset for frame number -1");
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(0, out actualBitOffset), "Should not be able to compute bit offset for frame number 0");
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(2, out actualBitOffset), "Should not be able to compute bit offset for frame number 2");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(1, out actualBitOffset), "Should be able to compute bit offset for frame number 1");
			Assert.AreEqual(0, actualBitOffset, "Wrong offset computed for frame number 1");
		}

		[Test]
		public void TestComputeOverlayDataBitOffset_SingleFrameOverlayData()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int rows = 97;
			const int columns = 101;

			const int overlayIndex = 0;
			var dataset = new DicomAttributeCollection();
			SetOverlay(dataset, overlayIndex, new bool[rows*columns], OverlayType.G, new Point(1, 1), rows, columns, null, null, bigEndian, false);

			var module = new OverlayPlaneModuleIod(dataset);
			var overlayPlane = module[overlayIndex];

			int actualBitOffset;
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(-1, out actualBitOffset), "Should not be able to compute bit offset for frame number -1");
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(0, out actualBitOffset), "Should not be able to compute bit offset for frame number 0");
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(2, out actualBitOffset), "Should not be able to compute bit offset for frame number 2");

			Assert.IsTrue(overlayPlane.TryComputeOverlayDataBitOffset(1, out actualBitOffset), "Should be able to compute bit offset for frame number 1");
			Assert.AreEqual(0, actualBitOffset, "Wrong offset computed for frame number 1");
		}

		[Test]
		public void TestComputeOverlayDataBitOffset_MultiframeEmbedded()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int rows = 97;
			const int columns = 101;
			const int frames = 7;

			const int overlayIndex = 0;
			var dataset = new DicomAttributeCollection();
			SetImage(dataset, new byte[rows*columns*frames*2], rows, columns, frames, 16, 12, 14, false);
			SetOverlay(dataset, overlayIndex, new bool[rows*columns*frames], OverlayType.G, new Point(1, 1), 15, bigEndian);

			var module = new OverlayPlaneModuleIod(dataset);
			var overlayPlane = module[overlayIndex];

			int actualBitOffset;

			// bit offset doesn't make any sense for embedded data

			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(-1, out actualBitOffset), "Should not be able to compute bit offset for frame number -1");
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(0, out actualBitOffset), "Should not be able to compute bit offset for frame number 0");
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(1, out actualBitOffset), "Should not be able to compute bit offset for frame number 1");
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(2, out actualBitOffset), "Should not be able to compute bit offset for frame number 2");
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(8, out actualBitOffset), "Should not be able to compute bit offset for frame number 8");
		}

		[Test]
		public void TestComputeOverlayDataBitOffset_SingleFrameEmbedded()
		{
			const bool bigEndian = false;

			// these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
			const int rows = 97;
			const int columns = 101;

			const int overlayIndex = 0;
			var dataset = new DicomAttributeCollection();
			SetImage(dataset, new byte[rows*columns*2], rows, columns, 1, 16, 12, 14, false);
			SetOverlay(dataset, overlayIndex, new bool[rows*columns], OverlayType.G, new Point(1, 1), 15, bigEndian);

			var module = new OverlayPlaneModuleIod(dataset);
			var overlayPlane = module[overlayIndex];

			int actualBitOffset;

			// bit offset doesn't make any sense for embedded data

			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(-1, out actualBitOffset), "Should not be able to compute bit offset for frame number -1");
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(0, out actualBitOffset), "Should not be able to compute bit offset for frame number 0");
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(1, out actualBitOffset), "Should not be able to compute bit offset for frame number 1");
			Assert.IsFalse(overlayPlane.TryComputeOverlayDataBitOffset(2, out actualBitOffset), "Should not be able to compute bit offset for frame number 2");
		}

        [Test]
        public void TestConversion_MultiframeEmbedded()
        {
            const bool bigEndian = false;

            // these parameters should be kept prime numbers so that we can exercise the overlay handling for rows/frames that cross byte boundaries
            const int rows = 97;
            const int columns = 101;
            const int frames = 7;

            const int overlayIndex = 0;

            var overlayData = new bool[rows*columns*frames];
            for (int i = 0; i < frames; i++)
            {
                overlayData[(i * rows*columns) + 1] = true;
                overlayData[(i * rows * columns) + 10] = true;
                overlayData[(i * rows * columns) + 100] = true;
                overlayData[(i * rows * columns) + 225] = true;
                overlayData[(i * rows * columns) + 1000] = true;
                overlayData[(i * rows * columns) + 1001] = true;
                overlayData[(i * rows * columns) + (rows * columns) - 1] = true;
            }

            var dataset = new DicomAttributeCollection();
            SetImage(dataset, new byte[rows * columns * 2 * frames], rows, columns, frames, 16, 12, 11, false);
            SetOverlay(dataset, overlayIndex, overlayData, OverlayType.G, new Point(1, 1), 12, bigEndian);

            DicomUncompressedPixelData pd = new DicomUncompressedPixelData(dataset);

            OverlayPlane overlayPlane = new OverlayPlane(overlayIndex, dataset);
            Assert.IsTrue(overlayPlane.ExtractEmbeddedOverlay(pd), "Failed to convert to Non-Embedded overlay plane.");

            
            int actualOverlayFrame;

            Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(-1, frames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #-1");
            Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(0, frames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #0");
            Assert.IsFalse(overlayPlane.TryGetRelevantOverlayFrame(8, frames, out actualOverlayFrame), "Should not any matching overlay frame for image frame #8");

            // all valid image frame inputs should map 1-to-1 with the same numbered overlay frame

            Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(1, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #1");
            Assert.AreEqual(1, actualOverlayFrame, "Wrong overlay frame matched to image frame #1");

            Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(2, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #2");
            Assert.AreEqual(2, actualOverlayFrame, "Wrong overlay frame matched to image frame #2");

            Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(3, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #3");
            Assert.AreEqual(3, actualOverlayFrame, "Wrong overlay frame matched to image frame #3");

            Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(4, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #4");
            Assert.AreEqual(4, actualOverlayFrame, "Wrong overlay frame matched to image frame #4");

            Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(5, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #5");
            Assert.AreEqual(5, actualOverlayFrame, "Wrong overlay frame matched to image frame #5");

            Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(6, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #6");
            Assert.AreEqual(6, actualOverlayFrame, "Wrong overlay frame matched to image frame #6");

            Assert.IsTrue(overlayPlane.TryGetRelevantOverlayFrame(7, frames, out actualOverlayFrame), "Should be able to match an overlay frame to image frame #7");
            Assert.AreEqual(7, actualOverlayFrame, "Wrong overlay frame matched to image frame #7");
        }

		#endregion

		#region Utility Helpers

		private static void SetImage(IDicomAttributeProvider dataset, byte[] pixelData, int rows, int columns, int frames, int bitsAllocated, int bitsStored, int highBit, bool isSigned)
		{
			DicomOverlayTestHelper.SetImagePixels(dataset, pixelData, rows, columns, frames, bitsAllocated, bitsStored, highBit, isSigned);
		}

		private static void SetOverlay(IDicomAttributeProvider dataset, int overlayIndex, bool[] overlayData, OverlayType type, Point origin, int rows, int columns, bool bigEndian, bool useOW)
		{
			DicomOverlayTestHelper.AddOverlayPlane(dataset, overlayIndex, overlayData, type, origin, rows, columns, bigEndian, useOW);
		}

		private static void SetOverlay(IDicomAttributeProvider dataset, int overlayIndex, bool[] overlayData, OverlayType type, Point origin, int rows, int columns, int? frames, bool bigEndian, bool useOW)
		{
			DicomOverlayTestHelper.AddOverlayPlane(dataset, overlayIndex, overlayData, type, origin, rows, columns, frames, bigEndian, useOW);
		}

		private static void SetOverlay(IDicomAttributeProvider dataset, int overlayIndex, bool[] overlayData, OverlayType type, Point origin, int rows, int columns, int? frames, int? frameOrigin, bool bigEndian, bool useOW)
		{
			DicomOverlayTestHelper.AddOverlayPlane(dataset, overlayIndex, overlayData, type, origin, rows, columns, frames, frameOrigin, bigEndian, useOW);
		}

		private static void SetOverlay(IDicomAttributeProvider dataset, int overlayIndex, bool[] overlayData, OverlayType type, Point origin, int bitPosition, bool bigEndian)
		{
			DicomOverlayTestHelper.AddOverlayPlane(dataset, overlayIndex, overlayData, type, origin, bitPosition, bigEndian);
		}

		#endregion
	}
}

#endif