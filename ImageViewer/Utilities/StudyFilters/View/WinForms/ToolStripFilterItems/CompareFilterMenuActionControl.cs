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
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.Properties;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems
{
	public partial class CompareFilterMenuActionControl : UserControl
	{
		private readonly Size _defaultSize;
		private readonly CompareFilterMenuAction _action;

		public CompareFilterMenuActionControl(CompareFilterMenuAction action)
		{
			InitializeComponent();
			_defaultSize = base.Size;

			_action = action;

			_txtValue.Text = _action.Value;

			UpdateMode();
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			Size size = base.GetPreferredSize(proposedSize);
			return new Size(Math.Max(_defaultSize.Width, size.Width), Math.Max(_defaultSize.Height, size.Height));
		}

		private void UpdateMode()
		{
			// tooltips are buggy since they stay on screen after the mouse moves off the control (provider cannot detect when to clear it)
			switch (_action.CurrentMode)
			{
				case CompareFilterMode.LessThan:
					_modeToggle.Text = Resources.LabelLessThanOperator;
					//_tooltipProvider.SetToolTip(_modeToggle, Resources.TooltipLessThanOperator);
					break;
				case CompareFilterMode.LessThenOrEquals:
					_modeToggle.Text = Resources.LabelLessThanOrEqualsOperator;
					//_tooltipProvider.SetToolTip(_modeToggle, Resources.TooltipLessThanOrEqualsOperator);
					break;
				case CompareFilterMode.GreaterThan:
					_modeToggle.Text = Resources.LabelGreaterThanOperator;
					//_tooltipProvider.SetToolTip(_modeToggle, Resources.TooltipGreaterThanOperator);
					break;
				case CompareFilterMode.GreaterThanOrEquals:
					_modeToggle.Text = Resources.LabelGreaterThanOrEqualsOperator;
					//_tooltipProvider.SetToolTip(_modeToggle, Resources.TooltipGreaterThanOrEqualsOperator);
					break;
				case CompareFilterMode.NotEquals:
					_modeToggle.Text = Resources.LabelNotEqualsOperator;
					//_tooltipProvider.SetToolTip(_modeToggle, Resources.TooltipNotEqualsOperator);
					break;
				case CompareFilterMode.Equals:
				default:
					_modeToggle.Text = Resources.LabelEqualsOperator;
					//_tooltipProvider.SetToolTip(_modeToggle, Resources.TooltipEqualsOperator);
					break;
			}
		}

		private void modeToggle_Click(object sender, EventArgs e)
		{
			IList<CompareFilterMode> list = _action.AllowedModes;
			int index = (Math.Max(-1, list.IndexOf(_action.CurrentMode)) + 1)%list.Count;
			_action.CurrentMode = list[index];
			UpdateMode();
		}

		private void _txtValue_TextChanged(object sender, EventArgs e)
		{
			try
			{
				_action.Value = _txtValue.Text;
				_errorProvider.Clear();
			}
			catch (Exception)
			{
				_errorProvider.SetError(this, Resources.MessageParsingFailed);
			}
		}
	}
}