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
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Base class for rules that represent simple invariant constraints on a property of an object.
	/// </summary>
	internal abstract class SimpleInvariantSpecification : ISpecification, IPropertyBoundRule
	{
		private readonly PropertyInfo[] _properties;

		protected SimpleInvariantSpecification(PropertyInfo[] properties)
		{
			_properties = properties;
		}

		protected SimpleInvariantSpecification(PropertyInfo property)
		{
			_properties = new [] { property };
		}


		#region ISpecification Members

		public abstract TestResult Test(object obj);

		#endregion


		public PropertyInfo[] Properties
		{
			get { return _properties; }
		}

		public PropertyInfo Property
		{
			get { return _properties[0]; }
		}

		protected object GetPropertyValue(object obj)
		{

			return GetPropertyValue(obj, _properties[0]);
		}

		protected object[] GetPropertyValues(object obj)
		{
			return CollectionUtils.Map(_properties, (PropertyInfo property) => GetPropertyValue(obj, property)).ToArray();
		}

		private static object GetPropertyValue(object obj, PropertyInfo property)
		{
			return property.GetGetMethod().Invoke(obj, null);
		}
	}
}
