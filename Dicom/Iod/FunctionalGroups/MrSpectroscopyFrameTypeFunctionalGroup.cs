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

namespace ClearCanvas.Dicom.Iod.FunctionalGroups
{
	/// <summary>
	/// MR Spectroscopy Frame Type Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.14.3.1 (Table C.8-104)</remarks>
	public class MrSpectroscopyFrameTypeFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrSpectroscopyFrameTypeFunctionalGroup"/> class.
		/// </summary>
		public MrSpectroscopyFrameTypeFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrSpectroscopyFrameTypeFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrSpectroscopyFrameTypeFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.MrSpectroscopyFrameTypeSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.FrameType;
				yield return DicomTags.VolumetricProperties;
				yield return DicomTags.VolumeBasedCalculationTechnique;
				yield return DicomTags.ComplexImageComponent;
				yield return DicomTags.AcquisitionContrast;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of MrSpectroscopyFrameTypeSequence in the underlying collection. Type 1.
		/// </summary>
		public MrSpectroscopyFrameTypeSequenceItem MrSpectroscopyFrameTypeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrSpectroscopyFrameTypeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new MrSpectroscopyFrameTypeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrSpectroscopyFrameTypeSequence];
				if (value == null)
				{
					const string msg = "MrSpectroscopyFrameTypeSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the MrSpectroscopyFrameTypeSequence in the underlying collection. Type 1.
		/// </summary>
		public MrSpectroscopyFrameTypeSequenceItem CreateMrSpectroscopyFrameTypeSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.MrSpectroscopyFrameTypeSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new MrSpectroscopyFrameTypeSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new MrSpectroscopyFrameTypeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// MR Spectroscopy Frame Type Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.14.3.1 (Table C.8-104)</remarks>
	public class MrSpectroscopyFrameTypeSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrSpectroscopyFrameTypeSequenceItem"/> class.
		/// </summary>
		public MrSpectroscopyFrameTypeSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrSpectroscopyFrameTypeSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrSpectroscopyFrameTypeSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

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

		/// <summary>
		/// Gets or sets the value of ComplexImageComponent in the underlying collection. Type 1.
		/// </summary>
		public string ComplexImageComponent
		{
			get { return DicomAttributeProvider[DicomTags.ComplexImageComponent].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "ComplexImageComponent is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.ComplexImageComponent].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of AcquisitionContrast in the underlying collection. Type 1.
		/// </summary>
		public string AcquisitionContrast
		{
			get { return DicomAttributeProvider[DicomTags.AcquisitionContrast].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "AcquisitionContrast is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.AcquisitionContrast].SetString(0, value);
			}
		}
	}
}