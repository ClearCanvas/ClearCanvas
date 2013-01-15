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

using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionPoint]
	public class BookingWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class BookingWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class BookingWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = Application.Extended.Common.AuthorityTokens.FolderSystems.Booking)]
	public class BookingWorkflowFolderSystem
		: RegistrationWorkflowFolderSystemBase<BookingWorkflowFolderExtensionPoint,
			BookingWorkflowFolderToolExtensionPoint, BookingWorkflowItemToolExtensionPoint>
	{
		public BookingWorkflowFolderSystem()
			: base(SR.TitleBookingFolderSystem)
		{
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<RegistrationWorklistItemSummary> items)
		{
			return WebResourcesSettings.Default.BookingFolderSystemUrl;
		}

		protected override PreviewOperationAuditData[] GetPreviewAuditData(WorkflowFolder folder, ICollection<RegistrationWorklistItemSummary> items)
		{
			return items.Select(item => new PreviewOperationAuditData("Booking", item)).ToArray();
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
        {
            return new Folders.Booking.BookingSearchFolder();
        }

	}
}