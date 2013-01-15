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
using System.Linq;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="TestComponent"/>.
    /// </summary>
    public partial class AlertViewerComponentControl : ApplicationComponentUserControl
    {
		class FilterItem
		{
			private readonly Func<object, string> _formatter;
			public FilterItem(object item, Func<object, string> formatter)
			{
				Item = item;
				_formatter = formatter;
			}

			public object Item { get; private set; }

			public override string ToString()
			{
				return _formatter(this.Item);
			}
		}

		private readonly AlertViewerComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
		public AlertViewerComponentControl(AlertViewerComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

			ToolStripBuilder.BuildToolStrip(ToolStripBuilder.ToolStripKind.Toolbar, _toolbar.Items, _component.AlertActions.ChildNodes);
			
			_alertTableView.Table = _component.Alerts;
        	_alertTableView.MenuModel = _component.AlertActions;

			// need to work with these manually, because data-binding doesn't work well with toolstrip comboboxes
			_filter.Items.AddRange(_component.FilterChoices.Cast<object>().Select(i => new FilterItem(i, _component.FormatFilter)).ToArray());
			_filter.SelectedIndex = 0;
			_filter.SelectedIndexChanged += _activityFilter_SelectedIndexChanged;
		}

		private void _activityFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			_component.Filter = ((FilterItem)_filter.SelectedItem).Item;
		}

	}
}
