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

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the possible states for a <see cref="DesktopObject"/>.
    /// </summary>
    public enum DesktopObjectState
    {
        /// <summary>
        /// The object has been created, but not opened.
        /// </summary>
        New,

        /// <summary>
        /// The object is in the process of opening.
        /// </summary>
        Opening,

        /// <summary>
        /// The object is open.
        /// </summary>
        Open,

        /// <summary>
        /// The object is in the process of closing, but may return to the <see cref="Open"/> state if the operation is cancelled.
        /// </summary>
        Closing,

        /// <summary>
        /// The object has closed and is disposed.
        /// </summary>
        Closed
    }
}
