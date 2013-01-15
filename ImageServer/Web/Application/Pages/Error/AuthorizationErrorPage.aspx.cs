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
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Security;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Error
{
    public partial class AuthorizationErrorPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Context.Items[ImageServerConstants.ContextKeys.ErrorMessage] != null) {
                ErrorMessageLabel.Text = Context.Items[ImageServerConstants.ContextKeys.ErrorMessage].ToString();
            } 
            if (Context.Items[ImageServerConstants.ContextKeys.StackTrace] != null)
            {
                StackTraceTextBox.Text = Context.Items[ImageServerConstants.ContextKeys.StackTrace].ToString();
                StackTraceTextBox.Visible = true;
                StackTraceMessage.Visible = true;
            }
            if (Context.Items[ImageServerConstants.ContextKeys.ErrorDescription] != null)
            {
                DescriptionLabel.Text = Context.Items[ImageServerConstants.ContextKeys.ErrorDescription].ToString();
            }

            #region UnitTest
            if (false==String.IsNullOrEmpty(Page.Request.QueryString["test"]))
            {
                StackTraceMessage.Visible = true;
                StackTraceTextBox.Visible = true;
                StackTraceTextBox.Text = "Dummy stack trace";
            }
            #endregion

            SetPageTitle(Titles.AuthorizationErrorPageTitle);
        }

        protected void Logout_Click(Object sender, EventArgs e)
        {
            SessionManager.SignOut();
            Response.Redirect(ImageServerConstants.PageURLs.LoginPage);
        }

        protected void DefaultPage_Click(Object sender, EventArgs e)
        {
            String defaultPageUrl = UserProfile.GetDefaultUrl();
			if (defaultPageUrl == null)
				Response.End();
            Response.Redirect(defaultPageUrl);
        }
    }
}
