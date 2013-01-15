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

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    /// <summary>
    /// Defines the interface of handler for different WADO request types.
    /// </summary>
    /// <remarks>
    /// Dicom standard says "WADO" is the only valid request type value currently defined, but additional ones may be added in the future.
    /// </remarks>
    interface IWADORequestTypeHandler : IDisposable
    {
        /// <summary>
        /// Gets the request type that can be handled by the handler.
        /// </summary>
        string RequestType { get;}

        /// <summary>
        /// Processes a WADO request.
        /// </summary>
        /// <param name="context"></param>
        WADOResponse Process(WADORequestTypeHandlerContext context);
    }

    class WADORequestTypeHandlerContext
    {
        public String ServerAE;
        public HttpListenerContext HttpContext;
    }

    /// <summary>
    /// Extension point to allow adding plugins for handling requests with different RequestType parameter
    /// </summary>
    [ExtensionPoint()]
    class WADORequestTypeExtensionPoint : ExtensionPoint<IWADORequestTypeHandler>
    {
    }
}
