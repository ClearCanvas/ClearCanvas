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
#pragma warning disable 1591

using System.Drawing;
using ClearCanvas.Dicom.Iod;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Graphics.Tests
{
	[TestFixture]
	public class MammographyImageSpatialTransformTest
	{
		/// <summary>
		/// Tests that secondary directions are ignored, whatever they may be
		/// </summary>
		[Test]
		public void TestDirectionalityAndLateralityInputErrorHandling()
		{
			using (var dummy = new GrayscaleImageGraphic(10, 10))
			{
				var case1 = CreateTransform(dummy, "ALH", "FAPH", "LRRR", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(0, 5), case1.ConvertToDestination(new PointF(0, 0)), "standard view");
				AssertAreEqual(new PointF(0, 15), case1.ConvertToDestination(new PointF(0, 10)), "standard view");
				AssertAreEqual(new PointF(10, 5), case1.ConvertToDestination(new PointF(10, 0)), "standard view");

				var case2 = CreateTransform(dummy, "PAH", "RLRLR", "LHHH", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(0, 5), case2.ConvertToDestination(new PointF(10, 0)), "mirrored view");
				AssertAreEqual(new PointF(0, 15), case2.ConvertToDestination(new PointF(10, 10)), "mirrored view");
				AssertAreEqual(new PointF(10, 5), case2.ConvertToDestination(new PointF(0, 0)), "mirrored view");

				var case3 = CreateTransform(dummy, "PFL", "HAPF", "RCCC", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(20, 5), case3.ConvertToDestination(new PointF(10, 10)), "flipped view");
				AssertAreEqual(new PointF(20, 15), case3.ConvertToDestination(new PointF(10, 0)), "flipped view");
				AssertAreEqual(new PointF(10, 5), case3.ConvertToDestination(new PointF(0, 10)), "flipped view");

				var case4 = CreateTransform(dummy, "AFR", "LRDSFS", "RLLL", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(20, 5), case4.ConvertToDestination(new PointF(0, 0)), "case4 view");
				AssertAreEqual(new PointF(20, 15), case4.ConvertToDestination(new PointF(0, 10)), "case4 view");
				AssertAreEqual(new PointF(10, 5), case4.ConvertToDestination(new PointF(10, 0)), "case4 view");
			}
		}

		/// <summary>
		/// Tests that all left laterals (e.g. MLO, LMO, 90-deg-LM, etc.) are displayed consistently.
		/// </summary>
		[Test]
		public void TestNormativeLeftBreastLateralView()
		{
			using (var dummy = new GrayscaleImageGraphic(10, 10))
			{
				var standard = CreateTransform(dummy, "A", "F", "L", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(0, 5), standard.ConvertToDestination(new PointF(0, 0)), "standard view");
				AssertAreEqual(new PointF(0, 15), standard.ConvertToDestination(new PointF(0, 10)), "standard view");
				AssertAreEqual(new PointF(10, 5), standard.ConvertToDestination(new PointF(10, 0)), "standard view");

				var mirrored = CreateTransform(dummy, "P", "F", "L", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(0, 5), mirrored.ConvertToDestination(new PointF(10, 0)), "mirrored view");
				AssertAreEqual(new PointF(0, 15), mirrored.ConvertToDestination(new PointF(10, 10)), "mirrored view");
				AssertAreEqual(new PointF(10, 5), mirrored.ConvertToDestination(new PointF(0, 0)), "mirrored view");

				var flipped = CreateTransform(dummy, "A", "H", "L", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(0, 5), flipped.ConvertToDestination(new PointF(0, 10)), "flipped view");
				AssertAreEqual(new PointF(0, 15), flipped.ConvertToDestination(new PointF(0, 0)), "flipped view");
				AssertAreEqual(new PointF(10, 5), flipped.ConvertToDestination(new PointF(10, 10)), "flipped view");

				var rotated = CreateTransform(dummy, "H", "P", "L", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(0, 5), rotated.ConvertToDestination(new PointF(10, 10)), "rotated view");
				AssertAreEqual(new PointF(0, 15), rotated.ConvertToDestination(new PointF(0, 10)), "rotated view");
				AssertAreEqual(new PointF(10, 5), rotated.ConvertToDestination(new PointF(10, 0)), "rotated view");
			}
		}

		/// <summary>
		/// Tests that all right laterals (e.g. MLO, LMO, 90-deg-LM, etc.) are displayed consistently.
		/// </summary>
		[Test]
		public void TestNormativeRightBreastLateralView()
		{
			using (var dummy = new GrayscaleImageGraphic(10, 10))
			{
				var standard = CreateTransform(dummy, "P", "F", "R", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(20, 5), standard.ConvertToDestination(new PointF(10, 0)), "standard view");
				AssertAreEqual(new PointF(20, 15), standard.ConvertToDestination(new PointF(10, 10)), "standard view");
				AssertAreEqual(new PointF(10, 5), standard.ConvertToDestination(new PointF(0, 0)), "standard view");

				var mirrored = CreateTransform(dummy, "A", "F", "R", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(20, 5), mirrored.ConvertToDestination(new PointF(0, 0)), "mirrored view");
				AssertAreEqual(new PointF(20, 15), mirrored.ConvertToDestination(new PointF(0, 10)), "mirrored view");
				AssertAreEqual(new PointF(10, 5), mirrored.ConvertToDestination(new PointF(10, 0)), "mirrored view");

				var flipped = CreateTransform(dummy, "P", "H", "R", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(20, 5), flipped.ConvertToDestination(new PointF(10, 10)), "flipped view");
				AssertAreEqual(new PointF(20, 15), flipped.ConvertToDestination(new PointF(10, 0)), "flipped view");
				AssertAreEqual(new PointF(10, 5), flipped.ConvertToDestination(new PointF(0, 10)), "flipped view");

				var rotated = CreateTransform(dummy, "H", "A", "R", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(20, 5), rotated.ConvertToDestination(new PointF(10, 0)), "rotated view");
				AssertAreEqual(new PointF(20, 15), rotated.ConvertToDestination(new PointF(0, 0)), "rotated view");
				AssertAreEqual(new PointF(10, 5), rotated.ConvertToDestination(new PointF(10, 10)), "rotated view");
			}
		}

		/// <summary>
		/// Tests that all left cranial-caudals are displayed consistently.
		/// </summary>
		[Test]
		public void TestNormativeLeftBreastCranialView()
		{
			using (var dummy = new GrayscaleImageGraphic(10, 10))
			{
				var standard = CreateTransform(dummy, "A", "R", "L", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(0, 5), standard.ConvertToDestination(new PointF(0, 0)), "standard view");
				AssertAreEqual(new PointF(0, 15), standard.ConvertToDestination(new PointF(0, 10)), "standard view");
				AssertAreEqual(new PointF(10, 5), standard.ConvertToDestination(new PointF(10, 0)), "standard view");

				var mirrored = CreateTransform(dummy, "P", "R", "L", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(0, 5), mirrored.ConvertToDestination(new PointF(10, 0)), "mirrored view");
				AssertAreEqual(new PointF(0, 15), mirrored.ConvertToDestination(new PointF(10, 10)), "mirrored view");
				AssertAreEqual(new PointF(10, 5), mirrored.ConvertToDestination(new PointF(0, 0)), "mirrored view");

				var flipped = CreateTransform(dummy, "A", "L", "L", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(0, 5), flipped.ConvertToDestination(new PointF(0, 10)), "flipped view");
				AssertAreEqual(new PointF(0, 15), flipped.ConvertToDestination(new PointF(0, 0)), "flipped view");
				AssertAreEqual(new PointF(10, 5), flipped.ConvertToDestination(new PointF(10, 10)), "flipped view");

				var rotated = CreateTransform(dummy, "R", "A", "L", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(0, 5), rotated.ConvertToDestination(new PointF(0, 0)), "rotated view");
				AssertAreEqual(new PointF(0, 15), rotated.ConvertToDestination(new PointF(10, 0)), "rotated view");
				AssertAreEqual(new PointF(10, 5), rotated.ConvertToDestination(new PointF(0, 10)), "rotated view");
			}
		}

		/// <summary>
		/// Tests that all right cranial-caudals are displayed consistently.
		/// </summary>
		[Test]
		public void TestNormativeRightBreastCranialView()
		{
			using (var dummy = new GrayscaleImageGraphic(10, 10))
			{
				var standard = CreateTransform(dummy, "P", "L", "R", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(20, 5), standard.ConvertToDestination(new PointF(10, 0)), "standard view");
				AssertAreEqual(new PointF(20, 15), standard.ConvertToDestination(new PointF(10, 10)), "standard view");
				AssertAreEqual(new PointF(10, 5), standard.ConvertToDestination(new PointF(0, 0)), "standard view");

				var mirrored = CreateTransform(dummy, "A", "L", "R", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(20, 5), mirrored.ConvertToDestination(new PointF(0, 0)), "mirrored view");
				AssertAreEqual(new PointF(20, 15), mirrored.ConvertToDestination(new PointF(0, 10)), "mirrored view");
				AssertAreEqual(new PointF(10, 5), mirrored.ConvertToDestination(new PointF(10, 0)), "mirrored view");

				var flipped = CreateTransform(dummy, "P", "R", "R", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(20, 5), flipped.ConvertToDestination(new PointF(10, 10)), "flipped view");
				AssertAreEqual(new PointF(20, 15), flipped.ConvertToDestination(new PointF(10, 0)), "flipped view");
				AssertAreEqual(new PointF(10, 5), flipped.ConvertToDestination(new PointF(0, 10)), "flipped view");

				var rotated = CreateTransform(dummy, "L", "P", "R", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(20, 5), rotated.ConvertToDestination(new PointF(0, 10)), "rotated view");
				AssertAreEqual(new PointF(20, 15), rotated.ConvertToDestination(new PointF(10, 10)), "rotated view");
				AssertAreEqual(new PointF(10, 5), rotated.ConvertToDestination(new PointF(0, 0)), "rotated view");
			}
		}

		/// <summary>
		/// Tests that the posterior edge of the image always aligns against a client edge when only flips and rotations are applied.
		/// </summary>
		[Test]
		public void TestPosteriorEdgeAlignment()
		{
			using (var dummy = new GrayscaleImageGraphic(10, 10))
			{
				var transform = CreateTransform(dummy, "A", "F", "L", new Rectangle(0, 0, 20, 20));
				AssertAreEqual(new PointF(0, 10), transform.ConvertToDestination(new PointF(0, 5)), "alignment in unrotated/unflipped view");

				transform.RotationXY = 90;
				AssertAreEqual(new PointF(10, 0), transform.ConvertToDestination(new PointF(0, 5)), "alignment with 90d rotation");
				transform.RotationXY = 180;
				AssertAreEqual(new PointF(20, 10), transform.ConvertToDestination(new PointF(0, 5)), "alignment with 180d rotation");
				transform.RotationXY = 270;
				AssertAreEqual(new PointF(10, 20), transform.ConvertToDestination(new PointF(0, 5)), "alignment with 270d rotation");

				transform.RotationXY = 0;
				transform.FlipX = true;
				AssertAreEqual(new PointF(0, 10), transform.ConvertToDestination(new PointF(0, 5)), "alignment with flip");

				transform.RotationXY = 90;
				AssertAreEqual(new PointF(10, 0), transform.ConvertToDestination(new PointF(0, 5)), "alignment with flip, 90d rotation");
				transform.RotationXY = 180;
				AssertAreEqual(new PointF(20, 10), transform.ConvertToDestination(new PointF(0, 5)), "alignment with flip, 180d rotation");
				transform.RotationXY = 270;
				AssertAreEqual(new PointF(10, 20), transform.ConvertToDestination(new PointF(0, 5)), "alignment with flip, 270d rotation");
			}
		}

		private static void AssertAreEqual(PointF expected, PointF actual, string message, params object[] args)
		{
			Assert.AreEqual(expected.X, actual.X, 1e-4, message + " (X: expected" + expected + " actual" + actual + ")", args);
			Assert.AreEqual(expected.Y, actual.Y, 1e-4, message + " (Y: expected" + expected + " actual" + actual + ")", args);
		}

		private static MammographyImageSpatialTransform CreateTransform(ImageGraphic imageGraphic, string rowDirection, string colDirection, string laterality, Rectangle clientRect)
		{
			return new MammographyImageSpatialTransform(imageGraphic, imageGraphic.Rows, imageGraphic.Columns, 0.5, 0.5, 1, 1, new PatientOrientation(rowDirection, colDirection), laterality) {ClientRectangle = clientRect, Scale = 1, ScaleToFit = false};
		}
	}
}

#pragma warning restore 1591
#endif