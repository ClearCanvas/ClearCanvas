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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// The pixel spacing appropriate to the modality.
	/// </summary>
	/// <remarks>
	/// For projection based modalities (i.e. CR, DX and MG), Imager Pixel Spacing is
	/// returned as the pixel spacing.  For all other modalities, the standard
	/// Pixel Spacing is returned.
	/// </remarks>
	public class NormalizedPixelSpacing : PixelSpacing
	{
		private event EventHandler _calibrated;
		private readonly Frame _frame;

		/// <summary>
		/// Initializes a new instance of <see cref="NormalizedPixelSpacing"/>.
		/// </summary>
		internal NormalizedPixelSpacing(Frame frame)
		{
			_frame = frame;
			Initialize();
		}

		/// <summary>
		/// Event fired when the pixel spacing is calibrated.
		/// </summary>
		public event EventHandler Calibrated
		{
			add { _calibrated += value; }
			remove { _calibrated -= value; }
		}

		/// <summary>
		/// Gets a value indicating the type of calibration represented by this pixel spacing.
		/// </summary>
		public NormalizedPixelSpacingCalibrationType CalibrationType { get; private set; }

		/// <summary>
		/// Gets a value providing additional details or parameters known about the calibration specified by <see cref="CalibrationType"/>.
		/// </summary>
		public string CalibrationDetails { get; private set; }

		/// <summary>
		/// Manually calibrates the pixel spacing.
		/// </summary>
		/// <param name="pixelSpacingRow">Pixel height.</param>
		/// <param name="pixelSpacingColumn">Pixel width.</param>
		/// <remarks>
		/// Using this method does not alter the original DICOM pixel spacing
		/// contained in the <see cref="Frame"/>.
		/// </remarks>
		public void Calibrate(double pixelSpacingRow, double pixelSpacingColumn)
		{
			Row = pixelSpacingRow;
			Column = pixelSpacingColumn;
			CalibrationType = NormalizedPixelSpacingCalibrationType.Manual;
			CalibrationDetails = String.Empty;
			OnCalibrated();
		}

		/// <summary>
		/// Resets any manual calibration of the pixel spacing back to the original DICOM pixel spacing
		/// contained in the <see cref="Frame"/>.
		/// </summary>
		public void Reset()
		{
			Initialize();
			OnCalibrated();
		}

		/// <summary>
		/// Called when the pixel spacing is calibrated.
		/// </summary>
		protected virtual void OnCalibrated()
		{
			EventsHelper.Fire(_calibrated, this, new EventArgs());
		}

		private void Initialize()
		{
			// since DICOM 2006, projection image SOP classes are now allowed to use both Pixel Spacing and Imager Pixel Spacing
			// (there are some particulars about what is required and optional based on SOP class, but that's what it boils down to)
			// * Imager Pixel Spacing (0018,1164) *ALWAYS* refers to the spacing at the detector plane
			// * Pixel Spacing (0028,0030) may refer to a number of different spacing values:
			//   1. spacing at the detector plane (due to past ubiquitous misuse of this attribute)
			//   2. spacing calibrated against an assumed geometry or fiducials of known size (with details provided in other attributes)
			// since both of these attributes may or may not be present, we now select values by applying logic suggested by David Clunie
			// for more details, please refer to ticket #9031

			if (IsCrossSectionalModality(_frame))
			{
				// cross-sectional images use Pixel Spacing (0028,0030)
				SetValues(_frame.PixelSpacing, NormalizedPixelSpacingCalibrationType.CrossSectionalSpacing, String.Empty);
			}
			else if (_frame.PixelSpacing.IsNull || _frame.PixelSpacing.Equals(_frame.ImagerPixelSpacing))
			{
				// projection images using Imager Pixel Spacing (0018,1164) alone or with same value in Pixel Spacing (0028,0030)
				SetValues(_frame.ImagerPixelSpacing, NormalizedPixelSpacingCalibrationType.Detector, String.Empty);
			}
			else
			{
				// projection images using a calibrated value in Pixel Spacing (0028,0030)
				var calibrationMacro = new BasicPixelSpacingCalibrationMacro(_frame);
				SetValues(_frame.PixelSpacing, ConvertCalibrationType(calibrationMacro.PixelSpacingCalibrationType), calibrationMacro.PixelSpacingCalibrationDescription);
			}
		}

		private void SetValues(PixelSpacing pixelSpacing, NormalizedPixelSpacingCalibrationType calibrationType, string calibrationDetails)
		{
			Row = pixelSpacing.Row;
			Column = pixelSpacing.Column;
			CalibrationType = !pixelSpacing.IsNull ? calibrationType : NormalizedPixelSpacingCalibrationType.None;
			CalibrationDetails = !pixelSpacing.IsNull ? calibrationDetails : String.Empty;
		}

		private static NormalizedPixelSpacingCalibrationType ConvertCalibrationType(PixelSpacingCalibrationType calibrationType)
		{
			switch (calibrationType)
			{
				case PixelSpacingCalibrationType.Geometry:
					return NormalizedPixelSpacingCalibrationType.Geometry;
				case PixelSpacingCalibrationType.Fiducial:
					return NormalizedPixelSpacingCalibrationType.Fiducial;
				case PixelSpacingCalibrationType.None:
				default:
					return NormalizedPixelSpacingCalibrationType.Unknown;
			}
		}

		#region Aspect Ratio Formatting

		/// <summary>
		/// Gets the equivalent Pixel Aspect Ratio attribute value for the current pixel spacing.
		/// </summary>
		public string GetPixelAspectRatioString()
		{
			if (IsNull) return string.Empty;

			int height, width;
			FindFraction(AspectRatio, out height, out width);
			return string.Format(@"{0:D}\{1:D}", height, width);
		}

		/// <summary>
		/// Finds a fraction that approximates the given decimal value using a recursive method to compute a truncated continued fraction.
		/// </summary>
		private static void FindFraction(double value, out int numerator, out int denominator, double tolerance = 1e-6)
		{
			// check for degenerate case where decimal is 0
			if (FloatComparer.AreEqual(value, 0))
			{
				numerator = 0;
				denominator = 1;
				return;
			}

			// algorithm only works on positive numbers, so extract the sign and store it separately
			var sgn = Math.Sign(value);
			var val = Math.Abs(value);

			// check if decimal exceeds the biggest or smallest fractions we can represent with 32-bit integers
			if (val < 1d/int.MaxValue || val > 1d*int.MaxValue)
				throw new OverflowException();

			// rX is the decimal we're trying to approximate as a fraction
			var rX = val;

			// aX is the whole number part of that decimal
			var aX = (int) rX;

			// initial solution is to approximate the decimal as a whole number (denominator=1)
			denominator = 1;
			numerator = aX;

			// dY and dZ are the denominators from the previous two recursion levels
			var dZ = 0;
			var dY = 1;

			var loop = 0;
			do
			{
				// fX is fractional part of the current recursion level
				var fX = rX - aX;

				// if fX==0, then aX is a good approximation of rX, so the solution is complete
				if (FloatComparer.AreEqual(fX, 0, tolerance)) break;

				// otherwise, invert fX to get the new decimal number we're trying to approximate as a fraction (i.e. the next recursion level)
				rX = 1/fX;

				// and aX is the whole number part of that decimal
				aX = (int) rX;

				// floating point roundoff tends to make 1/whatever operations drift, so we fix rX if the error would make it round the wrong way
				if (FloatComparer.AreEqual(rX, aX + 1, tolerance)) rX = aX + 1;

				// compute the solution if we terminated now
				denominator = dY*aX + dZ;
				numerator = (int) Math.Round(val*denominator);

				// update the denominator history
				dZ = dY;
				dY = denominator;

				// check end condition that the current solution is sufficiently close to decimal we're trying to approximate
			} while (!FloatComparer.AreEqual(1d*numerator/denominator, val, tolerance) && ++loop < 1000);

			// if the original decimal was negative, put the sign back in
			if (sgn < 0) numerator = -numerator;
		}

#if UNIT_TESTS

		internal static void TestFindFraction(double v, out int num, out int den, double tolerance = 1e-6)
		{
			FindFraction(v, out num, out den, tolerance);
		}

#endif

		#endregion

		#region Cross Sectional SOP Classes

		private static bool IsCrossSectionalModality(Frame frame)
		{
			// Imager Pixel Spacing definitely does not apply to these modalities
			switch (frame.ParentImageSop.Modality)
			{
				case "CT":
				case "MR":
				case "PT":
				case "NM":
					return true;
			}

			// for safety reasons, we assume everything else might be projectional
			return _crossSectionalSopClasses.Contains(frame.ParentImageSop.SopClassUid);
		}

		private static readonly IList<string> _crossSectionalSopClasses = new List<string>
		                                                                  	{
		                                                                  		// classic cross sectional modalities and their multiframe versions
		                                                                  		SopClass.CtImageStorageUid,
		                                                                  		SopClass.EnhancedCtImageStorageUid,
		                                                                  		SopClass.MrImageStorageUid,
		                                                                  		SopClass.EnhancedMrImageStorageUid,
		                                                                  		SopClass.PositronEmissionTomographyImageStorageUid,
		                                                                  		SopClass.EnhancedPetImageStorageUid,
		                                                                  		SopClass.NuclearMedicineImageStorageUid,
		                                                                  		// for 3D X-Ray types, Imager Pixel Spacing applies to the acquisition sources, but not the reconstruction
		                                                                  		SopClass.BreastTomosynthesisImageStorageUid,
		                                                                  		SopClass.XRay3dAngiographicImageStorageUid,
		                                                                  		SopClass.XRay3dCraniofacialImageStorageUid
		                                                                  	}.AsReadOnly();

		#endregion
	}

	/// <summary>
	/// Enumerated values indicating the type of calibration represented by pixel spacing, if any.
	/// </summary>
	public enum NormalizedPixelSpacingCalibrationType
	{
		/// <summary>
		/// Indicates that the pixel spacing is not calibrated.
		/// </summary>
		None,

		/// <summary>
		/// Indicates that the pixel spacing has been calibrated manually by the user.
		/// </summary>
		Manual,

		/// <summary>
		/// Indicates that the pixel spacing has been calibrated in some unspecified manner.
		/// </summary>
		Unknown,

		/// <summary>
		/// Indicates that the pixel spacing represents the actual spacing in the patient for the cross-sectional image.
		/// </summary>
		CrossSectionalSpacing,

		/// <summary>
		/// Indicates that the pixel spacing represents the spacing at the detector plane for the projection image.
		/// </summary>
		Detector,

		/// <summary>
		/// Indicates that the pixel spacing has been calibrated for assumed or known
		/// geometric magnification effects at some unspecified depth within the patient for the projection image.
		/// </summary>
		Geometry,

		/// <summary>
		/// Indicates that the pixel spacing has been calibrated by measurement of an
		/// object (fiducial) of known size in the projection image.
		/// </summary>
		Fiducial,

		/// <summary>
		/// Indicates that the pixel spacing has been calibrated against the estimated
		/// radiographic magnification factor provided in the projection image.
		/// </summary>
		Magnified
	}
}