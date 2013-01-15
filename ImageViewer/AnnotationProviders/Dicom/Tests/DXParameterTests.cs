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

using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.Tests
{
	[TestFixture]
	public class DXParameterTests
	{
		[Test]
		public void TestExposure()
		{
			using (var imageSop = new MockImageSop())
			{
				imageSop[DicomTags.ExposureInMas].SetFloat64(0, 78.9);
				imageSop[DicomTags.ExposureInUas].SetStringValue(@"456");
				imageSop[DicomTags.Exposure].SetStringValue(@"123");
				AssertExposureParameter(@"78.90", imageSop, DXImageAnnotationItemProvider.GetExposureInMas, @"9332 present, 1153 present, 1152 present");

				imageSop[DicomTags.ExposureInMas].SetFloat64(0, 78.9);
				imageSop[DicomTags.ExposureInUas].SetStringValue(@"456");
				imageSop[DicomTags.Exposure].SetEmptyValue();
				AssertExposureParameter(@"78.90", imageSop, DXImageAnnotationItemProvider.GetExposureInMas, @"9332 present, 1153 present, 1152 absent");

				imageSop[DicomTags.ExposureInMas].SetFloat64(0, 78.9);
				imageSop[DicomTags.ExposureInUas].SetEmptyValue();
				imageSop[DicomTags.Exposure].SetStringValue(@"123");
				AssertExposureParameter(@"78.90", imageSop, DXImageAnnotationItemProvider.GetExposureInMas, @"9332 present, 1153 absent, 1152 present");

				imageSop[DicomTags.ExposureInMas].SetFloat64(0, 78.9);
				imageSop[DicomTags.ExposureInUas].SetEmptyValue();
				imageSop[DicomTags.Exposure].SetEmptyValue();
				AssertExposureParameter(@"78.90", imageSop, DXImageAnnotationItemProvider.GetExposureInMas, @"9332 present, 1153 absent, 1152 absent");

				imageSop[DicomTags.ExposureInMas].SetEmptyValue();
				imageSop[DicomTags.ExposureInUas].SetStringValue(@"456");
				imageSop[DicomTags.Exposure].SetStringValue(@"123");
				AssertExposureParameter(@"0.46", imageSop, DXImageAnnotationItemProvider.GetExposureInMas, @"9332 absent, 1153 present, 1152 present");

				imageSop[DicomTags.ExposureInMas].SetEmptyValue();
				imageSop[DicomTags.ExposureInUas].SetStringValue(@"456");
				imageSop[DicomTags.Exposure].SetEmptyValue();
				AssertExposureParameter(@"0.46", imageSop, DXImageAnnotationItemProvider.GetExposureInMas, @"9332 absent, 1153 present, 1152 absent");

				imageSop[DicomTags.ExposureInMas].SetNullValue();
				imageSop[DicomTags.ExposureInUas].SetStringValue(@"");
				imageSop[DicomTags.Exposure].SetStringValue(@"123");
				AssertExposureParameter(@"123", imageSop, DXImageAnnotationItemProvider.GetExposureInMas, @"9332 absent, 1153 absent, 1152 present");

				imageSop[DicomTags.ExposureInMas].SetEmptyValue();
				imageSop[DicomTags.ExposureInUas].SetEmptyValue();
				imageSop[DicomTags.Exposure].SetEmptyValue();
				AssertExposureParameter(@"", imageSop, DXImageAnnotationItemProvider.GetExposureInMas, @"9332 absent, 1153 absent, 1152 absent");
			}
		}

		[Test]
		public void TestExposureTime()
		{
			using (var imageSop = new MockImageSop())
			{
				imageSop[DicomTags.ExposureTimeInMs].SetFloat64(0, 78.9);
				imageSop[DicomTags.ExposureTimeInUs].SetStringValue(@"4567.89");
				imageSop[DicomTags.ExposureTime].SetStringValue(@"123");
				AssertExposureParameter(@"78.90", imageSop, DXImageAnnotationItemProvider.GetExposureTimeInMs, @"9328 present, 8150 present, 1150 present");

				imageSop[DicomTags.ExposureTimeInMs].SetFloat64(0, 78.9);
				imageSop[DicomTags.ExposureTimeInUs].SetStringValue(@"4567.89");
				imageSop[DicomTags.ExposureTime].SetEmptyValue();
				AssertExposureParameter(@"78.90", imageSop, DXImageAnnotationItemProvider.GetExposureTimeInMs, @"9328 present, 8150 present, 1150 absent");

				imageSop[DicomTags.ExposureTimeInMs].SetFloat64(0, 78.9);
				imageSop[DicomTags.ExposureTimeInUs].SetEmptyValue();
				imageSop[DicomTags.ExposureTime].SetStringValue(@"123");
				AssertExposureParameter(@"78.90", imageSop, DXImageAnnotationItemProvider.GetExposureTimeInMs, @"9328 present, 8150 absent, 1150 present");

				imageSop[DicomTags.ExposureTimeInMs].SetFloat64(0, 78.9);
				imageSop[DicomTags.ExposureTimeInUs].SetEmptyValue();
				imageSop[DicomTags.ExposureTime].SetEmptyValue();
				AssertExposureParameter(@"78.90", imageSop, DXImageAnnotationItemProvider.GetExposureTimeInMs, @"9328 present, 8150 absent, 1150 absent");

				imageSop[DicomTags.ExposureTimeInMs].SetEmptyValue();
				imageSop[DicomTags.ExposureTimeInUs].SetStringValue(@"4567.89");
				imageSop[DicomTags.ExposureTime].SetStringValue(@"123");
				AssertExposureParameter(@"4.57", imageSop, DXImageAnnotationItemProvider.GetExposureTimeInMs, @"9328 absent, 8150 present, 1150 present");

				imageSop[DicomTags.ExposureTimeInMs].SetEmptyValue();
				imageSop[DicomTags.ExposureTimeInUs].SetStringValue(@"4567.89");
				imageSop[DicomTags.ExposureTime].SetEmptyValue();
				AssertExposureParameter(@"4.57", imageSop, DXImageAnnotationItemProvider.GetExposureTimeInMs, @"9328 absent, 8150 present, 1150 absent");

				imageSop[DicomTags.ExposureTimeInMs].SetNullValue();
				imageSop[DicomTags.ExposureTimeInUs].SetStringValue(@"");
				imageSop[DicomTags.ExposureTime].SetStringValue(@"123");
				AssertExposureParameter(@"123", imageSop, DXImageAnnotationItemProvider.GetExposureTimeInMs, @"9328 absent, 8150 absent, 1150 present");

				imageSop[DicomTags.ExposureTimeInMs].SetEmptyValue();
				imageSop[DicomTags.ExposureTimeInUs].SetEmptyValue();
				imageSop[DicomTags.ExposureTime].SetEmptyValue();
				AssertExposureParameter(@"", imageSop, DXImageAnnotationItemProvider.GetExposureTimeInMs, @"9328 absent, 8150 absent, 1150 absent");
			}
		}

		[Test]
		public void TestXRayTubeCurrent()
		{
			using (var imageSop = new MockImageSop())
			{
				imageSop[DicomTags.XRayTubeCurrentInMa].SetFloat64(0, 78.9);
				imageSop[DicomTags.XRayTubeCurrentInUa].SetStringValue(@"4567.89");
				imageSop[DicomTags.XRayTubeCurrent].SetStringValue(@"123");
				AssertExposureParameter(@"78.90", imageSop, DXImageAnnotationItemProvider.GetXRayTubeCurrentInMa, @"9330 present, 8151 present, 1151 present");

				imageSop[DicomTags.XRayTubeCurrentInMa].SetFloat64(0, 78.9);
				imageSop[DicomTags.XRayTubeCurrentInUa].SetStringValue(@"4567.89");
				imageSop[DicomTags.XRayTubeCurrent].SetEmptyValue();
				AssertExposureParameter(@"78.90", imageSop, DXImageAnnotationItemProvider.GetXRayTubeCurrentInMa, @"9330 present, 8151 present, 1151 absent");

				imageSop[DicomTags.XRayTubeCurrentInMa].SetFloat64(0, 78.9);
				imageSop[DicomTags.XRayTubeCurrentInUa].SetEmptyValue();
				imageSop[DicomTags.XRayTubeCurrent].SetStringValue(@"123");
				AssertExposureParameter(@"78.90", imageSop, DXImageAnnotationItemProvider.GetXRayTubeCurrentInMa, @"9330 present, 8151 absent, 1151 present");

				imageSop[DicomTags.XRayTubeCurrentInMa].SetFloat64(0, 78.9);
				imageSop[DicomTags.XRayTubeCurrentInUa].SetEmptyValue();
				imageSop[DicomTags.XRayTubeCurrent].SetEmptyValue();
				AssertExposureParameter(@"78.90", imageSop, DXImageAnnotationItemProvider.GetXRayTubeCurrentInMa, @"9330 present, 8151 absent, 1151 absent");

				imageSop[DicomTags.XRayTubeCurrentInMa].SetEmptyValue();
				imageSop[DicomTags.XRayTubeCurrentInUa].SetStringValue(@"4567.89");
				imageSop[DicomTags.XRayTubeCurrent].SetStringValue(@"123");
				AssertExposureParameter(@"4.57", imageSop, DXImageAnnotationItemProvider.GetXRayTubeCurrentInMa, @"9330 absent, 8151 present, 1151 present");

				imageSop[DicomTags.XRayTubeCurrentInMa].SetEmptyValue();
				imageSop[DicomTags.XRayTubeCurrentInUa].SetStringValue(@"4567.89");
				imageSop[DicomTags.XRayTubeCurrent].SetEmptyValue();
				AssertExposureParameter(@"4.57", imageSop, DXImageAnnotationItemProvider.GetXRayTubeCurrentInMa, @"9330 absent, 8151 present, 1151 absent");

				imageSop[DicomTags.XRayTubeCurrentInMa].SetNullValue();
				imageSop[DicomTags.XRayTubeCurrentInUa].SetStringValue(@"");
				imageSop[DicomTags.XRayTubeCurrent].SetStringValue(@"123");
				AssertExposureParameter(@"123", imageSop, DXImageAnnotationItemProvider.GetXRayTubeCurrentInMa, @"9330 absent, 8151 absent, 1151 present");

				imageSop[DicomTags.XRayTubeCurrentInMa].SetEmptyValue();
				imageSop[DicomTags.XRayTubeCurrentInUa].SetEmptyValue();
				imageSop[DicomTags.XRayTubeCurrent].SetEmptyValue();
				AssertExposureParameter(@"", imageSop, DXImageAnnotationItemProvider.GetXRayTubeCurrentInMa, @"9330 absent, 8151 absent, 1151 absent");
			}
		}

		private static void AssertExposureParameter(string expectedValue, ImageSop imageSop, ExposureParameterGetter function, string message)
		{
			Assert.AreEqual(expectedValue, function.Invoke(imageSop.Frames[1], @"{0}"), message);
		}

		private delegate string ExposureParameterGetter(Frame frame, string formatString);
	}
}

#endif