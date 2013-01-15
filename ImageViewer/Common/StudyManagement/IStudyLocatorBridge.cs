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
using System.Collections.Generic;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
	public interface IStudyLocatorBridge : IStudyLocator, IDisposable
	{
		/// <summary>
		/// Comparer used to sort the results returned from <see cref="IStudyLocator.LocateStudies"/>.
		/// </summary>
		IComparer<StudyRootStudyIdentifier> StudyComparer { get; set; }

		/// <summary>
		/// Comparer used to sort the results returned from <see cref="IStudyLocator.LocateSeries"/>.
		/// </summary>
		IComparer<SeriesIdentifier> SeriesComparer { get; set; }

		/// <summary>
		/// Comparer used to sort the results returned from <see cref="IStudyLocator.LocateImages"/>.
		/// </summary>
		IComparer<ImageIdentifier> ImageComparer { get; set; }

		IList<StudyRootStudyIdentifier> LocateStudyByAccessionNumber(string accessionNumber);
		IList<StudyRootStudyIdentifier> LocateStudyByAccessionNumber(string accessionNumber, out LocateFailureInfo[] failures);
		IList<StudyRootStudyIdentifier> LocateStudyByPatientId(string patientId);
		IList<StudyRootStudyIdentifier> LocateStudyByPatientId(string patientId, out LocateFailureInfo[] failures);
		IList<StudyRootStudyIdentifier> LocateStudyByInstanceUid(string studyInstanceUid);
		IList<StudyRootStudyIdentifier> LocateStudyByInstanceUid(string studyInstanceUid, out LocateFailureInfo[] failures);
		IList<StudyRootStudyIdentifier> LocateStudyByInstanceUid(IEnumerable<string> studyInstanceUids);
		IList<StudyRootStudyIdentifier> LocateStudyByInstanceUid(IEnumerable<string> studyInstanceUids, out LocateFailureInfo[] failures);
		IList<SeriesIdentifier> LocateSeriesByStudy(string studyInstanceUid);
		IList<SeriesIdentifier> LocateSeriesByStudy(string studyInstanceUid, out LocateFailureInfo[] failures);
		IList<ImageIdentifier> LocateImagesBySeries(string studyInstanceUid, string seriesInstanceUid);
		IList<ImageIdentifier> LocateImagesBySeries(string studyInstanceUid, string seriesInstanceUid, out LocateFailureInfo[] failures);
	}
}