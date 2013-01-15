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
using System.Text;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Validation
{
    /// <summary>
    /// Information associated with a validation failure.
    /// </summary>
    public class ValidationStudyInfo
    {
        #region Public Properties

    	public string ServerAE { get; set; }

    	public string PatientsName { get; set; }

    	public string PatientsId { get; set; }

    	public string StudyInstaneUid { get; set; }

    	public string AccessionNumber { get; set; }

    	public string StudyDate { get; set; }

    	#endregion

        public ValidationStudyInfo(){}

        public ValidationStudyInfo(Study theStudy, ServerPartition partition)
        {
            ServerAE = partition.AeTitle;
            PatientsName = theStudy.PatientsName;
            PatientsId = theStudy.PatientId;
            StudyInstaneUid = theStudy.StudyInstanceUid;
            AccessionNumber = theStudy.AccessionNumber;
            StudyDate = theStudy.StudyDate;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("Partition : {0}", ServerAE));
            sb.AppendLine(String.Format("Patient   : {0}", PatientsName));
            sb.AppendLine(String.Format("Patient ID: {0}", PatientsId));
            sb.AppendLine(String.Format("Study UID : {0}", StudyInstaneUid));
            sb.AppendLine(String.Format("Accession#: {0}", AccessionNumber));
            sb.AppendLine(String.Format("Study Date: {0}", StudyDate));

            return sb.ToString();
        }
    }
}