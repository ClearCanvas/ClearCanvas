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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ButtonAction("view", "folderexplorer-items-toolbar/MenuPatientBiography", "View")]
	[MenuAction("view", "folderexplorer-items-contextmenu/MenuPatientBiography", "View")]
	[ButtonAction("view", "patientsearch-items-toolbar/MenuPatientBiography", "View")]
	[MenuAction("view", "patientsearch-items-contextmenu/MenuPatientBiography", "View")]
	[EnabledStateObserver("view", "Enabled", "EnabledChanged")]
	[Tooltip("view", "TooltipPatientBiography")]
	[IconSet("view", "PatientDetailsToolSmall.png", "PatientDetailsToolMedium.png", "PatientDetailsToolLarge.png")]
	[ActionPermission("view", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.PatientBiography.View)]
	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(PatientSearchToolExtensionPoint))]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
	public class PatientBiographyTool : Tool<IToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;
		
		public override void Initialize()
		{
			base.Initialize();

			if (this.ContextBase is IWorkflowItemToolContext)
			{
				((IWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
			else if (this.ContextBase is IPatientSearchToolContext)
			{
				((IPatientSearchToolContext)this.ContextBase).SelectedProfileChanged += delegate
				{
					this.Enabled = DetermineEnablement();
				};
			}
		}

		private bool DetermineEnablement()
		{
			if (this.ContextBase is IWorkflowItemToolContext)
			{
				var ctx = (IWorkflowItemToolContext) this.ContextBase;
				return ctx.Selection != null && ctx.Selection.Items.Length == 1;
			}
			else if (this.ContextBase is IPatientSearchToolContext)
			{
				var context = (IPatientSearchToolContext)this.ContextBase;
				return (context.SelectedProfile != null && context.SelectedProfile.PatientProfileRef != null);
			}

			return false;
		}

		public bool Enabled
		{
			get
			{
				this.Enabled = DetermineEnablement();
				return _enabled;
			}
			set
			{
				if (_enabled == value)
					return;

				_enabled = value;
				EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public void View()
		{
			if(this.ContextBase is IWorkflowItemToolContext)
			{
				var context = (IWorkflowItemToolContext)ContextBase;
				var item = (WorklistItemSummaryBase) context.Selection.Item;
				OpenPatient(item, context.DesktopWindow);
			}
			else if (this.ContextBase is IPatientSearchToolContext)
			{
				var context = (IPatientSearchToolContext)this.ContextBase;
				OpenPatient(context.SelectedProfile, context.DesktopWindow);
			}
		}

		protected static void OpenPatient(WorklistItemSummaryBase worklistItem, IDesktopWindow window)
		{
			try
			{
				var document = DocumentManager.Get<PatientBiographyDocument>(worklistItem.PatientRef);
				if (document == null)
				{
					document = new PatientBiographyDocument(worklistItem, window);
					document.Open();
				}
				else
				{
					document.Open();
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, window);
			}
		}

		protected static void OpenPatient(PatientProfileSummary patientProfile, IDesktopWindow window)
		{
			try
			{
				var document = DocumentManager.Get<PatientBiographyDocument>(patientProfile.PatientRef);
				if (document == null)
				{
					document = new PatientBiographyDocument(patientProfile, window);
					document.Open();
				}
				else
				{
					document.Open();
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, window);
			}
		}
	}
}
