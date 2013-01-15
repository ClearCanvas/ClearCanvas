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
using System.Reflection;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// Identifies a rule as being logically bound to one or more properties of an object. In other words,
    /// the rule is strictly a function of the values of the specified properties, and is not a function of
    /// anything else.
    /// </summary>
    public interface IPropertyBoundRule
    {
        /// <summary>
        /// Gets an array of the properties of which this rule is a function.
        /// </summary>
        PropertyInfo[] Properties { get; }
    }
}
