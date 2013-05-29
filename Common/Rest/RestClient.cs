using System;

using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace ClearCanvas.Common.Rest
{
	public class RestClientException : Exception
	{
		internal RestClientException(WebException e)
			: base(e.Message, e)
		{
			this.RequestStatus = e.Status;
			if (e.Status == WebExceptionStatus.ProtocolError && e.Response != null)
			{
				var httpResponse = (HttpWebResponse) e.Response;
				this.HttpStatus = httpResponse.StatusCode;
				this.Response = new RestClient.Response(httpResponse);
			}
		}

		public WebExceptionStatus RequestStatus { get; private set; }
		public HttpStatusCode HttpStatus { get; private set; }
		public RestClient.Response Response { get; private set; }
	}


	/// <summary>
	/// 
	/// </summary>
	public class RestClient
	{
		public class Response
		{
			private readonly HttpWebResponse _response;
			internal Response(HttpWebResponse response)
			{
				_response = response;
			}

			public void WriteToStream(Stream stream)
			{
				using (_response)
				using (var responseStream = _response.GetResponseStream())
				{
					if (responseStream == null)
						return;

					responseStream.CopyTo(stream);
				}
			}

			public string AsString()
			{
				using (_response)
				using (var stream = _response.GetResponseStream())
				{
					if (stream == null)
						return null;

					using (var reader = new StreamReader(stream))
					{
						return reader.ReadToEnd();
					}
				}
			}
		}

		public class Request
		{
			private readonly RestClient _rc;
			private HttpWebRequest _request;
			private string _contentString;
			private Stream _contentStream;
			private string _contentType;

			public Request(RestClient rc, string resource, string args)
			{
				_rc = rc;
				Initialize(resource, args);
			}

			public Response Get()
			{
				_request.Method = "GET";
				return GetResponse(_request);
			}

			public Response Post()
			{
				_request.Method = "POST";
				WriteRequestBody();
				return GetResponse(_request);
			}

			public Response Put()
			{
				_request.Method = "PUT";
				WriteRequestBody();
				return GetResponse(_request);
			}

			public Response Delete()
			{
				_request.Method = "DELETE";
				return GetResponse(_request);
			}

			public Request SetData(Stream data, string contentType)
			{
				if (data == null)
					throw new ArgumentNullException("data");
				if (string.IsNullOrEmpty(contentType))
					throw new ArgumentNullException("contentType");
				if(!string.IsNullOrEmpty(_contentString) || _contentStream != null)
					throw new InvalidOperationException("content already specified");

				_contentStream = data;
				_contentType = contentType;

				return this;
			}

			public Request SetData(string data, string contentType)
			{
				if (string.IsNullOrEmpty(data))
					return this;
				if (string.IsNullOrEmpty(contentType))
					throw new ArgumentNullException("contentType");
				if (!string.IsNullOrEmpty(_contentString) || _contentStream != null)
					throw new InvalidOperationException("content already specified");

				_contentString = data;
				_contentType = contentType;

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
					_request.Headers[kvp.Key] = kvp.Value;
				}
			}

			private void WriteRequestBody()
			{
				if(_contentStream != null)
				{
					// set content type/length
					_request.ContentType = _contentType;
					using (var reqStream = _request.GetRequestStream())
					{
						_contentStream.CopyTo(reqStream);
					}
					return;
				}

				if (!string.IsNullOrEmpty(_contentString))
				{
					// set content type/length
					_request.ContentType = _contentType;
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
					throw new RestClientException(e);
				}
			}
		}

		private const int DefaultTimeout = 30000;	// default 30 sec time-out

		private readonly Uri _baseUrl;

		public RestClient(string baseUrl)
		{
			_baseUrl = new Uri(baseUrl);
			this.Timeout = TimeSpan.FromMilliseconds(DefaultTimeout);
			this.Headers = new Dictionary<HttpRequestHeader, string>();
		}

		public string UserAgent { get; set; }

		public TimeSpan Timeout { get; set; }

		public IDictionary<HttpRequestHeader, string> Headers { get; private set; }

		public Response Get(string resource, Dictionary<string, object> args)
		{
			return CreateRequest(resource, args).Get();
		}

		public Response Get(string resource, string args)
		{
			return CreateRequest(resource, args).Get();
		}

		public Request CreateRequest(string resource)
		{
			return new Request(this, resource, null);
		}

		public Request CreateRequest(string resource, string args)
		{
			return new Request(this, resource, args);
		}

		public Request CreateRequest(string resource, Dictionary<string, object> args)
		{
			return new Request(this, resource, QueryString(args));
		}

		private string QueryString(Dictionary<string, object> args)
		{
			return string.Join("&",
				args.Select(kvp => string.Format("{0}={1}", kvp.Key, HttpUtility.UrlEncode(kvp.Value.ToString()))
				).ToArray());
		}
	}
}
