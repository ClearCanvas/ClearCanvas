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
using System.Web.UI;
using AjaxControlToolkit;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly:
    WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.js",
        "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    public class WorkQueueDetailsButtonEventArg : EventArgs
    {
        public WorkQueueDetailsButtonEventArg(Model.WorkQueue item)
        {
            WorkQueueItem = item;
        }

        public Model.WorkQueue WorkQueueItem { get; set; }
    }

    [ClientScriptResource(
        ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel",
        ResourcePath =
            "ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.js")]
    /// <summary>
    /// The <see cref="WorkQueue"/> details panel
    /// </summary>
    public partial class WorkQueueItemDetailsPanel : AJAXScriptControl
    {
        #region Private members

        private EventHandler<WorkQueueDetailsButtonEventArg> _deleteClickHandler;
        private WorkQueueDetailsViewBase _detailsView;
        private EventHandler<WorkQueueDetailsButtonEventArg> _reprocessClickHandler;

        private EventHandler<WorkQueueDetailsButtonEventArg> _rescheduleClickHandler;
        private EventHandler<WorkQueueDetailsButtonEventArg> _resetClickHandler;
        private Model.WorkQueue _workQueue;

        #endregion Private members

        #region Private Properties
        protected Study ItemStudy { get; set; }
        #endregion

        #region Public Properties


        [ExtenderControlProperty]
        [ClientPropertyName("ViewStudiesButtonClientID")]
        public string ViewStudiesButtonClientID
        {
            get { return StudyDetailsButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("OpenStudyPageUrl")]
        public string OpenStudyPageUrl
        {
            get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.StudyDetailsPage); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("StudyInstanceUid")]
        public string StudyInstanceUid
        {
            get
            {
                return ItemStudy != null ? ItemStudy.StudyInstanceUid : String.Empty;
            }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ServerAE")]
        public string ServerAE
        {
            get
            {
                ServerPartition partition = ServerPartition.Load(_workQueue.ServerPartitionKey);
                return partition.AeTitle;
            }
        }

        /// <summary>
        /// Gets/Sets the <see cref="WorkQueue"/> item displayed in the panel
        /// </summary>
        /// <remarks>
        /// <see cref="DataBind"/> must be called to bind the work queue item data
        /// </remarks>
        public Model.WorkQueue WorkQueue
        {
            get { return _workQueue; }
            set { _workQueue = value; }
        }

        public bool AutoRefresh
        {
            get { return ViewState["AutoRefresh"] == null || (bool) ViewState["AutoRefresh"]; }
            set { ViewState["AutoRefresh"] = value; }
        }

        #endregion Public Properties

        #region Events

        /// <summary>
        /// Fired when user clicks on the Reschedule button
        /// </summary>
        public event EventHandler<WorkQueueDetailsButtonEventArg> RescheduleButtonClick
        {
            add { _rescheduleClickHandler += value; }
            remove { _rescheduleClickHandler -= value; }
        }

        /// <summary>
        /// Fired when user clicks on the Reset button
        /// </summary>
        public event EventHandler<WorkQueueDetailsButtonEventArg> ResetButtonClick
        {
            add { _resetClickHandler += value; }
            remove { _resetClickHandler -= value; }
        }

        /// <summary>
        /// Fired when user clicks on the Delete button
        /// </summary>
        public event EventHandler<WorkQueueDetailsButtonEventArg> DeleteButtonClick
        {
            add { _deleteClickHandler += value; }
            remove { _deleteClickHandler -= value; }
        }


        public event EventHandler<WorkQueueDetailsButtonEventArg> ReprocessButtonClick
        {
            add { _reprocessClickHandler += value; }
            remove { _reprocessClickHandler -= value; }
        }

        #endregion Events

        #region Protected Methods

        protected int GetRefreshInterval()
        {
            int interval = WorkQueueSettings.Default.NormalRefreshIntervalSeconds*1000;

            if (WorkQueue != null)
            {
                // the refresh rate should be high if the item was scheduled to start soon..
                TimeSpan span = WorkQueue.ScheduledTime.Subtract(Platform.Time);
                if (span < TimeSpan.FromMinutes(1))
                {
                    interval = WorkQueueSettings.Default.FastRefreshIntervalSeconds*1000;
                }
            }

            return interval;
        }

        public void ResetRefresh(bool enableAutoRefresh)
        {
            AutoRefresh = enableAutoRefresh;
            RefreshTimer.Reset(AutoRefresh);
        }


        protected override void OnPreRender(EventArgs e)
        {
            if (WorkQueue == null)
            {
                Visible = false;
            }

            if (RefreshTimer.Enabled)
            {
                if (WorkQueue != null)
                {
                    RefreshTimer.Interval = GetRefreshInterval();
                }
            }

            UpdateToolBarButtons();

            AutoRefreshIndicator.Visible = RefreshTimer.Enabled;
            base.OnPreRender(e);
        }

        protected void UpdateToolBarButtons()
        {
            RescheduleToolbarButton.Enabled = WorkQueue != null && WorkQueueController.CanReschedule(WorkQueue);
            ResetButton.Enabled = WorkQueue != null && WorkQueueController.CanReset(WorkQueue);
            DeleteButton.Enabled = WorkQueue != null && WorkQueueController.CanDelete(WorkQueue);
            ReprocessButton.Enabled = WorkQueue != null && WorkQueueController.CanReprocess(WorkQueue);
            StudyDetailsButton.Enabled = ItemStudy != null;
        }

        protected void Reschedule_Click(object sender, EventArgs arg)
        {
            EventsHelper.Fire(_rescheduleClickHandler, ReprocessButton, new WorkQueueDetailsButtonEventArg(WorkQueue));
        }

        protected void Delete_Click(object sender, EventArgs arg)
        {
            EventsHelper.Fire(_deleteClickHandler, ReprocessButton, new WorkQueueDetailsButtonEventArg(WorkQueue));
        }

        protected void Reprocess_Click(object sender, EventArgs arg)
        {
            EventsHelper.Fire(_reprocessClickHandler, ReprocessButton, new WorkQueueDetailsButtonEventArg(WorkQueue));
        }

        protected void Reset_Click(object sender, EventArgs arg)
        {
            EventsHelper.Fire(_resetClickHandler, ReprocessButton, new WorkQueueDetailsButtonEventArg(WorkQueue));
        }

        protected void RefreshTimer_Tick(object sender, EventArgs arg)
        {
            Refresh();
        }

        #endregion Protected Properties

        #region Public Methods

        public override void DataBind()
        {
            if (_detailsView == null && WorkQueue != null)
            {

                ItemStudy = WorkQueue.Study;

                if (WorkQueue.WorkQueueTypeEnum == WorkQueueTypeEnum.AutoRoute)
                {
                    _detailsView = LoadControl("AutoRouteWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    if (_detailsView != null) WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
                }
                else if (WorkQueue.WorkQueueTypeEnum == WorkQueueTypeEnum.WebMoveStudy)
                {
                    _detailsView = LoadControl("WebMoveStudyWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    if (_detailsView != null) WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
                }
                else if (WorkQueue.WorkQueueTypeEnum == WorkQueueTypeEnum.WebEditStudy)
                {
                    _detailsView = LoadControl("WebEditStudyWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    if (_detailsView != null) WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
                }
                else if (WorkQueue.WorkQueueTypeEnum == WorkQueueTypeEnum.MigrateStudy)
                {
                    _detailsView = LoadControl("TierMigrationWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    if (_detailsView != null) WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
                }
                else if (WorkQueue.WorkQueueTypeEnum == WorkQueueTypeEnum.ProcessDuplicate)
                {
                    _detailsView = LoadControl("ProcessDuplicateWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    if (_detailsView != null) WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
                }
                else if (WorkQueue.WorkQueueTypeEnum == WorkQueueTypeEnum.ExternalEdit || WorkQueue.WorkQueueTypeEnum == WorkQueueTypeEnum.WebEditStudy)
                {
                    _detailsView = LoadControl("EditWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    if (_detailsView != null) WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
                }
                else
                {
                    _detailsView = LoadControl("GeneralWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    if (_detailsView != null) WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
                }

                // If the entry isn't failed but hasn't been updated for some time, display the alert message
                WorkQueueAlertPanelRow.Visible = false;
                if (!WorkQueue.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.Failed) &&
                    !ServerPlatform.IsActiveWorkQueue(WorkQueue))
                {
                    WorkQueueAlertPanelRow.Visible = true;
                    WorkQueueAlertPanel.Text =
                        WorkQueue.LastUpdatedTime > DateTime.MinValue
                            ? String.Format(
                                  "There does not seem to be any activity for this entry since {0}. The server may not be running or there is a problem with this entry.",
                                  WorkQueue.LastUpdatedTime)
                            : "There does not seem to be any activity for this entry. The server may not be running or there is a problem with this entry.";
                }
            }
            if (_detailsView != null)
                _detailsView.WorkQueue = WorkQueue;

            base.DataBind();
        }

        /// <summary>
        /// Refreshes the Work Queue Item Details Panel UI
        /// </summary>
        public void Refresh()
        {
            UpdatePanel.Update();
        }

        #endregion Public Methods

        protected void OnAutoRefreshDisabled(object sender, TimerEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "AutoRefreshOff",
                                                "RaiseAppAlert('Auto refresh has been turned off due to inactivity.', 3000);",
                                                true);
        }
    }
}