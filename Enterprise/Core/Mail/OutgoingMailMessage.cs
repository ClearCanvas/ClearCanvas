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

using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core.Mail
{
	/// <summary>
	/// Represents an outgoing email message.
	/// </summary>
	public class OutgoingMailMessage
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public OutgoingMailMessage()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public OutgoingMailMessage(string sender, string recipient, string subject, string body, bool isHtml)
		{
			Sender = sender;
			Recipient = recipient;
			Subject = subject;
			Body = body;
			IsHtml = isHtml;
		}

		/// <summary>
		/// Gets or sets the sender address.
		/// </summary>
		public string Sender { get; set; }

		/// <summary>
		/// Gets or sets the recipient address.
		/// </summary>
		public string Recipient { get; set; }

		/// <summary>
		/// Gets or sets the subject line.
		/// </summary>
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the content of the message body.
		/// </summary>
		public string Body { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="Body"/> text is HTML.
		/// </summary>
		public bool IsHtml { get; set; }

		/// <summary>
		/// Enqueues this message for transmission.
		/// </summary>
		/// <remarks>
		/// This method is a convenience method and is equivalent to calling
		/// <see cref="IMailQueueService.EnqueueMessage"/> directly.
		/// </remarks>
		public void Enqueue()
		{
			Platform.GetService<IMailQueueService>(service => service.EnqueueMessage(this));
		}
	}
}
