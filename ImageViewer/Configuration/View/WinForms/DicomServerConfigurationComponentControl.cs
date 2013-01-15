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

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomServerConfigurationComponent"/>
    /// </summary>
    public partial class DicomServerConfigurationComponentControl : ApplicationComponentUserControl
    {
        private DicomServerConfigurationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerConfigurationComponentControl(DicomServerConfigurationComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

			_aeTitle.DataBindings.Add("Value", _component, "AETitle", true, DataSourceUpdateMode.OnPropertyChanged);
			Binding portBinding = new Binding("Value", _component, "Port", true, DataSourceUpdateMode.OnPropertyChanged);
			portBinding.Format += new ConvertEventHandler(OnPortBindingFormat);
			portBinding.Parse += new ConvertEventHandler(OnPortBindingParse);
			_port.DataBindings.Add(portBinding);
        }

		void OnPortBindingFormat(object sender, ConvertEventArgs e)
		{
			if (e.DesiredType != typeof(string))
				return;

			if ((int)e.Value <= 0)
				e.Value = "";
			else
				e.Value = e.Value.ToString();
		}

		void OnPortBindingParse(object sender, ConvertEventArgs e)
		{
			if (e.DesiredType != typeof(int))
				return;

			int value;
			if (!(e.Value is string) || !int.TryParse((string)e.Value, out value))
			{
				e.Value = 0;
			}
			else
			{
				e.Value = value;
			}
		}
    }
}
