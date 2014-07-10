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

using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common.Mail
{
	/// <summary>
	/// Defines an interface to a service that provides and outgoing mail queue.
	/// </summary>
	[ServiceContract]
	[EnterpriseCoreService, Authentication(false)]
	public interface IMailQueueService
	{
		/// <summary>
		/// Enqueues the specified message for transmission.
		/// </summary>
		/// <param name="request"></param>
		[OperationContract]
		[FaultContract(typeof (MessageValidationFault))]
		[FaultContract(typeof (RestrictedMailValidationFault))]
		EnqueueMessageResponse EnqueueMessage(EnqueueMessageRequest request);
	}

	[DataContract]
	public class EnqueueMessageRequest : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public OutgoingMailMessage Message { get; set; }

		[DataMember]
		public OutgoingMailClassification Classification { get; set; }
	}

	[DataContract]
	public class EnqueueMessageResponse : DataContractBase {}

	[DataContract]
	public class MessageValidationFault : DataContractBase
	{
		public MessageValidationFault() {}

		public MessageValidationFault(string message)
		{
			Message = message;
		}

		[DataMember]
		public string Message { get; set; }
	}

	[DataContract]
	public class RestrictedMailValidationFault : MessageValidationFault
	{
		public RestrictedMailValidationFault()
			: base("Specified message request is incompatible with restricted classification.") {}

		public RestrictedMailValidationFault(string message)
			: base(message) {}
	}
}