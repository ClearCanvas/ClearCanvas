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

namespace ClearCanvas.Ris.Client.Workflow.Extended.Folders
{
	public class Booking
	{
		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.RegistrationToBeScheduledWorklist)]
		[FolderPath("To Be Scheduled")]
		[FolderDescription("BookingToBeScheduledFolderDescription")]
		public class ToBeScheduledFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.RegistrationPendingProtocolWorklist)]
		[FolderPath("Pending Protocol")]
		[FolderDescription("BookingPendingProtocolFolderDescription")]
		public class PendingProtocolFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.RegistrationCompletedProtocolWorklist)]
		[FolderPath("Completed Protocol", true)]
		[FolderDescription("BookingCompletedProtocolFolderDescription")]
		public class CompletedProtocolFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.RegistrationRejectedProtocolWorklist)]
		[FolderPath("Rejected Protocol")]
		[FolderDescription("BookingRejectedProtocolFolderDescription")]
		public class RejectedProtocolFolder : RegistrationWorkflowFolder
		{
		}

		[FolderPath("Search Results")]
		public class BookingSearchFolder : WorklistSearchResultsFolder<RegistrationWorklistItemSummary, IRegistrationWorkflowService>
		{
			public BookingSearchFolder()
				: base(new RegistrationWorklistTable())
			{
			}


			//TODO: (JR may 2008) having the client specify the class name isn't a terribly good idea, but
			//it is the only way to get things working right now
			protected override string ProcedureStepClassName
			{
				get { return "ProtocolResolutionStep"; }
			}

		}
	}
}
