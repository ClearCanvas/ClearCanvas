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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.RoiGraphics.Analyzers
{
	/// <summary>
	/// An <see cref="IRoiAnalyzer"/> that displays the length of a <see cref="RoiGraphic"/>.
	/// </summary>
	[ExtensionOf(typeof (RoiAnalyzerExtensionPoint))]
	public class RoiLengthAnalyzer : IRoiAnalyzer
	{
		private Units _units = Units.Centimeters;

		/// <summary>
		/// Gets or sets the base unit of measurement in which analysis is performed.
		/// </summary>
		public Units Units
		{
			get { return _units; }
			set { _units = value; }
		}

		/// <summary>
		/// Checks if this analyzer class can analyze the given ROI.
		/// </summary>
		/// <param name="roi">The ROI to analyze.</param>
		/// <returns>True if this class can analyze the given ROI; False otherwise.</returns>
		public bool SupportsRoi(Roi roi)
		{
			return roi is IRoiLengthProvider;
		}

		/// <summary>
		/// Analyzes the given ROI.
		/// </summary>
		/// <param name="roi">The ROI being analyzed.</param>
		/// <param name="mode">The analysis mode.</param>
		/// <returns>A string containing the analysis results, which can be appended to the analysis
		/// callout of the associated <see cref="RoiGraphic"/>, if one exists.</returns>
		public IRoiAnalyzerResult Analyze(Roi roi, RoiAnalysisMode mode)
		{
			if (!SupportsRoi(roi))
				return null;

			IRoiLengthProvider lengthProvider = (IRoiLengthProvider) roi;

			Units oldUnits = lengthProvider.Units;
			lengthProvider.Units = lengthProvider.IsCalibrated ? _units : Units.Pixels;

			IRoiAnalyzerResult result;

			if (!lengthProvider.IsCalibrated || _units == Units.Pixels)
			{
				result = new SingleValueRoiAnalyzerResult("Length", SR.FormatLengthPixels, lengthProvider.Length,
				                                          String.Format(SR.FormatLengthPixels, lengthProvider.Length));
			}
			else if (_units == Units.Millimeters)
			{
				result = new SingleValueRoiAnalyzerResult("Length", SR.FormatLengthMm, lengthProvider.Length,
				                                          String.Format(SR.FormatLengthMm, lengthProvider.Length));
			}
			else
			{
				result = new SingleValueRoiAnalyzerResult("Length", SR.FormatLengthCm, lengthProvider.Length,
				                                          String.Format(SR.FormatLengthCm, lengthProvider.Length));
			}

			lengthProvider.Units = oldUnits;

			return result;
		}

		#region Public Static Helpers

		/// <summary>
		/// Helper method to compute the physical distance between two pixels.
		/// </summary>
		/// <param name="point1">The first point.</param>
		/// <param name="point2">The second point.</param>
		/// <param name="normalizedPixelSpacing">The normalized pixel spacing of the image.</param>
		/// <param name="units">The units in which the resultant distance is given, passed by reference. If <paramref name="normalizedPixelSpacing"/> is not calibrated, then the passed variable will change to <see cref="RoiGraphics.Units.Pixels"/>.</param>
		/// <returns>The distance between the two points, in units of <paramref name="units"/>.</returns>
		public static double CalculateLength(
			PointF point1,
			PointF point2,
			PixelSpacing normalizedPixelSpacing,
			ref Units units)
		{
			if (normalizedPixelSpacing.IsNull)
				units = Units.Pixels;

			double widthInPixels = point2.X - point1.X;
			double heightInPixels = point2.Y - point1.Y;

			double length;

			if (units == Units.Pixels)
			{
				length = Math.Sqrt(widthInPixels*widthInPixels + heightInPixels*heightInPixels);
			}
			else
			{
				double widthInMm = widthInPixels*normalizedPixelSpacing.Column;
				double heightInMm = heightInPixels*normalizedPixelSpacing.Row;
				double lengthInMm = Math.Sqrt(widthInMm*widthInMm + heightInMm*heightInMm);

				if (units == Units.Millimeters)
					length = lengthInMm;
				else
					length = lengthInMm/10;
			}

			return length;
		}

		#endregion
	}
}