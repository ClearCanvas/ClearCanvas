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
	/// Provides a Windows Forms user-interface for <see cref="PrintReportComponent"/>.
	/// </summary>
	public partial class PrintReportComponentControl : ApplicationComponentUserControl
	{
		private readonly PrintReportComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public PrintReportComponentControl(PrintReportComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			_practitionerLookup.LookupHandler = _component.RecipientsLookupHandler;
			_practitionerLookup.DataBindings.Add("Value", _component, "SelectedRecipient", true, DataSourceUpdateMode.OnPropertyChanged);
			_contactPoint.DataBindings.Add("DataSource", _component, "RecipientContactPointChoices", true, DataSourceUpdateMode.Never);
			_contactPoint.DataBindings.Add("Value", _component, "SelectedContactPoint", true, DataSourceUpdateMode.OnPropertyChanged);
			_contactPoint.Format += delegate(object source, ListControlConvertEventArgs e) { e.Value = _component.FormatContactPoint(e.ListItem); };
			_closeOnPrint.DataBindings.Add("Checked", _component, "CloseOnPrint", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _printButton_Click(object sender, EventArgs e)
		{
			_component.Print();
		}

		private void _closeButton_Click(object sender, EventArgs e)
		{
			_component.Close();
		}
	}
}
