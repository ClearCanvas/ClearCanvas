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
using ClearCanvas.Utilities.DicomEditor.Tools;

namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomEditorCreateToolComponent"/>
    /// </summary>
    public partial class DicomEditorCreateToolComponentControl : CustomUserControl
    {
        private DicomEditorCreateToolComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomEditorCreateToolComponentControl(DicomEditorCreateToolComponent component)
        {
            InitializeComponent();

            _component = component;

            _group.DataBindings.Add("Text", _component, "Group", true, DataSourceUpdateMode.OnPropertyChanged);
            _element.DataBindings.Add("Text", _component, "Element", true, DataSourceUpdateMode.OnPropertyChanged);
            _tagName.DataBindings.Add("Value", _component, "TagName", true, DataSourceUpdateMode.OnPropertyChanged);
            _vr.DataBindings.Add("Value", _component, "Vr", true, DataSourceUpdateMode.OnPropertyChanged);
            _vr.DataBindings.Add("Enabled", _component, "VrEnabled", true, DataSourceUpdateMode.Never);
            _value.DataBindings.Add("Value", _component, "Value", true, DataSourceUpdateMode.OnPropertyChanged);
            _accept.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.Never);

			base.AcceptButton = _accept;
			base.CancelButton = _cancel;
        }

        private void _accept_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancel_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
