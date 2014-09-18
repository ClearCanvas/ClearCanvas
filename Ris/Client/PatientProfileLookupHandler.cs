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

using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client
{
	public class PatientProfileLookupHandler : LookupHandler<TextQueryRequest, PatientProfileSummary>
	{
		private readonly DesktopWindow _desktopWindow;

		public PatientProfileLookupHandler(DesktopWindow desktopWindow)
			: base(PatientProfileLookupSettings.Default.MinQueryStringLength, PatientProfileLookupSettings.Default.QuerySpecificityThreshold)
		{
			_desktopWindow = desktopWindow;
		}

		protected override TextQueryResponse<PatientProfileSummary> DoQuery(TextQueryRequest request)
		{
			TextQueryResponse<PatientProfileSummary> response = null;

			Platform.GetService(
				delegate(IRegistrationWorkflowService service)
				{
					response = service.PatientProfileTextQuery(request);
				});

			return response;
		}

		public override bool ResolveNameInteractive(string query, out PatientProfileSummary result)
		{
			result = null;

			var summaryComponent = new PatientProfileSummaryComponent(true);
			if (!string.IsNullOrEmpty(query))
			{
				summaryComponent.SearchString = query;
			}

			var exitCode = ApplicationComponent.LaunchAsDialog(_desktopWindow, summaryComponent, SR.TitlePatients);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				result = (PatientProfileSummary)summaryComponent.SummarySelection.Item;
			}

			return (result != null);
		}


		public override string FormatItem(PatientProfileSummary item)
		{
			return item == null ? null : 
				string.Format("{0} {1}", item.Mrn.Id, Formatting.PersonNameFormat.Format(item.Name));
		}
	}
}
