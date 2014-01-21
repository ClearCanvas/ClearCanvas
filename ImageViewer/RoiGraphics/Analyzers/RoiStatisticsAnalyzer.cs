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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.RoiGraphics.Analyzers
{
	/// <summary>
	/// An <see cref="IRoiAnalyzer"/> that displays common statistics of a <see cref="RoiGraphic"/>.
	/// </summary>
	[ExtensionOf(typeof (RoiAnalyzerExtensionPoint))]
	public class RoiStatisticsAnalyzer : IRoiAnalyzer
	{
		/// <summary>
		/// This property is not applicable to this analyzer.
		/// </summary>
		Units IRoiAnalyzer.Units
		{
			get { return Units.Centimeters; }
			set { }
		}

		/// <summary>
		/// Checks if this analyzer class can analyze the given ROI.
		/// </summary>
		/// <remarks>
		/// Implementations should return a result based on the type of ROI, not on the particular current state of the ROI.
		/// </remarks>
		/// <param name="roi">The ROI to analyze.</param>
		/// <returns>True if this class can analyze the given ROI; False otherwise.</returns>
		public bool SupportsRoi(Roi roi)
		{
			return roi is IRoiStatisticsProvider && roi.PixelData != null;
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

			IRoiStatisticsProvider statisticsProvider = (IRoiStatisticsProvider) roi;

			MultiValueRoiAnalyzerResult result = new MultiValueRoiAnalyzerResult("Statistics") {ShowHeader = false};

			bool isGrayscale = roi.PixelData is GrayscalePixelData;

			var meanValue = SR.StringNotApplicable;
			var stdDevValue = SR.StringNotApplicable;

			if (isGrayscale && roi.ContainsPixelData)
			{
				if (mode == RoiAnalysisMode.Responsive)
				{
					meanValue = stdDevValue = SR.StringNoValue;
					result.Add(new RoiAnalyzerResultNoValue("Mean", String.Format(SR.FormatMean, meanValue)));
					result.Add(new RoiAnalyzerResultNoValue("StdDev", String.Format(SR.FormatStdDev, stdDevValue)));
				}
				else
				{
					double mean = statisticsProvider.Mean;
					double stdDev = statisticsProvider.StandardDeviation;

					var units = roi.ModalityLutUnits.Label;
					var displayFormat = @"{0:" + (roi.SubnormalModalityLut ? @"G3" : @"F1") + "}" + (!string.IsNullOrEmpty(units) ? ' ' + units : string.Empty);

					meanValue = string.Format(displayFormat, mean);
					stdDevValue = string.Format(displayFormat, stdDev);

					result.Add(new SingleValueRoiAnalyzerResult("Mean", units, mean,
					                                            String.Format(SR.FormatMean, meanValue)));
					result.Add(new SingleValueRoiAnalyzerResult("StdDev", units, stdDevValue,
					                                            String.Format(SR.FormatStdDev, stdDevValue)));
				}
			}
			else
			{
				result.Add(new RoiAnalyzerResultNoValue("Mean", String.Format(SR.FormatMean, meanValue)));
				result.Add(new RoiAnalyzerResultNoValue("StdDev", String.Format(SR.FormatStdDev, stdDevValue)));
			}

			return result;
		}
	}
}