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
    /// Attribute used to mark a class as defining an extension point.
    /// </summary>
    /// <remarks>
    /// Use this attribute to mark a class as defining an extension point.  This attribute must only be
    /// applied to subclasses of <see cref="ExtensionPoint" />.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class ExtensionPointAttribute : Attribute
    {
        private string _name;
        private string _description;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        public ExtensionPointAttribute()
        {
        }

        /// <summary>
        /// A friendly name for the extension point.  
        /// </summary>
        /// <remarks>
		/// This is optional and may be supplied as a named parameter.
		/// </remarks>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

		/// <summary>
		/// A friendly description for the extension point.  
		/// </summary>
		/// <remarks>
		/// This is optional and may be supplied as a named parameter.
		/// </remarks>
		public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
