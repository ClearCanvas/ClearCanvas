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
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Iods
{
    /// <summary>
    /// IOD for common Series Query Retrieve items.
    /// </summary>
    public class SeriesQueryIod : QueryIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesQueryIod"/> class.
        /// </summary>
        public SeriesQueryIod()
        {
            SetAttributeFromEnum(DicomAttributeProvider[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Series);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesQueryIod"/> class.
        /// </summary>
		public SeriesQueryIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider)
        {
            SetAttributeFromEnum(DicomAttributeProvider[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Series);
        }
        #endregion

        #region Public Properties

		/// <summary>
		/// Gets or sets the study instance uid.
		/// </summary>
		/// <value>The study instance uid.</value>
		public string StudyInstanceUid
		{
			get { return DicomAttributeProvider[DicomTags.StudyInstanceUid].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.StudyInstanceUid].SetString(0, value); }
		}

        /// <summary>
        /// Gets or sets the series instance uid.
        /// </summary>
        /// <value>The series instance uid.</value>
        public string SeriesInstanceUid
        {
            get { return DicomAttributeProvider[DicomTags.SeriesInstanceUid].GetString(0, String.Empty); }
            set { DicomAttributeProvider[DicomTags.SeriesInstanceUid].SetString(0, value); }
        }

		/// <summary>
		/// Gets or sets the modality.
		/// </summary>
		/// <value>The modality.</value>
		public string Modality
		{
			get { return DicomAttributeProvider[DicomTags.Modality].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.Modality].SetString(0, value); }
		}

		/// <summary>
        /// Gets or sets the series description.
        /// </summary>
        /// <value>The series description.</value>
        public string SeriesDescription
        {
            get { return DicomAttributeProvider[DicomTags.SeriesDescription].GetString(0, String.Empty); }
            set { DicomAttributeProvider[DicomTags.SeriesDescription].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the series number.
        /// </summary>
        /// <value>The series number.</value>
        public string SeriesNumber
        {
            get { return DicomAttributeProvider[DicomTags.SeriesNumber].GetString(0, String.Empty); }
            set { DicomAttributeProvider[DicomTags.SeriesNumber].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the number of series related instances.
        /// </summary>
        /// <value>The number of series related instances.</value>
        public uint NumberOfSeriesRelatedInstances
        {
            get { return DicomAttributeProvider[DicomTags.NumberOfSeriesRelatedInstances].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.NumberOfSeriesRelatedInstances].SetUInt32(0, value); }
        }

        /// <summary>
        /// Gets or sets the series date.
        /// </summary>
        /// <value>The series date.</value>
        public DateTime? SeriesDate
        {
            get { return DateTimeParser.ParseDateAndTime(String.Empty, 
                    DicomAttributeProvider[DicomTags.SeriesDate].GetString(0, String.Empty), 
                    DicomAttributeProvider[DicomTags.SeriesTime].GetString(0, String.Empty)); }

            set { DateTimeParser.SetDateTimeAttributeValues(value, DicomAttributeProvider[DicomTags.SeriesDate], DicomAttributeProvider[DicomTags.SeriesTime]); }
        }

        /// <summary>
        /// Gets or sets the performed procedure step start date.
        /// </summary>
        /// <value>The performed procedure step start date.</value>
        public DateTime? PerformedProcedureStepStartDate
        {
            get { return DateTimeParser.ParseDateAndTime(String.Empty, 
                    DicomAttributeProvider[DicomTags.PerformedProcedureStepStartDate].GetString(0, String.Empty), 
                    DicomAttributeProvider[DicomTags.PerformedProcedureStepStartTime].GetString(0, String.Empty)); }

            set { DateTimeParser.SetDateTimeAttributeValues(value, DicomAttributeProvider[DicomTags.PerformedProcedureStepStartDate], DicomAttributeProvider[DicomTags.PerformedProcedureStepStartTime]); }
        }

		/// <summary>
		/// Gets the request attributes sequence list.
		/// </summary>
		/// <value>The request attributes sequence list.</value>
		public SequenceIodList<RequestAttributesSequenceIod> RequestAttributesSequence
		{
			get
			{
				return new SequenceIodList<RequestAttributesSequenceIod>(DicomAttributeProvider[DicomTags.RequestAttributesSequence] as DicomAttributeSQ);
			}
		}
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the common tags for a query retrieve request.
        /// </summary>
        public void SetCommonTags()
        {
            SetCommonTags(DicomAttributeProvider);
        }

        public static void SetCommonTags(IDicomAttributeProvider dicomAttributeProvider)
        {
			SetAttributeFromEnum(dicomAttributeProvider[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Series);

			dicomAttributeProvider[DicomTags.SeriesInstanceUid].SetNullValue();
			dicomAttributeProvider[DicomTags.Modality].SetNullValue();
			dicomAttributeProvider[DicomTags.SeriesDescription].SetNullValue();
			dicomAttributeProvider[DicomTags.NumberOfSeriesRelatedInstances].SetNullValue();
			dicomAttributeProvider[DicomTags.SeriesNumber].SetNullValue();
			dicomAttributeProvider[DicomTags.SeriesDate].SetNullValue();
			dicomAttributeProvider[DicomTags.SeriesTime].SetNullValue();
			dicomAttributeProvider[DicomTags.RequestAttributesSequence].SetNullValue();
			dicomAttributeProvider[DicomTags.PerformedProcedureStepStartDate].SetNullValue();
			dicomAttributeProvider[DicomTags.PerformedProcedureStepStartTime].SetNullValue();
        }
        #endregion
    }

}
