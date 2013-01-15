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
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.MoveSeries
{
    public partial class SeriesGridView : System.Web.UI.UserControl
    {
        private IList<Series> _seriesList = new List<Series>();
        private ServerPartition _partition;
        private Study _study;

        public IList<Series> SeriesList
        {
            get { return _seriesList; }
            set { _seriesList = value;}
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            SeriesListControl.DataSource = _seriesList;
            SeriesListControl.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (GridViewRow row in SeriesListControl.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int index = SeriesListControl.PageIndex * SeriesListControl.PageSize + row.RowIndex;
                    Series series = SeriesList[index];

                    if (series != null)
                    {

                        row.Attributes.Add("instanceuid", series.SeriesInstanceUid);
                        row.Attributes.Add("serverae", Partition.AeTitle);
                        StudyController controller = new StudyController();
                        row.Attributes.Add("deleted", "true");
                    }
                }
            }
        }
    }
}
