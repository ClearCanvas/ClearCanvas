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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin;

namespace ClearCanvas.Ris.Client
{
	public class ProcedureTypeLookupHandler : LookupHandler<TextQueryRequest, ProcedureTypeSummary>
	{
		private readonly DesktopWindow _desktopWindow;

		public ProcedureTypeLookupHandler(DesktopWindow desktopWindow)
			: base(ProcedureTypeLookupSettings.Default.MinQueryStringLength, ProcedureTypeLookupSettings.Default.QuerySpecificityThreshold)
		{
			_desktopWindow = desktopWindow;
		}

		protected override TextQueryResponse<ProcedureTypeSummary> DoQuery(TextQueryRequest request)
		{
			TextQueryResponse<ProcedureTypeSummary> response = null;
			Platform.GetService<IProcedureTypeAdminService>(
				service => response = service.TextQuery(request));
			return response;
		}

		public override bool ResolveNameInteractive(string query, out ProcedureTypeSummary result)
		{
			result = null;

			var summaryComponent = new ProcedureTypeSummaryComponent(true) { IncludeDeactivatedItems = this.IncludeDeactivatedItems };
			if (!string.IsNullOrEmpty(query))
			{
				summaryComponent.Name = query;
			}

			var exitCode = ApplicationComponent.LaunchAsDialog(_desktopWindow, summaryComponent, SR.TitleProcedureTypes);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				result = (ProcedureTypeSummary)summaryComponent.SummarySelection.Item;
			}

			return (result != null);
		}


		public override string FormatItem(ProcedureTypeSummary item)
		{
			return string.Format("{0} ({1})", item.Name, item.Id);
		}
	}
}
