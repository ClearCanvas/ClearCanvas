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
using System.Drawing;
using System.Drawing.Imaging;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tests
{
	/// <summary>
	/// Identifies a standard algorithm implementation for use with <see cref="ImageDiff"/>.
	/// </summary>
	public enum ImageDiffAlgorithm
	{
		/// <summary>
		/// Performs a pixel-by-pixel comparison using the Euclidian distance method.
		/// </summary>
		/// <remarks>
		/// The result is the Euclidian distance between the two images.
		/// If the images have different sizes, the result is always <see cref="double.NaN"/>.
		/// Colour images are handled by computing the magnitude of the distance between the colours of two pixels in RGB 3-space and normalizing to be between 0 and 255.
		/// </remarks>
		Euclidian,

		/// <summary>
		/// Performs a pixel-by-pixel comparison by computing the mean pixel-wise difference.
		/// </summary>
		/// <remarks>
		/// The result is the mean pixel-wise difference if the results have low deviation, or <see cref="double.NaN"/> otherwise.
		/// If the images have different sizes, the result is always <see cref="double.NaN"/>.
		/// Colour images are handled by computing the magnitude of the distance between the colours of two pixels in RGB 3-space and normalizing to be between 0 and 255.
		/// </remarks>
		Legacy
	}

	/// <summary>
	/// Computes and quantifies the differences between two images.
	/// </summary>
	public abstract class ImageDiff
	{
		public virtual bool AreEqual(IPresentationImage referenceImage, IPresentationImage testImage)
		{
			using (var referenceBitmap = DrawToBitmap(referenceImage))
			{
				using (var testBitmap = DrawToBitmap(testImage))
				{
					return AreEqual(referenceBitmap, testBitmap);
				}
			}
		}

		public virtual bool AreEqual(IPresentationImage referenceImage, IPresentationImage testImage, float tolerance)
		{
			using (var referenceBitmap = DrawToBitmap(referenceImage))
			{
				using (var testBitmap = DrawToBitmap(testImage))
				{
					return AreEqual(referenceBitmap, testBitmap, tolerance);
				}
			}
		}

		public virtual bool AreEqual(Bitmap referenceImage, Bitmap testImage)
		{
			Bitmap diffImage;
			var result = PerformComparison(referenceImage, testImage, false, out diffImage);
			return FloatComparer.AreEqual((float) result, 0);
		}

		public virtual bool AreEqual(Bitmap referenceImage, Bitmap testImage, float tolerance)
		{
			Bitmap diffImage;
			var result = PerformComparison(referenceImage, testImage, false, out diffImage);
			return FloatComparer.AreEqual((float) result, 0, tolerance);
		}

		public virtual bool AreEqual(IPresentationImage referenceImage, IPresentationImage testImage, Rectangle bounds)
		{
			using (var referenceBitmap = DrawToBitmap(referenceImage))
			{
				using (var testBitmap = DrawToBitmap(testImage))
				{
					return AreEqual(referenceBitmap, testBitmap, bounds);
				}
			}
		}

		public virtual bool AreEqual(IPresentationImage referenceImage, IPresentationImage testImage, Rectangle bounds, float tolerance)
		{
			using (var referenceBitmap = DrawToBitmap(referenceImage))
			{
				using (var testBitmap = DrawToBitmap(testImage))
				{
					return AreEqual(referenceBitmap, testBitmap, bounds, tolerance);
				}
			}
		}

		public virtual bool AreEqual(Bitmap referenceImage, Bitmap testImage, Rectangle bounds)
		{
			Bitmap diffImage;
			var result = PerformComparison(referenceImage, testImage, bounds, false, out diffImage);
			return FloatComparer.AreEqual((float) result, 0);
		}

		public virtual bool AreEqual(Bitmap referenceImage, Bitmap testImage, Rectangle bounds, float tolerance)
		{
			Bitmap diffImage;
			var result = PerformComparison(referenceImage, testImage, bounds, false, out diffImage);
			return FloatComparer.AreEqual((float) result, 0, tolerance);
		}

		public virtual double Compare(IPresentationImage referenceImage, IPresentationImage testImage)
		{
			using (var referenceBitmap = DrawToBitmap(referenceImage))
			{
				using (var testBitmap = DrawToBitmap(testImage))
				{
					return Compare(referenceBitmap, testBitmap);
				}
			}
		}

		public virtual double Compare(Bitmap referenceImage, Bitmap testImage)
		{
			Bitmap diffImage;
			return PerformComparison(referenceImage, testImage, false, out diffImage);
		}

		public virtual double Compare(IPresentationImage referenceImage, IPresentationImage testImage, out Bitmap difference)
		{
			using (var referenceBitmap = DrawToBitmap(referenceImage))
			{
				using (var testBitmap = DrawToBitmap(testImage))
				{
					return Compare(referenceBitmap, testBitmap, out difference);
				}
			}
		}

		public virtual double Compare(Bitmap referenceImage, Bitmap testImage, out Bitmap difference)
		{
			return PerformComparison(referenceImage, testImage, true, out difference);
		}

		public virtual double Compare(IPresentationImage referenceImage, IPresentationImage testImage, Rectangle bounds)
		{
			using (var referenceBitmap = DrawToBitmap(referenceImage))
			{
				using (var testBitmap = DrawToBitmap(testImage))
				{
					return Compare(referenceBitmap, testBitmap, bounds);
				}
			}
		}

		public virtual double Compare(Bitmap referenceImage, Bitmap testImage, Rectangle bounds)
		{
			Bitmap diffImage;
			return PerformComparison(referenceImage, testImage, bounds, false, out diffImage);
		}

		public virtual double Compare(IPresentationImage referenceImage, IPresentationImage testImage, Rectangle bounds, out Bitmap difference)
		{
			using (var referenceBitmap = DrawToBitmap(referenceImage))
			{
				using (var testBitmap = DrawToBitmap(testImage))
				{
					return Compare(referenceBitmap, testBitmap, bounds, out difference);
				}
			}
		}

		public virtual double Compare(Bitmap referenceImage, Bitmap testImage, Rectangle bounds, out Bitmap difference)
		{
			return PerformComparison(referenceImage, testImage, bounds, true, out difference);
		}

		public virtual Bitmap Diff(IPresentationImage referenceImage, IPresentationImage testImage)
		{
			using (var referenceBitmap = DrawToBitmap(referenceImage))
			{
				using (var testBitmap = DrawToBitmap(testImage))
				{
					return Diff(referenceBitmap, testBitmap);
				}
			}
		}

		public virtual Bitmap Diff(Bitmap referenceImage, Bitmap testImage)
		{
			Bitmap diffImage;
			PerformComparison(referenceImage, testImage, true, out diffImage);
			return diffImage;
		}

		public virtual Bitmap Diff(IPresentationImage referenceImage, IPresentationImage testImage, Rectangle bounds)
		{
			using (var referenceBitmap = DrawToBitmap(referenceImage))
			{
				using (var testBitmap = DrawToBitmap(testImage))
				{
					return Diff(referenceBitmap, testBitmap, bounds);
				}
			}
		}

		public virtual Bitmap Diff(Bitmap referenceImage, Bitmap testImage, Rectangle bounds)
		{
			Bitmap diffImage;
			PerformComparison(referenceImage, testImage, bounds, true, out diffImage);
			return diffImage;
		}

		private double PerformComparison(Bitmap referenceImage, Bitmap testImage, bool generateDiffImage, out Bitmap diffImage)
		{
			return PerformComparison(referenceImage, testImage, new Rectangle(Point.Empty, referenceImage.Size), generateDiffImage, out diffImage);
		}

		protected abstract double PerformComparison(Bitmap referenceImage, Bitmap testImage, Rectangle bounds, bool generateDiffImage, out Bitmap diffImage);

		protected static Bitmap DrawToBitmap(IPresentationImage presentationImage)
		{
			var imageGraphicProvider = (IImageGraphicProvider) presentationImage;
			var annotationLayoutProvider = presentationImage as IAnnotationLayoutProvider;
			var annotationLayoutVisible = true;

			if (annotationLayoutProvider != null)
			{
				annotationLayoutVisible = annotationLayoutProvider.AnnotationLayout.Visible;
				annotationLayoutProvider.AnnotationLayout.Visible = false;
			}

			try
			{
				return presentationImage.DrawToBitmap(imageGraphicProvider.ImageGraphic.Columns, imageGraphicProvider.ImageGraphic.Rows);
			}
			finally
			{
				if (annotationLayoutProvider != null)
				{
					annotationLayoutProvider.AnnotationLayout.Visible = annotationLayoutVisible;
				}
			}
		}

		protected static Vector3D ToRgbVector(Color color)
		{
			return new Vector3D(color.R, color.G, color.B);
		}

		protected static Vector3D ToRgbVector(int argb)
		{
			return ToRgbVector(Color.FromArgb(argb));
		}

		#region Static Helpers

		public static ImageDiff GetImplementation(ImageDiffAlgorithm algorithm)
		{
			switch (algorithm)
			{
				case ImageDiffAlgorithm.Legacy:
					return new StatisticalDifferenceImageDiff();
				case ImageDiffAlgorithm.Euclidian:
				default:
					return new EuclidianImageDiff();
			}
		}

		public static bool AreEqual(ImageDiffAlgorithm algorithm, IPresentationImage referenceImage, IPresentationImage testImage)
		{
			return GetImplementation(algorithm).AreEqual(referenceImage, testImage);
		}

		public static bool AreEqual(ImageDiffAlgorithm algorithm, IPresentationImage referenceImage, IPresentationImage testImage, float tolerance)
		{
			return GetImplementation(algorithm).AreEqual(referenceImage, testImage, tolerance);
		}

		public static bool AreEqual(ImageDiffAlgorithm algorithm, Bitmap referenceImage, Bitmap testImage)
		{
			return GetImplementation(algorithm).AreEqual(referenceImage, testImage);
		}

		public static bool AreEqual(ImageDiffAlgorithm algorithm, Bitmap referenceImage, Bitmap testImage, float tolerance)
		{
			return GetImplementation(algorithm).AreEqual(referenceImage, testImage, tolerance);
		}

		public static bool AreEqual(ImageDiffAlgorithm algorithm, IPresentationImage referenceImage, IPresentationImage testImage, Rectangle bounds)
		{
			return GetImplementation(algorithm).AreEqual(referenceImage, testImage, bounds);
		}

		public static bool AreEqual(ImageDiffAlgorithm algorithm, IPresentationImage referenceImage, IPresentationImage testImage, Rectangle bounds, float tolerance)
		{
			return GetImplementation(algorithm).AreEqual(referenceImage, testImage, bounds, tolerance);
		}

		public static bool AreEqual(ImageDiffAlgorithm algorithm, Bitmap referenceImage, Bitmap testImage, Rectangle bounds)
		{
			return GetImplementation(algorithm).AreEqual(referenceImage, testImage, bounds);
		}

		public static bool AreEqual(ImageDiffAlgorithm algorithm, Bitmap referenceImage, Bitmap testImage, Rectangle bounds, float tolerance)
		{
			return GetImplementation(algorithm).AreEqual(referenceImage, testImage, bounds, tolerance);
		}

		public static double Compare(ImageDiffAlgorithm algorithm, IPresentationImage referenceImage, IPresentationImage testImage)
		{
			return GetImplementation(algorithm).Compare(referenceImage, testImage);
		}

		public static double Compare(ImageDiffAlgorithm algorithm, Bitmap referenceImage, Bitmap testImage)
		{
			return GetImplementation(algorithm).Compare(referenceImage, testImage);
		}

		public static double Compare(ImageDiffAlgorithm algorithm, IPresentationImage referenceImage, IPresentationImage testImage, out Bitmap difference)
		{
			return GetImplementation(algorithm).Compare(referenceImage, testImage, out difference);
		}

		public static double Compare(ImageDiffAlgorithm algorithm, Bitmap referenceImage, Bitmap testImage, out Bitmap difference)
		{
			return GetImplementation(algorithm).Compare(referenceImage, testImage, out difference);
		}

		public static double Compare(ImageDiffAlgorithm algorithm, IPresentationImage referenceImage, IPresentationImage testImage, Rectangle bounds)
		{
			return GetImplementation(algorithm).Compare(referenceImage, testImage, bounds);
		}

		public static double Compare(ImageDiffAlgorithm algorithm, Bitmap referenceImage, Bitmap testImage, Rectangle bounds)
		{
			return GetImplementation(algorithm).Compare(referenceImage, testImage, bounds);
		}

		public static double Compare(ImageDiffAlgorithm algorithm, IPresentationImage referenceImage, IPresentationImage testImage, Rectangle bounds, out Bitmap difference)
		{
			return GetImplementation(algorithm).Compare(referenceImage, testImage, bounds, out difference);
		}

		public static double Compare(ImageDiffAlgorithm algorithm, Bitmap referenceImage, Bitmap testImage, Rectangle bounds, out Bitmap difference)
		{
			return GetImplementation(algorithm).Compare(referenceImage, testImage, bounds, out difference);
		}

		public static Bitmap Diff(ImageDiffAlgorithm algorithm, IPresentationImage referenceImage, IPresentationImage testImage)
		{
			return GetImplementation(algorithm).Diff(referenceImage, testImage);
		}

		public static Bitmap Diff(ImageDiffAlgorithm algorithm, Bitmap referenceImage, Bitmap testImage)
		{
			return GetImplementation(algorithm).Diff(referenceImage, testImage);
		}

		public static Bitmap Diff(ImageDiffAlgorithm algorithm, IPresentationImage referenceImage, IPresentationImage testImage, Rectangle bounds)
		{
			return GetImplementation(algorithm).Diff(referenceImage, testImage, bounds);
		}

		public static Bitmap Diff(ImageDiffAlgorithm algorithm, Bitmap referenceImage, Bitmap testImage, Rectangle bounds)
		{
			return GetImplementation(algorithm).Diff(referenceImage, testImage, bounds);
		}

		#endregion

		#region Default Implementations

		private delegate void Iterator(int x, int y, int index, double delta);

		private static void PerformIterativeComparison(Bitmap referenceImage, Bitmap testImage, Rectangle bounds, bool generateDiffImage, out Bitmap diffImage, Iterator iterator)
		{
			if (referenceImage.Size != testImage.Size)
			{
				diffImage = null;
				return;
			}

			double root3 = Math.Sqrt(3);
			int width = referenceImage.Width;
			int height = referenceImage.Height;
			int count = width*height;
			var rect = new Rectangle(0, 0, width, height);

			diffImage = generateDiffImage ? new Bitmap(width, height) : null;

			unsafe
			{
				var referenceBits = referenceImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				try
				{
					var testBits = testImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
					try
					{
						int* testData = (int*) testBits.Scan0;
						int* referenceData = (int*) referenceBits.Scan0;
						for (int n = 0; n < count; n++)
						{
							var x = n%width;
							var y = n/width;
							var referenceValue = ToRgbVector(referenceData[n]);
							var testValue = ToRgbVector(testData[n]);
							var delta = 0.0;

							if (bounds.Contains(x, y))
							{
								delta = (referenceValue - testValue).Magnitude/root3;
								iterator.Invoke(x, y, n, delta);
							}

							if (generateDiffImage)
							{
								var v = (int) (Math.Min(255, Math.Max(0, delta)));
								diffImage.SetPixel(x, y, Color.FromArgb(v, v, v));
							}
						}
					}
					finally
					{
						testImage.UnlockBits(testBits);
					}
				}
				finally
				{
					referenceImage.UnlockBits(referenceBits);
				}
			}
		}

		#region Euclidian Distance

		private class EuclidianImageDiff : ImageDiff
		{
			protected override double PerformComparison(Bitmap referenceImage, Bitmap testImage, Rectangle bounds, bool generateDiffImage, out Bitmap diffImage)
			{
				if (referenceImage.Size != testImage.Size)
				{
					diffImage = null;
					return double.NaN;
				}

				var count = referenceImage.Width*referenceImage.Height;
				var sumDeltaSquares = 0.0;

				PerformIterativeComparison(referenceImage, testImage, bounds, generateDiffImage, out diffImage, (x, y, i, d) => sumDeltaSquares += d*d);

				return Math.Sqrt(sumDeltaSquares)/count;
			}
		}

		#endregion

		#region Legacy (Statistical Difference)

		private class StatisticalDifferenceImageDiff : ImageDiff
		{
			protected override double PerformComparison(Bitmap referenceImage, Bitmap testImage, Rectangle bounds, bool generateDiffImage, out Bitmap diffImage)
			{
				if (referenceImage.Size != testImage.Size)
				{
					diffImage = null;
					return double.NaN;
				}

				var list = new List<double>();

				PerformIterativeComparison(referenceImage, testImage, bounds, generateDiffImage, out diffImage, (x, y, i, d) => list.Add(d));

				var results = new Statistics(list);
				//TODO (CR Sept 2010): what's the significance of the number 16 here?
				if (results.StandardDeviation < 16)
					return results.Mean;
				return double.NaN;
			}
		}

		#endregion

		#endregion
	}
}

#endif