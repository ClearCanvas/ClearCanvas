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

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    /// <summary>
    /// Manages life-time of request type plugins.
    /// </summary>
    /// <remarks>
    /// <see cref="WADORequestTypeHandlerManager"/> instantiates and cleans up resources held by plugins that implements <see cref="IWADORequestTypeHandler"/>.
    /// When <see cref="WADORequestTypeHandlerManager"/> is <see cref="Dispose">disposed</see> the plugin instances held are also disposed of. 
    /// </remarks>
    class WADORequestTypeHandlerManager : IDisposable
    {
        #region Private Members
        private Dictionary<string, IWADORequestTypeHandler> _handlers = new Dictionary<string, IWADORequestTypeHandler>();
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="WADORequestTypeHandlerManager"/>
        /// </summary>
        public WADORequestTypeHandlerManager()
        {
            LoadHandlers();
        }
        #endregion

        #region Public Methods

        public IWADORequestTypeHandler GetHandler(string requestType)
        {
            String type = requestType.ToUpper();
            if (_handlers.ContainsKey(type))
                return _handlers[type];
            else
                throw new WADOException(HttpStatusCode.BadRequest, String.Format("Unsupported RequestType {0}", requestType));

        }

        #endregion

        #region Private Methods

        private void LoadHandlers()
        {
            WADORequestTypeExtensionPoint xp = new WADORequestTypeExtensionPoint();
            object[] plugins = xp.CreateExtensions();
            foreach (object plugin in plugins)
            {
                if (plugin is IWADORequestTypeHandler)
                {
                    IWADORequestTypeHandler typeHandler = plugin as IWADORequestTypeHandler;
                    _handlers.Add(typeHandler.RequestType.ToUpper(), typeHandler);
                }
            }
        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            foreach (IWADORequestTypeHandler plugin in _handlers.Values)
            {
                plugin.Dispose();
            }
        }

        #endregion
    }
}
