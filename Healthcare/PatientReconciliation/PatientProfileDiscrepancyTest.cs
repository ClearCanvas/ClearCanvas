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
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.PatientReconciliation
{
	/// <summary>
	/// Represents the result of evaluating a single discrepancy test on a pair of patient profiles
	/// </summary>
	public class DiscrepancyTestResult
	{
		private readonly PatientProfileDiscrepancy _discrepancy;
		private readonly bool _discrepant;
		private readonly StringDiff _diff;

		public DiscrepancyTestResult(PatientProfileDiscrepancy discrepancy, bool discrepant, StringDiff diff)
		{
			_discrepancy = discrepancy;
			_discrepant = discrepant;
			_diff = diff;
		}

		public PatientProfileDiscrepancy Discrepancy
		{
			get { return _discrepancy; }
		}

		public bool IsDiscrepant
		{
			get { return _discrepant; }
		}

		public StringDiff Diff
		{
			get { return _diff; }
		}
	}

	/// <summary>
	/// Utility class for comparing a set of <see cref="PatientProfile"/> objects for discrepancies.
	/// </summary>
	public class PatientProfileDiscrepancyTest
	{
		/// <summary>
		/// Returns a value that is a bitmask of <see cref="PatientProfileDiscrepancy"/> values indicating
		/// which discrepancies were found among the specified set of profiles.  Only the discrepancies specified in
		/// <paramref name="testableDiscrepancies"/> will be tested.
		/// </summary>
		/// <param name="x">The first profiles to test</param>
		/// <param name="y">The second profiles to test</param>
		/// <param name="testableDiscrepancies">A bitmask indicating the set of discrepancies to test for</param>
		/// <returns>A bitmask indicating the discrepancies found</returns>
		public static IList<DiscrepancyTestResult> GetDiscrepancies(PatientProfile x, PatientProfile y, PatientProfileDiscrepancy testableDiscrepancies)
		{
			var results = new List<DiscrepancyTestResult>();

			// Healthcard
			if ((testableDiscrepancies & PatientProfileDiscrepancy.Healthcard) == PatientProfileDiscrepancy.Healthcard)
			{
				results.Add(GetResult(x, y,
					PatientProfileDiscrepancy.Healthcard,
					p => p.Healthcard,
					Equals));
			}

			// FamilyName
			if ((testableDiscrepancies & PatientProfileDiscrepancy.FamilyName) == PatientProfileDiscrepancy.FamilyName)
			{
				results.Add(GetResult(x, y,
					PatientProfileDiscrepancy.FamilyName,
					p => p.Name.FamilyName,
					(a, b) => a.Equals(b, StringComparison.CurrentCultureIgnoreCase)));
			}

			// GivenName
			if ((testableDiscrepancies & PatientProfileDiscrepancy.GivenName) == PatientProfileDiscrepancy.GivenName)
			{
				results.Add(GetResult(x, y,
					PatientProfileDiscrepancy.GivenName,
					p => p.Name.GivenName,
					(a, b) => a.Equals(b, StringComparison.CurrentCultureIgnoreCase)));
			}

			// MiddleName
			if ((testableDiscrepancies & PatientProfileDiscrepancy.MiddleName) == PatientProfileDiscrepancy.MiddleName)
			{
				results.Add(GetResult(x, y,
					PatientProfileDiscrepancy.MiddleName,
					p => p.Name.MiddleName,
					(a, b) => a.Equals(b, StringComparison.CurrentCultureIgnoreCase)));
			}

			// DateOfBirth
			if ((testableDiscrepancies & PatientProfileDiscrepancy.DateOfBirth) == PatientProfileDiscrepancy.DateOfBirth)
			{
				results.Add(GetResult(x, y,
					PatientProfileDiscrepancy.DateOfBirth,
					p => p.DateOfBirth,
					(a, b) => Nullable.Compare(a, b) == 0,
					a => a == null ? null : a.Value.ToShortDateString()));
			}

			// Sex
			if ((testableDiscrepancies & PatientProfileDiscrepancy.Sex) == PatientProfileDiscrepancy.Sex)
			{
				results.Add(GetResult(x, y,
					PatientProfileDiscrepancy.Sex,
					p => p.Sex));
			}

			// HomePhone
			if ((testableDiscrepancies & PatientProfileDiscrepancy.HomePhone) == PatientProfileDiscrepancy.HomePhone)
			{
				results.Add(GetResult(x, y,
					PatientProfileDiscrepancy.HomePhone,
					p => p.CurrentHomePhone,
					(a, b) => a.IsSameNumber(b)));
			}

			// HomeAddress
			if ((testableDiscrepancies & PatientProfileDiscrepancy.HomeAddress) == PatientProfileDiscrepancy.HomeAddress)
			{
				results.Add(GetResult(x, y, 
					PatientProfileDiscrepancy.HomeAddress,
					p => p.CurrentHomeAddress,
					(a, b) => a.IsSameAddress(b)));
			}

			// WorkPhone
			if ((testableDiscrepancies & PatientProfileDiscrepancy.WorkPhone) == PatientProfileDiscrepancy.WorkPhone)
			{
				results.Add(GetResult(x, y, 
					PatientProfileDiscrepancy.WorkPhone,
					p => p.CurrentWorkPhone,
					(a, b) => a.IsSameNumber(b)));
			}

			// WorkAddress
			if ((testableDiscrepancies & PatientProfileDiscrepancy.WorkAddress) == PatientProfileDiscrepancy.WorkAddress)
			{
				results.Add(GetResult(x, y, 
					PatientProfileDiscrepancy.WorkAddress,
					p => p.CurrentWorkAddress,
					(a, b) => a.IsSameAddress(b)));
			}

			return results;
		}

		delegate T PropertyGetter<T>(PatientProfile p);
		delegate bool TestEqual<T>(T x, T y);
		delegate string ToStringDelegate<T>(T x);

		/// <summary>
		/// Computes a <see cref="DiscrepancyTestResult"/> for a specified property
		/// </summary>
		/// <typeparam name="T">The type of the property being tested</typeparam>
		/// <param name="x">Left operand</param>
		/// <param name="y">Right operand</param>
		/// <param name="discrepancy">Discrepancy being tested</param>
		/// <param name="getter">A delegate that returns the value of the property from a <see cref="PatientProfile"/></param>
		/// <param name="tester">A delegate that tests for equality of the property - need not be null-safe</param>
		/// <param name="toString">A delegate that converts the property to a string</param>
		/// <returns></returns>
		private static DiscrepancyTestResult GetResult<T>(PatientProfile x, PatientProfile y, PatientProfileDiscrepancy discrepancy, PropertyGetter<T> getter, TestEqual<T> tester, ToStringDelegate<T> toString)
		{
			var vx = getter(x);
			var vy = getter(y);

			if (Equals(vx, default(T)) && Equals(vy, default(T)))
			{
				return new DiscrepancyTestResult(discrepancy, false, StringDiff.Compute("", "", true));
			}

			if (Equals(vx, default(T)))
			{
				return new DiscrepancyTestResult(discrepancy, true, StringDiff.Compute("", toString(vy), true));
			}

			if (Equals(vy, default(T)))
			{
				return new DiscrepancyTestResult(discrepancy, true, StringDiff.Compute(toString(vx), "", true));
			}

			return new DiscrepancyTestResult(discrepancy, !tester(vx, vy), StringDiff.Compute(toString(vx), toString(vy), true));
		}

		private static DiscrepancyTestResult GetResult<T>(PatientProfile x, PatientProfile y, PatientProfileDiscrepancy discrepancy, PropertyGetter<T> propGetter, TestEqual<T> tester)
		{
			return GetResult(x, y, discrepancy, propGetter, tester, DefaultToString);
		}

		private static DiscrepancyTestResult GetResult<T>(PatientProfile x, PatientProfile y, PatientProfileDiscrepancy discrepancy, PropertyGetter<T> propGetter)
		{
			return GetResult(x, y, discrepancy, propGetter, DefaultTestEqual, DefaultToString);
		}

		private static bool DefaultTestEqual<T>(T x, T y)
		{
			return x.Equals(y);
		}

		private static string DefaultToString<T>(T x)
		{
			return x.ToString();
		}

	}
}
