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
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// ScMultiFrameImage Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.6.3 (Table C.8-25b)</remarks>
	public class ScMultiFrameImageModuleIod : BasicPixelSpacingCalibrationMacro
	{
		private const string _enumYes = "YES";
		private const string _enumNo = "NO";

		/// <summary>
		/// Initializes a new instance of the <see cref="ScMultiFrameImageModuleIod"/> class.
		/// </summary>	
		public ScMultiFrameImageModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScMultiFrameImageModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public ScMultiFrameImageModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.BurnedInAnnotation;
				yield return DicomTags.RecognizableVisualFeatures;
				yield return DicomTags.PresentationLutShape;
				yield return DicomTags.Illumination;
				yield return DicomTags.ReflectedAmbientLight;
				yield return DicomTags.RescaleIntercept;
				yield return DicomTags.RescaleSlope;
				yield return DicomTags.RescaleType;
				yield return DicomTags.FrameIncrementPointer;
				yield return DicomTags.NominalScannedPixelSpacing;
				yield return DicomTags.PixelSpacing;
				yield return DicomTags.PixelSpacingCalibrationDescription;
				yield return DicomTags.PixelSpacingCalibrationType;
				yield return DicomTags.DigitizingDeviceTransportDirection;
				yield return DicomTags.RotationOfScannedFilm;
			}
		}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes() {}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			return !(IsNullOrEmpty(BurnedInAnnotation)
			         && IsNullOrEmpty(RecognizableVisualFeatures)
			         && IsNullOrEmpty(PresentationLutShape)
			         && IsNullOrEmpty(Illumination)
			         && IsNullOrEmpty(ReflectedAmbientLight)
			         && IsNullOrEmpty(RescaleIntercept)
			         && IsNullOrEmpty(RescaleSlope)
			         && IsNullOrEmpty(RescaleType)
			         && IsNullOrEmpty(FrameIncrementPointer)
			         && IsNullOrEmpty(NominalScannedPixelSpacing)
			         && IsNullOrEmpty(PixelSpacing)
			         && IsNullOrEmpty(PixelSpacingCalibrationDescription)
			         && IsNullOrEmpty(PixelSpacingCalibrationType)
			         && IsNullOrEmpty(DigitizingDeviceTransportDirection)
			         && IsNullOrEmpty(RotationOfScannedFilm));
		}

		/// <summary>
		/// Gets or sets the value of BurnedInAnnotation in the underlying collection. Type 1.
		/// </summary>
		public bool? BurnedInAnnotation
		{
			get { return ParseBool(DicomAttributeProvider[DicomTags.BurnedInAnnotation].GetString(0, string.Empty), _enumYes, _enumNo); }
			set
			{
				if (!value.HasValue)
				{
					const string msg = "BurnedInAnnotation is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				SetAttributeFromBool(DicomAttributeProvider[DicomTags.BurnedInAnnotation], value, _enumYes, _enumNo);
			}
		}

		/// <summary>
		/// Gets or sets the value of RecognizableVisualFeatures in the underlying collection. Type 3.
		/// </summary>
		public bool? RecognizableVisualFeatures
		{
			get { return ParseBool(DicomAttributeProvider[DicomTags.RecognizableVisualFeatures].GetString(0, string.Empty), _enumYes, _enumNo); }
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.RecognizableVisualFeatures] = null;
					return;
				}
				SetAttributeFromBool(DicomAttributeProvider[DicomTags.RecognizableVisualFeatures], value, _enumYes, _enumNo);
			}
		}

		/// <summary>
		/// Gets or sets the value of PresentationLutShape in the underlying collection. Type 1C.
		/// </summary>
		public PresentationLutShape PresentationLutShape
		{
			get { return ParseEnum(DicomAttributeProvider[DicomTags.PresentationLutShape].GetString(0, string.Empty), PresentationLutShape.None); }
			set
			{
				if (value == PresentationLutShape.None)
				{
					DicomAttributeProvider[DicomTags.PresentationLutShape] = null;
					return;
				}
				SetAttributeFromEnum(DicomAttributeProvider[DicomTags.PresentationLutShape], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of Illumination in the underlying collection. Type 1C.
		/// </summary>
		public int? Illumination
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.Illumination].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.Illumination] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.Illumination].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ReflectedAmbientLight in the underlying collection. Type 3.
		/// </summary>
		public int? ReflectedAmbientLight
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.ReflectedAmbientLight].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.ReflectedAmbientLight] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ReflectedAmbientLight].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RescaleIntercept in the underlying collection. Type 1C.
		/// </summary>
		public double? RescaleIntercept
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.RescaleIntercept].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.RescaleIntercept] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.RescaleIntercept].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RescaleSlope in the underlying collection. Type 1C.
		/// </summary>
		public double? RescaleSlope
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.RescaleSlope].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.RescaleSlope] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.RescaleSlope].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RescaleType in the underlying collection. Type 1C.
		/// </summary>
		public string RescaleType
		{
			get { return DicomAttributeProvider[DicomTags.RescaleType].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.RescaleType] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.RescaleType].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of FrameIncrementPointer in the underlying collection. Type 1C.
		/// </summary>
		public uint? FrameIncrementPointer
		{
			get
			{
				uint result;
				if (DicomAttributeProvider[DicomTags.FrameIncrementPointer].TryGetUInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.FrameIncrementPointer] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.FrameIncrementPointer].SetUInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of NominalScannedPixelSpacing in the underlying collection. Type 1C.
		/// </summary>
		public double[] NominalScannedPixelSpacing
		{
			get
			{
				var result = new double[2];
				if (DicomAttributeProvider[DicomTags.NominalScannedPixelSpacing].TryGetFloat64(0, out result[0])
				    && DicomAttributeProvider[DicomTags.NominalScannedPixelSpacing].TryGetFloat64(1, out result[1]))
					return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 2)
				{
					DicomAttributeProvider[DicomTags.NominalScannedPixelSpacing] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.NominalScannedPixelSpacing].SetFloat64(0, value[0]);
				DicomAttributeProvider[DicomTags.NominalScannedPixelSpacing].SetFloat64(1, value[1]);
			}
		}

		/// <summary>
		/// Gets or sets the value of DigitizingDeviceTransportDirection in the underlying collection. Type 3.
		/// </summary>
		public string DigitizingDeviceTransportDirection
		{
			get { return DicomAttributeProvider[DicomTags.DigitizingDeviceTransportDirection].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.DigitizingDeviceTransportDirection] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DigitizingDeviceTransportDirection].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RotationOfScannedFilm in the underlying collection. Type 3.
		/// </summary>
		public double? RotationOfScannedFilm
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.RotationOfScannedFilm].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.RotationOfScannedFilm] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.RotationOfScannedFilm].SetFloat64(0, value.Value);
			}
		}
	}
}