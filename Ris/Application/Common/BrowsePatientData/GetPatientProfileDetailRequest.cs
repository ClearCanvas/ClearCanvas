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

using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.BrowsePatientData
{
    [DataContract]
    public class GetPatientProfileDetailRequest : DataContractBase
    {
        public GetPatientProfileDetailRequest(EntityRef patientProfileRef,
            bool includeAddresses,
            bool includeContactPersons,
            bool includeEmailAddresses,
            bool includeTelephoneNumbers,
            bool includeNotes,
            bool includeAttachments,
            bool includeAlerts,
			bool includeAllergies)
        {
            this.PatientProfileRef = patientProfileRef;
            this.IncludeAddresses = includeAddresses;
            this.IncludeContactPersons = includeContactPersons;
            this.IncludeEmailAddresses = includeEmailAddresses;
            this.IncludeTelephoneNumbers = includeTelephoneNumbers;
            this.IncludeNotes = includeNotes;
            this.IncludeAttachments = includeAttachments;
            this.IncludeAlerts = includeAlerts;
        	this.IncludeAllergies = includeAllergies;
        }

        public GetPatientProfileDetailRequest()
        {
        }

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public bool IncludeAddresses;

        [DataMember]
        public bool IncludeContactPersons;

        [DataMember]
        public bool IncludeEmailAddresses;

        [DataMember]
        public bool IncludeTelephoneNumbers;

        [DataMember]
        public bool IncludeNotes;

        [DataMember]
        public bool IncludeAttachments;

        [DataMember]
        public bool IncludeAlerts;

		[DataMember]
		public bool IncludeAllergies;
    }
}
