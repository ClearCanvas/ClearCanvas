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
using System.Net;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{
	/// <summary>
	/// An enumeration indicating the type of error represented by a <see cref="StreamingClientException"/>.
	/// </summary>
	public enum StreamingClientExceptionType
	{
		/// <summary>
		/// Indicates that a generic exception has occurred.
		/// </summary>
		Generic,

		/// <summary>
		/// Indicates that an access denied or resource availability error has occurred.
		/// </summary>
		Access,

		/// <summary>
		/// Indicates that a protocol or request error has occurred. These may be indicators of possible bugs.
		/// </summary>
		Protocol,

		/// <summary>
		/// Indicates that a network error has occurred.
		/// </summary>
		Network,

		/// <summary>
		/// Indicates that a server-side error has occurred.
		/// </summary>
		Server,

		/// <summary>
		/// Indicates that an unexpected response was received from the server. These may be indicators of unhandled HTTP scenarios.
		/// </summary>
		UnexpectedResponse
	}

	/// <summary>
	/// The exception that is thrown when an error occurs in the <see cref="StreamingClient"/>.
	/// </summary>
	public class StreamingClientException : Exception
	{
		/// <summary>
		/// Gets an enumerated value indicating the type of error represented by this exception.
		/// </summary>
		public readonly StreamingClientExceptionType Type;

		/// <summary>
		/// Gets the HTTP status code associated with this exception.
		/// </summary>
		/// <remarks>
		/// Returns -1 if the exception is not associated with an HTTP response status.
		/// </remarks>
		public readonly int HttpStatusCode;

		internal StreamingClientException(StreamingClientExceptionType type, Exception innerException)
			: base(GetMessage(type), innerException)
		{
			this.Type = type;
			this.HttpStatusCode = -1;
		}

		internal StreamingClientException(HttpStatusCode httpStatusCode, string httpStatusDescription)
			: base(GetMessage(ClassifyHttpStatusCode(httpStatusCode)), ConstructHttpInnerException(httpStatusCode, httpStatusDescription))
		{
			this.Type = ClassifyHttpStatusCode(httpStatusCode);
			this.HttpStatusCode = (int) httpStatusCode;
		}

		private static Exception ConstructHttpInnerException(HttpStatusCode httpStatusCode, string httpStatusDescription)
		{
			return new Exception(string.Format("The remote server responded with error {0}: {1}", (int) httpStatusCode, httpStatusDescription));
		}

		private static StreamingClientExceptionType ClassifyHttpStatusCode(HttpStatusCode httpStatusCode)
		{
			switch (httpStatusCode)
			{
				case System.Net.HttpStatusCode.Continue:
				case System.Net.HttpStatusCode.SwitchingProtocols:
				case System.Net.HttpStatusCode.OK:
				case System.Net.HttpStatusCode.Created:
				case System.Net.HttpStatusCode.Accepted:
				case System.Net.HttpStatusCode.NonAuthoritativeInformation:
				case System.Net.HttpStatusCode.NoContent:
				case System.Net.HttpStatusCode.ResetContent:
				case System.Net.HttpStatusCode.PartialContent:
				case System.Net.HttpStatusCode.MultipleChoices:
				case System.Net.HttpStatusCode.MovedPermanently:
				case System.Net.HttpStatusCode.Found:
				case System.Net.HttpStatusCode.SeeOther:
				case System.Net.HttpStatusCode.NotModified:
				case System.Net.HttpStatusCode.UseProxy:
				case System.Net.HttpStatusCode.Unused:
				case System.Net.HttpStatusCode.TemporaryRedirect:
					// All the 100, 200 and 300 series HTTP status codes do not technically signify errors,
					// but rather some aspect of HTTP that is not expected and/or handled by our streaming
					// protocol. We classify these as "unexpected responses" so that we can distinguish these
					// cases from real errors should we ever encounter them.
					return StreamingClientExceptionType.UnexpectedResponse;

				case System.Net.HttpStatusCode.BadRequest:
				case System.Net.HttpStatusCode.MethodNotAllowed:
				case System.Net.HttpStatusCode.NotAcceptable:
				case System.Net.HttpStatusCode.LengthRequired:
				case System.Net.HttpStatusCode.PreconditionFailed:
				case System.Net.HttpStatusCode.RequestEntityTooLarge:
				case System.Net.HttpStatusCode.RequestUriTooLong:
				case System.Net.HttpStatusCode.UnsupportedMediaType:
				case System.Net.HttpStatusCode.RequestedRangeNotSatisfiable:
				case System.Net.HttpStatusCode.ExpectationFailed:
					// These codes indicate some kind of protocol or request error.
					return StreamingClientExceptionType.Protocol;

				case System.Net.HttpStatusCode.Unauthorized:
				case System.Net.HttpStatusCode.PaymentRequired:
				case System.Net.HttpStatusCode.Forbidden:
				case System.Net.HttpStatusCode.NotFound:
				case System.Net.HttpStatusCode.ProxyAuthenticationRequired:
				case System.Net.HttpStatusCode.Conflict:
				case System.Net.HttpStatusCode.Gone:
					// These codes indicate some kind of access denied or availability issue error.
					return StreamingClientExceptionType.Access;

				case System.Net.HttpStatusCode.RequestTimeout:
				case System.Net.HttpStatusCode.BadGateway:
				case System.Net.HttpStatusCode.GatewayTimeout:
					// These codes indicate some kind of network error.
					return StreamingClientExceptionType.Network;

				case System.Net.HttpStatusCode.InternalServerError:
				case System.Net.HttpStatusCode.NotImplemented:
				case System.Net.HttpStatusCode.ServiceUnavailable:
				case System.Net.HttpStatusCode.HttpVersionNotSupported:
					// These codes indicate some kind of server-side error.
					return StreamingClientExceptionType.Server;

				default:
					// A generic error.
					return StreamingClientExceptionType.Generic;
			}
		}

		private static string GetMessage(StreamingClientExceptionType type)
		{
			switch (type)
			{
				case StreamingClientExceptionType.Server:
					return "A server error has occurred while streaming the image.";
				case StreamingClientExceptionType.Network:
					return "A network error has occurred while streaming the image.";
				case StreamingClientExceptionType.Protocol:
					return "A protocol error has occurred while streaming the image.";
				case StreamingClientExceptionType.Access:
					return "An access error has occurred while streaming the image.";
				case StreamingClientExceptionType.UnexpectedResponse:
					return "An unexpected response was received while streaming the image.";
				case StreamingClientExceptionType.Generic:
				default:
					return "An error has occurred while streaming the image.";
			}
		}
	}
}