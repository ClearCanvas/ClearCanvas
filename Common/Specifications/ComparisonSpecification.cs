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

#pragma warning disable 1591

using System;
using System.Globalization;

namespace ClearCanvas.Common.Specifications
{
	public abstract class ComparisonSpecification : PrimitiveSpecification
	{
		private Expression _refValueExpr;
		private bool _strict;

		public Expression RefValueExpression
		{
			get { return _refValueExpr; }
			set { _refValueExpr = value; }
		}

		public bool Strict
		{
			get { return _strict; }
			set { _strict = value; }
		}

		protected override TestResult InnerTest(object exp, object root)
		{
			if (_refValueExpr == null)
				throw new SpecificationException("Reference value required.");

			var refValue = _refValueExpr.Evaluate(root);

			// bug #3279:if the refValue is of a different type than the test expression, 
			// attempt to coerce the refValue to the same type
			// if the coercion fails, the comparison will be performed on the raw refValue
			if (!_strict && exp != null && refValue != null && (exp.GetType() != refValue.GetType()))
			{
				// try to coerce the reference value and the expression value to the same type
				TryCoerceToSameType(ref refValue, ref exp);
			}

			return DefaultTestResult(CompareValues(exp, refValue));
		}

		protected abstract bool CompareValues(object testValue, object refValue);

		private static void TryCoerceToSameType(ref object x, ref object y)
		{
			// if either value is null, nothing to do here
			if (x == null || y == null)
				return;

			// if both values are already the same type, don't change the types!
			if (x.GetType() == y.GetType())
				return;

			// try coercion both ways, and hope that one of the two will succeed
			if (TryCoerce(ref x, y.GetType()))
				return;

			TryCoerce(ref y, x.GetType());
		}

		private static bool TryCoerce(ref object value, Type type)
		{
			// special case: enum values cannot be parsed by Convert.ChangeType
			if (type.IsEnum && value is string)
			{
				try
				{
					value = Enum.Parse(type, (string)value, true);
					return true;
				}
				catch (ArgumentException)
				{
					// unable to parse - "value" is not modified
					return false;
				}
			}

			// special case: prohibit converting a non-string to a string
			// for example, if comparing float 1.0 to string "1.00", it does not make sense to
			// convert both to strings and compare strings, because the strings "1.0" and "1.00" are not equal.
			if (type == typeof(string) && !(value is string))
				return false;

			// special case: prohibit lossy casts, which Convert.ChangeType will happily do
			if (IsIntegralType(type) && IsFloatingPointType(value.GetType()))
				return false;

			// special case: in order to be able to coerce a string such as "1.0" to an integral data type,
			// we need to first explicitly parse it into a integral value
			if(IsIntegralType(type) && value is string)
			{
				// try to parse value as a number... 
				// whether or not this succeeds or fails, we carry on and let Convert.ChangeType have a go
				TryParseNumberString(ref value);
			}

			try
			{
				value = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
				return true;
			}
			catch (InvalidCastException)
			{
				// unable to convert - "value" is not modified
				return false;
			}
			catch (FormatException)
			{
				// unable to convert - "value" is not modified
				return false;
			}
		}

		private static void TryParseNumberString(ref object x)
		{
			if (!(x is string))
				return;

			long l;
			if (long.TryParse((string)x, NumberStyles.Any, CultureInfo.InvariantCulture, out l))
			{
				x = l;
				return;
			}

			ulong ul;
			if (ulong.TryParse((string)x, NumberStyles.Any, CultureInfo.InvariantCulture, out ul))
				x = ul;
		}

		private static bool IsIntegralType(Type type)
		{
			return type == typeof (short) || type == typeof (int) || type == typeof (long)
			       || type == typeof (ushort) || type == typeof (uint) || type == typeof (ulong)
				   || type == typeof (byte) || type == typeof (sbyte);
		}

		private static bool IsFloatingPointType(Type type)
		{
			return type == typeof (float) || type == typeof (double) || type == typeof (decimal);
		}
	}
}
