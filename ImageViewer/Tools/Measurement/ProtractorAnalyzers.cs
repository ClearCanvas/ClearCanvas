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
using ClearCanvas.ImageViewer.RoiGraphics;
using ClearCanvas.ImageViewer.RoiGraphics.Analyzers;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[ExtensionOf(typeof(RoiAnalyzerExtensionPoint))]
	public class ProtractorAngleCalculator : IRoiAnalyzer
	{
	    private RoiAnalyzerUpdateCallback _updateCallback;

	    Units IRoiAnalyzer.Units
		{
			get { return Units.Centimeters; }
			set { }
		}

        ////TODO (cr Feb 2010): All the analysis should really be done in the ProtractorRoiInfo.
        //public string Analyze(ProtractorRoiInfo roiInfo, RoiAnalysisMode mode)
        //{
        //    // Don't show the callout until the second ray is drawn
        //    if (roiInfo.Points.Count < 3)
        //        return SR.ToolsMeasurementSetVertex;

        //    List<PointF> normalizedPoints = NormalizePoints(roiInfo);

        //    double angle = Vector.SubtendedAngle(normalizedPoints[0], normalizedPoints[1], normalizedPoints[2]);

        //    return String.Format(SR.ToolsMeasurementFormatDegrees, Math.Abs(angle));
        //}

        //TODO (cr Feb 2010): All the analysis should really be done in the ProtractorRoiInfo.
        public IRoiAnalyzerResult Analyze(Roi roi, RoiAnalysisMode mode)
        {
            return Analyze((ProtractorRoiInfo) roi, mode);
        }

        private IRoiAnalyzerResult Analyze(ProtractorRoiInfo roiInfo, RoiAnalysisMode mode)
        {
            // Don't show the callout until the second ray is drawn
            if (roiInfo.Points.Count < 3)
            {
                //return SR.ToolsMeasurementSetVertex;
                return new RoiAnalyzerResultNoValue("Protactor", SR.ToolsMeasurementSetVertex);
            }

            List<PointF> normalizedPoints = NormalizePoints(roiInfo);

            double angle = Vector.SubtendedAngle(normalizedPoints[0], normalizedPoints[1], normalizedPoints[2]);

            //return String.Format(SR.ToolsMeasurementFormatDegrees, Math.Abs(angle));
            return new SingleValueRoiAnalyzerResult("Protactor", SR.ToolsMeasurementFormatDegrees, Math.Abs(angle),String.Format(SR.ToolsMeasurementFormatDegrees, Math.Abs(angle)));
        }

		private List<PointF> NormalizePoints(ProtractorRoiInfo roiInfo)
		{
			float aspectRatio = 1F;

			if (roiInfo.PixelAspectRatio.IsNull)
			{
				if (!roiInfo.NormalizedPixelSpacing.IsNull)
					aspectRatio = (float)roiInfo.NormalizedPixelSpacing.AspectRatio;
			}
			else
			{
				aspectRatio = roiInfo.PixelAspectRatio.Value;
			}

			List<PointF> normalized = new List<PointF>();
			foreach (PointF point in roiInfo.Points)
				normalized.Add(new PointF(point.X, point.Y * aspectRatio));

			return normalized;
		}

	    
        public void SetRoiAnalyzerUpdateCallback(RoiAnalyzerUpdateCallback callback)
	    {
	        _updateCallback = callback;
	    }

	    public bool SupportsRoi(Roi roi)
		{
			return roi is ProtractorRoiInfo;
		}
	}
}
