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

namespace ClearCanvas.ImageViewer.RoiGraphics.Analyzers
{
	/// <summary>
	/// An <see cref="IRoiAnalyzer"/> that displays the area of a <see cref="RoiGraphic"/>.
	/// </summary>
	[ExtensionOf(typeof (RoiAnalyzerExtensionPoint))]
	public class RoiAreaAnalyzer : IRoiAnalyzer
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
			return roi is IRoiAreaProvider;
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

			// performance enhancement to restrict excessive computation of polygon area.
			if (mode == RoiAnalysisMode.Responsive)
			{
				if (_units == Units.Pixels)
					return new RoiAnalyzerResultNoValue("Area", String.Format(SR.LabelUnitsPixels, SR.StringNoValue));
				if (_units == Units.Millimeters)
					return new RoiAnalyzerResultNoValue("Area", String.Format(SR.LabelUnitsMm2, SR.StringNoValue));
				return
					new RoiAnalyzerResultNoValue("Area", String.Format(SR.LabelUnitsCm2, SR.StringNoValue));
			}

			IRoiAreaProvider areaProvider = (IRoiAreaProvider) roi;

			IRoiAnalyzerResult result;

			Units oldUnits = areaProvider.Units;
			areaProvider.Units = areaProvider.IsCalibrated ? _units : Units.Pixels;

			if (!areaProvider.IsCalibrated || _units == Units.Pixels)
				result = new SingleValueRoiAnalyzerResult("Area", SR.LabelUnitsPixels, areaProvider.Area,
				                                          String.Format(SR.FormatAreaPixels, areaProvider.Area));

			else if (_units == Units.Millimeters)
				result = new SingleValueRoiAnalyzerResult("Area", SR.LabelUnitsMm2, areaProvider.Area,
				                                          String.Format(SR.FormatAreaSquareMm, areaProvider.Area));

			else
				result = new SingleValueRoiAnalyzerResult("Area", SR.LabelUnitsCm2, areaProvider.Area,
				                                          String.Format(SR.FormatAreaSquareCm, areaProvider.Area));

			areaProvider.Units = oldUnits;

			return result;
		}
	}
}