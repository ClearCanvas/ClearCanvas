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

using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails
{
    static public class PatientSummaryAssembler
    {
        /// <summary>
        /// Returns an instance of <see cref="PatientSummary"/> for a <see cref="Study"/>.
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        /// <remark>
        /// 
        /// </remark>
        static public PatientSummary CreatePatientSummary(Study study)
        {
            PatientSummary patient = new PatientSummary();

            patient.PatientId = study.PatientId;
            patient.Birthdate = study.PatientsBirthDate;
            patient.PatientName = study.PatientsName;
            patient.Sex = study.PatientsSex;

            PatientAdaptor adaptor = new PatientAdaptor();
            Patient pat = adaptor.Get(study.PatientKey);
            patient.IssuerOfPatientId = pat.IssuerOfPatientId;
            return patient;
        }
    }
}
