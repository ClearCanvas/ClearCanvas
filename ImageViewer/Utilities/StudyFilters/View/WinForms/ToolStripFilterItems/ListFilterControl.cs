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
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.Properties;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems
{
	public partial class ListFilterControl : UserControl, IClickableHostedControl
	{
		public event EventHandler ResetDropDownFocusRequested;
		public event EventHandler CloseDropDownRequested;

		private readonly Size _defaultSize;
		private readonly ListFilterMenuAction _action;
		private bool _ignoreItemCheck = false;

		public ListFilterControl(ListFilterMenuAction action)
		{
			InitializeComponent();
			_defaultSize = base.Size;

			_action = action;

			ResetListBox();
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			Size size = base.GetPreferredSize(proposedSize);
			return new Size(Math.Max(_defaultSize.Width, size.Width), Math.Max(_defaultSize.Height, size.Height));
		}

		private void ResetListBox()
		{
			_ignoreItemCheck = true;
			try
			{
				_listBox.Items.Add(new SelectAllValues());
				foreach (object value in _action.DataSource.Values)
				{
					_listBox.Items.Add(new ValueWrapper(value), _action.DataSource.GetSelectedState(value));
				}
				_listBox.SetItemCheckState(0, GetCombinedCheckState());
			}
			finally
			{
				_ignoreItemCheck = false;
			}
		}

		private CheckState GetCombinedCheckState()
		{
			int value = 0;
			for (int n = 1; n < _listBox.Items.Count; n++)
				value |= _listBox.GetItemChecked(n) ? 2 : 1;
			if (value == 2)
				return CheckState.Checked;
			else if (value == 1)
				return CheckState.Unchecked;
			else return CheckState.Indeterminate;
		}

		private void CloseDropDown()
		{
			EventsHelper.Fire(this.CloseDropDownRequested, this, EventArgs.Empty);
		}

		private void _listBox_MouseLeave(object sender, EventArgs e)
		{
			EventsHelper.Fire(this.ResetDropDownFocusRequested, this, EventArgs.Empty);
		}

		private void _listBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (_ignoreItemCheck)
				return;

			_ignoreItemCheck = true;
			try
			{
				ValueWrapper wrapper = (ValueWrapper) _listBox.Items[e.Index];
				if (e.Index == 0)
				{
					if (e.NewValue == CheckState.Checked)
					{
						for (int n = 1; n < _listBox.Items.Count; n++)
							_listBox.SetItemChecked(n, true);
					}
					else if (e.NewValue == CheckState.Unchecked)
					{
						for (int n = 1; n < _listBox.Items.Count; n++)
							_listBox.SetItemChecked(n, false);
					}
				}
				else
				{
					bool diff = false;

					for (int n = 1; n < _listBox.Items.Count; n++)
					{
						if (n != e.Index && e.NewValue != _listBox.GetItemCheckState(n))
						{
							diff = true;
							break;
						}
					}

					if (diff)
						_listBox.SetItemCheckState(0, CheckState.Indeterminate);
					else
						_listBox.SetItemCheckState(0, e.NewValue);
				}
			}
			finally
			{
				_ignoreItemCheck = false;
			}
		}

		private void _btnOk_Click(object sender, EventArgs e)
		{
			CheckState combined = GetCombinedCheckState();
			switch (combined)
			{
				case CheckState.Checked:
					_action.DataSource.SetAllSelectedState(true);
					break;
				case CheckState.Unchecked:
					_action.DataSource.SetAllSelectedState(false);
					break;
				default:
					for (int n = 1; n < _listBox.Items.Count; n++)
						_action.DataSource.SetSelectedState(((ValueWrapper) _listBox.Items[n]).Value, _listBox.GetItemChecked(n));
					break;
			}
			CloseDropDown();
		}

		private void _btnCancel_Click(object sender, EventArgs e)
		{
			CloseDropDown();
			ResetListBox();
		}

		private class ValueWrapper
		{
			public readonly object Value;

			public ValueWrapper(object value)
			{
				this.Value = value;
			}

			public override string ToString()
			{
				if (this.Value == null)
				{
					return Resources.LabelNullValue;
				}
				return this.Value.ToString();
			}
		}

		private sealed class SelectAllValues : ValueWrapper
		{
			public SelectAllValues() : base(null) {}

			public override string ToString()
			{
				return Resources.LabelSelectAll;
			}
		}
	}
}