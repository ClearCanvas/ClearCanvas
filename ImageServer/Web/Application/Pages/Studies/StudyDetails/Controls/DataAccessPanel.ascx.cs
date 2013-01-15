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
using System.Threading;
using System.Web.UI.WebControls;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class DataAccessPanel : System.Web.UI.UserControl
    {
        #region Private members

        #endregion Private members

        #region Public properties

        /// <summary>
        /// The Study to get the DataAccess Groups for.
        /// </summary>
        public StudySummary Study { get; set; }

        public ServerPartition Partition { get; set; }

        #endregion Public properties

        #region Overridden Public Methods

        public override void DataBind()
        {
            if (Study != null)
            {
                if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens.Study.EditDataAccess))
                {
                    StudyDataAccessController controller = new StudyDataAccessController();

                    var dataAccessGroupList = CollectionUtils.Sort(controller.ListDataAccessGroupsForStudy(Study.TheStudyStorage.Key), Compare);
                    UpdatableDataAccessGroupsGridView.DataSource = dataAccessGroupList;


                    var tokenAccessGroupList = CollectionUtils.Sort(controller.ListAuthorityGroupsForStudyViaToken(Study.TheStudyStorage), Compare);
                    OtherGroupsWithAccessGridView.DataSource = tokenAccessGroupList;
                    OtherGroupsListing.Visible = tokenAccessGroupList.Count > 0;

                    LinkToOtherGroupListing.Visible = dataAccessGroupList.Count > 10;

                }
            }

            base.DataBind();
        }
        
        #endregion

        #region Protected methods

        
        protected void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            UpdatableDataAccessGroupsGridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        #endregion Protected methods

        #region Helper Methods

        private static int Compare(AuthorityGroupStudyAccessInfo x, AuthorityGroupStudyAccessInfo y)
        {
            if (x.CanAccessToAllStudies && !y.CanAccessToAllStudies)
                return -1; // x first

            if (!x.CanAccessToAllStudies && y.CanAccessToAllStudies)
                return 1; // y first

            return x.Name.CompareTo(y.Name); // alphabetically
        }

        #endregion

    }

}