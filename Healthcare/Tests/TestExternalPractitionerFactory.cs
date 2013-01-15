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
using Iesi.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Tests
{
    internal static class TestExternalPractitionerFactory
    {
        internal static ExternalPractitioner CreatePractitioner()
        {
            return new ExternalPractitioner(
                new PersonName("Who", "Doctor", null, null, null, null),
                "1234",
                "5678",
				false,
				null,
				Platform.Time,
                new HashedSet<ExternalPractitionerContactPoint>(),
				new Dictionary<string, string>(),
				null);
        }
    }
}
