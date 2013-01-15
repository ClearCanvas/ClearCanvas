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
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using System.Drawing;
using ClearCanvas.Utilities.Manifest;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	public partial class LoginForm : Form
	{
		private string[] _facilityChoices;
		private Point _refPoint;

		public LoginForm()
		{
			// Need to explicitely dismiss the splash screen here, as the login dialog is shown before the desktop window, which is normally
			// responsible for dismissing it.
#if !MONO
			SplashScreenManager.DismissSplashScreen(this);
#endif

			InitializeComponent();

            _manifest.Visible = !ManifestVerification.Valid;           
		}

		public void SetMode(LoginDialogMode mode)
		{
			_userName.Enabled = mode == LoginDialogMode.InitialLogin;
			_facility.Enabled = mode == LoginDialogMode.InitialLogin;
		}

		public string[] FacilityChoices
		{
			get { return _facilityChoices; }
			set
			{
				_facilityChoices = value;
				_facility.Items.Clear();
				_facility.Items.AddRange(_facilityChoices);
			}
		}

		public string UserName
		{
			get { return _userName.Text; }
			set { _userName.Text = value; }
		}

		public string Password
		{
			get { return _password.Text; }
		}

		public string SelectedFacility
		{
			get { return (string)_facility.SelectedItem; }
			set
			{
				_facility.SelectedItem = value;
			}
		}

		private void _loginButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void LoginForm_Load(object sender, EventArgs e)
		{
			// depending on use-case, the username may already be filled in
			if (string.IsNullOrEmpty(_userName.Text))
				_userName.Select();
			else
				_password.Select();
		}

		private void _userName_TextChanged(object sender, EventArgs e)
		{
			UpdateButtonStates();
		}

		private void _password_TextChanged(object sender, EventArgs e)
		{
			UpdateButtonStates();
		}

		private void _facility_SelectedValueChanged(object sender, EventArgs e)
		{
			UpdateButtonStates();
		}

		private void UpdateButtonStates()
		{
		    bool ok = !string.IsNullOrEmpty(_userName.Text) && !string.IsNullOrEmpty(_password.Text);
			_loginButton.Enabled = ok;
		}

		private void LoginForm_MouseDown(object sender, MouseEventArgs e)
		{
			_refPoint = new Point(e.X, e.Y);
		}

		private void LoginForm_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.Left += (e.X - _refPoint.X);
				this.Top += (e.Y - _refPoint.Y);
			}
		}
	}
}
