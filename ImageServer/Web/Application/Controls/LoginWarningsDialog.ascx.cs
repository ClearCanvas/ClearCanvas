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
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public partial class LoginWarningsDialog : UserControl
    {
        public class LoginMessage
        {
            public string Message { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var loginList = new List<LoginMessage>();
                if (SessionManager.Current != null)
                {
                    foreach (string message in SessionManager.Current.User.WarningMessages)
                    {
                        loginList.Add(new LoginMessage { Message = message });
                    }
                }
                if (WarningListing != null)
                {
                    WarningListing.DataSource = loginList;           
                }
            }
            base.OnInit(e);
        }

        public void Show()
        {
            WarningListing.DataBind();
            ModalDialog1.Show();
        }

        public void DismissLoginWarnings_Click(object sender, EventArgs e)
        {
            ModalDialog1.Hide();            
        }
    }
}