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
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ProgressBarStyle=System.Windows.Forms.ProgressBarStyle;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="SearchPanelComponent"/>
	/// </summary>
	public partial class SearchPanelComponentControl : ApplicationComponentUserControl
	{
		private readonly SearchPanelComponent _component;
		private Control _lastActiveControl;

		/// <summary>
		/// Constructor
		/// </summary>
		public SearchPanelComponentControl(SearchPanelComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;

			_titleBar.DataBindings.Add("Text", component, "Title", true, DataSourceUpdateMode.OnPropertyChanged);

			_modalityPicker.SetAvailableModalities(_component.AvailableSearchModalities);
			_modalityPicker.DataBindings.Add("Enabled", component, "IsSearchEnabled");
			_modalityPicker.DataBindings.Add("CheckedModalities", component, "SearchModalities", true, DataSourceUpdateMode.OnPropertyChanged);

			_patientsName.DataBindings.Add("Value", component, "PatientsName", true, DataSourceUpdateMode.OnPropertyChanged);
			_patientsName.DataBindings.Add("Enabled", component, "IsSearchEnabled");

			_accessionNumber.DataBindings.Add("Value", component, "AccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_accessionNumber.DataBindings.Add("Enabled", component, "IsSearchEnabled");

			_patientID.DataBindings.Add("Value", component, "PatientID", true, DataSourceUpdateMode.OnPropertyChanged);
			_patientID.DataBindings.Add("Enabled", component, "IsSearchEnabled");

			_studyDescription.DataBindings.Add("Value", component, "StudyDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDescription.DataBindings.Add("Enabled", component, "IsSearchEnabled");

			_studyDateFrom.DataBindings.Add("Maximum", component, "MaximumStudyDateFrom", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDateTo.DataBindings.Add("Maximum", component, "MaximumStudyDateTo", true, DataSourceUpdateMode.OnPropertyChanged);

			_studyDateFrom.DataBindings.Add("Value", component, "StudyDateFrom", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDateTo.DataBindings.Add("Value", component, "StudyDateTo", true, DataSourceUpdateMode.OnPropertyChanged);

			_studyDateFrom.DataBindings.Add("Enabled", component, "IsSearchEnabled");
			_studyDateTo.DataBindings.Add("Enabled", component, "IsSearchEnabled");

			_referringPhysiciansName.DataBindings.Add("Value", component, "ReferringPhysiciansName", true, DataSourceUpdateMode.OnPropertyChanged);
			_referringPhysiciansName.DataBindings.Add("Enabled", component, "IsSearchEnabled");

			_searchTodayButton.DataBindings.Add("Enabled", component, "IsSearchEnabled");
			_searchLastWeekButton.DataBindings.Add("Enabled", component, "IsSearchEnabled");
			_clearButton.DataBindings.Add("Enabled", component, "IsSearchEnabled");

			_component.PropertyChanged += OnComponentPropertyChanged;

			UpdateState();
			UpdateIcons();
		}

		protected override void OnCurrentUIThemeChanged()
		{
			base.OnCurrentUIThemeChanged();

			UpdateIcons();
		}

		private void OnComponentPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsSearchEnabled")
			{
				UpdateState();
			}
		}

		private void UpdateState()
		{
			_searchButton.Text = _component.IsSearchEnabled ? SR.ButtonSearch : SR.ButtonCancelSearch;
			_progressBar.Style = _component.IsSearchEnabled ? ProgressBarStyle.Blocks : ProgressBarStyle.Marquee;
			_progressBar.Visible = _searchingLabel.Visible = !_component.IsSearchEnabled;

			if (_component.IsSearchEnabled && _lastActiveControl != null)
			{
				SynchronizationContext.Current.Post(s => RefocusControl((Control) s), _lastActiveControl);
				_lastActiveControl = null;
			}
		}

		private void UpdateIcons()
		{
			var resourceResolver = new ApplicationThemeResourceResolver(GetType(), false);
			_searchButton.Image = resourceResolver.OpenImage(@"Icons.Search.png");
			_searchTodayButton.Image = resourceResolver.OpenImage(@"Icons.Today.png");
			_searchLastWeekButton.Image = resourceResolver.OpenImage(@"Icons.Last7Days.png");
			_clearButton.Image = resourceResolver.OpenImage(@"Icons.Clear.png");
		}

		private void RefocusControl(Control control)
		{
			if (control is TextField)
			{
				var textField = (TextField) control;
				var lastSelectionStart = textField.SelectionStart;
				var lastSelectionLength = textField.SelectionLength;

				ActiveControl = control;
				SelectNextControl(control, true, true, true, true);
				control.Focus();

				textField.SelectionLength = 0;
				textField.SelectionStart = lastSelectionStart;
				textField.SelectionLength = lastSelectionLength;
			}
			else
			{
				ActiveControl = control;
				SelectNextControl(control, true, true, true, true);
				control.Focus();
			}
		}

		private void OnSearchButtonClicked(object sender, EventArgs e)
		{
			if (_component.IsSearchEnabled)
			{
				if (!(_lastActiveControl is Button)) _lastActiveControl = ActiveControl;
				_component.Search();
			}
			else
				_component.CancelSearch();
		}

		private void OnSearchLastWeekButtonClick(object sender, EventArgs e)
		{
			_component.SearchLastWeek();
		}

		private void OnSearchTodayButtonClicked(object sender, EventArgs e)
		{
			_component.SearchToday();
		}

		private void OnClearButonClicked(object sender, EventArgs e)
		{
			_component.Clear();
		}
	}
}