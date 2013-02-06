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
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Web.Common.Security;
using ClearCanvas.Web.Enterprise.Authentication;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Login
{
    public partial class PasswordExpiredDialog : UserControl
    {
        public void Show(string username, string password)
        {
            Username.Text = username;
            OriginalPassword.Value = password;
            ErrorMessagePanel.Visible = false;
            Panel1.DefaultButton = "OKButton";
            ModalDialog1.Show();

            SetInputFocus(NewPassword);
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
                    if (!NewPassword.Text.Equals(ConfirmNewPassword.Text) || NewPassword.Text.Equals(string.Empty))
                    {
                        ErrorMessage.Text = ErrorMessages.PasswordsDontMatch;
                        ErrorMessagePanel.Visible = true;
                    }
                    else
                    {
                        service.ChangePassword(Username.Text, OriginalPassword.Value, NewPassword.Text);
                        SessionManager.InitializeSession(Username.Text, NewPassword.Text);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage.Text = ex.Message;
                    ErrorMessagePanel.Visible = true;
                    NewPassword.Focus();
					// May want to elimiate this.
					Platform.Log(LogLevel.Error, ex, "Unexpected exception changing password: {0}.", ex.Message);
				}
            }
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