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
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Used by <see cref="IUpdateContext.Lock"/> to indicate whether an entity should be considered clean or dirty.
    /// </summary>
    public enum DirtyState
    {
        /// <summary>
        /// Treat the entity as clean
        /// </summary>
        Clean,

        /// <summary>
        /// Treat the entity as dirty
        /// </summary>
        Dirty,

        /// <summary>
        /// Treat the entity as new (unsaved)
        /// </summary>
        New
    }
}
