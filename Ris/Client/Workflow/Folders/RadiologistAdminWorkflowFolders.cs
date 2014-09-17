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
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public abstract class RadiologistAdmin
	{
		[ExtensionOf(typeof(RadiologistAdminWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingAdminUnreportedWorklist)]
		[FolderPath("FolderUnreportedItems", true)]
		[FolderDescription("ReportingAdminUnreportedFolderDescription")]
		public class ReportingAdminUnreportedFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RadiologistAdminWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingAdminAssignedWorklist)]
		[FolderPath("FolderActiveReportingItems", true)]
		[FolderDescription("ReportingAdminAssignedFolderDescription")]
		public class ReportingAdminAssignedFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RadiologistAdminWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingAdminToBeTranscribedWorklist)]
		[FolderPath("FolderToBeTranscribedItems", true)]
		[FolderDescription("ReportingAdminToBeTranscribedWorklistDescription")]
		public class ReportingAdminToBeTranscribedWorklist : ReportingWorkflowFolder
		{
		}

		[FolderPath("FolderSearchResults")]
		public class RadiologistAdminSearchFolder : WorklistSearchResultsFolder<ReportingWorklistItemSummary, IReportingWorkflowService>
		{
			public RadiologistAdminSearchFolder()
				: base(new ReportingWorklistTable())
			{
			}

			protected override string ProcedureStepClassName
			{
				//TODO: having the client specify the class name isn't a terribly good idea, but
				//it is the only way to get things working right now
				//This class uses two different ProcedureStepClassNames for query.  So this property is actually not used.
				get { return "ReportingProcedureStep and ProtocolAssignmentStep"; }
			}

			protected override TextQueryResponse<ReportingWorklistItemSummary> DoQuery(WorklistSearchParams query, int specificityThreshold)
			{
				TextQueryResponse<ReportingWorklistItemSummary> response;

				WorklistItemTextQueryOptions options = WorklistItemTextQueryOptions.ProcedureStepStaff
					| (DowntimeRecovery.InDowntimeRecoveryMode ? WorklistItemTextQueryOptions.DowntimeRecovery : 0);

				response = DoQueryCore(query, specificityThreshold, options, "ReportingProcedureStep");
				if (response.TooManyMatches)
					return response;

				List<ReportingWorklistItemSummary> storeMatches = new List<ReportingWorklistItemSummary>(response.Matches);
				response = DoQueryCore(query, specificityThreshold, options, "ProtocolAssignmentStep");

				if (!response.TooManyMatches)
					response.Matches.AddRange(storeMatches);

				return response;
			}
		}
	}
}
