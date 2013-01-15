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
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
	public partial class MouseImageViewerToolPropertyComponentControl : UserControl
	{
		public MouseImageViewerToolPropertyComponentControl(MouseImageViewerToolPropertyComponent component)
		{
			InitializeComponent();

			_chkInitiallySelected.DataBindings.Add("Checked", component, "InitiallyActive", false, DataSourceUpdateMode.OnPropertyChanged);

			_cboActiveMouseButtons.Format += OnCboActiveMouseButtonsFormat;
			_cboActiveMouseButtons.SelectedIndexChanged += OnComboBoxSelectedItemChangedUpdate;
			_cboActiveMouseButtons.Items.Add(XMouseButtons.Left);
			_cboActiveMouseButtons.Items.Add(XMouseButtons.Right);
			_cboActiveMouseButtons.Items.Add(XMouseButtons.Middle);
			_cboActiveMouseButtons.Items.Add(XMouseButtons.XButton1);
			_cboActiveMouseButtons.Items.Add(XMouseButtons.XButton2);
			_cboActiveMouseButtons.SelectedIndex = 0;
			_cboActiveMouseButtons.DataBindings.Add("SelectedItem", component, "ActiveMouseButtons", true, DataSourceUpdateMode.OnPropertyChanged);

			_cboGlobalMouseButtons.Format += OnCboActiveMouseButtonsFormat;
			_cboGlobalMouseButtons.SelectedIndexChanged += OnComboBoxSelectedItemChangedUpdate;
			_cboGlobalMouseButtons.Items.Add(XMouseButtons.None);
			_cboGlobalMouseButtons.Items.Add(XMouseButtons.Left);
			_cboGlobalMouseButtons.Items.Add(XMouseButtons.Right);
			_cboGlobalMouseButtons.Items.Add(XMouseButtons.Middle);
			_cboGlobalMouseButtons.Items.Add(XMouseButtons.XButton1);
			_cboGlobalMouseButtons.Items.Add(XMouseButtons.XButton2);
			_cboGlobalMouseButtons.SelectedIndex = 0;
			_cboGlobalMouseButtons.DataBindings.Add("SelectedItem", component, "GlobalMouseButtons", true, DataSourceUpdateMode.OnPropertyChanged);

			Binding keyModifierBinding = new Binding("KeyModifiers", component, "GlobalModifiers", true, DataSourceUpdateMode.OnPropertyChanged);
			keyModifierBinding.Format += OnKeyModifierBindingConvert;
			keyModifierBinding.Parse += OnKeyModifierBindingConvert;
			_chkGlobalModifiers.DataBindings.Add(keyModifierBinding);
		}

		private static void OnComboBoxSelectedItemChangedUpdate(object sender, EventArgs e)
		{
			var comboBox = sender as ComboBox;
			if (comboBox != null)
			{
				// someone needs to explain why data binding SelectedItem on property change doesn't work out of the box
				var binding = comboBox.DataBindings["SelectedItem"];
				if (binding != null && binding.DataSourceUpdateMode == DataSourceUpdateMode.OnPropertyChanged)
					binding.WriteValue();
			}
		}

		private static void OnCboActiveMouseButtonsFormat(object sender, ListControlConvertEventArgs e)
		{
			if (e.DesiredType == typeof (string))
			{
				e.Value = TypeDescriptor.GetConverter(typeof (XMouseButtons)).ConvertToString(e.ListItem);
			}
		}

		private static void OnKeyModifierBindingConvert(object sender, ConvertEventArgs e)
		{
			if (e.Value is XKeys && e.DesiredType == typeof (ModifierFlags))
			{
				ModifierFlags result = ModifierFlags.None;
				XKeys value = (XKeys) e.Value;
				if ((value & XKeys.Control) == XKeys.Control)
					result = result | ModifierFlags.Control;
				if ((value & XKeys.Alt) == XKeys.Alt)
					result = result | ModifierFlags.Alt;
				if ((value & XKeys.Shift) == XKeys.Shift)
					result = result | ModifierFlags.Shift;
				e.Value = result;
			}
			else if (e.Value is ModifierFlags && e.DesiredType == typeof (XKeys))
			{
				XKeys result = XKeys.None;
				ModifierFlags value = (ModifierFlags) e.Value;
				if ((value & ModifierFlags.Control) == ModifierFlags.Control)
					result = result | XKeys.Control;
				if ((value & ModifierFlags.Alt) == ModifierFlags.Alt)
					result = result | XKeys.Alt;
				if ((value & ModifierFlags.Shift) == ModifierFlags.Shift)
					result = result | XKeys.Shift;
				e.Value = result;
			}
		}

		private void OnCboGlobalMouseButtonsSelectedIndexChanged(object sender, EventArgs e)
		{
			_chkGlobalModifiers.Enabled = (XMouseButtons) _cboGlobalMouseButtons.SelectedItem != XMouseButtons.None;
		}
	}
}