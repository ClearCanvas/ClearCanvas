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
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuCompleteDowntimeRecovery", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuCompleteDowntimeRecovery", "Apply")]
	[IconSet("apply", "VerifyReportSmall.png", "VerifyReportMedium.png", "VerifyReportLarge.png")]
	[ActionPermission("apply", Application.Common.AuthorityTokens.Workflow.Downtime.RecoveryOperations)]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("apply", "Visible", "VisibleChanged")]
	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	public class DowntimeReportEntryTool : WorkflowItemTool<ModalityWorklistItemSummary, IPerformingWorkflowItemToolContext>
    {
    	public DowntimeReportEntryTool()
			: base("CompleteDowntimeProcedure")
    	{
    	}

    	public override void Initialize()
		{
			base.Initialize();

    		DowntimeRecovery.InDowntimeRecoveryModeChanged += OnInDowntimeRecoveryModeChanged;

			// init 
    		UpdateReportingWorkflowServiceRegistration();
		}

    	public bool Visible
    	{
			get { return DowntimeRecovery.InDowntimeRecoveryMode; }
    	}

		public event EventHandler VisibleChanged
		{
			add { }
			remove { }
		}

		public override bool Enabled
		{
			get
			{
				return DowntimeRecovery.InDowntimeRecoveryMode // bug #5616: don't check base enablement unless we are actually in downtime mode
					&& base.Enabled;
			}
		}
		
    	protected override bool Execute(ModalityWorklistItemSummary item)
    	{
			if (item.ProcedureRef == null)
				return false;

			var exitCode = ApplicationComponent.LaunchAsDialog(
				this.Context.DesktopWindow,
				new DowntimeReportEntryComponent(item.ProcedureRef),
				SR.TitleCompleteDowntimeRecovery);

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				DocumentManager.InvalidateFolder(typeof(Folders.Performing.PerformedFolder));
				return true;
			}
    		return false;
		}

		protected override void Dispose(bool disposing)
		{
			DowntimeRecovery.InDowntimeRecoveryModeChanged -= OnInDowntimeRecoveryModeChanged;

			base.Dispose(disposing);
		}

		private void UpdateReportingWorkflowServiceRegistration()
		{
			if (DowntimeRecovery.InDowntimeRecoveryMode)
			{
				// bug  #4866: in downtime mode register reporting service so we can get operation enablement
				this.Context.RegisterWorkflowService(typeof(IReportingWorkflowService));
			}
			else
			{
				// otherwise unregister reporting service, so we don't degrade performance for no reason
				this.Context.UnregisterWorkflowService(typeof(IReportingWorkflowService));
			}
		}

		private void OnInDowntimeRecoveryModeChanged(object sender, EventArgs e)
		{
			UpdateReportingWorkflowServiceRegistration();
		}
	}




	/// <summary>
	/// Extension point for views onto <see cref="DowntimeReportEntryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class DowntimeReportEntryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DowntimeReportEntryComponent class
	/// </summary>
	[AssociateView(typeof(DowntimeReportEntryComponentViewExtensionPoint))]
	public class DowntimeReportEntryComponent : ApplicationComponent
	{
		private readonly EntityRef _procedureRef;
		private bool _hasReport;
		private string _reportText;
		private StaffSummary _interpreter;
		private StaffSummary _transcriptionist;

		private ILookupHandler _interpreterLookupHandler;
		private ILookupHandler _transcriptionistLookupHandler;



		/// <summary>
		/// Constructor
		/// </summary>
		public DowntimeReportEntryComponent(EntityRef procedureRef)
		{
			_procedureRef = procedureRef;
		}

		public override void Start()
		{
			// radiologist staff lookup handler, using filters provided by application configuration
			var radFilters = DowntimeSettings.Default.ReportEntryRadiologistStaffTypeFilters;
			var radStaffTypes = string.IsNullOrEmpty(radFilters) ? new string[] { } :
				CollectionUtils.Map<string, string>(radFilters.Split(','), s => s.Trim()).ToArray();
			_interpreterLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, radStaffTypes);

			// transcriptionist staff lookup handler, using filters provided by application configuration
			var transFilters = DowntimeSettings.Default.ReportEntryTranscriptionistStaffTypeFilters;
			var transStaffTypes = string.IsNullOrEmpty(transFilters) ? new string[] { } :
				CollectionUtils.Map<string, string>(transFilters.Split(','), s => s.Trim()).ToArray();
			_transcriptionistLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, transStaffTypes);

			base.Start();
		}

		#region Presentation Model


		public bool HasReport
		{
			get { return _hasReport; }
			set
			{
				_hasReport = value;

				// only need to validate if submitting a report
				this.ShowValidation(_hasReport && this.HasValidationErrors);
			}
		}

		[ValidateNotNull]
		public string ReportText
		{
			get { return _reportText; }
			set { _reportText = value; }
		}

		public ILookupHandler InterpreterLookupHandler
		{
			get { return _interpreterLookupHandler; }
		}

		public ILookupHandler TranscriptionistLookupHandler
		{
			get { return _transcriptionistLookupHandler; }
		}

		[ValidateNotNull]
		public StaffSummary Interpreter
		{
			get { return _interpreter; }
			set { _interpreter = value; }
		}

		public StaffSummary Transcriptionist
		{
			get { return _transcriptionist; }
			set { _transcriptionist = value; }
		}

		public void Accept()
		{
			// only need to validate if submitting a report
			if(_hasReport && this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			try
			{
				Platform.GetService<IReportingWorkflowService>(
					service =>
					{
						var reportData = new Dictionary<string, string>();
						if (_hasReport)
						{
							var content = new ReportContent(_reportText);
							reportData[ReportPartDetail.ReportContentKey] = content.ToJsml();
						}

						service.CompleteDowntimeProcedure(
							new CompleteDowntimeProcedureRequest(
								_procedureRef,
								_hasReport,
								reportData,
								_interpreter == null ? null : _interpreter.StaffRef,
								_transcriptionist == null ? null : _transcriptionist.StaffRef));
					});

				Exit(ApplicationComponentExitCode.Accepted);
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, "", this.Host.DesktopWindow, () => Exit(ApplicationComponentExitCode.Error));
			}
		}

		public void Cancel()
		{
			Exit(ApplicationComponentExitCode.None);
		}

		#endregion
	}
}
