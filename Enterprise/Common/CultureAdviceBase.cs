using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace ClearCanvas.Enterprise.Common
{
	public abstract class CultureAdviceBase
	{
		[DataContract]
		public class CultureDirective
		{
			[DataMember]
			public string Culture { get; set; }

			[DataMember]
			public string UICulture { get; set; }
		}

		public const string HeaderName = "ClientCultureDirective";
		public const string HeaderNamespace = "urn:http://www.clearcanvas.ca";

		/// <summary>
		/// Writes the message header the operation context.
		/// </summary>
		/// <param name="directive"></param>
		/// <param name="operationContext"></param>
		protected internal static void WriteMessageHeaders(CultureDirective directive, OperationContext operationContext)
		{
			var header = MessageHeader.CreateHeader(HeaderName, HeaderNamespace, directive);
			operationContext.OutgoingMessageHeaders.Add(header);
		}

		/// <summary>
		/// Attempts to read the message header from the operation context, returning null if the header doesn't exist.
		/// </summary>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		protected internal static CultureDirective ReadMessageHeaders(OperationContext operationContext)
		{
			var h = operationContext.IncomingMessageHeaders.FindHeader(HeaderName, HeaderNamespace);
			return h > -1 ? operationContext.IncomingMessageHeaders.GetHeader<CultureDirective>(h) : null;
		}
	}
}
