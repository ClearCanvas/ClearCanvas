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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;
using Resources;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue
{

    /// <summary>
    /// Work Queue Search Panel
    /// </summary>

    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel", ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Private Members

        private ServerPartition _serverPartition;

        #endregion Private Members

        #region Events

        /// <summary>
        /// Occurs when the queue is refreshed because user clicked on the Search button.
        /// </summary>
        public event EventHandler<EventArgs> Search;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
        /// </summary>
        public ServerPartition ServerPartition
        {
            get { return _serverPartition; }
            set { _serverPartition = value; }
        }

        public Default EnclosingPage { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("ViewItemDetailsUrl")]
        public string ViewItemDetailsURL
        {
            get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.WorkQueueItemDetailsPage); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ItemListClientID")]
        public string ItemListClientID
        {
            get { return workQueueItemList.WorkQueueItemGridView.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ViewDetailsButtonClientID")]
        public string ViewDetailsButtonClientID
        {
            get { return ViewItemDetailsButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("RescheduleButtonClientID")]
        public string RescheduleButtonClientID
        {
            get { return RescheduleItemButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ResetButtonClientID")]
        public string ResetButtonClientID
        {
            get { return ResetItemButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteItemButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ReprocessButtonClientID")]
        public string ReprocessButtonClientID
        {
            get { return ReprocessItemButton.ClientID; }
        }

        #endregion Public Properties

        #region Protected Methods

        internal void Reset()
        {
            Clear();
            workQueueItemList.Reset();
        }

        protected override void OnInit(EventArgs e)
        {

            base.OnInit(e);

            ClearScheduleDateButton.OnClientClick = ScriptHelper.ClearDate(ScheduleDate.ClientID, ScheduleCalendarExtender.ClientID);

            // setup child controls
            GridPagerTop.InitializeGridPager(SR.GridPagerWorkQueueSingleItem, SR.GridPagerWorkQueueMultipleItems, workQueueItemList.WorkQueueItemGridView,
                                             () => workQueueItemList.ResultCount, ImageServerConstants.GridViewPagerPosition.Top);
            workQueueItemList.Pager = GridPagerTop;

            workQueueItemList.ServerPartition = _serverPartition;

            workQueueItemList.DataSourceCreated += delegate(WorkQueueDataSource source)
                                                       {
                                                           if (!String.IsNullOrEmpty(PatientName.TrimText))
                                                                    source.PatientsName = SearchHelper.NameWildCard(PatientName.TrimText);

                                                           source.Partition = ServerPartition;

                                                           if (!String.IsNullOrEmpty(PatientId.TrimText))
                                                                    source.PatientId = SearchHelper.TrailingWildCard(PatientId.TrimText);

                                                           if (!String.IsNullOrEmpty(ProcessingServer.TrimText))
                                                                    source.ProcessingServer = SearchHelper.TrailingWildCard(ProcessingServer.TrimText);

                                                                source.ScheduledDate = !string.IsNullOrEmpty(ScheduleDate.Text) ? ScheduleDate.Text : string.Empty;                                   

                                                           source.DateFormats = ScheduleCalendarExtender.Format;

                                                           if (TypeListBox.SelectedIndex > -1)
                                                           {
                                                               var types = new List<WorkQueueTypeEnum>();
                                                               foreach (ListItem item in TypeListBox.Items)
                                                               {
                                                                   if (item.Selected)
                                                                   {
                                                                       types.Add(WorkQueueTypeEnum.GetEnum(item.Value));
                                                                   }
                                                               }
                                                               source.TypeEnums = types.ToArray();
                                                           }

                                                           if (StatusListBox.SelectedIndex > -1)
                                                           {
                                                               var statuses = new List<WorkQueueStatusEnum>();
                                                               foreach (ListItem item in StatusListBox.Items)
                                                               {
                                                                   if (item.Selected)
                                                                   {
                                                                            statuses.Add(WorkQueueStatusEnum.GetEnum(item.Value));
                                                                   }
                                                               }
                                                               source.StatusEnums = statuses.ToArray();
                                                           }

                                                           if (PriorityDropDownList.SelectedValue != string.Empty)
                                                                    source.PriorityEnum = WorkQueuePriorityEnum.GetEnum(PriorityDropDownList.SelectedValue);
                                                       };

            MessageBox.Confirmed += delegate
                                        {
                                            workQueueItemList.RefreshCurrentPage();
                                        };
        }

        /// <summary>
        /// Handle user clicking the "Apply Filter" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            workQueueItemList.Refresh();

            EventsHelper.Fire(Search, this, EventArgs.Empty);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // re-populate the drop down lists and restore their states
            PopulateDropdownLists();

            ViewItemDetailsButton.Roles = AuthorityTokens.WorkQueue.View;
            DeleteItemButton.Roles = AuthorityTokens.WorkQueue.Delete;
            ReprocessItemButton.Roles = AuthorityTokens.WorkQueue.Reprocess;
            ResetItemButton.Roles = AuthorityTokens.WorkQueue.Reset; 
            RescheduleItemButton.Roles =AuthorityTokens.WorkQueue.Reschedule;

            if (!IsPostBack && !Page.IsAsync)
            {
                var patientId = Server.UrlDecode(Request["PatientID"]);
                var patientName = Server.UrlDecode(Request["PatientName"]);
                var processingServer = Server.UrlDecode(Request["ProcessorID"]);
                if (patientId != null || patientName != null || processingServer != null)
                {
                    PatientId.TrimText = patientId;
                    PatientName.TrimText = patientName;
                    ProcessingServer.TrimText = processingServer;

                    workQueueItemList.SetDataSource();
                    workQueueItemList.Refresh();
                }
            }
        }

        private void PopulateDropdownLists()
        {
            var workQueueTypes = WorkQueueTypeEnum.GetAll();
            var workQueueStatuses = WorkQueueStatusEnum.GetAll();
            var workQueuePriorities = WorkQueuePriorityEnum.GetAll();

            if (TypeListBox.Items.Count == 0)
            {
                foreach (WorkQueueTypeEnum t in workQueueTypes)
                {
                    TypeListBox.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(t), t.Lookup));
                }
            }
            if (StatusListBox.Items.Count == 0)
            {
                foreach (WorkQueueStatusEnum s in workQueueStatuses)
                {
                    // #10784: remove Completed status filter
                    if (s != WorkQueueStatusEnum.Completed)
                        StatusListBox.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(s), s.Lookup));
                }
            }

            if (PriorityDropDownList.Items.Count==0)
            {
                PriorityDropDownList.Items.Clear();
                PriorityDropDownList.Items.Add(new ListItem(SR.Any, string.Empty));
                foreach (WorkQueuePriorityEnum p in workQueuePriorities)
                    PriorityDropDownList.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(p), p.Lookup));
            }
        }

        public void Refresh()
        {
            workQueueItemList.RefreshCurrentPage();
            SearchUpdatePanel.Update();
        }


        protected void ResetItemButton_Click(object sender, EventArgs arg)
        {
            if (!SelectedItemExists()) return;
            EnclosingPage.ResetWorkQueueItem(workQueueItemList.SelectedDataKey);
            workQueueItemList.RefreshCurrentPage();
        }

        protected void DeleteItemButton_Click(object sender, EventArgs arg)
        {
            if (!SelectedItemExists()) return;
            EnclosingPage.DeleteWorkQueueItem(workQueueItemList.SelectedDataKey);
            workQueueItemList.RefreshCurrentPage();
        }

        protected void ReprocessItemButton_Click(object sender, EventArgs arg)
        {
            if (!SelectedItemExists()) return;
            EnclosingPage.ReprocessWorkQueueItem(workQueueItemList.SelectedDataKey);
            workQueueItemList.RefreshCurrentPage();
        }

        protected void RescheduleItemButton_Click(object sender, ImageClickEventArgs e)
        {
            if (!SelectedItemExists()) return;
            EnclosingPage.RescheduleWorkQueueItem(workQueueItemList.SelectedDataKey);
            workQueueItemList.RefreshCurrentPage();
        }

        #endregion Protected Methods

        private bool SelectedItemExists()
        {
            if (!workQueueItemList.SelectedItemExists())
            {
                MessageBox.BackgroundCSS = string.Empty;
                MessageBox.Message = SR.SelectedWorkQueueNoLongerOnTheList;
                MessageBox.MessageStyle = "color: red; font-weight: bold;";
                MessageBox.MessageType =
                    Web.Application.Controls.MessageBox.MessageTypeEnum.ERROR;
                MessageBox.Show();

                return false;
            }

            return true;
        }

        private void Clear()
        {
            PatientName.Text = string.Empty;
            PatientId.Text = string.Empty;
            PatientName.Text = string.Empty;
            ScheduleDate.Text = string.Empty;

            foreach (ListItem item in TypeListBox.Items)
            {
                if (item.Selected)
                {
                    item.Selected = false;
                }
            }

            foreach (ListItem item in StatusListBox.Items)
            {
                if (item.Selected)
                {
                    item.Selected = false;
                }
            }

            PriorityDropDownList.SelectedIndex = 0;
            ProcessingServer.Text = string.Empty;
        }
    }
}