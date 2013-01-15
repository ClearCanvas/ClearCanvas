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
	/// Provides a Windows Forms user-interface for <see cref="WorkQueueAdminComponent"/>.
	/// </summary>
	public partial class WorkQueueAdminComponentControl : ApplicationComponentUserControl
	{
		private readonly WorkQueueAdminComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public WorkQueueAdminComponentControl(WorkQueueAdminComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;

			_queue.Table = _component.Queue;
			_queue.MenuModel = _component.MenuModel;
			_queue.ToolbarModel = _component.ToolbarModel;

			var preview = (Control)_component.PreviewComponentHost.ComponentView.GuiElement;
			preview.Dock = DockStyle.Fill;
			_previewPanel.Controls.Add(preview);

			_statusDroplist.NullItem = _component.NullFilterItem;
			_statusDroplist.DataBindings.Add("Items", _component, "StatusChoices", true, DataSourceUpdateMode.Never);
			_statusDroplist.DataBindings.Add("CheckedItems", _component, "SelectedStatuses", true, DataSourceUpdateMode.OnPropertyChanged);

			_messageTypeDroplist.NullItem = _component.NullFilterItem;
			_messageTypeDroplist.DataBindings.Add("Items", _component, "TypeChoices", true, DataSourceUpdateMode.Never);
			_messageTypeDroplist.DataBindings.Add("CheckedItems", _component, "SelectedTypes", true, DataSourceUpdateMode.OnPropertyChanged);

			var scheduledOptionBinding = new Binding("Checked", _component, "SelectedTimeFilterOption", true, DataSourceUpdateMode.OnPropertyChanged);
			var processedOptionBinding = new Binding("Checked", _component, "SelectedTimeFilterOption", true, DataSourceUpdateMode.OnPropertyChanged);
			_scheduledOption.DataBindings.Add(scheduledOptionBinding);
			_processedOption.DataBindings.Add(processedOptionBinding);
			scheduledOptionBinding.Parse += OnRadioBindingParse;
			processedOptionBinding.Parse += OnRadioBindingParse;

			scheduledOptionBinding.Format += delegate(object sender, ConvertEventArgs e)
			{
				if (e.DesiredType != typeof(bool))
					return;

				e.Value = ((WorkQueueAdminComponent.TimeFilterOptions)e.Value) == WorkQueueAdminComponent.TimeFilterOptions.Scheduled;
			};

			processedOptionBinding.Format += delegate(object sender, ConvertEventArgs e)
			{
				if (e.DesiredType != typeof(bool))
					return;

				e.Value = ((WorkQueueAdminComponent.TimeFilterOptions)e.Value) == WorkQueueAdminComponent.TimeFilterOptions.Processed;
			};

			_startTime.DataBindings.Add("Value", _component, "StartTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_endTime.DataBindings.Add("Value", _component, "EndTime", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _queue_SelectionChanged(object sender, EventArgs e)
		{
			_component.SetSelectedItem(_queue.Selection);
		}

		private void _showAll_Click(object sender, EventArgs e)
		{
			using (new CursorManager(this, Cursors.WaitCursor))
			{
				_component.ClearFilter();
			}
		}

		private void _searchButton_Click(object sender, EventArgs e)
		{
			using (new CursorManager(this, Cursors.WaitCursor))
			{
				_component.ApplyFilter();
			}
		}

		void OnRadioBindingParse(object sender, ConvertEventArgs e)
		{
			if (e.DesiredType != typeof(WorkQueueAdminComponent.TimeFilterOptions))
				return;

			if (_scheduledOption.Checked)
				e.Value = WorkQueueAdminComponent.TimeFilterOptions.Scheduled;
			else
				e.Value = WorkQueueAdminComponent.TimeFilterOptions.Processed;
		}
	}
}
