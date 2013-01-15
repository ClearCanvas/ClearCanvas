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
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public static class AuthorityTokens
	{
		[AuthorityToken(Description = "Grant access to the Study Filters.")]
		public const string StudyFilters = "Viewer/Study Filters";

		public static class Study
		{
			[AuthorityToken(Description = "Permission to copy an unanonymized study out of the viewer.")]
			public const string Export = "Viewer/Study/Export ";

			[AuthorityToken(Description = "Permission to anonymize a study in the viewer.")]
			public const string Anonymize = "Viewer/Study/Anonymize";
		}
	}
}