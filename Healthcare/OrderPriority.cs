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

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// OrderPriority enumeration, defined by HL7 (see Timing Quantity TQ data-type)
    /// </summary>
    // NOTE: the numeric values are used for ordering and must not be modified! (Modification will break existing installations)
    [EnumValueClass(typeof(OrderPriorityEnum))]
	public enum OrderPriority
	{
        /// <summary>
        /// Routine
        /// </summary>
        [EnumValue("Routine")]
        R = 0,

        /// <summary>
        /// Urgent (or ASAP as defined by HL7)
        /// </summary>
        [EnumValue("Urgent")]
        A = 1,

        /// <summary>
        /// Stat
        /// </summary>
        [EnumValue("Stat")]
        S = 2
	}
}