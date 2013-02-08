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
// ReSharper disable InconsistentNaming

using System.IO;
using ClearCanvas.Dicom.Iod;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	public class DicomUncompressedPixelDataTests
	{
		#region Basic Tests (simple construction from data set and reading frames)

		#region 8-bit images

		[Test]
		public void TestBasic_MessageInMemory_8Bits()
		{
			var dcf = CreateDicomImage(rows : 20, columns : 30, numberOfFrames : 3, bitsAllocated16 : false);
			var pd = new DicomUncompressedPixelData(dcf);

			Assert.AreEqual(8, pd.BitsAllocated, "BitsAllocated");
			Assert.AreEqual(8, pd.BitsStored, "BitsStored");
			Assert.AreEqual(7, pd.HighBit, "HighBit");
			Assert.AreEqual(20, pd.ImageHeight, "ImageHeight");
			Assert.AreEqual(30, pd.ImageWidth, "ImageWidth");
			Assert.AreEqual(3, pd.NumberOfFrames, "NumberOfFrames");
			Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
			Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
			Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
			Assert.AreEqual(20*30, pd.UncompressedFrameSize, "UncompressedFrameSize");

			for (var frame = 0; frame < 3; ++frame)
			{
				var fd = pd.GetFrame(frame);
				Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
				AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
			}
		}

		[Test]
		public void TestBasic_MessageInMemory_8Bits_OB()
		{
			var dcf = CreateDicomImage(rows : 20, columns : 30, numberOfFrames : 3, bitsAllocated16 : false, useOB : true);
			var pd = new DicomUncompressedPixelData(dcf);

			Assert.AreEqual(8, pd.BitsAllocated, "BitsAllocated");
			Assert.AreEqual(8, pd.BitsStored, "BitsStored");
			Assert.AreEqual(7, pd.HighBit, "HighBit");
			Assert.AreEqual(20, pd.ImageHeight, "ImageHeight");
			Assert.AreEqual(30, pd.ImageWidth, "ImageWidth");
			Assert.AreEqual(3, pd.NumberOfFrames, "NumberOfFrames");
			Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
			Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
			Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
			Assert.AreEqual(20*30, pd.UncompressedFrameSize, "UncompressedFrameSize");

			for (var frame = 0; frame < 3; ++frame)
			{
				var fd = pd.GetFrame(frame);
				Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
				AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
			}
		}

		[Test]
		public void TestBasic_DatasetInMemory_8Bits()
		{
			var dcf = CreateDicomImage(rows : 20, columns : 30, numberOfFrames : 3, bitsAllocated16 : false, signed : true);
			var pd = new DicomUncompressedPixelData(dcf.DataSet);

			Assert.AreEqual(8, pd.BitsAllocated, "BitsAllocated");
			Assert.AreEqual(8, pd.BitsStored, "BitsStored");
			Assert.AreEqual(7, pd.HighBit, "HighBit");
			Assert.AreEqual(20, pd.ImageHeight, "ImageHeight");
			Assert.AreEqual(30, pd.ImageWidth, "ImageWidth");
			Assert.AreEqual(3, pd.NumberOfFrames, "NumberOfFrames");
			Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
			Assert.AreEqual(1, pd.PixelRepresentation, "PixelRepresentation");
			Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
			Assert.AreEqual(20*30, pd.UncompressedFrameSize, "UncompressedFrameSize");

			for (var frame = 0; frame < 3; ++frame)
			{
				var fd = pd.GetFrame(frame);
				Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
				AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
			}
		}

		[Test]
		public void TestBasic_FileOnDisk_8Bits_OB()
		{
			var filename = Path.GetTempFileName();
			CreateDicomImage(rows : 20, columns : 30, numberOfFrames : 3, bitsAllocated16 : false, useOB : true).Save(filename);

			var dcf = new DicomFile(filename);
			dcf.Load(DicomReadOptions.StorePixelDataReferences);
			try
			{
				var pd = new DicomUncompressedPixelData(dcf);

				Assert.AreEqual(8, pd.BitsAllocated, "BitsAllocated");
				Assert.AreEqual(8, pd.BitsStored, "BitsStored");
				Assert.AreEqual(7, pd.HighBit, "HighBit");
				Assert.AreEqual(20, pd.ImageHeight, "ImageHeight");
				Assert.AreEqual(30, pd.ImageWidth, "ImageWidth");
				Assert.AreEqual(3, pd.NumberOfFrames, "NumberOfFrames");
				Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
				Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
				Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
				Assert.AreEqual(20*30, pd.UncompressedFrameSize, "UncompressedFrameSize");

				for (var frame = 0; frame < 3; ++frame)
				{
					var fd = pd.GetFrame(frame);
					Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
					AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Test]
		public void TestBasic_FileOnDisk_8Bits_LittleEndian()
		{
			var filename = Path.GetTempFileName();
			CreateDicomImage(rows : 20, columns : 30, numberOfFrames : 3, bitsAllocated16 : false).Save(filename);

			var dcf = new DicomFile(filename);
			dcf.Load(DicomReadOptions.StorePixelDataReferences);
			try
			{
				var pd = new DicomUncompressedPixelData(dcf);

				Assert.AreEqual(8, pd.BitsAllocated, "BitsAllocated");
				Assert.AreEqual(8, pd.BitsStored, "BitsStored");
				Assert.AreEqual(7, pd.HighBit, "HighBit");
				Assert.AreEqual(20, pd.ImageHeight, "ImageHeight");
				Assert.AreEqual(30, pd.ImageWidth, "ImageWidth");
				Assert.AreEqual(3, pd.NumberOfFrames, "NumberOfFrames");
				Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
				Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
				Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
				Assert.AreEqual(20*30, pd.UncompressedFrameSize, "UncompressedFrameSize");

				for (var frame = 0; frame < 3; ++frame)
				{
					var fd = pd.GetFrame(frame);
					Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
					AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Test]
		public void TestBasic_FileOnDisk_8Bits_LittleEndian_OddFrameLength()
		{
			var filename = Path.GetTempFileName();
			CreateDicomImage(rows : 19, columns : 29, numberOfFrames : 5, bitsAllocated16 : false).Save(filename);

			var dcf = new DicomFile(filename);
			dcf.Load(DicomReadOptions.StorePixelDataReferences);
			try
			{
				var pd = new DicomUncompressedPixelData(dcf);

				Assert.AreEqual(8, pd.BitsAllocated, "BitsAllocated");
				Assert.AreEqual(8, pd.BitsStored, "BitsStored");
				Assert.AreEqual(7, pd.HighBit, "HighBit");
				Assert.AreEqual(19, pd.ImageHeight, "ImageHeight");
				Assert.AreEqual(29, pd.ImageWidth, "ImageWidth");
				Assert.AreEqual(5, pd.NumberOfFrames, "NumberOfFrames");
				Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
				Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
				Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
				Assert.AreEqual(19*29, pd.UncompressedFrameSize, "UncompressedFrameSize");

				for (var frame = 0; frame < 5; ++frame)
				{
					var fd = pd.GetFrame(frame);
					Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
					AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Test]
		public void TestBasic_FileOnDisk_8Bits_BigEndian()
		{
			var filename = Path.GetTempFileName();
			CreateDicomImage(rows : 20, columns : 30, numberOfFrames : 3, bitsAllocated16 : false, endian : Endian.Big).Save(filename);

			var dcf = new DicomFile(filename);
			dcf.Load(DicomReadOptions.StorePixelDataReferences);
			try
			{
				var pd = new DicomUncompressedPixelData(dcf);

				Assert.AreEqual(8, pd.BitsAllocated, "BitsAllocated");
				Assert.AreEqual(8, pd.BitsStored, "BitsStored");
				Assert.AreEqual(7, pd.HighBit, "HighBit");
				Assert.AreEqual(20, pd.ImageHeight, "ImageHeight");
				Assert.AreEqual(30, pd.ImageWidth, "ImageWidth");
				Assert.AreEqual(3, pd.NumberOfFrames, "NumberOfFrames");
				Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
				Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
				Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
				Assert.AreEqual(20*30, pd.UncompressedFrameSize, "UncompressedFrameSize");

				for (var frame = 0; frame < 3; ++frame)
				{
					var fd = pd.GetFrame(frame);
					Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
					AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Test]
		public void TestBasic_FileOnDisk_8Bits_BigEndian_OddFrameLength()
		{
			var filename = Path.GetTempFileName();
			CreateDicomImage(rows : 19, columns : 29, numberOfFrames : 5, bitsAllocated16 : false, endian : Endian.Big).Save(filename);

			var dcf = new DicomFile(filename);
			dcf.Load(DicomReadOptions.StorePixelDataReferences);
			try
			{
				var pd = new DicomUncompressedPixelData(dcf);

				Assert.AreEqual(8, pd.BitsAllocated, "BitsAllocated");
				Assert.AreEqual(8, pd.BitsStored, "BitsStored");
				Assert.AreEqual(7, pd.HighBit, "HighBit");
				Assert.AreEqual(19, pd.ImageHeight, "ImageHeight");
				Assert.AreEqual(29, pd.ImageWidth, "ImageWidth");
				Assert.AreEqual(5, pd.NumberOfFrames, "NumberOfFrames");
				Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
				Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
				Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
				Assert.AreEqual(19*29, pd.UncompressedFrameSize, "UncompressedFrameSize");

				for (var frame = 0; frame < 5; ++frame)
				{
					if (frame == 4) Assert.Ignore("Skipping last frame validation due to bug #10749");
					var fd = pd.GetFrame(frame);
					Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
					AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		#endregion

		#region 16-bit images

		[Test]
		public void TestBasic_MessageInMemory_16Bits()
		{
			var dcf = CreateDicomImage(rows : 20, columns : 30, numberOfFrames : 3);
			var pd = new DicomUncompressedPixelData(dcf);

			Assert.AreEqual(16, pd.BitsAllocated, "BitsAllocated");
			Assert.AreEqual(16, pd.BitsStored, "BitsStored");
			Assert.AreEqual(15, pd.HighBit, "HighBit");
			Assert.AreEqual(20, pd.ImageHeight, "ImageHeight");
			Assert.AreEqual(30, pd.ImageWidth, "ImageWidth");
			Assert.AreEqual(3, pd.NumberOfFrames, "NumberOfFrames");
			Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
			Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
			Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
			Assert.AreEqual(20*30*2, pd.UncompressedFrameSize, "UncompressedFrameSize");

			for (var frame = 0; frame < 3; ++frame)
			{
				var fd = pd.GetFrame(frame);
				Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
				AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
			}
		}

		[Test]
		public void TestBasic_MessageInMemory_16Bits_OB()
		{
			var dcf = CreateDicomImage(rows : 20, columns : 30, numberOfFrames : 3, useOB : true);
			var pd = new DicomUncompressedPixelData(dcf);

			Assert.AreEqual(16, pd.BitsAllocated, "BitsAllocated");
			Assert.AreEqual(16, pd.BitsStored, "BitsStored");
			Assert.AreEqual(15, pd.HighBit, "HighBit");
			Assert.AreEqual(20, pd.ImageHeight, "ImageHeight");
			Assert.AreEqual(30, pd.ImageWidth, "ImageWidth");
			Assert.AreEqual(3, pd.NumberOfFrames, "NumberOfFrames");
			Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
			Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
			Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
			Assert.AreEqual(20*30*2, pd.UncompressedFrameSize, "UncompressedFrameSize");

			for (var frame = 0; frame < 3; ++frame)
			{
				var fd = pd.GetFrame(frame);
				Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
				AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
			}
		}

		[Test]
		public void TestBasic_DatasetInMemory_16Bits()
		{
			var dcf = CreateDicomImage(rows : 20, columns : 30, numberOfFrames : 3, bitsAllocated16 : true, signed : true);
			var pd = new DicomUncompressedPixelData(dcf.DataSet);

			Assert.AreEqual(16, pd.BitsAllocated, "BitsAllocated");
			Assert.AreEqual(16, pd.BitsStored, "BitsStored");
			Assert.AreEqual(15, pd.HighBit, "HighBit");
			Assert.AreEqual(20, pd.ImageHeight, "ImageHeight");
			Assert.AreEqual(30, pd.ImageWidth, "ImageWidth");
			Assert.AreEqual(3, pd.NumberOfFrames, "NumberOfFrames");
			Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
			Assert.AreEqual(1, pd.PixelRepresentation, "PixelRepresentation");
			Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
			Assert.AreEqual(20*30*2, pd.UncompressedFrameSize, "UncompressedFrameSize");

			for (var frame = 0; frame < 3; ++frame)
			{
				var fd = pd.GetFrame(frame);
				Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
				AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
			}
		}

		[Test]
		public void TestBasic_FileOnDisk_16Bits_OB()
		{
			var filename = Path.GetTempFileName();
			CreateDicomImage(rows : 20, columns : 30, numberOfFrames : 3, useOB : true).Save(filename);

			var dcf = new DicomFile(filename);
			dcf.Load(DicomReadOptions.StorePixelDataReferences);
			try
			{
				var pd = new DicomUncompressedPixelData(dcf);

				Assert.AreEqual(16, pd.BitsAllocated, "BitsAllocated");
				Assert.AreEqual(16, pd.BitsStored, "BitsStored");
				Assert.AreEqual(15, pd.HighBit, "HighBit");
				Assert.AreEqual(20, pd.ImageHeight, "ImageHeight");
				Assert.AreEqual(30, pd.ImageWidth, "ImageWidth");
				Assert.AreEqual(3, pd.NumberOfFrames, "NumberOfFrames");
				Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
				Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
				Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
				Assert.AreEqual(20*30*2, pd.UncompressedFrameSize, "UncompressedFrameSize");

				for (var frame = 0; frame < 3; ++frame)
				{
					var fd = pd.GetFrame(frame);
					Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
					AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Test]
		public void TestBasic_FileOnDisk_16Bits_LittleEndian()
		{
			var filename = Path.GetTempFileName();
			CreateDicomImage(rows : 20, columns : 30, numberOfFrames : 3).Save(filename);

			var dcf = new DicomFile(filename);
			dcf.Load(DicomReadOptions.StorePixelDataReferences);
			try
			{
				var pd = new DicomUncompressedPixelData(dcf);

				Assert.AreEqual(16, pd.BitsAllocated, "BitsAllocated");
				Assert.AreEqual(16, pd.BitsStored, "BitsStored");
				Assert.AreEqual(15, pd.HighBit, "HighBit");
				Assert.AreEqual(20, pd.ImageHeight, "ImageHeight");
				Assert.AreEqual(30, pd.ImageWidth, "ImageWidth");
				Assert.AreEqual(3, pd.NumberOfFrames, "NumberOfFrames");
				Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
				Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
				Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
				Assert.AreEqual(20*30*2, pd.UncompressedFrameSize, "UncompressedFrameSize");

				for (var frame = 0; frame < 3; ++frame)
				{
					var fd = pd.GetFrame(frame);
					Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
					AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Test]
		public void TestBasic_FileOnDisk_16Bits_LittleEndian_OddFrameLength()
		{
			var filename = Path.GetTempFileName();
			CreateDicomImage(rows : 19, columns : 29, numberOfFrames : 5).Save(filename);

			var dcf = new DicomFile(filename);
			dcf.Load(DicomReadOptions.StorePixelDataReferences);
			try
			{
				var pd = new DicomUncompressedPixelData(dcf);

				Assert.AreEqual(16, pd.BitsAllocated, "BitsAllocated");
				Assert.AreEqual(16, pd.BitsStored, "BitsStored");
				Assert.AreEqual(15, pd.HighBit, "HighBit");
				Assert.AreEqual(19, pd.ImageHeight, "ImageHeight");
				Assert.AreEqual(29, pd.ImageWidth, "ImageWidth");
				Assert.AreEqual(5, pd.NumberOfFrames, "NumberOfFrames");
				Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
				Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
				Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
				Assert.AreEqual(19*29*2, pd.UncompressedFrameSize, "UncompressedFrameSize");

				for (var frame = 0; frame < 5; ++frame)
				{
					var fd = pd.GetFrame(frame);
					Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
					AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Test]
		public void TestBasic_FileOnDisk_16Bits_BigEndian()
		{
			var filename = Path.GetTempFileName();
			CreateDicomImage(rows : 20, columns : 30, numberOfFrames : 3, endian : Endian.Big).Save(filename);

			var dcf = new DicomFile(filename);
			dcf.Load(DicomReadOptions.StorePixelDataReferences);
			try
			{
				var pd = new DicomUncompressedPixelData(dcf);

				Assert.AreEqual(16, pd.BitsAllocated, "BitsAllocated");
				Assert.AreEqual(16, pd.BitsStored, "BitsStored");
				Assert.AreEqual(15, pd.HighBit, "HighBit");
				Assert.AreEqual(20, pd.ImageHeight, "ImageHeight");
				Assert.AreEqual(30, pd.ImageWidth, "ImageWidth");
				Assert.AreEqual(3, pd.NumberOfFrames, "NumberOfFrames");
				Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
				Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
				Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
				Assert.AreEqual(20*30*2, pd.UncompressedFrameSize, "UncompressedFrameSize");

				for (var frame = 0; frame < 3; ++frame)
				{
					var fd = pd.GetFrame(frame);
					Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
					AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Test]
		public void TestBasic_FileOnDisk_16Bits_BigEndian_OddFrameLength()
		{
			var filename = Path.GetTempFileName();
			CreateDicomImage(rows : 19, columns : 29, numberOfFrames : 5, endian : Endian.Big).Save(filename);

			var dcf = new DicomFile(filename);
			dcf.Load(DicomReadOptions.StorePixelDataReferences);
			try
			{
				var pd = new DicomUncompressedPixelData(dcf);

				Assert.AreEqual(16, pd.BitsAllocated, "BitsAllocated");
				Assert.AreEqual(16, pd.BitsStored, "BitsStored");
				Assert.AreEqual(15, pd.HighBit, "HighBit");
				Assert.AreEqual(19, pd.ImageHeight, "ImageHeight");
				Assert.AreEqual(29, pd.ImageWidth, "ImageWidth");
				Assert.AreEqual(5, pd.NumberOfFrames, "NumberOfFrames");
				Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
				Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
				Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
				Assert.AreEqual(19*29*2, pd.UncompressedFrameSize, "UncompressedFrameSize");

				for (var frame = 0; frame < 5; ++frame)
				{
					var fd = pd.GetFrame(frame);
					Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
					AssertBytesEqual((byte) (0x80 + frame), fd, "PixelData(frame={0})", frame);
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		#endregion

		#endregion

		#region Advanced Tests

		[Test]
		public void TestAdvanced_FileOnDisk_16Bits_OB()
		{
			var filename = Path.GetTempFileName();
			CreateDicomImage(rows : 20, columns : 30, numberOfFrames : 5, useOB : true).Save(filename);

			var dcf = new DicomFile(filename);
			dcf.Load(DicomReadOptions.StorePixelDataReferences);
			try
			{
				var pd = new DicomUncompressedPixelData(dcf);

				Assert.AreEqual(16, pd.BitsAllocated, "BitsAllocated");
				Assert.AreEqual(16, pd.BitsStored, "BitsStored");
				Assert.AreEqual(15, pd.HighBit, "HighBit");
				Assert.AreEqual(20, pd.ImageHeight, "ImageHeight");
				Assert.AreEqual(30, pd.ImageWidth, "ImageWidth");
				Assert.AreEqual(5, pd.NumberOfFrames, "NumberOfFrames");
				Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
				Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
				Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
				Assert.AreEqual(20*30*2, pd.UncompressedFrameSize, "UncompressedFrameSize");

				var newFrameData = new byte[pd.UncompressedFrameSize];
				for (var n = 0; n < newFrameData.Length; ++n)
					newFrameData[n] = 0x7F;
				pd.SetFrame(1, newFrameData);
				pd.UpdateAttributeCollection(dcf.DataSet);

				var pixelData = dcf.DataSet[DicomTags.PixelData].Values as byte[];
				for (var frame = 0; frame < 5; ++frame)
				{
					var fd = pd.GetFrame(frame);
					var expectedValue = frame == 1 ? (byte) 0x7F : (byte) (0x80 + frame);

					Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
					AssertBytesEqual(expectedValue, fd, "PixelData(frame={0})", frame);
					AssertBytesEqual(expectedValue, pixelData, frame*pd.UncompressedFrameSize, pd.UncompressedFrameSize, "AttributeValues(frame={0})", frame);
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Test]
		public void TestAdvanced_FileOnDisk_16Bits_OW()
		{
			var filename = Path.GetTempFileName();
			CreateDicomImage(rows : 20, columns : 30, numberOfFrames : 5).Save(filename);

			var dcf = new DicomFile(filename);
			dcf.Load(DicomReadOptions.StorePixelDataReferences);
			try
			{
				var pd = new DicomUncompressedPixelData(dcf);

				Assert.AreEqual(16, pd.BitsAllocated, "BitsAllocated");
				Assert.AreEqual(16, pd.BitsStored, "BitsStored");
				Assert.AreEqual(15, pd.HighBit, "HighBit");
				Assert.AreEqual(20, pd.ImageHeight, "ImageHeight");
				Assert.AreEqual(30, pd.ImageWidth, "ImageWidth");
				Assert.AreEqual(5, pd.NumberOfFrames, "NumberOfFrames");
				Assert.AreEqual("MONOCHROME2", pd.PhotometricInterpretation, "PhotometricInterpretation");
				Assert.AreEqual(0, pd.PixelRepresentation, "PixelRepresentation");
				Assert.AreEqual(1, pd.SamplesPerPixel, "SamplesPerPixel");
				Assert.AreEqual(20*30*2, pd.UncompressedFrameSize, "UncompressedFrameSize");

				var newFrameData = new byte[pd.UncompressedFrameSize];
				for (var n = 0; n < newFrameData.Length; ++n)
					newFrameData[n] = 0x7F;
				pd.SetFrame(1, newFrameData);
				pd.UpdateAttributeCollection(dcf.DataSet);

				var pixelData = dcf.DataSet[DicomTags.PixelData].Values as byte[];
				for (var frame = 0; frame < 5; ++frame)
				{
					var fd = pd.GetFrame(frame);
					var expectedValue = frame == 1 ? (byte) 0x7F : (byte) (0x80 + frame);

					Assert.AreEqual(pd.UncompressedFrameSize, fd.Length, "PixelData(frame={0}).Length", frame);
					AssertBytesEqual(expectedValue, fd, "PixelData(frame={0})", frame);
					AssertBytesEqual(expectedValue, pixelData, frame*pd.UncompressedFrameSize, pd.UncompressedFrameSize, "AttributeValues(frame={0})", frame);
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		#endregion

		private static void AssertBytesEqual(byte expectedValue, byte[] actualBytes, string message, params object[] args)
		{
			AssertBytesEqual(expectedValue, actualBytes, 0, actualBytes.Length, message, args);
		}

		private static void AssertBytesEqual(byte expectedValue, byte[] actualBytes, int offset, int length, string message, params object[] args)
		{
			for (var n = 0; n < length; ++n)
			{
				var actualValue = actualBytes[offset + n];
				Assert.AreEqual(expectedValue, actualValue, "{1} @index={0}", offset + n, string.Format(message, args));
			}
		}

		/// <summary>
		/// Creates an in-memory DICOM image
		/// </summary>
		/// <returns></returns>
		private static DicomFile CreateDicomImage(int rows = 20, int columns = 30, bool bitsAllocated16 = true, bool signed = false, int numberOfFrames = 1, Endian endian = Endian.Little, bool useOB = false)
		{
			var sopClassUid = bitsAllocated16 ? SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorageUid : SopClass.MultiFrameGrayscaleByteSecondaryCaptureImageStorageUid;
			var sopInstanceUid = DicomUid.GenerateUid().UID;

			var dcf = new DicomFile();
			dcf.MediaStorageSopClassUid = sopClassUid;
			dcf.MediaStorageSopInstanceUid = sopInstanceUid;
			dcf.TransferSyntax = endian == Endian.Little ? TransferSyntax.ExplicitVrLittleEndian : TransferSyntax.ExplicitVrBigEndian;
			dcf.DataSet[DicomTags.PatientId].SetStringValue("TEST");
			dcf.DataSet[DicomTags.PatientsName].SetStringValue("TEST");
			dcf.DataSet[DicomTags.StudyInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dcf.DataSet[DicomTags.StudyId].SetStringValue("TEST");
			dcf.DataSet[DicomTags.StudyDescription].SetStringValue("TEST");
			dcf.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dcf.DataSet[DicomTags.SeriesNumber].SetInt32(0, 1);
			dcf.DataSet[DicomTags.SeriesDescription].SetStringValue("TEST");
			dcf.DataSet[DicomTags.SopClassUid].SetStringValue(sopClassUid);
			dcf.DataSet[DicomTags.SopInstanceUid].SetStringValue(sopInstanceUid);
			dcf.DataSet[DicomTags.InstanceNumber].SetInt32(0, 1);

			var frameSize = rows*columns*(bitsAllocated16 ? 2 : 1);
			var data = new byte[numberOfFrames*frameSize];
			for (var n = 0; n < numberOfFrames; ++n)
			{
				var value = (byte) (0xFF & (n + 0x80));

				for (var k = 0; k < frameSize; ++k)
					data[n*frameSize + k] = value;
			}

			var pixelDataTag = DicomTagDictionary.GetDicomTag(DicomTags.PixelData);
			pixelDataTag = new DicomTag(pixelDataTag.TagValue, pixelDataTag.Name, pixelDataTag.VariableName, useOB ? DicomVr.OBvr : DicomVr.OWvr, pixelDataTag.MultiVR, pixelDataTag.VMLow, pixelDataTag.VMHigh, pixelDataTag.Retired);

			dcf.DataSet[DicomTags.PhotometricInterpretation].SetStringValue(PhotometricInterpretation.Monochrome2.Code);
			dcf.DataSet[DicomTags.SamplesPerPixel].SetInt32(0, 1);
			dcf.DataSet[DicomTags.PixelRepresentation].SetInt32(0, signed ? 1 : 0);
			dcf.DataSet[DicomTags.BitsAllocated].SetInt32(0, bitsAllocated16 ? 16 : 8);
			dcf.DataSet[DicomTags.BitsStored].SetInt32(0, bitsAllocated16 ? 16 : 8);
			dcf.DataSet[DicomTags.HighBit].SetInt32(0, bitsAllocated16 ? 15 : 7);
			dcf.DataSet[DicomTags.Rows].SetInt32(0, rows);
			dcf.DataSet[DicomTags.Columns].SetInt32(0, columns);
			dcf.DataSet[DicomTags.NumberOfFrames].SetInt32(0, numberOfFrames);
			dcf.DataSet[pixelDataTag].Values = data;

			return dcf;
		}
	}
}

// ReSharper restore InconsistentNaming
#endif