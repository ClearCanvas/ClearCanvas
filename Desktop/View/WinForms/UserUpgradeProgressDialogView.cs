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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.Desktop.View.WinForms
{
	[ExtensionOf(typeof(UserUpgradeProgressDialogViewExtensionPoint))]
	internal class UserUpgradeProgressDialogView : WinFormsView, IUserUpgradeProgressDialogView
	{
		private UserUpgradeProgressForm _form;

		#region IUserUpgradeProgressDialogView Members

		public void RunModal(string title, string startupMessage)
		{
			_form = new UserUpgradeProgressForm(title) { Message = startupMessage };
			_form.ShowDialog();
		}

		public void SetMessage(string message)
		{
			_form.Message = message;
		}

		public void SetProgressPercent(int progressPercent)
		{
			_form.ProgressPercent = progressPercent;
		}

		public void Close(string failureMessage)
		{
			_form.Close();

			if (String.IsNullOrEmpty(failureMessage))
				return;

			new MessageBox().Show(failureMessage, MessageBoxActions.Ok);
		}

		#endregion

		public override object GuiElement
		{
			get { throw new NotImplementedException(); }
		}
	}
}
