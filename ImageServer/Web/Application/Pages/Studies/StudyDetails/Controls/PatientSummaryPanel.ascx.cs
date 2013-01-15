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
using System.Web.UI;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code;
using SR = Resources.SR;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    /// <summary>
    /// Patient summary information panel within the <see cref="StudyDetailsPanel"/> 
    /// </summary>
    public partial class PatientSummaryPanel : UserControl
    {
        #region private members
        private PatientSummary _patientSummary;
        #endregion private members


        #region Public Properties
        /// <summary>
        /// Gets or sets the <see cref="PatientSummary"/> object used by the panel.
        /// </summary>
        public PatientSummary PatientSummary
        {
            get { return _patientSummary; }
            set { _patientSummary = value; }
        }

        #endregion Public Properties


        #region Protected methods

        public override void DataBind()
        {
            base.DataBind();
            if (_patientSummary != null)
            {

                personName.PersonName = _patientSummary.PatientName;
                PatientDOB.Value = _patientSummary.Birthdate;
                
				if (String.IsNullOrEmpty(_patientSummary.PatientsAge))
                    PatientAge.Text = SR.Unknown;
				else
                {
                    string patientAge = _patientSummary.PatientsAge.Substring(0, Math.Min(3,_patientSummary.PatientsAge.Length)).TrimStart('0');
                    if (_patientSummary.PatientsAge.Length > 3)
                    {
                        switch (_patientSummary.PatientsAge.Substring(3))
                        {
                            case "Y":
                                patientAge += " " + SR.Years;
                                break;
                            case "M":
                                patientAge += " " + SR.Months;
                                break;
                            case "W":
                                patientAge += " " + SR.Weeks;
                                break;
                            default:
                                patientAge += " " + SR.Days;
                                break;
                        }

                        if (_patientSummary.PatientsAge.Substring(0, Math.Min(3, _patientSummary.PatientsAge.Length)).
                                Equals("001"))
                            patientAge = patientAge.TrimEnd('s');
                    }
                    PatientAge.Text = patientAge;
				}


            	if (String.IsNullOrEmpty(_patientSummary.Sex))
                    PatientSex.Text = SR.Unknown;
                else
                {
                    if (_patientSummary.Sex.StartsWith("F"))
                        PatientSex.Text = SR.Female;
                    else if (_patientSummary.Sex.StartsWith("M"))
                        PatientSex.Text = SR.Male;
                    else if (_patientSummary.Sex.StartsWith("O"))
                        PatientSex.Text = SR.Other;
                    else
                        PatientSex.Text = SR.Unknown;
                }


                PatientId.Text = _patientSummary.PatientId;

            }

        }

        #endregion Protected methods
    }
}