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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Caching;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Defines an extension point for extensions that act as sources of validation rules.
	/// </summary>
	[ExtensionPoint]
	public class EntityValidationRuleSetSourceExtensionPoint : ExtensionPoint<IValidationRuleSetSource>
	{
	}


	/// <summary>
	/// Manages validation rules sets for domain object classes.
	/// </summary>
	/// <remarks>
	/// All methods on this class are safe for concurrent access by multiple threads.
	/// </remarks>
	internal static class ValidationRuleSetCache
	{
		private const string CacheId = "ClearCanvas.Enterprise.Core.Modelling.EntityValidationRuleSetCache";
		private const string CacheRegion = "default";

		/// <summary>
		/// Low-level rulesets are cached in a static variable, since they cannot change during the lifetime of the process.
		/// </summary>
		private static readonly Dictionary<Type, ValidationRuleSet> _lowLevelRuleSets = new Dictionary<Type, ValidationRuleSet>();

		/// <summary>
		/// Gets the low-level rule-set (rules for things like required fields, unique constraints, field lengths).
		/// </summary>
		/// <param name="domainClass"></param>
		/// <returns></returns>
		internal static ValidationRuleSet GetLowLevelRules(Type domainClass)
		{
			lock (_lowLevelRuleSets)
			{
				ValidationRuleSet rules;

				// return cached rules if possible
				if (_lowLevelRuleSets.TryGetValue(domainClass, out rules))
					return rules;

				// build rules for domainClass
				var builder = new ValidationBuilder(domainClass);
				rules = builder.LowLevelRules;

				// cache for future use
				_lowLevelRuleSets.Add(domainClass, rules);
				return rules;
			}
		}

		/// <summary>
		/// Gets the high-level rule-set, which may include both hard-coded high-level rules,
		/// and custom rules specified in XML.
		/// </summary>
		/// <param name="domainClass"></param>
		/// <returns></returns>
		internal static ValidationRuleSet GetHighLevelRules(Type domainClass)
		{
			// get the static rules
			var builder = new ValidationBuilder(domainClass);
			var staticRules = builder.HighLevelRules;

			// get the custom rules
			// Because custom rules may potentially be modified by an administrator, 
			// they are cached using the Cache object with an absolute expiry time,
			// which should be relatively short (a few minutes).
			// This ensures that changes made to these rules will be applied eventually, when the cache expires.
			ValidationRuleSet customRules;
			if (Cache.IsSupported())
			{
				using (var cacheClient = Cache.CreateClient(CacheId))
				{
					// check the cache for a compiled ruleset
					customRules = (ValidationRuleSet)cacheClient.Get(domainClass.FullName, new CacheGetOptions(CacheRegion));
					if (customRules == null)
					{
						// no cached, so compile the ruleset from source
						customRules = BuildCustomRules(domainClass);

						// cache the ruleset if desired
						var settings = new EntityValidationSettings();
						var ttl = TimeSpan.FromSeconds(settings.CustomRulesCachingTimeToLiveSeconds);
						if (ttl > TimeSpan.Zero)
						{
							cacheClient.Put(domainClass.FullName, customRules, new CachePutOptions(CacheRegion, ttl, false));
						}
					}
				}
			}
			else
			{
				// if the Cache object is not available in this environment, then we have no choice but to build from source
				Platform.Log(LogLevel.Warn, "Caching of custom rules is not supported in this configuration - rules are being compiled from source.");
				customRules = BuildCustomRules(domainClass);
			}
			return new ValidationRuleSet(new[] { staticRules, customRules });
		}

		/// <summary>
		/// Builds the custom rule-set (rules that are specified in an XML format).
		/// </summary>
		/// <param name="domainClass"></param>
		/// <returns></returns>
		private static ValidationRuleSet BuildCustomRules(Type domainClass)
		{
			try
			{
				// combine rules from all non-static (i.e. custom) sources
				var sources = new EntityValidationRuleSetSourceExtensionPoint().CreateExtensions();
				var rules = from source in sources.Cast<IValidationRuleSetSource>()
							where !source.IsStatic
							let r = source.GetRuleSet(domainClass)
							where !r.IsEmpty
							select r;
				return new ValidationRuleSet(rules);
			}
			catch (Exception e)
			{
				// any exceptions here must be logged and ignored, as their is really no good way to handle them
				Platform.Log(LogLevel.Error, e, "Error attempting to compile custom validation rules");
				return new ValidationRuleSet();
			}
		}

	}
}
