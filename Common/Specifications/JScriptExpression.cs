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

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Scripting;

namespace ClearCanvas.Common.Specifications
{
	[ExtensionOf(typeof(ExpressionFactoryExtensionPoint))]
	[LanguageSupport("jscript")]
	public class JScriptExpressionFactory : IExpressionFactory
	{
		#region IExpressionFactory Members

		public Expression CreateExpression(string text)
		{
			return new JScriptExpression(text);
		}

		#endregion
	}

	internal class JScriptExpression : Expression
	{
		private const string AutomaticVariableToken = "$";

		[ThreadStatic]
		private static IScriptEngine _scriptEngine;
		private IExecutableScript _script;

		internal JScriptExpression(string text)
			: base(text)
		{
		}

		public override object Evaluate(object arg)
		{
			if (string.IsNullOrEmpty(this.Text))
				return null;

			if (this.Text == AutomaticVariableToken)
				return arg;

			try
			{
				// create the script if not yet created
				if (_script == null)
					_script = CreateScript(this.Text);

				// evaluate the test expression
				var context = new Dictionary<string, object> {{AutomaticVariableToken, arg}};
				return _script.Run(context);
			}
			catch (Exception e)
			{
				throw new SpecificationException(string.Format(SR.ExceptionJScriptEvaluation, this.Text, e.Message), e);
			}
		}

		private static IExecutableScript CreateScript(string expression)
		{
			return ScriptEngine.CreateScript("return " + expression, new [] { AutomaticVariableToken });
		}

		private static IScriptEngine ScriptEngine
		{
			get { return _scriptEngine ?? (_scriptEngine = ScriptEngineFactory.GetEngine("jscript")); }
		}
	}
}
