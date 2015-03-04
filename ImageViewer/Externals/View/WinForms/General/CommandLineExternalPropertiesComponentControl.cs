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
using ClearCanvas.ImageViewer.Externals.General;
using ClearCanvas.ImageViewer.Externals.View.WinForms.Properties;
using MessageBox=System.Windows.Forms.MessageBox;

namespace ClearCanvas.ImageViewer.Externals.View.WinForms.General
{
	public partial class CommandLineExternalPropertiesComponentControl : ApplicationComponentUserControl
	{
		private readonly string _helpMessage = string.Empty;

		public CommandLineExternalPropertiesComponentControl(CommandLineExternalPropertiesComponent component) : base(component)
		{
			InitializeComponent();

			base.ErrorProvider.SetIconPadding(_txtCommand, _btnCommand.Width);

			_helpMessage = component.ArgumentFieldsHelpText;
			_lnkHelpFields.Visible = !string.IsNullOrEmpty(_helpMessage);
			_dlgCommand.Filter = string.Format("{0}|*.exe|{1}|*.*", SR.LabelExeFilesFilter, SR.LabelAllFilesFilter);

			_txtName.DataBindings.Add("Text", component, "Label", false, DataSourceUpdateMode.OnPropertyChanged);
			_txtCommand.DataBindings.Add("Text", component, "Command", false, DataSourceUpdateMode.OnPropertyChanged);
			_txtWorkingDir.DataBindings.Add("Text", component, "WorkingDirectory", false, DataSourceUpdateMode.OnPropertyChanged);
			_txtArguments.DataBindings.Add("Text", component, "Arguments", false, DataSourceUpdateMode.OnPropertyChanged);
			_chkAllowMultiValueFields.DataBindings.Add("Checked", component, "AllowMultiValueFields", false, DataSourceUpdateMode.OnPropertyChanged);
			_txtMultiValueFieldSeparator.DataBindings.Add("Text", component, "MultiValueFieldSeparator", false, DataSourceUpdateMode.OnPropertyChanged);
			_txtMultiValueFieldSeparator.DataBindings.Add("Enabled", component, "AllowMultiValueFields", false, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _lnkHelpFields_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			MessageBox.Show(this, _helpMessage, Resources.TitleHelpSpecialFields);
		}

		private void _btnCommand_Click(object sender, EventArgs e)
		{
			_dlgCommand.FileName = _txtCommand.Text;
			if (_dlgCommand.ShowDialog(this) == DialogResult.OK)
				_txtCommand.Text = _dlgCommand.FileName;
		}
	}
}