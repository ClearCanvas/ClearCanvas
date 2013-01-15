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

namespace ClearCanvas.Desktop.Validation
{
	/// <summary>
	/// Validates that a property is not null.
	/// </summary>
	public class ValidateNotNullAttribute : ValidationAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ValidateNotNullAttribute()
		{
		}

		/// <summary>
		/// Factory method to create an <see cref="IValidationRule"/> based on this attribute.
		/// </summary>
		/// <param name="property">The property on which the attribute is applied.</param>
		/// <param name="getter">A delegate that, when invoked, returns the current value of the property.</param>
		/// <param name="customMessage">A custom message to be displayed, or null if none was supplied.</param>
		/// <returns></returns>
		protected override IValidationRule CreateRule(PropertyInfo property, PropertyGetter getter, string customMessage)
		{
			string message = customMessage ?? SR.MessageValueRequired;
			return new ValidationRule(property.Name,
			   delegate(IApplicationComponent component)
			   {
				   object value = getter(component);
				   return new ValidationResult((value is string) ? !string.IsNullOrEmpty(value as string) : value != null, message);
			   });
		}
	}
}
