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
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Defines an interface for providing custom pages to be displayed in the reporting component.
	/// </summary>
	public interface IReportingPageProvider : IExtensionPageProvider<IReportingPage, IReportingContext>
	{
	}

	/// <summary>
	/// Defines an interface to a custom reporting page.
	/// </summary>
	public interface IReportingPage : IExtensionPage
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom page with access to the reporting context.
	/// </summary>
	public interface IReportingContext
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

	public interface IReportEditorContextBase<TCloseReason> : IReportingContext
	{
		/// <summary>
		/// Gets a value indicating the active report part is an addendum.
		/// </summary>
		bool IsAddendum { get; }

		/// <summary>
		/// Gets or sets the report content for the active report part.
		/// </summary>
		string ReportContent { get; set; }

		/// <summary>
		/// Gets or sets the extended properties for the active report part.
		/// </summary>
		Dictionary<string, string> ExtendedProperties { get; set; }

		/// <summary>
		/// Gets the specified extended property for the active report part.
		/// </summary>
		string GetReportPartExtendedProperty(string key);

		/// <summary>
		/// Sets the specified extended property for the active report part.
		/// </summary>
		void SetReportPartExtendedProperty(string key, string value);

		/// <summary>
		/// Allows the report editor to request that the reporting component close.
		/// For example, the report editor may wish to have a microphone button initiate the 'Verify' action.
		/// </summary>
		/// <param name="reason"></param>
		void RequestClose(TCloseReason reason);
	}

	public interface IReportEditorBase<TCloseReason>
	{
		/// <summary>
		/// Gets the report editor application component.
		/// </summary>
		/// <returns></returns>
		IApplicationComponent GetComponent();

		/// <summary>
		/// Instructs the report editor to save the report content and any other data.
		/// The report editor may be veto the action by returning false.
		/// </summary>
		/// <param name="reason"></param>
		/// <returns>True to continue with save, or false to veto the operation.</returns>
		bool Save(TCloseReason reason);
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the reporting component.
	/// </summary>
	[ExtensionPoint]
	public class ReportingPageProviderExtensionPoint : ExtensionPoint<IReportingPageProvider>
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom report editor.
	/// </summary>
	public interface IReportEditorProvider
	{
		IReportEditor GetEditor(IReportEditorContext context);
	}

	/// <summary>
	/// Defines an interface for providing a custom report editor page with access to the reporting
	/// context.
	/// </summary>
	public interface IReportEditorContext : IReportEditorContextBase<ReportEditorCloseReason>
	{
		/// <summary>
		/// Gets a value indicating whether the Verify operation is enabled.
		/// </summary>
		bool CanVerify { get; }

		/// <summary>
		/// Gets a value indicating whether the Submit For Review operation is enabled.
		/// </summary>
		bool CanSubmitForReview { get; }

		/// <summary>
		/// Gets a value indicating whether the Send To Transcription operation is enabled.
		/// </summary>
		bool CanSendToTranscription { get; }

		/// <summary>
		/// Gets the supervisor for the active report part.
		/// </summary>
		StaffSummary Supervisor { get; }

		/// <summary>
		/// Raises or lowers the order priority by the specified amount.
		/// </summary>
		/// <param name="delta"></param>
		/// <remarks>
		/// Attempting to raise the priority above the maximum value, or lower it below the minimum value, has no effect. Specifying a delta of 0 will cause the priority to cycle to the next valid value.
		/// </remarks>
		void ChangePriority(int delta);

	}

	/// <summary>
	/// Defines possible reasons that the report editor is closing.
	/// </summary>
	public enum ReportEditorCloseReason
	{
		/// <summary>
		/// User has cancelled editing, leaving the report in its current state.
		/// </summary>
		CancelEditing,

		/// <summary>
		/// Report is saved in its current state.
		/// </summary>
		SaveDraft,

		/// <summary>
		/// Report is saved for transcription.
		/// </summary>
		SendToTranscription,

		/// <summary>
		/// Report is saved and send back to the interpreter.
		/// </summary>
		ReturnToInterpreter,

		/// <summary>
		/// Report is saved and submitted for another radiologist to review.
		/// </summary>
		SubmitForReview,

		/// <summary>
		/// Report is saved and verified immediately.
		/// </summary>
		Verify
	}

	public interface IReportEditor : IReportEditorBase<ReportEditorCloseReason>
	{
	}

	/// <summary>
	/// Defines an extension point for providing a custom report editor.
	/// </summary>
	[ExtensionPoint]
	public class ReportEditorProviderExtensionPoint : ExtensionPoint<IReportEditorProvider>
	{
	}

	/// <summary>
	/// Extension point for views onto <see cref="ReportingComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ReportingComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ReportingComponent class
	/// </summary>
	[AssociateView(typeof(ReportingComponentViewExtensionPoint))]
	public class ReportingComponent : ApplicationComponent
	{
		private class UserSkippedItemWithIncompleteDocumentationException : Exception
		{
		}

		#region ReportingContext

		class ReportingContext : IReportingContext
		{
			private readonly ReportingComponent _owner;

			public ReportingContext(ReportingComponent owner)
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

			protected ReportingComponent Owner
			{
				get { return _owner; }
			}
		}

		#endregion

		#region ReportEditorContext

		class ReportEditorContext : ReportingContext, IReportEditorContext
		{
			public ReportEditorContext(ReportingComponent owner)
				: base(owner)
			{
			}

			public bool CanVerify
			{
				get { return Owner.CanVerify; }
			}

			public bool CanSubmitForReview
			{
				get { return Owner.CanSubmitForReview; }
			}

			public bool CanSendToTranscription
			{
				get { return Owner.CanSendToTranscription; }
			}

			public bool CanSaveReport
			{
				get { return Owner.SaveReportEnabled; }
			}

			public bool IsAddendum
			{
				get { return Owner._activeReportPartIndex > 0; }
			}

			public string ReportContent
			{
				get { return Owner.ReportContent; }
				set { Owner.ReportContent = value; }
			}

			public Dictionary<string, string> ExtendedProperties
			{
				get { return Owner._reportPartExtendedProperties; }
				set { Owner._reportPartExtendedProperties = value; }
			}

			public string GetReportPartExtendedProperty(string key)
			{
				return Owner.GetReportPartExtendedProperty(key);
			}

			public void SetReportPartExtendedProperty(string key, string value)
			{
				this.Owner.SetReportPartExtendedProperty(key, value);
			}

			public void ChangePriority(int delta)
			{
				this.Owner.ChangePriority(delta);
			}

			public StaffSummary Supervisor
			{
				get { return Owner._supervisor; }
			}

			public void RequestClose(ReportEditorCloseReason reason)
			{
				Owner.RequestClose(reason);
			}
		}

		#endregion

		private readonly ReportingComponentWorklistItemManager _worklistItemManager;

		private IReportEditor _reportEditor;
		private ChildComponentHost _reportEditorHost;
		private ChildComponentHost _bannerHost;

		private ChildComponentHost _rightHandComponentContainerHost;
		private TabComponentContainer _rightHandComponentContainer;

		private string _proceduresText;
		private readonly bool _shouldOpenImages;
		private string _imagesAvailablilityMessage;

		private bool _canCompleteInterpretationAndVerify;
		private bool _canCompleteVerification;
		private bool _canSubmitForReview;
		private bool _canReturnToInterpreter;
		private bool _canCompleteInterpretationForTranscription;
		private bool _canSaveReport;

		private EntityRef _assignedStaff;
		private ReportDetail _report;
		private OrderDetail _orderDetail;
		private int _activeReportPartIndex;
		private ILookupHandler _supervisorLookupHandler;
		private StaffSummary _supervisor;
		private bool _rememberSupervisor;
		private Dictionary<string, string> _reportPartExtendedProperties;
		private List<EnumValueInfo> _priorityChoices; 

		private ReportingOrderDetailViewComponent _orderComponent;
		private AttachedDocumentPreviewComponent _orderAttachmentsComponent;
		private PriorReportComponent _priorReportComponent;

		private List<IReportingPage> _extensionPages;
		private bool _userCancelled;
		private event EventHandler _worklistItemChanged;

		private readonly WorkflowConfigurationReader _workflowConfiguration = new WorkflowConfigurationReader();

		private IStudyViewer[] _openViewers;

		/// <summary>
		/// Constructor
		/// </summary>
		public ReportingComponent(ReportingWorklistItemSummary worklistItem, string folderName, EntityRef worklistRef, string worklistClassName, bool shouldOpenImages)
		{
			_worklistItemManager = new ReportingComponentWorklistItemManager(folderName, worklistRef, worklistClassName);
			_worklistItemManager.Initialize(worklistItem);
			_worklistItemManager.WorklistItemChanged += OnWorklistItemChangedEvent;
			_shouldOpenImages = shouldOpenImages;
		}

		#region ApplicationComponent overrides

		public override void Start()
		{
			this.Validation.Add(new ValidationRule("Supervisor",
				delegate
				{
					var ok = !this.SupervisorVisible || _supervisor != null || Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.Report.OmitSupervisor);
					return new ValidationResult(ok, SR.MessageChooseRadiologist);
				}));

			// create supervisor lookup handler, using filters supplied in application settings
			var filters = ReportingSettings.Default.SupervisorStaffTypeFilters;
			var staffTypes = string.IsNullOrEmpty(filters)
				? new string[] { }
				: CollectionUtils.Map<string, string>(filters.Split(','), s => s.Trim()).ToArray();
			_supervisorLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, staffTypes);

			_rememberSupervisor = this.SupervisorVisible && ReportingSettings.Default.ShouldApplyDefaultSupervisor;

			try
			{
				StartReportingWorklistItem();

				_bannerHost = new ChildComponentHost(this.Host, new BannerComponent(this.WorklistItem));
				_bannerHost.StartComponent();

				_rightHandComponentContainer = new TabComponentContainer();
				_rightHandComponentContainer.ValidationStrategy = new AllComponentsValidationStrategy();

				_orderComponent = new ReportingOrderDetailViewComponent(this.WorklistItem.PatientRef, this.WorklistItem.OrderRef);
				_rightHandComponentContainer.Pages.Add(new TabPage(SR.TitleOrder, _orderComponent));

				_orderAttachmentsComponent = new AttachedDocumentPreviewComponent(true, AttachmentSite.Order);
				_orderAttachmentsComponent.OrderRef = this.WorklistItem.OrderRef;
				_rightHandComponentContainer.Pages.Add(new TabPage(SR.TitleOrderAttachments, _orderAttachmentsComponent));

				_priorReportComponent = new PriorReportComponent(this.WorklistItem, _report.ReportRef);
				_rightHandComponentContainer.Pages.Add(new TabPage(SR.TitlePriors, _priorReportComponent));

				// instantiate all extension pages
				_extensionPages = new List<IReportingPage>();
				foreach (IReportingPageProvider pageProvider in new ReportingPageProviderExtensionPoint().CreateExtensions())
				{
					_extensionPages.AddRange(pageProvider.GetPages(new ReportingContext(this)));
				}

				// add extension pages to container and set initial context
				// the container will start those components if the user goes to that page
				foreach (var page in _extensionPages)
				{
					_rightHandComponentContainer.Pages.Add(new TabPage(page.Path, page.GetComponent()));
				}

				_rightHandComponentContainerHost = new ChildComponentHost(this.Host, _rightHandComponentContainer);
				_rightHandComponentContainerHost.StartComponent();

				// check for a report editor provider.  If not found, use the default one
				var provider = CollectionUtils.FirstElement<IReportEditorProvider>(new ReportEditorProviderExtensionPoint().CreateExtensions());

				_reportEditor = provider == null ? new ReportEditorComponent(new ReportEditorContext(this)) : provider.GetEditor(new ReportEditorContext(this));
				_reportEditorHost = new ChildComponentHost(this.Host, _reportEditor.GetComponent());
				_reportEditorHost.StartComponent();
				_reportEditorHost.Component.ModifiedChanged += ((sender, args) => this.Modified = this.Modified || _reportEditorHost.Component.Modified);

				SetInitialReportingTabPage();

				OpenImages();

			}
			catch (UserSkippedItemWithIncompleteDocumentationException)
			{
				_userCancelled = true;
			}

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

			if (_reportEditorHost != null)
			{
				_reportEditorHost.StopComponent();

				if (_reportEditor is IDisposable)
				{
					((IDisposable)_reportEditor).Dispose();
					_reportEditor = null;
				}
			}

			base.Stop();
		}

		public override bool HasValidationErrors
		{
			get
			{
				return _reportEditorHost.Component.HasValidationErrors || base.HasValidationErrors;
			}
		}

		public override void ShowValidation(bool show)
		{
			_reportEditorHost.Component.ShowValidation(show);
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

		public ApplicationComponentHost ReportEditorHost
		{
			get { return _reportEditorHost; }
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

		public string ImageAvailabilityMessage
		{
			get { return _imagesAvailablilityMessage; }
		}

		public bool ImageAvailabilityMessageVisible
		{
			get { return ViewImagesHelper.IsSupported && !string.IsNullOrEmpty(_imagesAvailablilityMessage); }
		}

		public string HasErrorsText
		{
			get
			{
				return this.WorklistItem != null && _report.GetPart(_activeReportPartIndex).TranscriptionRejectReason != null
					? string.Format("{0}: {1}", SR.MessageTranscriptionHasErrors, _report.GetPart(_activeReportPartIndex).TranscriptionRejectReason.Value)
					: "";
			}
		}

		public bool HasErrorsVisible
		{
			get { return this.WorklistItem != null ? this.WorklistItem.HasErrors : false; }
		}

		public string ProceduresText
		{
			get { return string.Format(SR.FormatReportedProcedures, _proceduresText); }
		}

		public bool ReportNextItem
		{
			get { return _worklistItemManager.ReportNextItem; }
			set { _worklistItemManager.ReportNextItem = value; }
		}

		public bool ReportNextItemEnabled
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

		public IList PriorityChoices
		{
			get { return _priorityChoices; }
		}

		public EnumValueInfo Priority
		{
			get { return _orderDetail.OrderPriority; }
			set
			{
				if(!Equals(value, _orderDetail.OrderPriority))
				{
					_orderDetail.OrderPriority = value;
					this.Modified = true;
					NotifyPropertyChanged("Priority");
				}
			}
		}

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
				return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.SubmitForReview)
				       && _workflowConfiguration.EnableInterpretationReviewWorkflow;
			}
		}

		public bool RememberSupervisor
		{
			get { return _rememberSupervisor; }
			set
			{
				if (!Equals(value, _rememberSupervisor))
				{
					_rememberSupervisor = value;
					ReportingSettings.Default.ShouldApplyDefaultSupervisor = _rememberSupervisor;
					ReportingSettings.Default.Save();
					NotifyPropertyChanged("RememberSupervisor");
				}
			}
		}

		public bool RememberSupervisorVisible
		{
			get { return this.SupervisorVisible; }
		}

		#endregion

		#region Verify

		public void Verify()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			// JR: removed during re-org to Extended Workflow plugin... 
			// We can put a hook in here if we need to, but for now assume it isn't needed

			//PreliminaryDiagnosis.ShowDialogOnVerifyIfRequired(this.WorklistItem, this.Host.DesktopWindow,
			//    delegate
			//    {
					try
					{
						CloseImages();

						if (!_reportEditor.Save(ReportEditorCloseReason.Verify))
							return;

						if (_canCompleteInterpretationAndVerify)
						{
							Platform.GetService<IReportingWorkflowService>(service =>
								service.CompleteInterpretationAndVerify(
									new CompleteInterpretationAndVerifyRequest(
										this.WorklistItem.ProcedureStepRef,
										_reportPartExtendedProperties,
										_supervisor == null ? null : _supervisor.StaffRef) { Priority = _orderDetail.OrderPriority }));
						}
						else if (_canCompleteVerification)
						{
							Platform.GetService<IReportingWorkflowService>(service =>
								service.CompleteVerification(
									new CompleteVerificationRequest(this.WorklistItem.ProcedureStepRef, _reportPartExtendedProperties) { Priority = _orderDetail.OrderPriority }));
						}

						// Source Folders
						DocumentManager.InvalidateFolder(typeof(Folders.Reporting.DraftFolder));
						// Destination Folders
						DocumentManager.InvalidateFolder(typeof(Folders.Reporting.VerifiedFolder));

						_worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Completed);
					}
					catch (Exception ex)
					{
						ExceptionHandler.Report(ex, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow, () => this.Exit(ApplicationComponentExitCode.Error));
					}
				//});
		}

		public bool VerifyEnabled
		{
			get { return CanVerify; }
		}

		public bool VerifyReportVisible
		{
			get { return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Verify); }
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

				CloseImages();

				if (!_reportEditor.Save(ReportEditorCloseReason.SubmitForReview))
					return;

				Platform.GetService<IReportingWorkflowService>(service =>
					service.CompleteInterpretationForVerification(
						new CompleteInterpretationForVerificationRequest(
							_worklistItemManager.WorklistItem.ProcedureStepRef,
							_reportPartExtendedProperties,
							_supervisor == null ? null : _supervisor.StaffRef) { Priority = _orderDetail.OrderPriority }));

				// Source Folders
				//DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.DraftFolder));
				// Destination Folders
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.AwaitingReviewFolder));

				_worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Completed);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow, () => this.Exit(ApplicationComponentExitCode.Error));
			}
		}

		public bool SubmitForReviewEnabled
		{
			get { return _workflowConfiguration.EnableInterpretationReviewWorkflow && CanSubmitForReview; }
		}

		public bool SubmitForReviewVisible
		{
			get
			{
				return _workflowConfiguration.EnableInterpretationReviewWorkflow && 
					Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.SubmitForReview);
			}
		}

		#endregion

		#region Send To Resident

		public void ReturnToInterpreter()
		{
			try
			{
				if (this.HasValidationErrors)
				{
					this.ShowValidation(true);
					return;
				}

				CloseImages();

				if (!_reportEditor.Save(ReportEditorCloseReason.ReturnToInterpreter))
					return;

				Platform.GetService<IReportingWorkflowService>(service =>
					service.ReturnToInterpreter(new ReturnToInterpreterRequest(
							this.WorklistItem.ProcedureStepRef,
							_reportPartExtendedProperties,
							_supervisor == null ? null : _supervisor.StaffRef) { Priority = _orderDetail.OrderPriority }));

				// Source Folders, no destination folder
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.ToBeReviewedFolder));

				_worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Completed);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow, () => this.Exit(ApplicationComponentExitCode.Error));
			}
		}

		public bool ReturnToInterpreterEnabled
		{
			get { return _canReturnToInterpreter && _workflowConfiguration.EnableInterpretationReviewWorkflow; }
		}

		public bool ReturnToInterpreterVisible
		{
			get
			{ 
				return _canReturnToInterpreter && _workflowConfiguration.EnableInterpretationReviewWorkflow
						&& Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.Report.Verify);
			}
		}

		#endregion

		#region Send To Transcription

		public void SendToTranscription()
		{
			try
			{
				if (this.HasValidationErrors)
				{
					this.ShowValidation(true);
					return;
				}

				CloseImages();

				if (!_reportEditor.Save(ReportEditorCloseReason.SendToTranscription))
					return;

				Platform.GetService<IReportingWorkflowService>(service =>
					service.CompleteInterpretationForTranscription(
						new CompleteInterpretationForTranscriptionRequest(
							this.WorklistItem.ProcedureStepRef,
							_reportPartExtendedProperties,
							_supervisor == null ? null : _supervisor.StaffRef) { Priority = _orderDetail.OrderPriority }));

				// Source Folders
				//DocumentManager.InvalidateFolder(typeof(Folders.Reporting.ToBeReportedFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.DraftFolder));
				// Destination Folders
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.InTranscriptionFolder));

				_worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Completed);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow, () => this.Exit(ApplicationComponentExitCode.Error));
			}
		}

		public bool SendToTranscriptionEnabled
		{
			get { return CanSendToTranscription; }
		}

		public bool SendToTranscriptionVisible
		{
			get { return _workflowConfiguration.EnableTranscriptionWorkflow; }
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
				CloseImages();

				if (!_reportEditor.Save(ReportEditorCloseReason.SaveDraft))
					return;

				Platform.GetService<IReportingWorkflowService>(service =>
					service.SaveReport(
						new SaveReportRequest(
							this.WorklistItem.ProcedureStepRef,
							_reportPartExtendedProperties,
							_supervisor == null ? null : _supervisor.StaffRef) { Priority = _orderDetail.OrderPriority }));

				// Source Folders
				//DocumentManager.InvalidateFolder(typeof(Folders.Reporting.ToBeReportedFolder));
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.VerifiedFolder));
				// Destination Folders
				DocumentManager.InvalidateFolder(typeof(Folders.Reporting.DraftFolder));
				//DocumentManager.InvalidateFolder(typeof(Folders.Reporting.ToBeVerifiedFolder));

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
				CloseImages();

				if (_worklistItemManager.ShouldUnclaim)
				{
					Platform.GetService<IReportingWorkflowService>(service =>
						service.CancelReportingStep(new CancelReportingStepRequest(this.WorklistItem.ProcedureStepRef, _assignedStaff)));
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

		#endregion

		#region Private Helpers

		private bool CanVerify
		{
			get
			{
				return (_canCompleteInterpretationAndVerify || _canCompleteVerification)
					&& Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Verify);
			}
		}

		private bool CanSubmitForReview
		{
			get
			{
				return _canSubmitForReview;
			}
		}

		private bool CanSendToTranscription
		{
			get
			{
				return _canCompleteInterpretationForTranscription
					&& _workflowConfiguration.EnableTranscriptionWorkflow;
			}
		}

		private bool CanSaveReport
		{
			get { return _canSaveReport; }
		}

		public bool UserCancelled
		{
			get { return _userCancelled; }
		}

		private void SetSupervisor(StaffSummary supervisor)
		{
			_supervisor = supervisor;
			ReportingSettings.Default.SupervisorID = supervisor == null ? "" : supervisor.StaffId;
			ReportingSettings.Default.Save();
		}

		private void RequestClose(ReportEditorCloseReason reason)
		{
			switch (reason)
			{
				case ReportEditorCloseReason.SaveDraft:
					SaveReport();
					break;
				case ReportEditorCloseReason.SendToTranscription:
					SendToTranscription();
					break;
				case ReportEditorCloseReason.ReturnToInterpreter:
					ReturnToInterpreter();
					break;
				case ReportEditorCloseReason.SubmitForReview:
					SubmitForReview();
					break;
				case ReportEditorCloseReason.Verify:
					Verify();
					break;
				case ReportEditorCloseReason.CancelEditing:
					CancelEditing();
					break;
			}
		}

		private void DoCancelCleanUp()
		{
			CloseImages();

			if (!_userCancelled)
			{
				if (_worklistItemManager.ShouldUnclaim)
				{
					Platform.GetService<IReportingWorkflowService>(
						service =>
						service.CancelReportingStep(new CancelReportingStepRequest(this.WorklistItem.ProcedureStepRef, _assignedStaff)));
				}

				SaveInitialReportingTabPage();
			}
		}

		private void OnWorklistItemChangedEvent(object sender, EventArgs args)
		{
			this.Modified = false;

			if (this.WorklistItem != null)
			{
				try
				{
					StartReportingWorklistItem();
					UpdateChildComponents(true);

					// notify extension pages that the worklist item has changed
					EventsHelper.Fire(_worklistItemChanged, this, EventArgs.Empty);

					OpenImages();
				}
				catch (FaultException<ConcurrentModificationException>)
				{
					this._worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Invalid);
				}
				catch (UserSkippedItemWithIncompleteDocumentationException)
				{
					this._worklistItemManager.ProceedToNextWorklistItem(WorklistItemCompletedResult.Skipped);
				}
			}
			else
			{
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
		}

		private static IEnumerable<ProcedureDetail> ReorderProcedures(IEnumerable<ProcedureDetail> procedures, string primaryProcedureName)
		{
			// Take the source list and move the item matching primaryProcedureName to the first item
			var newProcedures = new List<ProcedureDetail>();
			newProcedures.AddRange(procedures);

			var index = newProcedures.FindIndex(p => Equals(p.Type.Name, primaryProcedureName));
			var primaryProcedure = newProcedures[index];
			newProcedures.RemoveAt(index);
			newProcedures.Insert(0, primaryProcedure);

			return newProcedures;
		}

		private void StartReportingWorklistItem()
		{
			ClaimAndLinkWorklistItem(this.WorklistItem);

			Platform.GetService<IReportingWorkflowService>(service =>
			{
				var enablementResponse = service.GetOperationEnablement(new GetOperationEnablementRequest(this.WorklistItem));
				_canCompleteInterpretationAndVerify = enablementResponse.OperationEnablementDictionary["CompleteInterpretationAndVerify"];
				_canCompleteVerification = enablementResponse.OperationEnablementDictionary["CompleteVerification"];
				_canSubmitForReview = enablementResponse.OperationEnablementDictionary["CompleteInterpretationForVerification"];
				_canReturnToInterpreter = enablementResponse.OperationEnablementDictionary["ReturnToInterpreter"];
				_canCompleteInterpretationForTranscription = enablementResponse.OperationEnablementDictionary["CompleteInterpretationForTranscription"];
				_canSaveReport = enablementResponse.OperationEnablementDictionary["SaveReport"];

				var response = service.LoadReportForEdit(new LoadReportForEditRequest(this.WorklistItem.ProcedureStepRef));
				_report = response.Report;
				_activeReportPartIndex = response.ReportPartIndex;
				_orderDetail = response.Order;
				_priorityChoices = response.PriorityChoices;

				var sb = new StringBuilder();
				var procedures = ReorderProcedures(_report.Procedures, this.WorklistItem.ProcedureName);
				foreach (var procedureDetail in procedures)
				{
					sb.Append(ProcedureFormat.Format(procedureDetail) + ", ");
				}
				_proceduresText = sb.ToString().TrimEnd(", ".ToCharArray());

				var activePart = _report.GetPart(_activeReportPartIndex);
				_reportPartExtendedProperties = activePart == null ? null : activePart.ExtendedProperties;
				if (activePart != null && activePart.Supervisor != null)
				{
					// active part already has a supervisor assigned
					_supervisor = activePart.Supervisor;
				}
				else if (this.RememberSupervisorVisible)
				{
					// active part does not have a supervisor assigned
					// if this user has a default supervisor, retrieve it, otherwise leave supervisor as null
					if (_rememberSupervisor && !String.IsNullOrEmpty(ReportingSettings.Default.SupervisorID))
					{
						_supervisor = GetStaffByID(ReportingSettings.Default.SupervisorID);
					}
					else
					{
						_supervisor = null;
					}
				}
			});
		}

		private static StaffSummary GetStaffByID(string id)
		{
			StaffSummary staff = null;
			Platform.GetService<IStaffAdminService>(service =>
			{
				var response = service.ListStaff(new ListStaffRequest(id, null, null, null, null, true));
				staff = CollectionUtils.FirstElement(response.Staffs);
			});
			return staff;
		}

		private void UpdateChildComponents(bool orderDetailIsCurrent)
		{
			((BannerComponent)_bannerHost.Component).HealthcareContext = this.WorklistItem;
			_priorReportComponent.SetContext(this.WorklistItem, _report != null ? _report.ReportRef : null);
			_orderComponent.Context = new ReportingOrderDetailViewComponent.PatientOrderContext(this.WorklistItem.PatientRef, this.WorklistItem.OrderRef);
			_orderAttachmentsComponent.OrderRef = this.WorklistItem.OrderRef;

			this.Host.Title = ReportDocument.GetTitle(this.WorklistItem);

			NotifyPropertyChanged("StatusText");
			NotifyPropertyChanged("ProceduresText");
		}

		private void OpenImages()
		{
			if (!_shouldOpenImages)
				return;
			if (!ViewImagesHelper.IsSupported)
				return;
			if (!ViewImagesHelper.UserHasAccessToViewImages)
				return;

			try
			{
				// order the procedures such that the one the user selected is first
				var procRefs = new[] { this.WorklistItem.ProcedureRef }
					.Concat(_report.Procedures.Where(p => !p.ProcedureRef.Equals(this.WorklistItem.ProcedureRef, true)).Select(p => p.ProcedureRef));

				_openViewers = ViewImagesHelper.ViewStudies(_orderDetail.OrderRef, procRefs, new ViewImagesHelper.ViewStudiesOptions { FallbackToOrder = true });
				SetImageAvailabilityMessage(_openViewers.Length > 0 ? null : "No images found for this procedure.");

				// if multiple viewers were opened, ensure the first one is active
				if(_openViewers.Length > 1)
				{
					_openViewers.First().Activate();
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Info, e);
				SetImageAvailabilityMessage("Error loading images.");
			}
		}

		private void CloseImages()
		{
			if (_openViewers == null)
				return;

			foreach (var viewer in _openViewers)
			{
				try
				{
					viewer.Close();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, "Failed to close image viewer.");
				}
			}
			_openViewers = null;
		}

		#endregion

		internal void EnsureImagesAreVisible()
		{
			OpenImages();
		}

		private bool ClaimAndLinkWorklistItem(ReportingWorklistItemSummary item)
		{
			// no need to claim if the item is not scheduled
			if (item.ActivityStatus.Code != StepState.Scheduled)
				return true;

			if (item.ProcedureStepName == StepType.Interpretation)
			{
				if (ShouldSkipItemWithIncompleteDocumentation(item))
					throw new UserSkippedItemWithIncompleteDocumentationException();

				// if creating a new report, check for linked interpretations
				ReportingWorklistItemSummary primaryItem;
				List<ReportingWorklistItemSummary> linkedInterpretations;
				List<ReportingWorklistItemSummary> candidateInterpretations;
				PromptForLinkedInterpretations(item, out primaryItem, out linkedInterpretations, out candidateInterpretations);
				if (!Equals(item, primaryItem))
				{
					_worklistItemManager.SwapCurentItem(primaryItem);
				}

				try
				{
					// start the interpretation step
					// note: updating only the ProcedureStepRef is hacky - the service should return an updated item
					primaryItem.ProcedureStepRef = StartInterpretation(primaryItem, linkedInterpretations, out _assignedStaff);

					// Mark any of the linked items as ignored, so we don't visit them again
					_worklistItemManager.IgnoreWorklistItems(linkedInterpretations);
				}
				catch
				{
					// If start interpretation failed and there were candidates for linking, let the user know and move to next item.
					if (candidateInterpretations.Count > 0 && this.IsStarted)
					{
						this.Host.ShowMessageBox(SR.ExceptionCannotStartLinkedProcedures, MessageBoxActions.Ok);
						_worklistItemManager.IgnoreWorklistItems(candidateInterpretations);
					}
					throw;
				}
			}
			else if (item.ProcedureStepName == StepType.Verification)
			{
				// start the verification step
				// note: updating only the ProcedureStepRef is hacky - the service should return an updated item
				if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Verify))
					item.ProcedureStepRef = StartVerification(item);
			}
			else if (item.ProcedureStepName == StepType.TranscriptionReview)
			{
				// start the transcription review step
				// note: updating only the ProcedureStepRef is hacky - the service should return an updated item
				if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Create))
					item.ProcedureStepRef = StartTranscriptionReview(item);
			}

			return true;
		}

		private bool ShouldSkipItemWithIncompleteDocumentation(ReportingWorklistItemSummary item)
		{
			string message;
			if (ItemHasIncompleteDocumentation(item, out message))
			{
				ResetChildComponents();
				var dialogBoxAction = this.Host.ShowMessageBox(
					string.Format("{0} [{1}] {2}", message, AccessionFormat.Format(item.AccessionNumber), SR.MessageReportAnyways),
					MessageBoxActions.YesNo);
				return dialogBoxAction == DialogBoxAction.No;
			}
			return false;
		}

		private static bool ItemHasIncompleteDocumentation(ReportingWorklistItemSummary item, out string message)
		{
			var isIncomplete = true;
			var localMessage = "";  // Cannot use 'out' parameter in anonymous method

			Platform.GetService<IReportingWorkflowService>(service =>
			{
				var response = service.GetDocumentationStatus(new GetDocumentationStatusRequest(item.ProcedureRef));
				isIncomplete = response.IsIncomplete;
				localMessage = response.Reason;
			});

			message = localMessage;
			return isIncomplete;
		}

		private void PromptForLinkedInterpretations(ReportingWorklistItemSummary item, out ReportingWorklistItemSummary primaryItem, out List<ReportingWorklistItemSummary> linkedItems, out List<ReportingWorklistItemSummary> candidateItems)
		{
			primaryItem = item;
			linkedItems = new List<ReportingWorklistItemSummary>();
			candidateItems = new List<ReportingWorklistItemSummary>();

			// query server for link candidates
			var anonCandidates = new List<ReportingWorklistItemSummary>();  // cannot use out param in anonymous delegate.
			Platform.GetService<IReportingWorkflowService>(service =>
			{
				var response = service.GetLinkableInterpretations(new GetLinkableInterpretationsRequest(item.ProcedureStepRef));
				anonCandidates = response.IntepretationItems;
			});
			candidateItems.AddRange(anonCandidates);

			if (candidateItems.Count <= 0)
				return;

			// if there are candidates, prompt user to select
			ResetChildComponents();

			var component = new LinkProceduresComponent(item, candidateItems);
			var exitCode = LaunchAsDialog(this.Host.DesktopWindow, component, SR.TitleLinkProcedures);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				primaryItem = component.PrimaryItem;
				linkedItems.Clear();
				linkedItems.AddRange(component.CheckedItems);

				candidateItems.Clear();
				candidateItems.AddRange(component.UncheckedItems);
			}
		}

		private void ChangePriority(int delta)
		{
			var currentPriority = _priorityChoices.IndexOf(_orderDetail.OrderPriority);
			var newPriority = 0;
			if (delta != 0)
				newPriority = Math.Min(Math.Max(currentPriority + delta, 0), _priorityChoices.Count - 1);
			else if (currentPriority + 1 < _priorityChoices.Count)
				newPriority = currentPriority + 1;
			Priority = _priorityChoices[newPriority];
		}

		private void ResetChildComponents()
		{
			// if no child components have been initialized, just return
			if (_bannerHost == null)
				return;

			// force display of Order Details page.  Other pages may not have all the necessary data available to them
			// (e.g. anything that needs the order extended properties)
			_rightHandComponentContainer.CurrentPage = CollectionUtils.FirstElement(_rightHandComponentContainer.Pages);

			_canCompleteInterpretationAndVerify = false;
			_canCompleteVerification = false;
			_canSubmitForReview = false;
			_canReturnToInterpreter = false;
			_canCompleteInterpretationForTranscription = false;
			_canSaveReport = false;

			_proceduresText = "";
			SetImageAvailabilityMessage(null);

			UpdateChildComponents(false);
			_report = null;
			_activeReportPartIndex = 0;
			_orderDetail = null;
			_reportPartExtendedProperties = null;
			_supervisor = null;

			// notify extension pages that the worklist item has changed
			EventsHelper.Fire(_worklistItemChanged, this, EventArgs.Empty);
		}

		private void SetInitialReportingTabPage()
		{
			var selectedTabName = ReportingSettings.Default.InitiallySelectedTabPageName;
			if (string.IsNullOrEmpty(selectedTabName))
				return;

			var requestedTabPage = CollectionUtils.SelectFirst(
				_rightHandComponentContainer.Pages,
				tabPage => tabPage.Name.Equals(selectedTabName, StringComparison.InvariantCultureIgnoreCase));

			if (requestedTabPage != null)
				_rightHandComponentContainer.CurrentPage = requestedTabPage;
		}

		private void SaveInitialReportingTabPage()
		{
			ReportingSettings.Default.InitiallySelectedTabPageName = _rightHandComponentContainer.CurrentPage.Name;
			ReportingSettings.Default.Save();
		}

		private void SetImageAvailabilityMessage(string message)
		{
			_imagesAvailablilityMessage = message;
			NotifyPropertyChanged("ImageAvailabilityMessage");
			NotifyPropertyChanged("ImageAvailabilityMessageVisible");
		}

		[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Verify)]
		private static EntityRef StartVerification(ReportingWorklistItemSummary item)
		{
			EntityRef result = null;
			Platform.GetService<IReportingWorkflowService>(service =>
			{
				var response = service.StartVerification(new StartVerificationRequest(item.ProcedureStepRef));
				result = response.VerificationStepRef;
			});

			return result;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Create)]
		private static EntityRef StartInterpretation(ReportingWorklistItemSummary item, List<ReportingWorklistItemSummary> linkedInterpretations, out EntityRef assignedStaffRef)
		{
			var linkedInterpretationRefs = linkedInterpretations.ConvertAll(x => x.ProcedureStepRef);

			StartInterpretationResponse response = null;
			Platform.GetService<IReportingWorkflowService>(service =>
			{
				response = service.StartInterpretation(new StartInterpretationRequest(item.ProcedureStepRef, linkedInterpretationRefs));
			});

			var result = response.InterpretationStepRef;
			assignedStaffRef = response.AssignedStaffRef;

			return result;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Create)]
		private static EntityRef StartTranscriptionReview(ReportingWorklistItemSummary item)
		{
			EntityRef result = null;
			Platform.GetService<IReportingWorkflowService>(service =>
			{
				var response = service.StartTranscriptionReview(new StartTranscriptionReviewRequest(item.ProcedureStepRef));
				result = response.TranscriptionReviewStepRef;
			});

			return result;
		}
	}
}
