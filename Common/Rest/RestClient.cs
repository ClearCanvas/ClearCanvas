using System;

using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace ClearCanvas.Common.Rest
{
	#region RestClientException

	/// <summary>
	/// Represents an exception resulting from a request.
	/// </summary>
	public class RestClientException : Exception
	{
		internal RestClientException(string url, string method, WebException e)
			: base(e.Message, e)
		{
			this.Url = url;
			this.Method = method;
			this.RequestStatus = e.Status;
			if (e.Status == WebExceptionStatus.ProtocolError && e.Response != null)
			{
				var httpResponse = (HttpWebResponse)e.Response;
				this.HttpStatus = httpResponse.StatusCode;
				this.Response = new RestClient.Response(httpResponse);
			}
		}

		/// <summary>
		/// Gets the URL for the request.
		/// </summary>
		public string Url { get; private set; }

		/// <summary>
		/// Gets the method (HTTP verb) used for the request.
		/// </summary>
		public string Method { get; private set; }

		/// <summary>
		/// Gets the status of the request.
		/// </summary>
		public WebExceptionStatus RequestStatus { get; private set; }

		/// <summary>
		/// Gets the HTTP Status code returned in the response, if a response was obtained.
		/// </summary>
		public HttpStatusCode HttpStatus { get; private set; }

		/// <summary>
		/// Gets the response object, if a response was obtained.
		/// </summary>
		public RestClient.Response Response { get; private set; }
	}

	#endregion


	/// <summary>
	/// Utility class for interacting with HTTP REST style web services.
	/// </summary>
	public class RestClient
	{
		#region Response class

		/// <summary>
		/// Represents the response to an HTTP request.
		/// </summary>
		public class Response : IDisposable
		{
			private HttpWebResponse _response;
			private Stream _responseStream;

			internal Response(HttpWebResponse response)
			{
				_response = response;
				HttpStatus = response.StatusCode;
			}

			/// <summary>
			/// Gets the HTTP status code returned from the server.
			/// </summary>
			public HttpStatusCode HttpStatus { get; private set; }

			/// <summary>
			/// Writes the body of the response to the specified stream, and closes the response.
			/// </summary>
			/// <param name="stream"></param>
			public void WriteToStream(Stream stream)
			{
				try
				{
					using (_response)
					{
						using (_responseStream = _response.GetResponseStream())
						{
							if (_responseStream == null)
								return;

							_responseStream.CopyTo(stream);
							_responseStream.Close();
						}
						_responseStream = null;
					}
				}
				finally
				{
					EnsureCleanUp();
				}
			}

			/// <summary>
			/// Gets the response stream. It is the responsibility of the consumer to dispose
			/// of the stream when finished.
			/// </summary>
			public Stream GetResponseStream()
			{
				return _response.GetResponseStream();
			}

			/// <summary>
			/// Obtains the body of the response as a string, and closes the response.
			/// </summary>
			public string AsString()
			{
				try
				{
					using (_response)
					{
						using (_responseStream = _response.GetResponseStream())
						{
							if (_responseStream == null)
								return null;

							string result;
							using (var reader = new StreamReader(_responseStream))
							{
								result = reader.ReadToEnd();
							}
							_responseStream.Close();
							_responseStream = null;
							return result;
						}
					}
				}
				finally
				{
					EnsureCleanUp();
				}
			}

			/// <summary>
			/// Closes the response.
			/// </summary>
			public void Dispose()
			{
				EnsureCleanUp();
			}

			private void EnsureCleanUp()
			{
				try
				{
					if (_responseStream != null)
					{
						_responseStream.Close();
						_responseStream.Dispose();
						_responseStream = null;
					}
				}
				catch (Exception e)
				{
					/* suppress any possible errors here e.g. already closed */
					Platform.Log(LogLevel.Debug, e, "Error closing HttpWebResponse Stream object.");
				}

				try
				{
					if (_response != null)
					{
						_response.Close();
						_response = null;
					}
				}
				catch (Exception e)
				{
					/* suppress any possible errors here e.g. already closed */
					Platform.Log(LogLevel.Warn, e, "Error closing HttpWebResponse response object.");
				}
			}

			public bool IsContentMultipartRelated()
			{
				var s = _response.Headers["Content-Type"];

				return s == "multipart/related";
			}


			public string GetFilename()
			{
				var s = _response.Headers["Content-Disposition"];
				var i = s.IndexOf("filename=");
				s = s.Substring(i + "filename=".Length);
				return s;
			}
		}

		#endregion

		#region Request class

		/// <summary>
		/// Represents an HTTP request for a specified resource.
		/// </summary>
		public class Request
		{
			private readonly RestClient _rc;
			private HttpWebRequest _request;
			private string _contentString;
			private Stream _contentStream;
			private long _streamBytesWritten;

			internal Request(RestClient rc, string resource, string args)
			{
				_rc = rc;
				Initialize(resource, args);
			}

			/// <summary>
			/// Specifies the KeepAlive value for the web request
			/// </summary>
			public bool KeepAlive
			{
				set { _request.KeepAlive = value; }
				get { return _request.KeepAlive; }
			}

			/// <summary>
			/// Specifies the ProtocolVersion value for the web request
			/// </summary>
			public Version ProtocolVersion
			{
				set { _request.ProtocolVersion = value; }
				get { return _request.ProtocolVersion; }
			}

			/// <summary>
			/// Gets the total length of the content to be written, when it's a <see cref="Stream"/>.
			/// </summary>
			/// <returns>The length of the content to be written, when it's a <see cref="Stream"/>, otherwise zero.</returns>
			public long StreamContentLength
			{
				get { return _contentStream != null ? _contentStream.Length : 0; }
			}

			/// <summary>
			/// Gets the number of bytes written to the content <see cref="Stream"/>.
			/// </summary>
			public long StreamBytesWritten
			{
				get { return _contentStream != null ? _streamBytesWritten : 0; }
			}

			/// <summary>
			/// Fires as underlying <see cref="Stream"/> content is written.
			/// </summary>
			public event EventHandler StreamBytesWrittenChanged;

			/// <summary>
			/// Sends this request using the GET verb and returns the response.
			/// </summary>
			/// <returns></returns>
			public Response Get()
			{
				_request.Method = "GET";
				return GetResponse(_request);
			}

			/// <summary>
			/// Sends this request using the POST verb and returns the response.
			/// </summary>
			/// <returns></returns>
			public Response Post()
			{
				_request.Method = "POST";
				WriteRequestBody();
				return GetResponse(_request);
			}

			/// <summary>
			/// Sends this request using the PUT verb and returns the response.
			/// </summary>
			/// <returns></returns>
			public Response Put()
			{
				_request.Method = "PUT";
				WriteRequestBody();
				return GetResponse(_request);
			}

			/// <summary>
			/// Sends this request using the DELETE verb and returns the response.
			/// </summary>
			/// <returns></returns>
			public Response Delete()
			{
				_request.Method = "DELETE";
				return GetResponse(_request);
			}

			public Response Do(string verb)
			{
				_request.Method = verb;

				// If no request body has been set (via SetData) this is a no-op
				WriteRequestBody();

				return GetResponse(_request);
			}

			/// <summary>
			/// Sets the request body for this request, and returns itself.
			/// </summary>
			/// <param name="data"></param>
			/// <param name="buffer"></param>
			/// <returns></returns>
			public Request SetData(Stream data, bool buffer = true)
			{
				return SetData(data, null, buffer);
			}

			/// <summary>
			/// Sets the request body for this request, and returns itself.
			/// </summary>
			/// <param name="data"></param>
			/// <param name="contentType"></param>
			/// <param name="buffer">Specifies whether the request should buffer the data.</param>
			/// <returns></returns>
			public Request SetData(Stream data, string contentType, bool buffer = true)
			{
				if (data == null)
					throw new ArgumentNullException("data");
				if (!string.IsNullOrEmpty(_contentString) || _contentStream != null)
					throw new InvalidOperationException("content already specified");

				_contentStream = data;
				_request.ContentType = contentType;
				_request.ContentLength = data.Length;
				_request.AllowWriteStreamBuffering = buffer;

				return this;
			}

			/// <summary>
			/// Sets the request body for this request, and returns itself.
			/// </summary>
			/// <param name="data"></param>
			/// <param name="buffer"></param>
			/// <returns></returns>
			public Request SetData(string data, bool buffer = true)
			{
				return SetData(data, null, buffer);
			}

			/// <summary>
			/// Sets the request body for this request, and returns itself.
			/// </summary>
			/// <param name="data"></param>
			/// <param name="contentType"></param>
			/// <param name="buffer">Specifies whether the request should buffer the data.</param>
			/// <returns></returns>
			public Request SetData(string data, string contentType, bool buffer = true)
			{
				if (!string.IsNullOrEmpty(_contentString) || _contentStream != null)
					throw new InvalidOperationException("content already specified");
				if (data == null)
					return this;
				_contentString = data;
				_request.ContentType = contentType;
				_request.ContentLength = data.Length;
				_request.AllowWriteStreamBuffering = buffer;

				return this;
			}

			private void Initialize(string resource, string args)
			{
				var url = _rc._baseUrl;
				if (!string.IsNullOrEmpty(resource))
					url = new Uri(url, resource);

				// append the args to the url
				if (!string.IsNullOrEmpty(args))
				{
					url = new Uri(string.Format("{0}?{1}", url, args));
				}

				// initialize the request
				_request = (HttpWebRequest)WebRequest.Create(url.ToString());
				_request.Timeout = (int)_rc.Timeout.TotalMilliseconds;

				if (!string.IsNullOrEmpty(_rc.UserAgent))
				{
					_request.UserAgent = _rc.UserAgent;
				}

				// copy headers
				foreach (var kvp in _rc.Headers)
				{
					if (kvp.Key == HttpRequestHeader.Accept)
						_request.Accept = kvp.Value;
					else
						_request.Headers[kvp.Key] = kvp.Value;
				}
			}

			private void WriteRequestBody()
			{
				if (_contentStream != null)
				{
					using (var reqStream = _request.GetRequestStream())
					{
						//Avoid LOH on Mono, where the threshold is 8000 bytes.
						var bufferSize = Platform.IsMono ? 7500 : 65535;
						var buffer = new byte[bufferSize];
						int bytesRead;
						while (0 < (bytesRead = _contentStream.Read(buffer, 0, buffer.Length)))
						{
							reqStream.Write(buffer, 0, bytesRead);
							_streamBytesWritten += bytesRead;
							if (StreamBytesWrittenChanged != null)
								StreamBytesWrittenChanged(this, EventArgs.Empty);
						}
						reqStream.Flush();
					}
					return;
				}

				if (_contentString != null) // empty string (0 bytes) is valid
				{
					using (var reqStream = _request.GetRequestStream())
					{
						using (var writer = new StreamWriter(reqStream))
						{
							writer.Write(_contentString);
						}
						reqStream.Close();
					}
				}
			}

			private static Response GetResponse(HttpWebRequest request)
			{
				try
				{
					return new Response((HttpWebResponse)request.GetResponse());
				}
				catch (WebException e)
				{
					throw new RestClientException(request.RequestUri.ToString(), request.Method, e);
				}
			}
		}

		#endregion

		private const int DefaultTimeout = 30000;	// default 30 sec time-out

		private readonly Uri _baseUrl;

		/// <summary>
		/// Constructs a REST client for the specified base URL.
		/// </summary>
		/// <param name="baseUrl"></param>
		public RestClient(string baseUrl)
		{
			_baseUrl = new Uri(baseUrl);
			Timeout = TimeSpan.FromMilliseconds(DefaultTimeout);
			Headers = new Dictionary<HttpRequestHeader, string>();
		}

		#region Public API

		/// <summary>
		/// Gets or sets the user agent string.
		/// </summary>
		public string UserAgent { get; set; }

		/// <summary>
		/// Gets or sets the time-out.
		/// </summary>
		public TimeSpan Timeout { get; set; }

		/// <summary>
		/// Gets the collection of request headers.
		/// </summary>
		public IDictionary<HttpRequestHeader, string> Headers { get; private set; }

		/// <summary>
		/// Convenience method for issuing an HTTP GET request.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public Response Get(string resource, Dictionary<string, object> args)
		{
			return CreateRequest(resource, args).Get();
		}

		/// <summary>
		/// Convenience method for issuing an HTTP GET request.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public Response Get(string resource, string args)
		{
			return CreateRequest(resource, args).Get();
		}

		/// <summary>
		/// Creates a request for the specified resource.
		/// </summary>
		/// <param name="resource"></param>
		/// <returns></returns>
		public Request CreateRequest(string resource)
		{
			return new Request(this, resource, null);
		}

		/// <summary>
		/// Creates a request for the specified resource, with the specified query string arguments.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public Request CreateRequest(string resource, string args)
		{
			return new Request(this, resource, args);
		}

		/// <summary>
		/// Creates a request for the specified resource, with the specified query string arguments.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public Request CreateRequest(string resource, Dictionary<string, object> args)
		{
			return new Request(this, resource, QueryString(args));
		}

		#endregion

		#region Helpers

		private string QueryString(Dictionary<string, object> args)
		{
			return args == null ? null : string.Join("&",
				args.Select(kvp => string.Format("{0}={1}", kvp.Key, HttpUtility.UrlEncode((kvp.Value ?? "").ToString()))
				).ToArray());
		}

		#endregion
	}
}
