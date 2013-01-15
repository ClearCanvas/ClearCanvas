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
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionOf(typeof(ExternalPractitionerFolderExtensionPoint))]
	[FolderPath("Unverfied")]
	[FolderDescription("ExternalPractitionerUnverifiedFolderDescription")]
	internal class UnverifiedFolder : ExternalPractitionerFolder
	{
		public UnverifiedFolder()
			: base(new ExternalPractitionerWorkflowTable { PropertyNameForTimeColumn = "LastEditedTime", SortAscending = true})
		{
		}

		protected override void PrepareQueryRequest(ListExternalPractitionersRequest request)
		{
			request.VerifiedState = VerifiedState.NotVerified;
			request.SortByLastEditedTime = true;
			request.SortAscending = true;
		}
	}

	[ExtensionOf(typeof(ExternalPractitionerFolderExtensionPoint))]
	[FolderPath("Verfied Today")]
	[FolderDescription("ExternalPractitionerVerifiedTodayFolderDescription")]
	internal class VerifiedTodayFolder : ExternalPractitionerFolder
	{
		public VerifiedTodayFolder()
			: base(new ExternalPractitionerWorkflowTable { PropertyNameForTimeColumn = "LastVerifiedTime", SortAscending = false})
		{
		}

		protected override void PrepareQueryRequest(ListExternalPractitionersRequest request)
		{
			var today = Platform.Time;
			request.VerifiedState = VerifiedState.Verified;
			request.LastVerifiedRangeFrom = today.Date;
			request.LastVerifiedRangeUntil = today.Date.AddDays(1);
			request.SortByLastVerifiedTime = true;
			request.SortAscending = false;
		}
	}

	[FolderPath("Search Results")]
	public class ExternalPractitionerSearchFolder : SearchResultsFolder<ExternalPractitionerSummary, ExternalPractitionerSearchParams>
	{
		public ExternalPractitionerSearchFolder()
			: base(new ExternalPractitionerWorkflowTable())
		{
		}

		protected override TextQueryResponse<ExternalPractitionerSummary> DoQuery(ExternalPractitionerSearchParams query, int specificityThreshold)
		{
			TextQueryResponse<ExternalPractitionerSummary> response = null;

			Platform.GetService(
				delegate(IExternalPractitionerAdminService service)
				{
					var request = new TextQueryRequest {TextQuery = query.TextSearch, SpecificityThreshold = specificityThreshold};
					response = service.TextQuery(request);
				});

			return response;
		}
	}

}
