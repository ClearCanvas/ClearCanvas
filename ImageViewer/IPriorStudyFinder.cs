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
    public class PriorStudyFinderResult
    {
        public PriorStudyFinderResult(StudyItemList studies, bool resultsComplete)
        {
            ResultsComplete = resultsComplete;
            Studies = studies;
        }

        public StudyItemList Studies { get; private set; }
        public bool ResultsComplete { get; set; }
    }

	/// <summary>
	/// Defines the interface for finding related (or 'prior') studies
	/// based on the studies that are already loaded in an <see cref="IImageViewer"/>'s <see cref="StudyTree"/>.
	/// </summary>
	/// <remarks>
	/// The <see cref="ImageViewerComponent"/> internally uses an <see cref="IPriorStudyFinder"/> from within
	/// it's <see cref="IPriorStudyLoader"/> to find and load prior related studies into the <see cref="ImageViewerComponent"/>.
	/// </remarks>
	public interface IPriorStudyFinder
	{
		/// <summary>
		/// Sets the <see cref="IImageViewer"/> for which prior studies are to found (and added/loaded).
		/// </summary>
		void SetImageViewer(IImageViewer viewer);

		/// <summary>
		/// Gets the list of prior studies.
		/// </summary>
		/// <returns></returns>
        PriorStudyFinderResult FindPriorStudies();

		/// <summary>
		/// Cancels the search for prior studies.
		/// </summary>
		void Cancel();
	}
}
