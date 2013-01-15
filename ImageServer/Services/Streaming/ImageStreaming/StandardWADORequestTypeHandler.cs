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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    /// <summary>
    /// Represents handler that handles requests with RequestType of "WADO"
    /// </summary>
    [ExtensionOf(typeof(WADORequestTypeExtensionPoint))]
    class StandardWADORequestTypeHandler : IWADORequestTypeHandler
    {

        #region IWADORequestTypeHandler Members

        public string RequestType
        {
            get { return "WADO"; }
        }

        public void Validate(HttpListenerRequest request)
        {
            string studyUid = request.QueryString["studyUID"];
            string seriesUid = request.QueryString["seriesUid"];
            string objectUid = request.QueryString["objectUid"];

            if (String.IsNullOrEmpty(studyUid))
            {
                throw new WADOException(HttpStatusCode.BadRequest, String.Format("studyUID parameter is required"));
            }

            if (String.IsNullOrEmpty(seriesUid))
            {
                throw new WADOException(HttpStatusCode.BadRequest, String.Format("seriesUid parameter is required"));
                
            }

            if (String.IsNullOrEmpty(objectUid))
            {
                throw new WADOException(HttpStatusCode.BadRequest, String.Format("objectUid parameter is required"));
            }
        }
        
        #endregion

        #region IWADORequestTypeHandler Members


        #endregion

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion

        #region IWADORequestTypeHandler Members


        public WADOResponse Process(WADORequestTypeHandlerContext context)
        {
            //Validate(context.HttpContext.Request);

            ObjectStreamingHandlerFactory factory = new ObjectStreamingHandlerFactory();
            IObjectStreamingHandler handler = factory.CreateHandler(context.HttpContext.Request);
            return handler.Process(context);
        }

        #endregion
    }
}
