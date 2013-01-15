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
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="VolumeComponent"/>
    /// </summary>
    public partial class VolumeComponentControl : CustomUserControl
    {
		private BindingSource _bindingSource;
		private VolumeComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public VolumeComponentControl(VolumeComponent component)
        {
			_component = component;

			InitializeComponent();

			AddDefaultTabs();

			this._createVolumeButton.Click += new EventHandler(OnCreateVolumeButtonClick);

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _component;

			_component.AllPropertiesChanged += new EventHandler(Refresh);
			_createVolumeButton.DataBindings.Add("Enabled", _bindingSource, "CreateVolumeEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_tabControl.DataBindings.Add("Enabled", _bindingSource, "VolumeSettingsEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		void AddDefaultTabs()
		{
			for (int i = 0; i < 2; i++)
			{
				TabPage tabPage = new TabPage("Tissue");
				TissueControl control = new TissueControl();
				tabPage.Controls.Add(control);
				control.Dock = DockStyle.Fill;
				_tabControl.TabPages.Add(tabPage);
			}
		}

		void OnCreateVolumeButtonClick(object sender, EventArgs e)
		{
			_component.CreateVolume();
		}

		void Refresh(object sender, EventArgs e)
		{
			_bindingSource.ResetBindings(false);

			UpdateTabControl();
		}

		private void UpdateTabControl()
		{
			if (_component.VolumeGraphics == null)
				return;

			int i = 0;

			foreach (Graphic layer in _component.VolumeGraphics)
			{
				VolumeGraphic volumeLayer = layer as VolumeGraphic;

				if (volumeLayer != null)
				{
					TabPage page = _tabControl.TabPages[i];
					TissueControl control = page.Controls[0] as TissueControl;

					if (control != null)
						control.TissueSettings = volumeLayer.TissueSettings;
				}

				i++;
			}
		}
    }
}
