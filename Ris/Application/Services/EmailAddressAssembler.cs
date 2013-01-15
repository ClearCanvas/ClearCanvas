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
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
    public class EmailAddressAssembler
    {
        public EmailAddressDetail CreateEmailAddressDetail(EmailAddress emailAddress, IPersistenceContext context)
        {
            EmailAddressDetail detail = new EmailAddressDetail();

            detail.Address = emailAddress.Address;
            detail.ValidRangeFrom = emailAddress.ValidRange.From;
            detail.ValidRangeUntil = emailAddress.ValidRange.Until;

            return detail;
        }

        public EmailAddress CreateEmailAddress(EmailAddressDetail detail)
        {
            EmailAddress emailAddress = new EmailAddress();

            emailAddress.Address = detail.Address;
            emailAddress.ValidRange = new DateTimeRange(
                detail.ValidRangeFrom,
                detail.ValidRangeUntil);

            return emailAddress;
        }

    }
}
