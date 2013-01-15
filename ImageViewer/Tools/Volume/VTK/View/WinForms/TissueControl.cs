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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.ImageViewer.Tools.Volume;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK.View.WinForms
{
	public partial class TissueControl : UserControl
	{
		private TissueSettings _tissueSettings;
		private BindingSource _bindingSource;

		public TissueControl()
		{
			InitializeComponent();

			_bindingSource = new BindingSource();
			_presetComboBox.DataSource = TissueSettings.Presets;
			_presetComboBox.SelectedValueChanged += new EventHandler(OnPresetChanged);
			_surfaceRenderingRadio.Click += new EventHandler(OnSurfaceRenderingRadioClick);
			_volumeRenderingRadio.Click += new EventHandler(OnVolumeRenderingRadioClick);
		}

		void OnVolumeRenderingRadioClick(object sender, EventArgs e)
		{
			_volumeRenderingRadio.Checked = true;
		}

		void OnSurfaceRenderingRadioClick(object sender, EventArgs e)
		{
			_surfaceRenderingRadio.Checked = true;
		}

		public TissueSettings TissueSettings
		{
			get { return _tissueSettings; }
			set
			{
				if (_tissueSettings != value)
				{
					_tissueSettings = value;
					UpdateBindings();
				}
			}
		}

		private void UpdateBindings()
		{
			_bindingSource.Clear();
			_bindingSource.DataSource = _tissueSettings;

			_visibleCheckBox.DataBindings.Clear();
			_visibleCheckBox.DataBindings.Add("Checked", _bindingSource, "Visible", true, DataSourceUpdateMode.OnPropertyChanged);

			_surfaceRenderingRadio.DataBindings.Clear();
			_surfaceRenderingRadio.DataBindings.Add("Enabled", _bindingSource, "Visible", true, DataSourceUpdateMode.OnPropertyChanged);
			_surfaceRenderingRadio.DataBindings.Add("Checked", _bindingSource, "SurfaceRenderingSelected", true, DataSourceUpdateMode.OnPropertyChanged);

			_volumeRenderingRadio.DataBindings.Clear();
			_volumeRenderingRadio.DataBindings.Add("Enabled", _bindingSource, "Visible", true, DataSourceUpdateMode.OnPropertyChanged);
			_volumeRenderingRadio.DataBindings.Add("Checked", _bindingSource, "VolumeRenderingSelected", true, DataSourceUpdateMode.OnPropertyChanged);

			_opacityControl.DataBindings.Clear();
			_opacityControl.DataBindings.Add("Enabled", _bindingSource, "Visible", true, DataSourceUpdateMode.OnPropertyChanged);
			_opacityControl.DataBindings.Add("Minimum", _bindingSource, "MinimumOpacity", true, DataSourceUpdateMode.OnPropertyChanged);
			_opacityControl.DataBindings.Add("Maximum", _bindingSource, "MaximumOpacity", true, DataSourceUpdateMode.OnPropertyChanged);
			_opacityControl.DataBindings.Add("Value", _bindingSource, "Opacity", true, DataSourceUpdateMode.OnPropertyChanged);

			_windowControl.DataBindings.Clear();
			_windowControl.DataBindings.Add("Enabled", _bindingSource, "WindowEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowControl.DataBindings.Add("Minimum", _bindingSource, "MinimumWindow", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowControl.DataBindings.Add("Maximum", _bindingSource, "MaximumWindow", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowControl.DataBindings.Add("Value", _bindingSource, "Window", true, DataSourceUpdateMode.OnPropertyChanged);

			_levelControl.DataBindings.Clear();
			_levelControl.DataBindings.Add("Enabled", _bindingSource, "Visible", true, DataSourceUpdateMode.OnPropertyChanged);
			_levelControl.DataBindings.Add("Minimum", _bindingSource, "MinimumLevel", true, DataSourceUpdateMode.OnPropertyChanged);
			_levelControl.DataBindings.Add("Maximum", _bindingSource, "MaximumLevel", true, DataSourceUpdateMode.OnPropertyChanged);
			_levelControl.DataBindings.Add("Value", _bindingSource, "Level", true, DataSourceUpdateMode.OnPropertyChanged);

			_presetComboBox.DataBindings.Clear();
			_presetComboBox.DataBindings.Add("Enabled", _bindingSource, "Visible", true, DataSourceUpdateMode.OnPropertyChanged);
			_presetComboBox.DataBindings.Add("SelectedItem", _bindingSource, "SelectedPreset", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		void OnPresetChanged(object sender, EventArgs e)
		{
			_tissueSettings.SelectPreset(_presetComboBox.SelectedItem.ToString());
		}
	}
}
