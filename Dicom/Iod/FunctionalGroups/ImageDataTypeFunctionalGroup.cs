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
	/// Image Data Type Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.24 (Table C.7.6.16.2.24-1)</remarks>
	public class ImageDataTypeFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageDataTypeFunctionalGroup"/> class.
		/// </summary>
		public ImageDataTypeFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageDataTypeFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public ImageDataTypeFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.ImageDataTypeSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.DataType;
				yield return DicomTags.AliasedDataType;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of ImageDataTypeSequence in the underlying collection. Type 1.
		/// </summary>
		public ImageDataTypeSequenceItem ImageDataTypeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ImageDataTypeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new ImageDataTypeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ImageDataTypeSequence];
				if (value == null)
				{
					const string msg = "ImageDataTypeSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the ImageDataTypeSequence in the underlying collection. Type 1.
		/// </summary>
		public ImageDataTypeSequenceItem CreateImageDataTypeSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.ImageDataTypeSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new ImageDataTypeSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new ImageDataTypeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Image Data Type Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.24 (Table C.7.6.16.2.24-1)</remarks>
	public class ImageDataTypeSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageDataTypeSequenceItem"/> class.
		/// </summary>
		public ImageDataTypeSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageDataTypeSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public ImageDataTypeSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of DataType in the underlying collection. Type 1.
		/// </summary>
		public string DataType
		{
			get { return DicomAttributeProvider[DicomTags.DataType].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "DataType is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.DataType].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of AliasedDataType in the underlying collection. Type 1.
		/// </summary>
		public string AliasedDataType
		{
			get { return DicomAttributeProvider[DicomTags.AliasedDataType].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "AliasedDataType is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.AliasedDataType].SetString(0, value);
			}
		}
	}
}