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
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// As per Dicom DOC 3 C.4.15 (pg 256)
	/// </summary>
	public class ImageAcquisitionResultsModuleIod : IodBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageAcquisitionResultsModuleIod"/> class.
		/// </summary>
		public ImageAcquisitionResultsModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageAcquisitionResultsModuleIod"/> class.
		/// </summary>
		public ImageAcquisitionResultsModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		#endregion

		#region Public Properties

		public Modality Modality
		{
			get { return ParseEnum<Modality>(base.DicomAttributeProvider[DicomTags.Modality].GetString(0, String.Empty), Modality.None); }
			set { SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.Modality], value); }
		}

		public string StudyId
		{
			get { return base.DicomAttributeProvider[DicomTags.StudyId].GetString(0, String.Empty); }
			set { base.DicomAttributeProvider[DicomTags.StudyId].SetString(0, value); }
		}

		/// <summary>
		/// Gets the performed protocol code sequence list.
		/// Sequence describing the Protocol performed for this Procedure Step. This sequence 
		/// may have zero or more Items.
		/// </summary>
		/// <value>The performed protocol code sequence list.</value>
		public SequenceIodList<CodeSequenceMacro> PerformedProtocolCodeSequenceList
		{
			get { return new SequenceIodList<CodeSequenceMacro>(base.DicomAttributeProvider[DicomTags.PerformedProtocolCodeSequence] as DicomAttributeSQ); }
		}

		/// <summary>
		/// Gets the protocol context sequence list.
		/// Sequence that specifies the context for the Performed Protocol Code Sequence Item. 
		/// One or more items may be included in this sequence. See Section C.4.10.1.
		/// </summary>
		/// <value>The protocol context sequence list.</value>
		public SequenceIodList<ContentItemMacro> ProtocolContextSequenceList
		{
			get { return new SequenceIodList<ContentItemMacro>(base.DicomAttributeProvider[DicomTags.ProtocolContextSequence] as DicomAttributeSQ); }
		}

		/// <summary>
		/// Sequence that specifies modifiers for a Protocol Context Content Item. One or 
		/// more items may be included in this sequence. See Section C.4.10.1.
		/// </summary>
		/// <value>The content item modifier sequence list.</value>
		public SequenceIodList<ContentItemMacro> ContentItemModifierSequenceList
		{
			get { return new SequenceIodList<ContentItemMacro>(base.DicomAttributeProvider[DicomTags.ContentItemModifierSequence] as DicomAttributeSQ); }
		}

		public SequenceIodList<PerformedSeriesSequenceIod> PerformedSeriesSequenceList
		{
			get { return new SequenceIodList<PerformedSeriesSequenceIod>(base.DicomAttributeProvider[DicomTags.PerformedSeriesSequence] as DicomAttributeSQ); }
		}

		#endregion
	}
}