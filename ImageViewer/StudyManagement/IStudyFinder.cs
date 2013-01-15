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
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	//TODO (later): move this stuff into ImageViewer.Common and deprecate it.

	/// <summary>
	/// Defines a study finder.
	/// </summary>
	/// <remarks>
	/// <see cref="IStudyFinder"/> abstracts the finding of studies,
	/// allowing many means of finding studies (e.g., local database,
	/// DICOM query, DICOMDIR, etc.) to be treated in the same way.
	/// </remarks>
    public interface IStudyFinder
    {
		/// <summary>
		/// Gets the name of the study finder.
		/// </summary>
        string Name { get; }

		/// <summary>
		/// Queries for studies on a target server matching the specified query parameters.
		/// </summary>
        StudyItemList Query(QueryParameters queryParams, IApplicationEntity targetServer);
    }
}
