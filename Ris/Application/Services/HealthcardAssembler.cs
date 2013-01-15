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
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Services
{
    public class HealthcardAssembler
    {
        public HealthcardDetail CreateHealthcardDetail(HealthcardNumber healthcard)
        {
            if (healthcard == null)
                return new HealthcardDetail();

            return new HealthcardDetail(
                healthcard.Id,
                EnumUtils.GetEnumValueInfo(healthcard.AssigningAuthority),
                healthcard.VersionCode,
                healthcard.ExpiryDate);
        }

        public void UpdateHealthcard(HealthcardNumber hc, HealthcardDetail detail, IPersistenceContext context)
        {
            hc.Id = detail.Id;
            hc.AssigningAuthority = EnumUtils.GetEnumValue<InsuranceAuthorityEnum>(detail.AssigningAuthority, context);
            hc.VersionCode = detail.VersionCode;
            hc.ExpiryDate = detail.ExpiryDate;
        }
    }
}
