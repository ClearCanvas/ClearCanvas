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

namespace ClearCanvas.Dicom.Iod.Macros
{
	/// <summary>
	/// Image SOP Instance Reference Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section 10.3 (Table 10-3)</remarks>
	public class ImageSopInstanceReferenceMacro : SequenceIodBase, ISopInstanceReferenceMacro
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageSopInstanceReferenceMacro"/> class.
		/// </summary>
		public ImageSopInstanceReferenceMacro() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageSopInstanceReferenceMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public ImageSopInstanceReferenceMacro(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		#endregion

		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.ReferencedSopClassUid;
				yield return DicomTags.ReferencedSopInstanceUid;
				yield return DicomTags.ReferencedFrameNumber;
				yield return DicomTags.ReferencedSegmentNumber;
			}
		}

		void IIodMacro.InitializeAttributes() {}

		#region Public Properties

		/// <summary>
		/// Uniquely identifies the referenced SOP Class
		/// </summary>
		/// <value>The referenced sop class uid.</value>
		public string ReferencedSopClassUid
		{
			get { return base.DicomAttributeProvider[DicomTags.ReferencedSopClassUid].GetString(0, String.Empty); }
			set { base.DicomAttributeProvider[DicomTags.ReferencedSopClassUid].SetString(0, value); }
		}

		/// <summary>
		/// Uniquely identifies the referenced SOP Instance.
		/// </summary>
		/// <value>The referenced sop instance uid.</value>
		public string ReferencedSopInstanceUid
		{
			get { return base.DicomAttributeProvider[DicomTags.ReferencedSopInstanceUid].GetString(0, String.Empty); }
			set { base.DicomAttributeProvider[DicomTags.ReferencedSopInstanceUid].SetString(0, value); }
		}

		/// <summary>
		/// Identifies the frame numbers within the Referenced SOP Instance to which the 
		/// reference applies. The first frame shall be denoted as frame number 1. 
		/// <para>Note: This Attribute may be multi-valued. </para> 
		/// <para>
		/// Required if the Referenced SOP Instance is a multi-frame image and the reference 
		/// does not apply to all frames, and Referenced Segment Number (0062,000B) is not present.
		/// </para> 
		/// </summary>
		/// <value>The referenced frame number.</value>
		public DicomAttributeIS ReferencedFrameNumber
		{
			get { return base.DicomAttributeProvider[DicomTags.ReferencedFrameNumber] as DicomAttributeIS; }
		}

		/// <summary>
		/// Gets or sets the value of ReferencedFrameNumber in the underlying collection. Type 1C.
		/// </summary>
		public int[] ReferencedFrameNumber2
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ReferencedFrameNumber];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var values = new int[dicomAttribute.Count];
				for (int n = 0; n < values.Length; n++)
					values[n] = dicomAttribute.GetInt32(n, 0);
				return values;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.ReferencedFrameNumber] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.ReferencedFrameNumber];
				for (int n = 0; n < value.Length; n++)
					dicomAttribute.SetInt32(n, value[n]);
			}
		}

		/// <summary>
		/// Identifies the Segment Number to which the reference applies. Required if the Referenced
		///  SOP Instance is a Segmentation and the reference does not apply to all segments and
		///  Referenced Frame Number (0008,1160) is not present.
		/// </summary>
		/// <value>The referenced segment number.</value>
		public DicomAttributeUS ReferencedSegmentNumber
		{
			get { return base.DicomAttributeProvider[DicomTags.ReferencedSegmentNumber] as DicomAttributeUS; }
		}

		/// <summary>
		/// Gets or sets the value of ReferencedSegmentNumber in the underlying collection. Type 1C.
		/// </summary>
		public int[] ReferencedSegmentNumber2
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ReferencedSegmentNumber];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var values = new int[dicomAttribute.Count];
				for (int n = 0; n < values.Length; n++)
					values[n] = dicomAttribute.GetInt32(n, 0);
				return values;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.ReferencedSegmentNumber] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.ReferencedSegmentNumber];
				for (int n = 0; n < value.Length; n++)
					dicomAttribute.SetInt32(n, value[n]);
			}
		}

		#endregion
	}
}