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

using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	internal sealed class ImageCalibrationAnnotationItem : AnnotationItem
	{
		public ImageCalibrationAnnotationItem()
			: base("Presentation.ImageCalibration", new AnnotationResourceResolver(typeof (ImageCalibrationAnnotationItem).Assembly)) {}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			var imageSopProvider = presentationImage as IImageSopProvider;
			if (imageSopProvider == null)
				return string.Empty;

			var details = imageSopProvider.Frame.NormalizedPixelSpacing.CalibrationDetails;
			switch (imageSopProvider.Frame.NormalizedPixelSpacing.CalibrationType)
			{
				case NormalizedPixelSpacingCalibrationType.None:
					return string.Empty;
				case NormalizedPixelSpacingCalibrationType.Manual:
					return SR.ValueManualCalibration;
				case NormalizedPixelSpacingCalibrationType.CrossSectionalSpacing:
					return SR.ValueActualSpacingCalibration;
				case NormalizedPixelSpacingCalibrationType.Detector:
					return SR.ValueDetectorSpacingCalibration;
				case NormalizedPixelSpacingCalibrationType.Geometry:
					return FormatCalibrationDetails(SR.ValueGeometricCalibration, details);
				case NormalizedPixelSpacingCalibrationType.Fiducial:
					return FormatCalibrationDetails(SR.ValueFiducialCalibration, details);
				case NormalizedPixelSpacingCalibrationType.Magnified:
					return FormatCalibrationDetails(SR.ValueMagnifiedCalibration, details);
				case NormalizedPixelSpacingCalibrationType.Unknown:
				default:
					return FormatCalibrationDetails(SR.ValueUnknownCalibration, details);
			}
		}

		private static string FormatCalibrationDetails(string calibration, string details)
		{
			if (string.IsNullOrEmpty(details))
				return calibration;
			return string.Format(SR.FormatCalibrationDetails, calibration, details);
		}
	}
}