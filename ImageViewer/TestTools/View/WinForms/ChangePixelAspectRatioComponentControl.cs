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

namespace ClearCanvas.ImageViewer.TestTools.View.WinForms
{
    public partial class ChangePixelAspectRatioComponentControl : ApplicationComponentUserControl
    {
        private ChangePixelAspectRatioComponent _component;

        public ChangePixelAspectRatioComponentControl(ChangePixelAspectRatioComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

			_convertDisplaySet.DataBindings.Add("Checked", _component, "ConvertWholeDisplaySet", false, DataSourceUpdateMode.OnPropertyChanged);
			_increasePixelDimensions.DataBindings.Add("Checked", _component, "IncreasePixelDimensions", false, DataSourceUpdateMode.OnPropertyChanged);
			_removeCalibration.DataBindings.Add("Checked", _component, "RemoveCalibration", false, DataSourceUpdateMode.OnPropertyChanged);
			_row.DataBindings.Add("Value", _component, "AspectRatioRow", false, DataSourceUpdateMode.OnPropertyChanged);
			_column.DataBindings.Add("Value", _component, "AspectRatioColumn", false, DataSourceUpdateMode.OnPropertyChanged);

			_ok.Click += delegate { _component.Accept(); };
			_cancel.Click += delegate { _component.Cancel(); };
		}
    }
}
