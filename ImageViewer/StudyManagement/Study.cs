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
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM study.
	/// </summary>
	public class Study : IStudyData
	{
		private Sop _sop;
		private readonly Patient _parentPatient;
		private SeriesCollection _series;

		internal Study(Patient parentPatient)
		{
			_parentPatient = parentPatient;
		}

		/// <summary>
		/// Gets the parent <see cref="Patient"/>.
		/// </summary>
		public Patient ParentPatient
		{
			get { return _parentPatient; }
		}

		/// <summary>
		/// Gets the collection of <see cref="StudyManagement.Series"/> objects that belong
		/// to this <see cref="Study"/>.
		/// </summary>
		public SeriesCollection Series
		{
			get 
			{
				if (_series == null)
					_series = new SeriesCollection();

				return _series; 
			}
		}

		#region IStudyData Members

		/// <summary>
		/// Gets the Study Instance UID of the identified study.
		/// </summary>
		public string StudyInstanceUid
		{
			get { return _sop.StudyInstanceUid; }
		}

        /// <summary>
        /// Gets the modalities in the identified study.
        /// </summary>
        public string[] SopClassesInStudy
        {
            get
            {
                var sopClasses = new List<string>();
                foreach (Series series in this.Series)
                    foreach (var sop in series.Sops)
                    {
                        if (!String.IsNullOrEmpty(sop.SopClassUid) && !sopClasses.Contains(sop.SopClassUid))
                            sopClasses.Add(sop.SopClassUid);
                    }
                return sopClasses.ToArray();
            }
        }

		/// <summary>
		/// Gets the modalities in the identified study.
		/// </summary>
		public string[] ModalitiesInStudy
		{
			get
			{
				var modalities = new List<string>();
				foreach(Series series in this.Series)
				{
					if (!String.IsNullOrEmpty(series.Modality) && !modalities.Contains(series.Modality))
						modalities.Add(series.Modality);
				}
				return modalities.ToArray();
			}	
		}

		/// <summary>
		/// Gets the study description of the identified study.
		/// </summary>
		public string StudyDescription
		{
			get { return _sop.StudyDescription; }
		}

		/// <summary>
		/// Gets the study ID of the identified study.
		/// </summary>
		public string StudyId
		{
			get { return _sop.StudyId; }
		}

		/// <summary>
		/// Gets the study date of the identified study.
		/// </summary>
		public string StudyDate
		{
			get { return _sop.StudyDate; }
		}

		/// <summary>
		/// Gets the study time of the identified study.
		/// </summary>
		public string StudyTime
		{
			get { return _sop.StudyTime; }
		}

		/// <summary>
		/// Gets the accession number of the identified study.
		/// </summary>
		public string AccessionNumber
		{
			get { return _sop.AccessionNumber; }
		}

		string IStudyData.ReferringPhysiciansName
		{
			get { return _sop.ReferringPhysiciansName; }
		}

		/// <summary>
		/// Gets the number of series belonging to the identified study.
		/// </summary>
		public int NumberOfStudyRelatedSeries
		{
			get { return Series.Count; }
		}

		int? IStudyData.NumberOfStudyRelatedSeries
		{
			get { return NumberOfStudyRelatedSeries; }
		}

		/// <summary>
		/// Gets the number of composite object instances belonging to the identified study.
		/// </summary>
		public int NumberOfStudyRelatedInstances
		{
			get
			{
				int count = 0;
				foreach (Series series in Series)
					count += series.NumberOfSeriesRelatedInstances;
				return count;
			}
		}

		int? IStudyData.NumberOfStudyRelatedInstances
		{
			get { return NumberOfStudyRelatedInstances; }	
		}

		#endregion

		/// <summary>
		/// Gets the referring physician's name.
		/// </summary>
		public PersonName ReferringPhysiciansName
		{
			get { return _sop.ReferringPhysiciansName; }
		}

		/// <summary>
		/// Gets the names of physicians reading the study.
		/// </summary>
		public PersonName[] NameOfPhysiciansReadingStudy
		{
			get { return _sop.NameOfPhysiciansReadingStudy; }
		}

		/// <summary>
		/// Gets the patient's age at the time of the study.
		/// </summary>
		public string PatientsAge
		{
			get { return _sop.PatientsAge; }
		}

		/// <summary>
		/// Gets the admitting diagnoses descriptions.
		/// </summary>
		public string[] AdmittingDiagnosesDescription
		{
			get { return _sop.AdmittingDiagnosesDescription; }
		}

		/// <summary>
		/// Gets the additional patient's history.
		/// </summary>
		public string AdditionalPatientsHistory
		{
			get { return _sop.AdditionalPatientsHistory; }
		}

		/// <summary>
		/// Gets an <see cref="IStudyRootStudyIdentifier"/> for this <see cref="Study"/>.
		/// </summary>
		/// <remarks>An <see cref="IStudyRootStudyIdentifier"/> can be used in situations where you only
		/// need some data about the <see cref="Study"/>, but not the <see cref="Study"/> itself.  It can be problematic
		/// to hold references to <see cref="Study"/> objects outside the context of an <see cref="IImageViewer"/>
		/// because they are no longer valid when the viewer is closed; in these situations, it may be appropriate to
		/// use an identifier.
		/// </remarks>
		public IStudyRootStudyIdentifier GetIdentifier()
		{
			return _sop.GetStudyIdentifier();
		}

		/// <summary>
		/// Returns the study description and study instance UID associated with
		/// the <see cref="Study"/> in string form.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("{0} | {1}", this.StudyDescription, this.StudyInstanceUid);
		}

		internal void SetSop(Sop sop)
		{
			if (sop == null)
				_sop = null;
			else if (_sop == null)
				_sop = sop;

			this.ParentPatient.SetSop(sop);
		}
	}
}
