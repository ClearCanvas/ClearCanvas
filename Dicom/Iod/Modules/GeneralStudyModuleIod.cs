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
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// GeneralStudy Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.2.1 (Table C.7-3)</remarks>
	public class GeneralStudyModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneralStudyModuleIod"/> class.
		/// </summary>	
		public GeneralStudyModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneralStudyModuleIod"/> class.
		/// </summary>
		public GeneralStudyModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		/// <summary>
		/// Gets or sets the value of StudyInstanceUid in the underlying collection.
		/// </summary>
		public string StudyInstanceUid
		{
			get { return base.DicomAttributeProvider[DicomTags.StudyInstanceUid].GetString(0, string.Empty); }
			set { base.DicomAttributeProvider[DicomTags.StudyInstanceUid].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of StudyDate and StudyTime in the underlying collection.
		/// </summary>
		public DateTime? StudyDateTime
		{
			get
			{
				string date = base.DicomAttributeProvider[DicomTags.StudyDate].GetString(0, string.Empty);
				string time = base.DicomAttributeProvider[DicomTags.StudyTime].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				DicomAttribute date = base.DicomAttributeProvider[DicomTags.StudyDate];
				DicomAttribute time = base.DicomAttributeProvider[DicomTags.StudyTime];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of ReferringPhysiciansName in the underlying collection.
		/// </summary>
		public string ReferringPhysiciansName
		{
			get { return base.DicomAttributeProvider[DicomTags.ReferringPhysiciansName].GetString(0, string.Empty); }
			set { base.DicomAttributeProvider[DicomTags.ReferringPhysiciansName].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of ReferringPhysicianIdentificationSequence in the underlying collection.
		/// </summary>
		public PersonIdentificationMacro ReferringPhysicianIdentificationSequence
		{
			get
			{
				var dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferringPhysicianIdentificationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new PersonIdentificationMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.ReferringPhysicianIdentificationSequence] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ReferringPhysicianIdentificationSequence].Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Gets or sets the value of StudyId in the underlying collection.
		/// </summary>
		public string StudyId
		{
			get { return base.DicomAttributeProvider[DicomTags.StudyId].GetString(0, string.Empty); }
			set { base.DicomAttributeProvider[DicomTags.StudyId].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of AccessionNumber in the underlying collection.
		/// </summary>
		public string AccessionNumber
		{
			get { return base.DicomAttributeProvider[DicomTags.AccessionNumber].GetString(0, string.Empty); }
			set { base.DicomAttributeProvider[DicomTags.AccessionNumber].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of StudyDescription in the underlying collection.
		/// </summary>
		public string StudyDescription
		{
			get { return base.DicomAttributeProvider[DicomTags.StudyDescription].GetString(0, string.Empty); }
			set { base.DicomAttributeProvider[DicomTags.StudyDescription].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of PhysiciansOfRecord in the underlying collection.
		/// </summary>
		public string PhysiciansOfRecord
		{
			get { return base.DicomAttributeProvider[DicomTags.PhysiciansOfRecord].ToString(); }
			set { base.DicomAttributeProvider[DicomTags.PhysiciansOfRecord].SetStringValue(value); }
		}

		/// <summary>
		/// Gets or sets the value of PhysiciansOfRecordIdentificationSequence in the underlying collection.
		/// </summary>
		public PersonIdentificationMacro PhysiciansOfRecordIdentificationSequence
		{
			get
			{
				var dicomAttribute = base.DicomAttributeProvider[DicomTags.PhysiciansOfRecordIdentificationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new PersonIdentificationMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.PhysiciansOfRecordIdentificationSequence] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.PhysiciansOfRecordIdentificationSequence].Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Gets or sets the value of NameOfPhysiciansReadingStudy in the underlying collection.
		/// </summary>
		public string NameOfPhysiciansReadingStudy
		{
			get { return base.DicomAttributeProvider[DicomTags.NameOfPhysiciansReadingStudy].ToString(); }
			set { base.DicomAttributeProvider[DicomTags.NameOfPhysiciansReadingStudy].SetStringValue(value); }
		}

		/// <summary>
		/// Gets or sets the value of PhysiciansReadingStudyIdentificationSequence in the underlying collection.
		/// </summary>
		public PersonIdentificationMacro PhysiciansReadingStudyIdentificationSequence
		{
			get
			{
				var dicomAttribute = base.DicomAttributeProvider[DicomTags.PhysiciansReadingStudyIdentificationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new PersonIdentificationMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.PhysiciansReadingStudyIdentificationSequence] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.PhysiciansReadingStudyIdentificationSequence].Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Gets or sets the value of ReferencedStudySequence in the underlying collection.
		/// </summary>
		public ISopInstanceReferenceMacro ReferencedStudySequence
		{
			get
			{
				var dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedStudySequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new SopInstanceReferenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.ReferencedStudySequence] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ReferencedStudySequence].Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Gets or sets the value of ProcedureCodeSequence in the underlying collection.
		/// </summary>
		public CodeSequenceMacro ProcedureCodeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ProcedureCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new CodeSequenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.ProcedureCodeSequence] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ProcedureCodeSequence].Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			return !string.IsNullOrEmpty(AccessionNumber)
			       || !string.IsNullOrEmpty(NameOfPhysiciansReadingStudy)
			       || !string.IsNullOrEmpty(PhysiciansOfRecord)
			       || PhysiciansOfRecordIdentificationSequence != null
			       || PhysiciansReadingStudyIdentificationSequence != null
			       || ProcedureCodeSequence != null
			       || ReferencedStudySequence != null
			       || ReferringPhysicianIdentificationSequence != null
			       || !string.IsNullOrEmpty(ReferringPhysiciansName)
			       || StudyDateTime.HasValue
			       || !string.IsNullOrEmpty(StudyDescription)
			       || !string.IsNullOrEmpty(StudyId)
			       || !string.IsNullOrEmpty(StudyInstanceUid);
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.AccessionNumber;
				yield return DicomTags.NameOfPhysiciansReadingStudy;
				yield return DicomTags.PhysiciansOfRecord;
				yield return DicomTags.PhysiciansOfRecordIdentificationSequence;
				yield return DicomTags.PhysiciansReadingStudyIdentificationSequence;
				yield return DicomTags.ProcedureCodeSequence;
				yield return DicomTags.ReferencedStudySequence;
				yield return DicomTags.ReferringPhysicianIdentificationSequence;
				yield return DicomTags.ReferringPhysiciansName;
				yield return DicomTags.StudyDate;
				yield return DicomTags.StudyTime;
				yield return DicomTags.StudyDescription;
				yield return DicomTags.StudyId;
				yield return DicomTags.StudyInstanceUid;
			}
		}
	}
}