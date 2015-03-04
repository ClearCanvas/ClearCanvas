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
using System.Configuration;
using System.Web.SessionState;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Web.Common.Security;
using ClearCanvas.ImageServer.Web.Common;

namespace ClearCanvas.ImageServer.Web.Application
{
    public class Global : ImageServerHttpApplication
    {
        private DateTime start;

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            start = DateTime.Now;
            if (Context.Request.Path.Contains("ApplicationService.svc"))
                Context.SetSessionStateBehavior(SessionStateBehavior.Disabled);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["PerformanceLogging"].Equals("true"))
            {
                //Ignore some of the requests
                if (Request.Url.AbsoluteUri.Contains("PersistantImage.ashx") ||
                    Request.Url.AbsoluteUri.Contains("WebResource.axd") ||
                    Request.Url.AbsoluteUri.Contains("ScriptResource.axd") ||
                    Request.Url.AbsoluteUri.Contains("Pages/Login") ||
                    Request.Url.AbsoluteUri.Contains("Pages/Error") ||
                    Request.Url.AbsoluteUri.Contains("&error=")) return;
                TimeSpan elapsedTime = DateTime.Now.Subtract(start);
                string processingTime = elapsedTime.Minutes + ":" + elapsedTime.Seconds + ":" + elapsedTime.Milliseconds;

                string userName = "Not Logged In.";
                if (SessionManager.Current != null)
                {
                    userName = SessionManager.Current.User.Credentials.UserName;
                }
                Platform.Log(LogLevel.Debug,
                             string.Format("USER: {0} URL: {1} PROCESSING TIME: {2}", userName,
                                           this.Request.Url.AbsoluteUri, processingTime));
            }
        }
    }

}