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
using System.Security.Permissions;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Web.Common.Exceptions;
using System.Security;

namespace ClearCanvas.ImageServer.Web.Common.Modules
{
    /// <summary>
    /// Used to handle web application error. 
    /// </summary>
    /// <remarks>
    /// This module allows capture error and write to the log file. To use it, specify this in the <httpModules> section of web.config:
    /// 
    ///     <add  name="ErrorModule" type="ClearCanvas.ImageServer.Web.Common.ErrorModule, ClearCanvas.ImageServer.Web.Common" />
    /// 
    /// </remarks>
    public class ErrorModule : IHttpModule
    {
        #region IHttpModule Members

        public void Init(HttpApplication application)
        {
            application.Error += new EventHandler(application_Error);
        }

        public void Dispose()
        {
        }

        #endregion

        public void OnError(object obj, EventArgs args)
        {
            HttpContext context = HttpContext.Current;

            Exception baseException = context.Server.GetLastError();
            Platform.Log(LogLevel.Error, context.Error);

            if (baseException != null)
            {
                baseException = baseException.GetBaseException();

                context.Server.ClearError();

                string logMessage = string.Format("Message: {0}\nSource:{1}\nStack Trace:{2}", baseException.Message, baseException.Source, baseException.StackTrace);
                Platform.Log(LogLevel.Error, logMessage);

                ExceptionHandler.ThrowException(baseException);
            }
        }
        
        public void application_Error(object sender, EventArgs e)
        {
            HttpContext ctx = HttpContext.Current;
            Exception theException;
            
            for (theException = ctx.Server.GetLastError();
                 theException != null && theException.InnerException != null;
                 theException = theException.InnerException)
            {
            }

            ctx.Server.ClearError();

            
            if(theException is SecurityException)
            {
                var ex = theException as SecurityException;
                if (ex.PermissionType == typeof(PrincipalPermission))
                {
                    ExceptionHandler.ThrowException(new AuthorizationException());
                    return;
                }
                
            } 
            else if (theException is HttpException)
            {
                HttpException exception = theException as HttpException;
                Platform.Log(LogLevel.Error, "HTTP Error {0}: {0}", exception.ErrorCode, exception);
            }
            else
            {
                Platform.Log(LogLevel.Error, "Unhandled exception: {0}", theException);
            }

            Platform.Log(LogLevel.Error, theException);

            if (theException != null)
                ExceptionHandler.ThrowException(theException);
        }
    }
}
