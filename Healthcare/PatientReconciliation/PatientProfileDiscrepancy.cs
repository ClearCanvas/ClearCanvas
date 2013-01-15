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

namespace ClearCanvas.Healthcare.PatientReconciliation
{
    [Flags]
    public enum PatientProfileDiscrepancy : uint
    {
        None            = 0x00000000,

        Healthcard      = 0x00000001,
        FamilyName      = 0x00000002,
        GivenName       = 0x00000004,
        DateOfBirth     = 0x00000008,
        Sex             = 0x00000010,
        HomePhone       = 0x00000020,
        HomeAddress     = 0x00000040,
        WorkPhone       = 0x00000080,
        WorkAddress     = 0x00000100,
        MiddleName      = 0x00000200,

        All             = 0xffffffff
    }
}
