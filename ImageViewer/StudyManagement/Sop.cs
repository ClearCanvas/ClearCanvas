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
using System.Collections.ObjectModel;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Validation;

namespace ClearCanvas.ImageViewer.StudyManagement
{

	/// <summary>
	/// A DICOM SOP Instance.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Note that there should no longer be any need to derive from this class; the <see cref="Sop"/>, <see cref="ImageSop"/>
	/// and <see cref="Frame"/> classes are now just simple Bridge classes (see Bridge Design Pattern)
	/// to <see cref="ISopDataSource"/> and <see cref="ISopFrameData"/>.  See the
	/// remarks for <see cref="ISopDataSource"/> for more information.
	/// </para>
	/// <para>Also, for more information on 'transient references' and the lifetime of <see cref="Sop"/>s,
	/// see <see cref="ISopReference"/>.
	/// </para>
	/// </remarks>
	public partial class Sop : IDisposable, IDicomAttributeProvider, ISopInstanceData, ISeriesData, IStudyData, IPatientData
	{
        //Cached property values where the returned object will be immutable.
        private class CachedValues
        {
            public string[] SpecificCharacterSet;
            public string TransferSyntaxUid;
            public string StudyInstanceUid;
            public string SeriesInstanceUid;
            public string SopInstanceUid;
            public string SopClassUid;
            public int? InstanceNumber;

            public string ContentDate;
            public string ContentTime;

            public string PatientsName;
            public string PatientId;
            public string PatientsBirthDate;
            public string PatientsBirthTime;
            public string PatientsSex;
            public string PatientsAge;
            public string PatientSpeciesDescription;

            public string StudyDate;
            public string StudyTime;
            public string StudyDescription;
            public string AccessionNumber;
            public string StudyId;
            public string ReferringPhysiciansName;

            public string Modality;
            public int? SeriesNumber;
            public string Laterality;
            public string SeriesDate;
            public string SeriesTime;

            public string ProtocolName;
            public string SeriesDescription;
            public string Manufacturer;
            public string ManufacturersModelName;
            public string InstitutionName;
            public string InstitutionalDepartmentName;
            public string StationName;

            public string BodyPartExamined;
            public string PatientPosition;
        }

		private volatile Series _parentSeries;
		private ISopDataSource _dataSource;
	    private CachedValues _cache = new CachedValues();

		/// <summary>
		/// Creates a new instance of <see cref="Sop"/> from a local file.
		/// </summary>
		/// <param name="filename">The path to a local DICOM Part 10 file.</param>
		public Sop(string filename)
		{
			ISopDataSource dataSource = new LocalSopDataSource(filename);
			try
			{
				Initialize(dataSource);
			}
			catch
			{
				dataSource.Dispose();
				throw;
			}
		}

		/// <summary>
		/// Creates a new instance of <see cref="Sop"/>.
		/// </summary>
		public Sop(ISopDataSource dataSource)
		{
			Initialize(dataSource);
		}

		private void Initialize(ISopDataSource dataSource)
		{
			//We want to explicitly enforce that image data sources are wrapped in ImageSops.
			IsImage = this is ImageSop;

			if (dataSource.IsImage != IsImage)
				throw new ArgumentException("Data source/Sop type mismatch.", "dataSource");

			_dataSource = dataSource;
		}

		/// <summary>
		/// Gets the <see cref="Sop"/>'s data source.
		/// </summary>
		public ISopDataSource DataSource
		{
			get { return _dataSource; }
		}

		/// <summary>
		/// Gets a value indicating whether or not the SOP instance is 'stored',
		/// for example in the local store or a remote PACS server.
		/// </summary>
		public bool IsStored
		{
			get { return DataSource.IsStored; }
		}

		/// <summary>
		/// Gets a value indicating whether or not the SOP instance is an image class.
		/// </summary>
		public bool IsImage { get; private set; }

		/// <summary>
		/// Gets the parent <see cref="Series"/>.
		/// </summary>
		public virtual Series ParentSeries
		{
			get { return _parentSeries; }
			internal set { _parentSeries = value; }
		}

		/// <summary>
		/// Gets an <see cref="IImageIdentifier"/> for this <see cref="Sop"/>.
		/// </summary>
		/// <remarks>An <see cref="IImageIdentifier"/> can be used in situations where you only
		/// need some data about the <see cref="Sop"/>, but not the <see cref="Sop"/> itself.  It can be problematic
		/// to hold references to <see cref="Sop"/> objects outside the context of an <see cref="IImageViewer"/>
		/// <b>without creating a <see cref="ISopReference">transient reference</see></b>
		/// because they are no longer valid when the viewer is closed; in these situations, it may be appropriate to
		/// use an identifier.
		/// </remarks>
		public IImageIdentifier GetIdentifier()
		{
			var studyIdentifier = GetStudyIdentifier();
			return new ImageIdentifier(this, studyIdentifier);
		}

		internal IStudyRootStudyIdentifier GetStudyIdentifier()
		{
			return new StudyRootStudyIdentifier(this, this, null)
			       	{
			       		SpecificCharacterSet = DicomStringHelper.GetDicomStringArray(SpecificCharacterSet),
			       		RetrieveAE = DataSource.Server,
			       		InstanceAvailability = "ONLINE"
			       	};
		}

		internal ISeriesIdentifier GetSeriesIdentifier()
		{
			var studyIdentifier = GetStudyIdentifier();
			return new SeriesIdentifier(this, studyIdentifier);
		}

		#region Meta info

		/// <summary>
		/// Gets the Transfer Syntax UID.
		/// </summary>
		public virtual string TransferSyntaxUid
		{
		    get
		    {
		        var value = _cache.TransferSyntaxUid;
                if (value == null)
                    _cache.TransferSyntaxUid = value = DataSource.TransferSyntaxUid;
                return value;
		    }
		}

		#endregion

		#region SOP Common Module

		/// <summary>
		/// Gets the SOP Instance UID.
		/// </summary>
		public virtual string SopInstanceUid
		{
		    get
		    {
                var value = _cache.SopInstanceUid;
                if (value == null)
                    _cache.SopInstanceUid = value = DataSource.SopInstanceUid;
                return value;
            }
		}

		/// <summary>
		/// Gets the SOP Class UID.
		/// </summary>
		public virtual string SopClassUid
		{
		    get
		    {
                var value = _cache.SopClassUid;
                if (value == null)
                    _cache.SopClassUid = value = DataSource.SopClassUid;
                return value;
            }
		}

		/// <summary>
		/// Gets the specific character set.
		/// </summary>
		public virtual string[] SpecificCharacterSet
		{
			get
			{
			    var value = _cache.SpecificCharacterSet;
                if (value != null)
                    return value.ToArray();

			    DicomAttribute attribute;
				var specificCharacterSet = TryGetAttribute(DicomTags.SpecificCharacterSet, out attribute) ? attribute.ToString() : String.Empty;
			    value = !string.IsNullOrEmpty(specificCharacterSet) ? DicomStringHelper.GetStringArray(specificCharacterSet) : new string[0];
			    _cache.SpecificCharacterSet = value;
			    
                return value.ToArray();
			}
		}

		#endregion

		#region SOP Common Module

		/// <summary>
		/// Gets the instance number.
		/// </summary>
		public virtual int InstanceNumber
		{
		    get
		    {
                var value = _cache.InstanceNumber;
                if (value == null)
                    _cache.InstanceNumber = value = DataSource.InstanceNumber;
                return value.Value;
		    }
		}

		/// <summary>
		/// Gets the content date.
		/// </summary>
		public virtual string ContentDate
		{
		    get
		    {
                var value = _cache.ContentDate;
		        if (value != null) return value;

		        DicomAttribute attribute;
		        _cache.ContentDate = value = TryGetAttribute(DicomTags.ContentDate, out attribute) ? attribute.ToString() : String.Empty;
		        return value;
		    }
		}

		/// <summary>
		/// Gets the content time.
		/// </summary>
		public virtual string ContentTime
		{
		    get
		    {
                var value = _cache.ContentTime;
		        if (value != null) return value;

		        DicomAttribute attribute;
		        _cache.ContentTime = value = TryGetAttribute(DicomTags.ContentTime, out attribute) ? attribute.ToString() : String.Empty;
		        return value;
            }
		}

		#endregion

		#region Patient Module

		/// <summary>
		/// Gets the patient's name.
		/// </summary>
		public virtual PersonName PatientsName
		{
			get
			{
                var value = _cache.PatientsName;
			    if (value != null) return new PersonName(value);

			    DicomAttribute attribute;
                _cache.PatientsName = value = TryGetAttribute(DicomTags.PatientsName, out attribute) ? attribute.ToString() : String.Empty;
			    return new PersonName(value);
			}
		}

		/// <summary>
		/// Gets the patient ID.
		/// </summary>
		public virtual string PatientId
		{
			get
			{
                var value = _cache.PatientId;
			    if (value != null) return value;

			    DicomAttribute attribute;
                _cache.PatientId = value = TryGetAttribute(DicomTags.PatientId, out attribute) ? attribute.ToString() : String.Empty;
			    return value;
			}
		}

		/// <summary>
		/// Gets the patient's birth date.
		/// </summary>
		public virtual string PatientsBirthDate
		{
			get
			{
                var value = _cache.PatientsBirthDate;
			    if (value != null) return value;
			    
                DicomAttribute attribute;
                _cache.PatientsBirthDate = value = TryGetAttribute(DicomTags.PatientsBirthDate, out attribute) ? attribute.ToString() : String.Empty;
			    return value;
            }
		}

		/// <summary>
		/// Gets the patient's birth time.
		/// </summary>
		public virtual string PatientsBirthTime
		{
			get
			{
                var value = _cache.PatientsBirthTime;
			    if (value != null) return value;
			    
                DicomAttribute attribute;
                _cache.PatientsBirthTime = value = TryGetAttribute(DicomTags.PatientsBirthTime, out attribute) ? attribute.ToString() : String.Empty;
			    return value;
            }
		}

		/// <summary>
		/// Gets the patient's sex.
		/// </summary>
		public virtual string PatientsSex
		{
			get
			{
                var value = _cache.PatientsSex;
			    if (value != null) return value;
			    
                DicomAttribute attribute;
                _cache.PatientsSex = value = TryGetAttribute(DicomTags.PatientsSex, out attribute) ? attribute.ToString() : String.Empty;
			    return value;
            }
		}

		#region Species

		/// <summary>
		/// Gets the patient species description.
		/// </summary>
		public virtual string PatientSpeciesDescription
		{
		    get
		    {
                var value = _cache.PatientSpeciesDescription;
		        if (value != null) return value;

		        DicomAttribute attribute;
                _cache.PatientSpeciesDescription = value = TryGetAttribute(DicomTags.PatientSpeciesDescription, out attribute) ? attribute.ToString() : String.Empty;
		        return value;
		    }
		}

		/// <summary>
		/// Gets the patient species code sequence.
		/// </summary>
		public virtual Species PatientSpeciesCodeSequence
		{
			get
			{
			    DicomAttribute attribute;
				if (!TryGetAttribute(DicomTags.PatientSpeciesCodeSequence, out attribute) || attribute.IsNull || attribute.Count == 0)
					return null;

				Species species;
				return Species.TryParse(new CodeSequenceMacro(((DicomSequenceItem[]) attribute.Values)[0]), out species) ? species : null;
			}
		}

		/// <summary>
		/// Gets the coding scheme designator of the patient species code sequence.
		/// </summary>
		public virtual string PatientSpeciesCodeSequenceCodingSchemeDesignator
		{
			get
			{
                DicomAttribute attribute;
                if (!TryGetAttribute(DicomTags.PatientSpeciesCodeSequence, out attribute) || attribute.IsNull || attribute.Count == 0)
                    return null;

				var codeSquenceMacro = new CodeSequenceMacro(((DicomSequenceItem[]) attribute.Values)[0]);
				return codeSquenceMacro.CodingSchemeDesignator;
			}
		}

		/// <summary>
		/// Gets the code value of the patient species code sequence.
		/// </summary>
		public virtual string PatientSpeciesCodeSequenceCodeValue
		{
			get
			{
                DicomAttribute attribute;
                if (!TryGetAttribute(DicomTags.PatientSpeciesCodeSequence, out attribute) || attribute.IsNull || attribute.Count == 0)
                    return null;

				var codeSquenceMacro = new CodeSequenceMacro(((DicomSequenceItem[]) attribute.Values)[0]);
				return codeSquenceMacro.CodeValue;
			}
		}

		/// <summary>
		/// Gets the code meaning of the patient species code sequence.
		/// </summary>
		public string PatientSpeciesCodeSequenceCodeMeaning
		{
			get
			{
                DicomAttribute attribute;
                if (!TryGetAttribute(DicomTags.PatientSpeciesCodeSequence, out attribute) || attribute.IsNull || attribute.Count == 0)
                    return null;

				var codeSquenceMacro = new CodeSequenceMacro(((DicomSequenceItem[]) attribute.Values)[0]);
				return codeSquenceMacro.CodeMeaning;
			}
		}

		#endregion

		#region Breed

		/// <summary>
		/// Gets the patient breed description.
		/// </summary>
		public virtual string PatientBreedDescription
		{
		    get
		    {
		        DicomAttribute attribute;
		        return TryGetAttribute(DicomTags.PatientBreedDescription, out attribute) ? attribute.ToString() : String.Empty;
		    }
		}

		/// <summary>
		/// Gets the patient breed code sequence.
		/// </summary>
		public virtual Breed PatientBreedCodeSequence
		{
			get
			{
                DicomAttribute attribute;
                if (!TryGetAttribute(DicomTags.PatientBreedCodeSequence, out attribute) || attribute.IsNull || attribute.Count == 0)
                    return null;

				Breed breed;
				return Breed.TryParse(new CodeSequenceMacro(((DicomSequenceItem[]) attribute.Values)[0]), out breed) ? breed : null;
			}
		}

		/// <summary>
		/// Gets the coding scheme designator of the patient breed code sequence.
		/// </summary>
		public virtual string PatientBreedCodeSequenceCodingSchemeDesignator
		{
			get
			{
                DicomAttribute attribute;
                if (!TryGetAttribute(DicomTags.PatientBreedCodeSequence, out attribute) || attribute.IsNull || attribute.Count == 0)
                    return null;

				var codeSquenceMacro = new CodeSequenceMacro(((DicomSequenceItem[]) attribute.Values)[0]);
				return codeSquenceMacro.CodingSchemeDesignator;
			}
		}

		/// <summary>
		/// Gets the code value of the patient breed code sequence.
		/// </summary>
		public virtual string PatientBreedCodeSequenceCodeValue
		{
			get
			{
                DicomAttribute attribute;
                if (!TryGetAttribute(DicomTags.PatientBreedCodeSequence, out attribute) || attribute.IsNull || attribute.Count == 0)
                    return null;

				var codeSquenceMacro = new CodeSequenceMacro(((DicomSequenceItem[]) attribute.Values)[0]);
				return codeSquenceMacro.CodeValue;
			}
		}

		/// <summary>
		/// Gets the code meaning of the patient breed code sequence.
		/// </summary>
		public virtual string PatientBreedCodeSequenceCodeMeaning
		{
			get
			{
                DicomAttribute attribute;
                if (!TryGetAttribute(DicomTags.PatientBreedCodeSequence, out attribute) || attribute.IsNull || attribute.Count == 0)
                    return null;

				var codeSquenceMacro = new CodeSequenceMacro(((DicomSequenceItem[]) attribute.Values)[0]);
				return codeSquenceMacro.CodeMeaning;
			}
		}

		#endregion

		#region Responsible Person/Organization

		/// <summary>
		/// Gets the responsible person for the patient.
		/// </summary>
		public virtual PersonName ResponsiblePerson
		{
			get
			{
			    DicomAttribute attribute;
                var responsiblePerson = TryGetAttribute(DicomTags.ResponsiblePerson, out attribute) ? attribute.ToString() : String.Empty;
				return new PersonName(responsiblePerson);
			}
		}

		/// <summary>
		/// Gets the role of the responsible person for the patient.
		/// </summary>
		public virtual string ResponsiblePersonRole
		{
		    get
		    {
                DicomAttribute attribute;
                return TryGetAttribute(DicomTags.ResponsiblePersonRole, out attribute) ? attribute.ToString() : String.Empty;
		    }
		}

		/// <summary>
		/// Gets the organization responsible for the patient.
		/// </summary>
		public virtual string ResponsibleOrganization
		{
		    get
		    {
                DicomAttribute attribute;
                return TryGetAttribute(DicomTags.ResponsibleOrganization, out attribute) ? attribute.ToString() : String.Empty;
		    }
		}

		#endregion

		#endregion

		string IPatientData.ResponsiblePerson
		{
			get { return ResponsiblePerson.ToString(); }
		}

		string IPatientData.PatientsName
		{
			get { return this.PatientsName.ToString(); }
		}

		#region General Study Module

		/// <summary>
		/// Gets the Study Instance UID.
		/// </summary>
		public virtual string StudyInstanceUid
		{
		    get
		    {
                var value = _cache.StudyInstanceUid;
                if (value == null)
                    _cache.StudyInstanceUid = value = DataSource.StudyInstanceUid;
                return value;
            }
		}

		/// <summary>
		/// Gets the study date.
		/// </summary>
		public virtual string StudyDate
		{
			get
			{
                var value = _cache.StudyDate;
			    if (value != null) return value;
			    
                DicomAttribute attribute;
			    _cache.StudyDate = value = TryGetAttribute(DicomTags.StudyDate, out attribute) ? attribute.ToString() : String.Empty;
			    return value;
			}
		}

		/// <summary>
		/// Gets the study time.
		/// </summary>
		public virtual string StudyTime
		{
			get
			{
                var value = _cache.StudyTime;
			    if (value != null) return value;
			    
                DicomAttribute attribute;
			    _cache.StudyTime = value = TryGetAttribute(DicomTags.StudyTime, out attribute) ? attribute.ToString() : String.Empty;
			    return value;
            }
		}

		/// <summary>
		/// Gets the referring physician's name.
		/// </summary>
		public virtual PersonName ReferringPhysiciansName
		{
			get
			{
                var value = _cache.ReferringPhysiciansName;
			    if (value == null)
			    {
                    DicomAttribute attribute;
                    _cache.ReferringPhysiciansName = value = TryGetAttribute(DicomTags.ReferringPhysiciansName, out attribute) ? attribute.ToString() : String.Empty;
			    }
			    return new PersonName(value);
            }
		}

		/// <summary>
		/// Gets the accession number.
		/// </summary>
		public virtual string AccessionNumber
		{
			get
			{
                var value = _cache.AccessionNumber;
			    if (value != null) return value;
			    
                DicomAttribute attribute;
			    _cache.AccessionNumber = value = TryGetAttribute(DicomTags.AccessionNumber, out attribute) ? attribute.ToString() : String.Empty;
			    return value;
            }
		}

		/// <summary>
		/// Gets the study description.
		/// </summary>
		public virtual string StudyDescription
		{
			get
			{
                var value = _cache.StudyDescription;
			    if (value != null) return value;
			    
                DicomAttribute attribute;
			    _cache.StudyDescription = value = TryGetAttribute(DicomTags.StudyDescription, out attribute) ? attribute.ToString() : String.Empty;
			    return value;
            }
		}

		/// <summary>
		/// Gets the study ID.
		/// </summary>
		public virtual string StudyId
		{
			get
			{
                var value = _cache.StudyId;
                if (value != null) return value;

                DicomAttribute attribute;
                _cache.StudyId = value = TryGetAttribute(DicomTags.StudyId, out attribute) ? attribute.ToString() : String.Empty;
                return value;
            }
		}

		/// <summary>
		/// Gets the names of physicians reading the study.
		/// </summary>
		public virtual PersonName[] NameOfPhysiciansReadingStudy
		{
			get
			{
			    DicomAttribute attribute;
				var nameOfPhysiciansReadingStudy = TryGetAttribute(DicomTags.NameOfPhysiciansReadingStudy, out attribute) ? attribute.ToString() : String.Empty;
				return DicomStringHelper.GetPersonNameArray(nameOfPhysiciansReadingStudy);
			}
		}

		#endregion

		string[] IStudyData.SopClassesInStudy
		{
			get
			{
				if (_parentSeries != null && _parentSeries.ParentStudy != null)
					return _parentSeries.ParentStudy.SopClassesInStudy;

				return null;
			}
		}

		string[] IStudyData.ModalitiesInStudy
		{
			get
			{
				if (_parentSeries != null && _parentSeries.ParentStudy != null)
					return _parentSeries.ParentStudy.ModalitiesInStudy;

				return null;
			}
		}

		string IStudyData.ReferringPhysiciansName
		{
			get { return ReferringPhysiciansName.ToString(); }
		}

		int? IStudyData.NumberOfStudyRelatedSeries
		{
			get
			{
				if (_parentSeries != null && _parentSeries.ParentStudy != null)
					return _parentSeries.ParentStudy.NumberOfStudyRelatedSeries;

				return null;
			}
		}

		int? IStudyData.NumberOfStudyRelatedInstances
		{
			get
			{
				if (_parentSeries != null && _parentSeries.ParentStudy != null)
					return _parentSeries.ParentStudy.NumberOfStudyRelatedInstances;

				return null;
			}
		}

		#region Patient Study Module

		/// <summary>
		/// Gets the admitting diagnoses descriptions.
		/// </summary>
		public virtual string[] AdmittingDiagnosesDescription
		{
			get
			{
			    DicomAttribute attribute;
				var admittingDiagnosesDescription = TryGetAttribute(DicomTags.AdmittingDiagnosesDescription, out attribute)? attribute.ToString() : String.Empty;
				return DicomStringHelper.GetStringArray(admittingDiagnosesDescription);
			}
		}

		/// <summary>
		/// Gets the patient's age.
		/// </summary>
		public virtual string PatientsAge
		{
			get
			{
			    var value = _cache.PatientsAge;
			    if (value != null) return value;
			    
                DicomAttribute attribute;
			    _cache.PatientsAge = value = TryGetAttribute(DicomTags.PatientsAge, out attribute) ? attribute.ToString() : String.Empty;
			    return value;
			}
		}

		/// <summary>
		/// Gets the additional patient's history.
		/// </summary>
		public virtual string AdditionalPatientsHistory
		{
			get
			{
			    DicomAttribute attribute;
			    return TryGetAttribute(DicomTags.AdditionalPatientHistory, out attribute) ? attribute.ToString() : String.Empty;
			}
		}

		#endregion

		#region General Equipment Module

		/// <summary>
		/// Gets the manufacturer.
		/// </summary>
		public virtual string Manufacturer
		{
			get
			{
			    var value = _cache.Manufacturer;
			    if (value != null) return value;
			    
                DicomAttribute attribute;
			    _cache.Manufacturer = value = TryGetAttribute(DicomTags.Manufacturer, out attribute) ? attribute.ToString() : String.Empty;
			    return value;
			}
		}

		/// <summary>
		/// Gets the institution name.
		/// </summary>
		public virtual string InstitutionName
		{
			get
			{
                var value = _cache.InstitutionName;
			    if (value != null) return value;
			    
                DicomAttribute attribute;
			    _cache.InstitutionName = value = TryGetAttribute(DicomTags.InstitutionName, out attribute) ? attribute.ToString() : String.Empty;
			    return value;
            }
		}

		/// <summary>
		/// Gets the station name.
		/// </summary>
		public virtual string StationName
		{
			get
			{
                var value = _cache.StationName;
                if (value != null) return value;

                DicomAttribute attribute;
                _cache.StationName = value = TryGetAttribute(DicomTags.StationName, out attribute) ? attribute.ToString() : String.Empty;
                return value;
            }
		}

		/// <summary>
		/// Gets the institutional department name.
		/// </summary>
		public virtual string InstitutionalDepartmentName
		{
			get
			{
                var value = _cache.InstitutionalDepartmentName;
                if (value != null) return value;

                DicomAttribute attribute;
                _cache.InstitutionalDepartmentName = value = TryGetAttribute(DicomTags.InstitutionalDepartmentName, out attribute) ? attribute.ToString() : String.Empty;
                return value;
            }
		}

		/// <summary>
		/// Gets the manufacturer's model name.
		/// </summary>
		public virtual string ManufacturersModelName
		{
			get
			{
                var value = _cache.ManufacturersModelName;
                if (value != null) return value;

                DicomAttribute attribute;
                _cache.ManufacturersModelName = value = TryGetAttribute(DicomTags.ManufacturersModelName, out attribute) ? attribute.ToString() : String.Empty;
                return value;
            }
		}

		#endregion

		#region General Series Module

		/// <summary>
		/// Gets the modality.
		/// </summary>
		public virtual string Modality
		{
			get
			{
                var value = _cache.Modality;
                if (value != null) return value;

                DicomAttribute attribute;
                _cache.Modality = value = TryGetAttribute(DicomTags.Modality, out attribute) ? attribute.ToString() : String.Empty;
                return value;
            }
		}

		/// <summary>
		/// Gets the series instance UID.
		/// </summary>
		public virtual string SeriesInstanceUid
		{
		    get
		    {
                var value = _cache.SeriesInstanceUid;
		        if (value == null)
		            _cache.SeriesInstanceUid = value = DataSource.SeriesInstanceUid;
                return value;
		    }
		}

		/// <summary>
		/// Gets the series number.
		/// </summary>
		public virtual int SeriesNumber
		{
			get
			{
                var value = _cache.SeriesNumber;
			    if (value.HasValue) return value.Value;

                DicomAttribute attribute;
                _cache.SeriesNumber = value = TryGetAttribute(DicomTags.SeriesNumber, out attribute) ? attribute.GetInt32(0, 0) : 0;
                return value.Value;
            }
		}

		/// <summary>
		/// Gets the laterality.
		/// </summary>
		public virtual string Laterality
		{
			get
			{
                var value = _cache.Laterality;
                if (value != null) return value;

                DicomAttribute attribute;
                _cache.Laterality = value = TryGetAttribute(DicomTags.Laterality, out attribute) ? attribute.ToString() : String.Empty;
                return value;
            }
		}

		/// <summary>
		/// Gets the series date.
		/// </summary>
		public virtual string SeriesDate
		{
			get
			{
                var value = _cache.SeriesDate;
                if (value != null) return value;

                DicomAttribute attribute;
                _cache.SeriesDate = value = TryGetAttribute(DicomTags.SeriesDate, out attribute) ? attribute.ToString() : String.Empty;
                return value;
            }
		}

		/// <summary>
		/// Gets the series time.
		/// </summary>
		public virtual string SeriesTime
		{
			get
			{
                var value = _cache.SeriesTime;
                if (value != null) return value;

                DicomAttribute attribute;
                _cache.SeriesTime = value = TryGetAttribute(DicomTags.SeriesTime, out attribute) ? attribute.ToString() : String.Empty;
                return value;
            }
		}

		/// <summary>
		/// Gets the names of performing physicians.
		/// </summary>
		public virtual PersonName[] PerformingPhysiciansName
		{
			get
			{
			    DicomAttribute attribute;
				var performingPhysiciansNames = TryGetAttribute(DicomTags.PerformingPhysiciansName, out attribute) ? attribute.ToString() : String.Empty;
				return DicomStringHelper.GetPersonNameArray(performingPhysiciansNames);
			}
		}

		/// <summary>
		/// Gets the protocol name.
		/// </summary>
		public virtual string ProtocolName
		{
			get
			{
                var value = _cache.ProtocolName;
                if (value != null) return value;

                DicomAttribute attribute;
                _cache.ProtocolName = value = TryGetAttribute(DicomTags.ProtocolName, out attribute) ? attribute.ToString() : String.Empty;
                return value;
            }
		}

		/// <summary>
		/// Gets the series description.
		/// </summary>
		public virtual string SeriesDescription
		{
			get
			{
                var value = _cache.SeriesDescription;
                if (value != null) return value;

                DicomAttribute attribute;
                _cache.SeriesDescription = value = TryGetAttribute(DicomTags.SeriesDescription, out attribute) ? attribute.ToString() : String.Empty;
                return value;
            }
		}

		/// <summary>
		/// Gets the names of operators.
		/// </summary>
		public virtual PersonName[] OperatorsName
		{
			get
			{
			    DicomAttribute attribute;
				var operatorsNames = TryGetAttribute(DicomTags.OperatorsName, out attribute) ? attribute.ToString() : String.Empty;
				return DicomStringHelper.GetPersonNameArray(operatorsNames);
			}
		}

		/// <summary>
		/// Gets the body part examined.
		/// </summary>
		public virtual string BodyPartExamined
		{
			get
			{
                var value = _cache.BodyPartExamined;
                if (value != null) return value;

                DicomAttribute attribute;
                _cache.BodyPartExamined = value = TryGetAttribute(DicomTags.BodyPartExamined, out attribute) ? attribute.ToString() : String.Empty;
                return value;
            }
		}

		/// <summary>
		/// Gets the patient position.
		/// </summary>
		public virtual string PatientPosition
		{
			get
			{
                var value = _cache.PatientPosition;
                if (value != null) return value;

                DicomAttribute attribute;
                _cache.PatientPosition = value = TryGetAttribute(DicomTags.PatientPosition, out attribute) ? attribute.ToString() : String.Empty;
                return value;
            }
		}

		#endregion

		int? ISeriesData.NumberOfSeriesRelatedInstances
		{
			get
			{
				if (_parentSeries != null)
					return _parentSeries.NumberOfSeriesRelatedInstances;
				return null;
			}
		}

		#region Dicom Tag Retrieval Methods

		#region IDicomAttributeProvider Implementation

		/// <summary>
		/// Gets a specific DICOM attribute in the underlying native object.
		/// </summary>
		/// <remarks>
		/// <see cref="DicomAttribute"/>s returned from this indexer are considered
		/// read-only and should not be modified in any way.
		/// </remarks>
		/// <param name="tag">The DICOM tag of the attribute to retrieve.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the specified DICOM tag is not within the valid range for either the meta info or the dataset.</exception>
		public virtual DicomAttribute this[DicomTag tag]
		{
			get { return DataSource[tag]; }
		}

		/// <summary>
		/// Gets a specific DICOM attribute in the underlying native object.
		/// </summary>
		/// <remarks>
		/// <see cref="DicomAttribute"/>s returned from this indexer are considered
		/// read-only and should not be modified in any way.
		/// </remarks>
		/// <param name="tag">The DICOM tag of the attribute to retrieve.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the specified DICOM tag is not within the valid range for either the meta info or the dataset.</exception>
		public virtual DicomAttribute this[uint tag]
		{
			get { return DataSource[tag]; }
		}

		/// <summary>
		/// Gets a specific DICOM attribute in the underlying native object.
		/// </summary>
		/// <remarks>
		/// <see cref="DicomAttribute"/>s returned from this indexer are considered
		/// read-only and should not be modified in any way.
		/// </remarks>
		/// <param name="tag">The DICOM tag of the attribute to retrieve.</param>
		/// <param name="dicomAttribute">Returns the requested <see cref="DicomAttribute"/>.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the specified DICOM tag is not within the valid range for either the meta info or the dataset.</exception>
		public virtual bool TryGetAttribute(DicomTag tag, out DicomAttribute dicomAttribute)
		{
			return DataSource.TryGetAttribute(tag, out dicomAttribute);
		}

		/// <summary>
		/// Gets a specific DICOM attribute in the underlying native object.
		/// </summary>
		/// <remarks>
		/// <see cref="DicomAttribute"/>s returned from this indexer are considered
		/// read-only and should not be modified in any way.
		/// </remarks>
		/// <param name="tag">The DICOM tag of the attribute to retrieve.</param>
		/// <param name="dicomAttribute">Returns the requested <see cref="DicomAttribute"/>.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the specified DICOM tag is not within the valid range for either the meta info or the dataset.</exception>
		public virtual bool TryGetAttribute(uint tag, out DicomAttribute dicomAttribute)
		{
			return DataSource.TryGetAttribute(tag, out dicomAttribute);
		}

		DicomAttribute IDicomAttributeProvider.this[DicomTag tag]
		{
			get { return this[tag]; }
			set { throw new InvalidOperationException(); }
		}

		DicomAttribute IDicomAttributeProvider.this[uint tag]
		{
			get { return this[tag]; }
			set { throw new InvalidOperationException(); }
		}

		#endregion
		#endregion

		#region Static Helpers

		/// <summary>
		/// Creates either a <see cref="Sop"/> or <see cref="ImageSop"/> based
		/// on the <see cref="SopClass"/> of the given <see cref="ISopDataSource"/>.
		/// </summary>
		public static Sop Create(ISopDataSource dataSource)
		{
		    return dataSource.IsImage ? new ImageSop(dataSource) : new Sop(dataSource);
		}

	    /// <summary>
		/// Creates either a <see cref="Sop"/> or <see cref="ImageSop"/> based
		/// on the <see cref="SopClass"/> of the SOP instance specified by <paramref name="filename"/>.
		/// </summary>
		public static Sop Create(string filename)
		{
			return Create(new LocalSopDataSource(filename));
		}

		internal static bool IsImageSop(string sopClassUid)
		{
			return IsImageSop(SopClass.GetSopClass(sopClassUid));
		}

		// TODO (CR Jun 2012): Move to a more common place, like a SopClassExtensions class in IV.Common, or CC.Dicom?

		internal static bool IsImageSop(SopClass sopClass)
		{
			return _imageSopClasses.Contains(sopClass);
		}

		private static readonly ReadOnlyCollection<SopClass> _imageSopClasses = new List<SopClass>(GetImageSopClasses()).AsReadOnly();
        private static readonly bool _disableSopValidation = ValidationSettings.Default.DisableSopValidation;

	    private static IEnumerable<SopClass> GetImageSopClasses()
		{
			yield return SopClass.BreastTomosynthesisImageStorage;

			yield return SopClass.ComputedRadiographyImageStorage;
			yield return SopClass.CtImageStorage;

			yield return SopClass.DigitalIntraOralXRayImageStorageForPresentation;
			yield return SopClass.DigitalIntraOralXRayImageStorageForProcessing;

			yield return SopClass.DigitalMammographyXRayImageStorageForPresentation;
			yield return SopClass.DigitalMammographyXRayImageStorageForProcessing;

			yield return SopClass.DigitalXRayImageStorageForPresentation;
			yield return SopClass.DigitalXRayImageStorageForProcessing;

			yield return SopClass.EnhancedCtImageStorage;
			yield return SopClass.EnhancedMrImageStorage;
			yield return SopClass.EnhancedPetImageStorage;

			yield return SopClass.EnhancedUsVolumeStorage;

			yield return SopClass.EnhancedXaImageStorage;
			yield return SopClass.EnhancedXrfImageStorage;

			yield return SopClass.MrImageStorage;

			yield return SopClass.MultiFrameGrayscaleByteSecondaryCaptureImageStorage;
			yield return SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage;
			yield return SopClass.MultiFrameSingleBitSecondaryCaptureImageStorage;
			yield return SopClass.MultiFrameTrueColorSecondaryCaptureImageStorage;

			yield return SopClass.NuclearMedicineImageStorageRetired;
			yield return SopClass.NuclearMedicineImageStorage;

			yield return SopClass.OphthalmicPhotography16BitImageStorage;
			yield return SopClass.OphthalmicPhotography8BitImageStorage;
			yield return SopClass.OphthalmicTomographyImageStorage;

			yield return SopClass.PositronEmissionTomographyImageStorage;

			yield return SopClass.RtImageStorage;

			yield return SopClass.SecondaryCaptureImageStorage;

			yield return SopClass.UltrasoundImageStorage;
			yield return SopClass.UltrasoundImageStorageRetired;
			yield return SopClass.UltrasoundMultiFrameImageStorage;
			yield return SopClass.UltrasoundMultiFrameImageStorageRetired;

			yield return SopClass.VideoEndoscopicImageStorage;
			yield return SopClass.VideoMicroscopicImageStorage;
			yield return SopClass.VideoPhotographicImageStorage;

			yield return SopClass.VlEndoscopicImageStorage;
			yield return SopClass.VlMicroscopicImageStorage;
			yield return SopClass.VlPhotographicImageStorage;
			yield return SopClass.VlSlideCoordinatesMicroscopicImageStorage;

			yield return SopClass.XRay3dAngiographicImageStorage;
			yield return SopClass.XRay3dCraniofacialImageStorage;

			yield return SopClass.XRayAngiographicBiPlaneImageStorageRetired;
			yield return SopClass.XRayAngiographicImageStorage;

			yield return SopClass.XRayRadiofluoroscopicImageStorage;
		}

		#endregion

		#region Validation

		/// <summary>
		/// The <see cref="Sop"/> class (and derived classes) should not validate tag values from 
		/// within its properties, but instead clients should call this method at an appropriate time
		/// to determine whether or not the <see cref="Sop"/> should be used or discarded as invalid.
		/// </summary>
		/// <exception cref="SopValidationException">Thrown when validation fails.</exception>
		public void Validate()
		{
			try
			{
				ValidateInternal();
			}
			catch (SopValidationException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new SopValidationException("Sop validation failed.", e);
			}
		}

		public void ValidateAllowableTransferSyntax()
		{
			var mySyntax = TransferSyntaxUid;
			if (GetAllowableTransferSyntaxes().Any(syntax => mySyntax == syntax.UidString))
			    return;

			throw new SopValidationException(String.Format("Unsupported transfer syntax: {0}", TransferSyntaxUid));
		}

		protected virtual IEnumerable<TransferSyntax> GetAllowableTransferSyntaxes()
		{
			yield return TransferSyntax.ImplicitVrLittleEndian;
			yield return TransferSyntax.ExplicitVrLittleEndian;
			yield return TransferSyntax.ExplicitVrBigEndian;
		}

		/// <summary>
		/// Validates the <see cref="Sop"/> object.
		/// </summary>
		/// <remarks>
		/// Derived classes should call the base class implementation first, and then do further validation.
		/// The <see cref="Sop"/> class validates properties deemed vital to usage of the object.
		/// </remarks>
		/// <exception cref="SopValidationException">Thrown when validation fails.</exception>
		protected virtual void ValidateInternal()
		{
			if (_disableSopValidation)
				return;

			DicomValidator.ValidateSOPInstanceUID(SopInstanceUid, false);
            DicomValidator.ValidateSeriesInstanceUID(SeriesInstanceUid, false);
            DicomValidator.ValidateStudyInstanceUID(StudyInstanceUid, false);
		}

		#endregion

        /// <summary>
        /// Resets any values that were cached after reading from <see cref="DataSource"/>.
        /// </summary>
        /// <remarks>
        /// Many of the property values are cached for performance reasons, as they generally never change, 
        /// and parsing values from the image header can be expensive, especially when done repeatedly.
        /// </remarks>
        public virtual void ResetCache()
        {
            _cache = new CachedValues();
        }

		/// <summary>
		/// Disposes all resources being used by this <see cref="Sop"/>.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && _dataSource != null)
			{
				_dataSource.Dispose();
				_dataSource = null;
			}
		}

		/// <summary>
		/// Returns the SOP instance UID in string form.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("{0} | {1}", InstanceNumber, SopInstanceUid);
		}
	}
}