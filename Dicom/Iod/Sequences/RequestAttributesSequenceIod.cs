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

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// Referenced Series Sequence.  
	/// </summary>
	/// <remarks>As per Part 3, Table 10.4, pg 78</remarks>
	public class RequestAttributesSequenceIod : SequenceIodBase
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="RequestAttributesSequenceIod"/> class.
		/// </summary>
		public RequestAttributesSequenceIod()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RequestAttributesSequenceIod"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public RequestAttributesSequenceIod(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem)
		{
		}
		#endregion

		#region Public Properties

		/// <summary>
		/// Requested Procedure Id.
		/// </summary>
		/// <value>The requested procedure Id.</value>
		public string RequestedProcedureId
		{
			get { return DicomAttributeProvider[DicomTags.RequestedProcedureId].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.RequestedProcedureId].SetString(0, value); }
		}

		/// <summary>
		/// Scheduled Procedure Step Id.
		/// </summary>
		/// <value>The scheduled procedure step Id.</value>
		public string ScheduledProcedureStepId
		{
			get { return DicomAttributeProvider[DicomTags.ScheduledProcedureStepId].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.ScheduledProcedureStepId].SetString(0, value); }
		}
		#endregion
	}

}
