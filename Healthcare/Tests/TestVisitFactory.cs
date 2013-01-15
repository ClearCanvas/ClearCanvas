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

namespace ClearCanvas.Healthcare.Tests
{
	public static class TestVisitFactory
	{
		public static Visit CreateVisit(Patient patient)
		{
			return CreateVisit(patient, "10001111");
		}

		public static Visit CreateVisit(Patient patient, string visitNumber)
		{
			return new Visit(
				patient,
				new VisitNumber(visitNumber, new InformationAuthorityEnum("UHN", "UHN", "")),
				VisitStatus.AA,
				DateTime.Now - TimeSpan.FromDays(2),
				null,
				null,
				new PatientClassEnum("I", "Inpatient", null),
				new PatientTypeEnum("X", "Whatever", null),
				new AdmissionTypeEnum("A", "Who cares", null),
				TestFacilityFactory.CreateFacility(),
				TestLocationFactory.CreateLocation(),
				null,
				null,
				null,
				null,
				false,
				null,
				null,
				null);
		}
	}
}
