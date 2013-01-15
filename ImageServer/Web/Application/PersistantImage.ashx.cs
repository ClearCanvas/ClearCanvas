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
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using ClearCanvas.ImageServer.Web.Common;

namespace ClearCanvas.ImageServer.Web.Application
{
    public class PersistantImage : IHttpHandler
    {
        private HttpContext _currentContext = null;

        protected string Key
        {
            get
            {
                return ((_currentContext != null) && (_currentContext.Request != null) && (_currentContext.Request.QueryString["key"] != null)) ? _currentContext.Request.QueryString["key"] : "";
            }
        }

        protected string Url
        {
            get
            {
                return ((Key.Length > 0) && (WebConfigurationManager.AppSettings.Get(Key) != null)) ? WebConfigurationManager.AppSettings.Get(Key) : "";
            }
        }

        protected string Path
        {
            get
            {
                return (Url.Length > 0) ? HttpContext.Current.Server.MapPath("~/App_Themes/" + ThemeManager.CurrentTheme + "/" + Url) : "";
            }
        }

        protected string Extension
        {
            get
            {
                return ((Path.Length > 0) && (Path.LastIndexOf('.') > -1) && (Path.LastIndexOf('.') < (Path.Length - 1))) ? Path.Substring(Path.LastIndexOf('.') + 1).ToLower() : "";
            }
        }

        protected bool IsSafe()
        {
            bool bSafeExtension = (Extension.Equals("jpg") || Extension.Equals("jpeg") || Extension.Equals("gif") || Extension.Equals("png"));

            Regex regEx = new Regex("[^a-zA-Z0-9._-~/]");
            bool bSafeChars = (!regEx.IsMatch(Url)) && (!Url.Contains(".."));

            return (bSafeExtension && bSafeChars);
        }

        public void ProcessRequest(HttpContext context)
        {
            _currentContext = context;

            if ((Path.Length > 0) && IsSafe())
            {
                context.Response.ContentType = "image/" + Extension;
                context.Response.Cache.AppendCacheExtension("post-check=900,pre-check=3600");
                context.Response.WriteFile(Path);
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("");
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }

}