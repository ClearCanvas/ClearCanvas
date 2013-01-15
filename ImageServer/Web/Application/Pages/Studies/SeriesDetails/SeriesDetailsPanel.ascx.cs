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

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails
{
    /// <summary>
    /// Panel within the <see cref="SeriesDetailsPage"/>
    /// </summary>
    public partial class SeriesDetailsPanel : System.Web.UI.UserControl
    {
        #region Private members
        private Model.Study _study;
        private Model.Series _series;
        #endregion Private members


        #region Public Properties

        public Model.Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public Model.Series Series
        {
            get { return _series; }
            set { _series = value; }
        }

        #endregion Public Properties

        #region Protected Properties

        public override void DataBind()
        {
            if (_study != null)
            {
                PatientSummary.PatientSummary = PatientSummaryAssembler.CreatePatientSummary(_study);

                StudySummary.Study = _study;

                if (Series != null)
                    SeriesDetails.Series = _series;

            }

            base.DataBind();
        }
        #endregion Protected Properties

    }
}