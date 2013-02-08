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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// A dialog box that prompts users to reschedule a work queue entry and reschedule it if users confirm to do so.
    /// </summary>
    /// <remarks>
    /// To use this dialog, caller must indicate the <see cref="WorkQueue"/> entries to be rescheduled through the <see cref="WorkQueueKeys"/> property then
    /// call <see cref="Show"/> to display the dialog. Optionally, caller can register an event listener for <see cref="ResetWorkQueueDialog.WorkQueueItemResetListener"/>
    /// which is fired when users confirmed to reset the entry and it was sucessfully reset.
    /// </remarks>
    public partial class ScheduleWorkQueueDialog : UserControl
    {
        private List<Model.WorkQueue> _workQueues;

        #region Protected Properties

        protected List<Model.WorkQueue> WorkQueues
        {
            get
            {
                if (_workQueues != null)
                    return _workQueues;

                List<ServerEntityKey> keys = WorkQueueKeys;
                if (keys == null)
                    return null;

                var adaptor = new WorkQueueAdaptor();
                _workQueues = new List<Model.WorkQueue>();
                foreach (ServerEntityKey key in keys)
                {
                    Model.WorkQueue wq = adaptor.Get(key);
                    if (wq != null)
                        _workQueues.Add(wq);
                }
                return _workQueues;
            }
        }

        #endregion Protected Properties

        #region Public Properties

        public bool IsShown
        {
            get { return ViewState["IsShown"] != null && (bool) ViewState["IsShown"]; }
            set { ViewState["IsShown"] = value; }
        }

        /// <summary>
        /// Sets or gets the list of <see cref="ServerEntityKey"/> for the <see cref="Model.WorkQueue"/> to be edit
        /// </summary>
        public List<ServerEntityKey> WorkQueueKeys
        {
            get { return ViewState["_WorkQueueKeys"] as List<ServerEntityKey>; }
            set { ViewState["_WorkQueueKeys"] = value; }
        }

        public WorkQueueSettingsPanel SchedulePanel
        {
            get { return WorkQueueSettingsPanel; }
        }

        #endregion Public Properties

        #region Events

        #region Delegates

        public delegate void OnHideEventHandler();

        public delegate void OnShowEventHandler();


        /// <summary>
        /// Defines the event handler for <see cref="WorkQueueUpdated"/> event.
        /// </summary>
        /// <param name="workqueueItems"></param>
        public delegate void WorkQueueUpdatedListener(List<Model.WorkQueue> workqueueItems);

        #endregion

        /// <summary>
        /// Fires after changes to the work queue items have been committed
        /// </summary>
        public event WorkQueueUpdatedListener WorkQueueUpdated;

        public event OnShowEventHandler OnShow;

        public event OnHideEventHandler OnHide;

        #endregion Events

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PreOpenConfirmDialog.Confirmed += PreOpenConfirmDialog_Confirmed;
            PreApplyChangeConfirmDialog.Confirmed += PreApplyChangeConfirmDialog_Confirmed;

            SelectedWorkQueueItemList.WorkQueueItemGridView.SelectedIndexChanged +=
                WorkQueueListControl_SelectedIndexChanged;

            SelectedWorkQueueItemList.TheGrid = SelectedWorkQueueItemList.WorkQueueItemGridView;
        }


        internal void Reset()
        {
            SchedulePanel.ResetSelections();
        }

        protected void WorkQueueListControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WorkQueueKeys != null)
            {
                if (SelectedWorkQueueItemList.WorkQueueItems != null &&
                    SelectedWorkQueueItemList.WorkQueueItems.Count != WorkQueueKeys.Count)
                {
                    MessageDialog.Message = HttpContext.GetGlobalResourceObject("SR","WorkQueueNoLongerAvailable") as string;
                    MessageDialog.MessageType =
                        MessageBox.MessageTypeEnum.ERROR;
                    MessageDialog.Show();
                }
            }
        }

        protected void OnApplyButtonClicked(object sender, EventArgs arg)
        {
            SelectedWorkQueueItemList.Refresh();
            Hide();

            foreach (Model.WorkQueue wq in WorkQueues)
            {
                if (wq == null)
                {
                    // the workqueue no longer exist in the db
                    MessageDialog.Message = HttpContext.GetGlobalResourceObject("SR","WorkQueueRescheduleFailed_ItemNotAvailable") as string;
                    MessageDialog.MessageType =
                        MessageBox.MessageTypeEnum.ERROR;
                    MessageDialog.Show();
                    return; // don't apply the changes
                }

                if (wq.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress)
                {
                    MessageDialog.Message = HttpContext.GetGlobalResourceObject("SR","WorkQueueRescheduleConfirm_ItemBeingProcessed") as string;
                    MessageDialog.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    MessageDialog.Show();
                    return; // don't apply the changes
                }
                
                if (wq.WorkQueueStatusEnum == WorkQueueStatusEnum.Failed)
                {
                    MessageDialog.Message = HttpContext.GetGlobalResourceObject("SR","WorkQueueRescheduleFailed_ItemHasFailed") as string;
                    MessageDialog.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    MessageDialog.Show();
                    return; // don't apply the changes
                }

            }

            if (SelectedWorkQueueItemList.WorkQueueItems == null || SelectedWorkQueueItemList.WorkQueueItems.Count == 0)
            {
                MessageDialog.BackgroundCSS = string.Empty;
                MessageDialog.Message = HttpContext.GetGlobalResourceObject("SR", "SelectedWorkQueueNoLongerOnTheList") as string;
                MessageDialog.MessageStyle = "color: red; font-weight: bold;";
                MessageDialog.MessageType =
                    MessageBox.MessageTypeEnum.ERROR;
                MessageDialog.Show();

                return;
            }

            WorkQueueSettingsPanel.UpdateScheduleDateTime();
            ApplyChanges();
        }

        protected void ApplyChanges()
        {
            if (WorkQueues != null)
            {
                var toBeUpdatedList = new List<Model.WorkQueue>();
                foreach (Model.WorkQueue item in WorkQueues)
                {
                    if (item != null)
                    {
                        toBeUpdatedList.Add(item);
                    }
                }

                if (toBeUpdatedList.Count > 0)
                {
                    DateTime newScheduleTime = Platform.Time;

                    if (WorkQueueSettingsPanel.NewScheduledDateTime != null &&
                        WorkQueueSettingsPanel.ScheduleNow == false)
                    {
                        newScheduleTime = WorkQueueSettingsPanel.NewScheduledDateTime.Value;
                    }

                    if (newScheduleTime < Platform.Time && WorkQueueSettingsPanel.ScheduleNow == false)
                    {
                        MessageDialog.MessageType =
                            MessageBox.MessageTypeEnum.ERROR;
                        MessageDialog.Message = HttpContext.GetGlobalResourceObject("SR","WorkQueueRescheduleFailed_MustBeInFuture") as string;
                        MessageDialog.Show();
                        ModalDialog.Show();
                    }
                    else
                    {
                        DateTime expirationTime =
                            newScheduleTime.AddSeconds(WorkQueueSettings.Default.WorkQueueExpireDelaySeconds);

                        WorkQueuePriorityEnum priority = WorkQueueSettingsPanel.SelectedPriority;

                        try
                        {
                            var controller = new WorkQueueController();
                            bool result =
                                controller.RescheduleWorkQueueItems(toBeUpdatedList, newScheduleTime, expirationTime,
                                                                    priority);
                            if (result)
                            {
                                if (WorkQueueUpdated != null)
                                    WorkQueueUpdated(toBeUpdatedList);
                            }
                            else
                            {
                                Platform.Log(LogLevel.Error, "Unable to reschedule work queue items for user");
                                MessageDialog.MessageType =
                                    MessageBox.MessageTypeEnum.ERROR;
                                MessageDialog.Message = "Unable to reschedule this/these work queue items";
                                MessageDialog.Show();
                            }
                        }
                        catch (Exception e)
                        {
                            Platform.Log(LogLevel.Error, "Unable to reschedule work queue items for user : {0}",
                                         e.StackTrace);

                            MessageDialog.MessageType =
                                MessageBox.MessageTypeEnum.ERROR;
                            MessageDialog.Message =
                                String.Format(HttpContext.GetGlobalResourceObject("SR", "WorkQueueRescheduleFailed_Exception") as string, e.Message);
                            MessageDialog.Show();
                        }
                    }
                }
            }
        }

        protected void OnCancelButtonClicked(object sender, EventArgs arg)
        {
            Hide();
        }

        #endregion Protected Methods

        #region Private Methods

        private void PreOpenConfirmDialog_Confirmed(object data)
        {
            Display();
        }

        private void PreApplyChangeConfirmDialog_Confirmed(object data)
        {
            ApplyChanges();
        }

        private void Display()
        {
            if (WorkQueues != null && WorkQueues.Count > 0)
            {
                Model.WorkQueue workqueue = WorkQueues[0];
                WorkQueueSettingsPanel.SelectedPriority = workqueue.WorkQueuePriorityEnum;
                WorkQueueSettingsPanel.NewScheduledDateTime = workqueue.ScheduledTime;
            }

            SelectedWorkQueueItemList.Refresh();

            if (WorkQueues == null)
                return;

            if (SelectedWorkQueueItemList.WorkQueueItems.Count != WorkQueueKeys.Count)
            {
                MessageDialog.Message = HttpContext.GetGlobalResourceObject("SR", "WorkQueueNoLongerAvailable") as string;
                MessageDialog.MessageType =
                    MessageBox.MessageTypeEnum.INFORMATION;
                MessageDialog.Show();
            }

            ModalDialog.Show();
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Hides this dialog box
        /// </summary>
        public void Hide()
        {
            IsShown = false;
            if (OnHide != null) OnHide();
        }

        protected override void OnPreRender(EventArgs e)
        {
            ModalDialog.Hide();
            if (IsShown)
            {
                ModalDialog.Show();
            }
            base.OnPreRender(e);
        }

        /// <summary>
        /// Displays this dialog for rescheduling the work queue(s)
        /// </summary>
        public void Show()
        {
            IsShown = true;

            WorkQueueSettingsPanel.ScheduleNow = false;

            if (OnShow != null) OnShow();

            Display();
        }

        #endregion Public Methods

        protected void SetDataSourceFilter(WorkQueueDataSource thesource)
        {
            thesource.SearchKeys = WorkQueueKeys ?? new List<ServerEntityKey>();
        }
    }
}