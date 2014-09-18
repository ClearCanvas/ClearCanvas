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

#if DEBUG	// This tool is only used during development

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Client.Workflow
{


	[MenuAction("apply", "folderexplorer-items-contextmenu/MenuRandomOrder", "RandomOrder")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/MenuRandomOrder", "RandomOrder")]
	[Tooltip("apply", "TooltipRandomOrder")]
	[IconSet("apply", "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", Application.Common.AuthorityTokens.Development.CreateTestOrder)]
	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	public class RandomOrderTool : Tool<IToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		public RandomOrderTool()
		{
			_enabled = true;
		}

		public bool Enabled
		{
			get { return _enabled; }
			protected set
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

		public void RandomOrder()
		{
			var context = (IRegistrationWorkflowItemToolContext)this.ContextBase;

			try
			{
				var item = CollectionUtils.FirstElement(context.SelectedItems);
				if (item == null)
				{
					var profile = GetRandomPatient() ?? RandomUtils.CreatePatient();
					PlaceRandomOrderForPatient(profile, profile.Mrn.AssigningAuthority);
				}
				else
				{
					PlaceRandomOrderForPatient(new PatientProfileSummary {PatientProfileRef = item.PatientProfileRef, PatientRef = item.PatientRef}, item.Mrn.AssigningAuthority);
				}

				// invalidate the scheduled worklist folders
				context.InvalidateFolders(typeof(Folders.Registration.ScheduledFolder));
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, context.DesktopWindow);
			}
		}

		private static void PlaceRandomOrderForPatient(PatientProfileSummary patientProfile, EnumValueInfo informationAuthority)
		{
			// find a random active visit, or create one
			var randomVisit = GetActiveVisitForPatient(patientProfile.PatientRef, informationAuthority) ??
				RandomUtils.CreateVisit(patientProfile, informationAuthority, 0);

			// create the order
			RandomUtils.RandomOrder(randomVisit, informationAuthority, null, 0);
		}

		private static PatientProfileSummary GetRandomPatient()
		{
			var queryString = string.Format("{0}, {1}", RandomUtils.GetRandomAlphaChar(), RandomUtils.GetRandomAlphaChar());

			PatientProfileSummary randomProfile = null;

			Platform.GetService(
				delegate(IRegistrationWorkflowService service)
				{
					// Get only 10 patients
					var request = new TextQueryRequest { TextQuery = queryString, Page = new SearchResultPage(0, 10)};
					var response = service.PatientProfileTextQuery(request);
					randomProfile = RandomUtils.ChooseRandom(response.Matches);
				});

			return randomProfile;
		}

		private static VisitSummary GetActiveVisitForPatient(EntityRef patientRef, EnumValueInfo assigningAuthority)
		{
			VisitSummary visit = null;

			// choose from existing visits
			Platform.GetService(
				delegate(IOrderEntryService service)
				{
					var request = new ListVisitsForPatientRequest(patientRef);

					var visitResponse = service.ListVisitsForPatient(request);
					visit = RandomUtils.ChooseRandom(CollectionUtils.Select(visitResponse.Visits,
						summary => Equals(summary.VisitNumber.AssigningAuthority, assigningAuthority)));
				});

			return visit;
		}

	}
}
#endif
