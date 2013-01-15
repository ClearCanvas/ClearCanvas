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

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ProcedureTypeGroupSummaryComponent"/>
    /// </summary>
    public partial class ProcedureTypeGroupSummaryComponentControl : ApplicationComponentUserControl
    {
        private readonly ProcedureTypeGroupSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProcedureTypeGroupSummaryComponentControl(ProcedureTypeGroupSummaryComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

			_procedureTypeGroupTableView.ToolbarModel = _component.SummaryTableActionModel;
			_procedureTypeGroupTableView.MenuModel = _component.SummaryTableActionModel;

			_procedureTypeGroupTableView.Table = _component.SummaryTable;
			_procedureTypeGroupTableView.DataBindings.Add("Selection", _component, "SummarySelection", true, DataSourceUpdateMode.OnPropertyChanged);

			_category.DataSource = _component.CategoryChoices;
			_category.DataBindings.Add("Value", _component, "SelectedCategory", true, DataSourceUpdateMode.OnPropertyChanged);
			_category.Format += delegate(object sender, ListControlConvertEventArgs args) { args.Value = _component.FormatCategory(args.ListItem); };

			_okButton.DataBindings.Add("Visible", _component, "ShowAcceptCancelButtons");
			_okButton.DataBindings.Add("Enabled", _component, "AcceptEnabled");
			_cancelButton.DataBindings.Add("Visible", _component, "ShowAcceptCancelButtons");
		}

		private void _searchButton_Click(object sender, EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.Search();
			}
		}

		private void _clearButton_Click(object sender, EventArgs e)
		{
			_component.SelectedCategory = _component.NullFilter;
			_component.Search();
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void _procedureTypeGroupTableView_ItemDoubleClicked(object sender, EventArgs e)
		{
			_component.DoubleClickSelectedItem();
		}
	}
}
