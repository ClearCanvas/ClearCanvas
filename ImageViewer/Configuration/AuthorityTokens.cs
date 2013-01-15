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

namespace ClearCanvas.ImageViewer.Configuration
{
	public static class AuthorityTokens
	{
		[AuthorityToken(Description = "Allow publishing of locally created data to remote servers.")]
		public const string Publishing = "Viewer/Publishing";

		public static class Configuration
		{
		    [AuthorityToken(Description = "Allow configuration of data publishing options.", Formerly = "Viewer/Administration/Key Images")]
			public const string Publishing = "Viewer/Configuration/Publishing";

            [AuthorityToken(Description = "Allow administration/configuration of the local DICOM Server (e.g. set AE Title, Port).", Formerly = "Viewer/Administration/DICOM Server")]
            public const string DicomServer = "Viewer/Configuration/DICOM Server";

            [AuthorityToken(Description = "Allow configuration of local DICOM storage.", Formerly = "Viewer/Administration/Storage")]
            public const string Storage = "Viewer/Configuration/Storage";
        }
	}
}
