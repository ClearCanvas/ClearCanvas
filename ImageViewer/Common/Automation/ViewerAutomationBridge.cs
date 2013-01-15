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
using System.Linq;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.Common.Automation
{
	/// <summary>
	/// Default implementation of <see cref="IViewerAutomationBridge"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The bridge design pattern allows the public interface (<see cref="IViewerAutomationBridge"/>) and it's
	/// underlying implementation <see cref="IViewerAutomation"/> to vary independently.
	/// </para>
	/// <para>
	/// This is the default implementation of <see cref="IViewerAutomationBridge"/>, which uses both the
	/// <see cref="IStudyRootQuery"/> and <see cref="IViewerAutomation"/> interfaces to drive automation.
	/// The bridge pattern allows the <see cref="IViewerAutomation"/> (and in this case, also the <see cref="IStudyRootQuery"/>)
	/// implementor to be provided by the client through the constructors.
	/// </para>
	/// </remarks>
	public class ViewerAutomationBridge : IViewerAutomationBridge
	{
		private IViewerAutomation _viewerAutomationClient;
		private IStudyRootQueryBridge _studyRootQueryBridge;
		private IStudyLocatorBridge _studyLocatorBridge;
		
		private readonly OpenStudiesBehaviour _openStudiesBehaviour = new OpenStudiesBehaviour();
		private IComparer<StudyRootStudyIdentifier> _studyComparer;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ViewerAutomationBridge(IViewerAutomation viewerAutomationClient, IStudyRootQuery studyRootQueryClient)
			: this(viewerAutomationClient, new StudyRootQueryBridge(studyRootQueryClient))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ViewerAutomationBridge(IViewerAutomation viewerAutomationClient, IStudyRootQueryBridge studyRootQueryBridge)
		{
			Platform.CheckForNullReference(viewerAutomationClient, "viewerAutomationClient");
			Platform.CheckForNullReference(studyRootQueryBridge, "studyRootQueryBridge");

			_viewerAutomationClient = viewerAutomationClient;
			_studyRootQueryBridge = studyRootQueryBridge;

			_studyComparer = new StudyDateTimeComparer();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ViewerAutomationBridge(IViewerAutomation viewerAutomationClient, IStudyLocator studyLocator)
			: this(viewerAutomationClient, new StudyLocatorBridge(studyLocator))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ViewerAutomationBridge(IViewerAutomation viewerAutomationClient, IStudyLocatorBridge studyLocatorBridge)
		{
			Platform.CheckForNullReference(viewerAutomationClient, "viewerAutomationClient");
			Platform.CheckForNullReference(studyLocatorBridge, "studyLocatorBridge");

			_viewerAutomationClient = viewerAutomationClient;
			_studyLocatorBridge = studyLocatorBridge;

			_studyComparer = new StudyDateTimeComparer();
		}

		/// <summary>
		/// Gets the underlying <see cref="IViewerAutomation"/> client.
		/// </summary>
		protected IViewerAutomation ViewerAutomationClient
		{
			get { return _viewerAutomationClient; }	
		}

		/// <summary>
		/// Gets the underlying <see cref="IStudyRootQueryBridge"/>.
		/// </summary>
		protected IStudyRootQueryBridge StudyRootQueryBridge
		{
			get { return _studyRootQueryBridge; }
		}

		/// <summary>
		/// Gets the underlying <see cref="IStudyLocatorBridge"/>.
		/// </summary>
		protected IStudyLocatorBridge StudyLocatorBridge
		{
			get { return _studyLocatorBridge; }
		}

		/// <summary>
		/// Comparer used to sort results from the study query.
		/// </summary>
		public IComparer<StudyRootStudyIdentifier> StudyComparer
		{
			get { return _studyComparer; }
			set { _studyComparer = value; }
		}

		#region IViewerAutomationBridge Members

		/// <summary>
		/// Specifies how the ImageViewer should behave when opening a study for display.
		/// </summary>
		public OpenStudiesBehaviour OpenStudiesBehaviour
		{
			get { return _openStudiesBehaviour; }
		}

		/// <summary>
		/// Gets all the active <see cref="Viewer"/>s.
		/// </summary>
		/// <returns></returns>
		public IList<Viewer> GetViewers()
		{
			return _viewerAutomationClient.GetViewers(new GetViewersRequest()).Viewers;
		}

		/// <summary>
		/// Gets all the active viewers having the given primary study instance uid.
		/// </summary>
		public IList<Viewer> GetViewers(string primaryStudyInstanceUid)
		{
			Platform.CheckForEmptyString(primaryStudyInstanceUid, "primaryStudyInstanceUid");
			return GetViewers().Where(viewer => viewer.PrimaryStudyInstanceUid == primaryStudyInstanceUid).ToList();
		}

		/// <summary>
		/// Gets all the active viewers where the primary study has the given accession number.
		/// </summary>
		/// <remarks>
		/// The accession number is matched using a case-insensitive comparison.
		/// </remarks>
		public IList<Viewer> GetViewersByAccessionNumber(string accessionNumber)
		{
			return GetViewers().Where(viewer => viewer.PrimaryStudyIdentifier != null && string.Equals(viewer.PrimaryStudyIdentifier.AccessionNumber, accessionNumber, StringComparison.InvariantCultureIgnoreCase)).ToList();
		}

		/// <summary>
		/// Gets all the active viewers where the primary study has the given patient id.
		/// </summary>
		/// <remarks>
		/// The patient ID is matched using a case-insensitive comparison. This method does not support reconciled patient IDs, and must match the original patient ID.
		/// </remarks>
		public IList<Viewer> GetViewersByPatientId(string patientId)
		{
			// TODO CR (Oct 12): Add support for patient reconciliation (after we move that concept into viewer common)
			return GetViewers().Where(viewer => viewer.PrimaryStudyIdentifier != null && string.Equals(viewer.PrimaryStudyIdentifier.PatientId, patientId, StringComparison.InvariantCultureIgnoreCase)).ToList();
		}

		/// <summary>
		/// Opens the given study.
		/// </summary>
		public Viewer OpenStudy(string studyInstanceUid)
		{
			return OpenStudies(new string[] { studyInstanceUid });
		}

		/// <summary>
		/// Opens the given studies.
		/// </summary>
		public Viewer OpenStudies(IEnumerable<string> studyInstanceUids)
		{
			if (_studyLocatorBridge != null)
			{
				LocateFailureInfo[] failures;
				var results = _studyLocatorBridge.LocateStudyByInstanceUid(studyInstanceUids, out failures);
				return OpenStudies(results);
			}

			IList<StudyRootStudyIdentifier> studies = _studyRootQueryBridge.QueryByStudyInstanceUid(studyInstanceUids);
			return OpenStudies(studies);
		}

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> accession number.
		/// </summary>
		public Viewer OpenStudiesByAccessionNumber(string accessionNumber)
		{
			return OpenStudiesByAccessionNumber(new string[] {accessionNumber });
		}

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> accession numbers.
		/// </summary>
		public Viewer OpenStudiesByAccessionNumber(IEnumerable<string> accessionNumbers)
		{
			if (_studyLocatorBridge != null)
			{
				LocateFailureInfo[] failures;
				var results = accessionNumbers.SelectMany(accessionNumber => _studyLocatorBridge.LocateStudyByAccessionNumber(accessionNumber, out failures)).ToList();
				return OpenStudies(results);
			}

			var studies = new List<StudyRootStudyIdentifier>();

			foreach (string accessionNumber in accessionNumbers)
				studies.AddRange(_studyRootQueryBridge.QueryByAccessionNumber(accessionNumber));

			return OpenStudies(studies);
		}

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> patient id.
		/// </summary>
		public Viewer OpenStudiesByPatientId(string patientId)
		{
			return OpenStudiesByPatientId(new string[] { patientId });
		}

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> patient ids.
		/// </summary>
		public Viewer OpenStudiesByPatientId(IEnumerable<string> patientIds)
		{
			if (_studyLocatorBridge != null)
			{
				LocateFailureInfo[] failures;
				var results = patientIds.SelectMany(patientId => _studyLocatorBridge.LocateStudyByPatientId(patientId, out failures)).ToList();
				return OpenStudies(results);
			}

			var studies = new List<StudyRootStudyIdentifier>();

			foreach (string patientId in patientIds)
				studies.AddRange(_studyRootQueryBridge.QueryByPatientId(patientId));

			return OpenStudies(studies);
		}

		/// <summary>
		/// Opens the given study.
		/// </summary>
		public Viewer OpenStudies(List<OpenStudyInfo> studiesToOpen)
		{
		    var request = new OpenStudiesRequest
		                      {
		                          ActivateIfAlreadyOpen = _openStudiesBehaviour.ActivateExistingViewer,
		                          ReportFaultToUser = _openStudiesBehaviour.ReportFaultToUser,
		                          StudiesToOpen = studiesToOpen
		                      };

		    return _viewerAutomationClient.OpenStudies(request).Viewer;
		}

		/// <summary>
		/// Activates the given <see cref="Viewer"/>.
		/// </summary>
		public void ActivateViewer(Viewer viewer)
		{
		    var request = new ActivateViewerRequest {Viewer = viewer};
		    _viewerAutomationClient.ActivateViewer(request);
		}

		/// <summary>
		/// Closes the given <see cref="Viewer"/>.
		/// </summary>
		/// <param name="viewer"></param>
		public void CloseViewer(Viewer viewer)
		{
		    var request = new CloseViewerRequest {Viewer = viewer};
		    _viewerAutomationClient.CloseViewer(request);
		}

		/// <summary>
		/// Gets additional studies, not including the primary one, for the given <see cref="Viewer"/>.
		/// </summary>
		public IList<string> GetViewerAdditionalStudies(Viewer viewer)
		{
			var request = new GetViewerInfoRequest();
			return _viewerAutomationClient.GetViewerInfo(request).AdditionalStudyInstanceUids;
		}

		#endregion

		private Viewer OpenStudies(IList<StudyRootStudyIdentifier> studiesToOpen)
		{
			CheckAtLeastOneStudy(studiesToOpen);

			if (_studyComparer != null)
				studiesToOpen = CollectionUtils.Sort(studiesToOpen, _studyComparer.Compare);

			return OpenStudies(Convert(studiesToOpen));
		}

		private static void CheckAtLeastOneStudy(IList<StudyRootStudyIdentifier> studies)
		{
			if (studies.Count == 0)
				throw new QueryNoMatchesException("No studies were found matching the given criteria.");
		}

		private IList<Viewer> GetViewers(IEnumerable<StudyRootStudyIdentifier> studies)
		{
			var studyInstanceUids = studies.Select(identifier => identifier.StudyInstanceUid).ToList();
			return GetViewers().Where(viewer => studyInstanceUids.Contains(viewer.PrimaryStudyInstanceUid)).ToList();
		}

		private static List<OpenStudyInfo> Convert(IEnumerable<StudyRootStudyIdentifier> studyIdentifiers)
		{
		    return studyIdentifiers.Select(identifier => new OpenStudyInfo(identifier)).ToList();
		}

		/// <summary>
		/// Implementation of the Dispose pattern.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_viewerAutomationClient != null && _viewerAutomationClient is IDisposable)
				{
					((IDisposable)_viewerAutomationClient).Dispose();
					_viewerAutomationClient = null;
				}

				if (_studyRootQueryBridge != null)
				{
					_studyRootQueryBridge.Dispose();
					_studyRootQueryBridge = null;
				}

				if (_studyLocatorBridge != null)
				{
					_studyLocatorBridge.Dispose();
					_studyLocatorBridge = null;
				}
			}
		}

		#region IDisposable Members

		/// <summary>
		/// Disposes this instance.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (CommunicationException)
			{
				//connection already closed.
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e);
			}
		}

		#endregion

	}
}
