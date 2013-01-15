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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="ReassignComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ReassignComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ReassignComponent class
	/// </summary>
	[AssociateView(typeof(ReassignComponentViewExtensionPoint))]
	public class ReassignComponent : ApplicationComponent
	{
		private readonly ReportingWorklistItemSummary _worklistItem;
		private StaffSummary _radiologist;
		private ILookupHandler _radiologistLookupHandler;

		public ReassignComponent(ReportingWorklistItemSummary item)
		{
			_worklistItem = item;
		}

		public override void Start()
		{
			// create supervisor lookup handler, using filters supplied in application settings
			string filters = ReportingSettings.Default.SupervisorStaffTypeFilters;
			string[] staffTypes = string.IsNullOrEmpty(filters)
				? new string[] { }
				: CollectionUtils.Map<string, string>(filters.Split(','), delegate(string s) { return s.Trim(); }).ToArray();
			_radiologistLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, staffTypes);

			base.Start();
		}

		#region Presentation Model

		[ValidateNotNull]
		public StaffSummary Radiologist
		{
			get { return _radiologist; }
			set
			{
				if (!Equals(value, _radiologist))
				{
					_radiologist = value;
					NotifyPropertyChanged("Radiologist");
				}
			}
		}

		public ILookupHandler RadiologistLookupHandler
		{
			get { return _radiologistLookupHandler; }
		}

		public void Accept()
		{
			try
			{
				if (this.HasValidationErrors)
				{
					this.ShowValidation(true);
					return;
				}

				Platform.GetService<IReportingWorkflowService>(
					delegate(IReportingWorkflowService service)
						{
							service.ReassignProcedureStep(new ReassignProcedureStepRequest(
								_worklistItem.ProcedureStepRef, 
								_radiologist.StaffRef));
						});
				
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
				this.Exit(ApplicationComponentExitCode.Error);
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion


	}
}
