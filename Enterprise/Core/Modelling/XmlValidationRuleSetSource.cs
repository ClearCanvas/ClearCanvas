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
using System.Xml;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Abstract base class for implementations of <see cref="IValidationRuleSetSource"/> based on XML validation documents.
	/// </summary>
	public abstract class XmlValidationRuleSetSource : IValidationRuleSetSource
	{
		const string ScriptLanguage = "jscript";
		const string TagValidation = "validation";
		const string TagValidationRule = "validation-rule";
		const string TagApplicabililtyRule = "applicability-rule";
		const string TagValidationRuleset = "validation-ruleset";
		const string AttrClass = "class";

		#region Implementation of IXmlValidationRuleSetSource

		public bool IsStatic
		{
			get
			{
				// XML rules are not static - they can be modified at runtime
				return false;
			}
		}

		public ValidationRuleSet GetRuleSet(Type domainClass)
		{
			var ruleSetNodes = FindRulesetNodes(domainClass.FullName);

			// return a single rule set that combines all rule sets
			return ValidationRuleSet.Add(CollectionUtils.Map<XmlElement, ValidationRuleSet>(ruleSetNodes, CompileRuleset));
		}

		#endregion

		protected abstract XmlDocument RuleSetDocument { get; }

		private static ValidationRuleSet CompileRuleset(XmlElement rulesetNode)
		{
			var compiler = new XmlSpecificationCompiler(ScriptLanguage);
			var rules = CollectionUtils.Map<XmlElement, ISpecification>(
				rulesetNode.GetElementsByTagName(TagValidationRule), compiler.Compile);

			var applicabilityRuleNode = CollectionUtils.FirstElement(rulesetNode.GetElementsByTagName(TagApplicabililtyRule));
			return applicabilityRuleNode != null ?
				new ValidationRuleSet(rules, compiler.Compile((XmlElement)applicabilityRuleNode))
				: new ValidationRuleSet(rules);
		}

		private XmlNodeList FindRulesetNodes(string @class)
		{
			return RuleSetDocument.SelectNodes(
				string.Format("/{0}/{1}[@{2}='{3}']",
					TagValidation,
					TagValidationRuleset,
					AttrClass,
					@class));
		}
	}
}
