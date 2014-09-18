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
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public class Performing
	{
		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingScheduledWorklist)]
		[FolderPath("FolderScheduled")]
		[FolderDescription("PerformingScheduledFolderDescription")]
		public class ScheduledFolder : PerformingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingCheckedInWorklist)]
		[FolderPath("FolderCheckedIn", true)]
		[FolderDescription("PerformingCheckedInFolderDescription")]
		public class CheckedInFolder : PerformingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingInProgressWorklist)]
		[FolderPath("FolderInProgress")]
		[FolderDescription("PerformingInProgressFolderDescription")]
		public class InProgressFolder : PerformingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingPerformedWorklist)]
		[FolderPath("FolderPerformed")]
		[FolderDescription("PerformingPerformedFolderDescription")]
		public class PerformedFolder : PerformingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingUndocumentedWorklist)]
		[FolderPath("FolderIncompleteDocumentation")]
		[FolderDescription("PerformingUndocumentedFolderDescription")]
		public class UndocumentedFolder : PerformingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingCancelledWorklist)]
		[FolderPath("FolderCanceled")]
		[FolderDescription("PerformingCancelledFolderDescription")]
		public class CancelledFolder : PerformingWorkflowFolder
		{
		}

		[FolderPath("FolderSearchResults")]
		public class SearchFolder : WorklistSearchResultsFolder<ModalityWorklistItemSummary, IModalityWorkflowService>
		{
			public SearchFolder()
				: base(new PerformingWorklistTable())
			{
			}

			//TODO: (JR may 2008) having the client specify the class name isn't a terribly good idea, but
			//it is the only way to get things working right now
			protected override string ProcedureStepClassName
			{
				get { return "ModalityProcedureStep"; }
			}
		}
	}
}
