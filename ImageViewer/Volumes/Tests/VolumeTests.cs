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

using ClearCanvas.Common.Utilities.Tests;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Volumes.Tests
{
	[TestFixture]
	public class VolumeTests : AbstractVolumeTest
	{
		[Test]
		public void TestVolumeU16()
		{
			const ushort expectedMinimum = 100;
			const ushort expectedMaximum = 200;

			var rng = new PseudoRandom(0x48E4B0B4);
			var data = new ushort[1000];
			for (var n = 1; n < data.Length - 1; ++n)
				data[n] = (ushort) rng.Next(expectedMinimum + 1, expectedMaximum - 1);
			data[0] = expectedMaximum;
			data[999] = expectedMinimum;

			var volume = new U16Volume(data, new Size3D(5, 10, 20), new Vector3D(2, 1, 0.5f), new Vector3D(5, 0, -5),
			                           new Matrix3D(new float[,] {{0, 1, 0}, {0, 0, 1}, {1, 0, 0}}), new DicomAttributeCollection(), 123);

			Assert.AreSame(data, volume.Array, "Array");
			Assert.AreSame(data, volume.ArrayData, "ArrayData");
			Assert.AreEqual(new Size3D(5, 10, 20), volume.ArrayDimensions, "ArrayDimensions");
			Assert.AreEqual(1000, volume.ArrayLength, "ArrayLength");

			Assert.AreEqual(16, volume.BitsPerVoxel, "BitsPerVoxel");
			Assert.AreEqual(false, volume.Signed, "Signed");

			Assert.AreEqual(123, volume.PaddingValue, "PaddingValue");

			Assert.AreEqual(new Vector3D(10, 10, 10), volume.VolumeSize, "VolumeSize");
			Assert.AreEqual(new Rectangle3D(0, 0, 0, 10, 10, 10), volume.VolumeBounds, "VolumeBounds");
			Assert.AreEqual(new Vector3D(5, 5, 5), volume.VolumeCenter, "VolumeCenter");
			Assert.AreEqual(new Vector3D(10, 5, 0), volume.VolumeCenterPatient, "VolumeCenterPatient");
			Assert.AreEqual(new Vector3D(0, 1, 0), volume.VolumeOrientationPatientX, "VolumeOrientationPatientX");
			Assert.AreEqual(new Vector3D(0, 0, 1), volume.VolumeOrientationPatientY, "VolumeOrientationPatientY");
			Assert.AreEqual(new Vector3D(1, 0, 0), volume.VolumeOrientationPatientZ, "VolumeOrientationPatientZ");
			Assert.AreEqual(new Vector3D(5, 0, -5), volume.VolumePositionPatient, "VolumePositionPatient");

			Assert.AreEqual(expectedMinimum, volume.MinimumVolumeValue, "MinimumVolumeValue");
			Assert.AreEqual(expectedMaximum, volume.MaximumVolumeValue, "MaximumVolumeValue");
		}

		[Test]
		public void TestVolumeS16()
		{
			const short expectedMinimum = -100;
			const short expectedMaximum = 200;

			var rng = new PseudoRandom(0x48E4B0B4);
			var data = new short[1000];
			for (var n = 1; n < data.Length - 1; ++n)
				data[n] = (short) rng.Next(expectedMinimum + 1, expectedMaximum - 1);
			data[0] = expectedMaximum;
			data[999] = expectedMinimum;

			var volume = new S16Volume(data, new Size3D(5, 10, 20), new Vector3D(2, 1, 0.5f), new Vector3D(5, 0, -5),
			                           new Matrix3D(new float[,] {{0, 1, 0}, {0, 0, 1}, {1, 0, 0}}), new DicomAttributeCollection(), 123);

			Assert.AreSame(data, volume.Array, "Array");
			Assert.AreSame(data, volume.ArrayData, "ArrayData");
			Assert.AreEqual(new Size3D(5, 10, 20), volume.ArrayDimensions, "ArrayDimensions");
			Assert.AreEqual(1000, volume.ArrayLength, "ArrayLength");

			Assert.AreEqual(16, volume.BitsPerVoxel, "BitsPerVoxel");
			Assert.AreEqual(true, volume.Signed, "Signed");

			Assert.AreEqual(123, volume.PaddingValue, "PaddingValue");

			Assert.AreEqual(new Vector3D(10, 10, 10), volume.VolumeSize, "VolumeSize");
			Assert.AreEqual(new Rectangle3D(0, 0, 0, 10, 10, 10), volume.VolumeBounds, "VolumeBounds");
			Assert.AreEqual(new Vector3D(5, 5, 5), volume.VolumeCenter, "VolumeCenter");
			Assert.AreEqual(new Vector3D(10, 5, 0), volume.VolumeCenterPatient, "VolumeCenterPatient");
			Assert.AreEqual(new Vector3D(0, 1, 0), volume.VolumeOrientationPatientX, "VolumeOrientationPatientX");
			Assert.AreEqual(new Vector3D(0, 0, 1), volume.VolumeOrientationPatientY, "VolumeOrientationPatientY");
			Assert.AreEqual(new Vector3D(1, 0, 0), volume.VolumeOrientationPatientZ, "VolumeOrientationPatientZ");
			Assert.AreEqual(new Vector3D(5, 0, -5), volume.VolumePositionPatient, "VolumePositionPatient");

			Assert.AreEqual(expectedMinimum, volume.MinimumVolumeValue, "MinimumVolumeValue");
			Assert.AreEqual(expectedMaximum, volume.MaximumVolumeValue, "MaximumVolumeValue");
		}

		[Test]
		public void TestVolumeU8()
		{
			const byte expectedMinimum = 100;
			const byte expectedMaximum = 200;

			var rng = new PseudoRandom(0x48E4B0B4);
			var data = new byte[1000];
			for (var n = 1; n < data.Length - 1; ++n)
				data[n] = (byte) rng.Next(expectedMinimum + 1, expectedMaximum - 1);
			data[0] = expectedMaximum;
			data[999] = expectedMinimum;

			var volume = new U8Volume(data, new Size3D(5, 10, 20), new Vector3D(2, 1, 0.5f), new Vector3D(5, 0, -5),
			                          new Matrix3D(new float[,] {{0, 1, 0}, {0, 0, 1}, {1, 0, 0}}), new DicomAttributeCollection(), 123);

			Assert.AreSame(data, volume.Array, "Array");
			Assert.AreSame(data, volume.ArrayData, "ArrayData");
			Assert.AreEqual(new Size3D(5, 10, 20), volume.ArrayDimensions, "ArrayDimensions");
			Assert.AreEqual(1000, volume.ArrayLength, "ArrayLength");

			Assert.AreEqual(8, volume.BitsPerVoxel, "BitsPerVoxel");
			Assert.AreEqual(false, volume.Signed, "Signed");

			Assert.AreEqual(123, volume.PaddingValue, "PaddingValue");

			Assert.AreEqual(new Vector3D(10, 10, 10), volume.VolumeSize, "VolumeSize");
			Assert.AreEqual(new Rectangle3D(0, 0, 0, 10, 10, 10), volume.VolumeBounds, "VolumeBounds");
			Assert.AreEqual(new Vector3D(5, 5, 5), volume.VolumeCenter, "VolumeCenter");
			Assert.AreEqual(new Vector3D(10, 5, 0), volume.VolumeCenterPatient, "VolumeCenterPatient");
			Assert.AreEqual(new Vector3D(0, 1, 0), volume.VolumeOrientationPatientX, "VolumeOrientationPatientX");
			Assert.AreEqual(new Vector3D(0, 0, 1), volume.VolumeOrientationPatientY, "VolumeOrientationPatientY");
			Assert.AreEqual(new Vector3D(1, 0, 0), volume.VolumeOrientationPatientZ, "VolumeOrientationPatientZ");
			Assert.AreEqual(new Vector3D(5, 0, -5), volume.VolumePositionPatient, "VolumePositionPatient");

			Assert.AreEqual(expectedMinimum, volume.MinimumVolumeValue, "MinimumVolumeValue");
			Assert.AreEqual(expectedMaximum, volume.MaximumVolumeValue, "MaximumVolumeValue");
		}

		[Test]
		public void TestVolumeS8()
		{
			const sbyte expectedMinimum = -100;
			const sbyte expectedMaximum = 100;

			var rng = new PseudoRandom(0x48E4B0B4);
			var data = new sbyte[1000];
			for (var n = 1; n < data.Length - 1; ++n)
				data[n] = (sbyte) rng.Next(expectedMinimum + 1, expectedMaximum - 1);
			data[0] = expectedMaximum;
			data[999] = expectedMinimum;

			var volume = new S8Volume(data, new Size3D(5, 10, 20), new Vector3D(2, 1, 0.5f), new Vector3D(5, 0, -5),
			                          new Matrix3D(new float[,] {{0, 1, 0}, {0, 0, 1}, {1, 0, 0}}), new DicomAttributeCollection(), 123);

			Assert.AreSame(data, volume.Array, "Array");
			Assert.AreSame(data, volume.ArrayData, "ArrayData");
			Assert.AreEqual(new Size3D(5, 10, 20), volume.ArrayDimensions, "ArrayDimensions");
			Assert.AreEqual(1000, volume.ArrayLength, "ArrayLength");

			Assert.AreEqual(8, volume.BitsPerVoxel, "BitsPerVoxel");
			Assert.AreEqual(true, volume.Signed, "Signed");

			Assert.AreEqual(123, volume.PaddingValue, "PaddingValue");

			Assert.AreEqual(new Vector3D(10, 10, 10), volume.VolumeSize, "VolumeSize");
			Assert.AreEqual(new Rectangle3D(0, 0, 0, 10, 10, 10), volume.VolumeBounds, "VolumeBounds");
			Assert.AreEqual(new Vector3D(5, 5, 5), volume.VolumeCenter, "VolumeCenter");
			Assert.AreEqual(new Vector3D(10, 5, 0), volume.VolumeCenterPatient, "VolumeCenterPatient");
			Assert.AreEqual(new Vector3D(0, 1, 0), volume.VolumeOrientationPatientX, "VolumeOrientationPatientX");
			Assert.AreEqual(new Vector3D(0, 0, 1), volume.VolumeOrientationPatientY, "VolumeOrientationPatientY");
			Assert.AreEqual(new Vector3D(1, 0, 0), volume.VolumeOrientationPatientZ, "VolumeOrientationPatientZ");
			Assert.AreEqual(new Vector3D(5, 0, -5), volume.VolumePositionPatient, "VolumePositionPatient");

			Assert.AreEqual(expectedMinimum, volume.MinimumVolumeValue, "MinimumVolumeValue");
			Assert.AreEqual(expectedMaximum, volume.MaximumVolumeValue, "MaximumVolumeValue");
		}
	}
}

#endif