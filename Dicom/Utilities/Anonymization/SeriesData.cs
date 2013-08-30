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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.Utilities.Anonymization
{
	/// <summary>
	/// A class containing commonly anonymized dicom series attributes.
	/// </summary>
	[Cloneable(true)]
	public class SeriesData : ISeriesData
	{
		private string _seriesInstanceUid = "";
		private string _seriesDescription = "";
		private string _seriesNumber = "";
		private string _protocolName = "";

		/// <summary>
		/// Constructor.
		/// </summary>
		public SeriesData() {}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SeriesData(ISeriesData sourceData)
		{
			if (sourceData != null)
			{
				_seriesDescription = sourceData.SeriesDescription;
				_seriesNumber = sourceData.SeriesNumber.ToString("d");
			}
		}

		/// <summary>
		/// Gets or sets the series description.
		/// </summary>
		[DicomField(DicomTags.SeriesDescription)]
		public string SeriesDescription
		{
			get { return _seriesDescription; }
			set { _seriesDescription = value ?? ""; }
		}

		/// <summary>
		/// Gets or sets the series number.
		/// </summary>
		[DicomField(DicomTags.SeriesNumber)]
		public string SeriesNumber
		{
			get { return _seriesNumber; }
			set { _seriesNumber = value ?? ""; }
		}

		/// <summary>
		/// Gets or sets the protocol name.
		/// </summary>
		[DicomField(DicomTags.ProtocolName)]
		public string ProtocolName
		{
			get { return _protocolName; }
			set { _protocolName = value ?? ""; }
		}

		internal string SeriesInstanceUid
		{
			get { return _seriesInstanceUid; }
			set { _seriesInstanceUid = value ?? ""; }
		}

		internal void LoadFrom(DicomFile file)
		{
			file.DataSet.LoadDicomFields(this);
			this.SeriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid];
		}

		internal void SaveTo(DicomFile file)
		{
			file.DataSet.SaveDicomFields(this);
			file.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(this.SeriesInstanceUid);
		}

		/// <summary>
		/// Creates a deep clone of this instance.
		/// </summary>
		public SeriesData Clone()
		{
			return CloneBuilder.Clone(this) as SeriesData;
		}

		#region ISeriesData Implementation

		string ISeriesData.StudyInstanceUid
		{
			get { return string.Empty; }
		}

		string ISeriesData.SeriesInstanceUid
		{
			get { return string.Empty; }
		}

		string ISeriesData.Modality
		{
			get { return string.Empty; }
		}

		int ISeriesData.SeriesNumber
		{
			get
			{
				int i;
				return int.TryParse(SeriesNumber, out i) ? i : 0;
			}
		}

		int? ISeriesData.NumberOfSeriesRelatedInstances
		{
			get { return null; }
		}

		#endregion
	}
}