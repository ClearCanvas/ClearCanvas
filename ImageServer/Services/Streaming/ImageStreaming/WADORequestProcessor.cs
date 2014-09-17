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

using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;
using ClearCanvas.ImageServer.Services.Streaming.Shreds;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    
    /// <summary>
    /// Represents a Dicom WADO request processor.
    /// </summary>
    public class WADORequestProcessor
    {

        #region Private Members

        #endregion
		

        #region Public Properties

        public string ServerAE { get; set; }

        #endregion

        #region Constructors

        #endregion


        #region Private Methods

        /// <summary>
        /// Gets a string that represents the mime-types acceptable by the client for the specified context. The mime-types are separated by commas (,).
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string GetClientAcceptTypes(HttpListenerContext context)
        {
            Platform.CheckForNullReference(context, "context");

            if (context.Request.AcceptTypes == null)
                return null;

            StringBuilder mimes = new StringBuilder();
            foreach (string mime in context.Request.AcceptTypes)
            {
                if (mimes.Length > 0)
                    mimes.Append(",");
                mimes.Append(mime);
            }
            return mimes.ToString();
        }

        /// <summary>
        /// Logs information about the request.
        /// </summary>
        /// <param name="context"></param>
		private static void LogRequest(HttpListenerContext context)
        {
        	StringBuilder info = new StringBuilder();

        	info.AppendFormat("\n\tAgents={0}", context.Request.UserAgent);
        	info.AppendFormat("\n\tRequestType={0}", context.Request.QueryString["RequestType"]);
        	info.AppendFormat("\n\tStudyUid={0}", context.Request.QueryString["StudyUid"]);
        	info.AppendFormat("\n\tSeriesUid={0}", context.Request.QueryString["SeriesUid"]);
        	info.AppendFormat("\n\tObjectUid={0}", context.Request.QueryString["ObjectUid"]);
        	info.AppendFormat("\n\tAccepts={0}", GetClientAcceptTypes(context));

        	Platform.Log(LogLevel.Debug, info);
        }

    	/// <summary>
        /// Generates a http response for an error
        /// </summary>
        /// <param name="context"></param>
        private static void SendError(HttpStatusCode errorCode, HttpListenerContext context)
        {

            context.Response.StatusCode = (int)errorCode;
        }


        /// <summary>
        /// Generates a http response based on the specified <see cref="response"/> object and send it to the client
        /// </summary>
        /// <param name="response"></param>
        /// <param name="context"></param>
        private static void SendWADOResponse(WADOResponse response, HttpListenerContext context)
        {
                
            context.Response.StatusCode = (int) HttpStatusCode.OK; // TODO: what does http protocol say about how error that occurs after OK status has been sent should  be handled?

            context.Response.ContentType = response.ContentType;

            if (response.Output == null)
            {
                context.Response.ContentLength64 = 0;

            }
            else
            {
                context.Response.ContentLength64 = response.Output.Length;
                Stream output = context.Response.OutputStream;
                output.Write(response.Output, 0, response.Output.Length);
            }

        }

       
        #endregion

        #region Public Methods

        public void Process(HttpListenerContext context)
        {
            HandleRequest(context);
        }

        private static void HandleRequest(HttpListenerContext context)
        {
            WADORequestProcessorStatistics statistics;

            if (Platform.IsLogLevelEnabled(LogLevel.Debug))
            {
                statistics = new WADORequestProcessorStatistics("Image Streaming");
                statistics.TotalProcessTime.Start();
                //Don't hold up this thread for logging.
                Task.Factory.StartNew(() => LogRequest(context));
            }
            else
            {
                statistics = null;
            }
            
			try
            {
                using (WADORequestTypeHandlerManager handlerManager = new WADORequestTypeHandlerManager())
                {
                    string requestType = context.Request.QueryString["requestType"];
                    IWADORequestTypeHandler typeHandler = handlerManager.GetHandler(requestType);

                    WADORequestTypeHandlerContext ctx = new WADORequestTypeHandlerContext
                    {
                        HttpContext = context,
                        ServerAE = UriHelper.GetServerAE(context)
                    };

                    using (WADOResponse response = typeHandler.Process(ctx))
                    {
                        if (response != null)
                        {
                            if (statistics != null)
                                statistics.TransmissionSpeed.Start();
                            
                            SendWADOResponse(response, context);
                            
                            if (statistics != null)
                                statistics.TransmissionSpeed.End();

                            if (statistics != null && response.Output != null)
                                statistics.TransmissionSpeed.SetData(response.Output.Length);
                        }
                    }

                }
            }
            catch(MimeTypeProcessorError error)
            {
                SendError(error.HttpError, context);
            }

            if (statistics != null)
                statistics.TotalProcessTime.End();

			//Seems like something you'd only want to log if there was a problem.
			if (Platform.IsLogLevelEnabled(LogLevel.Debug))
			{
				//Don't hold up this thread for logging.
				Task.Factory.StartNew(() => StatisticsLogger.Log(LogLevel.Debug, statistics));
			}
        }

        #endregion

    }
}