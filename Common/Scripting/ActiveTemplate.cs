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
using System.Collections;
using System.IO;
using System.Text;
using ClearCanvas.Common.Utilities;
using System.Collections.Generic;

namespace ClearCanvas.Common.Scripting
{
	/// <summary>
	/// Represents an instance of an active template.
	/// </summary>
	/// <remarks>
	/// <para>
	/// An active template is equivalent to a classic ASP page: that is,
	/// it is a template that contains snippets of script code that can call back into the context in which the script
	/// is being evaluated.  Currently only the Jscript language is supported.
	/// </para>
	/// <para>
	/// Initialize the template context via the constructor.  The template
	/// can then be evaluated within a given context by calling one of the <b>Evaluate</b> methods.
	/// </para>
	/// </remarks>
	public class ActiveTemplate
	{
		private readonly string _inversion;
		private IExecutableScript _script;

		/// <summary>
		/// Instantiates an active template from an embedded resource.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="resolver"></param>
		/// <returns></returns>
		public static ActiveTemplate FromEmbeddedResource(string resource, IResourceResolver resolver)
		{
			using (var stream = resolver.OpenResource(resource))
			{
				using (var reader = new StreamReader(stream))
				{
					return new ActiveTemplate(reader);
				}
			}
		}

		/// <summary>
		/// Constructs a template from the specified content.
		/// </summary>
		public ActiveTemplate(TextReader content)
		{
			_inversion = ComputeInversion(content);
		}

		/// <summary>
		/// Overload that allows the output of the template evaluation to be written directly to a <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="context">A dictionary of objects to pass into the script.</param>
		/// <param name="output">A text writer to which the output should be written.</param>
		public void Evaluate(Dictionary<string, object> context, TextWriter output)
		{
			try
			{
				// add a variable for the output stream to the context
				context["__out__"] = output;

				// create executable script if not created
				if (_script == null)
				{
					var variables = CollectionUtils.Map(context.Keys, (string s) => s).ToArray();
					_script = ScriptEngineFactory.GetEngine("jscript").CreateScript(_inversion, variables);
				}

				_script.Run(context);

			}
			catch (Exception e)
			{
				throw new ActiveTemplateException(SR.ExceptionTemplateEvaluation, e);
			}
		}

		/// <summary>
		/// Evaluates this template in the specified context.
		/// </summary>
		/// <remarks>
		/// The context parameter allows a set of named objects to be passed into 
		/// the scripting environment.  Within the scripting environment
		/// these objects can be referenced as globals.  For example,
		/// <code>
		///     Dictionary&lt;string, object&gt; scriptingContext = new Dictionary&lt;string, object&gt;();
		///     scriptingContext["Patient"] = patient;  // add a reference to an existing instance of a patient object
		/// 
		///     Template template = new Template(...);
		///     template.Evaluate(scriptingContext);
		/// 
		///     // now, in the template, the script would access the object as shown
		///     &lt;%= Patient.Name %&gt;
		/// </code>
		/// </remarks>
		/// <param name="context">A dictionary of objects to pass into the script.</param>
		/// <returns>The result of the template evaluation as a string.</returns>
		public string Evaluate(Dictionary<string, object> context)
		{
			var output = new StringWriter();
			Evaluate(context, output);
			return output.ToString();
		}

		/// <summary>
		/// Inverts the template content, returning a Jscript script that, when evaluated, will return
		/// the full result of the template.
		/// </summary>
		private static string ComputeInversion(TextReader template)
		{
			var inversion = new StringBuilder();
			string line;
			var inCode = false;    // keep track of whether we are inside a <% %> or not

			// process each line of the template
			while ((line = template.ReadLine()) != null)
			{
				inCode = ProcessLine(line, inCode, inversion);

				// preserve the formatting of the original template by writing new lines appropriately
				if (!inCode)
					inversion.AppendLine("this.__out__.WriteLine();");
			}

			return inversion.ToString();
		}

		private static bool ProcessLine(string line, bool inCode, StringBuilder inversion)
		{
			inCode = !inCode;   // just make the loop work correctly

			// break the line up into code/non-code parts
			var parts = line.Split(new [] { "<%", "%>" }, StringSplitOptions.None);
			foreach (var part in parts)
			{
				inCode = !inCode;
				if (inCode)
				{
					if (part.StartsWith("="))
					{
						inversion.AppendLine(string.Format("this.__out__.Write({0});", part.Substring(1)));
					}
					else
					{
						inversion.Append(part);
						inversion.AppendLine();
					}
				}
				else
				{
					var escaped = part.Replace("\"", "\\\"");  // escape any " characters
					inversion.AppendLine(string.Format("this.__out__.Write(\"{0}\");", escaped));
				}
			}
			return inCode;
		}
	}
}
