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
using System.Web;

namespace ClearCanvas.ImageServer.Web.Common.Modules
{
    class HttpContextDataManagementModule:  IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
            // Per MSDN: we don't need to unregister events attached to HttpApplication here
        }

        public void Init(HttpApplication context)
        {
            // Apparently we don't need to unregister events added to HttpApplication.
            // IHttpModule.Dispose() is called when HttpApplication is dispose(). HttpApplication disposes
            // all events itself.
            context.EndRequest += EndRequest;
            
        }

        static void EndRequest(object sender, EventArgs e)
        {
            var contextData = HttpContextData.Current;
            if (contextData != null)
            {
                contextData.Dispose();
            } 
        }

        #endregion
    }
}
