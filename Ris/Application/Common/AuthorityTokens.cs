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

using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Ris.Application.Common
{
	/// <summary>
	/// Defines constants for all core RIS authority tokens.
	/// </summary>
	public static class AuthorityTokens
	{
		/// <summary>
		/// Tokens that allow access to administrative functionality.
		/// </summary>
		public static class Admin
		{
			public static class Data
			{
				[AuthorityToken(Description = "Allow administration of Facilities.")]
				public const string Facility = "RIS/Admin/Data/Facility";

				[AuthorityToken(Description = "Allow administration of Departments.")]
				public const string Department = "RIS/Admin/Data/Department";

				[AuthorityToken(Description = "Allow administration of Patient Locations.")]
				public const string Location = "RIS/Admin/Data/Location";

				[AuthorityToken(Description = "Allow administration of Modalities.")]
				public const string Modality = "RIS/Admin/Data/Modality";

				[AuthorityToken(Description = "Allow administration of Procedure Types.")]
				public const string ProcedureType = "RIS/Admin/Data/Procedure Type";

				[AuthorityToken(Description = "Allow administration of Procedure Type Groups (such as Performing, Reading, and Relevance Groups.")]
				public const string ProcedureTypeGroup = "RIS/Admin/Data/Procedure Type Group";

				[AuthorityToken(Description = "Allow administration of Imaging Services.")]
				public const string DiagnosticService = "RIS/Admin/Data/Imaging Service";

				[AuthorityToken(Description = "Allow administration of Enumerations.")]
				public const string Enumeration = "RIS/Admin/Data/Enumeration";

				[AuthorityToken(Description = "Allow administration of Worklists.")]
				public const string Worklist = "RIS/Admin/Data/Worklist";

				[AuthorityToken(Description = "Allow administration of Staff.")]
				public const string Staff = "RIS/Admin/Data/Staff";

				[AuthorityToken(Description = "Allow administration of Staff Groups.")]
				public const string StaffGroup = "RIS/Admin/Data/Staff Group";

				[AuthorityToken(Description = "Allow administration of External Practitioners.")]
				public const string ExternalPractitioner = "RIS/Admin/Data/External Practitioner";

				[AuthorityToken(Description = "Allow verification of External Practitioners.")]
				public const string ExternalPractitionerVerification = "RIS/Admin/Data/External Practitioner Verification";

				[AuthorityToken(Description = "Allow administration of Patient Note Categories.")]
				public const string PatientNoteCategory = "RIS/Admin/Data/Patient Note Category";

                [AuthorityToken(Description = "Allow administration of Scheduling.")]
                public const string Scheduling = "RIS/Admin/Data/Scheduling";
			}
		}

		/// <summary>
		/// Tokens that allow access to management tools and functionality.
		/// </summary>
		public static class Management
		{
			[AuthorityToken(Description = "Allow administration of the work queue.")]
			public const string WorkQueue = "RIS/Management/Work Queue";
		}

#if DEBUG	// these tokens should not be available in production builds
		/// <summary>
		/// Tokens that allow access to development tools and functionality.
		/// </summary>
		public static class Development
		{
			[AuthorityToken(Description = "Allow viewing of unfiltered worklists in top-level folders.")]
			public const string ViewUnfilteredWorkflowFolders = "RIS/Development/View Unfiltered Workflow Folders";

			[AuthorityToken(Description = "Allow creation of randomly generated test orders.")]
			public const string CreateTestOrder = "RIS/Development/Create Test Order";

			[AuthorityToken(Description = "Allow usage of the tool for manual publication of radiology reports.")]
			public const string TestPublishReport = "RIS/Development/Test Publish";
		}
#endif

		/// <summary>
		/// Tokens that permit workflow actions.
		/// </summary>
		public static class Workflow
		{
			public static class HomePage
			{
				[AuthorityToken(Description = "Allow access to the home page.")]
				public const string View = "RIS/Workflow/Home Page/View";
			}


			public static class PatientBiography
			{
				[AuthorityToken(Description = "Allow viewing of Patient Biography.")]
				public const string View = "RIS/Workflow/Patient Biography/View";
			}

			public static class CannedText
			{
				[AuthorityToken(Description = "Allow creation, modification and deletion of personal Canned Texts.")]
				public const string Personal = "RIS/Workflow/Canned Text/Personal";

				[AuthorityToken(Description = "Allow creation, modification and deletion of group Canned Texts.")]
				public const string Group = "RIS/Workflow/Canned Text/Group";
			}


			public static class Report
			{
				[AuthorityToken(Description = "Allow access to the Report Editor and creation of radiology reports.")]
				public const string Create = "RIS/Workflow/Report/Create";

				[AuthorityToken(Description = "Allow verification of radiology reports.")]
				public const string Verify = "RIS/Workflow/Report/Verify";

				[AuthorityToken(Description = "Allow radiology reports to be submitted for review by another radiologist.")]
				public const string SubmitForReview = "RIS/Workflow/Report/Submit for Review";

				[AuthorityToken(Description = "Allow creation of radiology reports without specifying a supervisor.")]
				public const string OmitSupervisor = "RIS/Workflow/Report/Omit Supervisor";

				[AuthorityToken(Description = "Allow re-assignment of a radiology report that is owned by one radiologist to another radiologist.")]
				public const string Reassign = "RIS/Workflow/Report/Reassign";

				[AuthorityToken(Description = "Allow cancellation of a radiology report that is owned by another radiologist.")]
				public const string Cancel = "RIS/Workflow/Report/Cancel";
			}

			public static class Transcription
			{
				[AuthorityToken(Description = "Allow access to the Transcription Editor and creation of report transcriptions.")]
				public const string Create = "RIS/Workflow/Transcription/Create";

				[AuthorityToken(Description = "Allow transcriptions to be submitted for review by another party.")]
				public const string SubmitForReview = "RIS/Workflow/Transcription/Submit For Review";
			}

			public static class Patient
			{
				[AuthorityToken(Description = "Allow creation of new Patient records.")]
				public const string Create = "RIS/Workflow/Patient/Create";

				[AuthorityToken(Description = "Allow updating of Patient records (excluding Patient Profile information).")]
				public const string Update = "RIS/Workflow/Patient/Update";
			}

			public static class PatientProfile
			{
				[AuthorityToken(Description = "Allow updating of existing Patient Profile records.")]
				public const string Update = "RIS/Workflow/Patient Profile/Update";
			}

			public static class Visit
			{
				[AuthorityToken(Description = "Allow creation of new Visit records.")]
				public const string Create = "RIS/Workflow/Visit/Create";

				[AuthorityToken(Description = "Allow updating of existing Visit records.")]
				public const string Update = "RIS/Workflow/Visit/Update";
			}

			public static class Order
			{
				[AuthorityToken(Description = "Allow creation of new Orders.")]
				public const string Create = "RIS/Workflow/Order/Create";

				[AuthorityToken(Description = "Allow modification of existing Orders.")]
				public const string Modify = "RIS/Workflow/Order/Modify";

				[AuthorityToken(Description = "Allow replacement of existing Orders.")]
				public const string Replace = "RIS/Workflow/Order/Replace";

				[AuthorityToken(Description = "Allow merging of orders.")]
				public const string Merge = "RIS/Workflow/Order/Merge";

				[AuthorityToken(Description = "Allow un-merging of merged orders.")]
				public const string Unmerge = "RIS/Workflow/Order/Unmerge";

				[AuthorityToken(Description = "Allow cancellation of existing Orders.")]
				public const string Cancel = "RIS/Workflow/Order/Cancel";
			}

			public static class ExternalPractitioner
			{
				[AuthorityToken(Description = "Allow creation of External Practitioner records.")]
				public const string Create = "RIS/Workflow/External Practitioner/Create";

				[AuthorityToken(Description = "Allow updating of existing External Practitioner records.")]
				public const string Update = "RIS/Workflow/External Practitioner/Update";

				[AuthorityToken(Description = "Allow merging of existing External Practitioner records.")]
				public const string Merge = "RIS/Workflow/External Practitioner/Merge";
			}

			public static class Procedure
			{
				[AuthorityToken(Description = "Allow access to the Procedure Check-In function.")]
				public const string CheckIn = "RIS/Workflow/Procedure/Check In";
			}

			public static class Documentation
			{
				[AuthorityToken(Description = "Allow access to the Exam Documentation function, and creation of Exam Documentation.")]
				public const string Create = "RIS/Workflow/Documentation/Create";

				[AuthorityToken(Description = "Allow acceptance of Exam Documentation.")]
				public const string Accept = "RIS/Workflow/Documentation/Accept";
			}

			public static class Downtime
			{
				[AuthorityToken(Description = "Allow printing of downtime forms.")]
				public const string PrintForms = "RIS/Workflow/Downtime/Print Forms";

				[AuthorityToken(Description = "Allow access to the downtime recovery operations.")]
				public const string RecoveryOperations = "RIS/Workflow/Downtime/Recovery Operations";
			}

			public static class Worklist
			{
				[AuthorityToken(Description = "Allow creation, modification and deletion of personal worklists.")]
				public const string Personal = "RIS/Workflow/Worklist/Personal";

				[AuthorityToken(Description = "Allow creation, modification and deletion of group worklists.")]
				public const string Group = "RIS/Workflow/Worklist/Group";
			}

			public static class StaffProfile
			{
				[AuthorityToken(Description = "Allow a user to view own Staff Profile.")]
				public const string View = "RIS/Workflow/Staff Profile/View";

				[AuthorityToken(Description = "Allow a user to update own Staff Profile.")]
				public const string Update = "RIS/Workflow/Staff Profile/Update";
			}

			public static class Images
			{
				[AuthorityToken(Description = "Allow a user to view images.")]
				public const string View = "RIS/Workflow/Images/View";
			}
		}

		/// <summary>
		/// Tokens that control access to Folder Systems.
		/// </summary>
		public static class FolderSystems
		{
			[AuthorityToken(Description = "Allow access to the Registration folder system.")]
			public const string Registration = "RIS/Folder Systems/Registration";

			[AuthorityToken(Description = "Allow access to the Performing folder system.")]
			public const string Performing = "RIS/Folder Systems/Performing";

			[AuthorityToken(Description = "Allow access to the Reporting folder system.")]
			public const string Reporting = "RIS/Folder Systems/Reporting";

			[AuthorityToken(Description = "Allow access to the Radiologist Admin folder system.")]
			public const string RadiologistAdmin = "RIS/Folder Systems/Radiologist Admin";

			[AuthorityToken(Description = "Allow access to the Transcription folder system.")]
			public const string Transcription = "RIS/Folder Systems/Transcription";

			[AuthorityToken(Description = "Allow access to the Scheduling folder system.")]
			public const string Scheduling = "RIS/Folder Systems/Scheduling";
		}
	}
}
