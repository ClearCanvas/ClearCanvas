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
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public class Transcription
	{
		[ExtensionOf(typeof(TranscriptionWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.TranscriptionToBeTranscribedWorklist)]
		[FolderPath("FolderToBeTranscribed", true)]
		[FolderDescription("TranscriptionToBeTranscribedFolderDescription")]
		public class ToBeTranscribedFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionDraftWorklist)]
		[FolderPath("FolderMyItems/FolderDraft")]
		[FolderDescription("TranscriptionDraftFolderDescription")]
		public class DraftFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionToBeReviewedWorklist)]
		[FolderPath("FolderMyItems/FolderToBeReviewed")]
		[FolderDescription("TranscriptionToBeReviewedFolderDescription")]
		public class ToBeReviewedFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionAwaitingReviewWorklist)]
		[FolderPath("FolderMyItems/FolderAwaitingReview")]
		[FolderDescription("TranscriptionAwaitingReviewFolderDescription")]
		public class AwaitingReviewFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionCompletedWorklist)]
		[FolderPath("FolderMyItems/FolderCompleted")]
		[FolderDescription("TranscriptionCompletedFolderDescription")]
		public class CompletedFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderPath("FolderSearchResults")]
		public class TranscriptionSearchFolder : WorklistSearchResultsFolder<ReportingWorklistItemSummary, ITranscriptionWorkflowService>
		{
			public TranscriptionSearchFolder()
				: base(new ReportingWorklistTable())
			{
			}

			//TODO: (JR may 2008) having the client specify the class name isn't a terribly good idea, but
			//it is the only way to get things working right now
			protected override string ProcedureStepClassName
			{
				get { return "ReportingProcedureStep"; }
			}

		}
	}
}