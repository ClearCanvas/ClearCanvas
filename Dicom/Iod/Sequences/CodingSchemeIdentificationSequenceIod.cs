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

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// CodingSchemeIdentification Sequence Item
	/// </summary>
	/// <remarks>
	/// <para>As defined in the DICOM Standard 2009, Part 3, Section C.12.1 (Table C.12-1)</para>
	/// </remarks>
	public class CodingSchemeIdentificationSequenceItem
		: SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodingSchemeIdentificationSequence"/> class.
		/// </summary>
		public CodingSchemeIdentificationSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodingSchemeIdentificationSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CodingSchemeIdentificationSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of CodingSchemeDesignator in the underlying collection. Type 1.
		/// </summary>
		public string CodingSchemeDesignator
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeDesignator].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "CodingSchemeDesignator is Type 1 Required.");
				DicomAttributeProvider[DicomTags.CodingSchemeDesignator].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CodingSchemeRegistry in the underlying collection. Type 1C.
		/// </summary>
		public string CodingSchemeRegistry
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeRegistry].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.CodingSchemeRegistry] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CodingSchemeRegistry].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CodingSchemeUid in the underlying collection. Type 1C.
		/// </summary>
		public string CodingSchemeUid
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeUid].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.CodingSchemeUid] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CodingSchemeUid].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CodingSchemeExternalId in the underlying collection. Type 2C.
		/// </summary>
		public string CodingSchemeExternalId
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeExternalId].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.CodingSchemeExternalId] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CodingSchemeExternalId].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CodingSchemeName in the underlying collection. Type 3.
		/// </summary>
		public string CodingSchemeName
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeName].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.CodingSchemeName] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CodingSchemeName].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CodingSchemeVersion in the underlying collection. Type 3.
		/// </summary>
		public string CodingSchemeVersion
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeVersion].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.CodingSchemeVersion] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CodingSchemeVersion].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CodingSchemeResponsibleOrganization in the underlying collection. Type 3.
		/// </summary>
		public string CodingSchemeResponsibleOrganization
		{
			get { return DicomAttributeProvider[DicomTags.CodingSchemeResponsibleOrganization].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.CodingSchemeResponsibleOrganization] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CodingSchemeResponsibleOrganization].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets an enumeration of <see cref="ClearCanvas.Dicom.DicomTag"/>s used by this sequence item.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.CodingSchemeDesignator;
				yield return DicomTags.CodingSchemeRegistry;
				yield return DicomTags.CodingSchemeUid;
				yield return DicomTags.CodingSchemeExternalId;
				yield return DicomTags.CodingSchemeName;
				yield return DicomTags.CodingSchemeVersion;
				yield return DicomTags.CodingSchemeResponsibleOrganization;
			}
		}
	}
}