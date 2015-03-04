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

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// OrderStatus enumeration as defined by HL7 (4.5.1.5).  This is only a subset of what HL7 defines.
    /// More values can be added later if necessary.
    /// </summary>
    [EnumValueClass(typeof(OrderStatusEnum))]
    public enum OrderStatus
	{
        /// <summary>
        /// Scheduled
        /// </summary>
		[EnumValue("Scheduled", Description = "ScheduledDescription")]
        SC,
 
        /// <summary>
        /// Canceled
        /// </summary>
		[EnumValue("Canceled", Description = "CanceledDescription")]
        CA,

        /// <summary>
        /// Completed
        /// </summary>
		[EnumValue("Completed", Description = "CompletedDescription")]
        CM,

        /// <summary>
        /// Discontinued
        /// </summary>
		[EnumValue("Discontinued", Description = "DiscontinuedDescription")]
        DC,

        /// <summary>
        /// In Progress
        /// </summary>
		[EnumValue("InProgress", Description = "InProgressDescription")]
        IP,

        /// <summary>
        /// Replaced
        /// </summary>
		[EnumValue("Replaced", Description = "ReplacedDescription")]
        RP,

		/// <summary>
		/// Merged
		/// </summary>
		[EnumValue("Merged", Description = "MergedDescription")]
		MG,
	}
}