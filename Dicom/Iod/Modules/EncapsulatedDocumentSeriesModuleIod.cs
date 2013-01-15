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
using ClearCanvas.Dicom.Iod.Macros.PerformedProcedureStepSummary;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// EncapsulatedDocumentSeries Module
	/// </summary>
	/// <remarks>
	/// <para>As defined in the DICOM Standard 2009, Part 3, Section C.24.1 (Table C.24-1)</para>
	/// </remarks>
	public class EncapsulatedDocumentSeriesModuleIod
		: IodBase, IPerformedProcedureStepSummaryMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EncapsulatedDocumentSeriesModuleIod"/> class.
		/// </summary>
		public EncapsulatedDocumentSeriesModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="EncapsulatedDocumentSeriesModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute provider.</param>
		public EncapsulatedDocumentSeriesModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets or sets the value of Modality in the underlying collection. Type 1.
		/// </summary>
		public string Modality
		{
			get { return DicomAttributeProvider[DicomTags.Modality].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "Modality is Type 1 Required.");
				DicomAttributeProvider[DicomTags.Modality].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SeriesInstanceUid in the underlying collection. Type 1.
		/// </summary>
		public string SeriesInstanceUid
		{
			get { return DicomAttributeProvider[DicomTags.SeriesInstanceUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "SeriesInstanceUid is Type 1 Required.");
				DicomAttributeProvider[DicomTags.SeriesInstanceUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SeriesNumber in the underlying collection. Type 1.
		/// </summary>
		public int SeriesNumber
		{
			get { return DicomAttributeProvider[DicomTags.SeriesNumber].GetInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.SeriesNumber].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of ReferencedPerformedProcedureStepSequence in the underlying collection. Type 3.
		/// </summary>
		public ISopInstanceReferenceMacro ReferencedPerformedProcedureStepSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ReferencedPerformedProcedureStepSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new SopInstanceReferenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				if (value == null)
				{
					DicomAttributeProvider[DicomTags.ReferencedPerformedProcedureStepSequence] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.ReferencedPerformedProcedureStepSequence];
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the ReferencedPerformedProcedureStepSequence in the underlying collection. Type 3.
		/// </summary>
		public ISopInstanceReferenceMacro CreateReferencedPerformedProcedureStepSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.ReferencedPerformedProcedureStepSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new SopInstanceReferenceMacro(dicomSequenceItem);
				sequenceType.InitializeAttributes();
				return sequenceType;
			}
			return new SopInstanceReferenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}

		/// <summary>
		/// Gets or sets the value of SeriesDescription in the underlying collection. Type 3.
		/// </summary>
		public string SeriesDescription
		{
			get { return DicomAttributeProvider[DicomTags.SeriesDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.SeriesDescription] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SeriesDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SeriesDescriptionCodeSequence in the underlying collection. Type 3.
		/// </summary>
		public CodeSequenceMacro SeriesDescriptionCodeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.SeriesDescriptionCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new CodeSequenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				if (value == null)
				{
					DicomAttributeProvider[DicomTags.SeriesDescriptionCodeSequence] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.SeriesDescriptionCodeSequence];
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the SeriesDescriptionCodeSequence in the underlying collection. Type 3.
		/// </summary>
		public CodeSequenceMacro CreateSeriesDescriptionCodeSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.SeriesDescriptionCodeSequence];
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
		/// Gets or sets the value of RequestAttributesSequence in the underlying collection. Type 3.
		/// </summary>
		public IRequestAttributesMacro RequestAttributesSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.RequestAttributesSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new RequestAttributesMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				if (value == null)
				{
					DicomAttributeProvider[DicomTags.RequestAttributesSequence] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.RequestAttributesSequence];
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the RequestAttributesSequence in the underlying collection. Type 3.
		/// </summary>
		public IRequestAttributesMacro CreateRequestAttributesSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.RequestAttributesSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new RequestAttributesMacro(dicomSequenceItem);
				sequenceType.InitializeAttributes();
				return sequenceType;
			}
			return new RequestAttributesMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}

		/// <summary>
		/// Initializes the attributes in this module to their default values.
		/// </summary>
		public void InitializeAttributes()
		{
			Modality = @"DOC";
			SeriesInstanceUid = DicomUid.GenerateUid().UID;
			SeriesNumber = 0;
			PerformedProcedureStepSummaryMacro.InitializeAttributes();
		}

		/// <summary>
		/// Gets an enumeration of <see cref="ClearCanvas.Dicom.DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.Modality;
				yield return DicomTags.SeriesInstanceUid;
				yield return DicomTags.SeriesNumber;
				yield return DicomTags.ReferencedPerformedProcedureStepSequence;
				yield return DicomTags.SeriesDescription;
				yield return DicomTags.SeriesDescriptionCodeSequence;
				yield return DicomTags.RequestAttributesSequence;

				foreach (var tag in PerformedProcedureStepSummaryMacro.DefinedTags)
					yield return tag;
			}
		}

		#region IPerformedProcedureStepSummaryMacro Members

		private PerformedProcedureStepSummaryMacro PerformedProcedureStepSummaryMacro
		{
			get { return new PerformedProcedureStepSummaryMacro(DicomAttributeProvider); }
		}

		/// <summary>
		/// Not all macros are sequence items.
		/// </summary>
		DicomSequenceItem IIodMacro.DicomSequenceItem
		{
			get { return DicomAttributeProvider as DicomSequenceItem; }
			set { DicomAttributeProvider = value; }
		}

		public string PerformedProcedureStepId
		{
			get { return PerformedProcedureStepSummaryMacro.PerformedProcedureStepId; }
			set { PerformedProcedureStepSummaryMacro.PerformedProcedureStepId = value; }
		}

		public DateTime? PerformedProcedureStepStartDateTime
		{
			get { return PerformedProcedureStepSummaryMacro.PerformedProcedureStepStartDateTime; }
			set { PerformedProcedureStepSummaryMacro.PerformedProcedureStepStartDateTime = value; }
		}

		public string PerformedProcedureStepDescription
		{
			get { return PerformedProcedureStepSummaryMacro.PerformedProcedureStepDescription; }
			set { PerformedProcedureStepSummaryMacro.PerformedProcedureStepDescription = value; }
		}

		public IPerformedProtocolCodeSequence[] PerformedProtocolCodeSequence
		{
			get { return PerformedProcedureStepSummaryMacro.PerformedProtocolCodeSequence; }
			set { PerformedProcedureStepSummaryMacro.PerformedProtocolCodeSequence = value; }
		}

		public string CommentsOnThePerformedProcedureStep
		{
			get { return PerformedProcedureStepSummaryMacro.CommentsOnThePerformedProcedureStep; }
			set { PerformedProcedureStepSummaryMacro.CommentsOnThePerformedProcedureStep = value; }
		}

		public IPerformedProtocolCodeSequence CreatePerformedProtocolCodeSequence()
		{
			return PerformedProcedureStepSummaryMacro.CreatePerformedProtocolCodeSequence();
		}

		#endregion
	}
}