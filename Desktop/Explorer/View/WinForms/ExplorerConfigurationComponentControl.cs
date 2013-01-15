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

namespace ClearCanvas.Desktop.Explorer.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ExplorerConfigurationComponent"/>.
    /// </summary>
    public partial class ExplorerConfigurationComponentControl : ApplicationComponentUserControl
    {
        private ExplorerConfigurationComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExplorerConfigurationComponentControl(ExplorerConfigurationComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

        	_launchAsShelf.DataBindings.Add("Checked", _component, "LaunchAsShelf", false,
        	                                DataSourceUpdateMode.OnPropertyChanged);

			_launchAsWorkspace.DataBindings.Add("Checked", _component, "LaunchAsWorkspace", false,
									DataSourceUpdateMode.OnPropertyChanged);

			_launchAtStartup.DataBindings.Add("Checked", _component, "LaunchAtStartup", false,
									DataSourceUpdateMode.OnPropertyChanged);
		}
    }
}
