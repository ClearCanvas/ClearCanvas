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
    public class PersonNameAssembler
    {
        public PersonNameDetail CreatePersonNameDetail(PersonName personName)
        {
            if (personName == null)
                return new PersonNameDetail();

            PersonNameDetail detail = new PersonNameDetail();
            detail.FamilyName = personName.FamilyName;
            detail.GivenName = personName.GivenName;
            detail.MiddleName = personName.MiddleName;
            detail.Prefix = personName.Prefix;
            detail.Suffix = personName.Suffix;
            detail.Degree = personName.Degree;
            return detail;
        }

        public void UpdatePersonName(PersonNameDetail detail, PersonName personName)
        {
            personName.FamilyName = TrimDetail(detail.FamilyName);
            personName.GivenName = TrimDetail(detail.GivenName);
            personName.MiddleName = TrimDetail(detail.MiddleName);
            personName.Prefix = TrimDetail(detail.Prefix);
            personName.Suffix = TrimDetail(detail.Suffix);
            personName.Degree = TrimDetail(detail.Degree);
        }

		private static string TrimDetail(string detail)
		{
			return string.IsNullOrEmpty(detail) ? detail : detail.Trim();
		}
    }
}
