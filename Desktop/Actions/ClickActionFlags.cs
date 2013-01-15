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

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Set of flags that customize the behaviour of click actions.
    /// </summary>
    [Flags]
    public enum ClickActionFlags
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Specifies that the action is a "check" action (e.g. that it has toggle behaviour).
        /// </summary>
        CheckAction = 0x01,

		/// <summary>
		/// Specifies that parents of the action should be checked when the action is checked.
		/// </summary>
		CheckParents = 0x02
    }
}
