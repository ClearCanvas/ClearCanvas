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

            object refValue = _refValueExpr.Evaluate(root);

			// bug #3279:if the refValue is of a different type than the test expression, 
			// attempt to coerce the refValue to the same type
			// if the coercion fails, the comparison will be performed on the raw refValue
			if(!_strict && exp != null && refValue != null && exp.GetType() != refValue.GetType())
			{
				// try to coerce the reference value to the expression type
				var success = TryCoerce(ref refValue, exp.GetType());

				// bug #5909: if that didn't work, try coercing the expression value to the reference value type
				if(!success)
				{
					TryCoerce(ref exp, refValue.GetType());
				}

				// if neither of the above worked, then we just proceed to compare the raw values
			}

            return DefaultTestResult(CompareValues(exp, refValue));
        }

        protected abstract bool CompareValues(object testValue, object refValue);

		private static bool TryCoerce(ref object value, Type type)
		{
			try
			{
				value = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
				return true;
			}
			catch (InvalidCastException)
			{
				// unable to cast - "value" is not modified
				return false;
			}
		}
    }
}
