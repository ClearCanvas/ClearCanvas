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
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public class Registration
	{
		[ExtensionOf(typeof(RegistrationWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationScheduledWorklist)]
		[FolderPath("Scheduled", true)]
		[FolderDescription("RegistrationScheduledFolderDescription")]
		public class ScheduledFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RegistrationWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationCheckedInWorklist)]
		[FolderPath("Checked In")]
		[FolderDescription("RegistrationCheckedInFolderDescription")]
		public class CheckedInFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RegistrationWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationInProgressWorklist)]
		[FolderPath("In Progress")]
		[FolderDescription("RegistrationInProgressFolderDescription")]
		public class InProgressFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RegistrationWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationPerformedWorklist)]
		[FolderPath("Performed")]
		[FolderDescription("RegistrationPerformedFolderDescription")]
		public class PerformedFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RegistrationWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationCancelledWorklist)]
		[FolderPath("Cancelled")]
		[FolderDescription("RegistrationCancelledFolderDescription")]
		public class CancelledFolder : RegistrationWorkflowFolder
		{
		}


		[FolderPath("Search Results")]
		public class RegistrationSearchFolder : WorklistSearchResultsFolder<RegistrationWorklistItemSummary, IRegistrationWorkflowService>
		{
			public RegistrationSearchFolder()
				: base(new RegistrationWorklistTable())
			{
			}

			protected override string ProcedureStepClassName
			{
				get { return "RegistrationProcedureStep"; }
			}
		}

	}
}
