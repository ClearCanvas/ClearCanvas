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
	/// ImageAvailability enumeration
    /// </summary>
    [EnumValueClass(typeof(ImageAvailabilityEnum))]
	public enum ImageAvailability
	{
		/// <summary>
		/// Unknown
		/// </summary>
		[EnumValue("Unknown", Description = "Image availability has not been checked.")]
		X,

		/// <summary>
		/// Indeterminate
		/// </summary>
        [EnumValue("Indeterminate", Description = "The system does not have enough information to determine image availability.")]
		N,

		/// <summary>
		/// Zero
		/// </summary>
		[EnumValue("Zero", Description = "No images are available.")]
		Z,

		/// <summary>
		/// Partial
		/// </summary>
		[EnumValue("Partial", Description = "Some images are available, but not as many as expected.")]
		P,

		/// <summary>
		/// Complete
		/// </summary>
		[EnumValue("Complete", Description = "All images are available.")]
		C
	}
}