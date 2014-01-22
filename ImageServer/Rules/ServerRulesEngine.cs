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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Rules;
using ClearCanvas.Dicom.Utilities.Rules.Specifications;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Rules
{

	[ExtensionPoint]
	public sealed class ServerRuleActionCompilerOperatorExtensionPoint : ExtensionPoint<IXmlActionCompilerOperator<ServerActionContext>>
	{
	}

	public class ServerRulesEngineCompletedEventArgs:EventArgs
	{
		public ServerRulesEngineCompletedEventArgs(IEnumerable<IRule> rulesApplied)
		{
			RulesApplied = rulesApplied;
		}

		/// <summary>
		/// Gets rules which were applied
		/// </summary>
		public IEnumerable<IRule> RulesApplied { get; private set; }
	}

	public interface IServerRulesEngine
	{
		event EventHandler<ServerRulesEngineCompletedEventArgs> Completed;
	}

	/// <summary>
    /// Rules engine for applying rules against DICOM files and performing actions.
    /// </summary>
    /// <remarks>
    /// The ServerRulesEngine encapsulates code to apply rules against DICOM file 
    /// objects.  It will load the rules from the persistent store, maintain them by type,
    /// and then can apply them against specific files.
    /// </remarks>
    /// <seealso cref="ServerActionContext"/>
    /// <example>
    /// Here is an example rule for routing all images with Modality set to CT to an AE
    /// Title CLEARCANVAS.
    /// <code>
    /// <rule id="CT Rule">
    ///   <condition expressionLanguage="dicom">
    ///     <equal test="$Modality" refValue="CT"/>
    ///   </condition>
    ///   <action>
    ///     <auto-route device="CLEARCANVAS"/>
    ///   </action>
    /// </rule>
    /// </code>
    /// </example>
    public class ServerRulesEngine : RulesEngine<ServerActionContext,ServerRuleTypeEnum>, IServerRulesEngine
	{
        private readonly ServerRuleApplyTimeEnum _applyTime;
        private readonly ServerEntityKey _serverPartitionKey;

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// A rules engine will only load rules that apply at a specific time.  The
        /// apply time is specified by the <paramref name="applyTime"/> parameter.
        /// </remarks>
        /// <param name="applyTime">An enumerated value as to when the rules shall apply.</param>
        /// <param name="serverPartitionKey">The Server Partition the rules engine applies to.</param>
        public ServerRulesEngine(ServerRuleApplyTimeEnum applyTime, ServerEntityKey serverPartitionKey)
        {
            _applyTime = applyTime;
            _serverPartitionKey = serverPartitionKey;
            Statistics = new RulesEngineStatistics(applyTime.Lookup, applyTime.LongDescription);
        }

		#endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="ServerRuleApplyTimeEnum"/> for the rules engine.
        /// </summary>
        public ServerRuleApplyTimeEnum RuleApplyTime
        {
            get { return _applyTime; }
        }

        #endregion

		#region Events

		/// <summary>
		/// Occurred when <see cref="Complete"/> in called.
		/// </summary>
		public event EventHandler<ServerRulesEngineCompletedEventArgs> Completed;

		/// <summary>
		/// Gets rules which were applied on previous call to <see cref="Execute"/>
		/// </summary>
		public IEnumerable<IRule> RulesApplied { private set; get; } 

		#endregion

		#region Public Methods


		/// <summary>
        /// Load the rules engine from the Persistent Store and compile the conditions and actions.
        /// </summary>
        public void Load()
        {
            Statistics.LoadTime.Start();

            // Clearout the current type list.
            _typeList.Clear();

            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IServerRuleEntityBroker broker = read.GetBroker<IServerRuleEntityBroker>();

                ServerRuleSelectCriteria criteria = new ServerRuleSelectCriteria();
                criteria.Enabled.EqualTo(true);
                criteria.ServerRuleApplyTimeEnum.EqualTo(_applyTime);
                criteria.ServerPartitionKey.EqualTo(_serverPartitionKey);

				// Add ommitted or included rule types, as appropriate
				if (_omitList.Count > 0)
					criteria.ServerRuleTypeEnum.NotIn(_omitList.ToArray());
				else if (_includeList.Count > 0)
					criteria.ServerRuleTypeEnum.In(_includeList.ToArray());

            	IList<ServerRule> list = broker.Find(criteria);

                // Create the specification and action compilers
                // We'll compile the rules right away
            	var specCompiler = GetSpecificationCompiler();

                foreach (ServerRule serverRule in list)
                {
                    try
                    {
                        var theRule = new Rule<ServerActionContext>();
                        theRule.Name = serverRule.RuleName;
                    	theRule.IsDefault = serverRule.DefaultRule;
                    	theRule.IsExempt = serverRule.ExemptRule;
                        theRule.Description = serverRule.ServerRuleApplyTimeEnum.Description;

                        XmlNode ruleNode =
                            CollectionUtils.SelectFirst<XmlNode>(serverRule.RuleXml.ChildNodes,
                                                                 delegate(XmlNode child) { return child.Name.Equals("rule"); });


                    	var actionCompiler = GetActionCompiler(serverRule.ServerRuleTypeEnum);
						theRule.Compile(ruleNode, specCompiler, actionCompiler);

                        RuleTypeCollection<ServerActionContext, ServerRuleTypeEnum> typeCollection;

                        if (!_typeList.ContainsKey(serverRule.ServerRuleTypeEnum))
                        {
                            typeCollection = new RuleTypeCollection<ServerActionContext, ServerRuleTypeEnum>(serverRule.ServerRuleTypeEnum);
                            _typeList.Add(serverRule.ServerRuleTypeEnum, typeCollection);
                        }
                        else
                        {
                            typeCollection = _typeList[serverRule.ServerRuleTypeEnum];
                        }

                        typeCollection.AddRule(theRule);
                    }
                    catch (Exception e)
                    {
                        // something wrong with the rule...
                        Platform.Log(LogLevel.Warn, e, "Unable to add rule {0} to the engine. It will be skipped",
                                     serverRule.RuleName);
                    }
                }
            }

            Statistics.LoadTime.End();
        }

		public override void Execute(ServerActionContext context)
		{
			var rulesApplied=new List<IRule>();

            Statistics.ExecutionTime.Start();

            foreach (var typeCollection in _typeList.Values)
            {
                typeCollection.Execute(context, false);

				if (typeCollection.LastAppliedRules!=null)
            		rulesApplied.AddRange(typeCollection.LastAppliedRules);
            }

            Statistics.ExecutionTime.End();

			RulesApplied = rulesApplied;
		}

		public void Complete(IEnumerable<IRule> rulesApplied)
		{
			if (Completed != null)
				Completed(this, new ServerRulesEngineCompletedEventArgs(rulesApplied));
		}

		public static XmlSpecificationCompiler GetSpecificationCompiler()
		{
			return new XmlSpecificationCompiler("dicom", new DicomRuleSpecificationCompilerOperatorExtensionPoint());
		}

		public static XmlActionCompiler<ServerActionContext> GetActionCompiler(ServerRuleTypeEnum ruleType)
		{
			var xp = new ServerRuleActionCompilerOperatorExtensionPoint();
			var operators = xp.CreateExtensions(ext => IsApplicableAction(ext.ExtensionClass.Resolve(), ruleType))
				.Cast<IXmlActionCompilerOperator<ServerActionContext>>();

			return new XmlActionCompiler<ServerActionContext>(operators);
		}

		#endregion

		private static bool IsApplicableAction(Type actionClass, ServerRuleTypeEnum ruleType)
		{
			var attrs = AttributeUtils.GetAttributes<ActionApplicabilityAttribute>(actionClass);

			// if no attributes, assume applicable to all rule types
			if (!attrs.Any())
				return true;

			return attrs.Any(a => a.RuleType.ToServerRuleTypeEnum().Equals(ruleType));
		}


	}
}