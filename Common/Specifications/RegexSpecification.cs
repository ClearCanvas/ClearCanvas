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

using System.Text.RegularExpressions;

namespace ClearCanvas.Common.Specifications
{
    public class RegexSpecification : PrimitiveSpecification
    {
        private readonly string _pattern;
    	private readonly bool _ignoreCase = true;	// true by default
    	private readonly bool _nullMatches;

        public RegexSpecification(string pattern, bool ignoreCase, bool nullMatches)
        {
			Platform.CheckForNullReference(pattern, "pattern");
			Platform.CheckForEmptyString(pattern, "pattern");

            _pattern = pattern;
            _ignoreCase = ignoreCase;
        	_nullMatches = nullMatches;
        }

		public RegexSpecification(string pattern)
			:this(pattern, true, false)
		{
		}

    	public string Pattern
    	{
			get { return _pattern; }
    	}

    	public bool IgnoreCase
    	{
			get { return _ignoreCase; }
    	}

    	public bool NullMatches
    	{
			get { return _nullMatches; }
    	}

        protected override TestResult InnerTest(object exp, object root)
        {
            if (exp == null)
				return DefaultTestResult(_nullMatches);

            if (exp is string)
            {
                if (_ignoreCase)
                    return DefaultTestResult(Regex.Match(exp as string, _pattern, RegexOptions.IgnoreCase).Success);
                else
                    return DefaultTestResult(Regex.Match(exp as string, _pattern).Success);
            }
            else
            {
                throw new SpecificationException(SR.ExceptionCastExpressionString);
            }
        }
    }
}
