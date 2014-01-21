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
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Utilities.Rules
{
    /// <summary>
    /// A collection of rules of similar types.
    /// </summary>
    /// <typeparam name="TContext">The context for the rules.</typeparam>
    /// <typeparam name="TTypeEnum">The type of the rule.</typeparam>
    public class RuleTypeCollection<TContext, TTypeEnum>
        where TContext : ActionContext
    {
        private readonly List<Rule<TContext>> _exemptRuleList = new List<Rule<TContext>>();
        private readonly List<Rule<TContext>> _ruleList = new List<Rule<TContext>>();

        #region Constructors

        public RuleTypeCollection(TTypeEnum type)
        {
            Type = type;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The type of rule for the collection.
        /// </summary>
        public TTypeEnum Type { get; private set; }

        /// <summary>
        /// The identified default rule for the collection of rules.
        /// </summary>
        public Rule<TContext> DefaultRule { get; private set; }

		/// <summary>
		/// Gets the rules which were applied in previous <see cref="Execute"/> call.
		/// </summary>
		public IList<IRule> LastAppliedRules { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a rule to the collection.
        /// </summary>
        /// <param name="rule"></param>
        public void AddRule(Rule<TContext> rule)
        {
            if (rule.IsDefault)
            {
                if (DefaultRule != null)
                {
                    Platform.Log(LogLevel.Error, "Unexpected multiple default rules for rule {0} of type {1}",
                                 rule.Name, rule.Description);
                    Platform.Log(LogLevel.Error, "Ignoring rule {0}", rule.Name);
                }
                else
                    DefaultRule = rule;
            }
            else if (rule.IsExempt)
                _exemptRuleList.Add(rule);
            else
                _ruleList.Add(rule);
        }

        /// <summary>
        /// Execute the rules within the <see cref="RuleTypeCollection{TActionContext,TTypeEnum}"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="stopOnFirst"></param>
        public void Execute(TContext context, bool stopOnFirst)
        {
			LastAppliedRules = new List<IRule>();

            bool doDefault = true;
            try
            {
                foreach (var theRule in _exemptRuleList)
                {
                    bool ruleApplied;
                    bool ruleSuccess;

					context.Name = theRule.Name;
					theRule.Execute(context, false, out ruleApplied, out ruleSuccess);

                    if (ruleApplied)
                    {
						LastAppliedRules.Add(theRule);
                        Platform.Log(LogLevel.Info, "Exempt rule found that applies for {0}, ignoring action.", Type.ToString());
                        return;
                    }
                }

                foreach (var theRule in _ruleList)
                {
                    bool ruleApplied;
                    bool ruleSuccess;

					context.Name = theRule.Name;
					theRule.Execute(context, false, out ruleApplied, out ruleSuccess);

                    if (ruleApplied && ruleSuccess)
                    {
						LastAppliedRules.Add(theRule);

                        if (stopOnFirst)
                            return;

                        doDefault = false;
                    }
                }

                if (doDefault && DefaultRule != null)
                {
                    bool ruleApplied;
                    bool ruleSuccess;

					context.Name = DefaultRule.Name;
					DefaultRule.Execute(context, true, out ruleApplied, out ruleSuccess);
					if (ruleApplied)
					{
						LastAppliedRules.Add(DefaultRule);
					}

                    if (!ruleSuccess)
                    {
                        Platform.Log(LogLevel.Error, "Unable to apply default rule of type {0}", Type);
                    }
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when applying rule of type: {0}", Type);
            }
        }

        #endregion
    }
}