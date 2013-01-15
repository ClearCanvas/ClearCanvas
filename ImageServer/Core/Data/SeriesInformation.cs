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
using System.Xml.Serialization;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Core.Data
{
	/// <summary>
	/// Represents serializable series information.
	/// </summary>
	[XmlRoot("Series")]
	public class SeriesInformation
	{
		#region Private Members

		private int _numberOfInstances;
       // private string _seriesNumber;
		#endregion

		#region Constructors

		public SeriesInformation()
		{
		}

		public SeriesInformation(IDicomAttributeProvider attributeProvider)
		{
			if (attributeProvider[DicomTags.SeriesInstanceUid] != null)
				SeriesInstanceUid = attributeProvider[DicomTags.SeriesInstanceUid].ToString();
			if (attributeProvider[DicomTags.SeriesDescription] != null)
				SeriesDescription = attributeProvider[DicomTags.SeriesDescription].ToString();
			if (attributeProvider[DicomTags.Modality] != null)
				Modality = attributeProvider[DicomTags.Modality].ToString();
            if (attributeProvider[DicomTags.SeriesNumber] != null)
                SeriesNumber = attributeProvider[DicomTags.SeriesNumber].ToString();

            if (attributeProvider[DicomTags.NumberOfSeriesRelatedInstances] != null)
                Int32.TryParse(attributeProvider[DicomTags.NumberOfSeriesRelatedInstances].ToString(), out _numberOfInstances);
		}

		#endregion

		#region Public Properties

		[XmlAttribute]
		public string SeriesInstanceUid { get; set; }

		[XmlAttribute]
		public string Modality { get; set; }

		public string SeriesDescription { get; set; }

        [XmlAttribute]
        public string SeriesNumber { get; set; }

		public int NumberOfInstances
		{
			get { return _numberOfInstances; }
			set { _numberOfInstances = value; }
		}

	    #endregion
	}
}