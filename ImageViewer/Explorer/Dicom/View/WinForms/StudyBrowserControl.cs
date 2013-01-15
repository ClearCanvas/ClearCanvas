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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.Collections;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	public partial class StudyBrowserControl : UserControl
	{
		private StudyBrowserComponent _studyBrowserComponent;
		private BindingSource _bindingSource;
		private bool _selectionUpdating = false;

		public StudyBrowserControl(StudyBrowserComponent component)
		{
			Platform.CheckForNullReference(component, "component");
			InitializeComponent();

			_studyBrowserComponent = component;
			_studyBrowserComponent.StudyTableChanged += OnStudyBrowserComponentOnStudyTableChanged;
			_studyBrowserComponent.SelectedStudyChanged += OnStudyBrowserComponentSelectedStudyChanged;
			_studyTableView.Table = _studyBrowserComponent.StudyTable;
			_studyTableView.ToolbarModel = _studyBrowserComponent.ToolbarModel;
			_studyTableView.MenuModel = _studyBrowserComponent.ContextMenuModel;
			_studyTableView.SelectionChanged += new EventHandler(OnStudyTableViewSelectionChanged);
			_studyTableView.ItemDoubleClicked += new EventHandler(OnStudyTableViewDoubleClick);

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _studyBrowserComponent;

			_resultsTitleBar.DataBindings.Add("Text", _studyBrowserComponent, "ResultsTitle", true, DataSourceUpdateMode.OnPropertyChanged);

			this.DataBindings.Add("Enabled", _studyBrowserComponent, "IsEnabled");
		}

		private void OnStudyBrowserComponentOnStudyTableChanged(object sender, EventArgs e)
		{
			_studyTableView.Table = _studyBrowserComponent.StudyTable;
		}

		private void OnStudyBrowserComponentSelectedStudyChanged(object sender, EventArgs e)
		{
			if (_selectionUpdating) return;
			_selectionUpdating = true;
			try
			{
				_studyTableView.Selection = new Selection(_studyBrowserComponent.SelectedStudies);
			}
			finally
			{
				_selectionUpdating = false;
			}
		}

		private void OnStudyTableViewSelectionChanged(object sender, EventArgs e)
		{
			if (_selectionUpdating) return;
			_selectionUpdating = true;
			try
			{
				//The table view remembers the selection order, with the most recent being first.
				//We actually want that same order, but in reverse.
				_studyBrowserComponent.SetSelection(ReverseSelection(_studyTableView.Selection));
			}
			finally
			{
				_selectionUpdating = false;
			}
		}

		void OnStudyTableViewDoubleClick(object sender, EventArgs e)
		{
			_studyBrowserComponent.ItemDoubleClick();
		}

		private static ISelection ReverseSelection(ISelection selection)
		{
			ArrayList list = new ArrayList();

			if (selection != null && selection.Items != null)
			{
				foreach (object o in selection.Items)
					list.Add(o);

				list.Reverse();
			}

			return new Selection(list);
		}
	}
}
