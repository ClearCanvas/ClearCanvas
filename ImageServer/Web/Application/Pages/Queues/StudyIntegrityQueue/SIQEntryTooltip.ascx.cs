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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    public partial class SIQEntryTooltip : System.Web.UI.UserControl
    {
        public StudyIntegrityQueueSummary SIQItem { get; set; }

        public string CssClass { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Container.CssClass = CssClass;
        }

        public override void DataBind()
        {
            base.DataBind();

            if (SIQItem != null)
            {
                FilesystemPath.Text = SIQItem.GetFilesystemStudyPath();
                ReconcilePath.Text = SIQItem.GetReconcileFolderPath();

                if (!SIQItem.CanReconcile)
                {
                    Note.Text = SIQItem.GetNotReconcilableReason();
                }

                if (SIQItem.StudyExists)
                {
                    StudyLink.NavigateUrl = HtmlUtility.ResolveStudyDetailsUrl(Page,
                                                SIQItem.StudySummary.ThePartition.AeTitle, SIQItem.StudyInstanceUid);

                    
                    StudyLink.Text = string.Format("{0}, {1}",
                                        SIQItem.StudySummary.AccessionNumber, 
                                        SIQItem.StudySummary.StudyDescription);

                }

            }

            
        }
    }
}