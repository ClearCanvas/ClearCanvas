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

using System.Xml.Serialization;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Core.Data
{
	/// <summary>
	/// Represents the serializable patient information
	/// </summary>
	[XmlRoot("Patient")]
	public class PatientInformation
	{
		#region Constructors
		public PatientInformation()
		{
		}

		public PatientInformation(IDicomAttributeProvider attributeProvider)
		{
			if (attributeProvider[DicomTags.PatientsName] != null)
				Name = attributeProvider[DicomTags.PatientsName].ToString();

			if (attributeProvider[DicomTags.PatientId] != null)
				PatientId = attributeProvider[DicomTags.PatientId].ToString();

			if (attributeProvider[DicomTags.PatientsAge] != null)
				Age = attributeProvider[DicomTags.PatientsAge].ToString();

			if (attributeProvider[DicomTags.PatientsBirthDate] != null)
				PatientsBirthdate = attributeProvider[DicomTags.PatientsBirthDate].ToString();

			if (attributeProvider[DicomTags.PatientsSex] != null)
				Sex = attributeProvider[DicomTags.PatientsSex].ToString();

			if (attributeProvider[DicomTags.IssuerOfPatientId] != null)
				IssuerOfPatientId = attributeProvider[DicomTags.IssuerOfPatientId].ToString();
		}
		#endregion

		#region Public Properties

		public string Name { get; set; }

		public string PatientId { get; set; }

		public string PatientsBirthdate { get; set; }

		public string Age { get; set; }

		public string Sex { get; set; }

		public string IssuerOfPatientId { get; set; }

		#endregion
	}
}