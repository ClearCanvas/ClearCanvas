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

namespace ClearCanvas.ImageViewer.Common.Automation
{
	/// <summary>
	/// Specifies how the ImageViewer should behave when opening a study for display.
	/// </summary>
	public class OpenStudiesBehaviour
	{
		internal OpenStudiesBehaviour()
		{
			ActivateExistingViewer = true;
		}

		/// <summary>
		/// Specifies whether or not an existing viewer should be reactivated if the specified study is the primary study in that instance.
		/// </summary>
		public bool ActivateExistingViewer { get; set; }

		/// <summary>
		/// Specifies whether or not errors should be reported to the user.
		/// </summary>
		public bool ReportFaultToUser { get; set; }
	}

	/// <summary>
	/// Exception thrown when a query fails via <see cref="IStudyRootQuery"/>.
	/// </summary>
	public class QueryNoMatchesException : Exception
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public QueryNoMatchesException(string message)
			: base(message)
		{
		}
	}

	/// <summary>
	/// Interface for bridge to <see cref="IViewerAutomation"/>.
	/// </summary>
	/// <remarks>
	/// The bridge design pattern allows the public interface (<see cref="IViewerAutomationBridge"/>) and it's
	/// underlying implementation <see cref="IViewerAutomation"/> to vary independently.
	/// </remarks>
	public interface IViewerAutomationBridge : IDisposable
	{
		/// <summary>
		/// Specifies how the ImageViewer should behave when opening a study for display.
		/// </summary>
		OpenStudiesBehaviour OpenStudiesBehaviour { get; }

		/// <summary>
		/// Comparer used to sort results from <see cref="IStudyRootQuery.StudyQuery"/>.
		/// </summary>
		IComparer<StudyRootStudyIdentifier> StudyComparer { get; set; }

		/// <summary>
		/// Gets all the active <see cref="Viewer"/>s.
		/// </summary>
		/// <returns></returns>
		IList<Viewer> GetViewers();

		/// <summary>
		/// Gets all the active viewers having the given primary study instance uid.
		/// </summary>
		IList<Viewer> GetViewers(string primaryStudyInstanceUid);

		/// <summary>
		/// Gets all the active viewers where the primary study has the given accession number.
		/// </summary>
		IList<Viewer> GetViewersByAccessionNumber(string accessionNumber);

		/// <summary>
		/// Gets all the active viewers where the primary study has the given patient id.
		/// </summary>
		IList<Viewer> GetViewersByPatientId(string patientId);

		/// <summary>
		/// Opens the given study.
		/// </summary>
		Viewer OpenStudy(string studyInstanceUid);

		/// <summary>
		/// Opens the given studies.
		/// </summary>
		Viewer OpenStudies(IEnumerable<string> studyInstanceUids);

		/// <summary>
		/// Opens the given study.
		/// </summary>
		Viewer OpenStudies(List<OpenStudyInfo> studiesToOpen);
				
		/// <summary>
		/// Opens all studies matching the given <b>exact</b> accession number.
		/// </summary>
		Viewer OpenStudiesByAccessionNumber(string accessionNumber);

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> accession numbers.
		/// </summary>
		Viewer OpenStudiesByAccessionNumber(IEnumerable<string> accessionNumbers);

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> patient id.
		/// </summary>
		Viewer OpenStudiesByPatientId(string patientId);

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> patient ids.
		/// </summary>
		Viewer OpenStudiesByPatientId(IEnumerable<string> patientIds);

		/// <summary>
		/// Activates the given <see cref="Viewer"/>.
		/// </summary>
		void ActivateViewer(Viewer viewer);

		/// <summary>
		/// Closes the given <see cref="Viewer"/>.
		/// </summary>
		/// <param name="viewer"></param>
		void CloseViewer(Viewer viewer);

		/// <summary>
		/// Gets additional studies, not including the primary one, for the given <see cref="Viewer"/>.
		/// </summary>
		IList<string> GetViewerAdditionalStudies(Viewer viewer);
	}
}