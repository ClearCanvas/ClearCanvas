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
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	public partial class StackedComponentContainerControl : UserControl
	{
		readonly StackedComponentContainer _component;

		public StackedComponentContainerControl(StackedComponentContainer component)
		{
			Platform.CheckForNullReference(component, "component");

			InitializeComponent();
			_component = component;
			_component.CurrentPageChanged += OnComponentCurrentPageChanged;

			ShowPage(_component.CurrentPage);
		}

		private void OnComponentCurrentPageChanged(object sender, EventArgs e)
		{
			ShowPage(_component.CurrentPage);
		}

		private void ShowPage(ContainerPage page)
		{
			// get the control to show
			var toShow = (Control)_component.GetPageView(page).GuiElement;

			// hide all others
			foreach (var c in this.Controls.Cast<Control>().Where(c => c != toShow))
			{
				c.Visible = false;
			}

			// if the control has not been added to the content panel, add it now
			if (!this.Controls.Contains(toShow))
			{
				toShow.Dock = DockStyle.Fill;
				this.Controls.Add(toShow);
			}

			toShow.Visible = true;

			// HACK: for some reason the error provider symbols don't show up the first time the control is shown
			// therefore we need to force it
			if (toShow is ApplicationComponentUserControl)
			{
				(toShow as ApplicationComponentUserControl).ErrorProvider.UpdateBinding();
			}
		}

	}
}
