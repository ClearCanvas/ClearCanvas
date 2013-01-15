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
using System.Collections.Generic;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails
{
    /// <summary>
    /// The series details view panel within the <see cref="SeriesDetailsPanel"/>
    /// </summary>
    public partial class SeriesDetailsView : System.Web.UI.UserControl
    {
        #region Private members
        private Model.Series _series;

        #endregion Private members

        #region Public Properties

        public Model.Series Series
        {
            get { return _series; }
            set { _series = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Series!=null)
            {
                IList<SeriesDetails> seriesDetails = new List<SeriesDetails>();
                seriesDetails.Add(SeriesDetailsAssembler.CreateSeriesDetails(Series));
                DetailsView1.DataSource = seriesDetails;
                DetailsView1.DataBind();
            }
        }

        #endregion Protected Methods

        protected void DetailsView1_DataBound(object sender, EventArgs e)
        {
            SeriesDetails series = DetailsView1.DataItem as SeriesDetails;
        }
    }
}