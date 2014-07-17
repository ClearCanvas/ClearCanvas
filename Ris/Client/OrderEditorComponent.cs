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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an interface for providing custom editing pages to be displayed in the order editor.
	/// </summary>
	public interface IOrderEditorPageProvider : IExtensionPageProvider<IOrderEditorPage, IOrderEditorContext>
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom editor page with access to the editor
	/// context.
	/// </summary>
	public interface IOrderEditorContext
	{
		event EventHandler PatientLoaded;

		event EventHandler OrderLoaded;

		/// <summary>
		/// Patient ref.
		/// </summary>
		EntityRef PatientRef { get; }

		/// <summary>
		/// Patient Profile ref.
		/// </summary>
		EntityRef PatientProfileRef { get; }


		/// <summary>
		/// Order ref.
		/// </summary>
		EntityRef OrderRef { get; }

		/// <summary>
		/// Exposes the extended properties associated with the Order.  Modifications made to these
		/// properties by the editor page will be persisted whenever the order editor is saved.
		/// </summary>
		IDictionary<string, string> OrderExtendedProperties { get; }

		/// <summary>
		/// Allows the extension page to indicate that data has changed.
		/// </summary>
		void SetModified();
	}

	/// <summary>
	/// Defines an interface to a custom order editor page.
	/// </summary>
	public interface IOrderEditorPage : IExtensionPage
	{
		void Save();
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the order editor.
	/// </summary>
	[ExtensionPoint]
	public class OrderEditorPageProviderExtensionPoint : ExtensionPoint<IOrderEditorPageProvider>
	{
	}

	/// <summary>
	/// Extension point for views onto <see cref="OrderEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class OrderEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// OrderEditorComponent class
	/// </summary>
	[AssociateView(typeof(OrderEditorComponentViewExtensionPoint))]
	public partial class OrderEditorComponent : ApplicationComponent
	{
		#region OrderEditorContext

		class OrderEditorContext : IOrderEditorContext
		{
			private readonly OrderEditorComponent _owner;

			public OrderEditorContext(OrderEditorComponent owner)
			{
				_owner = owner;
			}

			public event EventHandler PatientLoaded;
			public event EventHandler OrderLoaded;

			public EntityRef PatientRef
			{
				get { return _owner._patientProfile.PatientRef; }
			}

			public EntityRef PatientProfileRef
			{
				get { return _owner._patientProfile.PatientProfileRef; }
			}

			public EntityRef OrderRef
			{
				get { return _owner._orderRef; }
			}

			public IDictionary<string, string> OrderExtendedProperties
			{
				get { return _owner._extendedProperties; }
			}

			public void SetModified()
			{
				_owner.Modified = true;
			}

			internal void NotifyPatientLoaded()
			{
				EventsHelper.Fire(PatientLoaded, this, EventArgs.Empty);
			}

			internal void NotifyOrderLoaded()
			{
				EventsHelper.Fire(OrderLoaded, this, EventArgs.Empty);
			}
		}

		#endregion

		#region Private fields

		private readonly WorkflowConfigurationReader _workflowConfiguration = new WorkflowConfigurationReader();
		private readonly OperatingContext _operatingContext;
		private bool _isComplete;
		private PatientProfileLookupHandler _patientProfileLookupHandler;
		private PatientProfileSummary _patientProfile;
		private EntityRef _orderRef;

		private List<VisitSummary> _allVisits;
		private List<VisitSummary> _applicableVisits;
		private VisitSummary _selectedVisit;
		private bool _hideVisit = true;

		private DiagnosticServiceLookupHandler _diagnosticServiceLookupHandler;

		private List<FacilitySummary> _facilityChoices;
		private List<DepartmentSummary> _departmentChoices;
		private List<ModalitySummary> _modalityChoices;
		private List<EnumValueInfo> _priorityChoices;
		private List<EnumValueInfo> _cancelReasonChoices;

		private FacilitySummary _orderingFacility;

		private ExternalPractitionerLookupHandler _orderingPractitionerLookupHandler;
		private ExternalPractitionerSummary _selectedOrderingPractitioner;
		private ExternalPractitionerContactPointDetail _selectedOrderingPractitionerContactPoint;
		private List<ExternalPractitionerContactPointDetail> _orderingPractitionerContactPointChoices;

		private EnumValueInfo _selectedPriority;
		private EnumValueInfo _selectedCancelReason;

		private DiagnosticServiceSummary _selectedDiagnosticService;

		private DateTime? _schedulingRequestTime;

		private readonly ProcedureRequisitionTable _proceduresTable;
		private readonly CrudActionModel _proceduresActionModel;
		private List<ProcedureRequisition> _selectedProcedures = new List<ProcedureRequisition>();

		private readonly Table<ResultRecipientDetail> _recipientsTable;
		private readonly CrudActionModel _recipientsActionModel;
		private ResultRecipientDetail _selectedRecipient;
		private ExternalPractitionerLookupHandler _recipientLookupHandler;
		private ExternalPractitionerSummary _recipientToAdd;
		private ExternalPractitionerContactPointDetail _recipientContactPointToAdd;
		private List<ExternalPractitionerContactPointDetail> _recipientContactPointChoices;

		private string _indication;
		private List<EnumValueInfo> _lateralityChoices;
		private List<EnumValueInfo> _schedulingCodeChoices;

		private readonly OrderNoteSummaryComponent _noteSummaryComponent;
		private readonly AttachedDocumentPreviewComponent _attachmentSummaryComponent;
		private readonly List<AttachmentSummary> _newAttachments = new List<AttachmentSummary>();
		private Dictionary<string, string> _extendedProperties = new Dictionary<string, string>();

		private ChildComponentHost _noteComponentHost;
		private ChildComponentHost _attachmentsComponentHost;

		private readonly List<IOrderEditorPage> _extensionPages = new List<IOrderEditorPage>();
		private readonly Dictionary<IExtensionPage, ApplicationComponentHost> _extensionPageHosts = new Dictionary<IExtensionPage, ApplicationComponentHost>();
		private readonly OrderEditorContext _extensionPageContext;

		private string _downtimeAccessionNumber;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public OrderEditorComponent(OperatingContext operatingContext)
		{
			Platform.CheckForNullReference(operatingContext, "operatingContext");

			// immediately validate the operating context - will throw if invalid
			operatingContext.Validate();

			_operatingContext = operatingContext;

			_proceduresTable = new ProcedureRequisitionTable();

			_proceduresActionModel = new CrudActionModel();
			_proceduresActionModel.Add.SetClickHandler(AddProcedure);
			_proceduresActionModel.Edit.SetClickHandler(EditSelectedProcedures);
			_proceduresActionModel.Delete.SetClickHandler(CancelSelectedProcedure);

			// in "modify" mode, the Delete action is actually a Cancel action
			if (_operatingContext.Mode == Mode.ModifyOrder)
				_proceduresActionModel.Delete.Label = _proceduresActionModel.Delete.Tooltip = SR.TooltipCancel;


			UpdateProcedureActionModel();

			_recipientsTable = new Table<ResultRecipientDetail>();
			_recipientsTable.Columns.Add(new TableColumn<ResultRecipientDetail, string>(SR.ColumnPractitioner,
				item => PersonNameFormat.Format(item.Practitioner.Name)));
			_recipientsTable.Columns.Add(new TableColumn<ResultRecipientDetail, string>(SR.ColumnContactPoint,
				item => item.ContactPoint.Name));

			_recipientsActionModel = new CrudActionModel(true, false, true);
			_recipientsActionModel.Add.SetClickHandler(AddRecipient);
			_recipientsActionModel.Add.Visible = false;    // hide this action on the menu/toolbar - we'll use a special button instead
			_recipientsActionModel.Delete.SetClickHandler(RemoveSelectedRecipient);
			UpdateRecipientsActionModel();


			this.Validation.Add(new ValidationRule("SelectedVisit",
				component => new ValidationResult(_hideVisit || _selectedVisit != null, SR.MessageVisitRequired)));
			this.Validation.Add(new ValidationRule("SelectedCancelReason",
				component => new ValidationResult(!(_operatingContext.Mode == Mode.ReplaceOrder && _selectedCancelReason == null), SR.MessageCancellationReasonRequired)));
			this.Validation.Add(new ValidationRule("DowntimeAccessionNumber",
				component => new ValidationResult(
					!(this.IsDowntimeAccessionNumberVisible && string.IsNullOrEmpty(_downtimeAccessionNumber)),
					SR.MessageDowntimeAccessionNumberRequired)));

			// add validation rule to ensure the table has at least one non-cancelled procedure
			this.Validation.Add(new ValidationRule("SelectedProcedures",
				component => new ValidationResult(_isComplete || HasActiveProcedures(), SR.MessageNoActiveProcedures)));
			// add validation rule to ensure that any new procedures in the table have scheduled time
			this.Validation.Add(new ValidationRule("SelectedProcedures",
				component => new ValidationResult(_workflowConfiguration.AllowUnscheduledProcedures || AllProceduresAreScheduled(),
					SR.MessageAllProceduresMustBeScheduled)));

			_noteSummaryComponent = new OrderNoteSummaryComponent(OrderNoteCategory.General);
			_noteSummaryComponent.ModifiedChanged += ((sender, args) => this.Modified = true);

			_attachmentSummaryComponent = new AttachedDocumentPreviewComponent(false, AttachmentSite.Order);
			_attachmentSummaryComponent.ModifiedChanged += ((sender, args) => this.Modified = true);

			_extensionPageContext = new OrderEditorContext(this);
		}

		#endregion

		#region ApplicationComponent overrides

		public override void Start()
		{
			_hideVisit = !new WorkflowConfigurationReader().EnableVisitWorkflow;

			_patientProfileLookupHandler = new PatientProfileLookupHandler(this.Host.DesktopWindow);
			_recipientLookupHandler = new ExternalPractitionerLookupHandler(this.Host.DesktopWindow);
			_diagnosticServiceLookupHandler = new DiagnosticServiceLookupHandler(this.Host.DesktopWindow);
			_orderingPractitionerLookupHandler = new ExternalPractitionerLookupHandler(this.Host.DesktopWindow);

			// until we actually load the visits, use an empty list to avoid null ref issues
			_allVisits = new List<VisitSummary>();

			Platform.GetService<IOrderEntryService>(service =>
			{
				var formChoicesResponse = service.GetOrderEntryFormData(new GetOrderEntryFormDataRequest());

				_priorityChoices = formChoicesResponse.OrderPriorityChoices;
				_cancelReasonChoices = formChoicesResponse.CancelReasonChoices;
				_selectedCancelReason = _cancelReasonChoices.Count > 0 ? _cancelReasonChoices[0] : null;
				_facilityChoices = formChoicesResponse.FacilityChoices;
				_departmentChoices = formChoicesResponse.DepartmentChoices;
				_modalityChoices = formChoicesResponse.ModalityChoices;
				_lateralityChoices = formChoicesResponse.LateralityChoices;
				_schedulingCodeChoices = formChoicesResponse.SchedulingCodeChoices;
			});

			if (_operatingContext.Mode == Mode.NewOrder)
			{
				_orderingFacility = LoginSession.Current.WorkingFacility;
				_schedulingRequestTime = Platform.Time;
				_selectedPriority = _priorityChoices.Count > 0 ? _priorityChoices[0] : null;
				_attachmentSummaryComponent.Attachments = _newAttachments;
			}

			InitializeTabPages();

			_operatingContext.Initialize(this);

			base.Start();
		}

		public override void Stop()
		{
			if (_attachmentsComponentHost != null)
			{
				_attachmentsComponentHost.StopComponent();
				_attachmentsComponentHost = null;
			}

			if (_noteComponentHost != null)
			{
				_noteComponentHost.StopComponent();
				_noteComponentHost = null;
			}

			foreach (var kvp in _extensionPageHosts)
			{
				if (kvp.Value.IsStarted)
					kvp.Value.StopComponent();
			}

			base.Stop();
		}

		#endregion

		#region Presentation Model

		public ILookupHandler PatientProfileLookupHandler
		{
			get { return _patientProfileLookupHandler; }
		}

		[ValidateNotNull]
		public PatientProfileSummary SelectedPatientProfile
		{
			get { return _patientProfile; }
			set
			{
				if (value != this.SelectedPatientProfile)
				{
					UpdatePatientProfile(value);
					this.Modified = true;
				}
			}
		}

		public bool IsPatientProfileEditable
		{
			get { return _operatingContext.CanModifyPatient; }
		}

		public bool OrderIsNotCompleted
		{
			get { return _operatingContext.Mode != Mode.ModifyOrder || _isComplete == false; }
		}

		public ApplicationComponentHost OrderNoteSummaryHost
		{
			get { return _noteComponentHost; }
		}

		public ApplicationComponentHost AttachmentsComponentHost
		{
			get { return _attachmentsComponentHost; }
		}

		public IList<IOrderEditorPage> ExtensionPages
		{
			get { return _extensionPages.AsReadOnly(); }
		}

		public ApplicationComponentHost GetExtensionPageHost(IOrderEditorPage page)
		{
			return _extensionPageHosts[page];
		}

		public EntityRef OrderRef
		{
			get { return _orderRef; }
		}

		public bool IsDiagnosticServiceEditable
		{
			get { return _operatingContext.CanModifyDiagnosticService; }
		}

		public bool IsCancelReasonVisible
		{
			get { return _operatingContext.Mode == Mode.ReplaceOrder; }
		}

		public bool IsDowntimeAccessionNumberVisible
		{
			get { return DowntimeRecovery.InDowntimeRecoveryMode && _operatingContext.Mode == Mode.NewOrder; }
		}

		public string AccessionNumberMask
		{
			get { return TextFieldMasks.AccessionNumberMask; }
		}

		public string DowntimeAccessionNumber
		{
			get { return _downtimeAccessionNumber; }
			set
			{
				_downtimeAccessionNumber = value;
				this.Modified = true;
			}
		}

		public bool VisitVisible
		{
			get { return !_hideVisit; }
		}

		public IList ActiveVisits
		{
			get { return _applicableVisits; }
		}

		public VisitSummary SelectedVisit
		{
			get { return _selectedVisit; }
			set
			{
				if (!Equals(value, _selectedVisit))
				{
					_selectedVisit = value;
					NotifyPropertyChanged("SelectedVisit");
					this.Modified = true;
				}
			}
		}

		public string FormatVisit(object visit)
		{
			var visitSummary = (VisitSummary)visit;
			var visitIdentity = new StringBuilder();
			if (visitSummary.Facility != null)
			{
				visitIdentity.Append(visitSummary.Facility.Name);
				visitIdentity.Append(" ");
			}
			visitIdentity.Append(visitSummary.VisitNumber.Id);

			if (visitSummary.CurrentLocation != null)
			{
				visitIdentity.Append(", ");
				visitIdentity.Append(visitSummary.CurrentLocation.Name);
				visitIdentity.Append(",");
			}

			var visitType = new StringBuilder();
			visitType.Append(visitSummary.PatientClass.Value);
			if (visitSummary.Status != null)
			{
				visitType.Append(" - ");
				visitType.Append(visitSummary.Status.Value);
			}

			return string.Format("{0} {1} {2}",
				visitIdentity,
				visitType,
				Format.DateTime(visitSummary.AdmitTime));
		}

		public void ShowVisitSummary()
		{
			try
			{
				var visitSummaryComponent = new VisitSummaryComponent(_patientProfile, true);

				// Add a validation to the visit summary component, validating assigning authority of the selected visit.
				var validCodes = GetValidVisitAssigningAuthorityCodes();
				visitSummaryComponent.Validation.Add(new ValidationRule("SummarySelection",
					component => new ValidationResult(
						visitSummaryComponent.SummarySelection.Item != null && validCodes.Contains(((VisitSummary)visitSummaryComponent.SummarySelection.Item).VisitNumber.AssigningAuthority.Code),
						SR.MessageInvalidVisitAssigningAuthority)));

				var visitDialogArg = new DialogBoxCreationArgs(visitSummaryComponent, SR.TitlePatientVisits, null, DialogSizeHint.Large);
				var exitCode = LaunchAsDialog(this.Host.DesktopWindow, visitDialogArg);

				// remember the previous selection before updating the list
				var selectedVisitRef = _selectedVisit == null ? null : _selectedVisit.VisitRef;

				// if the user made a selection and accepted, then override the previous selection
				if (ApplicationComponentExitCode.Accepted == exitCode)
				{
					var selectedVisit = (VisitSummary)visitSummaryComponent.SummarySelection.Item;
					selectedVisitRef = selectedVisit == null ? null : selectedVisit.VisitRef;
				}

				// regardless of whether the user pressed OK or cancel, we should still update the list of active visits
				// because they could have added a new visit prior to cancelling out of the dialog
				LoadVisits();

				if (selectedVisitRef != null)
				{
					this.SelectedVisit = _applicableVisits.FirstOrDefault(v => v.VisitRef.Equals(selectedVisitRef, true));
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public ILookupHandler DiagnosticServiceLookupHandler
		{
			get { return _diagnosticServiceLookupHandler; }
		}

		[ValidateNotNull]
		public DiagnosticServiceSummary SelectedDiagnosticService
		{
			get { return _selectedDiagnosticService; }
			set
			{
				if (value != this.SelectedDiagnosticService)
				{
					UpdateDiagnosticService(value);
					this.Modified = true;
				}
			}
		}

		public string FormatDiagnosticService(object item)
		{
			return ((DiagnosticServiceSummary)item).Name;
		}

		public ITable Procedures
		{
			get { return _proceduresTable; }
		}

		public ActionModelNode ProceduresActionModel
		{
			get { return _proceduresActionModel; }
		}

		public ISelection SelectedProcedures
		{
			get { return new Selection(_selectedProcedures); }
			set
			{
				_selectedProcedures = value.Items.Cast<ProcedureRequisition>().ToList();

				UpdateProcedureActionModel();
				NotifyPropertyChanged("SelectedProcedures");
			}
		}

		public IList PriorityChoices
		{
			get { return _priorityChoices; }
		}

		[ValidateNotNull]
		public EnumValueInfo SelectedPriority
		{
			get { return _selectedPriority; }
			set
			{
				_selectedPriority = value;
				this.Modified = true;
			}
		}

		public IList CancelReasonChoices
		{
			get { return _cancelReasonChoices; }
		}

		public EnumValueInfo SelectedCancelReason
		{
			get { return _selectedCancelReason; }
			set
			{
				_selectedCancelReason = value;
				this.Modified = true;
			}
		}

		public string OrderingFacility
		{
			get { return _orderingFacility != null ? _orderingFacility.Name : ""; }
		}

		public ILookupHandler OrderingPractitionerLookupHandler
		{
			get { return _orderingPractitionerLookupHandler; }
		}

		[ValidateNotNull]
		public ExternalPractitionerSummary SelectedOrderingPractitioner
		{
			get { return _selectedOrderingPractitioner; }
			set
			{
				if (_selectedOrderingPractitioner != value)
				{
					_selectedOrderingPractitioner = value;
					NotifyPropertyChanged("SelectedOrderingPractitioner");

					_selectedOrderingPractitionerContactPoint = null;
					UpdateOrderingPractitionerContactPointChoices();
					NotifyPropertyChanged("OrderingPractitionerContactPointChoices");

					this.Modified = true;
				}
			}
		}

		public IList OrderingPractitionerContactPointChoices
		{
			get { return _orderingPractitionerContactPointChoices; }
		}

		[ValidateNotNull]
		public ExternalPractitionerContactPointDetail SelectedOrderingPractitionerContactPoint
		{
			get { return _selectedOrderingPractitionerContactPoint; }
			set
			{
				if (_selectedOrderingPractitionerContactPoint != value)
				{
					_selectedOrderingPractitionerContactPoint = value;
					NotifyPropertyChanged("SelectedOrderingPractitionerContactPoint");

					this.Modified = true;
				}
			}
		}

		public string FormatContactPoint(object cp)
		{
			var detail = (ExternalPractitionerContactPointDetail)cp;
			return ExternalPractitionerContactPointFormat.Format(detail);
		}

		public bool IsCopiesToRecipientsPageVisible
		{
			get { return OrderEditorComponentSettings.Default.EnableCopiesToRecipientsPage; }
		}

		public ITable Recipients
		{
			get { return _recipientsTable; }
		}

		public CrudActionModel RecipientsActionModel
		{
			get { return _recipientsActionModel; }
		}

		public ISelection SelectedRecipient
		{
			get { return new Selection(_selectedRecipient); }
			set
			{
				if (!Equals(value, _selectedRecipient))
				{
					_selectedRecipient = (ResultRecipientDetail)value.Item;
					UpdateRecipientsActionModel();
					NotifyPropertyChanged("SelectedRecipient");
				}
			}
		}

		public ILookupHandler RecipientsLookupHandler
		{
			get { return _recipientLookupHandler; }
		}

		public ExternalPractitionerSummary RecipientToAdd
		{
			get { return _recipientToAdd; }
			set
			{
				if (!Equals(value, _recipientToAdd))
				{
					_recipientToAdd = value;
					NotifyPropertyChanged("RecipientToAdd");

					_recipientContactPointToAdd = null;
					UpdateConsultantContactPointChoices();
				}
			}
		}

		public IList RecipientContactPointChoices
		{
			get { return _recipientContactPointChoices; }
		}

		public ExternalPractitionerContactPointDetail RecipientContactPointToAdd
		{
			get { return _recipientContactPointToAdd; }
			set
			{
				if (_recipientContactPointToAdd != value)
				{
					_recipientContactPointToAdd = value;
					NotifyPropertyChanged("RecipientContactPointToAdd");
				}
			}
		}

		[ValidateNotNull]
		public string Indication
		{
			get { return _indication; }
			set
			{
				_indication = value;
				this.Modified = true;
			}
		}

		public DateTime? SchedulingRequestTime
		{
			get { return _schedulingRequestTime; }
			set
			{
				_schedulingRequestTime = value;
				this.Modified = true;
			}
		}

		public bool SchedulingRequestTimeVisible
		{
			get { return _workflowConfiguration.AllowUnscheduledProcedures; }
		}

		public void AddProcedure()
		{
			try
			{
				var procedureRequisition = NewProcedureRequisition(null);
				var procedureEditor = new ProcedureEditorComponent(
					procedureRequisition,
					ProcedureEditorComponent.Mode.Add,
					_facilityChoices,
					_departmentChoices,
					_modalityChoices,
					_lateralityChoices,
					_schedulingCodeChoices);

				if (LaunchAsDialog(this.Host.DesktopWindow, procedureEditor, SR.TitleAddProcedure)
					== ApplicationComponentExitCode.Accepted)
				{
					_proceduresTable.Items.Add(procedureRequisition);

					UpdateApplicableVisits();

					this.Modified = true;
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void EditSelectedProcedures()
		{
			var cannotModifySelectedProcedures = CollectionUtils.Contains(_selectedProcedures, p => !p.CanModify);
			if (cannotModifySelectedProcedures)
				return;

			try
			{
				ProcedureEditorComponentBase editor;
				string title;

				if (_selectedProcedures.Count == 1)
				{
					title = SR.TitleModifyProcedure;
					editor = new ProcedureEditorComponent(
						_selectedProcedures[0],
						ProcedureEditorComponent.Mode.Edit,
						_facilityChoices,
						_departmentChoices,
						_modalityChoices,
						_lateralityChoices,
						_schedulingCodeChoices);
				}
				else
				{
					title = SR.TitleModifyMultipleProcedures;
					editor = new MultipleProceduresEditorComponent(
						_selectedProcedures,
						_facilityChoices,
						_departmentChoices,
						_modalityChoices,
						_lateralityChoices,
						_schedulingCodeChoices);
				}

				if (ApplicationComponentExitCode.Accepted ==
					LaunchAsDialog(this.Host.DesktopWindow, editor, title))
				{
					foreach (var p in _selectedProcedures)
						_proceduresTable.Items.NotifyItemUpdated(p);

					UpdateApplicableVisits();

					this.Modified = true;
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void CancelSelectedProcedure()
		{
			var cannotModifySelectedProcedures = CollectionUtils.Contains(_selectedProcedures, p => !p.CanModify);
			if (cannotModifySelectedProcedures)
				return;

			foreach (var p in _selectedProcedures)
			{
				if (p.Status == null)
				{
					// unsaved procedure
					_proceduresTable.Items.Remove(p);

					UpdateApplicableVisits();

					NotifyPropertyChanged("SelectedProcedure");
				}
				else
				{
					p.Cancelled = true;
					_proceduresTable.Items.NotifyItemUpdated(p);
				}
			}

			this.SelectedProcedures = Selection.Empty;
			this.Modified = true;

			CheckIfOrderShouldBeCancelled();
		}

		public void UpdateProcedureActionModel()
		{
			var canModifySelectedProcedures = CollectionUtils.Contains(_selectedProcedures, p => p.CanModify);
			var canDeleteSelectedProcedures = CollectionUtils.Contains(_selectedProcedures, p => p.CanModify && !p.Cancelled);

			_proceduresActionModel.Add.Enabled = _selectedDiagnosticService != null;
			_proceduresActionModel.Edit.Enabled = canModifySelectedProcedures;
			_proceduresActionModel.Delete.Enabled = canDeleteSelectedProcedures;
		}

		public void AddRecipient()
		{
			if (_recipientToAdd != null && _recipientContactPointToAdd != null)
			{
				_recipientsTable.Items.Add(new ResultRecipientDetail(_recipientToAdd, _recipientContactPointToAdd, new EnumValueInfo("ANY", null, null)));
				this.Modified = true;
			}
		}

		public void RemoveSelectedRecipient()
		{
			_recipientsTable.Items.Remove(_selectedRecipient);
			_selectedRecipient = null;
			NotifyPropertyChanged("SelectedRecipient");
			this.Modified = true;
		}

		public void UpdateRecipientsActionModel()
		{
			_recipientsActionModel.Add.Enabled = (_recipientToAdd != null && _recipientContactPointToAdd != null);
			_recipientsActionModel.Delete.Enabled = (_selectedRecipient != null);
		}

		public void Accept()
		{
			CheckIfOrderShouldBeCancelled();

			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			if (SubmitOrder())
			{
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		public int ProcedureCount
		{
			get { return this._proceduresTable.Items.Count; }
		}

		#endregion

		private void OnOrderRequisitionLoaded(OrderRequisition requisition)
		{
			// update order ref so we have the latest version
			_orderRef = requisition.OrderRef;
			_isComplete = !requisition.CanModify;

			// update form
			UpdateFromRequisition(requisition);
			UpdateApplicableVisits();

			// notify extension pages
			_extensionPageContext.NotifyOrderLoaded();

			this.Modified = false; // bug 6299: ensure we begin without modifications
		}

		private void LoadVisits()
		{
			// remember the previous selection before updating the list
			var selectedVisitRef = _selectedVisit == null ? null : _selectedVisit.VisitRef;

			Platform.GetService<IOrderEntryService>(service =>
			{
				var response = service.ListVisitsForPatient(new ListVisitsForPatientRequest(_patientProfile.PatientRef));
				_allVisits = response.Visits;
			});

			// attempt to re-select the previously selected visit
			if (selectedVisitRef != null)
			{
				this.SelectedVisit = _allVisits.FirstOrDefault(v => v.VisitRef.Equals(selectedVisitRef, true));
			}

			// update the applicable visits list
			UpdateApplicableVisits();
		}

		private void UpdatePatientProfile(PatientProfileSummary patientProfile)
		{
			_patientProfile = patientProfile;

			// notify extension pages
			_extensionPageContext.NotifyPatientLoaded();

			// re-load visits
			LoadVisits();

			NotifyPropertyChanged("SelectedPatientProfile");
		}

		private void UpdateDiagnosticService(DiagnosticServiceSummary summary)
		{
			_selectedDiagnosticService = summary;

			// update the table of procedures
			_proceduresTable.Items.Clear();
			if (_selectedDiagnosticService != null)
			{
				Platform.GetService<IOrderEntryService>(service =>
				{
					var response = service.LoadDiagnosticServicePlan(new LoadDiagnosticServicePlanRequest(summary.DiagnosticServiceRef));
					foreach (var procedureType in response.DiagnosticServicePlan.ProcedureTypes)
					{
						_proceduresTable.Items.Add(NewProcedureRequisition(procedureType));
					}
				});
			}

			UpdateProcedureActionModel();
			UpdateApplicableVisits();

			NotifyPropertyChanged("SelectedDiagnosticService");
		}

		private void UpdateApplicableVisits()
		{
			var selectedVisit = _selectedVisit;

			var validCodes = GetValidVisitAssigningAuthorityCodes();
			_applicableVisits = validCodes.Count == 0
				? _allVisits
				: _allVisits.Where(v => validCodes.Contains(v.VisitNumber.AssigningAuthority.Code)).ToList();

			NotifyPropertyChanged("ActiveVisits");

			// Change to ActiveVisits may have caused the SelectedVisit to update, so use either the saved selectedVisit
			// if it is still applicable, or empty selection.
			this.SelectedVisit = selectedVisit != null
				? _applicableVisits.FirstOrDefault(v => EntityRef.Equals(v.VisitRef, selectedVisit.VisitRef, true))
				: null;
		}

		private List<string> GetValidVisitAssigningAuthorityCodes()
		{
			if (_proceduresTable.Items.Count > 0)
			{
				// Filter by performing facility information authority if there are procedures present
				return _proceduresTable.Items
					.Select(p => p.PerformingFacility.InformationAuthority.Code)
					.Distinct()
					.ToList();
			}

			if (_orderingFacility != null)
			{
				// No procedures but there is an Ordering facility.  use its information authority
				return new List<string> { _orderingFacility.InformationAuthority.Code };
			}

			// Default is an empty list, meaning no filters.
			return new List<string>();
		}

		private OrderRequisition BuildOrderRequisition()
		{
			var requisition = new OrderRequisition
			{
				Patient = _patientProfile,
				Visit = _selectedVisit,
				DiagnosticService = _selectedDiagnosticService,
				ReasonForStudy = _indication,
				Priority = _selectedPriority,
				OrderingFacility = _orderingFacility,
				SchedulingRequestTime = _schedulingRequestTime,
				OrderingPractitioner = _selectedOrderingPractitioner,
				Procedures = new List<ProcedureRequisition>(_proceduresTable.Items),
				Attachments = new List<AttachmentSummary>(_attachmentSummaryComponent.Attachments),
				Notes = new List<OrderNoteDetail>(_noteSummaryComponent.Notes),
				ExtendedProperties = _extendedProperties,
				ResultRecipients = new List<ResultRecipientDetail>(_recipientsTable.Items)
			};

			// only send the downtime number if a new downtime order is being entered
			if (this.IsDowntimeAccessionNumberVisible)
			{
				requisition.DowntimeAccessionNumber = _downtimeAccessionNumber;
				requisition.Notes.Insert(0, new OrderNoteDetail(OrderNoteCategory.General.Key, SR.MessageDowntimeOrderNote, null, false, null, null));
			}
			else
			{
				requisition.DowntimeAccessionNumber = null;
			}

			// there should always be a selected contact point, unless the ordering practitioner has 0 contact points
			if (_selectedOrderingPractitionerContactPoint != null)
			{
				// add the ordering practitioner as a result recipient
				requisition.ResultRecipients.Add(new ResultRecipientDetail(
					_selectedOrderingPractitioner,
					_selectedOrderingPractitionerContactPoint,
					new EnumValueInfo("ANY", null)));
			}

			return requisition;
		}

		private void UpdateFromRequisition(OrderRequisition existingOrder)
		{
			UpdatePatientProfile(existingOrder.Patient);

			_selectedVisit = existingOrder.Visit;
			_selectedDiagnosticService = existingOrder.DiagnosticService;
			_indication = existingOrder.ReasonForStudy;
			_selectedPriority = existingOrder.Priority;
			_orderingFacility = existingOrder.OrderingFacility;
			_schedulingRequestTime = existingOrder.SchedulingRequestTime;
			_selectedOrderingPractitioner = existingOrder.OrderingPractitioner;

			_proceduresTable.Items.Clear();
			foreach (var procedureRequisition in EmptyIfNull(existingOrder.Procedures))
			{
				// apply default values to modifiable procedures, prior to adding to table
				if (procedureRequisition.CanModify)
				{
					_operatingContext.ApplyDefaults(procedureRequisition, this);
				}
				_proceduresTable.Items.Add(procedureRequisition);
			}

			var attachments = new List<AttachmentSummary>(EmptyIfNull(existingOrder.Attachments));
			attachments.AddRange(_newAttachments);
			_attachmentSummaryComponent.Attachments = attachments;

			_noteSummaryComponent.Notes = EmptyIfNull(existingOrder.Notes).ToList();

			_recipientsTable.Items.Clear();
			_recipientsTable.Items.AddRange(EmptyIfNull(existingOrder.ResultRecipients));

			// initialize contact point choices for ordering practitioner
			UpdateOrderingPractitionerContactPointChoices();

			_extendedProperties = existingOrder.ExtendedProperties;
		}

		private IEnumerable<T> EmptyIfNull<T>(IEnumerable<T> collection)
		{
			return collection ?? Enumerable.Empty<T>();
		}

		private bool SubmitOrder()
		{
			// give extension pages a chance to save data prior to commit
			_extensionPages.ForEach(page => page.Save());

			var requisition = BuildOrderRequisition();
			if (!ShowPreSubmitWarnings(requisition))
				return false;

			if (!NotifyOrderSubmitting(requisition))
				return false;

			try
			{
				_orderRef = _operatingContext.Submit(requisition, this);

				NotifyOrderSubmitted(requisition);
				return true;
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, "", this.Host.DesktopWindow, () => this.Exit(ApplicationComponentExitCode.Error));
				return false;
			}
		}

		private bool NotifyOrderSubmitting(OrderRequisition requisition)
		{
			var submittingArgs = new WorkflowEventListener.OrderSubmittingArgs(
				this.Host.DesktopWindow,
				_patientProfile,
				requisition,
				GetSubmitType());
			WorkflowEventPublisher.Instance.OrderSubmitting(submittingArgs);
			return !submittingArgs.Cancel;
		}

		private void NotifyOrderSubmitted(OrderRequisition requisition)
		{
			var submittingArgs = new WorkflowEventListener.OrderSubmittedArgs(
				this.Host.DesktopWindow,
				_patientProfile,
				requisition,
				GetSubmitType());
			WorkflowEventPublisher.Instance.OrderSubmitted(submittingArgs);
		}

		private WorkflowEventListener.OrderSubmitType GetSubmitType()
		{
			switch (_operatingContext.Mode)
			{
				case Mode.NewOrder:
					return WorkflowEventListener.OrderSubmitType.New;
				case Mode.ModifyOrder:
					return WorkflowEventListener.OrderSubmitType.Modify;
				case Mode.ReplaceOrder:
					return WorkflowEventListener.OrderSubmitType.Replace;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void InitializeTabPages()
		{
			_noteComponentHost = new ChildComponentHost(this.Host, _noteSummaryComponent);
			_noteComponentHost.StartComponent();

			_attachmentsComponentHost = new ChildComponentHost(this.Host, _attachmentSummaryComponent);
			_attachmentsComponentHost.StartComponent();

			// instantiate all extension pages
			foreach (IOrderEditorPageProvider pageProvider in new OrderEditorPageProviderExtensionPoint().CreateExtensions())
			{
				_extensionPages.AddRange(pageProvider.GetPages(_extensionPageContext));
			}

			// add extension pages to navigator
			// the navigator will start those components if the user goes to that page
			foreach (var page in _extensionPages)
			{
				var host = new ChildComponentHost(this.Host, page.GetComponent());
				host.StartComponent(); //todo: defer start until tab selected
				_extensionPageHosts[page] = host;
			}
		}

		private void UpdateOrderingPractitionerContactPointChoices()
		{
			GetPractitionerContactPoints(_selectedOrderingPractitioner, OnOrderingPractitionerContactPointChoicesLoaded);
		}

		private void OnOrderingPractitionerContactPointChoicesLoaded(GetExternalPractitionerContactPointsResponse response)
		{
			_orderingPractitionerContactPointChoices = response.ContactPoints;
			NotifyPropertyChanged("OrderingPractitionerContactPointChoices");

			RemovedSelectedOrderingPractitionerFromRecipientsList();
		}

		// what follows is some logic to try hide the ordering practitioner recipient from showing up in the
		// recipients table, since he already appears on the main part of the screen
		private void RemovedSelectedOrderingPractitionerFromRecipientsList()
		{
			// select the recipient representing the ordering practitioner at the default contact point
			var orderingRecipient = CollectionUtils.SelectFirst(
				_recipientsTable.Items,
				recipient => recipient.Practitioner.PractitionerRef == _selectedOrderingPractitioner.PractitionerRef
							 && recipient.ContactPoint.IsDefaultContactPoint);

			// if not found, then select the first recipient representing the ordering practitioner
			if (orderingRecipient == null)
			{
				orderingRecipient = CollectionUtils.SelectFirst(
					_recipientsTable.Items,
					recipient => recipient.Practitioner.PractitionerRef == _selectedOrderingPractitioner.PractitionerRef);
			}

			// if the recipient object exists for the ordering practitioner (and this *should* always be the case)
			if (orderingRecipient != null)
			{
				// initialize the ordering practitioner contact point
				_selectedOrderingPractitionerContactPoint = CollectionUtils.SelectFirst(
					_orderingPractitionerContactPointChoices,
					contactPoint => contactPoint.ContactPointRef == orderingRecipient.ContactPoint.ContactPointRef);

				_recipientsTable.Items.Remove(orderingRecipient);
			}
		}

		private void UpdateConsultantContactPointChoices()
		{
			GetPractitionerContactPoints(_recipientToAdd, OnRecipientContactPointChoicesLoaded);
		}

		private void OnRecipientContactPointChoicesLoaded(GetExternalPractitionerContactPointsResponse response)
		{
			_recipientContactPointChoices = response.ContactPoints;
			NotifyPropertyChanged("RecipientContactPointChoices");

			// must do this after contact point choices have been updated
			UpdateRecipientsActionModel();
		}

		private void GetPractitionerContactPoints(ExternalPractitionerSummary practitioner, Action<GetExternalPractitionerContactPointsResponse> callback)
		{
			if (practitioner != null)
			{
				Platform.GetService<IOrderEntryService>(service =>
				{
					var response = service.GetExternalPractitionerContactPoints(new GetExternalPractitionerContactPointsRequest(practitioner.PractitionerRef));
					callback(response);
				});
			}
			else
			{
				// Empty the contact point list
				callback(new GetExternalPractitionerContactPointsResponse(new List<ExternalPractitionerContactPointDetail>()));
			}
		}

		private ProcedureRequisition NewProcedureRequisition(ProcedureTypeDetail procedureType)
		{
			var performingFacility = _orderingFacility;

			var requisition = new ProcedureRequisition(procedureType != null ? procedureType.GetSummary() : null, performingFacility);
			if (procedureType != null)
			{
				requisition.ScheduledDuration = procedureType.DefaultDuration;
				if (procedureType.DefaultModality != null &&
					procedureType.DefaultModality.Facility != null &&
					procedureType.DefaultModality.Facility.Code == performingFacility.Code)
					requisition.Modality = procedureType.DefaultModality;
			}

			// apply default values
			_operatingContext.ApplyDefaults(requisition, this);
			return requisition;
		}

		private bool HasActiveProcedures()
		{
			return _proceduresTable.Items.Any(p => !p.Cancelled);
		}

		private bool AllProceduresAreScheduled()
		{
			return _proceduresTable.Items.All(p => p.ScheduledTime.HasValue);
		}

		private void CheckIfOrderShouldBeCancelled()
		{
			if (!_isComplete && _operatingContext.Mode != Mode.NewOrder && !HasActiveProcedures())
			{
				var action = this.Host.ShowMessageBox(SR.MessageCancelAllProceduresShouldCancelOrder, MessageBoxActions.YesNo);
				if (action == DialogBoxAction.Yes)
				{
					var cancelled = OrderCancelHelper.CancelOrder(_orderRef, PersonNameFormat.Format(_patientProfile.Name), this.Host.DesktopWindow);
					if (cancelled)
					{
						this.Exit(ApplicationComponentExitCode.Accepted);
					}
				}
			}
		}

		private bool ShowPreSubmitWarnings(OrderRequisition requisition)
		{
			// ignore any procedures that are no longer modifiable (or are pending cancellation), because we don't care if these are in the past
			var modifiableProcedures = requisition.Procedures.Where(p => p.CanModify && !p.Cancelled);

			// look for procedures scheduled in the past
			var now = Platform.Time;
			if (modifiableProcedures.Any(p => p.ScheduledTime.HasValue && (now - p.ScheduledTime.Value > TimeSpan.FromMinutes(1))) &&
				DialogBoxAction.No == this.Host.ShowMessageBox(SR.WarnProceduresScheduledInThePast, MessageBoxActions.YesNo))
				return false;

			return true;
		}
	}
}
