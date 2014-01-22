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
using System.Collections.Generic;
using System.Diagnostics;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;
using ClearCanvas.ImageViewer.Volumes.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tests
{
	/// <summary>
	/// Tests various known location/value pairs for slices in various planes.
	/// </summary>
	[TestFixture(Description = "Tests various known location/value pairs for slices in various planes.")]
	public class SlicerPlaneTests : AbstractMprTest
	{
		private readonly IList<IVolumeSlicerParams> _slicings;

		public SlicerPlaneTests()
		{
			List<IVolumeSlicerParams> slicings = new List<IVolumeSlicerParams>();
			slicings.Add(VolumeSlicerParams.Identity);
			slicings.Add(VolumeSlicerParams.OrthogonalX);
			slicings.Add(VolumeSlicerParams.OrthogonalY);
			slicings.Add(new VolumeSlicerParams(0, 90, 45));
			slicings.Add(new VolumeSlicerParams(32, -62, 69));
			slicings.Add(new VolumeSlicerParams(60, 0, -30));
			slicings.Add(new VolumeSlicerParams(-15, 126, -30));
			_slicings = slicings.AsReadOnly();
		}

		[Test]
		public void TestNoTiltSlicings()
		{
			TestVolume(VolumeFunction.Stars,
			           null,
			           volume =>
			           	{
			           		foreach (IVolumeSlicerParams slicing in _slicings)
			           		{
			           			ValidateVolumeSlicePoints(volume, slicing, StarsKnownSamples);
			           		}
			           	});
		}

		[Test]
		public void TestPositiveXAxialGantryTiltedSlicings()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateGantryTiltedAboutX(30);
			TestVolume(VolumeFunction.StarsTilted030X,
			           orientation.Initialize,
			           volume =>
			           	{
			           		foreach (IVolumeSlicerParams slicing in _slicings)
			           		{
			           			ValidateVolumeSlicePoints(volume, slicing, StarsKnownSamples, 30, 0, true);
			           		}
			           	});
		}

		[Test]
		public void TestNegativeXAxialGantryTiltedSlicings()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateGantryTiltedAboutX(-15);
			TestVolume(VolumeFunction.StarsTilted345X,
			           orientation.Initialize,
			           volume =>
			           	{
			           		foreach (IVolumeSlicerParams slicing in _slicings)
			           		{
			           			ValidateVolumeSlicePoints(volume, slicing, StarsKnownSamples, -15, 0, true);
			           		}
			           	});
		}

		protected static void ValidateVolumeSlicePoints(Volumes.Volume volume, IVolumeSlicerParams slicerParams, IList<KnownSample> expectedPoints)
		{
			ValidateVolumeSlicePoints(volume, slicerParams, expectedPoints, 0, 0, false);
		}

		protected static void ValidateVolumeSlicePoints(Volumes.Volume volume, IVolumeSlicerParams slicerParams, IList<KnownSample> expectedPoints,
		                                                double xAxialGantryTilt, double yAxialGantryTilt, bool gantryTiltInDegrees)
		{
			if (gantryTiltInDegrees)
			{
				xAxialGantryTilt *= Math.PI/180;
				yAxialGantryTilt *= Math.PI/180;
			}

			Trace.WriteLine(string.Format("Using slice plane: {0}", slicerParams.Description));
			using (VolumeSlicer slicer = new VolumeSlicer(volume, slicerParams))
			{
				foreach (ISopDataSource slice in slicer.CreateSliceSops())
				{
					using (ImageSop imageSop = new ImageSop(slice))
					{
						foreach (IPresentationImage image in PresentationImageFactory.Create(imageSop))
						{
							IImageGraphicProvider imageGraphicProvider = (IImageGraphicProvider) image;
							DicomImagePlane dip = DicomImagePlane.FromImage(image);

							foreach (KnownSample sample in expectedPoints)
							{
								Vector3D patientPoint = sample.Point;
								if (xAxialGantryTilt != 0 && yAxialGantryTilt == 0)
								{
									float cos = (float) Math.Cos(xAxialGantryTilt);
									float sin = (float) Math.Sin(xAxialGantryTilt);
									patientPoint = new Vector3D(patientPoint.X,
									                            patientPoint.Y*cos + (xAxialGantryTilt > 0 ? 100*sin : 0),
									                            patientPoint.Z/cos - patientPoint.Y*sin - (xAxialGantryTilt > 0 ? 100*sin*sin/cos : 0));
								}
								else if (yAxialGantryTilt != 0)
								{
									Assert.Fail("Unit test not designed to work with gantry tilts about Y (i.e. slew)");
								}

								Vector3D slicedPoint = dip.ConvertToImagePlane(patientPoint);
								if (slicedPoint.Z > -0.5 && slicedPoint.Z < 0.5)
								{
									int actual = imageGraphicProvider.ImageGraphic.PixelData.GetPixel((int) slicedPoint.X, (int) slicedPoint.Y);
									Trace.WriteLine(string.Format("Sample {0} @{1} (SLICE: {2}; PATIENT: {3})", actual, FormatVector(sample.Point), FormatVector(slicedPoint), FormatVector(patientPoint)));
									Assert.AreEqual(sample.Value, actual, "Wrong colour sample @{0}", sample.Point);
								}
							}

							image.Dispose();
						}
					}
					slice.Dispose();
				}
			}
		}
	}
}

#endif