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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public partial class CheckJavascript : UserControl
    {
        protected static string JSDISABLED = "0";
        protected static string JSENABLED = "1";
        protected static string JSQRYPARAM = "jse";

        protected bool IsJSEnabled
        {
            get
            {
                if (Session["JS"] == null)
                    Session["JS"] = true;

                return (bool) Session["JS"];
            }
            set { Session["JS"] = value; }
        }

        protected string JSTargetURL
        {
            get { return Request.Url.ToString(); }
        }

        protected string NonJSTargetURL
        {
            get { return ResolveServerUrl(ImageServerConstants.PageURLs.JavascriptErrorPage, false); }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Request.QueryString[JSQRYPARAM] != null)
            {
                IsJSEnabled = Request.QueryString[JSQRYPARAM] == JSENABLED;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected string GetAppendedUrl(string newParam, string newParamValue)
        {
            string targeturl = string.Empty;
            Uri url = (string.IsNullOrEmpty(NonJSTargetURL))
                          ? new Uri(JSTargetURL)
                          : new Uri(NonJSTargetURL);
            if (url == null)
                url = Request.Url;

            string[] qry = url.Query.Replace("?", "").Split('&');

            StringBuilder sb = new StringBuilder();
            foreach (string s in qry)
            {
                if (!s.ToLower().Contains(newParam.ToLower()))
                {
                    sb.Append(s + "&");
                }
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
                targeturl = string.Format("{0}?{1}&{2}={3}", url.AbsolutePath, sb, newParam, newParamValue);
            }
            else
            {
                targeturl = string.Format("{0}?{1}={2}", url.AbsolutePath, newParam, newParamValue);
            }
            return targeturl;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (IsJSEnabled)
            {
                string targeturl = GetAppendedUrl(JSQRYPARAM, JSDISABLED);
                HtmlGenericControl ctrl = new HtmlGenericControl("NOSCRIPT");
                ctrl.InnerHtml = string.Format("<meta http-equiv=REFRESH content=0;URL={0}>", targeturl);
                Page.Header.Controls.Add(ctrl);
            }
            else
            {
                if (!string.IsNullOrEmpty(NonJSTargetURL))
                    Response.Redirect(NonJSTargetURL);
                HtmlGenericControl ctrl = new HtmlGenericControl("NOSCRIPT");
                ctrl.InnerHtml = string.Empty;
                Page.Header.Controls.Add(ctrl);
            }
        }

        public string ResolveServerUrl(string serverUrl, bool forceHttps)
        {
            if (serverUrl.IndexOf("://") > -1)

                return serverUrl;
            string newUrl = ResolveUrl(serverUrl);
            Uri originalUri = HttpContext.Current.Request.Url;

            newUrl = (forceHttps ? "https" : originalUri.Scheme) +
                     "://" + originalUri.Authority + newUrl;
            return newUrl;
        }
    }

    public class CheckJavaScriptHelper
    {
        public static bool IsJavascriptEnabled
        {
            get
            {
                if (HttpContext.Current.Session["JS"] == null)
                    HttpContext.Current.Session["JS"] = true;

                return (bool) HttpContext.Current.Session["JS"];
            }
        }
    }
}