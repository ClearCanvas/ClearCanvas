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
using ClearCanvas.ImageViewer.Mathematics;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tests
{
	/// <summary>
	/// Tests basic operation of the <see cref="VolumeSlicer"/>.
	/// </summary>
	[TestFixture(Description = "Tests basic operation of the VolumeSlicer.")]
	public class SlicerTests : AbstractMprTest
	{
		[Test]
		public void TestInverseRotationMatrices()
		{
			for (int x = 0; x < 360; x += 15)
			{
				for (int y = 0; y < 360; y += 15)
				{
					for (int z = 0; z < 360; z += 15)
					{
						VolumeSlicerParams expected = new VolumeSlicerParams(x, y, z); // this constructor stores x, y, z separately from the matrix
						Matrix mExpected = ComputeRotationMatrix(expected.RotateAboutX, expected.RotateAboutY, expected.RotateAboutZ);

						VolumeSlicerParams test = new VolumeSlicerParams(expected.SlicingPlaneRotation); // this constructor must infer x, y, z from the matrix
						Matrix mTest = ComputeRotationMatrix(test.RotateAboutX, test.RotateAboutY, test.RotateAboutZ);

						for (int r = 0; r < 3; r++)
						{
							for (int c = 0; c < 3; c++)
							{
								Assert.AreEqual(mExpected[r, c], mTest[r, c], 1e-6, "Rotation Matrices differ at R{3}C{4} for Q={0},{1},{2}", x, y, z, r, c);
							}
						}
					}
				}
			}
		}

		private static Matrix ComputeRotationMatrix(float degreesAboutX, float degreesAboutY, float degreesAboutZ)
		{
			Matrix mTestX = Matrix.GetIdentity(3);
			mTestX[1, 1] = mTestX[2, 2] = Cos(degreesAboutX);
			mTestX[1, 2] = -(mTestX[2, 1] = Sin(degreesAboutX));

			Matrix mTestY = Matrix.GetIdentity(3);
			mTestY[0, 0] = mTestY[2, 2] = Cos(degreesAboutY);
			mTestY[2, 0] = -(mTestY[0, 2] = Sin(degreesAboutY));

			Matrix mTestZ = Matrix.GetIdentity(3);
			mTestZ[0, 0] = mTestZ[1, 1] = Cos(degreesAboutZ);
			mTestZ[0, 1] = -(mTestZ[1, 0] = Sin(degreesAboutZ));

			return mTestX*mTestY*mTestZ;
		}

		private static float Cos(float degrees)
		{
			return (float) Math.Cos(degrees*Math.PI/180);
		}

		private static float Sin(float degrees)
		{
			return (float) Math.Sin(degrees*Math.PI/180);
		}
	}
}

#endif