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

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class TranscriptionComponentWorklistItemManager : WorklistItemManager<ReportingWorklistItemSummary, ITranscriptionWorkflowService>
	{
		public TranscriptionComponentWorklistItemManager(string folderName, EntityRef worklistRef, string worklistClassName)
			: base(folderName, worklistRef, worklistClassName, TranscriptionSettings.Default.NextItemQueueCacheSize)
		{
		}

		protected override IContinuousWorkflowComponentMode GetMode(ReportingWorklistItemSummary worklistItem)
		{
			if (worklistItem == null)
				return TranscriptionComponentModes.Review;

			switch (worklistItem.ActivityStatus.Code)
			{
				case StepState.Scheduled:
					return TranscriptionComponentModes.Create;
				case StepState.InProgress:
					return TranscriptionComponentModes.Edit;
				default:
					return TranscriptionComponentModes.Review;
			}
		}

		protected override string TaskName
		{
			get { return "Transcribing"; }
		}
	}

	public class TranscriptionComponentModes
	{
		public static IContinuousWorkflowComponentMode Create = new CreateTranscriptionComponentMode();
		public static IContinuousWorkflowComponentMode Edit = new EditTranscriptionComponentMode();
		public static IContinuousWorkflowComponentMode Review = new ReviewTranscriptionComponentMode();
	}

	public class EditTranscriptionComponentMode : ContinuousWorkflowComponentMode
	{
		public EditTranscriptionComponentMode() : base(false, false, false)
		{
		}
	}

	public class CreateTranscriptionComponentMode : ContinuousWorkflowComponentMode
	{
		public CreateTranscriptionComponentMode() : base(true, true, true)
		{
		}
	}

	public class ReviewTranscriptionComponentMode : ContinuousWorkflowComponentMode
	{
		public ReviewTranscriptionComponentMode() : base(false, false, false)
		{
		}
	}
}