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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.ContextGroups
{
	public sealed class KeyObjectSelectionDocumentTitleContextGroup : ContextGroupBase<KeyObjectSelectionDocumentTitle>
	{
		private KeyObjectSelectionDocumentTitleContextGroup() : base(7010, "Key Object Selection Document Title", true, new DateTime(2004, 09, 20)) {}

		public static readonly KeyObjectSelectionDocumentTitle OfInterest = new KeyObjectSelectionDocumentTitle(113000, "Of Interest");
		public static readonly KeyObjectSelectionDocumentTitle RejectedForQualityReasons = new KeyObjectSelectionDocumentTitle(113001, "Rejected for Quality Reasons");
		public static readonly KeyObjectSelectionDocumentTitle ForReferringProvider = new KeyObjectSelectionDocumentTitle(113002, "For Referring Provider");
		public static readonly KeyObjectSelectionDocumentTitle ForSurgery = new KeyObjectSelectionDocumentTitle(113003, "For Surgery");
		public static readonly KeyObjectSelectionDocumentTitle ForTeaching = new KeyObjectSelectionDocumentTitle(113004, "For Teaching");
		public static readonly KeyObjectSelectionDocumentTitle ForConference = new KeyObjectSelectionDocumentTitle(113005, "For Conference");
		public static readonly KeyObjectSelectionDocumentTitle ForTherapy = new KeyObjectSelectionDocumentTitle(113006, "For Therapy");
		public static readonly KeyObjectSelectionDocumentTitle ForPatient = new KeyObjectSelectionDocumentTitle(113007, "For Patient");
		public static readonly KeyObjectSelectionDocumentTitle ForPeerReview = new KeyObjectSelectionDocumentTitle(113008, "For Peer Review");
		public static readonly KeyObjectSelectionDocumentTitle ForResearch = new KeyObjectSelectionDocumentTitle(113009, "For Research");
		public static readonly KeyObjectSelectionDocumentTitle QualityIssue = new KeyObjectSelectionDocumentTitle(113010, "Quality Issue");
		public static readonly KeyObjectSelectionDocumentTitle BestInSet = new KeyObjectSelectionDocumentTitle(113013, "Best In Set");
		public static readonly KeyObjectSelectionDocumentTitle ForPrinting = new KeyObjectSelectionDocumentTitle(113018, "For Printing");
		public static readonly KeyObjectSelectionDocumentTitle ForReportAttachment = new KeyObjectSelectionDocumentTitle(113020, "For Report Attachment");
		public static readonly KeyObjectSelectionDocumentTitle Manifest = new KeyObjectSelectionDocumentTitle(113030, "Manifest");
		public static readonly KeyObjectSelectionDocumentTitle SignedManifest = new KeyObjectSelectionDocumentTitle(113031, "Signed Manifest");
		public static readonly KeyObjectSelectionDocumentTitle CompleteStudyContent = new KeyObjectSelectionDocumentTitle(113032, "Complete Study Content");
		public static readonly KeyObjectSelectionDocumentTitle SignedCompleteStudyContent = new KeyObjectSelectionDocumentTitle(113033, "Signed Complete Study Content");
		public static readonly KeyObjectSelectionDocumentTitle CompleteAcquisitionContent = new KeyObjectSelectionDocumentTitle(113034, "Complete Acquisition Content");
		public static readonly KeyObjectSelectionDocumentTitle SignedCompleteAcquisitionContent = new KeyObjectSelectionDocumentTitle(113035, "Signed Complete Acquisition Content");
		public static readonly KeyObjectSelectionDocumentTitle GroupOfFramesForDisplay = new KeyObjectSelectionDocumentTitle(113036, "Group of Frames for Display");

		#region Singleton Instancing

		private static readonly KeyObjectSelectionDocumentTitleContextGroup _contextGroup = new KeyObjectSelectionDocumentTitleContextGroup();

		public static KeyObjectSelectionDocumentTitleContextGroup Instance
		{
			get { return _contextGroup; }
		}

		#endregion

		#region Static Enumeration of Values

		public static IEnumerable<KeyObjectSelectionDocumentTitle> Values
		{
			get
			{
				yield return OfInterest;
				yield return RejectedForQualityReasons;
				yield return ForReferringProvider;
				yield return ForSurgery;
				yield return ForTeaching;
				yield return ForConference;
				yield return ForTherapy;
				yield return ForPatient;
				yield return ForPeerReview;
				yield return ForResearch;
				yield return QualityIssue;
				yield return BestInSet;
				yield return ForPrinting;
				yield return ForReportAttachment;
				yield return Manifest;
				yield return SignedManifest;
				yield return CompleteStudyContent;
				yield return SignedCompleteStudyContent;
				yield return CompleteAcquisitionContent;
				yield return SignedCompleteAcquisitionContent;
				yield return GroupOfFramesForDisplay;
			}
		}

		/// <summary>
		/// Gets an enumerator that iterates through the defined titles.
		/// </summary>
		/// <returns>A <see cref="IEnumerator{T}"/> object that can be used to iterate through the defined titles.</returns>
		public override IEnumerator<KeyObjectSelectionDocumentTitle> GetEnumerator()
		{
			return Values.GetEnumerator();
		}

		public static KeyObjectSelectionDocumentTitle LookupTitle(CodeSequenceMacro codeSequence)
		{
			return Instance.Lookup(codeSequence);
		}

		#endregion
	}

	/// <summary>
	/// Represents a key object selection document title.
	/// </summary>
	public sealed class KeyObjectSelectionDocumentTitle : ContextGroupBase<KeyObjectSelectionDocumentTitle>.ContextGroupItemBase
	{
		/// <summary>
		/// Constructor for titles defined in DICOM 2008, Part 16, Annex B, CID 7010.
		/// </summary>
		internal KeyObjectSelectionDocumentTitle(int value, string meaning) : base("DCM", value.ToString(), meaning) {}

		/// <summary>
		/// Constructs a new key object selection document title.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public KeyObjectSelectionDocumentTitle(string codingSchemeDesignator, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codeValue, codeMeaning) {}

		/// <summary>
		/// Constructs a new key object selection document title.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codingSchemeVersion">The version of the coding scheme in which this code is defined, if known. Should be <code>null</code> if not explicitly specified.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public KeyObjectSelectionDocumentTitle(string codingSchemeDesignator, string codingSchemeVersion, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codingSchemeVersion, codeValue, codeMeaning) {}
	}
}