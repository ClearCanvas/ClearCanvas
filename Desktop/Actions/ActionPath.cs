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
    /// A subclass of <see cref="Path"/> that is used by <see cref="IAction"/> to represent an action path.
    /// </summary>
    public class ActionPath : Path
    {
        /// <summary>
        /// Constructs an action path from the specified path string, using the specified resource resolver.
        /// </summary>
        /// <param name="pathString">A string respresenting the path.</param>
        /// <param name="resolver">A resource resolver used to localize each path segment. If
        /// the resource resolver is null, the path segments will be treated as localized text.</param>
        public ActionPath(string pathString, IResourceResolver resolver)
            :base(pathString, resolver)
        {
        }

        /// <summary>
        /// Gets the action site (the first segment of the action path).
        /// </summary>
        public string Site
        {
            get { return this.Segments.Count > 0 ? this.Segments[0].ResourceKey : null; }
        }
    }
}
