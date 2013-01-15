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

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Used by the action attribute mechanism to maintain context 
    /// between attributes that co-operate to build the same action.
    /// </summary>
    public interface IActionBuildingContext
    {
        /// <summary>
        /// Gets or sets the action that is being built.
        /// </summary>
        Action Action { get; set; }

        /// <summary>
        /// Gets the resource resolver that is supplied to the action.
        /// </summary>
        IResourceResolver ResourceResolver { get; }

        /// <summary>
        /// Gets the target object on which the action operates.
        /// </summary>
        object ActionTarget { get; }

        /// <summary>
        /// Gets the logical action ID for the action.
        /// </summary>
        string ActionID { get; }
	}
}
