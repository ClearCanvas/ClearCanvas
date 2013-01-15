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
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails;
using ClearCanvas.ImageServer.Web.Common.Data;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Scripts.WorkQueueGridView.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.WorkQueueGridView",
                       ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Scripts.WorkQueueGridView.js")]
    
    /// <summary>
    /// list panel within the <see cref="StudyDetailsPanel"/> that contains all of the work queue
    /// entries for this study.
    /// </summary>
    public partial class WorkQueueGridView : ScriptUserControl
    {
        #region Private members

        private Study _study = null;

    	#endregion Private members

        #region Public properties

        /// <summary>
        /// Gets or sets the list of series to be displayed
        /// </summary>
        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }      

        public Web.Common.WebControls.UI.GridView WorkQueueListControl
        {
            get { return StudyWorkQueueGridView; }
        }

        #endregion Public properties

        #region Constructors

        public WorkQueueGridView()
            : base(false, HtmlTextWriterTag.Div)
            {
            }

        #endregion Constructors

            
        #region Protected methods

        protected void StudyWorkQueueGridView_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void StudyWorkQueueGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            StudyWorkQueueGridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        #endregion Protected methods

        #region Public methods

        public override void DataBind()
        {
            if (Study != null)
            {
                StudyController controller = new StudyController();
                StudyWorkQueueGridView.DataSource = controller.GetWorkQueueItems(Study);
            }

            base.DataBind();
        }

        #endregion Public methods

    }
}