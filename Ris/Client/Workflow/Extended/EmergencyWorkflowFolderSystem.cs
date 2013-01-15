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
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionPoint]
	public class EmergencyWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class EmergencyWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class EmergencyWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = Application.Extended.Common.AuthorityTokens.FolderSystems.Emergency)]
	public class EmergencyWorkflowFolderSystem
		: RegistrationWorkflowFolderSystemBase<EmergencyWorkflowFolderExtensionPoint, EmergencyWorkflowFolderToolExtensionPoint,
			EmergencyWorkflowItemToolExtensionPoint>
	{
		public EmergencyWorkflowFolderSystem()
			: base(SR.TitleEmergencyFolderSystem)
		{
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<RegistrationWorklistItemSummary> items)
		{
			return WebResourcesSettings.Default.EmergencyFolderSystemUrl;
		}

		protected override PreviewOperationAuditData[] GetPreviewAuditData(WorkflowFolder folder, ICollection<RegistrationWorklistItemSummary> items)
		{
			return items.Select(item => new PreviewOperationAuditData("Emergency", item)).ToArray();
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
        {
            return new Workflow.Folders.Registration.RegistrationSearchFolder();
        }
    }
}