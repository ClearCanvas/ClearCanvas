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
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using Resources;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Data.Model;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly:
    WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.js",
        "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    /// <summary>
    /// Represents an event fired when the View Details button is clicked
    /// </summary>
    public class DeletedStudyViewDetailsClickedEventArgs : EventArgs
    {
        public DeletedStudyInfo DeletedStudyInfo { get; set; }
    }

    /// <summary>
    /// Represents an event fired when the Delete button is clicked
    /// </summary>
    public class DeletedStudyDeleteClickedEventArgs : EventArgs
    {
        public DeletedStudyInfo SelectedItem { get; set; }
    }

    [ClientScriptResource(
        ComponentType =
            "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel",
        ResourcePath =
            "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.js")]
    public partial class DeletedStudiesSearchPanel : AJAXScriptControl
    {
        #region Private Fields

        private EventHandler<DeletedStudyDeleteClickedEventArgs> _deleteClicked;
        private EventHandler<DeletedStudyViewDetailsClickedEventArgs> _viewDetailsClicked;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when user clicks on the View Details button
        /// </summary>
        public event EventHandler<DeletedStudyViewDetailsClickedEventArgs> ViewDetailsClicked
        {
            add { _viewDetailsClicked += value; }
            remove { _viewDetailsClicked -= value; }
        }

        /// <summary>
        /// Occurs when user clicks on the Delete button
        /// </summary>
        public event EventHandler<DeletedStudyDeleteClickedEventArgs> DeleteClicked
        {
            add { _deleteClicked += value; }
            remove { _deleteClicked -= value; }
        }

        #endregion

        #region Ajax Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ViewDetailsButtonClientID")]
        public string ViewDetailsButtonClientID
        {
            get { return ViewStudyDetailsButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ListClientID")]
        public string ListClientID
        {
            get { return SearchResultGridView1.GridViewControl.ClientID; }
        }

        #endregion

        #region Private Methods

        private void DataSource_ObjectCreated(object sender, ObjectDataSourceEventArgs e)
        {
            var dataSource = e.ObjectInstance as DeletedStudyDataSource;
            if (dataSource != null)
            {
                dataSource.AccessionNumber = SearchHelper.TrailingWildCard(AccessionNumber.TrimText);
                dataSource.PatientsName = SearchHelper.NameWildCard(PatientName.TrimText);
                dataSource.PatientId = SearchHelper.TrailingWildCard(PatientId.TrimText);
                dataSource.StudyDescription = SearchHelper.LeadingAndTrailingWildCard(StudyDescription.TrimText);
                dataSource.DeletedBy = DeletedBy.TrimText;
                if (!String.IsNullOrEmpty(StudyDate.Text))
                {
                    DateTime value;
                    if (DateTime.TryParseExact(StudyDate.Text, StudyDateCalendarExtender.Format,
                                               CultureInfo.InvariantCulture, DateTimeStyles.None,
                                               out value))
                    {
                        dataSource.StudyDate = value;
                    }
                }
            }
        }

        #endregion

        #region Overridden Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ClearStudyDateButton.OnClientClick = ScriptHelper.ClearDate(StudyDate.ClientID,
                                                                        StudyDateCalendarExtender.ClientID);

            GridPagerTop.InitializeGridPager(Labels.GridPagerQueueSingleItem, Labels.GridPagerQueueMultipleItems,
                                             SearchResultGridView1.GridViewControl,
                                             () => SearchResultGridView1.ResultCount,
                                             ImageServerConstants.GridViewPagerPosition.Top);
            SearchResultGridView1.Pager = GridPagerTop;
            GridPagerTop.Reset();

            SearchResultGridView1.DataSourceContainer.ObjectCreated += DataSource_ObjectCreated;

            DeleteButton.Roles =
                AuthorityTokens.Admin.StudyDeleteHistory.Delete;
            ViewStudyDetailsButton.Roles =
                AuthorityTokens.Admin.StudyDeleteHistory.View;
        }

        #endregion

        #region Protected Methods

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            Refresh();
        }

        protected void ViewDetailsButtonClicked(object sender, ImageClickEventArgs e)
        {
            var args = new DeletedStudyViewDetailsClickedEventArgs
                           {
                               DeletedStudyInfo = SearchResultGridView1.SelectedItem
                           };
            EventsHelper.Fire(_viewDetailsClicked, this, args);
        }

        protected void DeleteButtonClicked(object sender, ImageClickEventArgs e)
        {
            var args = new DeletedStudyDeleteClickedEventArgs
                           {
                               SelectedItem = SearchResultGridView1.SelectedItem
                           };
            EventsHelper.Fire(_deleteClicked, this, args);
        }

        #endregion

        #region Public Methods

        public void Refresh()
        {
            SearchResultGridView1.RefreshCurrentPage();
            SearchUpdatePanel.Update();
        }

        #endregion
    }
}