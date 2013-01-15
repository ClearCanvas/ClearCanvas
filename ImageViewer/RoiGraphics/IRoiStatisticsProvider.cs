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

using System;
using System.Drawing;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.RoiGraphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Defines the properties to get the mean and standard deviation of a
	/// region of interest.
	/// </summary>
	public interface IRoiStatisticsProvider
	{
		/// <summary>
		/// Gets the standard deviation of the values over the <see cref="Roi"/>.
		/// </summary>
		/// <remarks>
		/// All stored pixel values are passed through the modality LUT function if it exists before any computation takes place.
		/// </remarks>
		double StandardDeviation { get; }

		/// <summary>
		/// Gets the mean of the values over the <see cref="Roi"/>.
		/// </summary>
		/// <remarks>
		/// All stored pixel values are passed through the modality LUT function if it exists before any computation takes place.
		/// </remarks>
		double Mean { get; }
	}

	internal class RoiStatistics
	{
		public readonly double Mean;
		public readonly double StandardDeviation;
		public readonly bool Valid;

		private RoiStatistics()
		{
			this.Valid = false;
		}

		private RoiStatistics(double mean, double stddev)
		{
			this.Valid = true;
			this.Mean = mean;
			this.StandardDeviation = stddev;
		}

		private delegate bool IsPointInRoiDelegate(int x, int y);

		public static RoiStatistics Calculate(Roi roi)
		{
			if (!(roi.PixelData is GrayscalePixelData))
				return new RoiStatistics();

			double mean = CalculateMean(
				roi.BoundingBox, 
				(GrayscalePixelData)roi.PixelData, 
				roi.ModalityLut, 
				roi.Contains);

			double stdDev = CalculateStandardDeviation(
				mean, 
				roi.BoundingBox, 
				(GrayscalePixelData)roi.PixelData, 
				roi.ModalityLut, 
				roi.Contains);

			return new RoiStatistics(mean, stdDev);
		}

		private static double CalculateMean
			(
			RectangleF roiBoundingBox,
			GrayscalePixelData pixelData,
			IModalityLut modalityLut,
			IsPointInRoiDelegate isPointInRoi
			)
		{
			double sum = 0;
			int pixelCount = 0;

            var boundingBox = RectangleUtilities.RoundInflate(RectangleUtilities.ConvertToPositiveRectangle(roiBoundingBox));
			pixelData.ForEachPixel(
                boundingBox.Left,
                boundingBox.Top,
                boundingBox.Right,
                boundingBox.Bottom,
				delegate(int i, int x, int y, int pixelIndex)
					{
						if (isPointInRoi(x, y))
						{
							++pixelCount;
							// Make sure we run the raw pixel through the modality LUT
							// when doing the calculation. Note that the modality LUT
							// can be something other than a rescale intercept, so we can't
							// just run the mean through the LUT.
							int storedValue = pixelData.GetPixel(pixelIndex);
							double realValue = modalityLut != null ? modalityLut[storedValue] : storedValue;
							sum += realValue;
						}
					});

			if (pixelCount == 0)
				return 0;

			return sum/pixelCount;
		}

		private static double CalculateStandardDeviation
			(
			double mean,
			RectangleF roiBoundingBox,
			GrayscalePixelData pixelData,
			IModalityLut modalityLut,
			IsPointInRoiDelegate isPointInRoi
			)
		{
			double sum = 0;
			int pixelCount = 0;

            var boundingBox = RectangleUtilities.RoundInflate(RectangleUtilities.ConvertToPositiveRectangle(roiBoundingBox));
            pixelData.ForEachPixel(
                boundingBox.Left,
                boundingBox.Top,
                boundingBox.Right,
                boundingBox.Bottom,
                delegate(int i, int x, int y, int pixelIndex)
                {
					if (isPointInRoi(x, y)) {
						++pixelCount;
						int storedValue = pixelData.GetPixel(pixelIndex);
						double realValue = modalityLut != null ? modalityLut[storedValue] : storedValue;

						double deviation = realValue - mean;
						sum += deviation*deviation;
					}
				});

			if (pixelCount == 0)
				return 0;

			return Math.Sqrt(sum/pixelCount);
		}
	}
}