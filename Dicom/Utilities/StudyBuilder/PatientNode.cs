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

namespace ClearCanvas.Dicom.Utilities.StudyBuilder
{
	/// <summary>
	/// A <see cref="StudyBuilderNode"/> representing a patient-level data node in the <see cref="StudyBuilder"/> tree hierarchy.
	/// </summary>
	public sealed class PatientNode : StudyBuilderNode
	{
		private readonly StudyNodeCollection _studies;
		private string _patientId;
		private string _name;
		private DateTime? _birthdate;
		private PatientSex _sex;

		/// <summary>
		/// Constructs a new <see cref="PatientNode"/> using default values.
		/// </summary>
		public PatientNode()
		{
			_studies = new StudyNodeCollection(this);
			_patientId = string.Format("PN{0}", this.Key);
			_name = "Unnamed Patient";
			_birthdate = null;
			_sex = PatientSex.Undefined;
		}

		/// <summary>
		/// Constructs a new <see cref="PatientNode"/> using the specified patient ID and default values for everything else.
		/// </summary>
		/// <param name="patientId">The desired patient ID.</param>
		public PatientNode(string patientId) : this()
		{
			_patientId = patientId;
		}

		/// <summary>
		/// Constructs a new <see cref="PatientNode"/> using actual values from attributes in the given <see cref="DicomAttributeCollection"/>.
		/// </summary>
		/// <param name="dicomDataSet">The data set from which to initialize this node.</param>
		public PatientNode(DicomAttributeCollection dicomDataSet)
		{
			_studies = new StudyNodeCollection(this);
			_patientId = dicomDataSet[DicomTags.PatientId].GetString(0, "");
			_name = dicomDataSet[DicomTags.PatientsName].GetString(0, "");
			_birthdate = DicomConverter.GetDateTime(dicomDataSet[DicomTags.PatientsBirthDate].GetDateTime(0), dicomDataSet[DicomTags.PatientsBirthTime].GetDateTime(0));
			_sex = DicomConverter.GetSex(dicomDataSet[DicomTags.PatientsSex].GetString(0, ""));
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source"></param>
		/// <param name="copyDescendants"></param>
		private PatientNode(PatientNode source, bool copyDescendants)
		{
			_studies = new StudyNodeCollection(this);
			_patientId = source._patientId;
			_name = source._name;
			_birthdate = source._birthdate;
			_sex = source._sex;

			if (copyDescendants) {
				foreach (StudyNode study in source._studies) {
					_studies.Add(study.Copy(true));
				}
			}
		}

		#region Data Properties

		/// <summary>
		/// Gets or sets the patient ID (medical record number).
		/// </summary>
		public string PatientId
		{
			get { return _patientId; }
			set
			{
				if (_patientId != value)
				{
					_patientId = value;
					FirePropertyChanged("PatientId");
				}
			}
		}

		/// <summary>
		/// Gets or sets the patient's name.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					FirePropertyChanged("Name");
				}
			}
		}

		/// <summary>
		/// Gets or sets the patient's birthdate and time.
		/// </summary>
		public DateTime? BirthDate
		{
			get { return _birthdate; }
			set
			{
				if (_birthdate != value)
				{
					_birthdate = value;
					FirePropertyChanged("BirthDate");
				}
			}
		}

		/// <summary>
		/// Gets or sets the patient's gender.
		/// </summary>
		public PatientSex Sex
		{
			get { return _sex; }
			set
			{
				if (_sex != value)
				{
					_sex = value;
					FirePropertyChanged("Sex");
				}
			}
		}

		#endregion

		#region Update Methods

		/// <summary>
		/// Writes the data in this node into the given <see cref="DicomAttributeCollection"/>
		/// </summary>
		/// <param name="dicomDataSet">The data set to write data into.</param>
		internal void Update(DicomAttributeCollection dicomDataSet)
		{
			dicomDataSet[DicomTags.PatientId].SetStringValue(_patientId);
			dicomDataSet[DicomTags.PatientsName].SetStringValue(_name);
			dicomDataSet[DicomTags.PatientsSex].SetStringValue(DicomConverter.SetSex(_sex));

			DicomConverter.SetDate(dicomDataSet[DicomTags.PatientsBirthDate], _birthdate);
			DicomConverter.SetTime(dicomDataSet[DicomTags.PatientsBirthTime], _birthdate);

		}

		#endregion

		#region Copy Methods

		/// <summary>
		/// Creates a new <see cref="PatientNode"/> with the same node data, nulling all references to other nodes.
		/// </summary>
		/// <returns>A copy of the node.</returns>
		public PatientNode Copy() {
			return this.Copy(false, false);
		}

		/// <summary>
		/// Creates a new <see cref="PatientNode"/> with the same node data, nulling all references to nodes outside of the copy scope.
		/// </summary>
		/// <param name="copyDescendants">Specifies that all the descendants of the node should also be copied.</param>
		/// <returns>A copy of the node.</returns>
		public PatientNode Copy(bool copyDescendants) {
			return this.Copy(copyDescendants, false);
		}

		/// <summary>
		/// Creates a new <see cref="PatientNode"/> with the same node data.
		/// </summary>
		/// <param name="copyDescendants">Specifies that all the descendants of the node should also be copied.</param>
		/// <param name="keepExtLinks">Specifies that references to nodes outside of the copy scope should be kept. If False, all references are nulled.</param>
		/// <returns>A copy of the node.</returns>
		public PatientNode Copy(bool copyDescendants, bool keepExtLinks) {
			return new PatientNode(this, copyDescendants);
		}

		#endregion

		#region Insert Methods

		/// <summary>
		/// Convenience method to insert SOP instance-level data nodes into the study builder tree under this patient, creating <see cref="StudyNode">study</see> and <see cref="SeriesNode">series</see> nodes as necessary.
		/// </summary>
		/// <param name="sopInstances">An array of <see cref="SopInstanceNode"/>s to insert into the study builder tree.</param>
		public void InsertSopInstance(SopInstanceNode[] sopInstances)
		{
			StudyNode study = new StudyNode();
			this.Studies.Add(study);
			SeriesNode series = new SeriesNode();
			study.Series.Add(series);
			foreach (SopInstanceNode node in sopInstances)
			{
				series.Images.Add(node);
			}
		}

		/// <summary>
		/// Convenience method to insert series-level data nodes into the study builder tree under this patient, creating a <see cref="StudyNode">study</see> node if necessary.
		/// </summary>
		/// <param name="series">An array of <see cref="SeriesNode"/>s to insert into the study builder tree.</param>
		public void InsertSeries(SeriesNode[] series)
		{
			StudyNode study = new StudyNode();
			this.Studies.Add(study);
			foreach (SeriesNode node in series)
			{
				study.Series.Add(node);
			}
		}

		/// <summary>
		/// Convenience method to insert study-level data nodes into the study builder tree under this patient.
		/// </summary>
		/// <param name="studies">An array of <see cref="StudyNode"/>s to insert into the study builder tree.</param>
		public void InsertStudy(StudyNode[] studies)
		{
			foreach (StudyNode node in studies)
			{
				this.Studies.Add(node);
			}
		}

		#endregion

		#region Studies Collection

		/// <summary>
		/// Gets a collection of all the <see cref="StudyNode">studies</see> that belong to this patient.
		/// </summary>
		public StudyNodeCollection Studies
		{
			get { return _studies; }
		}

		#endregion
	}
}