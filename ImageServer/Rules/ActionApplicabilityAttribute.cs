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

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ActionOperatorAttribute: Attribute
	{
		/// <summary>
		/// Specifies the operator tag for the action decorated by this attribute
		/// </summary>
		/// <param name="operatorTag"> </param>
		public ActionOperatorAttribute(String operatorTag)
		{
			this.OperatorTag = operatorTag;
		}

		public string OperatorTag { get; private set; }

	}
}
