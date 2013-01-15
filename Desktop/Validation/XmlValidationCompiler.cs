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
using System.Text;
using System.IO;
using System.Xml;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Compiles validation rules that are encoded as XML specifications.
    /// </summary>
    public class XmlValidationCompiler
    {
        private readonly XmlSpecificationCompiler _specCompiler;

        /// <summary>
        /// Constructor.
        /// </summary>
        public XmlValidationCompiler()
            :this("jscript")
        {
        }

        /// <summary>
        /// Constructor that allows specifying the expression language to use when evaluating the XML specification.
        /// </summary>
        /// <param name="defaultExpressionLanguage"></param>
        public XmlValidationCompiler(string defaultExpressionLanguage)
        {
            _specCompiler = new XmlSpecificationCompiler(defaultExpressionLanguage);
        }

        /// <summary>
        /// Compiles all rules contained in the specified XML rules document.
        /// </summary>
        /// <param name="rulesDocument"></param>
        /// <returns></returns>
        public List<IValidationRule> CompileRules(TextReader rulesDocument)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(rulesDocument);

            return CompileRules(xmlDocument);
        }

        /// <summary>
        /// Compiles all rules contained in the specified XML rules document.
        /// </summary>
        /// <param name="rulesDocument"></param>
        /// <returns></returns>
        public List<IValidationRule> CompileRules(XmlDocument rulesDocument)
        {
            XmlNodeList ruleNodes = rulesDocument.SelectNodes("validation-rules/validation-rule");
            return CollectionUtils.Map<XmlNode, IValidationRule>(ruleNodes,
                delegate(XmlNode ruleNode) { return CompileRule((XmlElement)ruleNode); });
        }

        /// <summary>
        /// Compiles the rule described by the specified XML element, which must be a "validation-rule" element.
        /// </summary>
        /// <param name="ruleNode"></param>
        /// <returns></returns>
        public IValidationRule CompileRule(XmlElement ruleNode)
        {
            string property = ruleNode.GetAttribute("boundProperty");

            ISpecification spec = _specCompiler.Compile(ruleNode);
            return new ValidationRule(property, spec);
        }
    }
}
