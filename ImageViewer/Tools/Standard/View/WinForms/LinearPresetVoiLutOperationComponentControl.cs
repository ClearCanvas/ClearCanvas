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
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	public partial class LinearPresetVoiLutOperationComponentControl : ClearCanvas.Desktop.View.WinForms.ApplicationComponentUserControl
	{
		private readonly LinearPresetVoiLutOperationComponent _component;

		public LinearPresetVoiLutOperationComponentControl(LinearPresetVoiLutOperationComponent component)
		{
			_component = component;
			InitializeComponent();

			BindingSource source = new BindingSource();
			source.DataSource = _component;

			_nameField.DataBindings.Add("Value", source, "PresetName", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowWidth.DataBindings.Add("Value", source, "WindowWidth", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowCenter.DataBindings.Add("Value", source, "WindowCenter", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}
