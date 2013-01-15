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

    [EnumValueClass(typeof(ReportStatusEnum))]
    public enum ReportStatus
	{
        /// <summary>
        /// Draft
        /// </summary>
        [EnumValue("Draft", Description = "Draft")]
        D,

        /// <summary>
        /// Preliminary
        /// </summary>
        [EnumValue("Preliminary", Description = "Preliminary (report has not been verified)")]
        P,
 
        /// <summary>
        /// Final
        /// </summary>
        [EnumValue("Final", Description = "Final (report has been verified)")]
        F,

        /// <summary>
        /// Corrected
        /// </summary>
        [EnumValue("Corrected", Description = "Corrected (report has one or more addenda)")]
        C,

        /// <summary>
        /// Cancelled
        /// </summary>
        [EnumValue("Cancelled", Description = "Report is cancelled")]
        X
	}
}