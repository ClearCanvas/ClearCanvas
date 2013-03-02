#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// PatientOrientation Code Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.4.6 (Table C.8-5)</remarks>
	public class PatientOrientationCodeSequence : CodeSequenceMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PatientOrientationCodeSequence"/> class.
		/// </summary>
		public PatientOrientationCodeSequence() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PatientOrientationCodeSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PatientOrientationCodeSequence(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of PatientOrientationModifierCodeSequence in the underlying collection. Type 2C.
		/// </summary>
		public CodeSequenceMacro PatientOrientationModifierCodeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PatientOrientationModifierCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}
				return new CodeSequenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PatientOrientationModifierCodeSequence];
				if (value == null)
				{
					DicomAttributeProvider[DicomTags.PatientOrientationModifierCodeSequence] = null;
					return;
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PatientOrientationModifierCodeSequence in the underlying collection. Type 2C.
		/// </summary>
		public CodeSequenceMacro CreatePatientOrientationModifierCodeSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PatientOrientationModifierCodeSequence];
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