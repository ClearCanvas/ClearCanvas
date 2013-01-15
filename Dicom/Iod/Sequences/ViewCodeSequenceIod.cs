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
using System.Text;

namespace ClearCanvas.Dicom.Iod.Sequences
{

	public class ViewCodeSequenceIod : SequenceIodBase
	{
		public ViewCodeSequenceIod()
		{
		}

		public ViewCodeSequenceIod(DicomSequenceItem sequenceItem)
			: base(sequenceItem)
		{
		}

		public string CodeValue
		{
			get { return base.DicomSequenceItem[DicomTags.CodeValue].GetString(0, ""); }
			set { base.DicomSequenceItem[DicomTags.CodeValue].SetString(0, value); }
		}

		public string CodingSchemeDesignator
		{
			get { return base.DicomSequenceItem[DicomTags.CodingSchemeDesignator].GetString(0, ""); }
			set { base.DicomSequenceItem[DicomTags.CodingSchemeDesignator].SetString(0, value); }
		}

		public string CodingSchemeVersion
		{
			get { return base.DicomSequenceItem[DicomTags.CodingSchemeVersion].GetString(0, ""); }
			set { base.DicomSequenceItem[DicomTags.CodingSchemeVersion].SetString(0, value); }
		}

		public string CodeMeaning
		{
			get { return base.DicomSequenceItem[DicomTags.CodeMeaning].GetString(0, ""); }
			set { base.DicomSequenceItem[DicomTags.CodeMeaning].SetString(0, value); }
		}

		public string ContextIdentifier
		{
			get { return base.DicomSequenceItem[DicomTags.ContextIdentifier].GetString(0, ""); }
			set { base.DicomSequenceItem[DicomTags.ContextIdentifier].SetString(0, value); }
		}

		public string MappingResource
		{
			get { return base.DicomSequenceItem[DicomTags.MappingResource].GetString(0, ""); }
			set { base.DicomSequenceItem[DicomTags.MappingResource].SetString(0, value); }
		}

		public DateTime? ContextGroupVersion
		{
			get { return base.DicomSequenceItem[DicomTags.ContextGroupVersion].GetDateTime(0); }
			set { base.DicomSequenceItem[DicomTags.ContextGroupVersion].SetDateTime(0, value); }
		}

		public string ContextGroupExtensionFlag
		{
			get { return base.DicomSequenceItem[DicomTags.ContextGroupExtensionFlag].GetString(0, ""); }
			set { base.DicomSequenceItem[DicomTags.ContextGroupExtensionFlag].SetString(0, value); }
		}

		public DateTime? ContextGroupLocalVersion
		{
			get { return base.DicomSequenceItem[DicomTags.ContextGroupLocalVersion].GetDateTime(0); }
			set { base.DicomSequenceItem[DicomTags.ContextGroupLocalVersion].SetDateTime(0, value); }
		}

		public string ContextGroupExtensionCreatorUid
		{
			get { return base.DicomSequenceItem[DicomTags.ContextGroupExtensionCreatorUid].GetString(0, ""); }
			set { base.DicomSequenceItem[DicomTags.ContextGroupExtensionCreatorUid].SetString(0, value); }
		}
	}
}
