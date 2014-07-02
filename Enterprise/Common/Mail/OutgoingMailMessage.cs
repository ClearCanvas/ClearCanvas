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

using System.Linq;
using System.Runtime.Serialization;
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
	}
}