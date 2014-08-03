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
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Drawing;
using ClearCanvas.Dicom;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Tests
{
	[TestFixture]
	public class NormalizedPixelSpacingTests
	{
		private const double _tolerance = 1e-7;

		[Test]
		public void TestCTUncalibrated()
		{
			using (var dataset = CreateMockDataset("CT", SopClass.CtImageStorage, null, null, null, null, null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(true, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.None, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestCTUncalibratedWithRogueImagerPixelSpacing()
		{
			using (var dataset = CreateMockDataset("CT", SopClass.CtImageStorage, new SizeF(1f, 2f), null, "GEOMETRY", "garbage", null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(true, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.None, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestCTCalibrated()
		{
			using (var dataset = CreateMockDataset("CT", SopClass.CtImageStorage, null, new SizeF(0.4f, 0.3f), null, null, null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.CrossSectionalSpacing, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestCTCalibratedWithRogueImagerPixelSpacing()
		{
			using (var dataset = CreateMockDataset("CT", SopClass.CtImageStorage, new SizeF(1f, 2f), new SizeF(0.4f, 0.3f), "GEOMETRY", "garbage", null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.CrossSectionalSpacing, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestDXUncalibrated()
		{
			using (var dataset = CreateMockDataset("DX", SopClass.DigitalXRayImageStorageForPresentation, null, null, null, null, null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(true, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.None, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestDXUncalibratedWithDetectorSpacing()
		{
			using (var dataset = CreateMockDataset("DX", SopClass.DigitalXRayImageStorageForPresentation, new SizeF(0.4f, 0.3f), null, null, null, null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Detector, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestMGUncalibrated()
		{
			using (var dataset = CreateMockDataset("MG", SopClass.DigitalMammographyXRayImageStorageForPresentation, null, null, null, null, null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(true, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.None, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestMGUncalibratedWithDetectorSpacing()
		{
			using (var dataset = CreateMockDataset("MG", SopClass.DigitalMammographyXRayImageStorageForPresentation, new SizeF(0.4f, 0.3f), null, null, null, null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Detector, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestMGUncalibratedWithDetectorSpacingWithMagnficationFactor()
		{
			// we don't yet support the IHE Mammo profile requirement to use Imager Pixel Spacing corrected by Estimated Radiographic Magnification Factor
			using (var dataset = CreateMockDataset("MG", SopClass.DigitalMammographyXRayImageStorageForPresentation, new SizeF(0.4f, 0.3f), null, null, null, 1.5))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Detector, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestMGCalibrationUnknown()
		{
			using (var dataset = CreateMockDataset("MG", SopClass.DigitalMammographyXRayImageStorageForPresentation, new SizeF(0.6f, 0.5f), new SizeF(0.4f, 0.3f), null, null, null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Unknown, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestMGCalibrationUnknownWithMagnificationFactor()
		{
			// we don't yet support the IHE Mammo profile requirement to use Imager Pixel Spacing corrected by Estimated Radiographic Magnification Factor
			using (var dataset = CreateMockDataset("MG", SopClass.DigitalMammographyXRayImageStorageForPresentation, new SizeF(0.6f, 0.5f), new SizeF(0.4f, 0.3f), null, null, 1.5))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Unknown, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestMGCalibratedUsingFiducial()
		{
			using (var dataset = CreateMockDataset("MG", SopClass.DigitalMammographyXRayImageStorageForPresentation, new SizeF(0.6f, 0.5f), new SizeF(0.4f, 0.3f), "FIDUCIAL", null, null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Fiducial, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestMGCalibratedUsingFiducialWithMagnificationFactor()
		{
			// we don't yet support the IHE Mammo profile requirement to use Imager Pixel Spacing corrected by Estimated Radiographic Magnification Factor
			using (var dataset = CreateMockDataset("MG", SopClass.DigitalMammographyXRayImageStorageForPresentation, new SizeF(0.6f, 0.5f), new SizeF(0.4f, 0.3f), "FIDUCIAL", null, 1.5))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Fiducial, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestMGCalibratedUsingGeometry()
		{
			using (var dataset = CreateMockDataset("MG", SopClass.DigitalMammographyXRayImageStorageForPresentation, new SizeF(0.6f, 0.5f), new SizeF(0.4f, 0.3f), "GEOMETRY", null, null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Geometry, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestMGCalibratedUsingGeometryWithMagnificationFactor()
		{
			// we don't yet support the IHE Mammo profile requirement to use Imager Pixel Spacing corrected by Estimated Radiographic Magnification Factor
			using (var dataset = CreateMockDataset("MG", SopClass.DigitalMammographyXRayImageStorageForPresentation, new SizeF(0.6f, 0.5f), new SizeF(0.4f, 0.3f), "GEOMETRY", null, 1.5))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Geometry, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestCRUncalibrated()
		{
			using (var dataset = CreateMockDataset("CR", SopClass.ComputedRadiographyImageStorage, null, null, null, null, null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(true, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.None, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestCRUncalibratedWithDetectorSpacing()
		{
			using (var dataset = CreateMockDataset("CR", SopClass.ComputedRadiographyImageStorage, new SizeF(0.4f, 0.3f), null, null, null, null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Detector, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestCRUncalibratedWithDetectorSpacingInPixelSpacing()
		{
			using (var dataset = CreateMockDataset("CR", SopClass.ComputedRadiographyImageStorage, new SizeF(0.4f, 0.3f), new SizeF(0.4f, 0.3f), null, null, null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Detector, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestCalibrationUnknown()
		{
			using (var dataset = CreateMockDataset("CR", SopClass.ComputedRadiographyImageStorage, null, new SizeF(0.4f, 0.3f), null, null, null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Unknown, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestCRCalibrationUnspecified()
		{
			using (var dataset = CreateMockDataset("CR", SopClass.ComputedRadiographyImageStorage, new SizeF(0.6f, 0.5f), new SizeF(0.4f, 0.3f), null, null, null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Unknown, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestCRCalibratedUsingGeometry()
		{
			using (var dataset = CreateMockDataset("CR", SopClass.ComputedRadiographyImageStorage, new SizeF(0.6f, 0.5f), new SizeF(0.4f, 0.3f), "GEOMETRY", "details", null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Geometry, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual("details", normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestCRCalibratedUsingGeometryWithoutImagerPixelSpacing()
		{
			using (var dataset = CreateMockDataset("CR", SopClass.ComputedRadiographyImageStorage, null, new SizeF(0.4f, 0.3f), "GEOMETRY", "details", null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Geometry, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual("details", normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestCRCalibratedUsingFiducial()
		{
			using (var dataset = CreateMockDataset("CR", SopClass.ComputedRadiographyImageStorage, new SizeF(0.6f, 0.5f), new SizeF(0.4f, 0.3f), "FIDUCIAL", "details", null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Fiducial, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual("details", normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestCRCalibratedUsingFiducialWithoutImagerPixelSpacing()
		{
			using (var dataset = CreateMockDataset("CR", SopClass.ComputedRadiographyImageStorage, null, new SizeF(0.4f, 0.3f), "FIDUCIAL", "details", null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Fiducial, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual("details", normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestManualCalibration()
		{
			using (var dataset = CreateMockDataset("CR", SopClass.ComputedRadiographyImageStorage, null, new SizeF(0.6f, 0.5f), "FIDUCIAL", "details", null))
			{
				using (var sop = (ImageSop) Sop.Create(dataset))
				{
					var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
					normalizedPixelSpacing.Calibrate(0.3, 0.4);
					Console.WriteLine(GetPixelSpacingAttributeInfo(dataset));
					Assert.AreEqual(false, normalizedPixelSpacing.IsNull, "IsNull property");
					Assert.AreEqual(0.3, normalizedPixelSpacing.Row, _tolerance, "Row property");
					Assert.AreEqual(0.4, normalizedPixelSpacing.Column, _tolerance, "Column property");
					Assert.AreEqual(NormalizedPixelSpacingCalibrationType.Manual, normalizedPixelSpacing.CalibrationType, "CalibrationType property");
					Assert.AreEqual(string.Empty, normalizedPixelSpacing.CalibrationDetails, "CalibrationDetails property");
					Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
				}
			}
		}

		[Test]
		public void TestGetPixelAspectRatioString()
		{
			using (var dataset = CreateMockDataset("CR", SopClass.ComputedRadiographyImageStorage, null, new SizeF(0.6f, 0.5f), "FIDUCIAL", "details", null))
			using (var sop = (ImageSop) Sop.Create(dataset))
			{
				var normalizedPixelSpacing = new NormalizedPixelSpacing(sop.Frames[1]);
				Assert.AreEqual(@"5\6", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");

				normalizedPixelSpacing.Calibrate(0.33333331, 0.3333333);
				Assert.AreEqual(@"1\1", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");

				normalizedPixelSpacing.Calibrate(0.7074200013, 0.7074200009);
				Assert.AreEqual(@"1\1", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");

				normalizedPixelSpacing.Calibrate(0.3, 0.4);
				Assert.AreEqual(@"3\4", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");

				normalizedPixelSpacing.Calibrate(0.4, 0.3000001);
				Assert.AreEqual(@"4\3", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");

				normalizedPixelSpacing.Calibrate(0.55, 0.4);
				Assert.AreEqual(@"11\8", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");

				normalizedPixelSpacing.Calibrate(0.7777777, 0.3333333);
				Assert.AreEqual(@"7\3", normalizedPixelSpacing.GetPixelAspectRatioString(), "GetPixelAspectRatioString result");
			}
		}

		[Test]
		public void TestFindFraction()
		{
			var testRational = new Action<int, int>((expectedNumerator, expectedDenominator) =>
			                                        	{
			                                        		var value = 1d*expectedNumerator/expectedDenominator;

			                                        		int actualNumerator, actualDenominator;
			                                        		NormalizedPixelSpacing.TestFindFraction(value, out actualNumerator, out actualDenominator);

			                                        		var expectedFraction = string.Format("{0}/{1}", expectedNumerator, expectedDenominator);
			                                        		var actualFraction = string.Format("{0}/{1}", actualNumerator, actualDenominator);

			                                        		const string msg = "Value={0}, Result={1}";
			                                        		Console.WriteLine(msg, value, actualFraction);
			                                        		Assert.AreEqual(expectedFraction, actualFraction, "Value={0}", value);
			                                        	});

			testRational.Invoke(0, 1);
			testRational.Invoke(1, 10);
			testRational.Invoke(-1, 10);
			testRational.Invoke(1, 100);
			testRational.Invoke(1, 1000);
			testRational.Invoke(1, 10000);
			testRational.Invoke(649, 200);
			testRational.Invoke(649, 300);
			testRational.Invoke(7, 11);
			testRational.Invoke(5, 19);
			testRational.Invoke(21, 19);
			testRational.Invoke(37, 61);
			testRational.Invoke(1, 9);
			testRational.Invoke(5, 9);
			testRational.Invoke(5, 99);
			testRational.Invoke(5, 999);

			var testIrrational = new Action<double, double>((expectedValue, tolerance) =>
			                                                	{
			                                                		int actualNumerator, actualDenominator;
			                                                		NormalizedPixelSpacing.TestFindFraction(expectedValue, out actualNumerator, out actualDenominator, tolerance);

			                                                		var actualFraction = string.Format("{0}/{1}", actualNumerator, actualDenominator);
			                                                		var actualValue = 1d*actualNumerator/actualDenominator;

			                                                		const string msg = "Value={0}, Result={1} ({2}), Delta={3}";
			                                                		Console.WriteLine(msg, expectedValue, actualFraction, actualValue, Math.Abs(expectedValue - actualValue));
			                                                		Assert.AreEqual(expectedValue, actualValue, tolerance, "Value={0}", expectedValue);
			                                                	});

			testIrrational.Invoke(Math.PI, 1e-6);
			testIrrational.Invoke(Math.PI, 1e-9);
			testIrrational.Invoke(Math.PI, 1e-15);
			testIrrational.Invoke(Math.E, 1e-6);
			testIrrational.Invoke(Math.E, 1e-9);
			testIrrational.Invoke(Math.E, 1e-15);

			try
			{
				testIrrational.Invoke(0.5/int.MaxValue, 1e-16);
				Assert.Fail("Expected overflow exception");
			}
			catch (OverflowException) {}

			try
			{
				testIrrational.Invoke(2d*int.MaxValue, 1e-16);
				Assert.Fail("Expected overflow exception");
			}
			catch (OverflowException) {}
		}

		private static string GetPixelSpacingAttributeInfo(IDicomAttributeProvider sopDataSource)
		{
			var modality = sopDataSource[DicomTags.Modality].ToString();
			var imagerPixelSpacing = "*";
			var pixelSpacing = "*";
			var pixelSpacingCalibrationType = "*";
			var pixelSpacingCalibrationDescription = "*";
			var estimatedRadiographicMagnificationFactor = "*";

			DicomAttribute attribute;
			if (sopDataSource.TryGetAttribute(DicomTags.ImagerPixelSpacing, out attribute))
				imagerPixelSpacing = attribute.ToString();

			if (sopDataSource.TryGetAttribute(DicomTags.PixelSpacing, out attribute))
				pixelSpacing = attribute.ToString();

			if (sopDataSource.TryGetAttribute(DicomTags.PixelSpacingCalibrationType, out attribute))
				pixelSpacingCalibrationType = attribute.ToString();

			if (sopDataSource.TryGetAttribute(DicomTags.PixelSpacingCalibrationDescription, out attribute))
				pixelSpacingCalibrationDescription = attribute.ToString();

			if (sopDataSource.TryGetAttribute(DicomTags.EstimatedRadiographicMagnificationFactor, out attribute))
				estimatedRadiographicMagnificationFactor = attribute.ToString();

			return string.Format("[{0}] ImagerPixSpac={1} PixSpac={2} PixSpacCalType={3} PixSpacCalDesc={4} EstRadMagFac={5}", modality, imagerPixelSpacing, pixelSpacing, pixelSpacingCalibrationType, pixelSpacingCalibrationDescription, estimatedRadiographicMagnificationFactor);
		}

		private static ISopDataSource CreateMockDataset(string modality, SopClass sopClass, SizeF? imagerPixelSpacing, SizeF? pixelSpacing, string pixelSpacingCalibrationType, string pixelSpacingCalibrationDescription, double? estimatedRadiographicMagnification)
		{
			var dicomFile = new DicomFile();
			var dataset = dicomFile.DataSet;
			dataset[DicomTags.PatientId].SetStringValue("PATIENT");
			dataset[DicomTags.PatientsName].SetStringValue("YOSHI");
			dataset[DicomTags.StudyId].SetStringValue("STUDY");
			dataset[DicomTags.SeriesDescription].SetStringValue("SERIES");
			dataset[DicomTags.StudyInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.Modality].SetStringValue(modality);
			dataset[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.SopClassUid].SetStringValue(sopClass.Uid);
			dataset[DicomTags.FrameOfReferenceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
			dataset[DicomTags.BitsStored].SetInt32(0, 16);
			dataset[DicomTags.BitsAllocated].SetInt32(0, 16);
			dataset[DicomTags.HighBit].SetInt32(0, 15);
			dataset[DicomTags.PixelRepresentation].SetInt32(0, 0);
			dataset[DicomTags.Rows].SetInt32(0, 100);
			dataset[DicomTags.Columns].SetInt32(0, 100);
			dataset[DicomTags.WindowCenter].SetInt32(0, 32768);
			dataset[DicomTags.WindowWidth].SetInt32(0, 65536);
			dataset[DicomTags.WindowCenterWidthExplanation].SetString(0, "Full Window");

			if (imagerPixelSpacing.HasValue)
			{
				dataset[DicomTags.ImagerPixelSpacing].SetFloat32(0, imagerPixelSpacing.Value.Height);
				dataset[DicomTags.ImagerPixelSpacing].SetFloat32(1, imagerPixelSpacing.Value.Width);
			}

			if (pixelSpacing.HasValue)
			{
				dataset[DicomTags.PixelSpacing].SetFloat32(0, pixelSpacing.Value.Height);
				dataset[DicomTags.PixelSpacing].SetFloat32(1, pixelSpacing.Value.Width);
			}

			if (!string.IsNullOrEmpty(pixelSpacingCalibrationType))
				dataset[DicomTags.PixelSpacingCalibrationType].SetStringValue(pixelSpacingCalibrationType);

			if (!string.IsNullOrEmpty(pixelSpacingCalibrationDescription))
				dataset[DicomTags.PixelSpacingCalibrationDescription].SetStringValue(pixelSpacingCalibrationDescription);

			if (estimatedRadiographicMagnification.HasValue)
				dataset[DicomTags.EstimatedRadiographicMagnificationFactor].SetFloat64(0, estimatedRadiographicMagnification.Value);

			return new TestDataSource(dicomFile);
		}
	}
}

#endif