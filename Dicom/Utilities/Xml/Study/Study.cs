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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.Utilities.Xml.Study
{

	/// <summary>
	/// Represents an <see cref="IStudy"/> whose main source of data is a <see cref="StudyXml"/> document.
	/// </summary>
	public class Study : IStudy
	{
		private class SeriesCollection : ISeriesCollection
		{
			private readonly Study _owner;

			public SeriesCollection(Study owner)
			{
				_owner = owner;
			}

			public IEnumerator<ISeries> GetEnumerator()
			{
				return Xml.Select(_owner.GetSeries).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public int Count
			{
				get { return Xml.NumberOfStudyRelatedSeries; }
			}

			public bool Contains(string seriesInstanceUid)
			{
				return Xml.Contains(seriesInstanceUid);
			}

			public ISeries Get(string seriesInstanceUid)
			{
				var seriesXml = Xml[seriesInstanceUid];
				if(seriesXml == null)
					throw new ArgumentException("Invalid value for series instance UID.");

				return _owner.GetSeries(seriesXml);
			}

			public ISeries this[string seriesInstanceUid]
			{
				get { return Get(seriesInstanceUid); }
			}

			public bool TryGet(string seriesInstanceUid, out ISeries series)
			{
				var seriesXml = Xml[seriesInstanceUid];
				if(seriesXml != null)
				{
					series = _owner.GetSeries(seriesXml);
					return true;
				}
				series = null;
				return false;
			}

			private StudyXml Xml
			{
				get { return _owner._xml; }
			}
		}



		private readonly string _studyInstanceUid;
		private readonly StudyXml _xml;
		private readonly IDicomFileLoader _dicomFileLoader;
		private readonly ISeries _firstSeries;
		private readonly ISopInstance _firstSopInstance;
		private readonly SeriesCollection _seriesCollection;

		public Study(string studyInstanceUid, StudyXml xml, IDicomFileLoader dicomFileLoader)
		{
			_studyInstanceUid = studyInstanceUid;
			_xml = xml;

			_firstSeries = GetSeries(xml.First());
			_firstSopInstance = _firstSeries.SopInstances.First();
			_dicomFileLoader = dicomFileLoader;
			_seriesCollection = new SeriesCollection(this);
		}

		public IDicomFileLoader DicomFileLoader
		{
			get { return _dicomFileLoader; }
		}

		#region Implementation of IStudy

		public ISeriesCollection Series
		{
			get { return _seriesCollection; }
		}

		public ISopInstance FirstSopInstance
		{
			get { return _firstSopInstance; }
		}

		public DateTime? StudyDate
		{
			get { return DateParser.Parse(_firstSopInstance.GetAttribute(DicomTags.StudyDate).ToString()); }
		}

		public TimeSpan? StudyTime
		{
			get
			{
				var time = TimeParser.Parse(_firstSopInstance.GetAttribute(DicomTags.StudyTime).ToString());
				if (time.HasValue)
					return time.Value.TimeOfDay;

				return null;
			}
		}

		#endregion

		#region IStudyData Members

		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
		}

		public string[] SopClassesInStudy
		{
			get
			{
				return (from series in _xml
						from sop in series
						select sop.SopClass.Uid).Distinct().ToArray();
			}
		}

		public string[] ModalitiesInStudy
		{
			get { return _xml.Select(series => series.First()[DicomTags.Modality].ToString()).Distinct().OrderBy(s => s).ToArray(); }
		}

		public string StudyDescription
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.StudyDescription).ToString(); }
		}

		public string StudyId
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.StudyId).ToString(); }
		}

		string IStudyData.StudyDate
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.StudyDate).ToString(); }
		}

		string IStudyData.StudyTime
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.StudyTime).ToString(); }
		}

		public string AccessionNumber
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.AccessionNumber).ToString(); }
		}

		public string PatientsAge
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.PatientsAge).ToString(); }
		}

		public string ReferringPhysiciansName
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.ReferringPhysiciansName).ToString(); }
		}

		public int? NumberOfStudyRelatedSeries
		{
			get { return _xml.NumberOfStudyRelatedSeries; }
		}

		public int? NumberOfStudyRelatedInstances
		{
			get { return _xml.NumberOfStudyRelatedInstances; }
		}

		#endregion

		#region IPatientData Members

		public string PatientId
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.PatientId).ToString(); }
		}

		public string PatientsName
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.PatientsName).ToString(); }
		}

		public DateTime? PatientsBirthDate
		{
			get { return DateParser.Parse(_firstSopInstance.GetAttribute(DicomTags.PatientsBirthDate).ToString()); }
		}

		string IPatientData.PatientsBirthDate
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.PatientsBirthDate).ToString(); }
		}

		public TimeSpan? PatientsBirthTime
		{
			get
			{
				var time = TimeParser.Parse(_firstSopInstance.GetAttribute(DicomTags.PatientsBirthTime).ToString());
				if (time.HasValue)
					return time.Value.TimeOfDay;

				return null;
			}
		}

		string IPatientData.PatientsBirthTime
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.PatientsBirthTime).ToString(); }
		}

		public string PatientsSex
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.PatientsSex).ToString(); }
		}

		public string PatientSpeciesDescription
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.PatientSpeciesDescription).ToString(); }
		}

		public string PatientSpeciesCodeSequenceCodingSchemeDesignator
		{
			get { return GetSequenceValue(DicomTags.PatientSpeciesCodeSequence, DicomTags.CodingSchemeDesignator); }
		}

		public string PatientSpeciesCodeSequenceCodeValue
		{
			get { return GetSequenceValue(DicomTags.PatientSpeciesCodeSequence, DicomTags.CodeValue); }
		}

		public string PatientSpeciesCodeSequenceCodeMeaning
		{
			get { return GetSequenceValue(DicomTags.PatientSpeciesCodeSequence, DicomTags.CodeMeaning); }
		}

		public string PatientBreedDescription
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.PatientBreedDescription).ToString(); }
		}

		public string PatientBreedCodeSequenceCodingSchemeDesignator
		{
			get { return GetSequenceValue(DicomTags.PatientBreedCodeSequence, DicomTags.CodingSchemeDesignator); }

		}

		public string PatientBreedCodeSequenceCodeValue
		{
			get { return GetSequenceValue(DicomTags.PatientBreedCodeSequence, DicomTags.CodeValue); }
		}

		public string PatientBreedCodeSequenceCodeMeaning
		{
			get { return GetSequenceValue(DicomTags.PatientBreedCodeSequence, DicomTags.CodeMeaning); }
		}

		public string ResponsiblePerson
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.ResponsiblePerson).ToString(); }
		}

		public string ResponsiblePersonRole
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.ResponsiblePersonRole).ToString(); }
		}

		public string ResponsibleOrganization
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.ResponsibleOrganization).ToString(); }
		}

		#endregion

		private Series GetSeries(SeriesXml xml)
		{
			return new Series(xml, this, _dicomFileLoader);
		}

		private string GetSequenceValue(uint sequenceTag, uint itemTag)
		{
			var sequence = _firstSopInstance.GetAttribute(sequenceTag) as DicomAttributeSQ;
			if (sequence == null)
				return String.Empty;

			var item = sequence[0];
			if (item == null)
				return String.Empty;

			return item[itemTag].ToString();
		}
	}
}