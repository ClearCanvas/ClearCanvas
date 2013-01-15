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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Volume.Mpr.Configuration;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Volume.Mpr.View.WinForms
{
	public partial class MprConfigurationComponentControl : ApplicationComponentUserControl
	{
		private MprConfigurationComponent _component;

		public MprConfigurationComponentControl(MprConfigurationComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;

			_txtProportionalSliceSpacing.DataBindings.Add("Text", _component, "SliceSpacingFactor", true, DataSourceUpdateMode.OnPropertyChanged);
			_txtProportionalSliceSpacing.DataBindings.Add("Enabled", _component, "ProportionalSliceSpacing", true, DataSourceUpdateMode.OnPropertyChanged);
			_radAutomaticSliceSpacing.DataBindings.Add("Checked", _component, "AutomaticSliceSpacing", false, DataSourceUpdateMode.OnPropertyChanged);
			_radProportionalSliceSpacing.DataBindings.Add("Checked", _component, "ProportionalSliceSpacing", false, DataSourceUpdateMode.OnPropertyChanged);

			base.ErrorProvider.SetIconAlignment(_txtProportionalSliceSpacing, ErrorIconAlignment.MiddleLeft);
		}
	}
}