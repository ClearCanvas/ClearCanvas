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

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// ProtocolStatus enumeration
    /// </summary>
    [EnumValueClass(typeof(ProtocolStatusEnum))]
    public enum ProtocolStatus
    {
        /// <summary>
        /// Pending
        /// </summary>
        [EnumValue("Pending", Description = "Protocol is pending")]
        PN,

        /// <summary>
        /// Protocolled
        /// </summary>
        [EnumValue("Protocolled", Description = "Protocol assigned and order accepted")]
        PR,

        /// <summary>
        /// Rejected
        /// </summary>
        [EnumValue("Rejected", Description = "Protocol assigned and order rejected")]
        RJ,

        /// <summary>
        /// Awaiting Approval
        /// </summary>
        [EnumValue("Awaiting Approval", Description = "Protocol submitted for approval by resident")]
        AA,

        /// <summary>
        /// Cancelled
        /// </summary>
        [EnumValue("Cancelled", Description = "Protocol is cancelled")]
        X
    }
}