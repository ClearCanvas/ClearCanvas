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

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// A set of flags that indicate how a shelf should be displayed.
    /// </summary>
    [Flags]
    public enum ShelfDisplayHint
    {
        /// <summary>
        /// None.
        /// </summary>
		None = 0,

        /// <summary>
        /// Dock the shelf on the left.
        /// </summary>
		DockLeft = 1,

        /// <summary>
        /// Dock the shelf on the right.
        /// </summary>
		DockRight = 2,

        /// <summary>
        /// Dock the shelf at the top.
        /// </summary>
		DockTop = 4,

        /// <summary>
        /// Dock the shelf at the bottom.
        /// </summary>
		DockBottom  = 8,

        /// <summary>
        /// Float the shelf.
        /// </summary>
		DockFloat = 16,

        /// <summary>
        /// Dock the shelf in auto-hide mode.
        /// </summary>
		DockAutoHide = 32,

        /// <summary>
        /// Hide the shelf whenever a new workspace opens.
        /// </summary>
		HideOnWorkspaceOpen = 64,

		/// <summary>
		/// Show the shelf floating (<see cref="DockFloat"/>) near the mouse.
		/// </summary>
		ShowNearMouse = 144 //128 + 16 (DockFloat)
    }
}
