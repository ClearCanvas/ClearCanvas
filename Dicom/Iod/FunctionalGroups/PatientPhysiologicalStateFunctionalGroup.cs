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
	/// Patient Physiological State Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.20 (Table C.7.6.16-19)</remarks>
	public class PatientPhysiologicalStateFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PatientPhysiologicalStateFunctionalGroup"/> class.
		/// </summary>
		public PatientPhysiologicalStateFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PatientPhysiologicalStateFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PatientPhysiologicalStateFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.PatientPhysiologicalStateSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.PatientPhysiologicalStateCodeSequence; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of PatientPhysiologicalStateSequence in the underlying collection. Type 1.
		/// </summary>
		public PatientPhysiologicalStateSequenceItem PatientPhysiologicalStateSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PatientPhysiologicalStateSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new PatientPhysiologicalStateSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PatientPhysiologicalStateSequence];
				if (value == null)
				{
					const string msg = "PatientPhysiologicalStateSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PatientPhysiologicalStateSequence in the underlying collection. Type 1.
		/// </summary>
		public PatientPhysiologicalStateSequenceItem CreatePatientPhysiologicalStateSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PatientPhysiologicalStateSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new PatientPhysiologicalStateSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new PatientPhysiologicalStateSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Patient Physiological State Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.20 (Table C.7.6.16-21)</remarks>
	public class PatientPhysiologicalStateSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PatientPhysiologicalStateSequenceItem"/> class.
		/// </summary>
		public PatientPhysiologicalStateSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PatientPhysiologicalStateSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PatientPhysiologicalStateSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of PatientPhysiologicalStateCodeSequence in the underlying collection. Type 1.
		/// </summary>
		public CodeSequenceMacro PatientPhysiologicalStateCodeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PatientPhysiologicalStateCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new CodeSequenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PatientPhysiologicalStateCodeSequence];
				if (value == null)
				{
					const string msg = "PatientPhysiologicalStateCodeSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PatientPhysiologicalStateCodeSequence in the underlying collection. Type 1.
		/// </summary>
		public CodeSequenceMacro CreatePatientPhysiologicalStateCodeSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PatientPhysiologicalStateCodeSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new CodeSequenceMacro(dicomSequenceItem);
				return sequenceType;
			}
			return new CodeSequenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}
}