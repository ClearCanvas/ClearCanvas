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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Services
{
    /// <summary>
    /// This class is basically a big hack to present the client with a simplified set of choices for phone
    /// number type.  This class maps back and forth between <see cref="SimplifiedPhoneType"/> and the
    /// the <see cref="TelephoneUse"/> and <see cref="TelephoneEquipment"/> enums.
    /// </summary>
    class SimplifiedPhoneTypeAssembler
    {
        public enum SimplifiedPhoneType
        {
            Unknown,    // need this to deal with combinations that don't map to anything

            Home,
            Work,
            Mobile,
            Fax,
            Pager,
        }

        public EnumValueInfo GetSimplifiedPhoneType(TelephoneNumber number)
        {
            SimplifiedPhoneType t = SimplifiedPhoneType.Unknown;
            if (number.Use == TelephoneUse.PRN)
            {
                if (number.Equipment == TelephoneEquipment.PH)
                    t = SimplifiedPhoneType.Home;
                else if (number.Equipment == TelephoneEquipment.CP)
                    t = SimplifiedPhoneType.Mobile;
            }
            else if (number.Use == TelephoneUse.WPN)
            {
                if (number.Equipment == TelephoneEquipment.PH)
                    t = SimplifiedPhoneType.Work;
                else if (number.Equipment == TelephoneEquipment.BP)
                    t = SimplifiedPhoneType.Pager;
                else if (number.Equipment == TelephoneEquipment.FX)
                    t = SimplifiedPhoneType.Fax;
            }
            return new EnumValueInfo(t.ToString(), t.ToString());
        }

        public void UpdatePhoneNumber(EnumValueInfo simplePhoneType, TelephoneNumber number)
        {
            SimplifiedPhoneType type = (SimplifiedPhoneType)Enum.Parse(typeof(SimplifiedPhoneType), simplePhoneType.Code);
            switch (type)
            {
                case SimplifiedPhoneType.Home:
                    number.Equipment = TelephoneEquipment.PH;
                    number.Use = TelephoneUse.PRN;
                    break;
                case SimplifiedPhoneType.Work:
                    number.Equipment = TelephoneEquipment.PH;
                    number.Use = TelephoneUse.WPN;
                    break;
                case SimplifiedPhoneType.Mobile:
                    number.Equipment = TelephoneEquipment.CP;
                    number.Use = TelephoneUse.PRN;
                    break;
                case SimplifiedPhoneType.Fax:
                    number.Equipment = TelephoneEquipment.FX;
                    number.Use = TelephoneUse.WPN;
                    break;
                case SimplifiedPhoneType.Pager:
                    number.Equipment = TelephoneEquipment.BP;
                    number.Use = TelephoneUse.WPN;
                    break;
                case SimplifiedPhoneType.Unknown:
                    // do nothing
                    break;
            }
        }

        public List<EnumValueInfo> GetPatientPhoneTypeChoices()
        {
            // order is important because it is the order that things will show up in the UI by default
            return GetPhoneTypeChoices(
                new SimplifiedPhoneType[]
                    {
                        SimplifiedPhoneType.Home,
                        SimplifiedPhoneType.Work,
                        SimplifiedPhoneType.Mobile
                    });
        }

        public List<EnumValueInfo> GetPractitionerPhoneTypeChoices()
        {
            // order is important because it is the order that things will show up in the UI by default
            return GetPhoneTypeChoices(
                new SimplifiedPhoneType[]
                    {
                        SimplifiedPhoneType.Fax,
                        SimplifiedPhoneType.Work,
                        SimplifiedPhoneType.Mobile,
                        SimplifiedPhoneType.Pager
                    });
        }

        public List<EnumValueInfo> GetPhoneTypeChoices(SimplifiedPhoneType[] list)
        {
            return CollectionUtils.Map<SimplifiedPhoneType, EnumValueInfo>(list,
                delegate(SimplifiedPhoneType t) { return new EnumValueInfo(t.ToString(), t.ToString()); });
        }



    }
}
