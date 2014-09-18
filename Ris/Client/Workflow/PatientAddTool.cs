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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using System.Threading;
using ClearCanvas.Ris.Application.Common;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuNewPatient", "Apply")]
	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuNewPatient", "Apply")]
	[ButtonAction("apply", "patientsearch-items-toolbar/MenuNewPatient", "Apply")]
	[MenuAction("apply", "patientsearch-items-contextmenu/MenuNewPatient", "Apply")]
	[Tooltip("apply", "TooltipNewPatient")]
	[IconSet("apply", "Icons.AddPatientToolSmall.png", "Icons.AddPatientToolMedium.png", "Icons.AddPatientToolLarge.png")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Patient.Create)]

	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(PatientSearchToolExtensionPoint))]
	public class PatientAddTool : Tool<IToolContext>
	{
		/// <summary>
		/// Called by the framework when the user clicks the "apply" menu item or toolbar button.
		/// </summary>
		public void Apply()
		{
			if (this.Context is IRegistrationWorkflowItemToolContext)
				Open(((IRegistrationWorkflowItemToolContext)this.Context).DesktopWindow);
			else if (this.Context is IPatientSearchToolContext)
				Open(((IPatientSearchToolContext)this.Context).DesktopWindow);
		}

		private void Open(IDesktopWindow desktopWindow)
		{
			try
			{
				var editor = new PatientProfileEditorComponent();
				var result = ApplicationComponent.LaunchAsDialog(
					desktopWindow,
					editor,
					SR.TitleNewPatient);

				if (result == ApplicationComponentExitCode.Accepted && this.Context is IRegistrationWorkflowItemToolContext)
				{
					// if patient successfully added, invoke a search on the MRN so that the patient appears in the Home page
					var searchParams = new WorklistSearchParams(new WorklistItemTextQueryRequest.AdvancedSearchFields()
																	{
																		Mrn = editor.PatientProfile.Mrn.Id
																	});
					((IRegistrationWorkflowItemToolContext)this.Context).ExecuteSearch(searchParams);
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, desktopWindow);
			}
		}
	}
}
