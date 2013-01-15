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

using System.Collections.Generic;
using System.Xml.Serialization;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Data
{
	/// <summary>
	/// Represents serializable study information.
	/// </summary>
	[XmlRoot("StudyInformation")]
	public class StudyInformation 
	{
		#region Private Fields

		private PatientInformation _patientInfo;
		private List<SeriesInformation> _series;

		#endregion

		#region Constructors
		public StudyInformation()
		{
		}

		public StudyInformation(IDicomAttributeProvider attributeProvider)
		{
			if (attributeProvider[DicomTags.StudyId]!=null)
				StudyId = attributeProvider[DicomTags.StudyId].ToString();
            
			if (attributeProvider[DicomTags.AccessionNumber]!=null)
				AccessionNumber = attributeProvider[DicomTags.AccessionNumber].ToString();

			if (attributeProvider[DicomTags.StudyDate] != null )
				StudyDate = attributeProvider[DicomTags.StudyDate].ToString();

			if (attributeProvider[DicomTags.ModalitiesInStudy] != null)
				Modalities = attributeProvider[DicomTags.ModalitiesInStudy].ToString();

			if (attributeProvider[DicomTags.StudyInstanceUid] != null)
				StudyInstanceUid = attributeProvider[DicomTags.StudyInstanceUid].ToString();

			if (attributeProvider[DicomTags.StudyDescription] != null)
				StudyDescription = attributeProvider[DicomTags.StudyDescription].ToString();


			if (attributeProvider[DicomTags.ReferringPhysiciansName] != null)
				ReferringPhysician = attributeProvider[DicomTags.ReferringPhysiciansName].ToString();

			PatientInfo = new PatientInformation(attributeProvider);

		    DicomAttribute seriesUidAttr;
		    if (attributeProvider.TryGetAttribute(DicomTags.SeriesInstanceUid, out seriesUidAttr))
            {
                SeriesInformation series = new SeriesInformation(attributeProvider);
                Add(series);
            }
			
		}

		#endregion

		#region Public Properties

		public string StudyId { get; set; }

		public string AccessionNumber { get; set; }

		public string StudyDate { get; set; }

		public string StudyTime { get; set; }

		public string Modalities { get; set; }

		public string StudyInstanceUid { get; set; }

		public string StudyDescription { get; set; }


		public string ReferringPhysician { get; set; }

		public PatientInformation PatientInfo
		{
			get { return _patientInfo; }
			set { _patientInfo = value; }
		}

		[XmlArray("Series")]
		public List<SeriesInformation> Series
		{
			get { return _series;}
			set { _series = value; }
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Adds a <see cref="SeriesInformation"/> data
		/// </summary>
		/// <param name="message"></param>
		public void Add(DicomMessageBase message)
		{
            if (PatientInfo==null)
            {
                PatientInfo = new PatientInformation(message.DataSet);
            }

            if (Series == null)
                Series = new List<SeriesInformation>();

			string seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].ToString();
			SeriesInformation theSeries = Series.Find(ser => ser.SeriesInstanceUid == seriesInstanceUid);
			if (theSeries==null)
			{
				SeriesInformation newSeries = new SeriesInformation(message.DataSet) {NumberOfInstances = 1};
				Series.Add(newSeries);
			}
			else
			{
				theSeries.NumberOfInstances++;
			}
		}

		public void Add(SeriesInformation series)
		{
            if (Series == null)
                Series = new List<SeriesInformation>();

			Series.Add(series);
		}

		#endregion

		#region Public Static Methods
		public static StudyInformation CreateFrom(Study study)
		{
			ServerEntityAttributeProvider studyWrapper = new ServerEntityAttributeProvider(study);
			StudyInformation studyInfo = new StudyInformation(studyWrapper);

			foreach(Series series in study.Series.Values)
			{
				ServerEntityAttributeProvider seriesWrapper = new ServerEntityAttributeProvider(series);
				SeriesInformation seriesInfo = new SeriesInformation(seriesWrapper);
				studyInfo.Add(seriesInfo);
			}

			return studyInfo;
		}
		#endregion
	}
}