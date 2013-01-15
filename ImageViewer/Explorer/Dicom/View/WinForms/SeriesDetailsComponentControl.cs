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
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
	/// Provides a Windows Forms user-interface for <see cref="SeriesDetailsComponent"/>.
    /// </summary>
    public partial class SeriesDetailsComponentControl : ApplicationComponentUserControl
    {
		private ISeriesDetailComponentViewModel _component;

        /// <summary>
        /// Constructor.
        /// </summary>
		public SeriesDetailsComponentControl(SeriesDetailsComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

        	_patientId.Value = _component.PatientId;
			_patientsName.Value = _component.PatientsName;
			_dob.Value = _component.PatientsBirthDate;
			_accessionNumber.Value = _component.AccessionNumber;
			_studyDate.Value = _component.StudyDate;
			_studyDescription.Value = _component.StudyDescription;

        	_seriesTable.Table = _component.SeriesTable;
        	_seriesTable.ToolbarModel = _component.ToolbarActionModel;
        	_seriesTable.MenuModel = _component.ContextMenuActionModel;

        	base.AcceptButton = _close;
			base.CancelButton = _close;
		}

		private void _close_Click(object sender, EventArgs e)
		{
			_component.Close();
		}

		private void _refresh_Click(object sender, EventArgs e)
		{
			_component.Refresh();
		}

		private void _seriesTable_SelectionChanged(object sender, EventArgs e)
		{
			_component.SetSeriesSelection(_seriesTable.Selection);
		}
    }
}
