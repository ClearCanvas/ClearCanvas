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

namespace ClearCanvas.Common
{
    /// <summary>
    /// Used by framework to provide a consistent interface for browsable meta-data objects.
    /// </summary>
    public interface IBrowsable
    {
        /// <summary>
        /// Formal name of this object, typically the type name or assembly name.  Cannot be null.
        /// </summary>
        string FormalName
        {
            get;
        }

        /// <summary>
        /// Friendly name of the object, if one exists, otherwise null.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// A friendly description of this object, if one exists, otherwise null.
        /// </summary>
        string Description
        {
            get;
        }
    }
}
