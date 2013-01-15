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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Core.Edit;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    public partial class EditWorkQueueDetailsView : WorkQueueDetailsViewBase
    {
   
        #region Private members

        #endregion Private members

        #region Public Properties

        /// <summary>
        /// Sets or gets the width of work queue details view panel
        /// </summary>
        public override Unit Width
        {
            get { return base.Width; }
            set
            {
                base.Width = value;
                EditInfoDetailsView.Width = value;
            }
        }

        public UpdateItem[] UpdateItems { get; set; }

        #endregion Public Properties

        #region Protected Methods

        #endregion Protected Methods

        #region Public Methods

        public override void DataBind()
        {
            if (WorkQueue != null)
            {
                var details = WorkQueueDetailsAssembler.CreateWorkQueueDetail(WorkQueue);
                UpdateItems = details.EditUpdateItems;
                var detailsList = new List<WorkQueueDetails>
                                      {
                                          details
                                      };
                EditInfoDetailsView.DataSource = detailsList;
            }
            else
                EditInfoDetailsView.DataSource = null;


            base.DataBind();
        }


        protected void GeneralInfoDetailsView_DataBound(object sender, EventArgs e)
        {
            var item = EditInfoDetailsView.DataItem as WorkQueueDetails;
            if (item != null)
            {
            }
        }

        #endregion Public Methods
        
    }
}