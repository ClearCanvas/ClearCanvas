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
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.Streaming.Shreds;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    
    /// <summary>
    /// Represents a handler that can stream dicom images to web clients.
    /// </summary>
    internal class ImageStreamingHandler : IObjectStreamingHandler
    {
    	// cache the extension for performance purpose
        static readonly Dictionary<string, Type> _processorMap = new Dictionary<string, Type>();

        private static bool _enableCache = ImageStreamingServerSettings.Default.EnableCache;
        private static TimeSpan _cacheRetentionTime = ImageStreamingServerSettings.Default.CacheRetentionWindow;

		static ImageStreamingHandler()
		{
			// Build the mime-type mapping
			var xp = new ImageMimeTypeProcessorExtensionPoint();
			object[] plugins = xp.CreateExtensions();
			foreach (IImageMimeTypeProcessor mimeTypeConverter in plugins)
			{
				_processorMap.Add(mimeTypeConverter.OutputMimeType, mimeTypeConverter.GetType());
			}
		}

        public WADOResponse Process(WADORequestTypeHandlerContext context)
        {
            Platform.CheckForNullReference(context, "httpContext");
            Platform.CheckForNullReference(context.ServerAE, "context.ServerAE");
            Platform.CheckForNullReference(context.HttpContext, "context.HttpContext");

            // Cache the query string for performance purpose. Every time Request.QueryString is called, .NET will rebuild the entire dictionary.
            NameValueCollection query = context.HttpContext.Request.QueryString;

            ServerPartition partition = ServerPartitionMonitor.Instance.GetPartition(context.ServerAE);
            if (partition== null)
				throw new WADOException(HttpStatusCode.NotFound, String.Format(SR.FaultPartitionNotExists, context.ServerAE));

            if (!partition.Enabled)
                throw new WADOException(HttpStatusCode.Forbidden, String.Format(SR.FaultPartitionDisabled, context.ServerAE));
            
            ImageStreamingContext streamingContext = new ImageStreamingContext(context.HttpContext);
            streamingContext.ServerAE = context.ServerAE;
            streamingContext.ContentType = query["ContentType"];
            streamingContext.AcceptTypes = context.HttpContext.Request.AcceptTypes;
            streamingContext.StudyInstanceUid = query["studyuid"];
            streamingContext.SeriesInstanceUid = query["seriesuid"];
            streamingContext.ObjectUid = query["objectuid"];

            string sessionId = context.HttpContext.Request.RemoteEndPoint.Address.ToString();
            StudyStorageLoader storageLoader = new StudyStorageLoader(sessionId);
            storageLoader.CacheEnabled = _enableCache;
            storageLoader.CacheRetentionTime = _cacheRetentionTime;
            streamingContext.StorageLocation = storageLoader.Find(streamingContext.StudyInstanceUid, partition);
            
            // convert the dicom image into the appropriate mime type
            WADOResponse response = new WADOResponse();
            IImageMimeTypeProcessor processor = GetMimeTypeProcessor(streamingContext);

            MimeTypeProcessorOutput output = processor.Process(streamingContext);   
            response.Output = output.Output;
            response.ContentType = output.ContentType;
            response.IsLast = output.IsLast;
            
            return response;
        }

        protected static bool ClientAcceptable(ImageStreamingContext context, string contentType)
        {
            if (context.AcceptTypes == null)
                return false;
            
            foreach(string rawmime in context.AcceptTypes)
            {
                string mime = rawmime;
                if (rawmime.Contains(";"))
                    mime = rawmime.Substring(0, rawmime.IndexOf(";"));

                if (mime=="*/*" || mime==contentType)
                    return true;
            }

            return false;
        }
        protected virtual IImageMimeTypeProcessor GetMimeTypeProcessor(ImageStreamingContext context)
        {
            string responseContentType = context.ContentType;
            if (String.IsNullOrEmpty(responseContentType))
            {
                if (context.IsMultiFrame)
                    responseContentType = "application/dicom";
                else
                {
                    responseContentType = "image/jpeg";
                    if (!ClientAcceptable(context, responseContentType))
                    {
                        responseContentType = "application/dicom";
                    }
                }
            }

			if (_processorMap.ContainsKey(responseContentType))
            {
				Type processorType = _processorMap[responseContentType];

				var processor = (IImageMimeTypeProcessor)Activator.CreateInstance(processorType);

				if (!ClientAcceptable(context, processor.OutputMimeType))
				{
					Platform.Log(LogLevel.Warn, "Client requested for {0} but did not indicate in the request headers that it could support this mime-type (check HTTP-Accept)", context.ContentType);
				}

            	return processor;

            }

			throw new WADOException(HttpStatusCode.BadRequest,
									String.Format("The specified contentType '{0}' is not supported", responseContentType));
        }

    }
}