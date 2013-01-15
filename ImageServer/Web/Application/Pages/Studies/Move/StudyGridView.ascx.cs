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

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move
{
    public partial class StudyGridView : System.Web.UI.UserControl
    {
        private IList<Study> _studyList = new List<Study>();
        private ServerPartition _partition;

        public IList<Study> StudyList
        {
            get { return _studyList; }
            set { _studyList = value;
                  StudyListControl.DataSource = _studyList;
            }
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            StudyListControl.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (GridViewRow row in StudyListControl.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int index = StudyListControl.PageIndex * StudyListControl.PageSize + row.RowIndex;
                    Study study = StudyList[index];

                    if (study != null)
                    {

                        row.Attributes.Add("instanceuid", study.StudyInstanceUid);
                        row.Attributes.Add("serverae", Partition.AeTitle);
                        StudyController controller = new StudyController();
                        bool deleted = controller.IsScheduledForDelete(study);
                        if (deleted)
                            row.Attributes.Add("deleted", "true");
                    }
                }
            }
        }
    }
}