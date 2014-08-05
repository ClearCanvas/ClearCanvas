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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageServer.Core.Edit
{
	public class PatientInfo : IEquatable<PatientInfo>
	{
		public PatientInfo()
		{
		}

		public PatientInfo(PatientInfo other)
		{
			PatientsName = other.PatientsName;
			PatientId = other.PatientId;
			IssuerOfPatientId = other.IssuerOfPatientId;
		}

		[DicomField(DicomTags.PatientsName, DefaultValue = DicomFieldDefault.Null)]
		public string PatientsName { get; set; }

		[DicomField(DicomTags.PatientId, DefaultValue = DicomFieldDefault.Null)]
		public string PatientId { get; set; }

		[DicomField(DicomTags.IssuerOfPatientId, DefaultValue = DicomFieldDefault.Null)]
		public string IssuerOfPatientId { get; set; }

		#region IEquatable<PatientInfo> Members

		public bool Equals(PatientInfo other)
		{
			var name = new PersonName(PatientsName);
			var otherName = new PersonName(other.PatientsName);
			return name.Equals(otherName)
				&& String.Equals(PatientId, other.PatientId, StringComparison.InvariantCultureIgnoreCase);
		}

		#endregion

		public bool AreSame(PatientInfo other, bool caseSensitive)
		{
			if (other == null)
				return false;

			var comparison = caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

			return string.Equals(this.PatientsName, other.PatientsName, comparison) &&
			       string.Equals(this.PatientId, other.PatientId, comparison) &&
				   string.Equals(this.IssuerOfPatientId, other.IssuerOfPatientId, comparison);
		}
	}
}