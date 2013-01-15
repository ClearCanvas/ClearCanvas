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
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PresetVoiLutConfigurationComponent"/>
    /// </summary>
	public partial class PresetVoiLutConfigurationComponentControl : ClearCanvas.Desktop.View.WinForms.ApplicationComponentUserControl
    {
        private readonly PresetVoiLutConfigurationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PresetVoiLutConfigurationComponentControl(PresetVoiLutConfigurationComponent component)
            :base(component)
        {
			_component = component;
			
			InitializeComponent();

			BindingSource source = new BindingSource();
        	source.DataSource = _component;

			OnAddPresetVisibleChanged(null, EventArgs.Empty);
			
			_presetVoiLuts.Table = _component.VoiLutPresets;
			_presetVoiLuts.ToolbarModel = _component.ToolbarModel;
        	_presetVoiLuts.MenuModel = _component.ContextMenuModel;

			_presetVoiLuts.DataBindings.Add("Selection", source, "Selection", true, DataSourceUpdateMode.OnPropertyChanged);

			_comboModality.DataSource = _component.Modalities;
        	_comboModality.DataBindings.Add("Value", source, "SelectedModality", true, DataSourceUpdateMode.OnPropertyChanged);

        	_comboAddPreset.DataSource = _component.AvailableAddFactories;
        	_comboAddPreset.DisplayMember = "Description";
			_comboAddPreset.DataBindings.Add("Visible", source, "HasMultipleFactories", true, DataSourceUpdateMode.Never);
			_comboAddPreset.DataBindings.Add("Value", source, "SelectedAddFactory", true, DataSourceUpdateMode.OnPropertyChanged);

			_addPresetButton.DataBindings.Add("Visible", source, "HasMultipleFactories", true, DataSourceUpdateMode.Never);
			_addPresetButton.DataBindings.Add("Enabled", source, "AddEnabled", true, DataSourceUpdateMode.Never);

			_comboAddPreset.VisibleChanged += new System.EventHandler(OnAddPresetVisibleChanged);
			
			_addPresetButton.Click += delegate { _component.OnAdd(); };
			_presetVoiLuts.ItemDoubleClicked += delegate { _component.OnEditSelected(); };
        }

		void OnAddPresetVisibleChanged(object sender, System.EventArgs e)
		{
			if (!_comboAddPreset.Visible)
			{
				_tableLayoutPanel.SetRowSpan(_presetVoiLuts, 2);
			}
			else
			{
				_tableLayoutPanel.SetRowSpan(_presetVoiLuts, 1);
			}
		}
    }
}
