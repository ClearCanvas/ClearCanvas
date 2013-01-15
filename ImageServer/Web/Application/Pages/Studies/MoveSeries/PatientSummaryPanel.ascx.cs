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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using SR = Resources.SR;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.MoveSeries
{
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
            if (_patientSummary != null)
            {
                personName.PersonName = _patientSummary.PatientName;
                PatientDOB.Value = _patientSummary.Birthdate;

                DateTime? bdate = DateParser.Parse(_patientSummary.Birthdate);

                if (bdate!=null)
                {
                    //TODO: Fix this. The patient's age should not changed whether the study is viewed today or next year.
                    TimeSpan age = Platform.Time - bdate.Value;
                    if (age > TimeSpan.FromDays(365))
                    {
                        PatientAge.Text = String.Format("{0:0} {1}", age.TotalDays / 365, SR.Years);
                    }
                    else if (age > TimeSpan.FromDays(30))
                    {
                        PatientAge.Text = String.Format("{0:0} {1}", age.TotalDays / 30, SR.Months);
                    }
                    else
                    {
                        PatientAge.Text = String.Format("{0:0} {1}", age.TotalDays, SR.Days);
                    }
                }
                else
                {
                    PatientAge.Text = SR.Unknown;
                }


                if (String.IsNullOrEmpty(_patientSummary.Sex))
                    PatientSex.Text = SR.Unknown;
                else
                {
                    if (_patientSummary.Sex.StartsWith("F"))
                        PatientSex.Text = SR.Female;
                    else if (_patientSummary.Sex.StartsWith("M"))
                        PatientSex.Text = SR.Male;
                    else
                        PatientSex.Text = SR.Unknown;
                }


                PatientId.Text = _patientSummary.PatientId;

            }

            base.DataBind();
        }

        #endregion Protected methods
    }
}