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
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Ris.Client.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="OrderNoteConversationComponent"/>.
	/// </summary>
	public partial class OrderNoteConversationComponentControl : ApplicationComponentUserControl
	{
		private readonly OrderNoteConversationComponent _component;
		private readonly CannedTextSupport _cannedTextSupport;

		/// <summary>
		/// Constructor.
		/// </summary>
		public OrderNoteConversationComponentControl(OrderNoteConversationComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;

			var orderNotes = (Control)_component.OrderNotesHost.ComponentView.GuiElement;
			orderNotes.Dock = DockStyle.Fill;
			_orderNotesPanel.Controls.Add(orderNotes);

			_templateSelectionPanel.Visible = _component.TemplateChoicesVisible;
			_template.DataSource = _component.TemplateChoices;
			_template.DataBindings.Add("Value", _component, "SelectedTemplate", true, DataSourceUpdateMode.OnPropertyChanged);
			_template.Format += (source, e) => e.Value = _component.FormatTemplate(e.ListItem);

			_replyBody.DataBindings.Add("Text", _component, "Body", true, DataSourceUpdateMode.OnPropertyChanged);
			_cannedTextSupport = new CannedTextSupport(_replyBody, _component.CannedTextLookupHandler);

			_urgent.DataBindings.Add("Checked", _component, "Urgent", true, DataSourceUpdateMode.OnPropertyChanged);
			_urgent.DataBindings.Add("Enabled", _component, "IsPosting", true, DataSourceUpdateMode.OnPropertyChanged);

			_onBehalf.DataSource = _component.OnBehalfOfGroupChoices;
			_onBehalf.DataBindings.Add("Value", _component, "OnBehalfOf", true, DataSourceUpdateMode.OnPropertyChanged);
			_onBehalf.DataBindings.Add("Enabled", _component, "IsOnBehalfOfEditable", true, DataSourceUpdateMode.OnPropertyChanged);
			_onBehalf.Format += ((source, e) => e.Value = _component.FormatOnBehalfOf(e.ListItem));

			_recipients.Table = _component.Recipients;
			_recipients.MenuModel = _component.RecipientsActionModel;
			_recipients.ToolbarModel = _component.RecipientsActionModel;
			_recipients.DataBindings.Add("Selection", _component, "SelectedRecipient", true, DataSourceUpdateMode.OnPropertyChanged);
			_recipients.DataBindings.Add("Enabled", _component, "IsPosting", true, DataSourceUpdateMode.OnPropertyChanged);

			_completeButton.DataBindings.Add("Text", _component, "CompleteButtonLabel", true, DataSourceUpdateMode.OnPropertyChanged);

			_notesGroupBox.Text = _component.OrderNotesLabel;

			_component.PropertyChanged += _component_propertyChanged;

			_component.NewRecipientAdded += _component_NewRecipientAdded;

			CreateSoftKeys();
		}

		private void CreateSoftKeys()
		{
			ClearSoftKeys();

			_softKeyFlowPanel.Visible = _component.SoftKeysVisible;

			foreach (var name in _component.SoftKeyNames)
			{
				var softKeyButton = new Button { Text = name, AutoEllipsis = true };
				softKeyButton.Click += softKeyButton_Click;
				_softKeyFlowPanel.Controls.Add(softKeyButton);
			}
		}

		private void ClearSoftKeys()
		{
			foreach (Button button in _softKeyFlowPanel.Controls)
			{
				button.Click -= softKeyButton_Click;
				button.Dispose();
			}

			_softKeyFlowPanel.Controls.Clear();
		}

		private void softKeyButton_Click(object sender, EventArgs e)
		{
			var softKeyButton = (Button)sender;
			_component.ApplySoftKey(softKeyButton.Text);
		}

		private void _component_NewRecipientAdded(object sender, EventArgs e)
		{
			// TODO: shouldn't hardcode the column index here
			_recipients.BeginEdit(1, false);
		}

		private void _component_propertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CompleteButtonLabel")
			{
				_completeButton.Text = _component.CompleteButtonLabel;
			}
			if(e.PropertyName == "SoftKeyNames")
			{
				CreateSoftKeys();
			}
		}

		private void _completeButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.AcknowledgeAndPost();
			}
		}

		private void _cancelButton_Click(object sender, System.EventArgs e)
		{
			_component.Cancel();
		}
	}
}
