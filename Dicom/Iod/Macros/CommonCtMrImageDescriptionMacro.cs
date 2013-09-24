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

namespace ClearCanvas.Dicom.Iod.Macros
{
	/// <summary>
	/// CommonCtMrImageDescription Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.16.2 (Table C.8-131)</remarks>
	public class CommonCtMrImageDescriptionMacro : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommonCtMrImageDescriptionMacro"/> class.
		/// </summary>
		public CommonCtMrImageDescriptionMacro() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommonCtMrImageDescriptionMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CommonCtMrImageDescriptionMacro(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.PixelPresentation;
				yield return DicomTags.VolumetricProperties;
				yield return DicomTags.VolumeBasedCalculationTechnique;
			}
		}

		/// <summary>
		/// Gets or sets the value of PixelPresentation in the underlying collection. Type 1.
		/// </summary>
		public string PixelPresentation
		{
			get { return DicomAttributeProvider[DicomTags.PixelPresentation].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "PixelPresentation is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.PixelPresentation].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of VolumetricProperties in the underlying collection. Type 1.
		/// </summary>
		public string VolumetricProperties
		{
			get { return DicomAttributeProvider[DicomTags.VolumetricProperties].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "VolumetricProperties is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.VolumetricProperties].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of VolumeBasedCalculationTechnique in the underlying collection. Type 1.
		/// </summary>
		public string VolumeBasedCalculationTechnique
		{
			get { return DicomAttributeProvider[DicomTags.VolumeBasedCalculationTechnique].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "VolumeBasedCalculationTechnique is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.VolumeBasedCalculationTechnique].SetString(0, value);
			}
		}
	}
}