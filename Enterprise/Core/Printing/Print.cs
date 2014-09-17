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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Common.Scripting;

namespace ClearCanvas.Enterprise.Core.Printing
{
	public class PrintJob
	{
		#region HttpServer class

		class HttpServer
		{
			private readonly Dictionary<Guid, Page> _runningJobs = new Dictionary<Guid, Page>();
			private HttpListener _httpListener;
			private string _host;
			private Thread _listenerThread;

			public HttpServer()
			{
				_httpListener = new HttpListener();
			}

			public void Start()
			{
				if (_httpListener.IsListening)
					return;

				var settings = new PrintSettings();
				var portRange = GetPortRange(settings.HttpProxyServerPortRange);
				Platform.Log(LogLevel.Info, "Starting print server...");

				foreach (var port in portRange)
				{
					if (!TryStartHttpListener(port, out _host))
						continue;

					Platform.Log(LogLevel.Info, "Print server started on port {0}.", port);

					_listenerThread = new Thread(Listen) { IsBackground = true };
					_listenerThread.Start();
					return;
				}
				throw new PrintException(string.Format("Unable to start HTTP print server on any port in range {0}.", settings.HttpProxyServerPortRange));
			}

			public void Stop()
			{
				_httpListener.Stop();
				_listenerThread.Join();
				_listenerThread = null;
			}

			public bool IsStarted
			{
				get { return _httpListener.IsListening; }
			}

			public string Host
			{
				get { return _host; }
			}

			public void AddPage(Page page)
			{
				lock (_runningJobs)
				{
					_runningJobs.Add(page.Id, page);
				}
			}

			public void RemovePage(Page page)
			{
				lock (_runningJobs)
				{
					_runningJobs.Remove(page.Id);
				}
			}

			private void Listen(object state)
			{
				try
				{
					while (_httpListener.IsListening)
					{
						var context = _httpListener.GetContext();
						ThreadPool.QueueUserWorkItem(ProcessRequest, context);
					}
				}
				catch (HttpListenerException e)
				{
					if (e.ErrorCode == 995)
					{
						// this exception gets thrown when Stop() is called
					}
					else
					{
						throw;
					}
				}
			}

			private void ProcessRequest(object state)
			{
				var httpContext = (HttpListenerContext)state;
				try
				{
					if(!HandleRequest(httpContext))
					{
						Redirect(httpContext);
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
					Error(httpContext.Response, 500, "Internal Server Error");
				}
				finally
				{
					// always close output stream, or the response will never terminate (even in an error condition)
					httpContext.Response.OutputStream.Close();
				}
			}

			private bool HandleRequest(HttpListenerContext httpContext)
			{
				var query = ParseQueryString(httpContext.Request.Url);
				var id = query["id"];
				if (string.IsNullOrEmpty(id))
					return false;

				Page page;
				lock(_runningJobs)
				{
					page = _runningJobs[new Guid(id)];
				}

				// we only handle request to our original url
				if (httpContext.Request.Url.AbsolutePath != page.TemplateUrl.AbsolutePath)
					return false;

				Platform.Log(LogLevel.Info, "Received print request: {0}", httpContext.Request.Url);

				// tell the browser that our response will be UTF-8
				httpContext.Response.Headers[HttpResponseHeader.ContentType] = "text/html;charset=UTF-8";

				// write response
				using(var writer = new StreamWriter(httpContext.Response.OutputStream))
				{
					page.WriteHtml(writer);
				}
				return true;
			}

			private static void Redirect(HttpListenerContext httpListenerContext)
			{
				var url = httpListenerContext.Request.Url;
				var redirectUrl = new Uri(new Uri(LocalHost), url.AbsolutePath);
				httpListenerContext.Response.Redirect(redirectUrl.ToString());
			}

			private static void Error(HttpListenerResponse response, int code, string message)
			{
				response.StatusCode = code;
				response.StatusDescription = message;
			}

			private NameValueCollection ParseQueryString(Uri url)
			{
				return string.IsNullOrEmpty(url.Query) ? new NameValueCollection() : HttpUtility.ParseQueryString(url.Query);
			}

			private bool TryStartHttpListener(int port, out string host)
			{
				try
				{
					host = string.Format("{0}:{1}/", LocalHost, port);
					_httpListener.Prefixes.Clear();
					_httpListener.Prefixes.Add(host);
					_httpListener.Start();
					return true;
				}
				catch (HttpListenerException)
				{
					host = null;
					// The existing HttpListener is actually disposed internally and no longer usable, create a new one instead
					_httpListener = new HttpListener();
					return false;
				}
				catch (ObjectDisposedException)
				{
					host = null;
					_httpListener = new HttpListener();
					return false;
				}
			}

			private static IEnumerable<int> GetPortRange(string portRangeString)
			{
				if (!string.IsNullOrEmpty(portRangeString))
				{
					var match = Regex.Match(portRangeString, @"^\s*(\d+)\s*\-\s*(\d+)\s*$");
					if (match.Success)
					{
						var lower = int.Parse(match.Groups[1].Value);
						var upper = int.Parse(match.Groups[2].Value);
						if (upper > lower)
						{
							return Enumerable.Range(lower, upper - lower + 1);
						}
					}
				}
				throw new PrintException("PrintSettings.HttpProxyServerPortRange must specify a valid port range in the form e.g. 10000-10005.");
			}
		}

		#endregion

		#region HttpServerScope

		/// <summary>
		/// Manages lifetime of HTTP server.
		/// </summary>
		class HttpServerScope : IDisposable
		{
			private static readonly HttpServer _httpServer = new HttpServer();
			private static int _refCount;

			private readonly IEnumerable<Page> _pages;
			private bool _disposed;

			public HttpServerScope(IEnumerable<Page> pages)
			{
				_pages = pages;
				lock (_httpServer)
				{
					if (!_httpServer.IsStarted)
						_httpServer.Start();

					foreach (var page in _pages)
						_httpServer.AddPage(page);

					_refCount++;
				}
			}

			public static HttpServer Server
			{
				get { return _httpServer; }
			}

			public void Dispose()
			{
				if(_disposed)
					throw new InvalidOperationException("Already disposed.");

				lock (_httpServer)
				{
					_refCount--;

					foreach (var page in _pages)
						_httpServer.RemovePage(page);

					if (_refCount == 0)
						_httpServer.Stop();
				}
				_disposed = true;
			}
		}

		#endregion

		#region Result class

		public class Result : IDisposable
		{
			public Result(string outputFilePath)
			{
				OutputFilePath = outputFilePath;
			}

			public string OutputFilePath { get; private set; }

			public void Dispose()
			{
				if(!string.IsNullOrEmpty(this.OutputFilePath))
				{
					File.Delete(this.OutputFilePath);
				}
			}
		}

		#endregion

		#region Page class

		class Page
		{
			private readonly PrintJob _job;
			private readonly Guid _id;
			private readonly Uri _templateUri;
			private readonly Dictionary<string, object> _variables; 

			internal Page(PrintJob job, Guid id, IPageModel pageModel)
			{
				_job = job;
				_id = id;
				_templateUri = pageModel.TemplateUrl;
				_variables = pageModel.Variables;
			}

			public Guid Id
			{
				get { return _id; }
			}

			public Uri TemplateUrl
			{
				get { return _templateUri; }
			}

			public Uri ConverterUrl
			{
				get
				{
					var sourcePath = string.Format("{0}?id={1}", TemplateUrl.AbsolutePath, _id.ToString("N"));
					return new Uri(new Uri(HttpServerScope.Server.Host), sourcePath);
				}
			}

			public void WriteHtml(TextWriter writer)
			{
				try
				{
					var request = WebRequest.Create(TemplateUrl);
					var response = (HttpWebResponse)request.GetResponse();
					using (var s = response.GetResponseStream())
					{
						// doubt that GetResponseStream ever returns null, but just in case
						if (s == null)
						{
							_job.SetError("No response stream available.");
							return;
						}

						using (var reader = new StreamReader(s, GetEncoding(response.CharacterSet)))
						{
							var template = new ActiveTemplate(reader);
							var html = template.Evaluate(_variables);
							writer.Write(html);
						}
					}
				}
				catch (WebException e)
				{
					// explicitly handle 404 to provide a helpful error message
					if (e.Status == WebExceptionStatus.ProtocolError && ((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.NotFound)
					{
						_job.SetError(string.Format("The template {0} was not found (404).", TemplateUrl));
					}
					else
					{
						_job.SetError(e.Message);
					}
					throw;
				}
				catch (Exception e)
				{
					_job.SetError(e.Message);
					throw;
				}
			}

			private Encoding GetEncoding(string enc)
			{
				try
				{
					return Encoding.GetEncoding(enc);
				}
				catch (Exception)
				{
					return Encoding.Default;
				}
			}

		}

		#endregion

		private const string LocalHost = "http://localhost";

		/// <summary>
		/// Creates and executes a single-page print job.
		/// </summary>
		/// <param name="pageModel"></param>
		/// <returns></returns>
		public static Result Run(IPageModel pageModel)
		{
			return Run(new[] {pageModel});
		}

		/// <summary>
		/// Creates and executes a multi-page print job.
		/// </summary>
		/// <param name="pageModels"></param>
		/// <returns></returns>
		public static Result Run(IList<IPageModel> pageModels)
		{
			// validate input
			foreach (var pageModel in pageModels)
			{
				if (!pageModel.TemplateUrl.IsLoopback)
					throw new ArgumentException("Must be a local address");

				var url = pageModel.TemplateUrl.ToString();
				if (!url.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase) &&
					!url.EndsWith(".htm", StringComparison.InvariantCultureIgnoreCase))
					throw new ArgumentException("Must be an html file");
			}

			var job = new PrintJob(pageModels);
			return job.Run();
		}

		private readonly IList<Page> _pages; 
		private bool _error;
		private string _errorMessage;

		private PrintJob(IEnumerable<IPageModel> pageModels)
		{
			_pages = pageModels.Select(m => new Page(this, Guid.NewGuid(), m)).ToList();
		}

		private Result Run()
		{
			var outputFilePath = Path.GetTempFileName();

			try
			{
				using (new HttpServerScope(_pages))
				{
					// try this up to five times
					for (var i = 0; i < 5; i++)
					{
						_error = false;
						RunConverter(outputFilePath);
						if (!_error)
							break;
					}
				}
			}
			catch
			{
				File.Delete(outputFilePath);
				throw;
			}
			finally
			{
				if(_error)
				{
					File.Delete(outputFilePath);
				}
			}

			if(_error)
				throw new PrintException(_errorMessage);

			return new Result(outputFilePath);
		}


		private void RunConverter(string outputFilePath)
		{
			var settings = new PrintSettings();
			try
			{
				var sourceUrls = string.Join(" ", _pages.Select(p => p.ConverterUrl.ToString()));
				var arguments = string.Format("{0} {1} {2}", settings.ConverterOptions, sourceUrls, outputFilePath);
				var startInfo = new ProcessStartInfo(settings.ConverterProgram, arguments) {UseShellExecute = false};
				var process = Process.Start(startInfo);
				var exited = process.WaitForExit(settings.ConverterTimeout * 1000);

				if (!exited)
				{
					SetError(string.Format("The converter program ({0}) timed out after {1} ms - printing aborted.",
						settings.ConverterProgram, settings.ConverterTimeout));
				}

			}
			catch (Win32Exception e)
			{
				SetError(
					string.Format("{0}. Usually this means the converter program is not installed, or its location ({1}) is incorrectly configured.",
					e.Message,
					settings.ConverterProgram)
				);
			}
		}

		private void SetError(string message)
		{
			_error = true;
			_errorMessage = message;
		}

	}
}
