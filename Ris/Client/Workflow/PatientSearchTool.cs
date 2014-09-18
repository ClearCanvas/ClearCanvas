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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("launch", "folderexplorer-items-contextmenu/MenuPatientSearch", "Launch")]
	[IconSet("launch", "Icons.SearchPatientToolSmall.png", "Icons.SearchPatientToolMedium.png", "Icons.SearchPatientToolLarge.png")]
	[Tooltip("launch", "TooltipPatientSearch")]

	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	public class PatientSearchTool : Tool<IRegistrationWorkflowItemToolContext>
	{
		class PreviewComponent : DHtmlComponent
		{
			private PatientProfileSummary _patientProfile;

			public PatientProfileSummary PatientProfile
			{
				get { return _patientProfile; }
				set
				{
					_patientProfile = value;
					this.SetUrl(WebResourcesSettings.Default.RegistrationFolderSystemUrl);
				}
			}

			protected override ActionModelNode GetActionModel()
			{
				return new ActionModelRoot();
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _patientProfile;
			}

		}


		private IWorkspace _workspace;

		public void Launch()
		{
			try
			{
				if (_workspace == null)
				{
					_workspace = ApplicationComponent.LaunchAsWorkspace(
						this.Context.DesktopWindow,
						BuildComponent(),
						SR.TitlePatientSearch);
					_workspace.Closed += delegate { _workspace = null; };
				}
				else
				{
					_workspace.Activate();
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		private static IApplicationComponent BuildComponent()
		{
			PatientSearchComponent searchComponent = new PatientSearchComponent();
			PreviewComponent previewComponent = new PreviewComponent();

			searchComponent.SelectedProfileChanged += delegate
			{
				previewComponent.PatientProfile = (PatientProfileSummary)searchComponent.SelectedProfile.Item;
			};

			SplitComponentContainer splitComponent = new SplitComponentContainer(SplitOrientation.Vertical);
			splitComponent.Pane1 = new SplitPane("Search", searchComponent, 1.0f);
			splitComponent.Pane2 = new SplitPane("Preview", previewComponent, 1.0f);

			return splitComponent;
		}
	}
}
