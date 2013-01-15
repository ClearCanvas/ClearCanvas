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
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// KeyObjectDocumentSeries Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.17.6.1 (Table C.17.6-1)</remarks>
	public class KeyObjectDocumentSeriesModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="KeyObjectDocumentSeriesModuleIod"/> class.
		/// </summary>
		public KeyObjectDocumentSeriesModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="KeyObjectDocumentSeriesModuleIod"/> class.
		/// </summary>
		public KeyObjectDocumentSeriesModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		/// <summary>
		/// Initializes the underlying collection to implement the module using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			this.Modality = Modality.KO;
			this.SeriesInstanceUid = "1";
			this.SeriesNumber = 0;
			this.SeriesDateTime = null;
			this.SeriesDescription = null;
			this.ReferencedPerformedProcedureStepSequence = null;
		}

		/// <summary>
		/// Gets or sets the value of Modality in the underlying collection. Type 1.
		/// </summary>
		public Modality Modality
		{
			get { return ParseEnum(base.DicomAttributeProvider[DicomTags.Modality].GetString(0, string.Empty), Modality.None); }
			set
			{
				if (value != Modality.KO)
					throw new ArgumentOutOfRangeException("value", "Modality must be KO.");
				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.Modality], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SeriesInstanceUid in the underlying collection. Type 1.
		/// </summary>
		public string SeriesInstanceUid
		{
			get { return base.DicomAttributeProvider[DicomTags.SeriesInstanceUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "SeriesInstanceUid is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.SeriesInstanceUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SeriesNumber in the underlying collection. Type 1.
		/// </summary>
		public int SeriesNumber
		{
			get { return base.DicomAttributeProvider[DicomTags.SeriesNumber].GetInt32(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.SeriesNumber].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of SeriesDate and SeriesTime in the underlying collection. Type 3.
		/// </summary>
		public DateTime? SeriesDateTime
		{
			get
			{
				string date = base.DicomAttributeProvider[DicomTags.SeriesDate].GetString(0, string.Empty);
				string time = base.DicomAttributeProvider[DicomTags.SeriesTime].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				DicomAttribute date = base.DicomAttributeProvider[DicomTags.SeriesDate];
				DicomAttribute time = base.DicomAttributeProvider[DicomTags.SeriesTime];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of SeriesDescription in the underlying collection. Type 3.
		/// </summary>
		public string SeriesDescription
		{
			// Type 3
			get { return base.DicomAttributeProvider[DicomTags.SeriesDescription].GetString(0, string.Empty); }
			set { base.DicomAttributeProvider[DicomTags.SeriesDescription].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of ReferencedPerformedProcedureStepSequence in the underlying collection. Type 2.
		/// </summary>
		public ISopInstanceReferenceMacro ReferencedPerformedProcedureStepSequence
		{
			// Type 2
			get
			{
				DicomAttribute referencedPerformedProcedureStepSequence = base.DicomAttributeProvider[DicomTags.ReferencedPerformedProcedureStepSequence];
				if (referencedPerformedProcedureStepSequence.IsNull || referencedPerformedProcedureStepSequence.Count == 0)
				{
					return null;
				}
				return new SopInstanceReferenceMacro(((DicomSequenceItem[]) referencedPerformedProcedureStepSequence.Values)[0]);
			}
			set
			{
				DicomAttribute referencedPerformedProcedureStepSequence = base.DicomAttributeProvider[DicomTags.ReferencedPerformedProcedureStepSequence];
				if (value == null)
				{
					referencedPerformedProcedureStepSequence.SetNullValue();
					return;
				}
				referencedPerformedProcedureStepSequence.Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the ReferencedPerformedProcedureStepSequence in the underlying collection.
		/// </summary>
		public ISopInstanceReferenceMacro CreateReferencedPerformedProcedureStepSequence()
		{
			DicomAttribute referencedPerformedProcedureStepSequence = base.DicomAttributeProvider[DicomTags.ReferencedPerformedProcedureStepSequence];
			if (referencedPerformedProcedureStepSequence.IsNull || referencedPerformedProcedureStepSequence.Count == 0)
			{
				DicomSequenceItem dicomSequenceItem = new DicomSequenceItem();
				referencedPerformedProcedureStepSequence.Values = new DicomSequenceItem[] {dicomSequenceItem};
				SopInstanceReferenceMacro sopInstanceReference = new SopInstanceReferenceMacro(dicomSequenceItem);
				sopInstanceReference.InitializeAttributes();
				return sopInstanceReference;
			}
			return new SopInstanceReferenceMacro(((DicomSequenceItem[]) referencedPerformedProcedureStepSequence.Values)[0]);
		}
	}
}