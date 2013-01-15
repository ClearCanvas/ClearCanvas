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
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Utility class for building a <see cref="ValidationRuleSet"/> based on attributes defined on an entity class.
	/// </summary>
	internal class ValidationBuilder
	{
		class AttributeEntityClassPair
		{
			public Type DeclaringClass { get; set; }
			public Attribute Attribute { get; set; }
		}

		private readonly List<ISpecification> _lowLevelRules = new List<ISpecification>();
		private readonly List<ISpecification> _highLevelRules = new List<ISpecification>();
		private readonly Type _entityClass;
		private bool _processed;

		public ValidationBuilder(Type entityClass)
		{
			_entityClass = entityClass;
		}


		#region Public API

		public ValidationRuleSet HighLevelRules
		{
			get
			{
				if (!_processed)
				{
					BuildRuleSet();
				}
				return new ValidationRuleSet(_highLevelRules);
			}
		}

		public ValidationRuleSet LowLevelRules
		{
			get
			{
				if (!_processed)
				{
					BuildRuleSet();
				}
				return new ValidationRuleSet(_lowLevelRules);
			}
		}

		#endregion

		/// <summary>
		/// Builds a set of validation rules by processing the attributes defined on the specified entity class.
		/// </summary>
		/// <returns></returns>
		public void BuildRuleSet()
		{
			ProcessClassProperties();

			// process class-level attributes
			foreach (var pair in GetClassAttributes(_entityClass))
			{
				ProcessEntityAttribute(pair);
			}

			// process external providers of static rules
			var sources = new EntityValidationRuleSetSourceExtensionPoint().CreateExtensions();
			var rules = from source in sources.Cast<IValidationRuleSetSource>()
						where source.IsStatic
						let r = source.GetRuleSet(_entityClass)
						where !r.IsEmpty
						select r;
			_highLevelRules.AddRange(rules);

			_processed = true;
		}

		private void ProcessEntityAttribute(AttributeEntityClassPair pair)
		{
			// TODO: this could be changed to a dictionary of delegates, or a visitor pattern of some kind

			if (pair.Attribute is UniqueKeyAttribute)
				ProcessUniqueKeyAttribute(pair);

			if (pair.Attribute is ValidationAttribute)
				ProcessValidationAttribute(pair);
		}

		private void ProcessValidationAttribute(AttributeEntityClassPair pair)
		{
			// check if the attribute specifies a method for retrieving additional rules
			var a = (ValidationAttribute)pair.Attribute;
			if (string.IsNullOrEmpty(a.HighLevelRulesProviderMethod))
				return;

			// find method on class (use the class that declared the attribute, not the entityClass)
			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
			var method = pair.DeclaringClass.GetMethod(a.HighLevelRulesProviderMethod, bindingFlags);

			// validate method signature
			if (method == null)
				throw new InvalidOperationException(string.Format("Method {0} not found on class {1}", a.HighLevelRulesProviderMethod, pair.DeclaringClass.FullName));
			if (method.GetParameters().Length != 0 || !typeof(IValidationRuleSet).IsAssignableFrom(method.ReturnType))
				throw new InvalidOperationException(string.Format("Method {0} must have 0 parameters and return IValidationRuleSet", a.HighLevelRulesProviderMethod));

			var ruleSet = (IValidationRuleSet)method.Invoke(null, null);

			_highLevelRules.Add(ruleSet);
		}

		private void ProcessUniqueKeyAttribute(AttributeEntityClassPair pair)
		{
			var uka = (UniqueKeyAttribute)pair.Attribute;
			_lowLevelRules.Add(new UniqueKeySpecification(pair.DeclaringClass, uka.LogicalName, uka.MemberProperties));
		}

		private void ProcessClassProperties()
		{
			// note: this will return all properties, including those that are inherited from a base class
			var properties = _entityClass.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (var property in properties)
			{
				foreach (Attribute attr in property.GetCustomAttributes(false))
				{
					ProcessPropertyAttribute(property, attr);
				}
			}
		}

		private void ProcessPropertyAttribute(PropertyInfo property, Attribute attr)
		{
			// TODO: this could be changed to a dictionary of delegates, or a visitor pattern of some kind

			if (attr is RequiredAttribute)
				ProcessRequiredAttribute(property, attr);

			if (attr is LengthAttribute)
				ProcessLengthAttribute(property, attr);

			if (attr is EmbeddedValueAttribute)
				ProcessEmbeddedValueAttribute(property, attr);

			if (attr is EmbeddedValueCollectionAttribute)
				ProcessEmbeddedValueCollectionAttribute(property, attr);

			if (attr is UniqueAttribute)
				ProcessUniqueAttribute(property, attr);
		}

		private void ProcessUniqueAttribute(PropertyInfo property, Attribute attr)
		{
			_lowLevelRules.Add(new UniqueSpecification(property));
		}

		private void ProcessRequiredAttribute(PropertyInfo property, Attribute attr)
		{
			_lowLevelRules.Add(new RequiredSpecification(property));
		}

		private void ProcessLengthAttribute(PropertyInfo property, Attribute attr)
		{
			CheckAttributeValidOnProperty(attr, property, typeof(string));

			var la = (LengthAttribute)attr;
			_lowLevelRules.Add(new LengthSpecification(property, la.Min, la.Max));
		}

		private void ProcessEmbeddedValueAttribute(PropertyInfo property, Attribute attr)
		{
			var innerBuiler = new ValidationBuilder(property.PropertyType);
			if (innerBuiler.LowLevelRules.Rules.Count > 0)
			{
				_lowLevelRules.Add(new EmbeddedValueRuleSet(property, new ValidationRuleSet(innerBuiler.LowLevelRules.Rules), false));
			}
		}

		private void ProcessEmbeddedValueCollectionAttribute(PropertyInfo property, Attribute attr)
		{
			var ca = (EmbeddedValueCollectionAttribute)attr;
			var innerBuiler = new ValidationBuilder(ca.ElementType);

			if (innerBuiler.LowLevelRules.Rules.Count > 0)
			{
				_lowLevelRules.Add(new EmbeddedValueRuleSet(property, new ValidationRuleSet(innerBuiler.LowLevelRules.Rules), true));
			}
		}

		private static void CheckAttributeValidOnProperty(Attribute attr, PropertyInfo property, params Type[] types)
		{
			if (!CollectionUtils.Contains(types, t => t.IsAssignableFrom(property.PropertyType)))
				throw new ModellingException(
					string.Format("{0} attribute cannot be applied to property of type {1}.", attr.GetType().Name, property.PropertyType.FullName));
		}

		private static List<AttributeEntityClassPair> GetClassAttributes(Type entityClass)
		{
			// get attributes on this class only - do not get inherited attributes, since these will be handled by recursion below
			var pairs = CollectionUtils.Map(entityClass.GetCustomAttributes(false),
								(Attribute a) => new AttributeEntityClassPair { DeclaringClass = entityClass, Attribute = a });

			// recur on base class
			var baseClass = entityClass.BaseType;
			if (baseClass != typeof(object))
				pairs.AddRange(GetClassAttributes(baseClass));

			return pairs;
		}
	}
}
