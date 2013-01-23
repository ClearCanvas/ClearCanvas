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
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.Xml;

namespace ClearCanvas.Desktop.Validation
{
	/// <summary>
	/// Defines an extension point to allow external sources of validation rule sets.
	/// </summary>
	[ExtensionPoint]
	public class ValidationRuleSourceExtensionPoint : ExtensionPoint<IValidationRuleSource>
	{
	}


	/// <summary>
	/// Caches attribute and XML-based validation rules for application components.
	/// </summary>
	/// <remarks>
	/// All operations on this class are safe for use by multiple threads.
	/// </remarks>
	public class ValidationCache
	{
        //TODO (Phoenix5): #10730 - remove this when it's fixed.
        [ThreadStatic]
		private static ValidationCache _instance;

		/// <summary>
		/// Gets the singleton instance of this class.
		/// </summary>
		public static ValidationCache Instance
		{
			get
			{
				if (_instance == null)
					_instance = new ValidationCache();
				return _instance;
			}
		}


		private readonly Dictionary<Type, List<IValidationRule>> _rulesCache = new Dictionary<Type, List<IValidationRule>>();

		/// <summary>
		/// Constructor
		/// </summary>
		private ValidationCache()
		{
		}

		#region Public API

		/// <summary>
		/// Retrieves rules from cache, or builds rules if not cached.
		/// </summary>
		/// <param name="applicationComponentClass"></param>
		/// <returns></returns>
		public IList<IValidationRule> GetRules(Type applicationComponentClass)
		{
			// locking on the applicationComponentClass here prevents multiple instantiations of the same class
			// from building the same rule-set concurrently
			lock (applicationComponentClass)
			{
				List<IValidationRule> rules;

				// try to get it from the cache
				lock (_rulesCache)
				{
					if (_rulesCache.TryGetValue(applicationComponentClass, out rules))
						return rules;
				}

				// build the validation rules (do this outside of the lock, in case it is slow)
				rules = new List<IValidationRule>();
				rules.AddRange(ProcessAttributeRules(applicationComponentClass));
				rules.AddRange(ProcessExtensionRules(applicationComponentClass));
				rules.AddRange(ProcessCustomRules(applicationComponentClass));

				// cache the rules
				lock (_rulesCache)
				{
					_rulesCache[applicationComponentClass] = rules;
				}

				return rules;
			}
		}

		/// <summary>
		/// Invalidates the rules cache for the specified application component class, causing the rules
		/// to be re-compiled the next time <see cref="GetRules"/> is called.
		/// </summary>
		/// <param name="applicationComponentClass"></param>
		public void Invalidate(Type applicationComponentClass)
		{
			lock (_rulesCache)
			{
				if (_rulesCache.ContainsKey(applicationComponentClass))
					_rulesCache.Remove(applicationComponentClass);
			}
		}

		#endregion

		#region Helpers

		private static List<IValidationRule> ProcessCustomRules(Type applicationComponentClass)
		{
			// if not supported, there are no custom rules
			if (!XmlValidationManager.Instance.IsSupported)
				return new List<IValidationRule>();

			var compiler = new XmlValidationCompiler();
			var ruleNodes = XmlValidationManager.Instance.GetRules(applicationComponentClass);
			return CollectionUtils.Map(ruleNodes, (XmlElement node) => compiler.CompileRule(node));
		}

		private static List<IValidationRule> ProcessExtensionRules(Type applicationComponentClass)
		{
			var rules = from source in new ValidationRuleSourceExtensionPoint().CreateExtensions().Cast<IValidationRuleSource>()
						from rule in source.GetRules(applicationComponentClass)
						select rule;
			return rules.ToList();
		}

		private static List<IValidationRule> ProcessAttributeRules(Type applicationComponentClass)
		{
			var rules = new List<IValidationRule>();
			foreach (var property in applicationComponentClass.GetProperties())
			{
				foreach (ValidationAttribute a in property.GetCustomAttributes(typeof(ValidationAttribute), true))
				{
					rules.Add(a.CreateRule(property, new ResourceResolver(applicationComponentClass.Assembly)));
				}
			}

			var methods = applicationComponentClass.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (var method in methods)
			{
				foreach (ValidationMethodForAttribute attribute in method.GetCustomAttributes(typeof(ValidationMethodForAttribute), true))
				{
					rules.Add(attribute.CreateRule(method));
				}
			}

			return rules;
		}

		#endregion
	}
}
