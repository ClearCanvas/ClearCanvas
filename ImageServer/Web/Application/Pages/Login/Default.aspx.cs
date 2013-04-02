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
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Security;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common;
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
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ForeachExtension<ILoginPageExtension>(ext => ext.OnLoginPageInit(this));

            SplashScreen.ImageUrl = "~/App_Themes/" + Theme + "/images/LoginSplash.png";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SessionManager.Current != null)
            {
                // already logged in. Maybe from a different page
                HttpContext.Current.Response.Redirect(FormsAuthentication.GetRedirectUrl(SessionManager.Current.Credentials.UserName, false), true);
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

            // Set the size of LoginSplash panel to be the same as the splash screen image
            // This (together with setting margin to auto in the css) will center the image
            using(System.Drawing.Image image = System.Drawing.Image.FromFile(this.Server.MapPath(SplashScreen.ImageUrl)))
            {
                LoginSplash.Width = image.Width;
                LoginSplash.Height = image.Height;

                ErrorMessagePanel.Width = image.Width;
            }
           
        }

        protected void LoginClicked(object sender, EventArgs e)
        {
            if (SessionManager.Current != null)
            {
                // already logged in. Maybe from different page
                HttpContext.Current.Response.Redirect(FormsAuthentication.GetRedirectUrl(SessionManager.Current.Credentials.UserName, false), true);
            } 

            try
            {
                SessionManager.InitializeSession(UserName.Text, Password.Text);

				UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
					EventIdentificationContentsEventOutcomeIndicator.Success, UserAuthenticationEventType.Login);
				audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, SessionManager.Current.Credentials.DisplayName));
				ServerPlatform.LogAuditMessage(audit);
			}
            catch (PasswordExpiredException)
            {
                Platform.Log(LogLevel.Info, "Password for {0} has expired. Requesting new password.",UserName.Text);
                PasswordExpiredDialog.Show(UserName.Text, Password.Text);

				UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
					EventIdentificationContentsEventOutcomeIndicator.Success, UserAuthenticationEventType.Login);
				audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, null));
				ServerPlatform.LogAuditMessage(audit);
			}
            catch (UserAccessDeniedException ex)
            {
                Platform.Log(LogLevel.Warn, "Login unsuccessful for {0}.  {1}", UserName.Text, ErrorMessages.UserAccessDenied);
                Platform.Log(LogLevel.Debug, ex, ex.Message);
                ShowError(ErrorMessages.UserAccessDenied);
                UserName.Focus();

                UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
                    EventIdentificationContentsEventOutcomeIndicator.SeriousFailureActionTerminated, UserAuthenticationEventType.Login);
                audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, null));
                ServerPlatform.LogAuditMessage(audit);
            }
            catch (CommunicationException ex)
            {
                Platform.Log(LogLevel.Error, ex, "Unable to contact A/A server");
                ShowError(ErrorMessages.CannotContactEnterpriseServer);

				UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
					EventIdentificationContentsEventOutcomeIndicator.MajorFailureActionMadeUnavailable, UserAuthenticationEventType.Login);
				audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, null));
				ServerPlatform.LogAuditMessage(audit);
			}
            catch (ArgumentException ex)
            {
                Platform.Log(LogLevel.Warn, ex.Message);
                Platform.Log(LogLevel.Debug, ex, "Login error:");
                ShowError(ex.Message);

                UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
                    EventIdentificationContentsEventOutcomeIndicator.MajorFailureActionMadeUnavailable, UserAuthenticationEventType.Login);
                audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, null));
                ServerPlatform.LogAuditMessage(audit);
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "Login error:");
                ShowError(ex.Message);

				UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(ServerPlatform.AuditSource,
					EventIdentificationContentsEventOutcomeIndicator.MajorFailureActionMadeUnavailable, UserAuthenticationEventType.Login);
				audit.AddUserParticipant(new AuditPersonActiveParticipant(UserName.Text, null, null));
				ServerPlatform.LogAuditMessage(audit);
			}
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
