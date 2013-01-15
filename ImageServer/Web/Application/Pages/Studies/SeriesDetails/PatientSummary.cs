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

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails
{
    /// <summary>
    /// Model object behind the <see cref="PatientSummaryPanel"/>
    /// </summary>
    public class PatientSummary
    {
        #region Private members
        private string _patientId;
        private string _issuerOfPatientId;
        private string _patientName;
        private string _birthdate;
        private string _sex;

        #endregion Private members

        #region Public Properties
        public string PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public string PatientName
        {
            get { return _patientName; }
            set { _patientName = value; }
        }

        public string Birthdate
        {
            get { return _birthdate; }
            set { _birthdate = value; }
        }

        public string Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }

        public string IssuerOfPatientId
        {
            get { return _issuerOfPatientId; }
            set { _issuerOfPatientId = value; }
        }

        #endregion Public Properties
    }
}
