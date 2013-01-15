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

using System.Drawing;
using ClearCanvas.Common;
using System.Windows.Forms;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	[ExtensionOf(typeof(LoginDialogExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class LoginDialog : ILoginDialog
	{
		private readonly LoginForm _form;
		private LoginDialogMode _mode;

		public LoginDialog()
		{
			_form = new LoginForm();
		}

		#region ILoginDialog Members

		public bool Show()
		{
			System.Windows.Forms.Application.EnableVisualStyles();

			// if location was not set manually, centre the dialog in the screen
			_form.StartPosition = _form.Location == Point.Empty ? FormStartPosition.CenterScreen : FormStartPosition.Manual;

			return _form.ShowDialog() == DialogResult.OK;
		}

		public Point Location
		{
			get { return _form.Location; }
			set { _form.Location = value; }
		}

		public LoginDialogMode Mode
		{
			get { return _mode; }
			set
			{
				_mode = value;
				_form.SetMode(_mode);
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
		}

		public string Facility
		{
			get { return _form.SelectedFacility; }
			set { _form.SelectedFacility = value; }
		}

		public string[] FacilityChoices
		{
			get { return _form.FacilityChoices; }
			set { _form.FacilityChoices = value; }
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
