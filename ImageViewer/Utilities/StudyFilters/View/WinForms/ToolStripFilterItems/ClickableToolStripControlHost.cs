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

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems
{
	internal interface IClickableHostedControl
	{
		event EventHandler ResetDropDownFocusRequested;
		event EventHandler CloseDropDownRequested;
	}

	internal sealed class ClickableToolStripControlHost : ToolStripControlHost
	{
		public ClickableToolStripControlHost(Control hostedControl) : base(hostedControl) {}
		public ClickableToolStripControlHost(Control hostedControl, string name) : base(hostedControl, name) {}

		protected override bool DismissWhenClicked
		{
			get { return false; }
		}

		protected override void OnSubscribeControlEvents(Control control)
		{
			base.OnSubscribeControlEvents(control);
			if (control is IClickableHostedControl)
			{
				((IClickableHostedControl) control).ResetDropDownFocusRequested += Control_ResetDropDownFocusRequested;
				((IClickableHostedControl) control).CloseDropDownRequested += Control_CloseDropDownRequested;
			}
		}

		protected override void OnUnsubscribeControlEvents(Control control)
		{
			if (control is IClickableHostedControl)
			{
				((IClickableHostedControl) control).CloseDropDownRequested -= Control_CloseDropDownRequested;
				((IClickableHostedControl) control).ResetDropDownFocusRequested -= Control_ResetDropDownFocusRequested;
			}
			base.OnUnsubscribeControlEvents(control);
		}

		private void Control_ResetDropDownFocusRequested(object sender, EventArgs e)
		{
			if (base.IsOnDropDown && base.Parent != null)
				base.Parent.Focus();
		}

		private void Control_CloseDropDownRequested(object sender, EventArgs e)
		{
			if (base.IsOnDropDown && base.Parent is ToolStripDropDown)
				((ToolStripDropDown) base.Parent).Close(ToolStripDropDownCloseReason.ItemClicked);
		}
	}
}