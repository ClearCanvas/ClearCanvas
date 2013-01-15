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
using ClearCanvas.ImageViewer.Clipboard.CopyToClipboard;

namespace ClearCanvas.ImageViewer.Clipboard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="CopySubsetToClipboardComponent"/>.
    /// </summary>
    public partial class CopySubsetToClipboardComponentControl : ApplicationComponentUserControl
    {
        private CopySubsetToClipboardComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CopySubsetToClipboardComponentControl(CopySubsetToClipboardComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

        	base.AcceptButton = _sendToClipboardButton;

			_sourceDisplaySet.DataBindings.Add("Text", _component, "SourceDisplaySetDescription", true, DataSourceUpdateMode.OnPropertyChanged);

			_radioUseInstanceNumber.DataBindings.Add("Checked", _component, "UseInstanceNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioUseInstanceNumber.DataBindings.Add("Enabled", _component, "UseInstanceNumberEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_radioUsePositionNumber.DataBindings.Add("Checked", _component, "UsePositionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioUsePositionNumber.DataBindings.Add("Enabled", _component, "UsePositionNumberEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			
			_radioCopyRange.DataBindings.Add("Checked", _component, "CopyRange", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioCopyRange.DataBindings.Add("Enabled", _component, "CopyRangeEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_radioCopyCustom.DataBindings.Add("Checked", _component, "CopyCustom", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioCopyCustom.DataBindings.Add("Enabled", _component, "CopyCustomEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_radioCopyRangeAll.DataBindings.Add("Checked", _component, "CopyRangeAll", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioCopyRangeAll.DataBindings.Add("Enabled", _component, "CopyRangeAllEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_radioCopyRangeAtInterval.DataBindings.Add("Checked", _component, "CopyRangeAtInterval", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioCopyRangeAtInterval.DataBindings.Add("Enabled", _component, "CopyRangeAtIntervalEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_copyRangeStart.DataBindings.Add("Minimum", _component, "RangeMinimum", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeStart.DataBindings.Add("Maximum", _component, "CopyRangeEnd", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeStart.DataBindings.Add("Value", _component, "CopyRangeStart", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeStart.DataBindings.Add("Enabled", _component, "CopyRangeStartEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_copyRangeEnd.DataBindings.Add("Minimum", _component, "CopyRangeStart", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeEnd.DataBindings.Add("Maximum", _component, "RangeMaximum", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeEnd.DataBindings.Add("Value", _component, "CopyRangeEnd", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeEnd.DataBindings.Add("Enabled", _component, "CopyRangeEndEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_copyRangeInterval.DataBindings.Add("Minimum", _component, "RangeMinInterval", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeInterval.DataBindings.Add("Maximum", _component, "RangeMaxInterval", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeInterval.DataBindings.Add("Value", _component, "CopyRangeInterval", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeInterval.DataBindings.Add("Enabled", _component, "CopyRangeIntervalEnabled", true, DataSourceUpdateMode.OnPropertyChanged); 

			_customRange.DataBindings.Add("Text", _component, "CustomRange", true, DataSourceUpdateMode.OnPropertyChanged);
			_customRange.DataBindings.Add("Enabled", _component, "CustomRangeEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_sendToClipboardButton.DataBindings.Add("Enabled", _component, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void OnSendToClipboard(object sender, EventArgs e)
		{
			_component.CopyToClipboard();
		}

		private void OnRangeCopyAllImagesCheckedChanged(object sender, EventArgs e)
		{
			_copyRangeInterval.Enabled = _component.CopyRangeAtInterval;
		}

		private void OnRangeCopyAtIntervalCheckedChanged(object sender, EventArgs e)
		{
			_copyRangeInterval.Enabled = _component.CopyRangeAtInterval;
		}
	}
}
