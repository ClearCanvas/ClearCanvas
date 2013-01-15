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
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core.Modelling;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// PatientProfile entity
    /// </summary>
    [UniqueKey("Mrn", new string[] { "Mrn.Id", "Mrn.AssigningAuthority" })]
	public partial class PatientProfile : Entity
	{
        private void CustomInitialize()
        {
        }

        public virtual DateTime? DateOfBirth 
        {
            get { return _dateOfBirth == null ? _dateOfBirth : _dateOfBirth.Value.Date; }
			set { _dateOfBirth = value == null ? value : value.Value.Date; } 
        }

        public virtual Address CurrentHomeAddress
        {
            get
            {
                return CollectionUtils.SelectFirst(this.Addresses,
                    delegate(Address address) { return address.Type == AddressType.R && address.IsCurrent; });
            }
        }

        public virtual Address CurrentWorkAddress
        {
            get
            {
                return CollectionUtils.SelectFirst(this.Addresses,
                   delegate(Address address) { return address.Type == AddressType.B && address.IsCurrent; });
            }
        }

        public virtual TelephoneNumber CurrentHomePhone
        {
            get
            {
                return CollectionUtils.SelectFirst(this.TelephoneNumbers,
                  delegate(TelephoneNumber phone) { return phone.Use == TelephoneUse.PRN && phone.Equipment == TelephoneEquipment.PH && phone.IsCurrent; });
            }
        }

        public virtual TelephoneNumber CurrentWorkPhone
        {
            get
            {
                return CollectionUtils.SelectFirst(this.TelephoneNumbers,
                    delegate(TelephoneNumber phone) { return phone.Use == TelephoneUse.WPN && phone.Equipment == TelephoneEquipment.PH && phone.IsCurrent; });
            }
        }

        public virtual void SetDeceased(DateTime timeOfDeath)
        {
            this.DeathIndicator = true;
            this.TimeOfDeath = timeOfDeath;
        }
    }
}