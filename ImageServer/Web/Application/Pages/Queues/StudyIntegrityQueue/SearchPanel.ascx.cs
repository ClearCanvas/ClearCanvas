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
using ClearCanvas.ImageServer.Common.Authentication;
using ClearCanvas.ImageServer.Model;
using Resources;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly:
    WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel.js",
        "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    [ClientScriptResource(
        ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel",
        ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Private members

        #endregion Private members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("ReconcileButtonClientID")]
        public string ReconcileButtonClientID
        {
            get { return ReconcileButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ItemListClientID")]
        public string ItemListClientID
        {
            get { return StudyIntegrityQueueItemList.StudyIntegrityQueueGrid.ClientID; }
        }

        /// <summary>
        /// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
        /// </summary>
        public ServerPartition ServerPartition { get; set; }

        #endregion Public Properties  

        #region Public Methods

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientName.Text = string.Empty;
            PatientId.Text = string.Empty;
            AccessionNumber.Text = string.Empty;
            FromDate.Text = string.Empty;
            ToDate.Text = string.Empty;
        }

        public void Refresh()
        {
            SearchUpdatePanel.Update();
            StudyIntegrityQueueItemList.RefreshCurrentPage();
        }

        internal void Reset()
        {
            Clear();
            StudyIntegrityQueueItemList.Reset();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ClearFromDateButton.OnClientClick = ScriptHelper.ClearDate(FromDate.ClientID,
                                                                       FromDateCalendarExtender.ClientID);
            ClearToDateButton.OnClientClick = ScriptHelper.ClearDate(ToDate.ClientID,
                                                                     ToDateCalendarExtender.ClientID);
            SearchButton.Attributes["onclick"] = ScriptHelper.CheckDateRange(FromDate.ClientID, ToDate.ClientID,
																		SR.ToFromDateValidationError);

            GridPagerTop.InitializeGridPager(Labels.GridPagerQueueSingleItem,
                                             Labels.GridPagerQueueMultipleItems,
                                             StudyIntegrityQueueItemList.StudyIntegrityQueueGrid,
                                             () => StudyIntegrityQueueItemList.ResultCount,
                                             ImageServerConstants.GridViewPagerPosition.Top);
            StudyIntegrityQueueItemList.Pager = GridPagerTop;

            StudyIntegrityQueueItemList.DataSourceCreated += delegate(StudyIntegrityQueueDataSource source)
                                                                 {
                                                                     source.Partition = ServerPartition;

                                                                     if (!String.IsNullOrEmpty(PatientName.TrimText))
                                                                         source.PatientName = SearchHelper.NameWildCard(PatientName.TrimText);
                                                                     if (!String.IsNullOrEmpty(PatientId.TrimText))
                                                                         source.PatientId = SearchHelper.TrailingWildCard(PatientId.TrimText);
                                                                     if (!String.IsNullOrEmpty(AccessionNumber.TrimText))
                                                                         source.AccessionNumber = SearchHelper.TrailingWildCard(AccessionNumber.TrimText);
                                                                     if (!String.IsNullOrEmpty(FromDate.Text))
                                                                         source.FromInsertTime = FromDate.Text;

                                                                     if (!String.IsNullOrEmpty(ToDate.Text))
                                                                         source.ToInsertTime = ToDate.Text;

                                                                     if (ReasonListBox.SelectedIndex > -1)
                                                                     {
                                                                         var reasonEnums =
                                                                             new List<StudyIntegrityReasonEnum>();
                                                                         foreach (ListItem item in ReasonListBox.Items)
                                                                         {
                                                                             if (item.Selected)
                                                                             {
                                                                                 reasonEnums.Add(
                                                                                     StudyIntegrityReasonEnum.GetEnum(
                                                                                         item.Value));
                                                                             }
                                                                         }

                                                                         source.ReasonEnum = reasonEnums;
                                                                     }
                                                                 };

            ReconcileButton.Roles =
                AuthorityTokens.StudyIntegrityQueue.Reconcile;

            List<StudyIntegrityReasonEnum> reasons = StudyIntegrityReasonEnum.GetAll();
            foreach (StudyIntegrityReasonEnum reason in reasons)
            {
                ReasonListBox.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(reason), reason.Lookup));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !Page.IsAsync)
            {
                var patientId = Server.UrlDecode(Request["PatientID"]);
                var patientName = Server.UrlDecode(Request["PatientName"]);
                var reason = Server.UrlDecode(Request["Reason"]);
                var databind = Server.UrlDecode(Request["Databind"]);
                if (patientId != null || patientName != null || reason != null || databind != null)
                {
                    PatientId.TrimText = patientId;
                    PatientName.TrimText = patientName;
                    if (reason != null)
                        ReasonListBox.Items.FindByValue(reason).Selected = true;

                    StudyIntegrityQueueItemList.SetDataSource();
                    StudyIntegrityQueueItemList.Refresh();
                }
            }
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            StudyIntegrityQueueItemList.Refresh();
        }

        protected void ReconcileButton_Click(object sender, EventArgs e)
        {
            var list = StudyIntegrityQueueItemList.SelectedItems;
            if (list == null || list.Count == 0)
            {
                StudyIntegrityQueueItemList.Refresh();
                return;
            }

            ReconcileDetails details =
                ReconcileDetailsAssembler.CreateReconcileDetails(list[0]);

            ((Default) Page).OnReconcileItem(details);
        }

        #endregion Protected Methods
    }
}