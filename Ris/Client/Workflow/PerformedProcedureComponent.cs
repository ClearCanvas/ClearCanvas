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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Defines an interface for providing custom editing pages to capture information about a performed procedure step.
	/// </summary>
	public interface IPerformedStepEditorPageProvider : IExtensionPageProvider<IPerformedStepEditorPage, IPerformedStepEditorContext>
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom editor page with access to the editor
	/// context.
	/// </summary>
	public interface IPerformedStepEditorContext
	{
		/// <summary>
		/// Gets a reference to the order.
		/// </summary>
		EntityRef OrderRef { get; }

		/// <summary>
		/// Gets a reference to the patient.
		/// </summary>
		EntityRef PatientRef { get; }

		/// <summary>
		/// Gets a reference to the patient profile.
		/// </summary>
		EntityRef PatientProfileRef { get; }

		/// <summary>
		/// Gets the currently selected performed procedure step.
		/// </summary>
		ModalityPerformedProcedureStepDetail SelectedPerformedStep { get; }

		/// <summary>
		/// Occurs when the <see cref="SelectedPerformedStep"/> property changes.
		/// </summary>
		event EventHandler SelectedPerformedStepChanged;

		/// <summary>
		/// Exposes the extended properties associated with the <see cref="SelectedPerformedStep"/>.  Modifications made to these
		/// properties by the editor page will be persisted whenever the editor is saved.
		/// </summary>
		IDictionary<string, string> SelectedPerformedStepExtendedProperties { get; }
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the performed procedure step editor.
	/// </summary>
	[ExtensionPoint]
	public class PerformedStepEditorPageProviderExtensionPoint : ExtensionPoint<IPerformedStepEditorPageProvider>
	{
	}

	/// <summary>
	/// Defines an interface to a custom performed procedure step editor page.
	/// </summary>
	public interface IPerformedStepEditorPage : IExtensionPage
	{
		void Save();
	}


	/// <summary>
	/// Extension point for views onto <see cref="PerformedProcedureComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class PerformedProcedureComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PerformedProcedureComponent class
	/// </summary>
	[AssociateView(typeof(PerformedProcedureComponentViewExtensionPoint))]
	public class PerformedProcedureComponent : ApplicationComponent
	{
		#region EditorContext

		class EditorContext : IPerformedStepEditorContext
		{
			private readonly PerformedProcedureComponent _owner;

			public EditorContext(PerformedProcedureComponent owner)
			{
				_owner = owner;
			}

			public EntityRef OrderRef
			{
				get { return _owner._worklistItem.OrderRef; }
			}

			public EntityRef PatientRef
			{
				get { return _owner._worklistItem.PatientRef; }
			}

			public EntityRef PatientProfileRef
			{
				get { return _owner._worklistItem.PatientProfileRef; }
			}

			public event EventHandler SelectedPerformedStepChanged
			{
				add { _owner._selectedMppsChanged += value; }
				remove { _owner._selectedMppsChanged -= value; }
			}

			public ModalityPerformedProcedureStepDetail SelectedPerformedStep
			{
				get { return _owner._selectedMpps; }
			}

			public IDictionary<string, string> SelectedPerformedStepExtendedProperties
			{
				get { return _owner._selectedMpps == null ? null : _owner._selectedMpps.ExtendedProperties; }
			}
		}

		#endregion

		private EntityRef _orderRef;
		private readonly WorklistItemSummaryBase _worklistItem;

		private readonly PerformingDocumentationMppsSummaryTable _mppsTable = new PerformingDocumentationMppsSummaryTable();
		private ModalityPerformedProcedureStepDetail _selectedMpps;
		private event EventHandler _selectedMppsChanged;

		private SimpleActionModel _mppsActionHandler;
		private ClickAction _stopAction;
		private ClickAction _discontinueAction;

		private ChildComponentHost _detailsPagesHost;

		private readonly List<IPerformedStepEditorPage> _editorPages = new List<IPerformedStepEditorPage>();

		private readonly PerformingDocumentationComponent _owner;

		private event EventHandler<ProcedurePlanChangedEventArgs> _procedurePlanChanged;

		/// <summary>
		/// Constructor
		/// </summary>
		public PerformedProcedureComponent(WorklistItemSummaryBase worklistItem, PerformingDocumentationComponent owner)
		{
			Platform.CheckForNullReference(worklistItem, "worklistItem");
			Platform.CheckForNullReference(owner, "owner");

			_worklistItem = worklistItem;
			_orderRef = worklistItem.OrderRef;
			_owner = owner;
		}

		internal void AddPerformedProcedureStep(ModalityPerformedProcedureStepDetail mpps)
		{
			_mppsTable.Items.Add(mpps);
			_mppsTable.Sort();
		}

		internal event EventHandler<ProcedurePlanChangedEventArgs> ProcedurePlanChanged
		{
			add { _procedurePlanChanged += value; }
			remove { _procedurePlanChanged -= value; }
		}

		internal void SaveData()
		{
			foreach (var page in _editorPages)
			{
				page.Save();
			}
		}

		internal IList<ModalityPerformedProcedureStepDetail> PerformedProcedureSteps
		{
			get { return _mppsTable.Items; }
		}

		#region ApplicationComponent overrides

		public override void Start()
		{
			var resolver = new ResourceResolver(this.GetType().Assembly);

			_mppsActionHandler = new SimpleActionModel(resolver);

			_stopAction = _mppsActionHandler.AddAction("stop", SR.TitleStopMpps, "Icons.CheckInToolSmall.png", SR.TitleStopMpps, StopPerformedProcedureStep);
			_discontinueAction = _mppsActionHandler.AddAction("discontinue", SR.TitleDiscontinueMpps, "Icons.DeleteToolSmall.png", SR.TitleDiscontinueMpps, DiscontinuePerformedProcedureStep);
			UpdateActionEnablement();

			if (_orderRef != null)
			{
				Platform.GetService<IModalityWorkflowService>(
					service =>
					{
						var mppsRequest = new ListPerformedProcedureStepsRequest(_orderRef);
						var mppsResponse = service.ListPerformedProcedureSteps(mppsRequest);

						_mppsTable.Items.AddRange(mppsResponse.PerformedProcedureSteps);
						_mppsTable.Sort();
					});
			}

			// create extension editor pages, if any exist
			foreach (IPerformedStepEditorPageProvider provider in (new PerformedStepEditorPageProviderExtensionPoint().CreateExtensions()))
			{
				_editorPages.AddRange(provider.GetPages(new EditorContext(this)));
			}

			// if no editor pages are available via extensions, create the default editor
			if (_editorPages.Count == 0)
			{
				_editorPages.Add(new MppsDocumentationComponent(new EditorContext(this)));

				if (PerformingDocumentationComponentSettings.Default.ShowDicomSeriesTab)
				{
					_editorPages.Add(new PerformedProcedureDicomSeriesComponent(new EditorContext(this)));
				}
			}


			// if there are multiple pages, need to create a tab container
			if (_editorPages.Count > 1)
			{
				var tabContainer = new TabComponentContainer();
				_detailsPagesHost = new ChildComponentHost(this.Host, tabContainer);
				foreach (var page in _editorPages)
				{
					tabContainer.Pages.Add(new TabPage(page.Path, page.GetComponent()));
				}
			}
			else
			{
				// don't create a tab container for just one page
				_detailsPagesHost = new ChildComponentHost(this.Host, _editorPages[0].GetComponent());
			}

			// start details pages host
			_detailsPagesHost.StartComponent();

			base.Start();
		}

		public override void Stop()
		{
			if (_detailsPagesHost != null)
			{
				_detailsPagesHost.StopComponent();
				_detailsPagesHost = null;
			}

			base.Stop();
		}

		public override bool HasValidationErrors
		{
			get
			{
				return _detailsPagesHost.Component.HasValidationErrors || base.HasValidationErrors;
			}
		}

		public override void ShowValidation(bool show)
		{
			_detailsPagesHost.Component.ShowValidation(show);
			base.ShowValidation(show);
		}

		#endregion

		#region Presentation Model

		public ITable ProcedurePlanSummaryTable
		{
			get { return _owner.ProcedurePlanSummaryTable; }
		}

		public ActionModelNode ProcedurePlanTreeActionModel
		{
			get { return _owner.ProcedurePlanTreeActionModel; }
		}

		public ITable MppsTable
		{
			get { return _mppsTable; }
		}

		public ISelection SelectedMpps
		{
			get { return new Selection(_selectedMpps); }
			set
			{
				var selectedMpps = (ModalityPerformedProcedureStepDetail)value.Item;
				if (selectedMpps != _selectedMpps)
				{
					OnSelectedMppsChanged(selectedMpps);
				}
			}
		}

		public ActionModelNode MppsTableActionModel
		{
			get { return _mppsActionHandler; }
		}

		public ApplicationComponentHost DetailsComponentHost
		{
			get { return _detailsPagesHost; }
		}

		private void StopPerformedProcedureStep()
		{
			// bail if no selected step (this shouldn't ever happen)
			if (_selectedMpps == null)
				return;

			// bail on validation errors
			if (this.HasValidationErrors)
			{
				ShowValidation(true);
				return;
			}

			// if downtime recovery mode, need to get the time from the user
			DateTime? endTime = _selectedMpps.StartTime;
			if (DowntimeRecovery.InDowntimeRecoveryMode)
			{
				if (!DateTimeEntryComponent.PromptForTime(this.Host.DesktopWindow, SR.TitleCompletedTime, false, ref endTime))
					return;
			}

			try
			{
				SaveData();

				CompleteModalityPerformedProcedureStepResponse response = null;
				Platform.GetService<IModalityWorkflowService>(
					service =>
					{
						var request = new CompleteModalityPerformedProcedureStepRequest(_selectedMpps)
										{
											CompletedTime = DowntimeRecovery.InDowntimeRecoveryMode ? endTime : null
										};
						response = service.CompleteModalityPerformedProcedureStep(request);
					});

				RefreshProcedurePlanTree(response.ProcedurePlan);

				_mppsTable.Items.Replace(
					mppsSummary => mppsSummary.ModalityPerformendProcedureStepRef.Equals(_selectedMpps.ModalityPerformendProcedureStepRef, true),
					response.StoppedMpps);

				// Refresh selection
				_selectedMpps = response.StoppedMpps;
				UpdateActionEnablement();
				_mppsTable.Sort();

				// notify pages that selection has been updated
				EventsHelper.Fire(_selectedMppsChanged, this, EventArgs.Empty);

				NotifyPerformedProcedureStepComplete();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		private void DiscontinuePerformedProcedureStep()
		{
			// bail if no selected step (this shouldn't ever happen)
			if (_selectedMpps == null)
				return;

			// confirm with user that they really want to do this
			if (this.Host.DesktopWindow.ShowMessageBox(SR.MessageConfirmDiscontinueSelectedProcedures,
				MessageBoxActions.YesNo) == DialogBoxAction.No)
				return;

			// if downtime recovery mode, need to get the time from the user
			DateTime? endTime = _selectedMpps.StartTime;
			if (DowntimeRecovery.InDowntimeRecoveryMode)
			{
				if (!DateTimeEntryComponent.PromptForTime(this.Host.DesktopWindow, SR.TitleDiscontinuedTime, false, ref endTime))
					return;
			}

			try
			{
				SaveData();

				DiscontinueModalityPerformedProcedureStepResponse response = null;
				Platform.GetService<IModalityWorkflowService>(
					service =>
					{
						var request = new DiscontinueModalityPerformedProcedureStepRequest(_selectedMpps)
											{
												DiscontinuedTime = DowntimeRecovery.InDowntimeRecoveryMode ? endTime : null
											};
						response = service.DiscontinueModalityPerformedProcedureStep(request);
					});

				RefreshProcedurePlanTree(response.ProcedurePlan);

				_mppsTable.Items.Replace(
					mpps => mpps.ModalityPerformendProcedureStepRef.Equals(_selectedMpps.ModalityPerformendProcedureStepRef, true),
					response.DiscontinuedMpps);

				// Refresh selection
				_selectedMpps = response.DiscontinuedMpps;
				UpdateActionEnablement();
				_mppsTable.Sort();

				// notify pages that selection has been updated
				EventsHelper.Fire(_selectedMppsChanged, this, EventArgs.Empty);

				NotifyPerformedProcedureStepComplete();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		#endregion

		#region Private Methods

		private void NotifyPerformedProcedureStepComplete()
		{
			var args = new WorkflowEventListener.PerformedProcedureStepCompletedArgs(
				this.Host.DesktopWindow,
				new WorkflowEventListener.PatientProfileInfo
					{
						FamilyName = _worklistItem.PatientName.FamilyName,
						GivenName = _worklistItem.PatientName.GivenName,
						Id = _worklistItem.Mrn.Id,
					},
				_worklistItem.AccessionNumber,
				_selectedMpps.ModalityProcedureSteps,
				(DateTime) _selectedMpps.EndTime);

			// need to contact the server to populate the DateOfBirth and Sex fields
			Platform.GetService<IBrowsePatientDataService>(service =>
			{
				var profileRequest = new GetPatientProfileDetailRequest { PatientProfileRef = _worklistItem.PatientProfileRef };
				var profile = service.GetData(new GetDataRequest { GetPatientProfileDetailRequest = profileRequest })
					.GetPatientProfileDetailResponse.PatientProfile;

				args.PatientProfile.Sex = profile.Sex.Value;
				args.PatientProfile.BirthDate = profile.DateOfBirth;
			});

			WorkflowEventPublisher.Instance.PerformedProcedureStepCompleted(args);
		}

		private void OnSelectedMppsChanged(ModalityPerformedProcedureStepDetail newSelection)
		{
			if (_selectedMpps != null)
			{
				// save changes to existing data first
				SaveData();
			}

			_selectedMpps = newSelection;

			UpdateActionEnablement();

			EventsHelper.Fire(_selectedMppsChanged, this, EventArgs.Empty);
		}

		private void UpdateActionEnablement()
		{
			if (_selectedMpps != null)
			{
				// TOOD:  replace with server side logic
				_stopAction.Enabled = _discontinueAction.Enabled = _selectedMpps.State.Code == "IP";
			}
			else
			{
				_stopAction.Enabled = _discontinueAction.Enabled = false;
			}
		}

		private void RefreshProcedurePlanTree(ProcedurePlanDetail procedurePlanDetail)
		{
			_orderRef = procedurePlanDetail.OrderRef;
			EventsHelper.Fire(_procedurePlanChanged, this, new ProcedurePlanChangedEventArgs(procedurePlanDetail));
		}

		#endregion
	}
}
