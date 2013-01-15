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
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.Sequences
{
    /// <summary>
    /// Scheduled Procedure Step Sequence (0040,0100)
    /// </summary>
    /// <remarks>As per Dicom Doc 3, C.4-10 (pg 249)</remarks>
    public class ScheduledProcedureStepSequenceIod : SequenceIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledProcedureStepSequenceIod"/> class.
        /// </summary>
        public ScheduledProcedureStepSequenceIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledProcedureStepSequenceIod"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public ScheduledProcedureStepSequenceIod(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }
        #endregion

        #region Public Properties
        public string ScheduledStationAeTitle
        {
            get { return base.DicomAttributeProvider[DicomTags.ScheduledStationAeTitle].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ScheduledStationAeTitle].SetString(0, value); }
        }

        public string ScheduledStationName
        {
            get { return base.DicomAttributeProvider[DicomTags.ScheduledStationName].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ScheduledStationName].SetString(0, value); }
        }

        public string ScheduledProcedureStepLocation
        {
            get { return base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepLocation].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepLocation].SetString(0, value); }
        }

        public DateTime ScheduledProcedureStepStartDate
        {
            get { return base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepStartDate].GetDateTime(0, DateTime.MinValue); }
            set { base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepStartDate].SetDateTime(0, value); }
        }

        public DateTime ScheduledProcedureStepEndDate
        {
            get { return base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepEndDate].GetDateTime(0, DateTime.MinValue); }
            set { base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepEndDate].SetDateTime(0, value); }
        }

        public PersonName ScheduledPerformingPhysiciansName
        {
            get { return new PersonName(base.DicomAttributeProvider[DicomTags.ScheduledPerformingPhysiciansName].GetString(0, String.Empty)); }
            set { base.DicomAttributeProvider[DicomTags.ScheduledPerformingPhysiciansName].SetString(0, value.ToString()); }
        }

        public string ScheduledProcedureStepDescription
        {
            get { return base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepDescription].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepDescription].SetString(0, value); }
        }

        public string ScheduledProcedureStepId
        {
            get { return base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepId].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepId].SetString(0, value); }
        }

        public ScheduledProcedureStepStatus ScheduledProcedureStepStatus
        {
            get { return IodBase.ParseEnum<ScheduledProcedureStepStatus>(base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepStatus].GetString(0, String.Empty), ScheduledProcedureStepStatus.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepStatus], value, false); }
        }



        public string CommentsOnTheScheduledProcedureStep
        {
            get { return base.DicomAttributeProvider[DicomTags.CommentsOnTheScheduledProcedureStep].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.CommentsOnTheScheduledProcedureStep].SetString(0, value); }
        }

        public string Modality
        {
            get { return base.DicomAttributeProvider[DicomTags.Modality].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.Modality].SetString(0, value); }
        }

        public string RequestedContrastAgent
        {
            get { return base.DicomAttributeProvider[DicomTags.RequestedContrastAgent].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.RequestedContrastAgent].SetString(0, value); }
        }

        public string PreMedication
        {
            get { return base.DicomAttributeProvider[DicomTags.PreMedication].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.PreMedication].SetString(0, value); }
        }

        public SequenceIodList<CodeSequenceMacro> ScheduledProtocolCodeSequenceList
        {
            get { return new SequenceIodList<CodeSequenceMacro>(DicomAttributeProvider[DicomTags.ScheduledProtocolCodeSequence] as DicomAttributeSQ); }
        }
       #endregion

        #region Public Methods
        /// <summary>
        /// Sets the common tags for a typical Modality Worklist Request.
        /// </summary>
        public void SetCommonTags()
        {
            SetCommonTags(this);
        }
        #endregion

        #region Public Static Methods


        /// <summary>
        /// Sets the common tags for a typical Modality Worklist Request.
        /// </summary>
		/// <param name="scheduledProcedureStepSequenceIod">The scheduled step attributes sequence iod.</param>
        public static void SetCommonTags(ScheduledProcedureStepSequenceIod scheduledProcedureStepSequenceIod)
        {
            scheduledProcedureStepSequenceIod.SetAttributeNull(DicomTags.Modality);
            scheduledProcedureStepSequenceIod.SetAttributeNull(DicomTags.ScheduledProcedureStepId);
            scheduledProcedureStepSequenceIod.SetAttributeNull(DicomTags.ScheduledProcedureStepDescription);
            scheduledProcedureStepSequenceIod.SetAttributeNull(DicomTags.ScheduledStationAeTitle);
            scheduledProcedureStepSequenceIod.SetAttributeNull(DicomTags.ScheduledProcedureStepStartDate);
            scheduledProcedureStepSequenceIod.SetAttributeNull(DicomTags.ScheduledProcedureStepStartTime);
            scheduledProcedureStepSequenceIod.SetAttributeNull(DicomTags.ScheduledPerformingPhysiciansName);
            scheduledProcedureStepSequenceIod.SetAttributeNull(DicomTags.ScheduledProcedureStepLocation);
            scheduledProcedureStepSequenceIod.SetAttributeNull(DicomTags.ScheduledProcedureStepStatus);
            scheduledProcedureStepSequenceIod.SetAttributeNull(DicomTags.CommentsOnTheScheduledProcedureStep);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ScheduledProcedureStepStatus
    {
        /// <summary>
        /// No status, or empty value
        /// </summary>
        None,
        /// <summary>
        /// Procedure Step scheduled
        /// </summary>
        Scheduled,
        /// <summary>
        /// patient is available for the Scheduled Procedure Step
        /// </summary>
        Arrived,
        /// <summary>
        /// all patient and other necessary preparation for this step has been completed
        /// </summary>
        Ready,
        /// <summary>
        /// at least one Performed Procedure Step has been created that references this Scheduled Procedure Step
        /// </summary>
        Started
    }
}
