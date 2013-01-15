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

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// When applied to an entity class, indicates that a specified set of properties on the class
    /// form a unique key for instances of that class within the set of persistent instances.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class UniqueKeyAttribute : Attribute
    {
        private readonly string[] _memberProperties;
        private readonly string _logicalName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logicalName">The logical name of the key.</param>
        /// <param name="memberProperties">
        /// An array of property names that form the unique key for the class.  For example, a Person class
        /// might have a unique key consisting of "FirstName" and "LastName" properties.  Note that compound
        /// property expressions may be used, e.g. for a Person class with a Name property that itself has First
        /// and Last properties, the unique key members might be "Name.First" and "Name.Last".
        /// </param>
        public UniqueKeyAttribute(string logicalName, string[] memberProperties)
        {
            _logicalName = logicalName;
            _memberProperties = memberProperties;
        }

        public string[] MemberProperties
        {
            get { return _memberProperties; }
        }

        public string LogicalName
        {
            get { return _logicalName; }
        }
    }
}
