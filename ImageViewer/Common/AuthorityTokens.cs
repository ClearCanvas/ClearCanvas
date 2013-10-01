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

// ReSharper disable CheckNamespace
// ReSharper disable MemberHidesStaticFromOuterClass

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines authority tokens for controlling access to various common viewer functionalities.
	/// </summary>
	public static class AuthorityTokens
	{
		/// <summary>
		/// Permission required in order to see any viewer components (i.e. without this, all viewer components are hidden).
		/// </summary>
		[AuthorityToken(Description = "Permission required in order to see any viewer components (e.g. without this, all viewer components are hidden).")]
		public const string ViewerVisible = "Viewer/Visible";

		/// <summary>
		/// Permission to use clinical tools with the viewer.
		/// </summary>
		[AuthorityToken(Description = "Permission to use clinical tools with the viewer.")]
		public const string ViewerClinical = "Viewer/Clinical";

		/// <summary>
		/// Defines authority tokens for controlling access to various study-based operations.
		/// </summary>
		public static class Study
		{
			/// <summary>
			/// Attachments tokens.
			/// </summary>
			public class Attachments
			{
				/// <summary>
				/// Permission to view study attachments.
				/// </summary>
				[AuthorityToken(Description = "Permission to view study attachments.")]
				public const string View = "Viewer/Study/Attachments/View";

				/// <summary>
				/// Permission to add attachments to a study.
				/// </summary>
				[AuthorityToken(Description = "Permission to add attachments to a study.")]
				public const string Add = "Viewer/Study/Attachments/Add";
			}

			/// <summary>
			/// Permission to open a study in the viewer.
			/// </summary>
			[AuthorityToken(Description = "Permission to open a study in the viewer.")]
			public const string Open = "Viewer/Study/Open";

			[AuthorityToken(Description = "Permission to send a study to another DICOM device (e.g. another workstation or PACS).")]
			public const string Send = "Viewer/Study/Send";

			[AuthorityToken(Description = "Permission to delete a study from the local store.")]
			public const string Delete = "Viewer/Study/Delete";

			[AuthorityToken(Description = "Permission to retrieve a study to the local store.")]
			public const string Retrieve = "Viewer/Study/Retrieve";

			[AuthorityToken(Description = "Permission to import study data into the local store.")]
			public const string Import = "Viewer/Study/Import";

			[AuthorityToken(Description = "Permission to copy an unanonymized study out of the viewer.")]
			public const string Export = "Viewer/Study/Export ";

			[AuthorityToken(Description = "Permission to anonymize a study in the viewer.")]
			public const string Anonymize = "Viewer/Study/Anonymize";

			[AuthorityToken(Description = "Grant access to key image functionality.", Formerly = "Viewer/Reporting/Key Images")]
			public const string KeyImages = "Viewer/Study/Key Images";

			[AuthorityToken(Description = "Permission to write studies onto media.")]
			public const string WriteMedia = "Viewer/Study/Write Media";
		}

		/// <summary>
		/// Defines authority tokens for controlling access to the activity monitor.
		/// </summary>
		public static class ActivityMonitor
		{
			[AuthorityToken(Description = "Permission to view the Activity Monitor.")]
			public const string View = "Viewer/Activity Monitor/View";

			public class WorkItems
			{
				[AuthorityToken(Description = "Permission to stop a work item.")]
				public const string Stop = "Viewer/Activity Monitor/Work Items/Stop";

				[AuthorityToken(Description = "Permission to restart a work item.")]
				public const string Restart = "Viewer/Activity Monitor/Work Items/Restart";

				[AuthorityToken(Description = "Permission to delete a work item.")]
				public const string Delete = "Viewer/Activity Monitor/Work Items/Delete";

				[AuthorityToken(Description = "Permission to stat or re-prioritize work items.")]
				public const string Prioritize = "Viewer/Activity Monitor/Work Items/Prioritize";
			}
		}

		/// <summary>
		/// Defines authority tokens for controlling access to various administrative functionalities.
		/// </summary>
		public static class Administration
		{
			[AuthorityToken(Description = "Permission to re-index the local file store.", Formerly = "Viewer/Administration/Reindex Local Data Store")]
			public const string ReIndex = "Viewer/Administration/Re-index";

			[AuthorityToken(Description = "Allow administration of the viewer services (e.g. Start/Stop/Restart).")]
			public const string Services = "Viewer/Administration/Services";
		}

		/// <summary>
		/// Defines authority tokens for controlling access to updating the viewer configuration.
		/// </summary>
		public static class Configuration
		{
			[AuthorityToken(Description = "Allow configuration of data publishing options.", Formerly = "Viewer/Administration/Key Images")]
			public const string Publishing = "Viewer/Configuration/Publishing";

			[AuthorityToken(Description = "Allow administration/configuration of the local DICOM Server (e.g. set AE Title, Port).", Formerly = "Viewer/Administration/DICOM Server")]
			public const string DicomServer = "Viewer/Configuration/DICOM Server";

			[AuthorityToken(Description = "Allow configuration of local DICOM storage.", Formerly = "Viewer/Administration/Storage")]
			public const string Storage = "Viewer/Configuration/Storage";

			[AuthorityToken(Description = "Allow configuration of 'My Servers'.")]
			public const string MyServers = "Viewer/Configuration/My Servers";
		}

		/// <summary>
		/// Defines authority tokens for controlling access to the viewer clipboard.
		/// </summary>
		public static class Clipboard
		{
			/// <summary>
			/// Clipboard export tokens
			/// </summary>
			public class Export
			{
				/// <summary>
				/// Permission to export clipboard items into JPG files.
				/// </summary>
				[AuthorityToken(Description = "Permission to export clipboard items into JPG files.")]
				public const string Jpeg = "Viewer/Clipboard/Export/JPG";

				/// <summary>
				/// Permission to export clipboard items into AVI files.
				/// </summary>
				[AuthorityToken(Description = "Permission to export clipboard items into AVI files.")]
				public const string Avi = "Viewer/Clipboard/Export/AVI";
			}
		}

		/// <summary>
		/// Defines authority tokens for controlling access to the explorer components.
		/// </summary>
		public static class Explorer
		{
			[AuthorityToken(Description = "Grant access to the DICOM explorer.")]
			public const string DicomExplorer = "Viewer/Explorer/DICOM";

			[AuthorityToken(Description = "Grant access to the 'My Computer' explorer.")]
			public const string MyComputer = "Viewer/Explorer/My Computer";
		}

		/// <summary>
		/// Defines authority tokens for controlling access to the print functionality in the viewer.
		/// </summary>
		public static class Print
		{
			[AuthorityToken(Description = "Permission to send images to a DICOM printer.", Formerly = "Viewer/Clipboard/Export/DICOM Print")]
			public const string Dicom = "Viewer/Print/DICOM";
		}

		[AuthorityToken(Description = "Allow publishing of locally created data to remote servers.")]
		public const string Publishing = "Viewer/Publishing";

		[AuthorityToken(Description = "Grant access to the External Applications feature.")]
		public const string Externals = "Viewer/Externals";

		[AuthorityToken(Description = "Grant access to the Study Filters.")]
		public const string StudyFilters = "Viewer/Study Filters";

		[AuthorityToken(Description = "Grant access to the DICOM Editor.")]
		public const string DicomEditor = "Viewer/DICOM Editor";
	}
}

// ReSharper restore MemberHidesStaticFromOuterClass
// ReSharper restore CheckNamespace