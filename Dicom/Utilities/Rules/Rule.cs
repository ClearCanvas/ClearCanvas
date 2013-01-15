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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.Utilities.Rules
{
    /// <summary>
    /// Representation of a rule.
    /// </summary>
    /// <typeparam name="TActionContext">The context passed to the <see cref="IActionSet{TActionContext}"/> when executing the action.</typeparam>
    /// <typeparam name="TTypeEnum">A type enum that represents the type of rule.</typeparam>
    public class Rule<TActionContext, TTypeEnum>
        where TActionContext : ActionContext
    {
        private IActionSet<TActionContext> _actions;
        private ISpecification _conditions;

        #region Public Properties

        /// <summary>
        /// The name of the rule.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description of the rule.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Is the rule a default rule?
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Is the rule an Exempt rule that exempts other rules from applying.
        /// </summary>
        public bool IsExempt { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Compile the rule.
        /// </summary>
        /// <param name="ruleNode">XML representation of the rule.</param>
        /// <param name="ruleType">The type of the rule.</param>
        /// <param name="specCompiler">An <see cref="XmlSpecificationCompiler"/>.</param>
        /// <param name="actionCompiler">An <see cref="XmlActionCompiler{TActionContext,TTypeEnum}"/>.</param>
        public void Compile(XmlNode ruleNode, TTypeEnum ruleType, XmlSpecificationCompiler specCompiler,
                            XmlActionCompiler<TActionContext, TTypeEnum> actionCompiler)
        {
            var conditionNode =
                CollectionUtils.SelectFirst(ruleNode.ChildNodes,
                                            (XmlNode child) => child.Name.Equals("condition"));

            if (conditionNode != null)
                _conditions = specCompiler.Compile(conditionNode as XmlElement, true);
            else if (!IsDefault)
                throw new ApplicationException("No condition element defined for the rule.");
            else
                _conditions = new AndSpecification();

            var actionNode =
                CollectionUtils.SelectFirst(ruleNode.ChildNodes,
                                            (XmlNode child) => child.Name.Equals("action"));

            if (actionNode != null)
                _actions = actionCompiler.Compile(actionNode as XmlElement, ruleType, true);
            else if (!IsExempt)
                throw new ApplicationException("No action element defined for the rule.");
            else
                _actions = new ActionSet<TActionContext>(new List<IActionItem<TActionContext>>());
        }

        /// <summary>
        /// Returns true if the rule condition is passed for the specified context
        /// </summary>
        /// <param name="context"></param>
        /// <returns>A <see cref="TestResult"/> representing if the conditions apply.</returns>
        public TestResult Test(TActionContext context)
        {
            if (IsDefault)
                return new TestResult(true);

            return _conditions.Test(context.Message);
        }

        /// <summary>
        /// Execute the rule.
        /// </summary>
        /// <param name="context">The context to pass to the action compiler.</param>
        /// <param name="defaultRule">Is the rule a default rule?</param>
        /// <param name="ruleApplied">Was the rule applied?</param>
        /// <param name="ruleSuccess">Was the rule successful?</param>
        public void Execute(TActionContext context, bool defaultRule, out bool ruleApplied, out bool ruleSuccess)
        {
            ruleApplied = false;
            ruleSuccess = true;

            TestResult result = Test(context);

            if (result.Success)
            {
                ruleApplied = true;
                Platform.Log(LogLevel.Debug, "Applying rule {0}", Name);
                TestResult actionResult = _actions.Execute(context);
                if (actionResult.Fail)
                {
                    foreach (TestResultReason reason in actionResult.Reasons)
                    {
                        Platform.Log(LogLevel.Error, "Unexpected error performing action {0}: {1}", Name, reason.Message);
                    }
                    ruleSuccess = false;
                }
            }
        }

        #endregion

        #region Static Public Methods

        /// <summary>
        /// Method for validating proper format of a ServerRule.
        /// </summary>
        /// <param name="type">The type of rule to validate</param>
        /// <param name="rule">The rule to validate</param>
        /// <param name="errorDescription">A failure description on error.</param>
        /// <returns>true on successful validation, otherwise false.</returns>
        public static bool ValidateRule(TTypeEnum type, XmlDocument rule, out string errorDescription)
        {
            var specCompiler = new XmlSpecificationCompiler("dicom");
            var actionCompiler = new XmlActionCompiler<TActionContext, TTypeEnum>();


            var theRule = new Rule<TActionContext, TTypeEnum>
                              {
                                  Name = string.Empty
                              };

            var ruleNode =
                CollectionUtils.SelectFirst(rule.ChildNodes,
                                            (XmlNode child) => child.Name.Equals("rule"));


            try
            {
                theRule.Compile(ruleNode, type, specCompiler, actionCompiler);
            }
            catch (Exception e)
            {
                errorDescription = e.Message;
                return false;
            }

            errorDescription = "Success";
            return true;
        }

        #endregion
    }
}