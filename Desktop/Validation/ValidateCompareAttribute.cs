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

namespace ClearCanvas.Desktop.Validation
{
	/// <summary>
	/// Abstract base class for comparison validations.
	/// </summary>
	public abstract class ValidateCompareAttribute : ValidationAttribute
	{
		private string _referenceProperty;
		private object _referenceValue;
		private bool _inclusive;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="referenceProperty">The name of another property to compare against.</param>
		protected ValidateCompareAttribute(string referenceProperty)
		{
			_referenceProperty = referenceProperty;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="referenceValue">A value to compare against.</param>
		protected ValidateCompareAttribute(int referenceValue)
		{
			_referenceValue = referenceValue;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="referenceValue">A value to compare against.</param>
		protected ValidateCompareAttribute(float referenceValue)
		{
			_referenceValue = referenceValue;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="referenceValue">A value to compare against.</param>
		protected ValidateCompareAttribute(double referenceValue)
		{
			_referenceValue = referenceValue;
		}

		/// <summary>
		/// Gets or sets whether or not the comparison should be inclusive.
		/// </summary>
		public bool Inclusive
		{
			get { return _inclusive; }
			set { _inclusive = value; }
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
			CheckPropertyIsType(property, typeof(IComparable));
			if (_referenceProperty != null)
			{
				// check that reference property is same type as validating property
				PropertyInfo refProp = property.DeclaringType.GetProperty(_referenceProperty);
				if (refProp.PropertyType != property.PropertyType)
					throw new ValidationAttributeException(SR.ExceptionReferencePropertyMustBeOfSameType);

				PropertyGetter refPropGetter = CreatePropertyGetter(refProp);
				return new ValidationRule(property.Name,
					delegate(IApplicationComponent c)
					{
						IComparable value = (IComparable)getter(c);
						object refValue = refPropGetter(c);
						return new ValidationResult(Compare(value, refValue, _inclusive), customMessage ?? GetDefaultMessage(refValue));
					});
			}
			else
			{
				// check that reference value is same type as validating property
				if (_referenceValue.GetType() != property.PropertyType)
					throw new ValidationAttributeException(SR.ExceptionReferenceValueMustBeOfSameType);

				// compare against fixed reference value
				return new ValidationRule(property.Name,
					delegate(IApplicationComponent c)
					{
						IComparable value = (IComparable)getter(c);
						return new ValidationResult(Compare(value, _referenceValue, _inclusive), customMessage ?? GetDefaultMessage(_referenceValue));
					});
			}
		}

		private bool Compare(IComparable x, object y, bool inclusive)
		{
			int i = GetCompareSign() * x.CompareTo(y);
			return i == 1 || (inclusive && i == 0);
		}

		private string GetDefaultMessage(object referenceValue)
		{
			if (GetCompareSign() > 0)
			{
				return _inclusive ?
					string.Format(SR.FormatMustBeGreaterThanOrEqualTo, referenceValue) :
					string.Format(SR.FormatMustBeGreaterThan, referenceValue);
			}
			else
			{
				return _inclusive ?
				   string.Format(SR.FormatMustBeLessThanOrEqualTo, referenceValue) :
				   string.Format(SR.FormatMustBeLessThan, referenceValue);
			}
		}

		/// <summary>
		/// Gets the sign of the comparison.
		/// </summary>
		protected abstract int GetCompareSign();
	}
}
