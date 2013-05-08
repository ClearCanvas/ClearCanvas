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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Provides domain object validation functionality.
	/// </summary>
	/// <remarks>
	/// Instances of this class are not thread-safe and should never be used by more than one thread.
	/// A corollary is that these objects are intended to be short-lived, not spanning more than a
	/// single request.
	/// </remarks>
	public class DomainObjectValidator : IDomainObjectValidator
	{
		private class NullDomainObjectValidator : IDomainObjectValidator
		{
			#region Implementation of IDomainObjectValidator

			public void Validate(object obj)
			{
			}

			public void ValidateRequiredFieldsPresent(object obj)
			{
			}

			public void ValidateLowLevel(object obj, Predicate<ISpecification> ruleFilter)
			{
			}

			public void ValidateHighLevel(object obj)
			{
			}

			#endregion
		}

		/// <summary>
		/// Gets an instance of <see cref="IDomainObjectValidator"/> that does nothing.
		/// </summary>
		public static IDomainObjectValidator NullValidator = new NullDomainObjectValidator();



		[Flags]
		enum RuleLevel
		{
			Low = 0x01,
			High = 0x10
		}

		private readonly Dictionary<Type, ValidationRuleSet> _lowLevelRuleSets = new Dictionary<Type, ValidationRuleSet>();
		private readonly Dictionary<Type, ValidationRuleSet> _highLevelRuleSets = new Dictionary<Type, ValidationRuleSet>();

		#region Public API


		/// <summary>
		/// Checks whether validation is enabled on the specified domain class.
		/// </summary>
		/// <param name="domainClass"></param>
		/// <returns></returns>
		public static bool IsValidationEnabled(Type domainClass)
		{
			// then check the attributes
			var a = AttributeUtils.GetAttribute<ValidationAttribute>(domainClass, true);

			// if no attribute present, then by default validation is enabled
			return (a == null) || a.EnableValidation;
		}

		/// <summary>
		/// Validates the specified domain object, applying all known validation rules.
		/// </summary>
		/// <param name="obj"></param>
		/// <exception cref="EntityValidationException">Validation failed.</exception>
		public void Validate(object obj)
		{
			// validate all rules
			Validate(obj, RuleLevel.Low|RuleLevel.High, rule => true);
		}

		/// <summary>
		/// Validates only that the specified object has required fields set.
		/// </summary>
		/// <param name="obj"></param>
		public void ValidateRequiredFieldsPresent(object obj)
		{
			ValidateLowLevel(obj, rule => rule is RequiredSpecification);
		}


		/// <summary>
		/// Validates the specified domain object, applying only "low-level" rules, subject to the specified filter.
		/// </summary>
		/// <remarks>
		/// Low-level rules are:
		/// 1. Required fields.
		/// 2. String field lengths.
		/// 3. Unique constraints.
		/// </remarks>
		/// <param name="obj"></param>
		/// <param name="ruleFilter"></param>
		public void ValidateLowLevel(object obj, Predicate<ISpecification> ruleFilter)
		{
			Validate(obj, RuleLevel.Low, ruleFilter);
		}

		/// <summary>
		/// Validates the specified domain object, applying only high-level rules.
		/// </summary>
		/// <remarks>
		/// High-level rules include any rules that are not low-level rules.
		/// </remarks>
		/// <param name="obj"></param>
		public void ValidateHighLevel(object obj)
		{
			Validate(obj, RuleLevel.High, r => true);
		}

		#endregion

		/// <summary>
		/// Validates the specified domain object, ignoring any rules that do not satisfy the filter.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="level"></param>
		/// <param name="ruleFilter"></param>
		/// <exception cref="EntityValidationException">Validation failed.</exception>
		private void Validate(object obj, RuleLevel level, Predicate<ISpecification> ruleFilter)
		{
			var domainClass = obj.GetClass();

			var ruleSets = new List<ISpecification>();

			if (CheckLevel(level, RuleLevel.Low))
			{
				// first check for a cached rule-set
				ValidationRuleSet lowLevelRules;
				if (!_lowLevelRuleSets.TryGetValue(domainClass, out lowLevelRules))
				{
					// otherwise build it
					lowLevelRules = IsValidationEnabled(domainClass) ?
						ValidationRuleSetCache.GetLowLevelRules(domainClass)
						: new ValidationRuleSet();

					// cache for future use
					_lowLevelRuleSets.Add(domainClass, lowLevelRules);
				}
				ruleSets.Add(lowLevelRules);
			}

			if (CheckLevel(level, RuleLevel.High))
			{
				// first check for a cached rule-set
				ValidationRuleSet highLevelRules;
				if (!_highLevelRuleSets.TryGetValue(domainClass, out highLevelRules))
				{
					// otherwise build it
						highLevelRules = IsValidationEnabled(domainClass) ?
							ValidationRuleSetCache.GetHighLevelRules(domainClass)
						: new ValidationRuleSet();

						// cache for future use
						_highLevelRuleSets.Add(domainClass, highLevelRules);
					}
					ruleSets.Add(highLevelRules);
				}

			var rules = new ValidationRuleSet(ruleSets);
			var result = rules.Test(obj, ruleFilter);
			if (result.Fail)
			{
				var message = string.Format(SR.ExceptionInvalidEntity, TerminologyTranslator.Translate(obj.GetClass()));
				throw new EntityValidationException(message, result.Reasons);
			}
		}

		private static bool CheckLevel(RuleLevel level, RuleLevel refLevel)
		{
			return (level & refLevel) == refLevel;
		}
	}
}
