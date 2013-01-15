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
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// PresentationStateMask Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.11.13 (Table C.11.13-1)</remarks>
	public class PresentationStateMaskModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PresentationStateMaskModuleIod"/> class.
		/// </summary>	
		public PresentationStateMaskModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentationStateMaskModuleIod"/> class.
		/// </summary>
		public PresentationStateMaskModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets or sets the value of MaskSubtractionSequence in the underlying collection. Type 1C.
		/// </summary>
		public MaskSubtractionSequenceIod MaskSubtractionSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MaskSubtractionSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new MaskSubtractionSequenceIod(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				if (value == null)
				{
					DicomAttributeProvider[DicomTags.MaskSubtractionSequence] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.MaskSubtractionSequence];
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the MaskSubtractionSequence in the underlying collection. Type 1C.
		/// </summary>
		public MaskSubtractionSequenceIod CreateMaskSubtractionSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.MaskSubtractionSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new MaskSubtractionSequenceIod(dicomSequenceItem);
				sequenceType.InitializeAttributes();
				return sequenceType;
			}
			return new MaskSubtractionSequenceIod(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}

		/// <summary>
		/// Gets or sets the value of RecommendedViewingMode in the underlying collection. Type 1C.
		/// </summary>
		public RecommendedViewingMode RecommendedViewingMode
		{
			get { return ParseEnum(DicomAttributeProvider[DicomTags.RecommendedViewingMode].GetString(0, string.Empty), RecommendedViewingMode.None); }
			set
			{
				if (value == RecommendedViewingMode.None)
				{
					DicomAttributeProvider[DicomTags.RecommendedViewingMode] = null;
					return;
				}
				SetAttributeFromEnum(DicomAttributeProvider[DicomTags.RecommendedViewingMode], value);
			}
		}

		public void InitializeAttributes()
		{
			MaskSubtractionSequence = null;
			RecommendedViewingMode = RecommendedViewingMode.None;
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.MaskSubtractionSequence;
				yield return DicomTags.RecommendedViewingMode;
			}
		}
	}
}