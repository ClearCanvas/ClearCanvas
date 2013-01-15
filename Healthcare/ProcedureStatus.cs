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
    /// ProcedureStatus enumeration
    /// </summary>
    [EnumValueClass(typeof(ProcedureStatusEnum))]
	public enum ProcedureStatus
	{
        /// <summary>
        /// Scheduled
        /// </summary>
        [EnumValue("Scheduled", Description = "In process, scheduled")]
        SC,

        /// <summary>
        /// Canceled
        /// </summary>
        [EnumValue("Cancelled", Description = "Procedure was cancelled")]
        CA,

        /// <summary>
        /// Completed
        /// </summary>
        [EnumValue("Completed", Description = "Procedure was completed, including report publishing")]
        CM,

        /// <summary>
        /// Discontinued
        /// </summary>
        [EnumValue("Discontinued", Description = "Procedure was discontinued")]
        DC,

        /// <summary>
        /// In Progress
        /// </summary>
        [EnumValue("In Progress", Description = "In process, unspecified")]
        IP,

		/// <summary>
		/// Ghost
		/// </summary>
		[EnumValue("Ghost", Description = "Ghost of procedure that was moved to another order")]
		GH,
	}
}