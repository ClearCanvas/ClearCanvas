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
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.Utilities.StudyBuilder
{
	/// <summary>
	/// A <see cref="StudyBuilderNode"/> representing a study-level data node in the <see cref="StudyBuilder"/> tree hierarchy.
	/// </summary>
	public sealed class StudyNode : StudyBuilderNode
	{
		private readonly SeriesNodeCollection _series;
		private string _instanceUid;
		private string _description;
		private DateTime? _dateTime;
		private string _accessionNum;
		private string _studyId;

		/// <summary>
		/// Constructs a new <see cref="StudyNode"/> using default values.
		/// </summary>
		public StudyNode()
		{
			_series = new SeriesNodeCollection(this);
			_instanceUid = StudyBuilder.NewUid();
			_studyId = string.Format("ST{0}", this.Key);
			_description = "Untitled Study";
			_dateTime = System.DateTime.Now;
			_accessionNum = "";
		}

		/// <summary>
		/// Constructs a new <see cref="StudyNode"/> using the specified study ID and default values for everything else.
		/// </summary>
		/// <param name="studyId">The desired study ID.</param>
		public StudyNode(string studyId) : this()
		{
			_studyId = studyId;
		}

		/// <summary>
		/// Constructs a new <see cref="StudyNode"/> using actual values from attributes in the given <see cref="DicomAttributeCollection"/>.
		/// </summary>
		/// <param name="dicomDataSet">The data set from which to initialize this node.</param>
		public StudyNode(DicomAttributeCollection dicomDataSet)
		{
			_series = new SeriesNodeCollection(this);
			_studyId = dicomDataSet[DicomTags.StudyId].GetString(0, "");
			_description = dicomDataSet[DicomTags.StudyDescription].GetString(0, "");
			_dateTime = DicomConverter.GetDateTime(dicomDataSet[DicomTags.StudyDate].GetDateTime(0), dicomDataSet[DicomTags.StudyTime].GetDateTime(0));
			_accessionNum = dicomDataSet[DicomTags.AccessionNumber].GetString(0, "");
			_instanceUid = dicomDataSet[DicomTags.StudyInstanceUid].GetString(0, "");
			if (_instanceUid == "")
				_instanceUid = StudyBuilder.NewUid();
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source"></param>
		/// <param name="copyDescendants"></param>
		public StudyNode(StudyNode source, bool copyDescendants) {
			_series = new SeriesNodeCollection(this);
			_instanceUid = StudyBuilder.NewUid();
			_studyId = source._studyId;
			_description = source._description;
			_dateTime = source._dateTime;
			_accessionNum = source._accessionNum;

			if(copyDescendants)
			{
				foreach (SeriesNode series in source._series)
				{
					_series.Add(series.Copy(true));
				}
			}
		}

		#region Misc

		/// <summary>
		/// Gets the parent of this node, or null if the node is not in a study builder tree.
		/// </summary>
		public new PatientNode Parent {
			get { return base.Parent as PatientNode; }
			internal set { base.Parent = value; }
		}

		#endregion

		#region Data Properties

		/// <summary>
		/// Gets or sets the study instance UID.
		/// </summary>
		public string InstanceUid
		{
			get { return _instanceUid; }
			internal set
			{
				if (_instanceUid != value)
				{
					if(string.IsNullOrEmpty(value))
						value = StudyBuilder.NewUid();

					_instanceUid = value;
					FirePropertyChanged("InstanceUid");
				}
			}
		}

		/// <summary>
		/// Gets or sets the study ID.
		/// </summary>
		public string StudyId
		{
			get { return _studyId; }
			set
			{
				if(_studyId != value)
				{
					_studyId = value;
					FirePropertyChanged("StudyId");
				}
			}
		}

		/// <summary>
		/// Gets or sets the study description.
		/// </summary>
		public string Description
		{
			get { return _description; }
			set
			{
				if(_description != value)
				{
					_description = value;
					FirePropertyChanged("Description");
				}
			}
		}

		/// <summary>
		/// Gets or sets the study date/time stamp.
		/// </summary>
		public DateTime? DateTime
		{
			get { return _dateTime; }
			set
			{
				if(_dateTime != value)
				{
					_dateTime = value;
					FirePropertyChanged("DateTime");
				}
			}
		}

		/// <summary>
		/// Gets or sets the accession number.
		/// </summary>
		public string AccessionNumber
		{
			get { return _accessionNum; }
			set
			{
				if(_accessionNum != value)
				{
					_accessionNum = value;
					FirePropertyChanged("AccessionNumber");
				}
			}
		}

		#endregion

		#region Update Methods

		/// <summary>
		/// Writes the data in this node into the given <see cref="DicomAttributeCollection"/>
		/// </summary>
		/// <param name="dicomDataSet">The data set to write data into.</param>
		internal void Update(DicomAttributeCollection dicomDataSet, bool writeUid)
		{
			dicomDataSet[DicomTags.StudyId].SetStringValue(_studyId);
			dicomDataSet[DicomTags.StudyDescription].SetStringValue(_description);
			dicomDataSet[DicomTags.AccessionNumber].SetStringValue(_accessionNum);

			DicomConverter.SetDate(dicomDataSet[DicomTags.StudyDate], _dateTime);
			DicomConverter.SetTime(dicomDataSet[DicomTags.StudyTime], _dateTime);

			if (writeUid)
				dicomDataSet[DicomTags.StudyInstanceUid].SetStringValue(_instanceUid);
		}

		#endregion

		#region Copy Methods

		/// <summary>
		/// Creates a new <see cref="StudyNode"/> with the same node data, nulling all references to other nodes.
		/// </summary>
		/// <returns>A copy of the node.</returns>
		public StudyNode Copy() {
			return this.Copy(false, false);
		}

		/// <summary>
		/// Creates a new <see cref="StudyNode"/> with the same node data, nulling all references to nodes outside of the copy scope.
		/// </summary>
		/// <param name="copyDescendants">Specifies that all the descendants of the node should also be copied.</param>
		/// <returns>A copy of the node.</returns>
		public StudyNode Copy(bool copyDescendants) {
			return this.Copy(copyDescendants, false);
		}

		/// <summary>
		/// Creates a new <see cref="StudyNode"/> with the same node data.
		/// </summary>
		/// <param name="copyDescendants">Specifies that all the descendants of the node should also be copied.</param>
		/// <param name="keepExtLinks">Specifies that references to nodes outside of the copy scope should be kept. If False, all references are nulled.</param>
		/// <returns>A copy of the node.</returns>
		public StudyNode Copy(bool copyDescendants, bool keepExtLinks) {
			return new StudyNode(this, copyDescendants);
		}

		#endregion

		#region Insert Methods

		/// <summary>
		/// Convenience method to insert SOP instance-level data nodes into the study builder tree under this study, creating a <see cref="SeriesNode">series</see> node if necessary.
		/// </summary>
		/// <param name="sopInstances">An array of <see cref="SopInstanceNode"/>s to insert into the study builder tree.</param>
		public void InsertSopInstance(SopInstanceNode[] sopInstances)
		{
			SeriesNode series = new SeriesNode();
			this.Series.Add(series);
			foreach (SopInstanceNode node in sopInstances)
			{
				series.Images.Add(node);
			}
		}

		/// <summary>
		/// Convenience method to insert series-level data nodes into the study builder tree under this study.
		/// </summary>
		/// <param name="series">An array of <see cref="SeriesNode"/>s to insert into the study builder tree.</param>
		public void InsertSeries(SeriesNode[] series)
		{
			foreach (SeriesNode node in series)
			{
				this.Series.Add(node);
			}
		}

		#endregion

		#region Series Collection

		/// <summary>
		/// Gets a list of all the <see cref="SeriesNode"/>s that belong to this study.
		/// </summary>
		public SeriesNodeCollection Series
		{
			get { return _series; }
		}

		#endregion
	}
}