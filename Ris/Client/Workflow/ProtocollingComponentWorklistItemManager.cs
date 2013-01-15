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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class ProtocollingComponentWorklistItemManager : WorklistItemManager<ReportingWorklistItemSummary, IReportingWorkflowService>
	{
		public ProtocollingComponentWorklistItemManager(string folderName, EntityRef worklistRef, string worklistClassName)
			: base(folderName, worklistRef, worklistClassName)
		{
		}

		protected override IContinuousWorkflowComponentMode GetMode<TWorklistITem>(ReportingWorklistItemSummary worklistItem)
		{
			throw new NotSupportedException("Protocolling component mode should be initialized externally.  ReportingWorklistItemSummary does not have enough context.");
		}

		protected override string TaskName
		{
			get { return "Protocolling"; }
		}
	}

	public static class ProtocollingComponentModes
	{
		public static IContinuousWorkflowComponentMode Assign = new AssignProtocolMode();
		public static IContinuousWorkflowComponentMode Edit = new EditProtocolMode();
		public static IContinuousWorkflowComponentMode Review = new ReviewProtocolMode();
	}

	public class AssignProtocolMode : ContinuousWorkflowComponentMode
	{
		public AssignProtocolMode()
			: base(true, true, true)
		{
		}
	}

	public class EditProtocolMode : ContinuousWorkflowComponentMode
	{
		public EditProtocolMode()
			: base(false, false, false)
		{
		}
	}

	public class ReviewProtocolMode : ContinuousWorkflowComponentMode
	{
		public ReviewProtocolMode()
			: base(false, false, false)
		{
		}
	}
}