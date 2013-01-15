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

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

using Iesi.Collections;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class PatientAdminService : ApplicationServiceBase, IPatientAdminService
    {
        public PatientAdminService()
        {
        }

        #region IPatientAdminService Members

        [ReadOperation]
        public IList<PatientProfile> ListPatientProfiles(PatientProfileSearchCriteria criteria)
        {
            IPatientProfileBroker profileBroker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            return profileBroker.Find(criteria);
        }

        [ReadOperation]
        public PatientProfile LoadPatientProfile(EntityRef profileRef)
        {
            IPatientProfileBroker profileBroker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            PatientProfile patient = profileBroker.Load(profileRef);

            return patient;
        }

        [ReadOperation]
        public PatientProfile LoadPatientProfileDetails(EntityRef profileRef)
        {
            IPatientProfileBroker broker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            PatientProfile patient = broker.Load(profileRef);

            // load all relevant collections
            broker.LoadAddressesForPatientProfile(patient);
            broker.LoadTelephoneNumbersForPatientProfile(patient);

            return patient;
        }

        [UpdateOperation]
        public void AddNewPatient(PatientProfile patient)
        {
            throw new NotImplementedException();
        }

        [UpdateOperation]
        public void UpdatePatientProfile(PatientProfile patient)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
