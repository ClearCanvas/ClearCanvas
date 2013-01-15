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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    public partial class SearchComponentControl : ApplicationComponentUserControl
    {
        private readonly SearchComponent _component;

        public SearchComponentControl(SearchComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _accession.DataBindings.Add("Value", _component, "AccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _accession.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _mrn.DataBindings.Add("Value", _component, "Mrn", true, DataSourceUpdateMode.OnPropertyChanged);
            _mrn.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcard.DataBindings.Add("Value", _component, "HealthcardNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcard.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
            _familyName.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _fromDate.DataBindings.Add("Value", _component, "FromDate", true, DataSourceUpdateMode.OnPropertyChanged);
            _fromDate.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _untilDate.DataBindings.Add("Value", _component, "UntilDate", true, DataSourceUpdateMode.OnPropertyChanged);
            _untilDate.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _orderingPractitioner.DataBindings.Add("Value", _component, "OrderingPractitioner", true, DataSourceUpdateMode.OnPropertyChanged);
            _orderingPractitioner.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _orderingPractitioner.LookupHandler = _component.OrderingPractitionerLookupHandler;

            _diagnosticService.DataBindings.Add("Value", _component, "DiagnosticService", true, DataSourceUpdateMode.OnPropertyChanged);
            _diagnosticService.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _diagnosticService.LookupHandler = _component.DiagnosticServiceLookupHandler;

            _procedureType.DataBindings.Add("Value", _component, "ProcedureType", true, DataSourceUpdateMode.OnPropertyChanged);
            _procedureType.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _procedureType.LookupHandler = _component.ProcedureTypeLookupHandler;

            _keepOpen.DataBindings.Add("Checked", _component, "KeepOpen", true, DataSourceUpdateMode.OnPropertyChanged);
			_showDeactivatedChoices.DataBindings.Add("Checked", _component, "IncludeDeactivatedChoices", true, DataSourceUpdateMode.OnPropertyChanged);
			_searchButton.DataBindings.Add("Enabled", _component, "SearchEnabled");
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
            _component.Clear();
        }
    }
}
