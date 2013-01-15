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

using System.Reflection;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Specifies that a given property of an object is unique within the set of persistent instances of that object's class.
	/// </summary>
	/// <remarks>
	/// This class is similar to <see cref="UniqueKeySpecification"/>, except that the unique key is limited to a single
	/// primitive-valued property.  However, this limitation allows this class to implement <see cref="IPropertyBoundRule"/>
	/// since the key value is a function of a single property.
	/// </remarks>
	internal class UniqueSpecification : UniqueKeySpecification, IPropertyBoundRule
	{
		private readonly PropertyInfo _property;

		internal UniqueSpecification(PropertyInfo property)
			: base(property.DeclaringType, property.Name, new[] { property.Name })
		{
			_property = property;
		}

		#region IPropertyBoundRule Members

		public PropertyInfo[] Properties
		{
			get { return new[] { _property }; }
		}

		#endregion
	}
}
