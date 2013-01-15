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

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="WorklistTimeWindowEditorComponent"/>
	/// </summary>
	public partial class WorklistTimeWindowEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly WorklistTimeWindowEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public WorklistTimeWindowEditorComponentControl(WorklistTimeWindowEditorComponent component)
			:base(component)
		{
			InitializeComponent();
			_component = component;

			_fixedWindowRadioButton.DataBindings.Add("Checked", _component, "IsFixedTimeWindow", true, DataSourceUpdateMode.OnPropertyChanged);
			_fixedWindowRadioButton.DataBindings.Add("Enabled", _component, "FixedSlidingChoiceEnabled", true, DataSourceUpdateMode.Never);
			_slidingWindowRadioButton.DataBindings.Add("Checked", _component, "IsSlidingTimeWindow", true, DataSourceUpdateMode.Never);
			_slidingWindowRadioButton.DataBindings.Add("Enabled", _component, "FixedSlidingChoiceEnabled", true, DataSourceUpdateMode.Never);

			_slidingScale.DataSource = _component.SlidingScaleChoices;
			_slidingScale.DataBindings.Add("Value", _component, "SlidingScale", true, DataSourceUpdateMode.OnPropertyChanged);
			_slidingScale.DataBindings.Add("Enabled", _component, "SlidingScaleEnabled", true, DataSourceUpdateMode.Never);


			_fromCheckBox.DataBindings.Add("Checked", _component, "StartTimeChecked", true, DataSourceUpdateMode.OnPropertyChanged);
			_fromFixed.DataBindings.Add("Enabled", _component, "FixedStartTimeEnabled", true, DataSourceUpdateMode.Never);
			_fromFixed.DataBindings.Add("Value", _component, "FixedStartTime", true, DataSourceUpdateMode.OnPropertyChanged);

			_fromSliding.DataBindings.Add("Enabled", _component, "SlidingStartTimeEnabled", true, DataSourceUpdateMode.Never);
			_fromSliding.DataBindings.Add("Value", _component, "SlidingStartTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_fromSliding.Format = _component.FormatSlidingTime;

			_toCheckBox.DataBindings.Add("Checked", _component, "EndTimeChecked", true, DataSourceUpdateMode.OnPropertyChanged);
			_toFixed.DataBindings.Add("Enabled", _component, "FixedEndTimeEnabled", true, DataSourceUpdateMode.Never);
			_toFixed.DataBindings.Add("Value", _component, "FixedEndTime", true, DataSourceUpdateMode.OnPropertyChanged);

			_toSliding.DataBindings.Add("Enabled", _component, "SlidingEndTimeEnabled", true, DataSourceUpdateMode.Never);
			_toSliding.DataBindings.Add("Value", _component, "SlidingEndTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_toSliding.Format = _component.FormatSlidingTime;
		}
	}
}
