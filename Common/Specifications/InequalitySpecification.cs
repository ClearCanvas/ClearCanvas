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

namespace ClearCanvas.Common.Specifications
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// From MSDN IComparable.CompareTo docs: 
	/// "By definition, any object compares greater than a null reference, and two null references compare equal to each other."
	/// </remarks>
	public abstract class InequalitySpecification : ComparisonSpecification
	{
		private bool _inclusive;
		private readonly int _multiplier;

		internal InequalitySpecification(int multiplier)
		{
			_multiplier = multiplier;
		}

		public bool Inclusive
		{
			get { return _inclusive; }
			set { _inclusive = value; }
		}

		protected override bool CompareValues(object testValue, object refValue)
		{
			// two nulls compare equal
			if (testValue == null && refValue == null)
				return true;

			// if testValue is null, refValue is greater by definition
			if (testValue == null)
				return (_multiplier == -1);

			// if refValue is null, testValue is greater by definition
			if (refValue == null)
				return (_multiplier == 1);

			// need to perform a comparison - ensure IComparable is implemented
			if (!(testValue is IComparable))
				throw new SpecificationException("Test expression does not evaluate to an IComparable object");

			var x = (testValue as IComparable).CompareTo(refValue) * _multiplier;
			return x > 0 || x == 0 && _inclusive;
		}
	}
}
