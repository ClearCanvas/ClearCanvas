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
	/// Derivation Image Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.6 (Table C.7.6.16-7)</remarks>
	public class DerivationImageFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DerivationImageFunctionalGroup"/> class.
		/// </summary>
		public DerivationImageFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="DerivationImageFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public DerivationImageFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.DerivationImageSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.DerivationDescription;
				yield return DicomTags.DerivationCodeSequence;
				yield return DicomTags.SourceImageSequence;
			}
		}

		public override bool CanHaveMultipleItems
		{
			get { return true; }
		}

		public override void InitializeAttributes()
		{
			DerivationImageSequence = null;
		}

		/// <summary>
		/// Gets or sets the value of DerivationImageSequence in the underlying collection. Type 2.
		/// </summary>
		public DerivationImageSequenceItem[] DerivationImageSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.DerivationImageSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}

				var result = new DerivationImageSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new DerivationImageSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.DerivationImageSequence].SetNullValue();
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.DerivationImageSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a DerivationImageSequence item. Does not modify the DerivationImageSequence in the underlying collection.
		/// </summary>
		public DerivationImageSequenceItem CreateDerivationImageSequenceItem()
		{
			var iodBase = new DerivationImageSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}

	/// <summary>
	/// Derivation Image Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.6 (Table C.7.6.16-7)</remarks>
	public class DerivationImageSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DerivationImageSequenceItem"/> class.
		/// </summary>
		public DerivationImageSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="DerivationImageSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public DerivationImageSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of DerivationDescription in the underlying collection. Type 3.
		/// </summary>
		public string DerivationDescription
		{
			get { return DicomAttributeProvider[DicomTags.DerivationDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.DerivationDescription] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DerivationDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DerivationCodeSequence in the underlying collection. Type 1.
		/// </summary>
		public CodeSequenceMacro DerivationCodeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.DerivationCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new CodeSequenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.DerivationCodeSequence];
				if (value == null)
				{
					const string msg = "DerivationCodeSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the DerivationCodeSequence in the underlying collection. Type 1.
		/// </summary>
		public CodeSequenceMacro CreateDerivationCodeSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.DerivationCodeSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new CodeSequenceMacro(dicomSequenceItem);
				return sequenceType;
			}
			return new CodeSequenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}

		/// <summary>
		/// Gets or sets the value of SourceImageSequence in the underlying collection. Type 2.
		/// </summary>
		public ImageSopInstanceReferenceMacro[] SourceImageSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.SourceImageSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}

				var result = new ImageSopInstanceReferenceMacro[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new ImageSopInstanceReferenceMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.SourceImageSequence].SetNullValue();
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.SourceImageSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a SourceImageSequence item. Does not modify the SourceImageSequence in the underlying collection.
		/// </summary>
		public ImageSopInstanceReferenceMacro CreateSourceImageSequenceItem()
		{
			var iodBase = new ImageSopInstanceReferenceMacro(new DicomSequenceItem());
			return iodBase;
		}
	}
}