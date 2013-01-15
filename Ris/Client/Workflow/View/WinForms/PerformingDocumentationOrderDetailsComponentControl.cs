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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="PerformingDocumentationOrderDetailsComponent"/>
	/// </summary>
	public partial class PerformingDocumentationOrderDetailsComponentControl : ApplicationComponentUserControl
	{
		private readonly PerformingDocumentationOrderDetailsComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public PerformingDocumentationOrderDetailsComponentControl(PerformingDocumentationOrderDetailsComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;

			var protocols = (Control)_component.ProtocolHost.ComponentView.GuiElement;
			protocols.Dock = DockStyle.Fill;
			_protocolsPanel.Controls.Add(protocols);

			var notes = (Control)_component.NotesHost.ComponentView.GuiElement;
			notes.Dock = DockStyle.Fill;
			_orderNotesGroupBox.Controls.Add(notes);

			var rightHandContent = (Control)_component.RightHandComponentContainerHost.ComponentView.GuiElement;
			rightHandContent.Dock = DockStyle.Fill;
			_rightHandPanel.Controls.Add(rightHandContent);
		}
	}
}
