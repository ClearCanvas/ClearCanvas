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

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ImportDiagnosticServicesComponent"/>
    /// </summary>
    public partial class DataImportComponentControl : ApplicationComponentUserControl
    {
        private DataImportComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DataImportComponentControl(DataImportComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _importer.DataSource = _component.ImportTypeChoices;
            _importer.DataBindings.Add("Value", _component, "ImportType", true, DataSourceUpdateMode.OnPropertyChanged);
            _dataFile.DataBindings.Add("Text", _component, "FileName", true, DataSourceUpdateMode.OnPropertyChanged);
            //_batchSize.DataBindings.Add("Value", _component, "BatchSize", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _browseButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = ".";
            openFileDialog1.Filter = string.Format("{0}|*.csv|{1}|*.*", SR.LabelCsvFileFilter, SR.LabelAllFilesFilter);
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _dataFile.Text = openFileDialog1.FileName;
            }
        }

        private void _startButton_Click(object sender, EventArgs e)
        {
            _component.StartImport();
        }
    }
}
