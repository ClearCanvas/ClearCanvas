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
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ExtensionPoint]
	public class TranscriptionWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class TranscriptionWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class TranscriptionWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.FolderSystems.Transcription)]
	public class TranscriptionWorkflowFolderSystem
		: ReportingWorkflowFolderSystemBase<TranscriptionWorkflowFolderExtensionPoint, TranscriptionWorkflowFolderToolExtensionPoint,
			TranscriptionWorkflowItemToolExtensionPoint>
	{
		private readonly WorkflowConfigurationReader _workflowConfiguration;

		public TranscriptionWorkflowFolderSystem()
			: base(SR.TitleTranscriptionFolderSystem)
		{
			_workflowConfiguration = new WorkflowConfigurationReader();
			if(!_workflowConfiguration.EnableTranscriptionWorkflow)
				throw new NotSupportedException("Transcription workflow has been disabled in the system configuration.");
		}

		protected override void AddDefaultFolders()
		{

			if (_workflowConfiguration.EnableTranscriptionReviewWorkflow)
			{
				this.Folders.Add(new Folders.Transcription.ToBeReviewedFolder());
			}

			this.Folders.Add(new Folders.Transcription.DraftFolder());

			if (_workflowConfiguration.EnableTranscriptionReviewWorkflow &&
				Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Transcription.SubmitForReview))
				this.Folders.Add(new Folders.Transcription.AwaitingReviewFolder());

			this.Folders.Add(new Folders.Transcription.CompletedFolder());
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ReportingWorklistItemSummary> items)
		{
			return WebResourcesSettings.Default.TranscriptionFolderSystemUrl;
		}

		protected override PreviewOperationAuditData[] GetPreviewAuditData(WorkflowFolder folder, ICollection<ReportingWorklistItemSummary> items)
		{
			return items.Select(item => new PreviewOperationAuditData("Transcription", item)).ToArray();
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
		{
			return new Folders.Transcription.TranscriptionSearchFolder();
		}
	}
}