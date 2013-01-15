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
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	public abstract class ExternalPractitionerFolder : WorkflowFolder<ExternalPractitionerSummary>
	{
		protected ExternalPractitionerFolder(Table<ExternalPractitionerSummary> table)
			: base(table)
		{
		}

		protected override QueryItemsResult QueryItems(int firstRow, int maxRows)
		{
			QueryItemsResult result = null;
			Platform.GetService(
				delegate(IExternalPractitionerAdminService service)
				{
					var request = new ListExternalPractitionersRequest { QueryItems = true, QueryCount = true, Page = new SearchResultPage(firstRow, maxRows) };
					PrepareQueryRequest(request);
					var response = service.ListExternalPractitioners(request);
					result = new QueryItemsResult(response.Practitioners, response.ItemCount);
				});

			return result;
		}

		protected override int QueryCount()
		{
			var count = -1;
			Platform.GetService(
				delegate(IExternalPractitionerAdminService service)
				{
					var request = new ListExternalPractitionersRequest { QueryItems = false, QueryCount = true };
					PrepareQueryRequest(request);
					var response = service.ListExternalPractitioners(request);
					count = response.ItemCount;
				});

			return count;
		}

		protected abstract void PrepareQueryRequest(ListExternalPractitionersRequest request);
	}
}