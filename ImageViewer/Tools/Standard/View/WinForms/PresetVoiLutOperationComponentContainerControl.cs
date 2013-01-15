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

using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;
using System;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    public partial class PresetVoiLutOperationComponentContainerControl : ApplicationComponentUserControl
    {
        private readonly PresetVoiLutOperationsComponentContainer _component;

		/// <summary>
        /// Constructor
        /// </summary>
        public PresetVoiLutOperationComponentContainerControl(PresetVoiLutOperationsComponentContainer component)
            :base(component)
        {
			_component = component;
			InitializeComponent();

			BindingSource source = new BindingSource();
			source.DataSource = _component;

			_keyStrokeComboBox.DataSource = _component.AvailableKeyStrokes;
			_keyStrokeComboBox.DataBindings.Add("Value", source, "SelectedKeyStroke", true, DataSourceUpdateMode.OnPropertyChanged);

			_okButton.DataBindings.Add("Enabled", source, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			IApplicationComponentView customEditView;
			try
			{
				customEditView = _component.ComponentHost.ComponentView;

				Size sizeBefore = _tableLayoutPanel.Size;

				_tableLayoutPanel.Controls.Add(customEditView.GuiElement as Control);
				_tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

				Size sizeAfter = _tableLayoutPanel.Size;

				this.Size += (sizeAfter - sizeBefore);
			}
			catch(NotSupportedException)
			{
			}

			base.AcceptButton = _okButton;
			base.CancelButton = _cancelButton;

			_cancelButton.Click += delegate { _component.Cancel(); };
			_okButton.Click += delegate { _component.OK(); };
        }
    }
}
