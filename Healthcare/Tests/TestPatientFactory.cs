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

namespace ClearCanvas.Healthcare.Tests
{
	public static class TestPatientFactory
    {
		public static Patient CreatePatient()
        {
        	return CreatePatient("0000111");
        }

		public static Patient CreatePatient(string mrn)
        {
            Patient patient = new Patient();
            PatientProfile profile = new PatientProfile(
                new PatientIdentifier(mrn, new InformationAuthorityEnum("UHN", "UHN", "")),
                null,
                new HealthcardNumber("1111222333", new InsuranceAuthorityEnum("OHIP", "OHIP", ""), null, null),
                new PersonName("Roberts", "Bob", null, null, null, null),
                DateTime.Now - TimeSpan.FromDays(4000),
                Sex.M,
                new SpokenLanguageEnum("en", "English", null),
                new ReligionEnum("X", "unknown", null),
                false,
				null,
                null,
                null,
                null,
                null,
                null,
                null,
                patient
                );

            patient.AddProfile(profile);

            return patient;
        }
    }
}
