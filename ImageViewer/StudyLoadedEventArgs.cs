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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Event args for <see cref="EventBroker.StudyLoadFailed"/>.
	/// </summary>
	public class StudyLoadFailedEventArgs : EventArgs
	{
		internal StudyLoadFailedEventArgs(StudyItem study, Exception error)
		{
			this.Error = error;
			this.Study = study;
		}

		internal StudyLoadFailedEventArgs(LoadStudyArgs loadArgs, Exception error)
		{
			this.LoadArgs = loadArgs;
			this.Error = error;
		}

		/// <summary>
		/// Gets the <see cref="LoadStudyArgs"/> that were used to attempt to load the study.
		/// </summary>
		public readonly LoadStudyArgs LoadArgs;

		/// <summary>
		/// Gets the <see cref="StudyItem"/> that failed to load.
		/// </summary>
		/// <remarks>
		/// This object is generated via a query mechanism, such as <see cref="IStudyFinder"/>
		/// or <see cref="IPriorStudyFinder"/>.
		/// </remarks>
		public readonly StudyItem Study;

		/// <summary>
		/// Gets the <see cref="Exception"/> that occurred.
		/// </summary>
		public readonly Exception Error;
	}

	/// <summary>
	/// Event args for <see cref="EventBroker.StudyLoaded"/>.
	/// </summary>
	public class StudyLoadedEventArgs : EventArgs
	{
		internal StudyLoadedEventArgs(Study study, Exception error)
		{
			this.Error = error;
			this.Study = study;
		}

		/// <summary>
		/// Gets the <see cref="StudyManagement.Study"/> that was loaded.
		/// </summary>
		public readonly Study Study;

		/// <summary>
		/// If <see cref="Study"/> was only partially loaded, this
		/// will contain the <see cref="Exception"/> that describes the
		/// partial load failure.
		/// </summary>
		public readonly Exception Error;
	}
}
