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

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="CancelOrderComponent"/>
	/// </summary>
	public partial class CancelOrderComponentControl : ApplicationComponentUserControl
	{
		private readonly CancelOrderComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public CancelOrderComponentControl(CancelOrderComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_proceduresTableView.Table = _component.ProceduresTable;
			_cancelReason.DataSource = _component.CancelReasonChoices;
			_cancelReason.DataBindings.Add("Value", _component, "SelectedCancelReason", true, DataSourceUpdateMode.OnPropertyChanged);
			_okButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}
