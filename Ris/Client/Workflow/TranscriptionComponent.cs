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
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	#region TranscriptionPage definitions

	/// <summary>
	/// Defines an interface for providing custom pages to be displayed in the reporting component.
	/// </summary>
	public interface ITranscriptionPageProvider : IExtensionPageProvider<ITranscriptionPage, ITranscriptionContext>
	{
	}

	/// <summary>
	/// Defines an interface to a custom reporting page.
	/// </summary>
	public interface ITranscriptionPage : IExtensionPage
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom page with access to the reporting context.
	/// </summary>
	public interface ITranscriptionContext
	{
		/// <summary>
		/// Gets the reporting worklist item.
		/// </summary>
		ReportingWorklistItemSummary WorklistItem { get; }

		/// <summary>
		/// Occurs to indicate that the <see cref="WorklistItem"/> property has changed,
		/// meaning the entire reporting context is now focused on a different report.
		/// </summary>
		event EventHandler WorklistItemChanged;

		/// <summary>
		/// Gets the report associated with the worklist item.  Modifications made to this object
		/// will not be persisted.
		/// </summary>
		ReportDetail Report { get; }

		/// <summary>
		/// Gets the index of the active report part (the part that is being edited).
		/// </summary>
		int ActiveReportPartIndex { get; }

		/// <summary>
		/// Gets the order detail associated with the report.
		/// </summary>
		OrderDetail Order { get; }
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the reporting component.
	/// </summary>
	[ExtensionPoint]
	public class TranscriptionPageProviderExtensionPoint : ExtensionPoint<ITranscriptionPageProvider>
	{
	}

	#endregion

	#region TranscriptionEditor interfaces and extension point

	/// <summary>
	/// Defines an interface for providing a custom report editor.
	/// </summary>
	public interface ITranscriptionEditorProvider
	{
		ITranscriptionEditor GetEditor(ITranscriptionEditorContext context);
	}

	/// <summary>
	/// Defines an interface for providing a custom report editor page with access to the reporting
	/// context.
	/// </summary>
	public interface ITranscriptionEditorContext : IReportEditorContextBase<TranscriptionEditorCloseReason>
	{
		/// <summary>
		/// Gets a value indicating whether the Verify operation is enabled.
		/// </summary>
		bool CanComplete { get; }

		/// <summary>
		/// Gets a value indicating whether the Save operation is enabled.
		/// </summary>
		bool CanSaveReport { get; }

		/// <summary>
		/// Gets a value indicating whether the Reject operation is enabled.
		/// </summary>
		bool CanReject { get; }
	}

	/// <summary>
	/// Defines possible reasons that the report editor is closing.
	/// </summary>
	public enum TranscriptionEditorCloseReason
	{
		/// <summary>
		/// User has cancelled editing, leaving the report in its current state.
		/// </summary>
		CancelEditing,

		/// <summary>
		/// Transcription is saved in its current state.
		/// </summary>
		SaveDraft,

		/// <summary>
		/// Transcription is saved and submitted for review.
		/// </summary>
		SubmitForReview,

		/// <summary>
		/// Transcription is saved, completed and sent to rad.
		/// </summary>
		Complete,

		/// <summary>
		/// Transcription is saved, completed and sent to rad with the indication that it requires corrections.
		/// </summary>
		Reject
	}

	public interface ITranscriptionEditor : IReportEditorBase<TranscriptionEditorCloseReason>
	{
	}

	/// <summary>
	/// Defines an extension point for providing a custom report editor.
	/// </summary>
	[ExtensionPoint]
	public class TranscriptionEditorProviderExtensionPoint : ExtensionPoint<ITranscriptionEditorProvider>
	{
	}

	#endregion

	/// <summary>
	/// Extension point for views onto <see cref="TranscriptionComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class TranscriptionComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// TranscriptionComponent class.
	/// </summary>
	[AssociateView(typeof(TranscriptionComponentViewExtensionPoint))]
	public class TranscriptionComponent : ApplicationComponent
	{
		#region TranscriptionContext

		class TranscriptionContext : ITranscriptionContext
		{
			private readonly TranscriptionComponent _owner;

			protected TranscriptionContext(TranscriptionComponent owner)
			{
				_owner = owner;
			}

			public ReportingWorklistItemSummary WorklistItem
			{
				get { return _owner.WorklistItem; }
			}

			public event EventHandler WorklistItemChanged
			{
				add { _owner._worklistItemChanged += value; }
				remove { _owner._worklistItemChanged -= value; }
			}

			public ReportDetail Report
			{
				get { return _owner._report; }
			}

			public int ActiveReportPartIndex
			{
				get { return _owner._activeReportPartIndex; }
			}

			public OrderDetail Order
			{
				get { return _owner._orderDetail; }
			}

			protected TranscriptionComponent Owner
			{
				get { return _owner; }
			}
		}

		#endregion

		#region TranscriptionEditorContext

		class TranscriptionEditorContext : TranscriptionContext, ITranscriptionEditorContext
		{
			public TranscriptionEditorContext(TranscriptionComponent owner)
				: base(owner)
			{
			}

			public bool CanComplete
			{
				get { return this.Owner.CanComplete; }
			}

			public bool CanReject
			{
				get { return this.Owner.CanReject; }
			}

			public bool CanSaveReport
			{
				get { return this.Owner.SaveReportEnabled; }
			}

			public bool IsAddendum
			{
				get { return this.Owner._activeReportPartIndex > 0; }
			}

			public string ReportContent
			{
				get { return this.Owner.ReportContent; }
				set { this.Owner.ReportContent = value; }
			}

			public Dictionary<string, string> ExtendedProperties
			{
				get { return this.Owner._reportPartExtendedProperties; }
				set { this.Owner._reportPartExtendedProperties = value; }
			}

			public string GetReportPartExtendedProperty(string key)
			{
				return Owner.GetReportPartExtendedProperty(key);
			}

			public void SetReportPartExtendedProperty(string key, string value)
			{
				this.Owner.SetReportPartExtendedProperty(key, value);
			}

			public void RequestClose(TranscriptionEditorCloseReason reason)
			{
				this.Owner.RequestClose(reason);
			}
		}

		#endregion

		private readonly TranscriptionComponentWorklistItemManager _worklistItemManager;

		private ITranscriptionEditor _transcriptionEditor;
		private ChildComponentHost _transcriptionEditorHost;
		private ChildComponentHost _bannerHost;

		private ChildComponentHost _rightHandComponentContainerHost;
		private TabComponentContainer _rightHandComponentContainer;

		private bool _canComplete;
		private bool _canReject;
		private bool _canSubmitForReview;
		private bool _canSaveReport;

		private ReportDetail _report;
		private OrderDetail _orderDetail;
		private int _activeReportPartIndex;
		private ILookupHandler _supervisorLookupHandler;
		private StaffSummary _supervisor;
		private Dictionary<string, string> _reportPartExtendedProperties;

		private ReportingOrderDetailViewComponent _orderComponent;

		private event EventHandler _worklistItemChanged;

		private readonly WorkflowConfigurationReader _workflowConfiguration = new WorkflowConfigurationReader();

		/// <summary>
		/// Constructor.
		/// </summary>
		public TranscriptionComponent(ReportingWorklistItemSummary worklistItem, string folderName, EntityRef worklistRef, string worklistClassName)
		{
			_worklistItemManager = new TranscriptionComponentWorklistItemManager(folderName, worklistRef, worklistClassName);
			_worklistItemManager.Initialize(worklistItem);
			_worklistItemManager.WorklistItemChanged += OnWorklistItemChangedEvent;
		}

		#region ApplicationComponent overrides

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			this.Validation.Add(new ValidationRule("Supervisor",
				delegate
				{
					var ok = _supervisor != null || SubmitForReviewVisible == false;
					return new ValidationResult(ok, SR.MessageChooseRadiologist);
				}));

			//// create supervisor lookup handler, using filters supplied in application settings
			var filters = TranscriptionSettings.Default.SupervisorStaffTypeFilters;
			var staffTypes = string.IsNullOrEmpty(filters)
				? new string[] { }
				: CollectionUtils.Map<string, string>(filters.Split(','), s => s.Trim()).ToArray();
			_supervisorLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, staffTypes);

			StartTranscribingWorklistItem();

			_bannerHost = new ChildComponentHost(this.Host, new BannerComponent(this.WorklistItem));
			_bannerHost.StartComponent();

			_rightHandComponentContainer = new TabComponentContainer
			{
				ValidationStrategy = new AllComponentsValidationStrategy()
			};

			_orderComponent = new ReportingOrderDetailViewComponent(this.WorklistItem.PatientRef, this.WorklistItem.OrderRef);
			_rightHandComponentContainer.Pages.Add(new TabPage("Order", _orderComponent));

			_rightHandComponentContainerHost = new ChildComponentHost(this.Host, _rightHandComponentContainer);
			_rightHandComponentContainerHost.StartComponent();

			// check for a report editor provider.  If not found, use the default one
			var transcriptionEditorProvider = CollectionUtils.FirstElement<ITranscriptionEditorProvider>(
													new TranscriptionEditorProviderExtensionPoint().CreateExtensions());

			_transcriptionEditor = transcriptionEditorProvider == null
				? new TranscriptionEditorComponent(new TranscriptionEditorContext(this))
				: transcriptionEditorProvider.GetEditor(new TranscriptionEditorContext(this));
			_transcriptionEditorHost = new ChildComponentHost(this.Host, _transcriptionEditor.GetComponent());
			_transcriptionEditorHost.StartComponent();
			_transcriptionEditorHost.Component.ModifiedChanged += ((sender, args) => this.Modified = this.Modified || _transcriptionEditorHost.Component.Modified);

			base.Start();
		}

		public override void Stop()
		{
			if (_bannerHost != null)
			{
				_bannerHost.StopComponent();
				_bannerHost = null;
			}

			if (_rightHandComponentContainerHost != null)
			{
				_rightHandComponentContainerHost.StopComponent();
				_rightHandComponentContainerHost = null;
			}

			if (_transcriptionEditorHost != null)
			{
				_transcriptionEditorHost.StopComponent();

				if (_transcriptionEditorHost is IDisposable)
				{
					((IDisposable)_transcriptionEditorHost).Dispose();
					_transcriptionEditor = null;
				}
			}

			base.Stop();
		}

		public override bool HasValidationErrors
		{
			get
			{
				return _transcriptionEditorHost.Component.HasValidationErrors || base.HasValidationErrors;
			}
		}

		public override void ShowValidation(bool show)
		{
			_transcriptionEditorHost.Component.ShowValidation(show);
			base.ShowValidation(show);
		}

		public override bool PrepareExit()
		{
			var exit = base.PrepareExit();
			if (exit)
			{
				// same as cancel
				DoCancelCleanUp();
			}
			return exit;
		}

		#endregion

		private ReportingWorklistItemSummary WorklistItem
		{
			get { return _worklistItemManager.WorklistItem; }
		}

		#region Presentation Model

		public int BannerHeight
		{
			get { return BannerSettings.Default.BannerHeight; }
		}

		public ApplicationComponentHost BannerHost
		{
			get { return _bannerHost; }
		}

		public ApplicationComponentHost TranscriptionEditorHost
		{
			get { return _transcriptionEditorHost; }
		}

		public ApplicationComponentHost RightHandComponentContainerHost
		{
			get { return _rightHandComponentContainerHost; }
		}

		public string StatusText
		{
			get { return _worklistItemManager.StatusText; }
		}

		public bool StatusTextVisible
		{
			get { return _worklistItemManager.StatusTextVisible; }
		}

		public bool TranscribeNextItem
		{
			get { return _worklistItemManager.ReportNextItem; }
			set { _worklistItemManager.ReportNextItem = value; }
		}

		public bool TranscribeNextItemEnabled
		{
			get { return _worklistItemManager.ReportNextItemEnabled; }
		}

		public string ReportContent
		{
			get { return GetReportPartExtendedProperty(ReportPartDetail.ReportContentKey); }
			set { SetReportPartExtendedProperty(ReportPartDetail.ReportContentKey, value); }
		}

		private string GetReportPartExtendedProperty(string key)
		{
			if (_reportPartExtendedProperties == null || !_reportPartExtendedProperties.ContainsKey(key))
				return null;

			return _reportPartExtendedProperties[key];
		}

		private void SetReportPartExtendedProperty(string key, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				if (_reportPartExtendedProperties != null && _reportPartExtendedProperties.ContainsKey(key))
				{
					_reportPartExtendedProperties.Remove(key);
				}
			}
			else
			{
				if (_reportPartExtendedProperties == null)
					_reportPartExtendedProperties = new Dictionary<string, string>();

				_reportPartExtendedProperties[key] = value;
			}
		}

		#region Complete

		public void Complete()
		{
			try
			{
				if (this.HasValidationErrors)
				{
					this.ShowValidation(true);
					return;
				}

				if (!_transcriptionEditor.Save(TranscriptionEditorCloseReason.Complete))
					return;

				Platform.GetService<ITranscriptionWorkflowService>(service =>
					service.CompleteTranscription(new CompleteTranscriptionRequest(this.WorklistItem.ProcedureStepRef, _reportPartExtendedProperties)));

				// Source Folders
				DocumentManager.InvalidateFolder(typeof(Folders.Transcription.DraftFolder));
				// Destination Folders
				DocumentManager.InvalidateFolder(typeof(Folders.Transcription.CompletedFolder));

				_worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Completed);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow, () => this.Exit(ApplicationComponentExitCode.Error));
			}
		}

		public bool CompleteEnabled
		{
			get { return this.CanComplete; }
		}

		#endregion

		#region Reject

		public void Reject()
		{
			try
			{
				EnumValueInfo rejectReason;
				string additionalComments;

				var rejectReasonSelected = GetRejectReason("Reject Reason", out rejectReason, out additionalComments);

				if (!rejectReasonSelected || rejectReason == null)
					return;

				if (!_transcriptionEditor.Save(TranscriptionEditorCloseReason.Complete))
					return;

				Platform.GetService<ITranscriptionWorkflowService>(service =>
					service.RejectTranscription(new RejectTranscriptionRequest(
						this.WorklistItem.ProcedureStepRef,
						_reportPartExtendedProperties,
						rejectReason,
						CreateAdditionalCommentsNote(additionalComments))));

				// Source Folders
				DocumentManager.InvalidateFolder(typeof(Folders.Transcription.DraftFolder));
				// Destination Folders
				DocumentManager.InvalidateFolder(typeof(Folders.Transcription.CompletedFolder));

				_worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Completed);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow, () => this.Exit(ApplicationComponentExitCode.Error));
			}
		}

		public bool RejectEnabled
		{
			get { return this.CanReject; }
		}

		#endregion

		#region Submit For Review

		public void SubmitForReview()
		{
			try
			{
				if (this.HasValidationErrors)
				{
					this.ShowValidation(true);
					return;
				}

				if (!_transcriptionEditor.Save(TranscriptionEditorCloseReason.SaveDraft))
					return;

				Platform.GetService<ITranscriptionWorkflowService>(service =>
					service.SubmitTranscriptionForReview(new SubmitTranscriptionForReviewRequest(
						this.WorklistItem.ProcedureStepRef,
						_reportPartExtendedProperties,
						_supervisor.StaffRef)));

				// Destination Folders
				DocumentManager.InvalidateFolder(typeof(Folders.Transcription.AwaitingReviewFolder));

				_worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Completed);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.ExceptionFailedToSaveReport, this.Host.DesktopWindow, () => this.Exit(ApplicationComponentExitCode.Error));
			}
		}

		public bool SubmitForReviewEnabled
		{
			get { return _workflowConfiguration.EnableTranscriptionReviewWorkflow && CanSubmitForReview; }
		}

		public bool SubmitForReviewVisible
		{
			get
			{
				return _workflowConfiguration.EnableTranscriptionReviewWorkflow &&
					Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Transcription.SubmitForReview);
			}
		}

		#endregion

		#region Save

		public void SaveReport()
		{
			SaveReport(false);
		}

		public void SaveReport(bool overrideDoNotPerformNextItem)
		{
			try
			{
				if (!_transcriptionEditor.Save(TranscriptionEditorCloseReason.SaveDraft))
					return;

				Platform.GetService<ITranscriptionWorkflowService>(service => 
					service.SaveTranscription(new SaveTranscriptionRequest(
						this.WorklistItem.ProcedureStepRef,
						_reportPartExtendedProperties,
						_supervisor == null ? null : _supervisor.StaffRef)));

				// Source Folders
				DocumentManager.InvalidateFolder(typeof(Folders.Transcription.CompletedFolder));
				// Destination Folders
				DocumentManager.InvalidateFolder(typeof(Folders.Transcription.DraftFolder));

				_worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Completed, overrideDoNotPerformNextItem);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.ExceptionFailedToSaveReport, this.Host.DesktopWindow, () => this.Exit(ApplicationComponentExitCode.Error));
			}
		}

		public bool SaveReportEnabled
		{
			get { return CanSaveReport; }
		}

		#endregion

		#region Skip

		public void Skip()
		{
			try
			{
				if (_worklistItemManager.ShouldUnclaim)
				{
					Platform.GetService<ITranscriptionWorkflowService>(service => 
						service.DiscardTranscription(new DiscardTranscriptionRequest(this.WorklistItem.ProcedureStepRef)));
				}

				_worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Skipped);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public bool SkipEnabled
		{
			get { return _worklistItemManager.CanSkipItem; }
		}

		#endregion

		#region Cancel

		public void CancelEditing()
		{
			try
			{
				DoCancelCleanUp();
				this.Exit(ApplicationComponentExitCode.None);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		#endregion

		#region Supervisor

		public StaffSummary Supervisor
		{
			get { return _supervisor; }
			set
			{
				if (!Equals(value, _supervisor))
				{
					SetSupervisor(value);
					NotifyPropertyChanged("Supervisor");
				}
			}
		}

		public ILookupHandler SupervisorLookupHandler
		{
			get { return _supervisorLookupHandler; }
		}

		public bool SupervisorVisible
		{
			get
			{ 
				return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Transcription.SubmitForReview)
							&& _workflowConfiguration.EnableTranscriptionReviewWorkflow;
			}
		}

		#endregion

		#endregion

		#region Private methods

		private bool CanComplete
		{
			get { return _canComplete; }
		}

		private bool CanReject
		{
			get { return _canReject; }
		}

		private bool CanSubmitForReview
		{
			get { return _canSubmitForReview; }
		}

		private bool CanSaveReport
		{
			get { return _canSaveReport; }
		}

		private void RequestClose(TranscriptionEditorCloseReason reason)
		{
			switch (reason)
			{
				case TranscriptionEditorCloseReason.SaveDraft:
					SaveReport();
					break;
				case TranscriptionEditorCloseReason.SubmitForReview:
					SubmitForReview();
					break;
				case TranscriptionEditorCloseReason.Complete:
					Complete();
					break;
				case TranscriptionEditorCloseReason.Reject:
					Reject();
					break;
				case TranscriptionEditorCloseReason.CancelEditing:
					CancelEditing();
					break;
			}
		}

		private void DoCancelCleanUp()
		{
			if (_worklistItemManager.ShouldUnclaim)
			{
				Platform.GetService<ITranscriptionWorkflowService>(
					service => service.DiscardTranscription(new DiscardTranscriptionRequest(this.WorklistItem.ProcedureStepRef)));
			}
		}

		private void OnWorklistItemChangedEvent(object sender, EventArgs args)
		{
			this.Modified = false;

			if (this.WorklistItem != null)
			{
				try
				{
					StartTranscribingWorklistItem();
					UpdateChildComponents();
					// notify extension pages that the worklist item has changed
					EventsHelper.Fire(_worklistItemChanged, this, EventArgs.Empty);
				}
				catch (FaultException<ConcurrentModificationException>)
				{
					this._worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Invalid);
				}
			}
			else
			{
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
		}

		private void StartTranscribingWorklistItem()
		{
			ClaimWorklistItem(this.WorklistItem);

			Platform.GetService<ITranscriptionWorkflowService>(service =>
			{
				var enablementResponse = service.GetOperationEnablement(new GetOperationEnablementRequest(this.WorklistItem));
				_canComplete = enablementResponse.OperationEnablementDictionary["CompleteTranscription"];
				_canReject = enablementResponse.OperationEnablementDictionary["RejectTranscription"];
				_canSubmitForReview = enablementResponse.OperationEnablementDictionary["SubmitTranscriptionForReview"];
				_canSaveReport = enablementResponse.OperationEnablementDictionary["SaveTranscription"];

				var loadTranscriptionForEditResponse = service.LoadTranscriptionForEdit(new LoadTranscriptionForEditRequest(this.WorklistItem.ProcedureStepRef));
				_report = loadTranscriptionForEditResponse.Report;
				_activeReportPartIndex = loadTranscriptionForEditResponse.ReportPartIndex;
				_orderDetail = loadTranscriptionForEditResponse.Order;

				var activePart = _report.GetPart(_activeReportPartIndex);
				_reportPartExtendedProperties = activePart == null ? null : activePart.ExtendedProperties;

				if (activePart != null && activePart.TranscriptionSupervisor != null)
				{
					// active part already has a supervisor assigned
					_supervisor = activePart.TranscriptionSupervisor;
				}
				else if (
					Thread.CurrentPrincipal.IsInRole(
						ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Transcription.SubmitForReview))
				{
					// active part does not have a supervisor assigned
					// if this user has a default supervisor, retreive it, otherwise leave supervisor as null
					if (!String.IsNullOrEmpty(TranscriptionSettings.Default.SupervisorID))
					{
						_supervisor = GetStaffByID(TranscriptionSettings.Default.SupervisorID);
					}
				}
			});
		}

		private StaffSummary GetStaffByID(string id)
		{
				StaffSummary staff = null;
				Platform.GetService<IStaffAdminService>(service =>
				{
					var response = service.ListStaff(new ListStaffRequest(id, null, null, null, null, true));
					staff = CollectionUtils.FirstElement(response.Staffs);
				});
				return staff;
		}

		private void ClaimWorklistItem(ReportingWorklistItemSummary item)
		{
			if (item.ActivityStatus.Code != StepState.Scheduled) 
				return;
			
			// start the interpretation step
			// note: updating only the ProcedureStepRef is hacky - the service should return an updated item
			StartTranscriptionResponse response = null;
			Platform.GetService<ITranscriptionWorkflowService>(
				service => response = service.StartTranscription(new StartTranscriptionRequest(item.ProcedureStepRef)));

			item.ProcedureStepRef = response.TranscriptionStepRef;
		}
		private void UpdateChildComponents()
		{
			((BannerComponent)_bannerHost.Component).HealthcareContext = this.WorklistItem;
			_orderComponent.Context = new ReportingOrderDetailViewComponent.PatientOrderContext(this.WorklistItem.PatientRef, this.WorklistItem.OrderRef);

			this.Host.Title = ReportDocument.GetTitle(this.WorklistItem);

			NotifyPropertyChanged("StatusText");
		}

		private void SetSupervisor(StaffSummary supervisor)
		{
			_supervisor = supervisor;
			TranscriptionSettings.Default.SupervisorID = supervisor == null ? "" : supervisor.StaffId;
			TranscriptionSettings.Default.Save();
		}

		private bool GetRejectReason(string title, out EnumValueInfo reason, out string additionalComments)
		{
			var rejectReasonComponent = new TranscriptionRejectReasonComponent();

			var exitCode = LaunchAsDialog(this.Host.DesktopWindow, rejectReasonComponent, title);

			reason = rejectReasonComponent.Reason;
			additionalComments = rejectReasonComponent.OtherReason;

			return exitCode == ApplicationComponentExitCode.Accepted;
		}

		private static OrderNoteDetail CreateAdditionalCommentsNote(string additionalComments)
		{
			return !string.IsNullOrEmpty(additionalComments) 
				? new OrderNoteDetail(OrderNoteCategory.General.Key, additionalComments, null, false, null, null) 
				: null;
		}

		#endregion
	}
}
