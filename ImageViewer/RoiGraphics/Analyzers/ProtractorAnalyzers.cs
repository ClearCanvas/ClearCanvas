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
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.RoiGraphics.Analyzers
{
	[ExtensionOf(typeof (RoiAnalyzerExtensionPoint))]
	public class ProtractorAnalyzer : IRoiAnalyzer
	{
		Units IRoiAnalyzer.Units
		{
			get { return Units.Centimeters; }
			set { }
		}

		//TODO (cr Feb 2010): All the analysis should really be done in the ProtractorRoi.
		public IRoiAnalyzerResult Analyze(Roi roi, RoiAnalysisMode mode)
		{
			return Analyze((ProtractorRoi) roi, mode);
		}

		private IRoiAnalyzerResult Analyze(ProtractorRoi roi, RoiAnalysisMode mode)
		{
			// Don't show the callout until the second ray is drawn
			if (roi.Points.Count < 3)
			{
				return new RoiAnalyzerResultNoValue("Protactor", SR.StringNoValue);
			}

			List<PointF> normalizedPoints = NormalizePoints(roi);

			double angle = Vector.SubtendedAngle(normalizedPoints[0], normalizedPoints[1], normalizedPoints[2]);

			return new SingleValueRoiAnalyzerResult("Protactor", SR.FormatAngleDegrees, Math.Abs(angle), String.Format(SR.FormatAngleDegrees, Math.Abs(angle)));
		}

		private List<PointF> NormalizePoints(ProtractorRoi roi)
		{
			float aspectRatio = 1F;

			if (roi.PixelAspectRatio.IsNull)
			{
				if (!roi.NormalizedPixelSpacing.IsNull)
					aspectRatio = (float) roi.NormalizedPixelSpacing.AspectRatio;
			}
			else
			{
				aspectRatio = roi.PixelAspectRatio.Value;
			}

			List<PointF> normalized = new List<PointF>();
			foreach (PointF point in roi.Points)
				normalized.Add(new PointF(point.X, point.Y*aspectRatio));

			return normalized;
		}

		public bool SupportsRoi(Roi roi)
		{
			return roi is ProtractorRoi;
		}
	}
}