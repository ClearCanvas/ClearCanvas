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

namespace ClearCanvas.Dicom.Iod.FunctionalGroups
{
	/// <summary>
	/// MR Image Frame Type Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.1 (Table C.8-88)</remarks>
	public class MrImageFrameTypeFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrImageFrameTypeFunctionalGroup"/> class.
		/// </summary>
		public MrImageFrameTypeFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrImageFrameTypeFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrImageFrameTypeFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.MrImageFrameTypeSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.FrameType;
				foreach (var t in CommonCtMrImageDescriptionMacro.DefinedTags) yield return t;
				yield return DicomTags.ComplexImageComponent;
				yield return DicomTags.AcquisitionContrast;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of MrImageFrameTypeSequence in the underlying collection. Type 1.
		/// </summary>
		public MrImageFrameTypeSequenceItem MrImageFrameTypeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrImageFrameTypeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new MrImageFrameTypeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrImageFrameTypeSequence];
				if (value == null)
				{
					const string msg = "MrImageFrameTypeSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the MrImageFrameTypeSequence in the underlying collection. Type 1.
		/// </summary>
		public MrImageFrameTypeSequenceItem CreateMrImageFrameTypeSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.MrImageFrameTypeSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new MrImageFrameTypeSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new MrImageFrameTypeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// MR Image Frame Type Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.1 (Table C.8-88)</remarks>
	public class MrImageFrameTypeSequenceItem : CommonCtMrImageDescriptionMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrImageFrameTypeSequenceItem"/> class.
		/// </summary>
		public MrImageFrameTypeSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrImageFrameTypeSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrImageFrameTypeSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

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