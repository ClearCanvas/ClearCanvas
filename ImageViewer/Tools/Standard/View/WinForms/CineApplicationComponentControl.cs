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

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="CineApplicationComponent"/>
    /// </summary>
    public partial class CineApplicationComponentControl : ApplicationComponentUserControl
    {
        private readonly CineApplicationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public CineApplicationComponentControl(CineApplicationComponent component)
            :base(component)
        {
			_component = component;
			
			InitializeComponent();

			BindingSource source = new BindingSource();
        	source.DataSource = _component;
        	_startForwardButton.DataBindings.Add("Enabled", source, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_startReverseButton.DataBindings.Add("Enabled", source, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
        	_stopButton.DataBindings.Add("Enabled", source, "Running", true, DataSourceUpdateMode.OnPropertyChanged);
			_cineSpeed.DataBindings.Add("Minimum", source, "MinimumScale", true, DataSourceUpdateMode.OnPropertyChanged);
			_cineSpeed.DataBindings.Add("Maximum", source, "MaximumScale", true, DataSourceUpdateMode.OnPropertyChanged);
			_cineSpeed.DataBindings.Add("Value", source, "CurrentScaleValue", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void StartReverseButtonClicked(object sender, EventArgs e)
		{
			_component.Reverse = true;
			_component.StartCine();
		}

		private void StopButtonClicked(object sender, EventArgs e)
		{
			_component.StopCine();
		}

		private void StartForwardButtonClicked(object sender, EventArgs e)
		{
			_component.Reverse = false;
			_component.StartCine();
		}
    }
}
