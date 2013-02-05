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
using System.Globalization;
using System.ServiceModel;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Web.Enterprise.Authentication;
using Resources;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Login
{
    public partial class ChangePasswordDialog : UserControl
    {
        public void Show(bool updateUI)
        {
            ChangePasswordUsername.Text = ((System.Web.UI.WebControls.TextBox)Page.FindControl("UserName")).Text;
            LoginPasswordChange.Checked = true;
            ErrorMessage.Text = String.Empty;
            ErrorMessagePanel.Visible = false;
            Panel1.DefaultButton = "OKButton";
            ModalDialog1.Show();

            SetInputFocus(ChangePasswordUsername);
            

        }

        public void Cancel_Click(object sender, EventArgs e)
        {
            Panel1.DefaultButton = "";
            
            ModalDialog1.Hide();
        }

        public void ChangePassword_Click(object sender, EventArgs e)
        {
            using(LoginService service = new LoginService())
            {
                try
                {
                    SessionInfo session = service.Login(ChangePasswordUsername.Text, OriginalPassword.Text, ImageServerConstants.DefaultApplicationName);

                    if (!NewPassword.Text.Equals(ConfirmNewPassword.Text) || NewPassword.Text.Equals(string.Empty))
                    {
                        ErrorMessage.Text = ErrorMessages.PasswordsDoNotMatch;
                        ErrorMessagePanel.Visible = true;
                    }
                    else
                    {
                        service.ChangePassword(ChangePasswordUsername.Text, OriginalPassword.Text,NewPassword.Text);

                        session = service.Login(ChangePasswordUsername.Text, NewPassword.Text, ImageServerConstants.DefaultApplicationName);
                        SessionManager.InitializeSession(session);

                        if (LoginPasswordChange.Checked)
                        {
                            Response.Redirect(
                                FormsAuthentication.GetRedirectUrl(ChangePasswordUsername.Text, false), false);
                        }
                        else
                        {
                            ModalDialog1.Hide();
                        }
                    }
                }
                catch(ArgumentException ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Unable to change password for {0}: {1}", ChangePasswordUsername.Text, ex.Message);
                    string error = String.Format(ErrorMessages.ChangePasswordError, ex.Message);
                    ShowError(error);
                }
                catch (PasswordExpiredException ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Unable to change password for {0}: {1}", ChangePasswordUsername.Text, ex.Message);
                    ShowError(ErrorMessages.PasswordExpired);
                }
                catch (UserAccessDeniedException ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Unable to change password for {0}: {1}", ChangePasswordUsername.Text, ex.Message);
                    ShowError(ErrorMessages.UserAccessDenied);
                }
                catch (RequestValidationException ex)
                {
                    // NOTE: The server is throwing FaultException<RequestValidationException> when username or password doesn't match the configured policy
                    Platform.Log(LogLevel.Error, ex, "Unable to change password for {0}: {1}", ChangePasswordUsername.Text, ex.Message);
                    
                    string error = String.Format(ErrorMessages.PasswordPolicyNotMet);
                    ShowError(error);
                }
                catch (CommunicationException ex)
                {
                    Platform.Log(LogLevel.Error, ex, ErrorMessages.CannotContactEnterpriseServer);
                    ShowError(ErrorMessages.CannotContactEnterpriseServer);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                    // May want to elimiate this.
                    Platform.Log(LogLevel.Error, ex, "Unexpected exception changing password: {0}.", ex.Message);
                }
            }
        }

        private void ShowError(string error)
        {
            ErrorMessage.Text = error;
            ErrorMessagePanel.Visible = true;
            ChangePasswordUsername.Focus();
        }



        private void SetInputFocus(WebControl control)
        {
            //Note: yes, we need to use javascript to set focus because we are using AjaxPopupExtender

            var script = @"           
                Sys.Application.add_load(function(){
                    // need a bit of delay, can't set focus on something that's still hidden
                    setTimeout(function()
                    {
                        $get('@@INPUT_ID@@').focus();
                    },
                    200);                    
                });
            ";

            script = script.Replace("@@INPUT_ID@@", control.ClientID);
            Page.ClientScript.RegisterStartupScript(GetType(), "SetFocus", script, true);
        }
    }
}