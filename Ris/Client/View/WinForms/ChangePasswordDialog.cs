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
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    [ExtensionOf(typeof(ChangePasswordDialogExtensionPoint))]
    public class ChangePasswordDialog : IChangePasswordDialog
    {
        private ChangePasswordForm _form;

        public ChangePasswordDialog()
        {
            _form = new ChangePasswordForm();
        }

        #region IChangePasswordDialog Members

        public bool Show()
        {
            System.Windows.Forms.Application.EnableVisualStyles();

            if (_form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string UserName
        {
            get { return _form.UserName; }
            set { _form.UserName = value; }
        }

        public string Password
        {
            get { return _form.Password; }
            set { _form.Password = value; }
        }

        public string NewPassword
        {
            get { return _form.NewPassword; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // nothing to do
        }

        #endregion
    }
}
