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
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomExplorerConfigurationApplicationComponent"/>
    /// </summary>
    public partial class DicomExplorerConfigurationComponentControl : ApplicationComponentUserControl
    {
        private DicomExplorerConfigurationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomExplorerConfigurationComponentControl(DicomExplorerConfigurationComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

			_selectDefaultServerOnStartup.DataBindings.Add("Checked", bindingSource, "SelectDefaultServerOnStartup", true, DataSourceUpdateMode.OnPropertyChanged);
			_showNumberOfImages.DataBindings.Add("Checked", bindingSource, "ShowNumberOfImagesInStudy", true, DataSourceUpdateMode.OnPropertyChanged);
			_showPhoneticIdeographicNames.DataBindings.Add("Checked", bindingSource, "ShowPhoneticIdeographicNames", true, DataSourceUpdateMode.OnPropertyChanged);
		}
    }
}
