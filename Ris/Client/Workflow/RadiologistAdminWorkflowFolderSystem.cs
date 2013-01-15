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
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ExtensionPoint]
	public class RadiologistAdminWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class RadiologistAdminWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class RadiologistAdminWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}


	/// <summary>
	/// A folder system that allowing radiologist admin to monitor personal items and reassign to another radiologist
	/// </summary>
	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.FolderSystems.RadiologistAdmin)]
	public class RadiologistAdminWorkflowFolderSystem
		: ReportingWorkflowFolderSystemBase<RadiologistAdminWorkflowFolderExtensionPoint, RadiologistAdminWorkflowFolderToolExtensionPoint,
			RadiologistAdminWorkflowItemToolExtensionPoint>
	{
		public RadiologistAdminWorkflowFolderSystem()
			: base(SR.TitleRadiologistAdminFolderSystem)
		{
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ReportingWorklistItemSummary> items)
		{
			if (items.Count != 1)
				return null;

			var reportingWorklistItem = CollectionUtils.FirstElement(items);

			//TODO: having the client specify the step name name may not be a terribly good idea
			switch (reportingWorklistItem.ProcedureStepName)
			{
				case "Interpretation":
				case "Transcription":
				case "Transcription Review":
				case "Verification":
				case "Publication":
					return WebResourcesSettings.Default.ReportingFolderSystemUrl;
				case "Protocol Assignment":
				case "Protocol Resolution":
					return WebResourcesSettings.Default.ProtocollingFolderSystemUrl;
				default:
					return null;
			}
		}

		protected override PreviewOperationAuditData[] GetPreviewAuditData(WorkflowFolder folder, ICollection<ReportingWorklistItemSummary> items)
		{
			return items.Select(item => new PreviewOperationAuditData("Radiologist Admin", item)).ToArray();
		}

		public override bool AdvancedSearchEnabled
		{
			get { return false; }
		}

		public override string SearchMessage
		{
			get { return SR.MessageRadiologistAdminSearchMessage; }
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
		{
			return new Folders.RadiologistAdmin.RadiologistAdminSearchFolder();
		}
	}
}
