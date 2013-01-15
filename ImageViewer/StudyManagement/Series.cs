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
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM series.
	/// </summary>
	public class Series : ISeriesData
	{
		private Sop _sop;
		private readonly Study _parentStudy;
		private SopCollection _sops;

		internal Series(Study parentStudy)
		{
			_parentStudy = parentStudy;
		}

		/// <summary>
		/// Gets the parent <see cref="Study"/>.
		/// </summary>
		public Study ParentStudy
		{
			get { return _parentStudy; }
		}

		/// <summary>
		/// Gets the collection of <see cref="Sop"/> objects that belong
		/// to this <see cref="Study"/>.
		/// </summary>
		public SopCollection Sops
		{
			get 
			{
				if (_sops == null)
					_sops = new SopCollection();

				return _sops; 
			}
		}

		#region ISeriesData Members

		/// <summary>
		/// Gets the Study Instance UID of the identified series.
		/// </summary>
		public string StudyInstanceUid
		{
			get { return _sop.StudyInstanceUid; }
		}

		/// <summary>
		/// Gets the Series Instance UID of the identified series.
		/// </summary>
		public string SeriesInstanceUid
		{
			get { return _sop.SeriesInstanceUid; }
		}

		/// <summary>
		/// Gets the modality of the identified series.
		/// </summary>
		public string Modality
		{
			get { return _sop.Modality; }
		}

		/// <summary>
		/// Gets the series description of the identified series.
		/// </summary>
		public string SeriesDescription
		{
			get { return _sop.SeriesDescription; }
		}

		/// <summary>
		/// Gets the series number of the identified series.
		/// </summary>
		public int SeriesNumber
		{
			get { return _sop.SeriesNumber; }
		}

		/// <summary>
		/// Gets the number of composite object instances belonging to the identified series.
		/// </summary>
		public int NumberOfSeriesRelatedInstances
		{
			get { return Sops.Count; }
		}

		int? ISeriesData.NumberOfSeriesRelatedInstances
		{
			get { return NumberOfSeriesRelatedInstances; }	
		}

		#endregion

		/// <summary>
		/// Gets the laterality.
		/// </summary>
		public string Laterality 
		{ 
			get { return _sop.Laterality; } 
		}

		/// <summary>
		/// Gets the series date.
		/// </summary>
		public string SeriesDate 
		{ 
			get { return _sop.SeriesDate; } 
		}

		/// <summary>
		/// Gets the series time.
		/// </summary>
		public string SeriesTime 
		{ 
			get { return _sop.SeriesTime; } 
		}

		/// <summary>
		/// Gets the names of performing physicians.
		/// </summary>
		public PersonName[] PerformingPhysiciansName 
		{ 
			get { return _sop.PerformingPhysiciansName; } 
		}

		/// <summary>
		/// Gets the names of operators.
		/// </summary>
		public PersonName[] OperatorsName 
		{ 
			get { return _sop.OperatorsName; } 
		}

		/// <summary>
		/// Gets the body part examined.
		/// </summary>
		public string BodyPartExamined 
		{ 
			get { return _sop.BodyPartExamined; } 
		}

		/// <summary>
		/// Gets the patient position.
		/// </summary>
		public string PatientPosition 
		{ 
			get { return _sop.PatientPosition; } 
		}

		/// <summary>
		/// Gets the manufacturer.
		/// </summary>
		public string Manufacturer 
		{ 
			get { return _sop.Manufacturer; } 
		}

		/// <summary>
		/// Gets the institution name.
		/// </summary>
		public string InstitutionName 
		{ 
			get { return _sop.InstitutionName; } 
		}

		/// <summary>
		/// Gets the station name.
		/// </summary>
		public string StationName 
		{ 
			get { return _sop.StationName; } 
		}

		/// <summary>
		/// Gets the institutional department name.
		/// </summary>
		public string InstitutionalDepartmentName 
		{ 
			get { return _sop.InstitutionalDepartmentName; } 
		}

		/// <summary>
		/// Gets the manufacturer's model name.
		/// </summary>
		public string ManufacturersModelName 
		{ 
			get { return _sop.ManufacturersModelName; }
		}

		/// <summary>
		/// Gets an <see cref="ISeriesIdentifier"/> for this <see cref="Series"/>.
		/// </summary>
		/// <remarks>An <see cref="ISeriesIdentifier"/> can be used in situations where you only
		/// need some data about the <see cref="Series"/>, but not the <see cref="Series"/> itself.  It can be problematic
		/// to hold references to <see cref="Series"/> objects outside the context of an <see cref="IImageViewer"/>
		/// because they are no longer valid when the viewer is closed; in these situations, it may be appropriate to
		/// use an identifier.
		/// </remarks>
		public ISeriesIdentifier GetIdentifier()
		{
			return _sop.GetSeriesIdentifier();
		}

		internal void SetSop(Sop sop)
		{
			if (sop == null)
				_sop = null;
			else if (_sop == null)
				_sop = sop;

			ParentStudy.SetSop(sop);
		}

		/// <summary>
		/// Returns the series description and series instance UID in string form.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("{0} | {1} | {2}", this.SeriesNumber, this.SeriesDescription, this.SeriesInstanceUid);
		}
	}
}
