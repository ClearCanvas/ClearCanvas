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
using System.Web.UI;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups
{
    public partial class PasswordConfirmDialog : System.Web.UI.UserControl
    {

        public delegate void OnOKClickedEventHandler();
        public event OnOKClickedEventHandler OKClicked;

        public string PasswordString
        {
            get
            {
                return Password.Text;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void OKButton_Click(object sender, ImageClickEventArgs e)
        {
            if (OKClicked != null)
                OKClicked();

            Close();
        }

        protected void CancelButton_Click(object sender, ImageClickEventArgs e)
        {
            Close();
        }


        public void Show()
        {
            ModalDialog1.Show();
        }


        public void Close()
        {
            ModalDialog1.Hide();
        }
    }
}