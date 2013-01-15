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
using ClearCanvas.Ris.Client.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ProtocolReasonComponent"/>
	/// </summary>
	public partial class ReasonSelectionComponentControl : ApplicationComponentUserControl
	{
		private ReasonSelectionComponentBase _component;
		private readonly CannedTextSupport _cannedTextSupport;

		/// <summary>
		/// Constructor
		/// </summary>
		public ReasonSelectionComponentControl(ReasonSelectionComponentBase component)
			: base(component)
		{
			InitializeComponent();

			_component = component;

			_reason.DataSource = _component.ReasonChoices;
			_reason.DataBindings.Add("Value", _component, "SelectedReasonChoice", true, DataSourceUpdateMode.OnPropertyChanged);

			_otherReason.DataBindings.Add("Value", _component, "OtherReason", true, DataSourceUpdateMode.OnPropertyChanged);
			_cannedTextSupport = new CannedTextSupport(_otherReason, _component.CannedTextLookupHandler);

			_btnOK.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _btnOK_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _btnCancel_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}
