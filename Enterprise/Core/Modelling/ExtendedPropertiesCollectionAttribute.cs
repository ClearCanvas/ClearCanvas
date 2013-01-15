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
	/// When applied to a property of a domain object class, indicates that the property
	/// represents a collection of extended properties.
	/// </summary>
	/// <remarks>
	/// This attribute only makes sense when applied to properties of type IDictionary{string,TValue}.
	/// In other words, the property must be a map of strings to some type of value.  The keys are
	/// considered to be extended "properties" of the object that owns the collection.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class ExtendedPropertiesCollectionAttribute : Attribute
	{
	}
}
