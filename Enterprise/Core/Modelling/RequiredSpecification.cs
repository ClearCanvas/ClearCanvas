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

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Specifies that a given property of an object is required to have a value (e.g. non-null).
	/// </summary>
	internal class RequiredSpecification : SimpleInvariantSpecification
	{
		internal RequiredSpecification(PropertyInfo property)
			: base(property)
		{
		}

		public override TestResult Test(object obj)
		{
			var value = GetPropertyValue(obj);

			// special consideration for strings - empty string should be considered "null"
			var isNull = (value is string) ? string.IsNullOrEmpty((string)value) : value == null;

			return isNull ? new TestResult(false, new TestResultReason(GetMessage())) : new TestResult(true);
		}

		private string GetMessage()
		{
			return string.Format(SR.RuleRequired, TerminologyTranslator.Translate(this.Property));
		}
	}
}
