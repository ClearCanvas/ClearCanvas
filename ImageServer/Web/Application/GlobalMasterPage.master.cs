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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Exceptions;
using ClearCanvas.ImageServer.Web.Common.Security;
using ClearCanvas.Web.Enterprise.Authentication;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application
{
    public partial class GlobalMasterPage : MasterPage, IMasterProperties
    {
        private bool _displayUserInfo = true;

        public bool DisplayUserInformationPanel
        {
            get { return _displayUserInfo; }
            set { _displayUserInfo = value; }
        }

		public bool RequiresLegacyIE { get; set; }

		public GlobalMasterPage()
		{
			//TODO: in the future, it is best to revert this
			RequiresLegacyIE = true;
		}

        protected void Page_Load(object sender, EventArgs e)
        {
			if (RequiresLegacyIE)
			{
				var meta = new HtmlMeta {Name = "IE7Compatible", HttpEquiv = "X-UA-Compatible", Content = "IE=7"};
				Page.Header.Controls.Add(meta);
			}
			else
			{
				// Still need to force IE to use IE10 mode because Compatibility View may be turned on for intranet websites. 
				// This will make IE to run in compatibility mode. Some pages designed for IE10+ will not be rendered properly in that case
				var meta = new HtmlMeta { Name = "IE7Compatible", HttpEquiv = "X-UA-Compatible", Content = "IE=10" };
				Page.Header.Controls.Add(meta);
			}

            if (IsPostBack)
                return;

			LogoutLink.HRef = "~/Pages/Login/Logout.aspx" + "?ReturnUrl=" + this.Request.Url.AbsoluteUri;

			if (SessionManager.Current != null && SessionManager.Current.User != null && SessionManager.Current.User.WarningMessages.Count > 0)
            {
                if (!SessionManager.Current.User.WarningsDisplayed)
                {
                    SessionManager.Current.User.WarningsDisplayed = true;
                    SessionManager.InitializeSession(SessionManager.Current);
                    LoginWarningsDialog.Show();
                }
            }

            if (ConfigurationManager.AppSettings.GetValues("CachePages")[0].Equals("false"))
            {
                Response.CacheControl = "no-cache";
                Response.AddHeader("Pragma", "no-cache");
                Response.Expires = -1;
            }

            AddIE6PngBugFixCSS();
            if (SessionManager.Current != null && SessionManager.Current.User != null)
            {
                var id = SessionManager.Current.User.Identity as CustomIdentity;

                if (DisplayUserInformationPanel)
                {
                    Username.Text = id != null ? id.DisplayName : "unknown";

                    try
                    {
                        var alertControl = (AlertIndicator)LoadControl("~/Controls/AlertIndicator.ascx");
                        AlertIndicatorPlaceHolder.Controls.Add(alertControl);
                    }
                    catch (Exception)
                    {
                        //No permissions for Alerts, control won't be displayed
                        //hide table cell that contains the control.
                        AlertIndicatorCell.Visible = false;
                    }
                    try
                    {
                        var warningControl = (LoginWarningIndicator)LoadControl("~/Controls/LoginWarningIndicator.ascx");
                        LoginIndicatorPlaceHolder.Controls.Add(warningControl);
                    }
                    catch (Exception)
                    {
                        //No permissions for Warning Indicator, control won't be displayed
                        //hide table cell that contains the control.
                        LoginWarningCell.Visible = false;
                    }
                }
                else
                {
                    UserInformationCell.Width = Unit.Percentage(0);
                    MenuCell.Width = Unit.Percentage(100);
                }
            }


           
        }

        private void AddIE6PngBugFixCSS()
        {
            IE6PNGBugFixCSS.InnerHtml = @"
            input, img
            {
                background-image: expression
                (
                        this.src.toLowerCase().indexOf('.png')>-1?
                        (
                            this.runtimeStyle.backgroundImage = ""none"",
                            this.runtimeStyle.filter = ""progid:DXImageTransform.Microsoft.AlphaImageLoader(src='"" + this.src + ""', sizingMethod='image')"",
                            this.src = """ + Page.ResolveClientUrl("~/App_Themes/Default/Images/blank.gif") + @"""
                        )
                        
                );
            }
        ";
        }

        protected void GlobalScriptManager_AsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e)
        {
            GlobalScriptManager.AsyncPostBackErrorMessage = ExceptionHandler.ThrowAJAXException(e.Exception);
        }

        protected void OnMainMenuDataBound(object sender, EventArgs e)
        {
            // Yes this looks hacky
            // if (!LicenseInformation.IsFeatureAuthorized("ImageServer.QualityControl"))
            {
                var menuPath = string.Format("{0}/{1}", Titles.Admin, Titles.QCSummaryPageTitle);
                var menu = MainMenu.FindItem(menuPath);
                if (menu != null && menu.Parent!=null)
                {
                    menu.Parent.ChildItems.Remove(menu);
                }
            }
        }
    }
}