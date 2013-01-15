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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using System.Threading;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ExtensionPoint]
	public class RegistrationWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class RegistrationWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class RegistrationWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.FolderSystems.Registration)]
	public class RegistrationWorkflowFolderSystem
		: RegistrationWorkflowFolderSystemBase<RegistrationWorkflowFolderExtensionPoint, RegistrationWorkflowFolderToolExtensionPoint,
			RegistrationWorkflowItemToolExtensionPoint>
	{
		public RegistrationWorkflowFolderSystem()
			: base(SR.TitleRegistrationFolderSystem)
		{
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<RegistrationWorklistItemSummary> items)
		{
			return WebResourcesSettings.Default.RegistrationFolderSystemUrl;
		}

		protected override PreviewOperationAuditData[] GetPreviewAuditData(WorkflowFolder folder, ICollection<RegistrationWorklistItemSummary> items)
		{
			return items.Select(item => new PreviewOperationAuditData("Registration", item)).ToArray();
		}

        protected override SearchResultsFolder CreateSearchResultsFolder()
        {
            return new Folders.Registration.RegistrationSearchFolder();
        }
    }
}