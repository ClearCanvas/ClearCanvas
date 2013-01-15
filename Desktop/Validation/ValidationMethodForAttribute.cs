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
using System.Reflection;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Validation
{
	/// <summary>
	/// Thrown by <see cref="ValidationMethodForAttribute"/>.
	/// </summary>
	public class ValidationMethodForAttributeException : Exception
	{
		internal ValidationMethodForAttributeException(string message)
			: base(message)
		{
		}
	}

	/// <summary>
	/// Attribute used to decorate a method as a validation method.
	/// </summary>
	/// <remarks>
	/// The property matching <see cref="PropertyName"/> will be validated
	/// via the decorated method.  The method must match the signature of
	/// <see cref="ValidationMethod"/>.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class ValidationMethodForAttribute : Attribute
	{
		/// <summary>
		/// Defines the method signature for methods decorated with <see cref="ValidationMethodForAttribute"/>.
		/// </summary>
		public delegate ValidationResult ValidationMethod();

		private readonly string _propertyName;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">The property the decorated method is intended to validate.</param>
		public ValidationMethodForAttribute(string propertyName)
		{
			_propertyName = propertyName;
		}

		/// <summary>
		/// Identifies the property whose validation will be done via the decorated method.
		/// </summary>
		public string PropertyName
		{
			get { return _propertyName; }	
		}

		private static void CheckMethodSignature(MethodInfo method)
		{
			if (!(typeof(ValidationResult).IsAssignableFrom(method.ReturnType)))
				throw new ValidationAttributeException("The decorated method does not have the correct signature.");

			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length != 0)
				throw new ValidationAttributeException("The decorated method does not have the correct signature.");
		}

		/// <summary>
		/// Factory method that creates an <see cref="IValidationRule"/> for 
		/// the property with the name <see cref="PropertyName"/>.
		/// </summary>
		internal IValidationRule CreateRule(MethodInfo method)
		{
			CheckMethodSignature(method);

			return new ValidationRule(_propertyName,
			                          delegate(IApplicationComponent component)
			                          	{
			                          		return (ValidationResult)method.Invoke(component, null);
			                          	});
		}
	}
}
