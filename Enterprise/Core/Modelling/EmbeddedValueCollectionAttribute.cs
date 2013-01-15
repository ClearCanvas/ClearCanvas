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
using System.Collections;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// When applied to a collection-type property of a domain class, indicates that that property models
    /// an embedded collection of values as opposed to an association.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class EmbeddedValueCollectionAttribute : Attribute
    {
        private Type _elementType;

        public EmbeddedValueCollectionAttribute(Type elementType)
        {
            _elementType = elementType;
        }

        public Type ElementType
        {
            get { return _elementType; }
        }
    }
}
