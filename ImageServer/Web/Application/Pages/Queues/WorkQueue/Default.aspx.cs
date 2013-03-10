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
using System.Globalization;
using System.Security.Permissions;
using System.Web.UI;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.WorkQueue.Search)]
    public partial class Default : BasePage
    {
        /// <summary>
        /// Sets/Gets a value which indicates whether auto refresh is on
        /// </summary>
        public bool AutoRefresh
        {
            get
            {
                if (ViewState["AutoRefresh"] == null)
                    return false;
                return (bool)ViewState["AutoRefresh"];
            }
            set
            {
                ViewState["AutoRefresh"] = value;
                RefreshTimer.Reset(value);
            }
        }

        public int RefreshRate
        {
            get
            {
                if (ViewState["RefreshRate"] == null)
                    return WorkQueueSettings.Default.NormalRefreshIntervalSeconds;
                return (int)ViewState["RefreshRate"];
            }
            set { ViewState["RefreshRate"] = value; }
        }

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ServerPartitionSelector.IsEmpty)
            {
                SearchPanel.Visible = false;
            }
            else
            {
                SearchPanel.ServerPartition = ServerPartitionSelector.SelectedPartition;

                string controlName = Request.Params.Get("__EVENTTARGET");
                if (controlName != null && controlName.Equals(RefreshRateTextBox.ClientID))
                {
                    RefreshRate = Int32.Parse(RefreshRateTextBox.Text);
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SearchPanel.EnclosingPage = this;

            RefreshTimer.AutoDisabled += RefreshTimer_AutoDisabled;
            ConfirmRescheduleDialog.Confirmed += ConfirmationContinueDialog_Confirmed;
            ScheduleWorkQueueDialog.WorkQueueUpdated += ScheduleWorkQueueDialog_OnWorkQueueUpdated;
            ScheduleWorkQueueDialog.OnShow += DisableRefresh;
            ScheduleWorkQueueDialog.OnHide += () => RefreshTimer.Reset(AutoRefresh);

            ResetWorkQueueDialog.WorkQueueItemReseted += ResetWorkQueueDialog_WorkQueueItemReseted;
            ResetWorkQueueDialog.OnShow += DisableRefresh;
            ResetWorkQueueDialog.OnHide += () => RefreshTimer.Reset(AutoRefresh);
            DeleteWorkQueueDialog.WorkQueueItemDeleted += DeleteWorkQueueDialog_WorkQueueItemDeleted;
            DeleteWorkQueueDialog.OnShow += DisableRefresh;
            DeleteWorkQueueDialog.OnHide += () => RefreshTimer.Reset(AutoRefresh);
            InformationDialog.OnShow += DisableRefresh;
            InformationDialog.OnHide += () => RefreshTimer.Reset(AutoRefresh);

            if (!Page.IsPostBack)
            {
                AutoRefresh = false;
                RefreshTimer.Enabled = false;
                RefreshTimer.Interval = (int)TimeSpan.FromSeconds(Math.Max(WorkQueueSettings.Default.NormalRefreshIntervalSeconds, 5)).TotalMilliseconds;// min refresh rate: every 5 sec 
                RefreshRateTextBox.Text = TimeSpan.FromMilliseconds(RefreshTimer.Interval).TotalSeconds.ToString(CultureInfo.InvariantCulture);
            }

            ServerPartitionSelector.PartitionChanged += delegate(ServerPartition partition)
            {
                SearchPanel.ServerPartition = partition;
                SearchPanel.Reset();
            };

            ServerPartitionSelector.SetUpdatePanel(PageContent);

            SetPageTitle(Titles.WorkQueuePageTitle);
        }

        void RefreshTimer_AutoDisabled(object sender, TimerEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "AutoRefreshOff",
                     "RaiseAppAlert('Auto refresh has been turned off due to inactivity.', 3000);",
                     true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            RefreshTimer.Enabled &= AutoRefresh && !DeleteWorkQueueDialog.IsShown && !ScheduleWorkQueueDialog.IsShown;
            RefreshRateEnabled.SelectedValue = AutoRefresh && RefreshTimer.Enabled ? "Y" : "N";
            RefreshTimer.Interval = (int)TimeSpan.FromSeconds(Int32.Parse(RefreshRateTextBox.Text)).TotalMilliseconds;// min refresh rate: every 5 sec 
            RefreshIntervalPanel.Visible = RefreshRateEnabled.SelectedValue == "Y";

            // If the request is casued by the refresh timer consecutively N times, 
            // it is an indication that users are not using the screen, we should turn off the auto-refresh

            base.OnPreRender(e);
        }

        #endregion Protected Methods


        #region Public Methods


        /// <summary>
        /// Popup a dialog to reschedule the work queue item.
        /// </summary>
        /// <param name="itemKey"></param>
        public void RescheduleWorkQueueItem(ServerEntityKey itemKey)
        {
            if (itemKey == null)
                return;

            var adaptor = new WorkQueueAdaptor();

            Model.WorkQueue item = adaptor.Get(itemKey);

            if (item==null)
            {
                InformationDialog.Message = SR.WorkQueueNotAvailable;
                InformationDialog.Show();

            }
            else
            {
                if (item.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress)
                {
                    // prompt the user first
                    InformationDialog.Message = SR.WorkQueueBeingProcessed_CannotReschedule;
                    InformationDialog.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    InformationDialog.Show();
                    return;
                }

                if (item.WorkQueueStatusEnum == WorkQueueStatusEnum.Failed)
                {
                    InformationDialog.Message = SR.WorkQueueFailed_CannotReschedule;
                    InformationDialog.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    InformationDialog.Show();
                    return;
                }

                ScheduleWorkQueueDialog.WorkQueueKeys = new List<ServerEntityKey> {itemKey};
                ScheduleWorkQueueDialog.Reset();
                ScheduleWorkQueueDialog.Show();
            }
        }

        public void ResetWorkQueueItem(ServerEntityKey itemKey)
        {
            if (itemKey != null)
            {
                ResetWorkQueueDialog.WorkQueueItemKey = itemKey;
                ResetWorkQueueDialog.Show();
            }
        }

        public void DeleteWorkQueueItem(ServerEntityKey itemKey)
        {
            if (itemKey != null)
            {
                DeleteWorkQueueDialog.WorkQueueItemKey = itemKey;
                DeleteWorkQueueDialog.Show();
            }
        }

        public void ReprocessWorkQueueItem(ServerEntityKey itemKey)
        {
            if (itemKey != null)
            {
                Model.WorkQueue item = Model.WorkQueue.Load(itemKey);
                var controller = new WorkQueueController();
                try
                {
                    if (controller.ReprocessWorkQueueItem(item))
                    {
                        InformationDialog.Message = SR.ReprocessOK;
                        InformationDialog.MessageType = MessageBox.MessageTypeEnum.INFORMATION;
                        InformationDialog.Show();
                    }
                    else
                    {
                        InformationDialog.Message = SR.ReprocessFailed;
                        InformationDialog.MessageType = MessageBox.MessageTypeEnum.ERROR;
                        InformationDialog.Show();
                    }
                }
                catch(InvalidStudyStateOperationException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
        
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Message = message;
            MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
            MessageBox.Show();
        }

        public void HideRescheduleDialog()
        {
            ScheduleWorkQueueDialog.Hide();
        }

        #endregion Public Methods

        #region Private Methods

        private void ScheduleWorkQueueDialog_OnWorkQueueUpdated(List<Model.WorkQueue> workqueueItems)
        {
            SearchPanel.Refresh();
        }

        void ResetWorkQueueDialog_WorkQueueItemReseted(Model.WorkQueue item)
        {
            SearchPanel.Refresh();
        }

        void DeleteWorkQueueDialog_WorkQueueItemDeleted(Model.WorkQueue item)
        {
            SearchPanel.Refresh();
        }

        void ConfirmationContinueDialog_Confirmed(object data)
        {
            DataBind();
            ScheduleWorkQueueDialog.Show();
        }

        private void DisableRefresh()
        {
            RefreshTimer.Reset(false);
        }

        #endregion Private Methods

        protected void RefreshRate_IndexChanged(Object sender, EventArgs arg)
        {
            if (RefreshRateEnabled.SelectedItem.Value.Equals("Y"))
            {
                AutoRefresh = true;
                RefreshRate = Int32.Parse(RefreshRateTextBox.Text);
                RefreshRateTextBox.Enabled = true;
            }
            else
            {
                AutoRefresh = false;
                RefreshRateTextBox.Enabled = false;
            }
        }

        protected void RefreshTimer_Tick(object sender, EventArgs e)
        {
            SearchPanel.Refresh();
        }

        protected void OnResetWorkQueueError(object sender, WorkQueueItemResetErrorEventArgs e)
        {
            MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
            MessageBox.Message = e.ErrorMessage;
            MessageBox.Show();
            PageContent.Update();
        }
    }
}
