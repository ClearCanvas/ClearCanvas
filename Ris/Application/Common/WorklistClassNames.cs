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

namespace ClearCanvas.Ris.Application.Common
{
	public class WorklistClassNames
	{
		#region Registration Worklist Class Names

		[WorklistClassName]
		public const string RegistrationScheduledWorklist = "RegistrationScheduledWorklist";

		[WorklistClassName]
		public const string RegistrationCheckedInWorklist = "RegistrationCheckedInWorklist";

		[WorklistClassName]
		public const string RegistrationCancelledWorklist = "RegistrationCancelledWorklist";

		[WorklistClassName]
		public const string RegistrationPerformedWorklist = "RegistrationPerformedWorklist";

		[WorklistClassName]
		public const string RegistrationInProgressWorklist = "RegistrationInProgressWorklist";

		#endregion

		#region Performing Worklist Class Names

		[WorklistClassName]
		public const string PerformingScheduledWorklist = "PerformingScheduledWorklist";

		[WorklistClassName]
		public const string PerformingCheckedInWorklist = "PerformingCheckedInWorklist";

		[WorklistClassName]
		public const string PerformingCancelledWorklist = "PerformingCancelledWorklist";

		[WorklistClassName]
		public const string PerformingPerformedWorklist = "PerformingPerformedWorklist";

		[WorklistClassName]
		public const string PerformingInProgressWorklist = "PerformingInProgressWorklist";

		[WorklistClassName]
		public const string PerformingUndocumentedWorklist = "PerformingUndocumentedWorklist";

		#endregion

		#region Reporting Worklist Class Names

		[WorklistClassName]
		public const string ReportingToBeReportedWorklist = "ReportingToBeReportedWorklist";

		[WorklistClassName]
		public const string ReportingAssignedWorklist = "ReportingAssignedWorklist";

		[WorklistClassName]
		public const string ReportingDraftWorklist = "ReportingDraftWorklist";

		[WorklistClassName]
		public const string ReportingInTranscriptionWorklist = "ReportingInTranscriptionWorklist";

		[WorklistClassName]
		public const string ReportingReviewTranscriptionWorklist = "ReportingReviewTranscriptionWorklist";

		[WorklistClassName]
		public const string ReportingVerifiedWorklist = "ReportingVerifiedWorklist";

		[WorklistClassName]
		public const string ReportingToBeReviewedReportWorklist = "ReportingToBeReviewedReportWorklist";

		[WorklistClassName]
		public const string ReportingAssignedReviewWorklist = "ReportingAssignedReviewWorklist";

		[WorklistClassName]
		public const string ReportingAwaitingReviewWorklist = "ReportingAwaitingReviewWorklist";

		#endregion


		#region Reporting Tracking

		[WorklistClassName]
		public const string ReportingTrackingActiveWorklist = "ReportingTrackingActiveWorklist";

		[WorklistClassName]
		public const string ReportingTrackingDraftWorklist = "ReportingTrackingDraftWorklist";

		[WorklistClassName]
		public const string ReportingTrackingPreliminaryWorklist = "ReportingTrackingPreliminaryWorklist";

		[WorklistClassName]
		public const string ReportingTrackingFinalWorklist = "ReportingTrackingFinalWorklist";

		[WorklistClassName]
		public const string ReportingTrackingCorrectedWorklist = "ReportingTrackingCorrectedWorklist";

		#endregion

		#region Radiologist Admin Worklist Class Names

		[WorklistClassName]
		public const string ReportingAdminUnreportedWorklist = "ReportingAdminUnreportedWorklist";

		[WorklistClassName]
		public const string ReportingAdminAssignedWorklist = "ReportingAdminAssignedWorklist";

		[WorklistClassName]
		public const string ReportingAdminToBeTranscribedWorklist = "ReportingAdminToBeTranscribedWorklist";

		#endregion

		#region Transcription Worklist Class Names

		[WorklistClassName]
		public const string TranscriptionToBeTranscribedWorklist = "TranscriptionToBeTranscribedWorklist";

		[WorklistClassName]
		public const string TranscriptionDraftWorklist = "TranscriptionDraftWorklist";

		[WorklistClassName]
		public const string TranscriptionCompletedWorklist = "TranscriptionCompletedWorklist";

		[WorklistClassName]
		public const string TranscriptionToBeReviewedWorklist = "TranscriptionToBeReviewedWorklist";

		[WorklistClassName]
		public const string TranscriptionAwaitingReviewWorklist = "TranscriptionAwaitingReviewWorklist";


		#endregion
	}
}
