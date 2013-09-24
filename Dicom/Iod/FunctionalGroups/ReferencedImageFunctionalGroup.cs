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

using System.Collections.Generic;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.FunctionalGroups
{
	/// <summary>
	/// Referenced Image Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.5 (Table C.7.6.16-6)</remarks>
	public class ReferencedImageFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReferencedImageFunctionalGroup"/> class.
		/// </summary>
		public ReferencedImageFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReferencedImageFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public ReferencedImageFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.ReferencedImageSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				foreach (var t in ImageSopInstanceReferenceMacro.DefinedTags) yield return t;
				yield return DicomTags.PurposeOfReferenceCodeSequence;
			}
		}

		public override bool CanHaveMultipleItems
		{
			get { return true; }
		}

		public override void InitializeAttributes()
		{
			ReferencedImageSequence = null;
		}

		/// <summary>
		/// Gets or sets the value of ReferencedImageSequence in the underlying collection. Type 2.
		/// </summary>
		public ReferencedImageSequence[] ReferencedImageSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ReferencedImageSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}

				var result = new ReferencedImageSequence[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new ReferencedImageSequence(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.ReferencedImageSequence].SetNullValue();
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.ReferencedImageSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a ReferencedImageSequence item. Does not modify the ReferencedImageSequence in the underlying collection.
		/// </summary>
		public ReferencedImageSequence CreateReferencedImageSequenceItem()
		{
			var iodBase = new ReferencedImageSequence(new DicomSequenceItem());
			return iodBase;
		}
	}
}