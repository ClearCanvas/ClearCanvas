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

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Common viewer authority tokens.
	/// </summary>
	public class AuthorityTokens
	{
		/// <summary>
		/// Permission required in order to see any viewer components (e.g. without this, all viewer components are hidden).
		/// </summary>
		[AuthorityToken(Description = "Permission required in order to see any viewer components (e.g. without this, all viewer components are hidden).")]
		public const string ViewerVisible = "Viewer/Visible";

        /// <summary>
        /// Permission to use clinical tools with the viewer (users with this token will require a license seat).
        /// </summary>
        [AuthorityToken(Description = "Permission to use clinical tools with the viewer.")]
        public const string ViewerClinical = "Viewer/Clinical";

		/// <summary>
		/// Study tokens.
		/// </summary>
		public class Study
		{
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
        }

	    public class ActivityMonitor
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


	    public static class Administration
	    {
	        [AuthorityToken(Description = "Permission to re-index the local file store.", Formerly = "Viewer/Administration/Reindex Local Data Store")]
	        public const string ReIndex = "Viewer/Administration/Re-index";

	        [AuthorityToken(Description = "Allow administration of the viewer services (e.g. Start/Stop/Restart).")]
	        public const string Services = "Viewer/Administration/Services";
	    }
	}
}
