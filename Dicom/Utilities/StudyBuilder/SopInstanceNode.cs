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
using System.IO;

namespace ClearCanvas.Dicom.Utilities.StudyBuilder
{
	/// <summary>
	/// A <see cref="StudyBuilderNode"/> representing a SOP instance-level data node in the <see cref="StudyBuilder"/> tree hierarchy.
	/// </summary>
	public sealed class SopInstanceNode : StudyBuilderNode
	{
		private readonly DicomFile _dicomFile;
		private string _instanceUid;

		/// <summary>
		/// Constructs a new <see cref="SopInstanceNode"/> using default values.
		/// </summary>
		public SopInstanceNode()
		{
			_dicomFile = new DicomFile("");
			_instanceUid = StudyBuilder.NewUid();
		}

		/// <summary>
		/// Constructs a new <see cref="SopInstanceNode"/> using the given <see cref="DicomFile"/> as a template.
		/// </summary>
		/// <param name="sourceDicomFile">The <see cref="DicomFile"/> from which to initialize this node.</param>
		public SopInstanceNode(DicomMessageBase sourceDicomFile)
		{
			_dicomFile = new DicomFile("", sourceDicomFile.MetaInfo.Copy(), sourceDicomFile.DataSet.Copy());

			_instanceUid = sourceDicomFile.DataSet[DicomTags.SopInstanceUid].GetString(0, "");
			if (_instanceUid == "")
				_instanceUid = StudyBuilder.NewUid();
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source"></param>
		private SopInstanceNode(SopInstanceNode source)
		{
			_instanceUid = StudyBuilder.NewUid();
			_dicomFile = new DicomFile("", source._dicomFile.MetaInfo.Copy(true, true, true), source._dicomFile.DataSet.Copy(true, true, true));
		}

		#region Data Properties

		/// <summary>
		/// Gets or sets the SOP instance UID.
		/// </summary>
		public string InstanceUid
		{
			get { return _instanceUid; }
			internal set
			{
				if (_instanceUid != value)
				{
					if (string.IsNullOrEmpty(value))
						value = StudyBuilder.NewUid();

					_instanceUid = value;
					FirePropertyChanged("InstanceUid");
				}
			}
		}

		#endregion

		#region Update Methods

		/// <summary>
		/// Writes the data in this node into the given <see cref="DicomAttributeCollection"/>.
		/// </summary>
		/// <param name="dataSet">The data set to write data into.</param>
		/// <param name="writeUid"></param>
		internal void Update(DicomAttributeCollection dataSet, bool writeUid)
		{
			int imageNumber = 0;
			if (this.Parent != null)
				imageNumber = this.Parent.Images.IndexOf(this) + 1;

			DicomConverter.SetInt32(dataSet[DicomTags.InstanceNumber], imageNumber);

			if (writeUid)
				dataSet[DicomTags.SopInstanceUid].SetStringValue(_instanceUid);
		}

		#endregion

		#region Copy Methods

		/// <summary>
		/// Creates a new <see cref="SopInstanceNode"/> with the same node data, nulling all references to other nodes.
		/// </summary>
		/// <returns>A copy of the node.</returns>
		public SopInstanceNode Copy()
		{
			return this.Copy(false);
		}

		/// <summary>
		/// Creates a new <see cref="SopInstanceNode"/> with the same node data.
		/// </summary>
		/// <param name="keepExtLinks">Specifies that references to nodes outside of the copy scope should be kept. If False, all references are nulled.</param>
		/// <returns>A copy of the node.</returns>
		public SopInstanceNode Copy(bool keepExtLinks)
		{
			return new SopInstanceNode(this);
		}

		#endregion

		#region Misc

		/// <summary>
		/// Gets the parent of this node, or null if the node is not in a study builder tree.
		/// </summary>
		public new SeriesNode Parent
		{
			get { return base.Parent as SeriesNode; }
			internal set { base.Parent = value; }
		}

		/// <summary>
		/// Gets the underlying data set of this node.
		/// </summary>
		public DicomAttributeCollection DicomData
		{
			get { return _dicomFile.DataSet; }
		}

		internal DicomFile DicomFile
		{
			get { return _dicomFile; }
		}

		/// <summary>
		/// Exports the contents of the data set to a DICOM file in the specified directory.
		/// </summary>
		/// <remarks>
		/// The filename is automatically generated using the SOP instance uid and the &quot;.dcm&quot; extension.
		/// </remarks>
		/// <param name="path">The directory to export the data to.</param>
		internal string ExportToDirectory(string path)
		{
			string filename = Path.Combine(path, _instanceUid + ".dcm");
			_dicomFile.Save(filename);
			return filename;
		}

		#endregion
	}
}