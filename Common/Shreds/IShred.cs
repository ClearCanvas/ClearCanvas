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

namespace ClearCanvas.Common.Shreds
{
    /// <summary>
    /// Defines the set of operations that are possible on a Shred
    /// </summary>
    /// <remarks>See also <see cref="ShredExtensionPoint"/></remarks>
    public interface IShred
    {
        /// <summary>
        /// Shred should initialize all required resources and data structures and begin 
        /// exeuction of its mainline code
        /// </summary>
        void Start();
        /// <summary>
        /// Shred should stop, and release all held resources.
        /// </summary>
        void Stop();
        /// <summary>
        /// Shred should return a human-readable, friendly name that will be used in
        /// display lists and other human-readable user-interfaces.
        /// </summary>
        /// <returns></returns>
        string GetDisplayName();
        /// <summary>
        /// Shred should return a lengthier description of what it was created for and 
        /// what it was created to do.
        /// </summary>
        /// <returns></returns>
        string GetDescription();
    }
}
