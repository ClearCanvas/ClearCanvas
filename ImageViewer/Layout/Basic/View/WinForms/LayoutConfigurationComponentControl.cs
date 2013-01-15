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

using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="LayoutSettingsApplicationComponent"/>
    /// </summary>
    public partial class LayoutConfigurationComponentControl : UserControl
    {
        private LayoutConfigurationComponent _component;

        public LayoutConfigurationComponentControl(LayoutConfigurationComponent component)
        {
            InitializeComponent();

            _component = component;

			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component.Layouts;
			_modality.DataSource = bindingSource;
			_modality.DisplayMember = "Text";

			//these values are just constants, so we won't databind them, it's unnecessary.
			_imageBoxRows.Minimum = 1;
			_imageBoxColumns.Minimum = 1;
			_tileRows.Minimum = 1;
			_tileColumns.Minimum = 1;

			_imageBoxRows.Maximum = _component.MaximumImageBoxRows;
			_imageBoxColumns.Maximum = _component.MaximumImageBoxColumns;
			_tileRows.Maximum = _component.MaximumTileRows;
			_tileColumns.Maximum = _component.MaximumTileColumns;

			_imageBoxRows.DataBindings.Add("Value", bindingSource, "ImageBoxRows", true, DataSourceUpdateMode.OnPropertyChanged);
			_imageBoxColumns.DataBindings.Add("Value", bindingSource, "ImageBoxColumns", true, DataSourceUpdateMode.OnPropertyChanged);
			_tileRows.DataBindings.Add("Value", bindingSource, "TileRows", true, DataSourceUpdateMode.OnPropertyChanged);
			_tileColumns.DataBindings.Add("Value", bindingSource, "TileColumns", true, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
