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
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.Shreds
{
    internal static class UriHelper
    {
        private static int SegmentCount
        {
            get
            {
                String testUrl = String.Format("http://localhost:{0}{1}", ImageStreamingServerSettings.Default.Port, ImageStreamingServerSettings.Default.Path);
                UriBuilder builder = new UriBuilder(testUrl);
                return builder.Uri.Segments.Length;
            }
        }

        public static string GetServerAE(HttpListenerContext context)
        {
            string[] requestSegments = context.Request.Url.Segments;
            if (requestSegments.Length <= SegmentCount)
                return String.Empty;

            return requestSegments[SegmentCount];
        }
    }

	/// <summary>
	/// Represents an image streaming server.
	/// </summary>
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class ImageStreamingServer : HttpServer
    {
        #region Private Fields
        private readonly List<IPEndPoint> _currentRequests = new List<IPEndPoint>();
        #endregion

	    		
        #region Constructor

        /// <summary>
	    /// Creates an instance of <see cref="ImageStreamingServer"/>
	    /// </summary>
	    public ImageStreamingServer()
	        : base(SR.ImageStreamingServerDisplayName,
                ImageStreamingServerSettings.Default.Port,
                ImageStreamingServerSettings.Default.Path)
		{
            HttpRequestReceived += OnHttpRequestReceived;
            	
		}

        
	    #endregion

        #region Static Methods

        
        private static void Validate(HttpListenerContext context)
        {
            string serverAE = UriHelper.GetServerAE(context);

            if (String.IsNullOrEmpty(serverAE))
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, 
                    String.Format("Partition AE Title is required after {0}", ImageStreamingServerSettings.Default.Path));
            }


            ServerPartition partition = ServerPartitionMonitor.Instance.GetPartition(serverAE);
            if (partition == null)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, String.Format("Partition AE {0} is invalid", serverAE));
            }

            string requestType = context.Request.QueryString["requestType"] ?? "";
            if (String.IsNullOrEmpty(requestType))
            {
                throw new WADOException(HttpStatusCode.BadRequest, "RequestType parameter is missing");
            }

        }

        #endregion

        #region Private Methods
        void AddContext(HttpListenerContext ctx)
        {
            lock (_currentRequests)
            {
                _currentRequests.Add(ctx.Request.RemoteEndPoint);
                if (_currentRequests.Count > ImageStreamingServerSettings.Default.ConcurrencyWarningThreshold)
                {
                    StringBuilder log = new StringBuilder();
                    log.AppendLine(String.Format("Concurrency threshold detected: {0} requests are being processed.", _currentRequests.Count));
                    Dictionary<IPAddress, List<IPEndPoint>> map = CollectionUtils.GroupBy<IPEndPoint, IPAddress>(_currentRequests, delegate(IPEndPoint item) { return item.Address; });
                    foreach (IPAddress client in map.Keys)
                    {
                        log.AppendLine(String.Format("From {0} : {1}", client, map[client].Count));
                    }
                    Platform.Log(LogLevel.Warn, log.ToString());
                }
            }
        }

        void RemoveContext(HttpListenerContext ctx)
        {
            lock (_currentRequests)
            {
                _currentRequests.Remove(ctx.Request.RemoteEndPoint);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
		/// Event handler for <see cref="HttpServer.HttpRequestReceived"/> events.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected void OnHttpRequestReceived(object sender, HttpRequestReceivedEventArg args)
		{

			// NOTE: This method is run under different threads for different http requests.
            HttpListenerContext context = args.Context;
            if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
            Thread.CurrentThread.Name = String.Format("Streaming (client: {0}:{1})", context.Request.RemoteEndPoint.Address, context.Request.RemoteEndPoint.Port);

		    AddContext(context);

			try
			{
			    Validate(context);

                WADORequestProcessor processor = new WADORequestProcessor();
                processor.Process(context);
				
			}
            catch (WADOException e)
            {
                SetResponseError(context, e.GetHttpCode(), e.Message);
            }
            catch (HttpException e)
            {
                SetResponseError(context, e.GetHttpCode(), e.Message);
            }
            catch (ServerTransientError e)
            {
                SetResponseError(context, (int)HttpStatusCode.NoContent, e.Message);
            }
            catch(StudyNotFoundException e)
            {
                SetResponseError(context, (int)HttpStatusCode.BadRequest, e.Message);
            }
            catch (Exception e)
			{
                if (e.InnerException!=null)
					SetResponseError(context, (int)HttpStatusCode.InternalServerError, e.InnerException.Message);
				else
                    SetResponseError(context, (int)HttpStatusCode.InternalServerError, e.Message);
			}
            finally
			{
               
                    // note: the connection might have been aborted or lost too
                    try
                    {
                        context.Response.OutputStream.Flush();
                        context.Response.OutputStream.Close();
                    }
                    catch(Exception ex)
                    {
                        Platform.Log(LogLevel.Warn, "Unexpected exception occurred: {0}", ex.Message);
                    }
                    finally
                    {
                        RemoveContext(context);
                    }
			}

		}

        protected void SetResponseError(HttpListenerContext context, int code, string message)
	    {
            Platform.CheckForNullReference(context, "Context");
	        try
	        {
                Platform.Log(LogLevel.Error, "Streaming Error: {0}  {1}", code, message);
                context.Response.StatusCode = code;
                context.Response.StatusDescription = HttpUtility.HtmlEncode(message);
	        }
            catch(Exception)
            {
                // probably caused by the control characters in the message. 
                // just ignore it
            }
	    }

	    #endregion

        #region Overridden Public Methods


        public override string GetDisplayName()
		{
			return SR.ImageStreamingServerDescription;
		}

		public override string GetDescription()
		{
			return SR.ImageStreamingServerDescription;
		}

		#endregion

		protected override void OnStarted()
		{
			AutoStartMe();
		}

		private void AutoStartMe()
		{
			// Send a dummy request to kick start the service
			// Must be done in a thread because we are 
			// in the thread that handles incoming requests
			Task.Factory.StartNew(() =>
			                      	{
										try
										{
											using (var dummyClient = new WebClient())
											{
												foreach (var p in ServerPartitionMonitor.Instance.Partitions)
												{
													var request =
														String.Format(
															"{0}{1}?requestType=WADO&studyUID=28282&seriesUid=478844&objectUid=1231233&contentType=image%2Fjpeg&rows=100&columns=100",
															ServerEndPointUri, p.AeTitle);

													var uri = new Uri(request);
													
													// Note: this request will be rejected because the study doens't exist but we don't care 
													dummyClient.OpenRead(uri); 
													break;
												}
											}
										}
										catch(Exception e)
										{
											// ignore it
										}
										
			                      	});
			
		}
    }
}