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
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ButtonAction("edit", "folderexplorer-items-toolbar/MenuEditPatient", "Apply")]
	[MenuAction("edit", "folderexplorer-items-contextmenu/MenuEditPatient", "Apply")]
	[ButtonAction("edit", "patientsearch-items-toolbar/MenuEditPatient", "Apply")]
	[MenuAction("edit", "patientsearch-items-contextmenu/MenuEditPatient", "Apply")]
    [EnabledStateObserver("edit", "Enabled", "EnabledChanged")]
	[Tooltip("edit", "TooltipEditPatient")]
    [IconSet("edit", "Icons.EditPatientToolSmall.png", "Icons.EditPatientToolMedium.png", "Icons.EditPatientToolLarge.png")]
    [ActionPermission("edit", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Patient.Update)]
	[ActionPermission("edit", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.PatientProfile.Update)]

    [ExtensionOf(typeof(PatientBiographyToolExtensionPoint))]
    [ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    [ExtensionOf(typeof(PatientSearchToolExtensionPoint))]
    public class PatientEditTool : ToolBase
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public override void Initialize()
        {
            base.Initialize();
            _enabled = false;   // disable by default

            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectionChanged += delegate
                {
                    this.Enabled = (((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems != null
                    && ((IRegistrationWorkflowItemToolContext)this.ContextBase).SelectedItems.Count == 1);
                };
            }
            else if (this.ContextBase is IPatientSearchToolContext)
            {
                ((IPatientSearchToolContext)this.ContextBase).SelectedProfileChanged += delegate
                {
                    IPatientSearchToolContext context = (IPatientSearchToolContext)this.ContextBase;
                    this.Enabled = (context.SelectedProfile != null && context.SelectedProfile.PatientProfileRef != null);
                };
            }
            else if (this.ContextBase is IPatientBiographyToolContext)
            {
                this.Enabled = true;
            }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }
        
        public void Apply()
        {
            if (this.ContextBase is IRegistrationWorkflowItemToolContext)
            {
                IRegistrationWorkflowItemToolContext context = (IRegistrationWorkflowItemToolContext)this.ContextBase;
                RegistrationWorklistItemSummary item = CollectionUtils.FirstElement(context.SelectedItems);
                if (Edit(item.PatientProfileRef, context.DesktopWindow))
                {
                    context.InvalidateSelectedFolder();
                }
            }
            else if (this.ContextBase is IPatientSearchToolContext)
            {
                IPatientSearchToolContext context = (IPatientSearchToolContext)this.ContextBase;
                Edit(context.SelectedProfile.PatientProfileRef, context.DesktopWindow);
            }
            else if (this.ContextBase is IPatientBiographyToolContext)
            {
                IPatientBiographyToolContext context = (IPatientBiographyToolContext)this.ContextBase;
                Edit(context.PatientProfileRef, context.DesktopWindow);
            }
        }

        private bool Edit(EntityRef profileRef, IDesktopWindow desktopWindow)
        {
            try
            {
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    desktopWindow,
                    new PatientProfileEditorComponent(profileRef),
                    SR.TitleEditPatient);

                return exitCode == ApplicationComponentExitCode.Accepted;

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, desktopWindow);
                return false;
            }
        }
    }
}
