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
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common.Mail
{
	/// <summary>
	/// Represents an outgoing email message.
	/// </summary>
	[DataContract]
	public class OutgoingMailMessage : DataContractBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public OutgoingMailMessage() {}

		/// <summary>
		/// Constructor.
		/// </summary>
		public OutgoingMailMessage(string sender, string recipient, string subject, string body, bool isHtml)
			: this(sender, new[] {recipient}, subject, body, isHtml) {}

		/// <summary>
		/// Constructor.
		/// </summary>
		public OutgoingMailMessage(string sender, string[] recipients, string subject, string body, bool isHtml)
		{
			Sender = sender;
			Recipients = recipients;
			Subject = subject;
			Body = body;
			IsHtml = isHtml;
		}

		/// <summary>
		/// Gets or sets the sender address.
		/// </summary>
		[DataMember]
		public string Sender { get; set; }

		/// <summary>
		/// Gets or sets a single recipient address.
		/// </summary>
		public string Recipient
		{
			get { return Recipients != null ? Recipients.SingleOrDefault() : null; }
			set { Recipients = new[] {value}; }
		}

		/// <summary>
		/// Gets or sets the recipient addresses.
		/// </summary>
		[DataMember]
		public string[] Recipients { get; set; }

		/// <summary>
		/// Gets or sets the subject line.
		/// </summary>
		[DataMember]
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the content of the message body.
		/// </summary>
		[DataMember]
		public string Body { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="Body"/> text is HTML.
		/// </summary>
		[DataMember]
		public bool IsHtml { get; set; }

		/// <summary>
		/// Enqueues this message for transmission.
		/// </summary>
		/// <remarks>
		/// This method is a convenience method and is equivalent to calling
		/// <see cref="IMailQueueService.EnqueueMessage"/> directly.
		/// </remarks>
		public void Enqueue(OutgoingMailClassification classification = OutgoingMailClassification.Default)
		{
			Platform.GetService<IMailQueueService>(service => service.EnqueueMessage(new EnqueueMessageRequest {Message = this, Classification = classification}));
		}

		#region Static Helpers

		private static readonly Regex _templateParameterRegex = new Regex(@"\$([a-z]*)\$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <summary>
		/// Processes a parameterized string to resolve parameter variables of form $paramName$.
		/// </summary>
		/// <remarks>
		/// This overload requires that all resolved parameters be determined up-front. If the resolution of some
		/// parameters is costly and the corresponding variables are potentially not present in the template, consider
		/// using the <see cref="ResolveParameters{T}"/> overload, which allows for lazy resolution of parameters.
		/// </remarks>
		/// <param name="s">Template string containing parameter variables of form $paramName$ (use $$ for literal '$' character).</param>
		/// <param name="args">Dictionary of arguments with parameter names as the key (e.g. paramName for the variable $paramName$)</param>
		/// <returns>The resolved string.</returns>
		public static string ResolveParameters(string s, IDictionary<string, string> args)
		{
			Platform.CheckForEmptyString(s, "s");
			Platform.CheckForNullReference(args, "args");

			return _templateParameterRegex.Replace(s, m =>
			{
				string value;
				var key = m.Groups[1].Value;
				if (key == "") return "$";
				return args.TryGetValue(key, out value) ? value : string.Concat("$", key, "$");
			});
		}

		/// <summary>
		/// Processes a parameterized string to resolve parameter variables of form $paramName$.
		/// </summary>
		/// <remarks>
		/// This overload allows for lazy resolution of parameters (i.e. only when the corresponding
		/// variable is encountered in the template). If the parameters are constant or trivially constructed,
		/// or all variables would typically be present in the template, consider using the <see cref="ResolveParameters"/>
		/// overload to eliminate the overhead of the lazy resolution.
		/// </remarks>
		/// <typeparam name="T">Type of state object to be passed to parameter resolution function.</typeparam>
		/// <param name="s">Template string containing parameter variables of form $paramName$ (use $$ for literal '$' character).</param>
		/// <param name="state">State object to be passed to parameter resolution function.</param>
		/// <param name="args">Dictionary of parameter resolution function with parameter names as the key (e.g. paramName for the variable $paramName$)</param>
		/// <returns>The resolved string.</returns>
		public static string ResolveParameters<T>(string s, T state, IDictionary<string, Func<T, string>> args)
		{
			Platform.CheckForEmptyString(s, "s");
			Platform.CheckForNullReference(args, "args");

			var resolvedArgs = new Dictionary<string, string> {{"", "$"}};
			return _templateParameterRegex.Replace(s, m =>
			{
				string value;
				var key = m.Groups[1].Value;
				if (resolvedArgs.TryGetValue(key, out value)) return value;

				Func<T, string> valueGetter;
				return resolvedArgs[key] = (args.TryGetValue(key, out valueGetter) ? valueGetter.Invoke(state) : string.Concat("$", key, "$"));
			});
		}

		#endregion
	}
}