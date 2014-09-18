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
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Security;
using SR = Resources.SR;
using Resources;
using ClearCanvas.ImageServer.Web.Common.Extensions;
using System.Web.UI;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Login
{
    [ExtensibleAttribute(ExtensionPoint=typeof(LoginPageExtensionPoint))]
    public partial class LoginPage : BasePage, ILoginPage
    {
		private string CustomCssUrl;

    	const String SplashImageNamePrefix = "LoginSplash";

		protected String ApplicationName { get; set; }
		protected String SplashScreenUrl { get; set; }
		protected String CssClassName { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

			// Check for customization parameters
			ApplicationName = Request.Params["AppName"] ?? ImageServerConstants.DefaultApplicationName;
			SplashScreenUrl = Request.Params["SplashScreenUrl"] ?? DefaultSplashScreenPath;
			CssClassName = Request.Params["CssClassName"];
			CustomCssUrl = Request.Params["CssUrl"];

			if (!string.IsNullOrEmpty(CssClassName))
			{
				this.PageBody.Attributes["class"] += " " + CssClassName;
			}

			if (!string.IsNullOrEmpty(CustomCssUrl))
			{
				// include the custom css (this will append to the end of the list and will overwrite the default css)
				var stylesheet = new HtmlLink { Href = CustomCssUrl };
				stylesheet.Attributes.Add("rel", "stylesheet");
				stylesheet.Attributes.Add("type", "text/css");
				Page.Header.Controls.Add(stylesheet);
			}

        	SetSplashScreen();
			
            ForeachExtension<ILoginPageExtension>(ext => ext.OnLoginPageInit(this));
        }

		/// <summary>
		/// Return the virtual path for the default splash screen
		/// </summary>
    	protected string DefaultSplashScreenPath
    	{
    		get
    		{
				return string.Format("~/App_Themes/{0}/images/{1}.png", Theme, SplashImageNamePrefix);
    		}
    	}

    	protected void Page_Load(object sender, EventArgs e)
        {
    		if (!Page.IsPostBack)
    		{
				if (SessionManager.Current != null)
				{
					// User has logged in from another page.
					// Make sure the session (based on the cookie) is actually valid before redirecting
					var userId = SessionManager.Current.User.UserName;
					var sessionId = SessionManager.Current.User.SessionTokenId;
					string[] authorityTokens;
					if (SessionManager.VerifySession(userId, sessionId, out authorityTokens, true))
					{
						RedirectAfterLogin();
					}

					// session is invalid, looks like the web server and the authentication server are out of sync? 
					// To be safe, redirect user to the logout page
					var originalRedirectUrl = SessionManager.GetRedirectUrl(SessionManager.Current);
					var logoutUrl = string.Format(ImageServerConstants.PageURLs.LogoutPage, originalRedirectUrl);

					Response.Redirect(Page.ResolveClientUrl(logoutUrl), true);
					return;
				}

				if (!ServerPlatform.IsManifestVerified)
				{
					ManifestWarningTextLabel.Text = SR.NonStandardInstallation;
				}

				VersionLabel.Text = String.IsNullOrEmpty(ServerPlatform.VersionString) ? Resources.SR.Unknown : ServerPlatform.VersionString;
				LanguageLabel.Text = Thread.CurrentThread.CurrentUICulture.NativeName;
				CopyrightLabel.Text = ProductInformation.Copyright;

				DataBind();

				SetPageTitle(Titles.LoginPageTitle);

				UserName.Focus();
	
    		}
        }

        protected void LoginClicked(object sender, EventArgs e)
        {
            if (SessionManager.Current != null)
            {
                // already logged in. Maybe from different page
            	RedirectAfterLogin();
            } 

            try
            {
                SessionManager.InitializeSession(UserName.Text, Password.Text, ApplicationName ?? ImageServerConstants.DefaultApplicationName);

			}
            catch (PasswordExpiredException)
            {
                Platform.Log(LogLevel.Info, "Password for {0} has expired. Requesting new password.",UserName.Text);
                PasswordExpiredDialog.Show(UserName.Text, Password.Text);

			}
            catch (UserAccessDeniedException ex)
            {
                Platform.Log(LogLevel.Warn, "Login unsuccessful for {0}.  {1}", UserName.Text, ErrorMessages.UserAccessDenied);
                Platform.Log(LogLevel.Debug, ex, ex.Message);
                ShowError(ErrorMessages.UserAccessDenied);
                UserName.Focus();
            }
            catch (CommunicationException ex)
            {
                Platform.Log(LogLevel.Error, ex, "Unable to contact A/A server");
                ShowError(ErrorMessages.CannotContactEnterpriseServer);

			}
            catch (ArgumentException ex)
            {
                Platform.Log(LogLevel.Warn, ex.Message);
                Platform.Log(LogLevel.Debug, ex, "Login error:");
                ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "Login error:");
                ShowError(ex.Message);
			}
        }

		private void RedirectAfterLogin()
		{
			//The GetRedirectUrl method returns the URL specified in the query string using the ReturnURL variable name.

			var redirectUrl = SessionManager.GetRedirectUrl(SessionManager.Current);
			HttpContext.Current.Response.Redirect(redirectUrl, true);
		}

		/// <summary>
		/// Sets the splash screen for the current application
		/// </summary>
		private void SetSplashScreen()
		{
			SplashScreen.ImageUrl = SplashScreenUrl;
		}

        public void ChangePassword(object sender, EventArgs e)
        {
            ChangePasswordDialog.Show(true);
        }

        private void ShowError(string error)
        {
            ErrorMessage.Text = error;
            ErrorMessagePanel.Visible = true;
        }

        public Control SplashScreenControl { get { return this.LoginSplash; } }
    }
}
