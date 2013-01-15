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
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	public partial class ConfigurationDialogComponentControl : ApplicationComponentUserControl
	{
		private ConfigurationDialogComponent _component;

		internal ConfigurationDialogComponentControl(ConfigurationDialogComponent component, Control navigatorControl)
			: base(component)
		{
			InitializeComponent();

			_component = component;
			component.PropertyChanged += OnPropertyChanged;

			_warningMessage.DataBindings.Add("Text", _component, "ConfigurationWarning", true);

			UpdateOfflineWarningVisibility();
			_tableLayoutPanel.Controls.Add(navigatorControl, 0, 1);

			Size = _tableLayoutPanel.Size;
			_tableLayoutPanel.SizeChanged += (s, e) => Size = _tableLayoutPanel.Size;
		}

		void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ConfigurationWarning")
				UpdateOfflineWarningVisibility();
		}

		private void UpdateOfflineWarningVisibility()
		{
			var rowStyle = _tableLayoutPanel.RowStyles[0];
			if (String.IsNullOrEmpty(_component.ConfigurationWarning))
			{
				_warningLayoutTable.Visible = false;
				rowStyle.SizeType = SizeType.Absolute;
				rowStyle.Height = 0;
			}
			else
			{
				_warningLayoutTable.Visible = true;
				rowStyle.SizeType = SizeType.AutoSize;
			}
		}
	}
}
