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
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Configuration.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomServerGroupEditComponent"/>
    /// </summary>
    public partial class DicomServerGroupEditComponentControl : ApplicationComponentUserControl
	{
        private readonly DicomServerGroupEditComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerGroupEditComponentControl(DicomServerGroupEditComponent component)
			: base(component)
		{
            InitializeComponent();

            _component = component;

			this.AcceptButton = _btnAccept;
			this.CancelButton = _btnCancel;
			
			AcceptClicked += new EventHandler(OnAcceptClicked);
            CancelClicked += new EventHandler(OnCancelClicked);

            this._serverGroupName.DataBindings.Add("Text", _component, "ServerGroupName", true, DataSourceUpdateMode.OnPropertyChanged);
            _btnAccept.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        public event EventHandler AcceptClicked
        {
            add { _btnAccept.Click += value; }
            remove { _btnAccept.Click -= value; }
        }

        public event EventHandler CancelClicked
        {
            add { _btnCancel.Click += value; }
            remove { _btnCancel.Click -= value; }
        }

        private void OnAcceptClicked(object sender, EventArgs e)
        {
			_component.Accept();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
