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
using System.Reflection;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ValidationEditorComponent"/>
    /// </summary>
    public partial class ValidationEditorComponentControl : ApplicationComponentUserControl
    {
        private ValidationEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationEditorComponentControl(ValidationEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _propertiesTableView.Table = _component.Rules;
            _propertiesTableView.ToolbarModel = _component.RulesActionModel;
            _propertiesTableView.MenuModel = _component.RulesActionModel;
            _propertiesTableView.DataBindings.Add("Selection", _component, "SelectedRule", true, DataSourceUpdateMode.OnPropertyChanged);
            _testButton.DataBindings.Add("Enabled", _component, "CanTestRules");

            foreach (PropertyInfo item in _component.ComponentPropertyChoices)
            {
                _propertiesMenu.Items.Add(item.Name);
            }

            Control editor = (Control)_component.EditorComponentHost.ComponentView.GuiElement;
            editor.Dock = DockStyle.Fill;
            _editorPanel.Controls.Add(editor);
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void _testButton_Click(object sender, EventArgs e)
        {
            _component.TestRules();
        }

        private void _macroButton_Click(object sender, EventArgs e)
        {
            _propertiesMenu.Show(_macroButton, new Point(0, _macroButton.Height), ToolStripDropDownDirection.BelowRight);
        }

        private void _propertiesMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            _component.InsertText(e.ClickedItem.Text);
        }
    }
}
