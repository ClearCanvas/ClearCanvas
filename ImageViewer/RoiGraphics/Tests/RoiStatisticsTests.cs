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

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.RoiGraphics.Tests
{
	[TestFixture]
	public class RoiStatisticsTests
	{
		[Test]
		public void TestTrivial()
		{
			const byte value = 129;
			byte[] data = new byte[36];
			for (int n = 0; n < 36; n++)
				data[n] = value;
			RoiStatistics stats;
			using (TestRoi roi = new TestRoi(data))
			{
				stats = RoiStatistics.Calculate(roi);
			}
			Assert.IsTrue(stats.Valid);
			Assert.AreEqual(value, stats.Mean, "Mean incorrect.");
			Assert.AreEqual(0, stats.StandardDeviation, "StdDev incorrect.");
		}

		[Test]
		public void TestTrivial2()
		{
			const byte value1 = 0;
			const byte value2 = 255;
			const double mean = (value2 + value1)/2d;
			byte[] data = new byte[36];
			for (int n = 0; n < 36; n++)
				data[n] = n < 18 ? value1 : value2;
			RoiStatistics stats;
			using (TestRoi roi = new TestRoi(data))
			{
				stats = RoiStatistics.Calculate(roi);
			}
			Assert.IsTrue(stats.Valid);
			Assert.AreEqual(mean, stats.Mean, "Mean incorrect.");
			Assert.AreEqual(mean - value1, stats.StandardDeviation, "StdDev incorrect.");
		}

		[Test]
		public void TestTrivial3()
		{
			const byte value1 = 97;
			const byte value2 = 232;
			const double mean = (value2 + value1)/2d;
			byte[] data = new byte[36];
			for (int n = 0; n < 36; n++)
				data[n] = n < 18 ? value1 : value2;
			RoiStatistics stats;
			using (TestRoi roi = new TestRoi(data))
			{
				stats = RoiStatistics.Calculate(roi);
			}
			Assert.IsTrue(stats.Valid);
			Assert.AreEqual(mean, stats.Mean, "Mean incorrect.");
			Assert.AreEqual(mean - value1, stats.StandardDeviation, "StdDev incorrect.");
		}

		[Test]
		public void TestNonTrivial()
		{
			RoiStatistics stats;
			using (TestRoi roi = new TestRoi(6, 86, 142, 208, 205, 214, 71, 8, 224, 231, 131, 4, 56, 123, 186, 180, 103, 163, 166, 212, 48, 130, 111, 94, 37, 239, 200, 117, 188, 56, 50, 42, 135, 114, 168, 231))
			{
				stats = RoiStatistics.Calculate(roi);
			}
			Assert.IsTrue(stats.Valid);
			Assert.AreEqual(129.9722222, stats.Mean, 1, "Mean incorrect.");
			Assert.AreEqual(71.59189344, stats.StandardDeviation, 2, "StdDev incorrect.");
		}

		[Test]
		public void TestNonTrivial2()
		{
			RoiStatistics stats;
			using (TestRoi roi = new TestRoi(80, 233, 92, 23, 201, 13, 162, 42, 149, 102, 179, 4, 24, 102, 124, 125, 28, 66, 32, 223, 80, 61, 234, 230, 77, 251, 188, 119, 135, 116, 204, 184, 198, 52, 200, 7))
			{
				stats = RoiStatistics.Calculate(roi);
			}
			Assert.IsTrue(stats.Valid);
			Assert.AreEqual(120.5555556, stats.Mean, 1, "Mean incorrect.");
			Assert.AreEqual(76.14026322, stats.StandardDeviation, 2, "StdDev incorrect.");
		}

		[Test]
		public void TestNonTrivial3()
		{
			RoiStatistics stats;
			using (TestRoi roi = new TestRoi(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 150, 247, 28, 86, 47, 137, 107, 233, 52, 235, 152, 249, 190, 56, 106, 255, 221, 89, 151, 193))
			{
				stats = RoiStatistics.Calculate(roi);
			}
			Assert.IsTrue(stats.Valid);
			Assert.AreEqual(82.88888889, stats.Mean, 1, "Mean incorrect.");
			Assert.AreEqual(93.56549357, stats.StandardDeviation, 2, "StdDev incorrect.");
		}

		private class TestRoi : Roi, IDisposable
		{
			private const int _height = 6;
			private const int _width = 6;
			private readonly IPresentationImage _image;

			public TestRoi(params byte[] data) : this(CreatePresentationImage(data)) {}

			private TestRoi(IPresentationImage image) : base(image)
			{
				_image = image;
			}

			void IDisposable.Dispose()
			{
				_image.Dispose();
			}

			protected override RectangleF ComputeBounds()
			{
				return new RectangleF(0, 0, _width - 1, _height - 1);
			}

			public override bool Contains(PointF point)
			{
				return true;
			}

			//[Obsolete("CopyTo is not implemented by this test type.", true)]
			public override Roi CopyTo(IPresentationImage presentationImage)
			{
				throw new NotImplementedException();
			}

			private static IPresentationImage CreatePresentationImage(byte[] data)
			{
				Platform.CheckForNullReference(data, "data");
				Platform.CheckTrue(data.Length == _width*_height, string.Format("Data must be exactly {0} in length", _width*_height));
				return new TestPresentationImage(data);
			}

			private class TestPresentationImage : BasicPresentationImage, IDicomPresentationImage
			{
				private readonly ImageSop _imageSop;

				public TestPresentationImage(byte[] data)
					: base(CreateTestPattern(data))
				{
					DicomFile dcf = new DicomFile();
					dcf.DataSet[DicomTags.StudyInstanceUid].SetStringValue("1");
					dcf.DataSet[DicomTags.SeriesInstanceUid].SetStringValue("2");
					dcf.DataSet[DicomTags.SopInstanceUid].SetStringValue("3");
					dcf.DataSet[DicomTags.SopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);
					dcf.DataSet[DicomTags.InstanceNumber].SetStringValue("1");
					dcf.DataSet[DicomTags.NumberOfFrames].SetStringValue("1");
					dcf.MetaInfo[DicomTags.TransferSyntaxUid].SetStringValue(TransferSyntax.ImplicitVrLittleEndianUid);
					dcf.MetaInfo[DicomTags.MediaStorageSopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);
					dcf.MetaInfo[DicomTags.MediaStorageSopInstanceUid].SetStringValue("3");
					_imageSop = new ImageSop(new TestDataSource(dcf));
				}

				private static GrayscaleImageGraphic CreateTestPattern(byte[] data)
				{
					GrayscaleImageGraphic imageGraphic = new GrayscaleImageGraphic(_height, _width);
					for (int x = 0; x < _width; x++)
					{
						for (int y = 0; y < _height; y++)
						{
							imageGraphic.PixelData.SetPixel(x, y, data[x*_width + y]);
						}
					}
					return imageGraphic;
				}

				//[Obsolete("CreateFreshCopy is not implemented by this test type.", true)]
				public override IPresentationImage CreateFreshCopy()
				{
					throw new NotImplementedException();
				}

				#region IDicomPresentationImage Members (Not Implemented)

				GraphicCollection IDicomPresentationImage.DicomGraphics
				{
					get { throw new NotImplementedException(); }
				}

				#endregion

				#region IImageSopProvider Members

				ImageSop IImageSopProvider.ImageSop
				{
					get { return _imageSop; }
				}

				Frame IImageSopProvider.Frame
				{
					get { return _imageSop.Frames[1]; }
				}

				#endregion

				#region ISopProvider Members (Not Implemented)

				Sop ISopProvider.Sop
				{
					get { throw new NotImplementedException(); }
				}

				#endregion

				#region IPatientPresentationProvider Members (Not Implemented)

				public IPatientPresentation PatientPresentation
				{
					get { throw new NotImplementedException(); }
				}

				#endregion

				#region IPatientCoordinateMappingProvider Members (Not Implemented)

				public IPatientCoordinateMapping PatientCoordinateMapping
				{
					get { throw new NotImplementedException(); }
				}

				#endregion

				protected override void Dispose(bool disposing)
				{
					if (disposing)
					{
						_imageSop.Dispose();
					}
				}
			}
		}
	}
}

#endif