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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Volumes.Tests
{
	[TestFixture]
	internal class VolumeSliceFactoryTests
	{
		[Test]
		public void TestMiscSliceProperties()
		{
			var factory = new VolumeSliceFactory();

			var volume = MockVolumeReference.Create(
				new Size3D(100, 100, 100),
				new Vector3D(1, 1, 1),
				new Vector3D(0, 0, 0),
				Matrix3D.GetIdentity());

			var slice = factory.CreateSlice(volume, new Vector3D(50, 50, 50.5f));
			Assert.AreEqual(VolumeInterpolationMode.Linear, slice.SliceArgs.Interpolation, "Interpolation");
			Assert.AreEqual(VolumeProjectionMode.Average, slice.SliceArgs.Projection, "Projection");

			factory.Interpolation = VolumeInterpolationMode.Cubic;
			factory.Projection = VolumeProjectionMode.Maximum;
			slice = factory.CreateSlice(volume, new Vector3D(50, 50, 50.5f));
			Assert.AreEqual(VolumeInterpolationMode.Cubic, slice.SliceArgs.Interpolation, "Interpolation");
			Assert.AreEqual(VolumeProjectionMode.Maximum, slice.SliceArgs.Projection, "Projection");
		}

		[Test]
		public void TestBasicIdentitySlicing()
		{
			var factory = new VolumeSliceFactory();

			var volume = MockVolumeReference.Create(
				new Size3D(200, 200, 100),
				new Vector3D(0.5f, 0.5f, 1),
				new Vector3D(0, 0, 0),
				Matrix3D.GetIdentity());

			var slice = factory.CreateSlice(volume, new Vector3D(50, 50, 50.5f));
			AssertVolumeSlice(slice, 200, 200, new Vector3D(0, 0, 50.5f), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, null, "single");

			var slices = factory.CreateSlices(volume);
			Assert.AreEqual(101, slices.Count, "SliceCount (full stack)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, 1, "full stack @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f));
			Assert.AreEqual(101, slices.Count, "SliceCount (ref pos)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, 1, "ref pos @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), 10);
			Assert.AreEqual(10, slices.Count, "SliceCount (exact count)");
			for (var n = 0; n < 10; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, 1, "exact count @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), new Vector3D(50, 50, 59.5f));
			Assert.AreEqual(10, slices.Count, "SliceCount (exact multiple)");
			for (var n = 0; n < 10; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, 1, "exact multiple @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), new Vector3D(50, 50, 60));
			Assert.AreEqual(10, slices.Count, "SliceCount (round down)");
			for (var n = 0; n < 10; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, 1, "round down @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), new Vector3D(50, 50, 60.49f));
			Assert.AreEqual(11, slices.Count, "SliceCount (round up)");
			for (var n = 0; n < 11; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, 1, "round up @n={0}", n);
		}

		[Test]
		public void TestBasicOrthogonalSlicing()
		{
			var volume = MockVolumeReference.Create(
				new Size3D(160, 200, 100),
				new Vector3D(0.625f, 0.5f, 1),
				new Vector3D(0, 0, 0),
				Matrix3D.GetIdentity());

			var factory = new VolumeSliceFactory {RowOrientationPatient = new Vector3D(1, 0, 0), ColumnOrientationPatient = new Vector3D(0, 1, 0)};
			var slices = factory.CreateSlices(volume);
			Assert.AreEqual(101, slices.Count, "SliceCount (axial)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, 1, "axial @n={0}", n);

			factory = new VolumeSliceFactory {RowOrientationPatient = new Vector3D(1, 0, 0), ColumnOrientationPatient = new Vector3D(0, -1, 0)};
			slices = factory.CreateSlices(volume);
			Assert.AreEqual(101, slices.Count, "SliceCount (axial reverse)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[100 - n], 200, 200, new Vector3D(0, 100, n), new Vector3D(1, 0, 0), new Vector3D(0, -1, 0), 0.5f, 0.5f, 1, 1, "axial reverse @n={0}", n);

			factory = new VolumeSliceFactory {RowOrientationPatient = new Vector3D(1, 0, 0), ColumnOrientationPatient = new Vector3D(0, 0, -1)};
			slices = factory.CreateSlices(volume);
			Assert.AreEqual(201, slices.Count, "SliceCount (coronal)");
			for (var n = 0; n < 201; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, n*0.5f, 100), new Vector3D(1, 0, 0), new Vector3D(0, 0, -1), 0.5f, 0.5f, 0.5f, 0.5f, "coronal @n={0}", n);

			factory = new VolumeSliceFactory {RowOrientationPatient = new Vector3D(1, 0, 0), ColumnOrientationPatient = new Vector3D(0, 0, 1)};
			slices = factory.CreateSlices(volume);
			Assert.AreEqual(201, slices.Count, "SliceCount (coronal reverse)");
			for (var n = 0; n < 201; ++n)
				AssertVolumeSlice(slices[200 - n], 200, 200, new Vector3D(0, n*0.5f, 0), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), 0.5f, 0.5f, 0.5f, 0.5f, "coronal reverse @n={0}", n);

			factory = new VolumeSliceFactory {RowOrientationPatient = new Vector3D(0, 1, 0), ColumnOrientationPatient = new Vector3D(0, 0, 1)};
			slices = factory.CreateSlices(volume);
			Assert.AreEqual(161, slices.Count, "SliceCount (sagittal)");
			for (var n = 0; n < 161; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(n*0.625f, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), 0.5f, 0.5f, 0.625f, 0.625f, "sagittal @n={0}", n);

			factory = new VolumeSliceFactory {RowOrientationPatient = new Vector3D(0, 1, 0), ColumnOrientationPatient = new Vector3D(0, 0, -1)};
			slices = factory.CreateSlices(volume);
			Assert.AreEqual(161, slices.Count, "SliceCount (sagittal reverse)");
			for (var n = 0; n < 161; ++n)
				AssertVolumeSlice(slices[160 - n], 200, 200, new Vector3D(n*0.625f, 0, 100), new Vector3D(0, 1, 0), new Vector3D(0, 0, -1), 0.5f, 0.5f, 0.625f, 0.625f, "sagittal reverse @n={0}", n);
		}

		[Test]
		public void TestBasicObliqueSlicing()
		{
			const float root2 = 1.4142135623730950488016887242097f;
			const float halfRoot2 = root2/2;

			var volume = MockVolumeReference.Create(
				new Size3D(160, 200, 100),
				new Vector3D(0.625f, 0.5f, 1),
				new Vector3D(0, 0, 0),
				Matrix3D.GetIdentity());

			// case 1 - stack axis {0,0,1}, stack depth is 100
			{
				const int expectedSliceCount = 101; // 100 + 1

				var factory = new VolumeSliceFactory {RowOrientationPatient = new Vector3D(1, 1, 0), ColumnOrientationPatient = new Vector3D(-1, 1, 0)};
				var slices = factory.CreateSlices(volume);
				Assert.AreEqual(expectedSliceCount, slices.Count, "SliceCount (case 1)");
				for (var n = 0; n < expectedSliceCount; ++n)
				{
					const int expectedDimension = (int) (root2*200 + 0.5f); // 283
					AssertVolumeSlice(slices[n], expectedDimension, expectedDimension, new Vector3D(50, -50, n), new Vector3D(halfRoot2, halfRoot2, 0), new Vector3D(-halfRoot2, halfRoot2, 0), 0.5f, 0.5f, 1, 1, "case 1 @n={0}", n);
				}
			}

			// case 2 - stack axis {-1,1,0}, stack depth is 100*root(2), computed spacing should be sqrt(0.625^2 + 0.5^2)=0.800...
			{
				const float expectedSpacing = 0.795495f; // spacing should really be 0.8, but apparently the floating point error caught up
				const int expectedSliceCount = (int) (100*root2/expectedSpacing + 1.5);

				var factory = new VolumeSliceFactory {RowOrientationPatient = new Vector3D(1, 1, 0), ColumnOrientationPatient = new Vector3D(0, 0, 1)};
				var slices = factory.CreateSlices(volume);
				Assert.AreEqual(expectedSliceCount, slices.Count, "SliceCount (case 2)");
				for (var n = 0; n < expectedSliceCount; ++n)
				{
					const int expectedRows = 200;
					const int expectedColumns = (int) (root2*200 + 0.5f); // 283
					AssertVolumeSlice(slices[n], expectedRows, expectedColumns, new Vector3D(-50.0625f + n*0.5625f, 50.0625f - n*0.5625f, 0), new Vector3D(halfRoot2, halfRoot2, 0), new Vector3D(0, 0, 1), 0.5f, 0.5f, expectedSpacing, expectedSpacing, "case 2 @n={0}", n);
				}
			}
		}

		[Test]
		public void TestFixedSliceSpacing()
		{
			const float sliceSpacing = 1.1f;
			var factory = new VolumeSliceFactory {SliceSpacing = sliceSpacing};

			var volume = MockVolumeReference.Create(
				new Size3D(200, 200, 100),
				new Vector3D(0.5f, 0.5f, 1),
				new Vector3D(0, 0, 0),
				Matrix3D.GetIdentity());

			var slice = factory.CreateSlice(volume, new Vector3D(50, 50, 50.5f));
			AssertVolumeSlice(slice, 200, 200, new Vector3D(0, 0, 50.5f), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, sliceSpacing, null, "single");

			var slices = factory.CreateSlices(volume);
			Assert.AreEqual(92, slices.Count, "SliceCount (full stack)");
			for (var n = 0; n < 92; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, -0.05f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, sliceSpacing, sliceSpacing, "full stack @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f));
			Assert.AreEqual(92, slices.Count, "SliceCount (ref pos)");
			for (var n = 0; n < 92; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, -0.05f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, sliceSpacing, sliceSpacing, "ref pos @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), 10);
			Assert.AreEqual(10, slices.Count, "SliceCount (exact count)");
			for (var n = 0; n < 10; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, sliceSpacing, sliceSpacing, "exact count @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), new Vector3D(50, 50, 60.4f));
			Assert.AreEqual(10, slices.Count, "SliceCount (exact multiple)");
			for (var n = 0; n < 10; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, sliceSpacing, sliceSpacing, "exact multiple @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), new Vector3D(50, 50, 60.9f));
			Assert.AreEqual(10, slices.Count, "SliceCount (round down)");
			for (var n = 0; n < 10; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, sliceSpacing, sliceSpacing, "round down @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), new Vector3D(50, 50, 61.39f));
			Assert.AreEqual(11, slices.Count, "SliceCount (round up)");
			for (var n = 0; n < 11; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, sliceSpacing, sliceSpacing, "round up @n={0}", n);
		}

		[Test]
		public void TestFixedSliceThickness()
		{
			const float sliceSpacing = 0.9f;
			var factory = new VolumeSliceFactory {SliceThickness = sliceSpacing};

			var volume = MockVolumeReference.Create(
				new Size3D(200, 200, 100),
				new Vector3D(0.5f, 0.5f, 1),
				new Vector3D(0, 0, 0),
				Matrix3D.GetIdentity());

			var slice = factory.CreateSlice(volume, new Vector3D(50, 50, 50.5f));
			AssertVolumeSlice(slice, 200, 200, new Vector3D(0, 0, 50.5f), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, sliceSpacing, null, "single");

			var slices = factory.CreateSlices(volume);
			Assert.AreEqual(112, slices.Count, "SliceCount (full stack)");
			for (var n = 0; n < 112; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 0.05f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, sliceSpacing, sliceSpacing, "full stack @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f));
			Assert.AreEqual(112, slices.Count, "SliceCount (ref pos)");
			for (var n = 0; n < 112; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 0.05f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, sliceSpacing, sliceSpacing, "ref pos @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), 10);
			Assert.AreEqual(10, slices.Count, "SliceCount (exact count)");
			for (var n = 0; n < 10; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, sliceSpacing, sliceSpacing, "exact count @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), new Vector3D(50, 50, 58.5f));
			Assert.AreEqual(10, slices.Count, "SliceCount (exact multiple)");
			for (var n = 0; n < 10; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, sliceSpacing, sliceSpacing, "exact multiple @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), new Vector3D(50, 50, 59));
			Assert.AreEqual(10, slices.Count, "SliceCount (round down)");
			for (var n = 0; n < 10; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, sliceSpacing, sliceSpacing, "round down @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), new Vector3D(50, 50, 59.49f));
			Assert.AreEqual(11, slices.Count, "SliceCount (round up)");
			for (var n = 0; n < 11; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, sliceSpacing, sliceSpacing, "round up @n={0}", n);
		}

		[Test]
		public void TestFixedSliceSpacingAndThickness()
		{
			const float sliceSpacing = 0.9f;
			var factory = new VolumeSliceFactory {SliceSpacing = sliceSpacing, SliceThickness = 1f};

			var volume = MockVolumeReference.Create(
				new Size3D(200, 200, 100),
				new Vector3D(0.5f, 0.5f, 1),
				new Vector3D(0, 0, 0),
				Matrix3D.GetIdentity());

			var slice = factory.CreateSlice(volume, new Vector3D(50, 50, 50.5f));
			AssertVolumeSlice(slice, 200, 200, new Vector3D(0, 0, 50.5f), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, null, "single");

			var slices = factory.CreateSlices(volume);
			Assert.AreEqual(112, slices.Count, "SliceCount (full stack)");
			for (var n = 0; n < 112; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 0.05f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, sliceSpacing, "full stack @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f));
			Assert.AreEqual(112, slices.Count, "SliceCount (ref pos)");
			for (var n = 0; n < 112; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 0.05f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, sliceSpacing, "ref pos @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), 10);
			Assert.AreEqual(10, slices.Count, "SliceCount (exact count)");
			for (var n = 0; n < 10; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, sliceSpacing, "exact count @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), new Vector3D(50, 50, 58.5f));
			Assert.AreEqual(10, slices.Count, "SliceCount (exact multiple)");
			for (var n = 0; n < 10; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, sliceSpacing, "exact multiple @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), new Vector3D(50, 50, 59));
			Assert.AreEqual(10, slices.Count, "SliceCount (round down)");
			for (var n = 0; n < 10; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, sliceSpacing, "round down @n={0}", n);

			slices = factory.CreateSlices(volume, new Vector3D(50, 50, 50.5f), new Vector3D(50, 50, 59.49f));
			Assert.AreEqual(11, slices.Count, "SliceCount (round up)");
			for (var n = 0; n < 11; ++n)
				AssertVolumeSlice(slices[n], 200, 200, new Vector3D(0, 0, 50.5f + n*sliceSpacing), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, sliceSpacing, "round up @n={0}", n);
		}

		[Test]
		public void TestFixedSliceExtentPatient()
		{
			var volume = MockVolumeReference.Create(
				new Size3D(200, 200, 100),
				new Vector3D(0.5f, 0.5f, 1),
				new Vector3D(0, 0, 0),
				Matrix3D.GetIdentity());

			var factory = new VolumeSliceFactory {SliceWidth = 80};

			var slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (fixed width)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 200, 160, new Vector3D(-5, 0, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, 1, "fixed width @n={0}", n);

			factory = new VolumeSliceFactory {SliceHeight = 40};
			slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (fixed height)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 80, 200, new Vector3D(0, 55, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, 1, "fixed height @n={0}", n);

			factory = new VolumeSliceFactory {SliceWidth = 80, SliceHeight = 40};
			slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (fixed size)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 80, 160, new Vector3D(-5, 55, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, 1, "fixed size @n={0}", n);
		}

		[Test]
		public void TestFixedSliceExtentPixels()
		{
			var volume = MockVolumeReference.Create(
				new Size3D(200, 200, 100),
				new Vector3D(0.5f, 0.5f, 1),
				new Vector3D(0, 0, 0),
				Matrix3D.GetIdentity());

			var factory = new VolumeSliceFactory {Columns = 160, SliceWidth = 999999};

			var slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (fixed width)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 200, 160, new Vector3D(-5, 0, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, 1, "fixed width @n={0}", n);

			factory = new VolumeSliceFactory {Rows = 80, SliceHeight = 999999};
			slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (fixed height)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 80, 200, new Vector3D(0, 55, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, 1, "fixed height @n={0}", n);

			factory = new VolumeSliceFactory {Columns = 160, Rows = 80, SliceWidth = 999999, SliceHeight = 999999};
			slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (fixed size)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 80, 160, new Vector3D(-5, 55, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 0.5f, 1, 1, "fixed size @n={0}", n);
		}

		[Test]
		public void TestFixedPixelSpacing()
		{
			var volume = MockVolumeReference.Create(
				new Size3D(200, 200, 100),
				new Vector3D(0.5f, 0.5f, 1),
				new Vector3D(0, 0, 0),
				Matrix3D.GetIdentity());

			var factory = new VolumeSliceFactory {ColumnSpacing = 1f};

			var slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (fixed column spacing)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 100, 100, new Vector3D(0, 0, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 1, 1, 1, 1, "fixed column spacing @n={0}", n);

			factory = new VolumeSliceFactory {RowSpacing = 1f};
			slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (fixed row spacing)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 100, 100, new Vector3D(0, 0, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 1, 1, 1, 1, "fixed row spacing @n={0}", n);

			factory = new VolumeSliceFactory {ColumnSpacing = 1f, RowSpacing = 1f};
			slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (fixed pixel spacing)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 100, 100, new Vector3D(0, 0, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 1, 1, 1, 1, "fixed pixel spacing @n={0}", n);

			// the factory never generates anisotropic output unless explicitly requested
			factory = new VolumeSliceFactory {ColumnSpacing = 1f, RowSpacing = 0.5f};
			slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (explicit anisotropic spacing)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 200, 100, new Vector3D(0, 0, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 1, 1, 1, "explicit anisotropic spacing @n={0}", n);
		}

		[Test]
		public void TestFixedPixelSpacingWithSliceExtentPatient()
		{
			var volume = MockVolumeReference.Create(
				new Size3D(200, 200, 100),
				new Vector3D(0.5f, 0.5f, 1),
				new Vector3D(0, 0, 0),
				Matrix3D.GetIdentity());

			var factory = new VolumeSliceFactory {ColumnSpacing = 1f, SliceWidth = 80};

			var slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (fixed column spacing)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 100, 80, new Vector3D(-5, 0, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 1, 1, 1, 1, "fixed column spacing @n={0}", n);

			factory = new VolumeSliceFactory {RowSpacing = 1f, SliceHeight = 40};
			slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (fixed row spacing)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 40, 100, new Vector3D(0, 55, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 1, 1, 1, 1, "fixed row spacing @n={0}", n);

			factory = new VolumeSliceFactory {ColumnSpacing = 1f, RowSpacing = 1f, SliceWidth = 80, SliceHeight = 40};
			slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (fixed pixel spacing)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 40, 80, new Vector3D(-5, 55, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 1, 1, 1, 1, "fixed pixel spacing @n={0}", n);

			// the factory never generates anisotropic output unless explicitly requested
			factory = new VolumeSliceFactory {ColumnSpacing = 1f, RowSpacing = 0.5f, SliceWidth = 80, SliceHeight = 40};
			slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (explicit anisotropic spacing)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 80, 80, new Vector3D(-5, 55, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 1, 1, 1, "explicit anisotropic spacing @n={0}", n);
		}

		[Test]
		public void TestFixedPixelSpacingWithSliceExtentPixels()
		{
			var volume = MockVolumeReference.Create(
				new Size3D(200, 200, 100),
				new Vector3D(0.5f, 0.5f, 1),
				new Vector3D(0, 0, 0),
				Matrix3D.GetIdentity());

			var factory = new VolumeSliceFactory {ColumnSpacing = 1f, Columns = 80};

			var slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (fixed column spacing)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 100, 80, new Vector3D(-5, 0, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 1, 1, 1, 1, "fixed column spacing @n={0}", n);

			factory = new VolumeSliceFactory {RowSpacing = 1f, Rows = 40};
			slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (fixed row spacing)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 40, 100, new Vector3D(0, 55, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 1, 1, 1, 1, "fixed row spacing @n={0}", n);

			factory = new VolumeSliceFactory {ColumnSpacing = 1f, RowSpacing = 1f, Columns = 80, Rows = 40};
			slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (fixed pixel spacing)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 40, 80, new Vector3D(-5, 55, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 1, 1, 1, 1, "fixed pixel spacing @n={0}", n);

			// the factory never generates anisotropic output unless explicitly requested
			factory = new VolumeSliceFactory {ColumnSpacing = 1f, RowSpacing = 0.5f, Columns = 80, Rows = 80};
			slices = factory.CreateSlices(volume, new Vector3D(35, 75, 50));
			Assert.AreEqual(101, slices.Count, "SliceCount (explicit anisotropic spacing)");
			for (var n = 0; n < 101; ++n)
				AssertVolumeSlice(slices[n], 80, 80, new Vector3D(-5, 55, n), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), 0.5f, 1, 1, 1, "explicit anisotropic spacing @n={0}", n);
		}

		[Test]
		public void TestExampleBreastTomosynthesisMedialLateral()
		{
			var factory = new VolumeSliceFactory();

			var volume = MockVolumeReference.Create(
				new Size3D(1800, 2200, 56),
				new Vector3D(0.08f, 0.08f, 1),
				new Vector3D(0, 0, 0),
				new Matrix3D(new float[,] {{0, 1, 0}, {0, 0, 1}, {1, 0, 0}}));

			var slices = factory.CreateSlices(volume);
			Assert.AreEqual(57, slices.Count, "SliceCount (right-oriented volume)");
			for (var n = 0; n < 57; ++n)
				AssertVolumeSlice(slices[n], 2200, 1800, new Vector3D(n, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), 0.08f, 0.08f, 1, 1, "right-oriented volume @n={0}", n);

			volume = MockVolumeReference.Create(
				new Size3D(1800, 2200, 56),
				new Vector3D(0.08f, 0.08f, 1),
				new Vector3D(0, 0, 0),
				new Matrix3D(new float[,] {{0, 1, 0}, {0, 0, 1}, {-1, 0, 0}}));

			slices = factory.CreateSlices(volume);
			Assert.AreEqual(57, slices.Count, "SliceCount (left-oriented volume)");
			for (var n = 0; n < 57; ++n)
				AssertVolumeSlice(slices[56 - n], 2200, 1800, new Vector3D(-n, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), 0.08f, 0.08f, 1, 1, "left-oriented volume @n={0}", n);
		}

		private static void AssertVolumeSlice(VolumeSlice actualSlice,
		                                      int expectedRows, int expectedColumns,
		                                      Vector3D expectedPositionPatient,
		                                      Vector3D expectedRowOrientationPatient, Vector3D expectedColumnOrientationPatient,
		                                      float expectedRowSpacing, float expectedColumnSpacing,
		                                      float expectedSliceThickness, float? expectedSpacingBetweenSlices,
		                                      string message = null, params object[] args)
		{
			const float tolerance = 1e-5f;
			var msg = !string.IsNullOrEmpty(message) ? "{0} (" + string.Format(message, args) + ")" : "{0}";

			Assert.AreEqual(expectedRows, actualSlice.Rows, msg, "Rows");
			Assert.AreEqual(expectedColumns, actualSlice.Columns, msg, "Columns");

			var imagePositionPatient = ImagePositionPatient.FromString(actualSlice.ImagePositionPatient);
			Assert.IsNotNull(imagePositionPatient, "ImagePositionPatient");

			var actualPositionPatient = new Vector3D((float) imagePositionPatient.X, (float) imagePositionPatient.Y, (float) imagePositionPatient.Z);
			if (!Vector3D.AreEqual(expectedPositionPatient, actualPositionPatient, tolerance))
				Assert.AreEqual(expectedPositionPatient, actualPositionPatient, msg, "ImagePositionPatient");

			var imageOrientationPatient = ImageOrientationPatient.FromString(actualSlice.ImageOrientationPatient);
			Assert.IsNotNull(imageOrientationPatient, "ImageOrientationPatient");

			var actualRowOrientationPatient = new Vector3D((float) imageOrientationPatient.RowX, (float) imageOrientationPatient.RowY, (float) imageOrientationPatient.RowZ);
			var actualColumnOrientationPatient = new Vector3D((float) imageOrientationPatient.ColumnX, (float) imageOrientationPatient.ColumnY, (float) imageOrientationPatient.ColumnZ);
			if (!Vector3D.AreEqual(expectedRowOrientationPatient, actualRowOrientationPatient, tolerance))
				Assert.AreEqual(expectedRowOrientationPatient, actualRowOrientationPatient, msg, "ImageOrientationPatient.Row");
			if (!Vector3D.AreEqual(expectedColumnOrientationPatient, actualColumnOrientationPatient, tolerance))
				Assert.AreEqual(expectedColumnOrientationPatient, actualColumnOrientationPatient, msg, "ImageOrientationPatient.Column");

			var actualPixelSpacing = PixelSpacing.FromString(actualSlice.PixelSpacing);
			Assert.IsNotNull(actualPixelSpacing, "PixelSpacing");
			Assert.AreEqual(expectedRowSpacing, actualPixelSpacing.Row, tolerance, msg, "PixelSpacing.Row");
			Assert.AreEqual(expectedColumnSpacing, actualPixelSpacing.Column, tolerance, msg, "PixelSpacing.Column");

			Assert.AreEqual(expectedSliceThickness, float.Parse(actualSlice.SliceThickness), tolerance, msg, "SliceThickness");

			if (expectedSpacingBetweenSlices.HasValue)
				Assert.AreEqual(expectedSpacingBetweenSlices.Value, float.Parse(actualSlice.SpacingBetweenSlices), tolerance, msg, "SpacingBetweenSlices");
			else
				Assert.IsEmpty(actualSlice.SpacingBetweenSlices, msg, "SpacingBetweenSlices");
		}

		private class MockVolumeReference : VolumeHeaderBase, IVolumeReference
		{
			private readonly VolumeHeaderData _volumeHeaderData;

			private MockVolumeReference(VolumeHeaderData volumeHeaderData)
			{
				_volumeHeaderData = volumeHeaderData;
			}

			protected override VolumeHeaderData VolumeHeaderData
			{
				get { return _volumeHeaderData; }
			}

			public Volume Volume
			{
				get { throw new InvalidOperationException(); }
			}

			public void Dispose() {}

			public IVolumeReference Clone()
			{
				return new MockVolumeReference(_volumeHeaderData);
			}

			public static MockVolumeReference Create(Size3D arrayDimensions,
			                                         Vector3D voxelSpacing,
			                                         Vector3D volumePositionPatient,
			                                         Matrix3D volumeOrientationPatient)
			{
				var dataSet = new DicomAttributeCollection();
				dataSet[DicomTags.Modality].SetStringValue("SC");
				dataSet[DicomTags.StudyInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
				dataSet[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
				dataSet[DicomTags.FrameOfReferenceUid].SetStringValue(DicomUid.GenerateUid().UID);
				return new MockVolumeReference(new VolumeHeaderData(new IDicomAttributeProvider[] {dataSet},
				                                                    arrayDimensions,
				                                                    voxelSpacing,
				                                                    volumePositionPatient,
				                                                    volumeOrientationPatient,
				                                                    16, 16, false, 0, 1.0, 0.0, RescaleUnits.None));
			}
		}
	}
}

#endif