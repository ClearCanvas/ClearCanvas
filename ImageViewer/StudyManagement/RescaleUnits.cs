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
using System.Diagnostics;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Represents the units of the output of the rescaling function (modality LUT) in a DICOM image.
	/// </summary>
	public sealed class RescaleUnits : IEquatable<RescaleUnits>
	{
		private static readonly Dictionary<string, RescaleUnits> _index = new Dictionary<string, RescaleUnits>();

		/// <summary>
		/// Indicates that values are unitless.
		/// </summary>
		public static readonly RescaleUnits None = new RescaleUnits(string.Empty, @"NONE", string.Empty, string.Empty);

		/// <summary>
		/// Indicates that the units of the values are unknown.
		/// </summary>
		public static readonly RescaleUnits Unspecified = new RescaleUnits(@"US", string.Empty, ImageViewer.SR.LabelUnitsUnspecified, ImageViewer.SR.DescriptionUnitsUnspecified);

		/// <summary>
		/// Gets the units of the output of the rescaling function (modality LUT) in the specified DICOM data set.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM data set.</param>
		/// <returns>A <see cref="RescaleUnits"/> instance representing the units of the output of the rescaling function.</returns>
		public static RescaleUnits GetRescaleUnits(IDicomAttributeProvider dicomAttributeProvider)
		{
			var modality = GetAttributeStringValue(dicomAttributeProvider, DicomTags.Modality);
			var rescaleType = GetAttributeStringValue(dicomAttributeProvider, DicomTags.RescaleType);
			switch (modality)
			{
				case @"PT":
					// use Units (0054,1001) if specified
					var units = GetAttributeStringValue(dicomAttributeProvider, DicomTags.Units);
					if (!string.IsNullOrEmpty(units))
						return ParsePetSeriesUnits(units);

					// otherwise use Rescale Type (0028,1054) - if empty, return unspecified
					return !string.IsNullOrEmpty(rescaleType) ? ParseRescaleType(rescaleType) : Unspecified;
				case @"CT":
					// use Rescale Type (0028,1054) - if empty, assume HU (see DICOM PS3.3)
					return !string.IsNullOrEmpty(rescaleType) ? ParseRescaleType(rescaleType) : HounsfieldUnits;
				default:
					// use Rescale Type (0028,1054) if specified
					if (!string.IsNullOrEmpty(rescaleType))
						return ParseRescaleType(rescaleType);

					// otherwise if slope/intercept specify anything other than the identity function, return unspecified
					var slope = GetAttributeDoubleValue(dicomAttributeProvider, DicomTags.RescaleSlope, 1);
					var intercept = GetAttributeDoubleValue(dicomAttributeProvider, DicomTags.RescaleIntercept, 0);

// ReSharper disable CompareOfFloatsByEqualityOperator
					return slope == 1 && intercept == 0 ? None : Unspecified;
// ReSharper restore CompareOfFloatsByEqualityOperator
			}
		}

		private static string GetAttributeStringValue(IDicomAttributeProvider dicomAttributeProvider, uint tag)
		{
			DicomAttribute attribute;
			if (dicomAttributeProvider.TryGetAttribute(tag, out attribute))
				return attribute.ToString().Trim('\\', ' ');
			return null;
		}

		private static double GetAttributeDoubleValue(IDicomAttributeProvider dicomAttributeProvider, uint tag, double defaultValue)
		{
			DicomAttribute attribute;
			if (dicomAttributeProvider.TryGetAttribute(tag, out attribute))
				return attribute.GetFloat64(0, defaultValue);
			return defaultValue;
		}

		#region Rescale Type (0028,1054)

		/// <summary>
		/// Indicates that the values are in Hounsfield Units (HU).
		/// </summary>
		public static readonly RescaleUnits HounsfieldUnits = new RescaleUnits(@"HU", null, ImageViewer.SR.LabelUnitsHU, ImageViewer.SR.DescriptionUnitsHU);

		/// <summary>
		/// Indicates that the values are in 1/1000ths of optical density (i.e. 2140 represents an optical density of 2.140).
		/// </summary>
		public static readonly RescaleUnits OpticalDensity = new RescaleUnits(@"OD", null, ImageViewer.SR.LabelUnitsOD, ImageViewer.SR.DescriptionUnitsOD);

		/// <summary>
		/// Parses a <see cref="RescaleUnits"/> from the value of a Rescale Type (0028,1054) attribute.
		/// </summary>
		/// <param name="rescaleType">The rescale type.</param>
		/// <returns>A <see cref="RescaleUnits"/> instance representing the rescale type, or <see cref="None"/> if the value is empty.</returns>
		public static RescaleUnits ParseRescaleType(string rescaleType)
		{
			if (string.IsNullOrEmpty(rescaleType))
				return None;

			RescaleUnits value;
			if (_index.TryGetValue(rescaleType.ToUpperInvariant(), out value) && value._codeRescaleType != null)
				return value;
			return new RescaleUnits(rescaleType, null);
		}

		/// <summary>
		/// Formats the <see cref="RescaleUnits"/> as a string for the Rescale Type (0028,1054) attribute.
		/// </summary>
		/// <returns>A rescale type string value.</returns>
		public string ToRescaleType()
		{
			if (_codeRescaleType == null)
				throw new InvalidOperationException(string.Format(@"The unit {0} cannot be represented as a Rescale Type (0028,1054)", Label));
			return _codeRescaleType;
		}

		#endregion

		#region Units (0054,1001)

		/// <summary>
		/// Indicates that the values are in square centimetres.
		/// </summary>
		public static readonly RescaleUnits Cm2 = new RescaleUnits(null, @"CM2", ImageViewer.SR.LabelUnitsCm2, ImageViewer.SR.DescriptionUnitsCm2);

		/// <summary>
		/// Indicates that the values are in square centimetres per millilitre.
		/// </summary>
		public static readonly RescaleUnits Cm2PerMl = new RescaleUnits(null, @"CM2ML", ImageViewer.SR.LabelUnitsCm2ml, ImageViewer.SR.DescriptionUnitsCm2ml);

		/// <summary>
		/// Indicates that the values are in percent.
		/// </summary>
		public static readonly RescaleUnits Percent = new RescaleUnits(null, @"PCNT", ImageViewer.SR.LabelUnitsPcnt, ImageViewer.SR.DescriptionUnitsPcnt);

		/// <summary>
		/// Indicates that the values are in counts per second.
		/// </summary>
		public static readonly RescaleUnits CountsPerSec = new RescaleUnits(null, @"CPS", ImageViewer.SR.LabelUnitsCps, ImageViewer.SR.DescriptionUnitsCps);

		/// <summary>
		/// Indicates that the values are in counts.
		/// </summary>
		public static readonly RescaleUnits Counts = new RescaleUnits(null, @"CNTS", ImageViewer.SR.LabelUnitsCnts, ImageViewer.SR.DescriptionUnitsCnts);

		/// <summary>
		/// Indicates that the values are in becquerels per millilitre.
		/// </summary>
		public static readonly RescaleUnits BecquerelsPerMl = new RescaleUnits(null, @"BQML", ImageViewer.SR.LabelUnitsBqml, ImageViewer.SR.DescriptionUnitsBqml);

		/// <summary>
		/// Indicates that the values are in milligrams per minute per millilitre.
		/// </summary>
		public static readonly RescaleUnits MgPerMinPerMl = new RescaleUnits(null, @"MGMINML", ImageViewer.SR.LabelUnitsMgminml, ImageViewer.SR.DescriptionUnitsMgminml);

		/// <summary>
		/// Indicates that the values are in micromoles per minute per millilitre.
		/// </summary>
		public static readonly RescaleUnits UmolPerMinPerMl = new RescaleUnits(null, @"UMOLMINML", ImageViewer.SR.LabelUnitsUmolminml, ImageViewer.SR.DescriptionUnitsUmolminml);

		/// <summary>
		/// Indicates that the values are in millilitres per minute per gram.
		/// </summary>
		public static readonly RescaleUnits MlPerMinPerG = new RescaleUnits(null, @"MLMING", ImageViewer.SR.LabelUnitsMlming, ImageViewer.SR.DescriptionUnitsMlming);

		/// <summary>
		/// Indicates that the values are in millilitres per gram.
		/// </summary>
		public static readonly RescaleUnits MlPerG = new RescaleUnits(null, @"MLG", ImageViewer.SR.LabelUnitsMlg, ImageViewer.SR.DescriptionUnitsMlg);

		/// <summary>
		/// Indicates that the values are in units per centimetre.
		/// </summary>
		public static readonly RescaleUnits PerCm = new RescaleUnits(null, @"1CM", ImageViewer.SR.LabelUnits1cm, ImageViewer.SR.DescriptionUnits1cm);

		/// <summary>
		/// Indicates that the values are in micromoles per millilitre.
		/// </summary>
		public static readonly RescaleUnits UmolPerMl = new RescaleUnits(null, @"UMOLML", ImageViewer.SR.LabelUnitsUmolml, ImageViewer.SR.DescriptionUnitsUmolml);

		/// <summary>
		/// Indicates that the values are proportional to counts.
		/// </summary>
		public static readonly RescaleUnits PropCounts = new RescaleUnits(null, @"PROPCNTS", ImageViewer.SR.LabelUnitsPropcnts, ImageViewer.SR.DescriptionUnitsPropcnts);

		/// <summary>
		/// Indicates that the values are proportional to counts per second.
		/// </summary>
		public static readonly RescaleUnits PropCountsPerSec = new RescaleUnits(null, @"PROPCPS", ImageViewer.SR.LabelUnitsPropcps, ImageViewer.SR.DescriptionUnitsPropcps);

		/// <summary>
		/// Indicates that the values are in millilitres per minute per millilitre.
		/// </summary>
		public static readonly RescaleUnits MlPerMinPerMl = new RescaleUnits(null, @"MLMINML", ImageViewer.SR.LabelUnitsMlminml, ImageViewer.SR.DescriptionUnitsMlminml);

		/// <summary>
		/// Indicates that the values are in millilitres per millilitre.
		/// </summary>
		public static readonly RescaleUnits MlPerMl = new RescaleUnits(null, @"MLML", ImageViewer.SR.LabelUnitsMlml, ImageViewer.SR.DescriptionUnitsMlml);

		/// <summary>
		/// Indicates that the values are in grams per millilitre.
		/// </summary>
		public static readonly RescaleUnits GPerMl = new RescaleUnits(null, @"GML", ImageViewer.SR.LabelUnitsGml, ImageViewer.SR.DescriptionUnitsGml);

		/// <summary>
		/// Indicates that the values are in standard deviations.
		/// </summary>
		public static readonly RescaleUnits StdDev = new RescaleUnits(null, @"STDDEV", ImageViewer.SR.LabelUnitsStddev, ImageViewer.SR.DescriptionUnitsStddev);

		/// <summary>
		/// Parses a <see cref="RescaleUnits"/> from the value of a Units (0054,1001) attribute in a PET Series Module.
		/// </summary>
		/// <param name="units">The PET series units.</param>
		/// <returns>A <see cref="RescaleUnits"/> instance representing the PET series units type, or <see cref="Unspecified"/> if the value is empty.</returns>
		public static RescaleUnits ParsePetSeriesUnits(string units)
		{
			if (string.IsNullOrEmpty(units))
				return Unspecified;

			RescaleUnits value;
			if (_index.TryGetValue(units.ToUpperInvariant(), out value) && value._codeUnits != null)
				return value;
			return new RescaleUnits(null, units);
		}

		/// <summary>
		/// Formats the <see cref="RescaleUnits"/> as a string for the Units (0054,1001) attribute in the PET Series Module.
		/// </summary>
		/// <returns>A PET series units string value.</returns>
		public string ToPetSeriesUnits()
		{
			if (_codeUnits == null)
				throw new InvalidOperationException(string.Format(@"The unit {0} cannot be represented as a Units (0054,1001)", Label));
			return _codeUnits;
		}

		#endregion

		private readonly string _codeRescaleType;
		private readonly string _codeUnits;
		private readonly string _label;
		private readonly string _description;

		/// <summary>
		/// Constructor for units not specified by DICOM.
		/// </summary>
		/// <param name="codeRescaleType"></param>
		/// <param name="codeUnits"></param>
		private RescaleUnits(string codeRescaleType, string codeUnits)
		{
			Debug.Assert(codeRescaleType != null || codeUnits != null, "At least one of codeRescaleType or codeUnits must be set");

			_codeRescaleType = codeRescaleType;
			_codeUnits = codeUnits;
			_label = _description = (codeRescaleType ?? codeUnits);
		}

		/// <summary>
		/// Constructor for DICOM-defined units.
		/// </summary>
		/// <param name="codeRescaleType"></param>
		/// <param name="codeUnits"></param>
		/// <param name="label"></param>
		/// <param name="description"></param>
		private RescaleUnits(string codeRescaleType, string codeUnits, string label, string description)
		{
			Debug.Assert(codeRescaleType != null || codeUnits != null, "At least one of codeRescaleType or codeUnits must be set");

			_codeRescaleType = codeRescaleType;
			_codeUnits = codeUnits;
			_label = label;
			_description = description;

			if (!string.IsNullOrEmpty(_codeRescaleType)) _index.Add(_codeRescaleType, this);
			if (!string.IsNullOrEmpty(_codeUnits)) _index.Add(_codeUnits, this);
		}

		/// <summary>
		/// Gets the code string for the unit.
		/// </summary>
		public string Code
		{
			get { return _codeRescaleType ?? _codeUnits; }
		}

		/// <summary>
		/// Gets a label for the unit suitable for usage when formatting a value.
		/// </summary>
		public string Label
		{
			get { return _label; }
		}

		/// <summary>
		/// Gets a description of the unit's meaning.
		/// </summary>
		public string Description
		{
			get { return _description; }
		}

		public override int GetHashCode()
		{
			return -0x36116875 ^ (_codeRescaleType != null ? _codeRescaleType.GetHashCode() : 0) ^ (_codeUnits != null ? _codeUnits.GetHashCode() : 0);
		}

		public bool Equals(RescaleUnits other)
		{
			return _codeRescaleType == other._codeRescaleType && _codeUnits == other._codeUnits;
		}

		public override bool Equals(object obj)
		{
			return obj is RescaleUnits && Equals((RescaleUnits) obj);
		}

		public override string ToString()
		{
			return Label;
		}

		public static bool operator ==(RescaleUnits x, RescaleUnits y)
		{
			return Equals(x, y);
		}

		public static bool operator !=(RescaleUnits x, RescaleUnits y)
		{
			return !Equals(x, y);
		}

		public static explicit operator string(RescaleUnits units)
		{
			return units != null ? (!string.IsNullOrEmpty(units._codeRescaleType) ? units._codeRescaleType : units._codeUnits ?? string.Empty) : string.Empty;
		}
	}
}