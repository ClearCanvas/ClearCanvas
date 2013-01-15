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

using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Ris.Client.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="TranscriptionEditorComponent"/>.
	/// </summary>
	public partial class RichTextReportEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly IReportEditorComponent _component;
		private readonly CannedTextSupport _cannedTextSupport;

		/// <summary>
		/// Constructor.
		/// </summary>
		public RichTextReportEditorComponentControl(IReportEditorComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			_richText.DataBindings.Add("Text", _component, "EditorText", true, DataSourceUpdateMode.OnPropertyChanged);
			_cannedTextSupport = new CannedTextSupport(_richText, _component.CannedTextLookupHandler);

			Control reportPreview = (Control)_component.ReportPreviewHost.ComponentView.GuiElement;
			reportPreview.Dock = DockStyle.Fill;
			_splitContainer.Panel1.Controls.Add(reportPreview);
			UpdatePreviewVisibility();

			((INotifyPropertyChanged)_component).PropertyChanged += _component_PropertyChanged;
		}

		void _component_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "EditorText")
			{
				_richText.Text = _component.EditorText;
			}
			else if (e.PropertyName == "PreviewVisible")
			{
				UpdatePreviewVisibility();
			}
		}

		private void UpdatePreviewVisibility()
		{
			_splitContainer.Panel1Collapsed = !_component.PreviewVisible;
		}
	}
}
