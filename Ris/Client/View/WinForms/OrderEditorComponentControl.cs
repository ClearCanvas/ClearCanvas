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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using TabPage = System.Windows.Forms.TabPage;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="OrderEditorComponent"/>
	/// </summary>
	public partial class OrderEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly OrderEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public OrderEditorComponentControl(OrderEditorComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			InitTabPage(_component.OrderNoteSummaryHost, _notesPage);
			InitTabPage(_component.AttachmentsComponentHost, _attachmentsPage);

			foreach (var extensionPage in _component.ExtensionPages)
			{
				AddExtensionPage(extensionPage);
			}

			// force toolbars to be displayed (VS designer seems to have a bug with this)
			_proceduresTableView.ShowToolbar = true;
			_recipientsTableView.ShowToolbar = true;

			_patient.LookupHandler = _component.PatientProfileLookupHandler;
			_patient.DataBindings.Add("Value", _component, "SelectedPatientProfile", true, DataSourceUpdateMode.OnPropertyChanged);
			_patient.DataBindings.Add("Enabled", _component, "IsPatientProfileEditable");

			_diagnosticService.LookupHandler = _component.DiagnosticServiceLookupHandler;
			_diagnosticService.DataBindings.Add("Value", _component, "SelectedDiagnosticService", true, DataSourceUpdateMode.OnPropertyChanged);
			_diagnosticService.DataBindings.Add("Enabled", _component, "IsDiagnosticServiceEditable");

			_indication.DataBindings.Add("Value", _component, "Indication", true, DataSourceUpdateMode.OnPropertyChanged);
			_indication.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");

			_proceduresTableView.Table = _component.Procedures;
			_proceduresTableView.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
			_proceduresTableView.MenuModel = _component.ProceduresActionModel;
			_proceduresTableView.ToolbarModel = _component.ProceduresActionModel;
			_proceduresTableView.DataBindings.Add("Selection", _component, "SelectedProcedures", true, DataSourceUpdateMode.OnPropertyChanged);

			if(_component.IsCopiesToRecipientsPageVisible)
			{
				_recipientsTableView.Table = _component.Recipients;
				_recipientsTableView.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
				_recipientsTableView.MenuModel = _component.RecipientsActionModel;
				_recipientsTableView.ToolbarModel = _component.RecipientsActionModel;
				_recipientsTableView.DataBindings.Add("Selection", _component, "SelectedRecipient", true, DataSourceUpdateMode.OnPropertyChanged);

				_addConsultantButton.DataBindings.Add("Enabled", _component.RecipientsActionModel.Add, "Enabled");

				_recipientLookup.LookupHandler = _component.RecipientsLookupHandler;
				_recipientLookup.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
				_recipientLookup.DataBindings.Add("Value", _component, "RecipientToAdd", true, DataSourceUpdateMode.OnPropertyChanged);

				_recipientContactPoint.DataBindings.Add("DataSource", _component, "RecipientContactPointChoices", true, DataSourceUpdateMode.Never);
				_recipientContactPoint.DataBindings.Add("Value", _component, "RecipientContactPointToAdd", true, DataSourceUpdateMode.OnPropertyChanged);
				_recipientContactPoint.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
				_recipientContactPoint.Format += delegate(object source, ListControlConvertEventArgs e) { e.Value = _component.FormatContactPoint(e.ListItem); };
			}
			else
			{
				_mainTab.TabPages.Remove(_copiesToRecipients);
			}


			_visit.DataSource = _component.ActiveVisits;
			_visit.DataBindings.Add("Value", _component, "SelectedVisit", true, DataSourceUpdateMode.OnPropertyChanged);
			_visit.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
			_visit.DataBindings.Add("Visible", _component, "VisitVisible");
			_visit.Format += delegate(object source, ListControlConvertEventArgs e) { e.Value = _component.FormatVisit(e.ListItem); };
			_visitSummaryButton.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
			_visitSummaryButton.DataBindings.Add("Visible", _component, "VisitVisible");

			_priority.DataSource = _component.PriorityChoices;
			_priority.DataBindings.Add("Value", _component, "SelectedPriority", true, DataSourceUpdateMode.OnPropertyChanged);
			_priority.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");

			_orderingFacility.DataBindings.Add("Value", _component, "OrderingFacility", true, DataSourceUpdateMode.OnPropertyChanged);
			// Ordering Facility's Enabled is not bound since it is always readonly (via designer)

			_orderingPractitioner.LookupHandler = _component.OrderingPractitionerLookupHandler;
			_orderingPractitioner.DataBindings.Add("Value", _component, "SelectedOrderingPractitioner", true, DataSourceUpdateMode.OnPropertyChanged);
			_orderingPractitioner.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");

			_orderingPractitionerContactPoint.DataBindings.Add("DataSource", _component, "OrderingPractitionerContactPointChoices", true, DataSourceUpdateMode.Never);
			_orderingPractitionerContactPoint.DataBindings.Add("Value", _component, "SelectedOrderingPractitionerContactPoint", true, DataSourceUpdateMode.OnPropertyChanged);
			_orderingPractitionerContactPoint.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
			_orderingPractitionerContactPoint.Format += delegate(object source, ListControlConvertEventArgs e) { e.Value = _component.FormatContactPoint(e.ListItem); };

			// bind date and time to same property
			_schedulingRequestDate.DataBindings.Add("Value", _component, "SchedulingRequestTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_schedulingRequestDate.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
			_schedulingRequestDate.DataBindings.Add("Visible", _component, "SchedulingRequestTimeVisible");
			_schedulingRequestTime.DataBindings.Add("Value", _component, "SchedulingRequestTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_schedulingRequestTime.DataBindings.Add("Enabled", _component, "OrderIsNotCompleted");
			_schedulingRequestTime.DataBindings.Add("Visible", _component, "SchedulingRequestTimeVisible");

			_reorderReason.DataSource = _component.CancelReasonChoices;
			_reorderReason.DataBindings.Add("Value", _component, "SelectedCancelReason", true, DataSourceUpdateMode.OnPropertyChanged);
			_reorderReason.DataBindings.Add("Visible", _component, "IsCancelReasonVisible");

			_downtimeAccession.DataBindings.Add("Visible", _component, "IsDowntimeAccessionNumberVisible");
			_downtimeAccession.DataBindings.Add("Value", _component, "DowntimeAccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);

			_component.PropertyChanged += _component_PropertyChanged;
		}

		private void _component_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ActiveVisits")
			{
				_visit.DataSource = _component.ActiveVisits;
			}
			else if (e.PropertyName == "PriorityChoices")
			{
				_priority.DataSource = _component.PriorityChoices;
			}
			else if (e.PropertyName == "CancelReasonChoices")
			{
				_reorderReason.DataSource = _component.CancelReasonChoices;
			}
			else if (e.PropertyName == "OrderingPractitionerContactPointChoices")
			{
				_orderingPractitionerContactPoint.DataSource = _component.OrderingPractitionerContactPointChoices;
			}
			else if (e.PropertyName == "RecipientContactPointChoices")
			{
				_recipientContactPoint.DataSource = _component.RecipientContactPointChoices;
			}
		}

		private void _placeOrderButton_Click(object sender, EventArgs e)
		{
			// bug #7781: switch back to this tab prior to validation
			_mainTab.SelectedTab = _generalPage;

			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.Accept();
			}
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void _addConsultantButton_Click(object sender, EventArgs e)
		{
			_component.AddRecipient();
		}

		private void _proceduresTableView_ItemDoubleClicked(object sender, EventArgs e)
		{
			_component.EditSelectedProcedures();
		}

		private void _visitSummaryButton_Click(object sender, EventArgs e)
		{
			_component.ShowVisitSummary();
		}

		private void OrderEditorComponentControl_Load(object sender, EventArgs e)
		{
			_downtimeAccession.Mask = _component.AccessionNumberMask;
		}

		private void AddExtensionPage(IOrderEditorPage page)
		{
			var tabPage = new TabPage(page.Path.LocalizedPath);
			_mainTab.TabPages.Add(tabPage);

			var componentHost = _component.GetExtensionPageHost(page);
			InitTabPage(componentHost, tabPage);
		}

		private static void InitTabPage(ApplicationComponentHost componentHost, TabPage tabPage)
		{
			var control = (Control)componentHost.ComponentView.GuiElement;
			control.Dock = DockStyle.Fill;
			tabPage.Controls.Add(control);
		}

	}
}
