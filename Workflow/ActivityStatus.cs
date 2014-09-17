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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Workflow
{
    [EnumValueClass(typeof(ActivityStatusEnum))]
    public enum ActivityStatus
    {
        /// <summary>
        /// Scheduled
        /// </summary>
        [EnumValue("Scheduled")]
        SC = 0,

        /// <summary>
        /// In Progress
        /// </summary>
        [EnumValue("InProgress")]
        IP = 1,

        /// <summary>
        /// Suspended
        /// </summary>
        [EnumValue("Suspended")]
        SU = 2,

        /// <summary>
        /// Completed
        /// </summary>
        [EnumValue("Completed")]
        CM = 3,

        /// <summary>
        /// Discontinued
        /// </summary>
        [EnumValue("Discontinued")]
        DC = 4
    }
}
