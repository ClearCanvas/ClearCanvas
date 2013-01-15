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
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Specifies minimum and maximum allowable length of the value of a given string-typed property of an object.
	/// </summary>
	internal class LengthSpecification : SimpleInvariantSpecification
	{
		private readonly int _min;
		private readonly int _max;

		internal LengthSpecification(PropertyInfo property, int min, int max)
			: base(property)
		{
			_min = min;
			_max = max;
		}

		public override TestResult Test(object obj)
		{
			var value = GetPropertyValue(obj);

			// ignore null values
			if (value == null)
				return new TestResult(true);

			try
			{
				var text = (string)value;

				return text.Length >= _min && text.Length <= _max ? new TestResult(true) :
					new TestResult(false, new TestResultReason(GetMessage()));

			}
			catch (InvalidCastException e)
			{
				throw new SpecificationException(SR.ExceptionExpectedStringValue, e);
			}
		}

		private string GetMessage()
		{
			return string.Format(SR.RuleLength,
				TerminologyTranslator.Translate(this.Property), _min, _max);
		}
	}
}
