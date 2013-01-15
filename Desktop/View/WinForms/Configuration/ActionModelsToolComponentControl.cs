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
using System.Linq;
using System.Windows.Forms;
using ClearCanvas.Desktop.Configuration.Tools;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	internal partial class ActionModelsToolComponentControl : UserControl
	{
		public ActionModelsToolComponentControl()
		{
			InitializeComponent();
		}

#if DEBUG

		private readonly ActionModelsToolComponent _component;

		public ActionModelsToolComponentControl(ActionModelsToolComponent component)
			: this()
		{
			_component = component;
			_component.ActiveComponentChanged += _component_ComponentChanged;

			_cboActionModel.Items.AddRange(component.AvailableActionModels.Cast<object>().ToArray());
		}

		private void _component_ComponentChanged(object sender, EventArgs e)
		{
			_pnlHostedControl.Controls.Clear();

			var control = _component.ActiveComponentHost.ComponentView.GuiElement as Control;
			if (control != null)
			{
				_pnlHostedControl.Controls.Add(control);
				control.Dock = DockStyle.Fill;
			}
		}

#endif

		private void button1_Click(object sender, EventArgs e)
		{
#if DEBUG
			_component.Accept();
#endif
		}

		private void button2_Click_1(object sender, EventArgs e)
		{
#if DEBUG
			_component.Cancel();
#endif
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
#if DEBUG
			_component.SelectedActionModel = _cboActionModel.SelectedItem as string;
#endif
		}
	}
}