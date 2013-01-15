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
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
	public class StudyLocatorBridge : IStudyLocatorBridge
	{
		private IStudyLocator _client;

		private IComparer<StudyRootStudyIdentifier> _studyComparer;
		private IComparer<SeriesIdentifier> _seriesComparer;
		private IComparer<ImageIdentifier> _imageComparer;

		public StudyLocatorBridge(IStudyLocator client)
		{
			Platform.CheckForNullReference(client, "client");
			_client = client;
			_studyComparer = new StudyDateTimeComparer();
		}

		/// <summary>
		/// Comparer used to sort the results returned from <see cref="IStudyLocator.LocateStudies"/>.
		/// </summary>
		public IComparer<StudyRootStudyIdentifier> StudyComparer
		{
			get { return _studyComparer; }
			set { _studyComparer = value; }
		}

		/// <summary>
		/// Comparer used to sort the results returned from <see cref="IStudyLocator.LocateSeries"/>.
		/// </summary>
		public IComparer<SeriesIdentifier> SeriesComparer
		{
			get { return _seriesComparer; }
			set { _seriesComparer = value; }
		}

		/// <summary>
		/// Comparer used to sort the results returned from <see cref="IStudyLocator.LocateImages"/>.
		/// </summary>
		public IComparer<ImageIdentifier> ImageComparer
		{
			get { return _imageComparer; }
			set { _imageComparer = value; }
		}

		public IList<StudyRootStudyIdentifier> LocateStudyByAccessionNumber(string accessionNumber)
		{
			LocateFailureInfo[] failures;
			var results = LocateStudyByAccessionNumber(accessionNumber, out failures);
			if (failures != null && failures.Length > 0)
			{
				var fault = failures.First().Fault;
				if (fault != null)
					throw new FaultException<QueryFailedFault>(fault, fault.Description);
			}
			return results;
		}

		public IList<StudyRootStudyIdentifier> LocateStudyByAccessionNumber(string accessionNumber, out LocateFailureInfo[] failures)
		{
			Platform.CheckForEmptyString(accessionNumber, "accessionNumber");
			if (accessionNumber.Contains("*") || accessionNumber.Contains("?"))
				throw new ArgumentException("Accession Number cannot contain wildcard characters.");

			var criteria = new StudyRootStudyIdentifier {AccessionNumber = accessionNumber};
			var result = LocateStudies(new LocateStudiesRequest {Criteria = criteria});
			failures = result.Failures.ToArray();
			return result.Studies;
		}

		public IList<StudyRootStudyIdentifier> LocateStudyByPatientId(string patientId)
		{
			LocateFailureInfo[] failures;
			var results = LocateStudyByPatientId(patientId, out failures);
			if (failures != null && failures.Length > 0)
			{
				var fault = failures.First().Fault;
				if (fault != null)
					throw new FaultException<QueryFailedFault>(fault, fault.Description);
			}
			return results;
		}

		public IList<StudyRootStudyIdentifier> LocateStudyByPatientId(string patientId, out LocateFailureInfo[] failures)
		{
			Platform.CheckForEmptyString(patientId, "patientId");
			if (patientId.Contains("*") || patientId.Contains("?"))
				throw new ArgumentException("Patient Id cannot contain wildcard characters.");

			var criteria = new StudyRootStudyIdentifier {PatientId = patientId};
			var result = LocateStudies(new LocateStudiesRequest {Criteria = criteria});
			failures = result.Failures.ToArray();
			return result.Studies;
		}

		public IList<StudyRootStudyIdentifier> LocateStudyByInstanceUid(string studyInstanceUid)
		{
			return LocateStudyByInstanceUid(new[] {studyInstanceUid});
		}

		public IList<StudyRootStudyIdentifier> LocateStudyByInstanceUid(string studyInstanceUid, out LocateFailureInfo[] failures)
		{
			return LocateStudyByInstanceUid(new[] {studyInstanceUid}, out failures);
		}

		public IList<StudyRootStudyIdentifier> LocateStudyByInstanceUid(IEnumerable<string> studyInstanceUids)
		{
			LocateFailureInfo[] failures;
			var results = LocateStudyByInstanceUid(studyInstanceUids, out failures);
			if (failures != null && failures.Length > 0)
			{
				var fault = failures.First().Fault;
				if (fault != null)
					throw new FaultException<QueryFailedFault>(fault, fault.Description);
			}
			return results;
		}

		public IList<StudyRootStudyIdentifier> LocateStudyByInstanceUid(IEnumerable<string> studyInstanceUids, out LocateFailureInfo[] failures)
		{
			var instanceUids = studyInstanceUids.ToArray();
			foreach (string studyInstanceUid in instanceUids)
			{
				Platform.CheckForEmptyString(studyInstanceUid, "studyInstanceUid");

				if (studyInstanceUid.Contains("*") || studyInstanceUid.Contains("?"))
					throw new ArgumentException("Study Instance Uid cannot contain wildcard characters.");
			}

			var criteria = new StudyRootStudyIdentifier {StudyInstanceUid = DicomStringHelper.GetDicomStringArray(instanceUids)};
			var result = LocateStudies(new LocateStudiesRequest {Criteria = criteria});
			failures = result.Failures.ToArray();
			return result.Studies;
		}

		public IList<SeriesIdentifier> LocateSeriesByStudy(string studyInstanceUid)
		{
			LocateFailureInfo[] failures;
			var results = LocateSeriesByStudy(studyInstanceUid, out failures);
			if (failures != null && failures.Length > 0)
			{
				var fault = failures.First().Fault;
				if (fault != null)
					throw new FaultException<QueryFailedFault>(fault, fault.Description);
			}
			return results;
		}

		public IList<SeriesIdentifier> LocateSeriesByStudy(string studyInstanceUid, out LocateFailureInfo[] failures)
		{
			Platform.CheckForEmptyString(studyInstanceUid, "studyInstanceUid");

			var criteria = new SeriesIdentifier {StudyInstanceUid = studyInstanceUid};
			var result = LocateSeries(new LocateSeriesRequest {Criteria = criteria});
			failures = result.Failures.ToArray();
			return result.Series;
		}

		public IList<ImageIdentifier> LocateImagesBySeries(string studyInstanceUid, string seriesInstanceUid)
		{
			LocateFailureInfo[] failures;
			var results = LocateImagesBySeries(studyInstanceUid, seriesInstanceUid, out failures);
			if (failures != null && failures.Length > 0)
			{
				var fault = failures.First().Fault;
				if (fault != null)
					throw new FaultException<QueryFailedFault>(fault, fault.Description);
			}
			return results;
		}

		public IList<ImageIdentifier> LocateImagesBySeries(string studyInstanceUid, string seriesInstanceUid, out LocateFailureInfo[] failures)
		{
			Platform.CheckForEmptyString(studyInstanceUid, "studyInstanceUid");
			Platform.CheckForEmptyString(seriesInstanceUid, "seriesInstanceUid");

			var criteria = new ImageIdentifier {StudyInstanceUid = studyInstanceUid, SeriesInstanceUid = seriesInstanceUid};
			var result = LocateImages(new LocateImagesRequest {Criteria = criteria});
			failures = result.Failures.ToArray();
			return result.Images;
		}

		#region IStudyLocator Members

		public LocateStudiesResult LocateStudies(LocateStudiesRequest queryCriteria)
		{
			var result = _client.LocateStudies(queryCriteria);
			if (_studyComparer != null)
				result.Studies = CollectionUtils.Sort(result.Studies, _studyComparer.Compare);
			return result;
		}

		public LocateSeriesResult LocateSeries(LocateSeriesRequest queryCriteria)
		{
			var result = _client.LocateSeries(queryCriteria);
			if (_seriesComparer != null)
				result.Series = CollectionUtils.Sort(result.Series, _seriesComparer.Compare);
			return result;
		}

		public LocateImagesResult LocateImages(LocateImagesRequest queryCriteria)
		{
			var result = _client.LocateImages(queryCriteria);
			if (_imageComparer != null)
				result.Images = CollectionUtils.Sort(result.Images, _imageComparer.Compare);
			return result;
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				var disposable = _client as IDisposable;
				if (disposable != null) (disposable).Dispose();
				_client = null;
			}
		}

		#region IDisposable Members

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
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion
	}
}