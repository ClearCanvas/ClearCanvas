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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class PatientAllergyDetail : DataContractBase
	{
		// Default parameterless constructor for Jsml deserialization
		public PatientAllergyDetail()
		{
			this.ReporterName = new PersonNameDetail();
		}

		public PatientAllergyDetail(EnumValueInfo allergenType,
			string allergenDescription,
			EnumValueInfo severity,
			string reaction,
			EnumValueInfo sensitivityType,
			DateTime? onsetTime,
			DateTime? reportedTime,
			PersonNameDetail reporterName,
			EnumValueInfo reporterRelationshipType)
		{
			this.AllergenType = allergenType;
			this.AllergenDescription = allergenDescription;
			this.Severity = severity;
			this.Reaction = reaction;
			this.SensitivityType = sensitivityType;
			this.OnsetTime = onsetTime;
			this.ReportedTime = reportedTime;
			this.ReporterName = reporterName;
			this.ReporterRelationshipType = reporterRelationshipType;
		}

		[DataMember]
		public EnumValueInfo AllergenType;

		[DataMember]
		public string AllergenDescription;

		[DataMember]
		public EnumValueInfo Severity;

		[DataMember]
		public string Reaction;

		[DataMember]
		public EnumValueInfo SensitivityType;

		[DataMember]
		public DateTime? OnsetTime;

		[DataMember]
		public DateTime? ReportedTime;

		[DataMember]
		public PersonNameDetail ReporterName;

		[DataMember]
		public EnumValueInfo ReporterRelationshipType;
	}
}
