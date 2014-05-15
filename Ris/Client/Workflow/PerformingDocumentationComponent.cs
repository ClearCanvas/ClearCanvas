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
using System.Security.Permissions;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;


namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Defines an interface for providing custom documentation pages to be displayed in the documentation workspace.
	/// </summary>
	public interface IPerformingDocumentationPageProvider : IExtensionPageProvider<IPerformingDocumentationPage, IPerformingDocumentationContext>
	{
	}

	/// <summary>
	/// Defines an extension point for adding custom documentation pages to the performing documentation workspace.
	/// </summary>
	[ExtensionPoint]
	public class PerformingDocumentationPageProviderExtensionPoint : ExtensionPoint<IPerformingDocumentationPageProvider>
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom documentation page with access to the documentation
	/// context.
	/// </summary>
	public interface IPerformingDocumentationContext
	{
		/// <summary>
		/// Exposes the order reference.
		/// </summary>
		EntityRef OrderRef { get; }

		/// <summary>
		/// Exposes the extended properties associated with the Order.  Modifications made to these
		/// properties by the documentation page will be persisted whenever the documentation workspace is saved.
		/// </summary>
		IDictionary<string, string> OrderExtendedProperties { get; }

		/// <summary>
		/// Exposes the order notes associated with the order.  Modifications made to this
		/// collection will be persisted when the documentation workspace is saved.
		/// </summary>
		List<OrderNoteDetail> OrderNotes { get; }

		/// <summary>
		/// Gets the <see cref="ProcedurePlanDetail"/> representing this order.
		/// </summary>
		ProcedurePlanDetail ProcedurePlan { get; }

		/// <summary>
		/// Occurs when the value of the <see cref="ProcedurePlan"/> property changes.
		/// </summary>
		event EventHandler ProcedurePlanChanged;
	}

	/// <summary>
	/// Extension point for views onto <see cref="PerformingDocumentationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class PerformingDocumentationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PerformingDocumentationComponent class
	/// </summary>
	[AssociateView(typeof(PerformingDocumentationComponentViewExtensionPoint))]
	public class PerformingDocumentationComponent : ApplicationComponent
	{
		#region PerformingDocumentationContext class

		class PerformingDocumentationContext : IPerformingDocumentationContext
		{
			private readonly PerformingDocumentationComponent _owner;

			public PerformingDocumentationContext(PerformingDocumentationComponent owner)
			{
				_owner = owner;
			}

			#region IPerformingDocumentationContext Members

			public EntityRef OrderRef
			{
				get { return _owner._procedurePlan.OrderRef; }
			}

			public IDictionary<string, string> OrderExtendedProperties
			{
				get { return _owner._orderExtendedProperties; }
			}

			public List<OrderNoteDetail> OrderNotes
			{
				get { return _owner._orderNotes; }
			}

			public ProcedurePlanDetail ProcedurePlan
			{
				get { return _owner._procedurePlan; }
			}

			public event EventHandler ProcedurePlanChanged
			{
				add { _owner._procedurePlanChanged += value; }
				remove { _owner._procedurePlanChanged -= value; }
			}


			#endregion
		}

		#endregion

		#region Private Members

		private readonly ModalityWorklistItemSummary _worklistItem;
		private Dictionary<string, string> _orderExtendedProperties;
		private List<OrderNoteDetail> _orderNotes;

		private ProcedurePlanDetail _procedurePlan;
		private ProcedurePlanSummaryTable _procedurePlanSummaryTable;
		private event EventHandler _procedurePlanChanged;

		private SimpleActionModel _procedurePlanActionHandler;
		private ClickAction _startAction;
		private ClickAction _discontinueAction;

		private ChildComponentHost _bannerComponentHost;
		private ChildComponentHost _documentationHost;
		private TabComponentContainer _documentationTabContainer;

		private ILookupHandler _radiologistLookupHandler;
		private StaffSummary _assignedRadiologist;

		private readonly List<IPerformingDocumentationPage> _extensionPages = new List<IPerformingDocumentationPage>();

		private PerformedProcedureComponent _ppsComponent;
		private PerformingDocumentationOrderDetailsComponent _orderDetailsComponent;

		private bool _saveEnabled;
		private bool _completeEnabled;
		private bool _alreadyCompleted;

		#endregion

		public PerformingDocumentationComponent(ModalityWorklistItemSummary item)
		{
			_worklistItem = item;
			_saveEnabled = true;
		}

		#region ApplicationComponent overrides

		public override void Start()
		{
			InitializeProcedurePlanSummary();
			InitializeDocumentationTabPages();

			// create staff lookup handler, using filters provided by application configuration
			var filters = PerformingDocumentationComponentSettings.Default.RadiologistStaffTypeFilters;
			var staffTypes = string.IsNullOrEmpty(filters)
				? new string[] { }
				: CollectionUtils.Map<string, string>(filters.Split(','), s => s.Trim()).ToArray();

			_radiologistLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, staffTypes);

			base.Start();
		}

		public override void Stop()
		{
			if (_bannerComponentHost != null)
			{
				_bannerComponentHost.StopComponent();
				_bannerComponentHost = null;
			}

			if (_documentationHost != null)
			{
				_documentationHost.StopComponent();
				_documentationHost = null;
			}

			base.Stop();
		}

		public override bool HasValidationErrors
		{
			get
			{
				return _documentationTabContainer.HasValidationErrors || base.HasValidationErrors;
			}
		}

		public override void ShowValidation(bool show)
		{
			_documentationTabContainer.ShowValidation(show);
			base.ShowValidation(show);
		}

		#endregion

		#region Presentation Model Methods

		public int BannerHeight
		{
			get { return BannerSettings.Default.BannerHeight; }
		}

		public ApplicationComponentHost BannerHost
		{
			get { return _bannerComponentHost; }
		}

		public ApplicationComponentHost DocumentationHost
		{
			get { return _documentationHost; }
		}

		public ITable ProcedurePlanSummaryTable
		{
			get { return _procedurePlanSummaryTable; }
		}

		public event EventHandler ProcedurePlanChanged
		{
			add { _procedurePlanChanged += value; }
			remove { _procedurePlanChanged -= value; }
		}

		public ActionModelNode ProcedurePlanTreeActionModel
		{
			get { return _procedurePlanActionHandler; }
		}

		public ILookupHandler RadiologistLookupHandler
		{
			get { return _radiologistLookupHandler; }
		}

		public StaffSummary AssignedRadiologist
		{
			get { return _assignedRadiologist; }
			set
			{
				if (!Equals(value, _assignedRadiologist))
				{
					_assignedRadiologist = value;
					NotifyPropertyChanged("AssignedRadiologist");
				}
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Documentation.Create)]
		public void SaveDocumentation()
		{
			try
			{
				if (Save(_alreadyCompleted))
				{
					DocumentManager.InvalidateFolder(typeof(Folders.Performing.InProgressFolder));
					DocumentManager.InvalidateFolder(typeof(Folders.Performing.UndocumentedFolder));
					DocumentManager.InvalidateFolder(typeof(Folders.Performing.CancelledFolder));
					this.Exit(ApplicationComponentExitCode.Accepted);
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public string SaveText
		{
			get { return _alreadyCompleted ? SR.TextUpdate : SR.TextSave; }
		}

		public bool SaveEnabled
		{
			get { return _saveEnabled; }
		}

		[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Documentation.Accept)]
		public void CompleteDocumentation()
		{
			try
			{
				// validate first
				if (Save(true))
				{
					DocumentManager.InvalidateFolder(typeof(Folders.Performing.CancelledFolder));
					DocumentManager.InvalidateFolder(typeof(Folders.Performing.PerformedFolder));
					this.Exit(ApplicationComponentExitCode.Accepted);
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public bool CompleteEnabled
		{
			get { return _completeEnabled; }
		}

		public bool CompleteVisible
		{
			get
			{
				return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Documentation.Accept);
			}
		}

		#endregion

		#region Action Handler Methods

		private void StartModalityProcedureSteps()
		{
			try
			{
				var checkedItems = ListCheckedSummmaryTableItems();
				var firstItem = CollectionUtils.FirstElement(checkedItems);

				if (CollectionUtils.Contains(
					checkedItems,
					item => item.ModalityProcedureStep.Modality.Id != firstItem.ModalityProcedureStep.Modality.Id))
				{
					this.Host.ShowMessageBox(SR.MessageCannotStartDifferentModalityProcedureStepsAtSameTime,
											 MessageBoxActions.Ok);
					return;
				}

				var checkedMpsRefs = CollectionUtils.Map<ProcedurePlanSummaryTableItem, EntityRef>(
					checkedItems,
					item => item.ModalityProcedureStep.ProcedureStepRef);

				if (checkedMpsRefs.Count > 0)
				{
					DateTime? startTime = Platform.Time;
					if (DowntimeRecovery.InDowntimeRecoveryMode)
					{
						if (!DateTimeEntryComponent.PromptForTime(this.Host.DesktopWindow, SR.TitleStartTime, false, ref startTime))
							return;
					}

					Platform.GetService<IModalityWorkflowService>(service =>
					{
						var request = new StartModalityProcedureStepsRequest(checkedMpsRefs)
						{
							StartTime = DowntimeRecovery.InDowntimeRecoveryMode ? startTime : null
						};
						var response = service.StartModalityProcedureSteps(request);

						RefreshProcedurePlanSummary(response.ProcedurePlan);
						UpdateActionEnablement();

						_ppsComponent.AddPerformedProcedureStep(response.StartedMpps);
					});
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		private void DiscontinueModalityProcedureSteps()
		{
			try
			{
				var checkedMpsRefs = CollectionUtils.Map<ProcedurePlanSummaryTableItem, EntityRef, List<EntityRef>>(
					ListCheckedSummmaryTableItems(),
					item => item.ModalityProcedureStep.ProcedureStepRef);

				if (checkedMpsRefs.Count > 0)
				{
					if (this.Host.DesktopWindow.ShowMessageBox(SR.MessageConfirmDiscontinueSelectedProcedures, MessageBoxActions.YesNo) != DialogBoxAction.No)
					{
						DateTime? discontinueTime = Platform.Time;
						if (DowntimeRecovery.InDowntimeRecoveryMode)
						{
							if (!DateTimeEntryComponent.PromptForTime(this.Host.DesktopWindow, SR.TitleCancelTime, false, ref discontinueTime))
								return;
						}

						Platform.GetService<IModalityWorkflowService>(service =>
						{
							var request = new DiscontinueModalityProcedureStepsRequest(checkedMpsRefs)
							{
								DiscontinuedTime = DowntimeRecovery.InDowntimeRecoveryMode ? discontinueTime : null
							};
							var response = service.DiscontinueModalityProcedureSteps(request);

							RefreshProcedurePlanSummary(response.ProcedurePlan);
							UpdateActionEnablement();
						});
					}
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		#endregion

		#region Private methods

		private bool Save(bool completeDocumentation)
		{
			// only do validation if they are completing the documentation, not if they are just saving a draft
			if (completeDocumentation)
			{
				if (this.HasValidationErrors)
				{
					ShowValidation(true);
					return false;
				}
			}

			try
			{
				// allow extension pages to save data
				var veto = false;
				foreach (var page in _extensionPages)
				{
					veto = veto || !page.Save(completeDocumentation);
				}
				if (veto)
					return false;

				_orderDetailsComponent.SaveData();
				_ppsComponent.SaveData();
				Platform.GetService<IModalityWorkflowService>(service =>
				{
					var saveRequest = new SaveOrderDocumentationDataRequest(
						_procedurePlan.OrderRef,
						_orderExtendedProperties,
						_orderNotes,
						new List<ModalityPerformedProcedureStepDetail>(_ppsComponent.PerformedProcedureSteps),
						_assignedRadiologist);

					var saveResponse = service.SaveOrderDocumentationData(saveRequest);

					if (completeDocumentation)
					{
						var completeRequest = new CompleteOrderDocumentationRequest(saveResponse.ProcedurePlan.OrderRef);
						var completeResponse = service.CompleteOrderDocumentation(completeRequest);

						RefreshProcedurePlanSummary(completeResponse.ProcedurePlan);
					}
					else
					{
						RefreshProcedurePlanSummary(saveResponse.ProcedurePlan);
					}
				});

				return true;
			}
			catch (FaultException<ConcurrentModificationException>)
			{
				// bug #3469: we handle this exception explicitly, so we can instruct the user to close the workspace
				this.Host.ShowMessageBox(SR.MessageConcurrentModificationCloseWorkspace, MessageBoxActions.Ok);

				_saveEnabled = false;
				NotifyPropertyChanged("SaveEnabled");

				_completeEnabled = false;
				NotifyPropertyChanged("CompleteEnabled");
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}

			return false;
		}

		private void InitializeProcedurePlanSummary()
		{
			_procedurePlanSummaryTable = new ProcedurePlanSummaryTable();
			_procedurePlanSummaryTable.CheckedRowsChanged += ((sender, args) => UpdateActionEnablement());

			Platform.GetService<IModalityWorkflowService>(service =>
			{
				var procedurePlanRequest = new GetProcedurePlanRequest(_worklistItem.OrderRef);
				var procedurePlanResponse = service.GetProcedurePlan(procedurePlanRequest);
				_procedurePlan = procedurePlanResponse.ProcedurePlan;
			});

			RefreshProcedurePlanSummary(_procedurePlan);

			Platform.GetService<IModalityWorkflowService>(service =>
			{
				var response = service.LoadOrderDocumentationData(new LoadOrderDocumentationDataRequest(_worklistItem.OrderRef));
				_orderExtendedProperties = response.OrderExtendedProperties;
				_orderNotes = response.OrderNotes;
				this.AssignedRadiologist = response.AssignedInterpreter;
			});

			InitializeProcedurePlanSummaryActionHandlers();
		}

		private void InitializeProcedurePlanSummaryActionHandlers()
		{
			_procedurePlanActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
			_startAction = _procedurePlanActionHandler.AddAction("start", SR.TitleStartMps, "Icons.StartToolSmall.png", SR.TitleStartMps, StartModalityProcedureSteps);
			_discontinueAction = _procedurePlanActionHandler.AddAction("discontinue", SR.TitleDiscontinueMps, "Icons.DeleteToolSmall.png", SR.TitleDiscontinueMps, DiscontinueModalityProcedureSteps);
			UpdateActionEnablement();
		}

		private void InitializeDocumentationTabPages()
		{
			var context = new PerformingDocumentationContext(this);

			_bannerComponentHost = new ChildComponentHost(this.Host, new BannerComponent(_worklistItem));
			_bannerComponentHost.StartComponent();

			_documentationTabContainer = new TabComponentContainer { ValidationStrategy = new AllComponentsValidationStrategy() };

			_orderDetailsComponent = new PerformingDocumentationOrderDetailsComponent(context, _worklistItem);
			_documentationTabContainer.Pages.Add(new TabPage("Order", _orderDetailsComponent));

			_ppsComponent = new PerformedProcedureComponent(_worklistItem, this);
			_ppsComponent.ProcedurePlanChanged += ((sender, e) => RefreshProcedurePlanSummary(e.ProcedurePlanDetail));
			_documentationTabContainer.Pages.Add(new TabPage("Exam", _ppsComponent));


			// create extension pages
			foreach (IPerformingDocumentationPageProvider pageProvider in (new PerformingDocumentationPageProviderExtensionPoint()).CreateExtensions())
			{
				_extensionPages.AddRange(pageProvider.GetPages(context));
			}

			foreach (var page in _extensionPages)
			{
				_documentationTabContainer.Pages.Add(new TabPage(page.Path, page.GetComponent()));
			}

			// For #7401.  Al the pages in this component are explicitly started so data for validation
			// iill be readily available when user complete the documentation.
			CollectionUtils.ForEach(_documentationTabContainer.Pages, p => p.LazyStart = false );

			_documentationHost = new ChildComponentHost(this.Host, _documentationTabContainer);
			_documentationHost.StartComponent();

			SetInitialDocumentationTabPage();
		}

		private void SetInitialDocumentationTabPage()
		{
			var selectedTabName = PerformingDocumentationComponentSettings.Default.InitiallySelectedTabPageName;
			if (string.IsNullOrEmpty(selectedTabName))
				return;

			var requestedTabPage = CollectionUtils.SelectFirst(
				_documentationTabContainer.Pages,
				tabPage => tabPage.Name.Equals(selectedTabName, StringComparison.InvariantCultureIgnoreCase));

			if (requestedTabPage != null)
				_documentationTabContainer.CurrentPage = requestedTabPage;
		}

		private List<ProcedurePlanSummaryTableItem> ListCheckedSummmaryTableItems()
		{
			return CollectionUtils.Map<Checkable<ProcedurePlanSummaryTableItem>, ProcedurePlanSummaryTableItem>(
				CollectionUtils.Select(_procedurePlanSummaryTable.Items, checkable => checkable.IsChecked),
				checkable => checkable.Item);
		}

		private void UpdateActionEnablement()
		{
			var checkedSummaryTableItems = ListCheckedSummmaryTableItems();
			if (checkedSummaryTableItems.Count == 0)
			{
				_startAction.Enabled = _discontinueAction.Enabled = false;
			}
			else
			{
				_startAction.Enabled = CollectionUtils.TrueForAll(
					checkedSummaryTableItems,
					item => item.ModalityProcedureStep.State.Code == "SC");

				_discontinueAction.Enabled = CollectionUtils.TrueForAll(
					checkedSummaryTableItems,
					item => item.ModalityProcedureStep.State.Code == "SC");
			}
		}

		private void RefreshProcedurePlanSummary(ProcedurePlanDetail procedurePlanDetail)
		{
			_procedurePlan = procedurePlanDetail;

			try
			{
				Platform.GetService<IModalityWorkflowService>(service =>
				{
					var response = service.CanCompleteOrderDocumentation(
						new CanCompleteOrderDocumentationRequest(_procedurePlan.OrderRef));

					_completeEnabled = response.CanComplete;
					_alreadyCompleted = response.AlreadyCompleted;
					this.NotifyPropertyChanged("CompleteEnabled");
				});
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}

			_procedurePlanSummaryTable.Items.Clear();
			foreach (var rp in procedurePlanDetail.Procedures)
			{
				foreach (var mps in rp.ProcedureSteps)
				{
					_procedurePlanSummaryTable.Items.Add(
						new Checkable<ProcedurePlanSummaryTableItem>(
							new ProcedurePlanSummaryTableItem(rp, mps)));
				}
			}
			_procedurePlanSummaryTable.Sort();

			EventsHelper.Fire(_procedurePlanChanged, this, EventArgs.Empty);
		}

		#endregion
	}
}
