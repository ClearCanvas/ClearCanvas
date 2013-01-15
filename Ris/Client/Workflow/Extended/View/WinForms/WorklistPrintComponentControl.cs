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
using ClearCanvas.Desktop.View.WinForms;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="WorklistPrintComponent"/>.
	/// </summary>
	public partial class WorklistPrintComponentControl : ApplicationComponentUserControl
	{
		private readonly WorklistPrintComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public WorklistPrintComponentControl(WorklistPrintComponent component)
			:base(component)
		{
			_component = component;
			InitializeComponent();

			var browser = (Control)_component.WorklistPrintPreviewComponentHost.ComponentView.GuiElement;
			_previewPanel.Controls.Add(browser);
			browser.Dock = DockStyle.Fill;
		}

		private void _printButton_Click(object sender, EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.Print();
			}
		}

		private void _closeButton_Click(object sender, EventArgs e)
		{
			_component.Close();
		}
	}
}
