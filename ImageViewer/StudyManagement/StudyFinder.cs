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

using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Abstract base class for an <see cref="IStudyFinder"/>.
	/// </summary>
	/// <remarks>
	/// <see cref="IStudyFinder"/> abstracts the finding of studies,
	/// allowing many means of finding studies (e.g., local database,
	/// DICOM query, DICOMDIR, etc.) to be treated in the same way..
	/// </remarks>
	public abstract class StudyFinder : IStudyFinder
	{
		private readonly string _name;

		/// <summary>
		/// Constructs a new <see cref="StudyFinder"/> with the given <paramref name="name"/>.
		/// </summary>
		/// <param name="name"></param>
		protected StudyFinder(string name)
		{
			_name = name;
		}

		#region IStudyFinder Members

		/// <summary>
		/// Gets the name of the study finder.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

	    /// <summary>
		/// Queries for studies on a target server matching the specified query parameters.
		/// </summary>
        public abstract StudyItemList Query(QueryParameters queryParams, IApplicationEntity targetServer);

		#endregion
	}
}