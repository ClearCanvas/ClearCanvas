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
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// Print Job Module as per Part 3 Table C.13-8 page 873
    /// </summary>
    public class PrintJobModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PrintJobModuleIod"/> class.
        /// </summary>
        public PrintJobModuleIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintJobModuleIod"/> class.
        /// </summary>
		public PrintJobModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the execution status of print job.
        /// </summary>
        /// <value>The execution status.</value>
        public ExecutionStatus ExecutionStatus
        {
            get { return IodBase.ParseEnum<ExecutionStatus>(base.DicomAttributeProvider[DicomTags.ExecutionStatus].GetString(0, String.Empty), ExecutionStatus.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.ExecutionStatus], value, false); }
        }

        /// <summary>
        /// Gets or sets the execution status info.
        /// <para> Additional information about <see cref="ExecutionStatus"/> (2100,0020). </para>
        /// <para>Defined Terms when the Execution Status is DONE or PRINTING: NORMAL</para>
        /// <para>Defined Terms when the Execution Status is FAILURE: </para>
        /// <para>INVALID PAGE DES = The specified page layout cannot be printed or other page description errors have been detected.</para>
        /// <para>INSUFFIC MEMORY = There is not enough memory available to complete this job.</para>
		/// <para>See Section C.13.9.1 for additional Defined Terms when the Execution Status is PENDING or FAILURE.</para>
        /// </summary>
        /// <value>The execution status info.</value>
        public string ExecutionStatusInfo
        {
            get { return base.DicomAttributeProvider[DicomTags.ExecutionStatusInfo].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ExecutionStatusInfo].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the date of print job creation.
        /// </summary>
        /// <value>The creation date.</value>
        public DateTime? CreationDate
        {
            get { return DateTimeParser.ParseDateAndTime(base.DicomAttributeProvider, 0, DicomTags.CreationDate, DicomTags.CreationTime); }
        
          set { DateTimeParser.SetDateTimeAttributeValues(value, base.DicomAttributeProvider, 0, DicomTags.CreationDate, DicomTags.CreationTime); }
        }

        /// <summary>
        /// Gets or sets the print priority.
        /// </summary>
        /// <value>The print priority.</value>
        public PrintPriority PrintPriority
        {
            get { return IodBase.ParseEnum<PrintPriority>(base.DicomAttributeProvider[DicomTags.PrintPriority].GetString(0, String.Empty), PrintPriority.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.PrintPriority], value, false); }
        }

        /// <summary>
        /// Gets or sets the user defined name identifying the printer.
        /// </summary>
        /// <value>The name of the printer.</value>
        public string PrinterName
        {
            get { return base.DicomAttributeProvider[DicomTags.PrinterName].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.PrinterName].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the DICOM Application Entity Title that issued the print operation.
        /// </summary>
        /// <value>The originator.</value>
        public string Originator
        {
            get { return base.DicomAttributeProvider[DicomTags.Originator].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.Originator].SetString(0, value); }
        }
        
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the commonly used tags in the base dicom attribute collection.
        /// </summary>
        public void SetCommonTags()
        {
            SetCommonTags(base.DicomAttributeProvider);
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Sets the commonly used tags in the specified dicom attribute collection.
        /// </summary>
        public static void SetCommonTags(IDicomAttributeProvider dicomAttributeProvider)
        {
            if (dicomAttributeProvider == null)
				throw new ArgumentNullException("dicomAttributeProvider");

			dicomAttributeProvider[DicomTags.CreationDate].SetNullValue();
			dicomAttributeProvider[DicomTags.PrinterName].SetNullValue();
			dicomAttributeProvider[DicomTags.Originator].SetNullValue();
			dicomAttributeProvider[DicomTags.PrintPriority].SetNullValue();
			dicomAttributeProvider[DicomTags.ExecutionStatus].SetNullValue();
			dicomAttributeProvider[DicomTags.ExecutionStatusInfo].SetNullValue();
        }
        #endregion
    }

    #region ExecutionStatus Enum
    /// <summary>
    /// Execution status of print job.
    /// </summary>
    public enum ExecutionStatus
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        Pending,
        /// <summary>
        /// 
        /// </summary>
        Printing,
        /// <summary>
        /// 
        /// </summary>
        Done,
        /// <summary>
        /// 
        /// </summary>
        Failure
    }
    #endregion
    
}

