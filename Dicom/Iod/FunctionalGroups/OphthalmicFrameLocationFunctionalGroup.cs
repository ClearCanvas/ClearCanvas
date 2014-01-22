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
using System.Linq;
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.FunctionalGroups
{
	/// <summary>
	/// Ophthalmic Frame Location Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.17.10.1 (Table C.8.17.10-1)</remarks>
	public class OphthalmicFrameLocationFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OphthalmicFrameLocationFunctionalGroup"/> class.
		/// </summary>
		public OphthalmicFrameLocationFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="OphthalmicFrameLocationFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public OphthalmicFrameLocationFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.OphthalmicFrameLocationSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				return ImageSopInstanceReferenceMacro.DefinedTags
					.Concat(new[]
					        	{
					        		DicomTags.ReferenceCoordinates,
					        		DicomTags.DepthOfTransverseImage,
					        		DicomTags.OphthalmicImageOrientation
					        	});
			}
		}

		public override bool CanHaveMultipleItems
		{
			get { return true; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of OphthalmicFrameLocationSequence in the underlying collection. Type 1.
		/// </summary>
		public OphthalmicFrameLocationSequenceItem[] OphthalmicFrameLocationSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.OphthalmicFrameLocationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new OphthalmicFrameLocationSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new OphthalmicFrameLocationSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "OphthalmicFrameLocationSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.OphthalmicFrameLocationSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a OphthalmicFrameLocationSequence item. Does not modify the OphthalmicFrameLocationSequence in the underlying collection.
		/// </summary>
		public OphthalmicFrameLocationSequenceItem CreateOphthalmicFrameLocationSequenceItem()
		{
			var iodBase = new OphthalmicFrameLocationSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}

	/// <summary>
	/// Ophthalmic Frame Location Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.17.10.1 (Table C.8.17.10-1)</remarks>
	public class OphthalmicFrameLocationSequenceItem : ImageSopInstanceReferenceMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OphthalmicFrameLocationSequenceItem"/> class.
		/// </summary>
		public OphthalmicFrameLocationSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="OphthalmicFrameLocationSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public OphthalmicFrameLocationSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}