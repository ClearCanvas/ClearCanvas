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

using System.Linq;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="DicomSendConfigurationComponent"/>
	/// </summary>
	public partial class DicomSendConfigurationComponentControl : ApplicationComponentUserControl
	{
		private readonly DicomSendConfigurationComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public DicomSendConfigurationComponentControl(DicomSendConfigurationComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_maxNumberOfRetries.DataBindings.Add("Value", _component, "MaxNumberOfRetries", true, DataSourceUpdateMode.OnPropertyChanged);
			_retryDelayValue.DataBindings.Add("Value", _component, "RetryDelayValue", true, DataSourceUpdateMode.OnPropertyChanged);
			_retryDelayValue.DataBindings.Add("Maximum", _component, "MaxRetryDelayValue");

			_retryDelayUnits.Items.AddRange(_component.RetryDelayUnits.Cast<object>().ToArray());
			_retryDelayUnits.DataBindings.Add("SelectedItem", _component, "RetryDelayUnit", true, DataSourceUpdateMode.OnPropertyChanged);
			_retryDelayUnits.Format += (sender, e) => { e.Value = _component.FormatRetryDelayUnit(e.ListItem); };
			
			// bug #10076: combobox databinding doesn't apply change until it loses focus, so we do it manually
			_retryDelayUnits.SelectedIndexChanged += (sender, args) =>
			{
				_component.RetryDelayUnit = (RetryDelayTimeUnit)_retryDelayUnits.SelectedItem;
			};
		}
	}
}
