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

namespace ClearCanvas.Common
{
    /// <summary>
    /// Implements an extension filter that performs matching on types.
    /// </summary>
    /// <remarks>
    /// The filter will test true if the extension in question implements all of the
    /// types supplied as criteria to this filter.  Typically these types are interfaces, however, a
    /// single class may be supplied, in which case the extension must be a subclass of that class.
    /// </remarks>
    public class TypeExtensionFilter : ExtensionFilter
    {
        private Type[] _types;

        /// <summary>
        /// Creates a filter that matches on multiple types.
        /// </summary>
        /// <param name="types">The types used as criteria to match.</param>
        public TypeExtensionFilter(Type[] types)
        {
            _types = types;
        }

        /// <summary>
        /// Creates a filter that matches on a single type.
        /// </summary>
        /// <param name="type">The type used as criteria to match.</param>
        public TypeExtensionFilter(Type type)
            : this(new Type[] { type })
        {
        }

        /// <summary>
        /// Checks whether the specified extension implements/subclasses all of the criteria types.
        /// </summary>
        /// <param name="extension">The extension to test.</param>
        /// <returns>True if the test succeeds.</returns>
        public override bool Test(ExtensionInfo extension)
        {
            foreach (Type filterType in _types)
            {
                if (!filterType.IsAssignableFrom(extension.ExtensionClass.Resolve()))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
