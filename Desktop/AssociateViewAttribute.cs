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
    /// Associates a view extension point with a "model" class.
    /// </summary>
    /// <remarks>
	/// The model class may be any class that participates in a model-view 
	/// relationship and defines an associated view extension point.
	/// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class AssociateViewAttribute : Attribute
    {
        private Type _viewExtensionPointType;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="viewExtensionPointType">The view extension point class.</param>
        public AssociateViewAttribute(Type viewExtensionPointType)
        {
            _viewExtensionPointType = viewExtensionPointType;
        }

        /// <summary>
        /// Gets the view extension point class.
        /// </summary>
        public Type ViewExtensionPointType
        {
            get { return _viewExtensionPointType; }
        }
    }
}
