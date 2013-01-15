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
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Encapsulates a set of validation rules.
	/// </summary>
	/// <remarks>
	/// Instances of this class are immutable.
	/// </remarks>
	public class ValidationRuleSet : IValidationRuleSet
	{
		class AlwaysTrue : ISpecification
		{
			public TestResult Test(object obj)
			{
				return new TestResult(true);
			}
		}


		private readonly List<ISpecification> _rules;
		private readonly ISpecification _applicabilityRule;

		/// <summary>
		/// Constructor
		/// </summary>
		public ValidationRuleSet()
			: this(new List<ISpecification>(), new AlwaysTrue())
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rules"></param>
		public ValidationRuleSet(IEnumerable<ISpecification> rules)
			: this(rules, new AlwaysTrue())
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rules"></param>
		/// <param name="applicabilityRule"></param>
		public ValidationRuleSet(IEnumerable<ISpecification> rules, ISpecification applicabilityRule)
		{
			_rules = new List<ISpecification>(rules);
			_applicabilityRule = applicabilityRule;
		}

		/// <summary>
		/// Gets a value indicating whether this rule set is empty.
		/// </summary>
		public bool IsEmpty { get { return _rules.Count == 0; } }

		/// <summary>
		/// Returns a new rule set representing the addition of this rule set and the other.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public ValidationRuleSet Add(ValidationRuleSet other)
		{
			return new ValidationRuleSet(new[] { this, other });
		}

		/// <summary>
		/// Returns a new rule set representing the addition of the specified rule sets.
		/// </summary>
		/// <param name="ruleSets"></param>
		/// <returns></returns>
		public static ValidationRuleSet Add(IList<ValidationRuleSet> ruleSets)
		{
			return new ValidationRuleSet(CollectionUtils.Map<ValidationRuleSet, ISpecification>(ruleSets, r => r));
		}

		#region ISpecification Members

		/// <summary>
		/// Tests all rules against the specified object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public TestResult Test(object obj)
		{
			return TestCore(obj, null);
		}

		#endregion

		#region IValidationRuleSet Members

		/// <summary>
		/// Tests the subset of rules (those that are selected by the filter) against the specified object.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public TestResult Test(object obj, Predicate<ISpecification> filter)
		{
			return TestCore(obj, filter);
		}

		#endregion

		/// <summary>
		/// Gets the list of rules contained in this rule set.
		/// </summary>
		internal IList<ISpecification> Rules
		{
			get { return _rules.AsReadOnly(); }
		}

		/// <summary>
		/// Gets the specification that indicates whether this rule-set is applicable to a given test object.
		/// </summary>
		internal ISpecification ApplicabilityRule
		{
			get { return _applicabilityRule; }
		}

		private TestResult TestCore(object obj, Predicate<ISpecification> filter)
		{
			Platform.CheckForNullReference(obj, "obj");

			// test applicability of this rule set - if it fails, this rule set is not applicable, hence the result of testing it is success
			if (_applicabilityRule.Test(obj).Fail)
				return new TestResult(true);

			// a null filter is a nop filter
			filter = filter ?? (x => true);

			// test every rule in the set of rules that is accepted by the filter
			var failureReasons = new List<TestResultReason>();
			foreach (var rule in _rules)
			{
				if (filter(rule))
				{
					// if the rule is itself a ruleset, then apply the filter recursively
					var result = (rule is IValidationRuleSet) ? (rule as IValidationRuleSet).Test(obj, filter) : rule.Test(obj);
					if (result.Fail)
					{
						failureReasons.AddRange(result.Reasons);
					}
				}
			}

			return new TestResult(failureReasons.Count == 0, failureReasons.ToArray());
		}
	}
}
