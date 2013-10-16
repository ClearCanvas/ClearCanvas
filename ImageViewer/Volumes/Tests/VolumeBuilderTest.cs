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
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Volumes.Tests
{
	/// <summary>
	/// These unit tests exercise the VolumeBuilder and the constraints on acceptable frame data sources for MPR
	/// </summary>
	[TestFixture]
	public class VolumeBuilderTest : AbstractVolumeTest
	{
		[Test]
		public void TestWellBehavedSource()
		{
			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, null, null);
		}

		[Test]
		public void TestInsufficientFramesSource()
		{
			// it doesn't really matter what function we use
			VolumeFunction function = VolumeFunction.Void.Normalize(100);

			List<ImageSop> images = new List<ImageSop>();
			try
			{
				// create only 2 slices!!
				images.AddRange(function.CreateSops(100, 100, 2, false).Select(sopDataSource => new ImageSop(sopDataSource)));

				// this line *should* throw an exception
				using (Volume volume = Volume.Create(EnumerateFrames(images)))
				{
					Assert.Fail("Expected an exception of type {0}, instead got {1}", typeof (InsufficientFramesException), volume);
				}
			}
			catch (InsufficientFramesException) {}
			finally
			{
				DisposeAll(images);
			}
		}

		[Test]
		public void TestMissingStudySource()
		{
			try
			{
				// it doesn't really matter what function we use
				TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.StudyInstanceUid].SetEmptyValue(), null);
				Assert.Fail("Expected an exception of type {0}", typeof (NullSourceSeriesException));
			}
			catch (NullSourceSeriesException) {}
		}

		[Test]
		public void TestMissingSeriesSource()
		{
			try
			{
				// it doesn't really matter what function we use
				TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.SeriesInstanceUid].SetEmptyValue(), null);
				Assert.Fail("Expected an exception of type {0}", typeof (NullSourceSeriesException));
			}
			catch (NullSourceSeriesException) {}
		}

		[Test]
		public void TestDifferentSeriesSource()
		{
			try
			{
				// it doesn't really matter what function we use
				TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID), null);
				Assert.Fail("Expected an exception of type {0}", typeof (MultipleSourceSeriesException));
			}
			catch (MultipleSourceSeriesException) {}
		}

		[Test]
		public void TestMissingFramesOfReferenceSource()
		{
			try
			{
				// it doesn't really matter what function we use
				TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.FrameOfReferenceUid].SetEmptyValue(), null);
				Assert.Fail("Expected an exception of type {0}", typeof (NullFrameOfReferenceException));
			}
			catch (NullFrameOfReferenceException) {}
		}

		[Test]
		public void TestDifferentFramesOfReferenceSource()
		{
			try
			{
				// it doesn't really matter what function we use
				TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.FrameOfReferenceUid].SetStringValue(DicomUid.GenerateUid().UID), null);
				Assert.Fail("Expected an exception of type {0}", typeof (MultipleFramesOfReferenceException));
			}
			catch (MultipleFramesOfReferenceException) {}
		}

		[Test]
		public void TestMissingImageOrientationSource()
		{
			try
			{
				// it doesn't really matter what function we use
				TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.ImageOrientationPatient].SetEmptyValue(), null);
				Assert.Fail("Expected an exception of type {0}", typeof (NullImageOrientationException));
			}
			catch (NullImageOrientationException) {}
		}

		[Test]
		public void TestDifferentImageOrientationSource()
		{
			int n = 0;

			try
			{
				// it doesn't really matter what function we use
				TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.ImageOrientationPatient].SetStringValue(DataSetOrientation.CreateGantryTiltedAboutX(n++).ImageOrientationPatient), null);
				Assert.Fail("Expected an exception of type {0}", typeof (MultipleImageOrientationsException));
			}
			catch (MultipleImageOrientationsException) {}
		}

		[Test]
		public void TestUncalibratedImageSource()
		{
			try
			{
				// it doesn't really matter what function we use
				TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.PixelSpacing].SetEmptyValue(), null);
				Assert.Fail("Expected an exception of type {0}", typeof (UncalibratedFramesException));
			}
			catch (UncalibratedFramesException) {}
		}

		[Test]
		public void TestIsotropicPixelAspectRatioImageSource()
		{
			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.PixelSpacing].SetStringValue(@"0.9999999\1.000000"), null);
		}

		[Test]
		public void TestAnisotropicPixelAspectRatio4To3ImageSource()
		{
			try
			{
				// it doesn't really matter what function we use
				TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.PixelSpacing].SetStringValue(@"0.13333333\0.0999999"), null);
				Assert.Fail("Expected an exception of type {0}", typeof (AnisotropicPixelAspectRatioException));
			}
			catch (AnisotropicPixelAspectRatioException) {}
		}

		[Test]
		public void TestAnisotropicPixelAspectRatio3To4ImageSource()
		{
			try
			{
				// it doesn't really matter what function we use
				TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.PixelSpacing].SetStringValue(@"0.10000000\0.13333333"), null);
				Assert.Fail("Expected an exception of type {0}", typeof (AnisotropicPixelAspectRatioException));
			}
			catch (AnisotropicPixelAspectRatioException) {}
		}

		[Test]
		public void TestUnevenlySpacedFramesSource()
		{
			int sliceSpacing = 1;
			int sliceLocation = 100;

			try
			{
				// it doesn't really matter what function we use
				TestVolume(VolumeFunction.Void,
				           sopDataSource =>
				           	{
				           		sopDataSource[DicomTags.ImagePositionPatient].SetStringValue(string.Format(@"100\100\{0}", sliceLocation));
				           		sliceLocation += sliceSpacing++;
				           	}, null);
				Assert.Fail("Expected an exception of type {0}", typeof (UnevenlySpacedFramesException));
			}
			catch (UnevenlySpacedFramesException) {}
		}

		[Test]
		public void TestCoincidentFramesSource()
		{
			try
			{
				// it doesn't really matter what function we use
				TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.ImagePositionPatient].SetStringValue(@"100\100\100"), null);
				Assert.Fail("Expected an exception of type {0}", typeof (UnevenlySpacedFramesException));
			}
			catch (UnevenlySpacedFramesException) {}
		}

		[Test]
		public void TestMultiAxialGantryTiltedSource()
		{
			string imageOrientationPatient = string.Format(@"{1:f9}\{1:f9}\{0:f9}\-{0:f9}\{0:f9}\0", Math.Cos(Math.PI/4), Math.Pow(Math.Cos(Math.PI/4), 2));

			try
			{
				// it doesn't really matter what function we use
				TestVolume(VolumeFunction.Void, sopDataSource => sopDataSource[DicomTags.ImageOrientationPatient].SetStringValue(imageOrientationPatient), null);
				Assert.Fail("Expected an exception of type {0}", typeof (UnsupportedGantryTiltAxisException));
			}
			catch (UnsupportedGantryTiltAxisException) {}
		}

		[Test]
		public void Test030DegreeXAxialRotationGantryTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateGantryTiltedAboutX(30);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void Test330DegreeXAxialRotationGantryTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateGantryTiltedAboutX(-30);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void Test030DegreeYAxialRotationGantryTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateGantryTiltedAboutY(30);

			try
			{
				// it doesn't really matter what function we use
				TestVolume(VolumeFunction.Void, orientation.Initialize, null);
				Assert.Fail("Expected an exception of type {0}", typeof (UnsupportedGantryTiltAxisException));
			}
			catch (UnsupportedGantryTiltAxisException) {}
		}

		[Test]
		public void Test330DegreeYAxialRotationGantryTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateGantryTiltedAboutY(-30);

			try
			{
				// it doesn't really matter what function we use
				TestVolume(VolumeFunction.Void, orientation.Initialize, null);
				Assert.Fail("Expected an exception of type {0}", typeof (UnsupportedGantryTiltAxisException));
			}
			catch (UnsupportedGantryTiltAxisException) {}
		}

		[Test]
		public void Test030DegreeXAxialRotationCouchTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateCouchTiltedAboutX(30);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void Test330DegreeXAxialRotationCouchTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateCouchTiltedAboutX(-30);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void Test030DegreeYAxialRotationCouchTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateCouchTiltedAboutY(30);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void Test330DegreeYAxialRotationCouchTiltedSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateCouchTiltedAboutY(-30);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void TestAxialSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateAxial(false);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void TestCoronalSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateCoronal(false);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void TestSagittalSource()
		{
			DataSetOrientation orientation = DataSetOrientation.CreateSagittal(false);

			// it doesn't really matter what function we use
			TestVolume(VolumeFunction.Void, orientation.Initialize, null);
		}

		[Test]
		public void TestFilledVoxelData()
		{
			TestVolume(VolumeFunction.Stars, null,
			           volume =>
			           	{
			           		foreach (KnownSample sample in StarsKnownSamples)
			           		{
			           			int actual = volume[(int) sample.Point.X, (int) sample.Point.Y, (int) sample.Point.Z];
			           			Trace.WriteLine(string.Format("Sample {0} @{1}", actual, sample.Point));
			           			Assert.AreEqual(sample.Value, actual, "Wrong colour sample @{0}", sample.Point);
			           		}
			           	});
		}

		[Test]
		public void TestXAxialRotationGantryTiltedVoxelData()
		{
			const double angle = 30;
			DataSetOrientation orientation = DataSetOrientation.CreateGantryTiltedAboutX(angle);

			TestVolume(VolumeFunction.Stars,
			           orientation.Initialize,
			           volume =>
			           	{
			           		foreach (KnownSample sample in StarsKnownSamples)
			           		{
			           			Vector3D realPoint = sample.Point;
			           			Vector3D paddedPoint = realPoint + new Vector3D(0, (float) (Math.Tan(angle*Math.PI/180)*(100 - realPoint.Z)), 0);

			           			int actual = volume[(int) paddedPoint.X, (int) paddedPoint.Y, (int) paddedPoint.Z];
			           			Trace.WriteLine(string.Format("Sample {0} @{1} ({2} before padding)", actual, paddedPoint, realPoint));
			           			Assert.AreEqual(sample.Value, actual, "Wrong colour sample @{0} ({1} before padding)", paddedPoint, realPoint);
			           		}
			           	});
		}

		[Test]
		public void TestVoxelPaddingValueFromAttributes()
		{
			Trace.WriteLine("Testing Pixel Padding Value (0028,0120)");
			TestVolume(VolumeFunction.Void,
			           s =>
			           	{
			           		s[DicomTags.PixelRepresentation].SetInt32(0, 0);
			           		s[DicomTags.BitsStored].SetInt32(0, 16);
			           		s[DicomTags.PixelPaddingValue].SetInt32(0, 747);
			           		s[DicomTags.SmallestPixelValueInSeries].SetInt32(0, 767);
			           		s[DicomTags.SmallestImagePixelValue].SetInt32(0, 787);
			           	},
			           volume => Assert.AreEqual(747, Rescale(volume, volume.PaddingValue), "Volume padding value should be Pixel Padding Value (0028,0120) where available."));

			Trace.WriteLine("Testing Smallest Pixel Value In Series (0028,0108)");
			TestVolume(VolumeFunction.Void,
			           s =>
			           	{
			           		s[DicomTags.PixelRepresentation].SetInt32(0, 0);
			           		s[DicomTags.BitsStored].SetInt32(0, 16);
			           		s[DicomTags.SmallestPixelValueInSeries].SetInt32(0, 767);
			           		s[DicomTags.SmallestImagePixelValue].SetInt32(0, 787);
			           	},
			           volume => Assert.AreEqual(767, Rescale(volume, volume.PaddingValue), "Volume padding value should be Smallest Pixel Value In Series (0028,0108)"
			                                                                                + " if Pixel Padding Value (0028,0120) is not available."));

			Trace.WriteLine("Testing Smallest Image Pixel Value (0028,0106)");
			TestVolume(VolumeFunction.Void,
			           s =>
			           	{
			           		s[DicomTags.PixelRepresentation].SetInt32(0, 0);
			           		s[DicomTags.BitsStored].SetInt32(0, 16);
			           		s[DicomTags.SmallestImagePixelValue].SetInt32(0, 787);
			           	},
			           volume => Assert.AreEqual(787, Rescale(volume, volume.PaddingValue), "Volume padding value should be Smallest Image Pixel Value (0028,0106)"
			                                                                                + " if Pixel Padding Value (0028,0120) and Smallest Pixel Value In Series (0028,0108) are not available."));
		}

		[Test]
		public void TestVoxelPaddingValueAutoUnsigned()
		{
			Trace.WriteLine("Testing 16-bit unsigned");
			TestVolume(VolumeFunction.Void,
			           s =>
			           	{
			           		s[DicomTags.PixelRepresentation].SetInt32(0, 0);
			           		s[DicomTags.BitsStored].SetInt32(0, 16);
			           	},
			           volume => Assert.AreEqual(0, Rescale(volume, volume.PaddingValue), "Volume padding value should be 0 if images are unsigned (16-bit)."));

			Trace.WriteLine("Testing 15-bit unsigned");
			TestVolume(VolumeFunction.Void,
			           s =>
			           	{
			           		s[DicomTags.PixelRepresentation].SetInt32(0, 0);
			           		s[DicomTags.BitsStored].SetInt32(0, 15);
			           	},
			           volume => Assert.AreEqual(0, Rescale(volume, volume.PaddingValue), "Volume padding value should be 0 if images are unsigned (15-bit)."));

			Trace.WriteLine("Testing 8-bit unsigned");
			TestVolume(VolumeFunction.Void,
			           s =>
			           	{
			           		s[DicomTags.PixelRepresentation].SetInt32(0, 0);
			           		s[DicomTags.BitsStored].SetInt32(0, 8);
			           	},
			           volume => Assert.AreEqual(0, Rescale(volume, volume.PaddingValue), "Volume padding value should be 0 if images are unsigned (8-bit)."));

			Trace.WriteLine("Testing 1-bit unsigned");
			TestVolume(VolumeFunction.Void,
			           s =>
			           	{
			           		s[DicomTags.PixelRepresentation].SetInt32(0, 0);
			           		s[DicomTags.BitsStored].SetInt32(0, 1);
			           	},
			           volume => Assert.AreEqual(0, Rescale(volume, volume.PaddingValue), "Volume padding value should be 0 if images are unsigned (1-bit)."));

			Trace.WriteLine("Testing 16-bit unsigned MONOCHROME1");
			TestVolume(VolumeFunction.Void,
			           s =>
			           	{
			           		s[DicomTags.PhotometricInterpretation].SetString(0, PhotometricInterpretation.Monochrome1.Code);
			           		s[DicomTags.PixelRepresentation].SetInt32(0, 0);
			           		s[DicomTags.BitsStored].SetInt32(0, 16);
			           	},
			           volume => Assert.AreEqual(65535, Rescale(volume, volume.PaddingValue), "Volume padding value should be 65535 if images are 16-bit unsigned MONOCHROME1."));
		}

		[Test]
		public void TestVoxelPaddingValueAutoSigned()
		{
			Trace.WriteLine("Testing 16-bit signed");
			TestVolume(VolumeFunction.Void,
			           s =>
			           	{
			           		s[DicomTags.PixelRepresentation].SetInt32(0, 1);
			           		s[DicomTags.BitsStored].SetInt32(0, 16);
			           	},
			           volume => Assert.AreEqual(-32768, Rescale(volume, volume.PaddingValue), "Volume padding value should be -2**15 if images are 16-bit signed."));

			Trace.WriteLine("Testing 15-bit signed");
			TestVolume(VolumeFunction.Void,
			           s =>
			           	{
			           		s[DicomTags.PixelRepresentation].SetInt32(0, 1);
			           		s[DicomTags.BitsStored].SetInt32(0, 15);
			           	},
			           volume => Assert.AreEqual(-16384, Rescale(volume, volume.PaddingValue), "Volume padding value should be -2**14 if images are 15-bit signed."));

			Trace.WriteLine("Testing 8-bit signed");
			TestVolume(VolumeFunction.Void,
			           s =>
			           	{
			           		s[DicomTags.PixelRepresentation].SetInt32(0, 1);
			           		s[DicomTags.BitsStored].SetInt32(0, 8);
			           	},
			           volume => Assert.AreEqual(-128, Rescale(volume, volume.PaddingValue), "Volume padding value should be -2**7 if images are 8-bit signed."));

			Trace.WriteLine("Testing 2-bit signed");
			TestVolume(VolumeFunction.Void,
			           s =>
			           	{
			           		s[DicomTags.PixelRepresentation].SetInt32(0, 1);
			           		s[DicomTags.BitsStored].SetInt32(0, 2);
			           	},
			           volume => Assert.AreEqual(-2, Rescale(volume, volume.PaddingValue), "Volume padding value should be -2 if images are 2-bit signed."));

			Trace.WriteLine("Testing 16-bit signed MONOCHROME1");
			TestVolume(VolumeFunction.Void,
			           s =>
			           	{
			           		s[DicomTags.PhotometricInterpretation].SetString(0, PhotometricInterpretation.Monochrome1.Code);
			           		s[DicomTags.PixelRepresentation].SetInt32(0, 1);
			           		s[DicomTags.BitsStored].SetInt32(0, 16);
			           	},
			           volume => Assert.AreEqual(32767, Rescale(volume, volume.PaddingValue), "Volume padding value should be 32767 if images are 16-bit signed MONOCHROME1."));
		}

		[Test(Description = "Brought to you by the letter G, as in Garbage.")]
		public void TestVoxelPaddingValueBadInputs()
		{
			try
			{
				short dummySigned16;
				ushort dummyUnsigned16;

				Trace.WriteLine("Testing 17-bit unsigned");
				TestVolume(VolumeFunction.Void,
				           s =>
				           	{
				           		s[DicomTags.PixelRepresentation].SetInt32(0, 0);
				           		s[DicomTags.BitsStored].SetInt32(0, 17);
				           	},
				           volume => Assert.IsTrue(ushort.TryParse(Rescale(volume, volume.PaddingValue).ToString("d"), out dummyUnsigned16),
				                                   "Volume padding value should still be a valid UInt16 if images are purportedly 17-bit unsigned (GIGO)."));

				Trace.WriteLine("Testing 17-bit signed");
				TestVolume(VolumeFunction.Void,
				           s =>
				           	{
				           		s[DicomTags.PixelRepresentation].SetInt32(0, 1);
				           		s[DicomTags.BitsStored].SetInt32(0, 17);
				           	},
				           volume => Assert.IsTrue(short.TryParse(Rescale(volume, volume.PaddingValue).ToString("d"), out dummySigned16),
				                                   "Volume padding value should still be a valid Int16 if images are purportedly 17-bit signed (GIGO)."));

				Trace.WriteLine("Testing 1-bit signed");
				TestVolume(VolumeFunction.Void,
				           s =>
				           	{
				           		s[DicomTags.PixelRepresentation].SetInt32(0, 1);
				           		s[DicomTags.BitsStored].SetInt32(0, 1);
				           	},
				           volume => Assert.IsTrue(short.TryParse(Rescale(volume, volume.PaddingValue).ToString("d"), out dummySigned16),
				                                   "Volume padding value should still be a valid Int16 if images are purportedly 1-bit signed (GIGO)."));
			}
			catch (Exception)
			{
				Trace.WriteLine("Exception was thrown. Even if the input is garbage, the voxel padding value determination algorithm shouldn't explode...");
				throw;
			}
		}

		[Test(Description = "Validates that pixel padding attributes are correct for image encoding")]
		public void TestVoxelPaddingValueCorrectedForBug7026()
		{
			DicomTag tagPixelPaddingValueUS = new DicomTag(DicomTags.PixelPaddingValue, "Pixel Padding Value US", "pixelPaddingValueUS", DicomVr.USvr, true, 1, 1, false);
			DicomTag tagPixelPaddingValueSS = new DicomTag(DicomTags.PixelPaddingValue, "Pixel Padding Value SS", "pixelPaddingValueSS", DicomVr.SSvr, true, 1, 1, false);

			Trace.WriteLine("Testing for correct SS pixel padding value in a signed image");
			TestVolume(VolumeFunction.Void,
			           s =>
			           	{
			           		s[DicomTags.PixelRepresentation].SetInt32(0, 1);
			           		s[DicomTags.BitsStored].SetInt32(0, 16);

			           		// simulates an ILE dataset where an SS tag value is incorrectly assumed to be US
			           		s[tagPixelPaddingValueUS].Values = new[] {BitConverter.ToUInt16(BitConverter.GetBytes((short) -32000), 0)};
			           	},
			           volume => Assert.AreEqual(-32000, Rescale(volume, volume.PaddingValue), ""));

			Trace.WriteLine("Testing for correct US pixel padding value in an unsigned image");
			TestVolume(VolumeFunction.Void,
			           s =>
			           	{
			           		s[DicomTags.PixelRepresentation].SetInt32(0, 0);
			           		s[DicomTags.BitsStored].SetInt32(0, 16);

			           		// simulates an ILE dataset where a US tag value is incorrectly assumed to be SS
			           		s[tagPixelPaddingValueSS].Values = new[] {BitConverter.ToInt16(BitConverter.GetBytes((ushort) 64000), 0)};
			           	},
			           volume => Assert.AreEqual(64000, Rescale(volume, volume.PaddingValue), ""));
		}

		private static int Rescale(Volume volume, int rawVoxelValue)
		{
			var intercept = volume.DataSet[DicomTags.RescaleIntercept].GetFloat64(0, 0);
			var slope = volume.DataSet[DicomTags.RescaleSlope].GetFloat64(0, 1);
			return (int) Math.Round(rawVoxelValue*slope + intercept);
		}
	}
}

#endif