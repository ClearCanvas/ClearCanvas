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
    /// Printer Module as per Part 3 Table C.13-9, page 872
    /// </summary>
    public class PrinterModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PrinterModuleIod"/> class.
        /// </summary>
        public PrinterModuleIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrinterModuleIod"/> class.
        /// </summary>
		public PrinterModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the printer status.
        /// </summary>
        /// <value>The printer status.</value>
        public PrinterStatus PrinterStatus
        {
            get { return IodBase.ParseEnum<PrinterStatus>(base.DicomAttributeProvider[DicomTags.PrinterStatus].GetString(0, String.Empty), PrinterStatus.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.PrinterStatus], value, false); }
        }

        /// <summary>
        /// Gets or sets the printer status info.
        /// </summary>
        /// <value>The printer status info.</value>
        public string PrinterStatusInfo
        {
            get { return base.DicomAttributeProvider[DicomTags.PrinterStatusInfo].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.PrinterStatusInfo].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the name of the printer.
        /// </summary>
        /// <value>The name of the printer.</value>
        public string PrinterName
        {
            get { return base.DicomAttributeProvider[DicomTags.PrinterName].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.PrinterName].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the manufacturer.
        /// </summary>
        /// <value>The manufacturer.</value>
        public string Manufacturer
        {
            get { return base.DicomAttributeProvider[DicomTags.Manufacturer].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.Manufacturer].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the name of the manufacturers model.
        /// </summary>
        /// <value>The name of the manufacturers model.</value>
        public string ManufacturersModelName
        {
            get { return base.DicomAttributeProvider[DicomTags.ManufacturersModelName].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ManufacturersModelName].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the device serial number.
        /// </summary>
        /// <value>The device serial number.</value>
        public string DeviceSerialNumber
        {
            get { return base.DicomAttributeProvider[DicomTags.DeviceSerialNumber].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.DeviceSerialNumber].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the software versions.
        /// </summary>
        /// <value>The software versions.</value>
        public string SoftwareVersions
        {
            get { return base.DicomAttributeProvider[DicomTags.SoftwareVersions].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.SoftwareVersions].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the date of last calibration.
        /// </summary>
        /// <value>The date of last calibration.</value>
        public DateTime? DateOfLastCalibration
        {
        	get { return DateTimeParser.ParseDateAndTime(String.Empty, 
        					base.DicomAttributeProvider[DicomTags.DateOfLastCalibration].GetString(0, String.Empty), 
                  base.DicomAttributeProvider[DicomTags.TimeOfLastCalibration].GetString(0, String.Empty)); }

                  set { DateTimeParser.SetDateTimeAttributeValues(value, base.DicomAttributeProvider[DicomTags.DateOfLastCalibration], base.DicomAttributeProvider[DicomTags.TimeOfLastCalibration]); }
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

            dicomAttributeProvider[DicomTags.PrinterStatus].SetNullValue();
            dicomAttributeProvider[DicomTags.PrinterStatusInfo].SetNullValue();
            dicomAttributeProvider[DicomTags.PrinterName].SetNullValue();
            dicomAttributeProvider[DicomTags.Manufacturer].SetNullValue();
            dicomAttributeProvider[DicomTags.ManufacturersModelName].SetNullValue();
            dicomAttributeProvider[DicomTags.DeviceSerialNumber].SetNullValue();
            dicomAttributeProvider[DicomTags.SoftwareVersions].SetNullValue();
            dicomAttributeProvider[DicomTags.DateOfLastCalibration].SetNullValue();
            dicomAttributeProvider[DicomTags.TimeOfLastCalibration].SetNullValue();

        }
        #endregion
    }

    #region PrinterStatus Enum
    /// <summary>
    /// Enumeration for Printer Status
    /// </summary>
    public enum PrinterStatus
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        Normal,
        /// <summary>
        /// 
        /// </summary>
        Warning,
        /// <summary>
        /// 
        /// </summary>
        Failure
    }
    #endregion
}
