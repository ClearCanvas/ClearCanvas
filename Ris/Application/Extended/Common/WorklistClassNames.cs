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

using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common
{
	public class WorklistClassNames
	{
		#region Registration Protocoling Worklist Class Names

		[WorklistClassName]
		public const string RegistrationCompletedProtocolWorklist = "RegistrationCompletedProtocolWorklist";

		[WorklistClassName]
		public const string RegistrationRejectedProtocolWorklist = "RegistrationRejectedProtocolWorklist";

		[WorklistClassName]
		public const string RegistrationPendingProtocolWorklist = "RegistrationPendingProtocolWorklist";

		[WorklistClassName]
		public const string RegistrationAsapPendingProtocolWorklist = "RegistrationAsapPendingProtocolWorklist";

		[WorklistClassName]
		public const string RegistrationToBeScheduledWorklist = "RegistrationToBeScheduledWorklist";

		#endregion

		#region Protocolling

		[WorklistClassName]
		public const string ReportingToBeProtocolledWorklist = "ReportingToBeProtocolledWorklist";

		[WorklistClassName]
		public const string ReportingAssignedProtocolWorklist = "ReportingAssignedProtocolWorklist";

		[WorklistClassName]
		public const string ReportingToBeReviewedProtocolWorklist = "ReportingToBeReviewedProtocolWorklist";

		[WorklistClassName]
		public const string ReportingAssignedReviewProtocolWorklist = "ReportingAssignedReviewProtocolWorklist";

		[WorklistClassName]
		public const string ReportingDraftProtocolWorklist = "ReportingDraftProtocolWorklist";

		[WorklistClassName]
		public const string ReportingCompletedProtocolWorklist = "ReportingCompletedProtocolWorklist";

		[WorklistClassName]
		public const string ReportingRejectedProtocolWorklist = "ReportingRejectedProtocolWorklist";

		[WorklistClassName]
		public const string ReportingAwaitingApprovalProtocolWorklist = "ReportingAwaitingApprovalProtocolWorklist";

		#endregion

		#region Emergency Worklist Class Names

		[WorklistClassName]
		public const string EmergencyScheduledWorklist = "EmergencyScheduledWorklist";

		[WorklistClassName]
		public const string EmergencyInProgressWorklist = "EmergencyInProgressWorklist";

		[WorklistClassName]
		public const string EmergencyPerformedWorklist = "EmergencyPerformedWorklist";

		[WorklistClassName]
		public const string EmergencyCancelledWorklist = "EmergencyCancelledWorklist";
		
		#endregion

		#region Radiologist Admin Worklist Class Names

		[WorklistClassName]
		public const string ProtocollingAdminAssignedWorklist = "ProtocollingAdminAssignedWorklist";

		#endregion
	}
}
