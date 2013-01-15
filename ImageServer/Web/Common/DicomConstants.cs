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

using System.Web.UI;

public class DicomConstants
{
    public const string DicomDate = "yyyyMMdd";
    public const string DicomDateTime = "YYYYMMDDHHMMSS.FFFFFF";
    public const string DicomSeparator = "^";
    public const string Male = "M";
    public const string Female = "F";
    public const string Other = "O";
    
    public class DicomTags {
        public const string PatientsName = "00100010";
        public const string PatientID = "00100020";
        public const string PatientsBirthDate = "00100030";
        public const string PatientsSex = "00100040";
        public const string PatientsAge = "00101010";
        public const string ReferringPhysician = "00080090";
        public const string StudyDate = "00080020";
        public const string StudyTime = "00080030";
        public const string AccessionNumber = "00080050";
        public const string StudyDescription = "00081030";
        public const string StudyInstanceUID = "0020000D";
        public const string StudyID = "00200010";
        public const string IssuerOfPatientID = "00100021";
    }
}
