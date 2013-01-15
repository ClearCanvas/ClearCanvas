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
    /// An abstract base class for extension filters.  
    /// </summary>
    /// <remarks>
	/// Extension filters are used to filter the extension points returned by 
	/// one of the <b>CreateExtensions</b> methods.  Subclasses of this
	/// class implement specific types of filters.
	/// </remarks>
    public abstract class ExtensionFilter
    {
        /// <summary>
        /// Tests the specified extension against the criteria of this filter.
        /// </summary>
        /// <param name="extension">The extension to test.</param>
        /// <returns>True if the extension meets the criteria, false otherwise.</returns>
        public abstract bool Test(ExtensionInfo extension);
    }
}
