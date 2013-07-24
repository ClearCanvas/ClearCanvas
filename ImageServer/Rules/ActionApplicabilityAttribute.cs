using System;
using ClearCanvas.Common.Actions;

namespace ClearCanvas.ImageServer.Rules
{
	/// <summary>
	/// When applied to a class that implements <see cref="IXmlActionCompilerOperator{TActionContext}"/>, specifies
	/// that the action operator is valid only for rules of the specified type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class ActionApplicabilityAttribute : Attribute
	{
		/// <summary>
		/// Specifies the type of rule for which an action operator is valid.
		/// </summary>
		/// <param name="ruleType"></param>
		public ActionApplicabilityAttribute(ApplicableRuleType ruleType)
		{
			this.RuleType = ruleType;
		}

		/// <summary>
		/// Gets the rule type.
		/// </summary>
		public ApplicableRuleType RuleType { get; private set; }
	}
}
