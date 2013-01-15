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
using SR=Resources.SR;
using ClearCanvas.ImageServer.Core.ModelExtensions;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// A dialog box that prompts users for confirmation to delete a work queue entry and carries out the deletion if users do so.
    /// </summary>
    /// <remarks>
    /// To use this dialog, caller must indicate the <see cref="WorkQueue"/> entry through the <see cref="WorkQueueItemKey"/> property then
    /// call <see cref="Show"/> to display the dialog. Optionally, caller can register an event listener for <see cref="WorkQueueItemDeleted"/>
    /// which is fired when users confirmed to delete the entry and it was sucessfully deleted.
    /// </remarks>
    public partial class DeleteWorkQueueDialog : UserControl
    {
        private Model.WorkQueue _workQueue;

        #region Events

        #region Delegates

        public delegate void OnHideEventHandler();

        public delegate void OnShowEventHandler();

        /// <summary>
        /// Defines handler for <see cref="WorkQueueItemDeleted"/> event.
        /// </summary>
        /// <param name="item"></param>
        public delegate void WorkQueueItemDeletedListener(Model.WorkQueue item);

        #endregion

        /// <summary>
        /// Fired when the <see cref="WorkQueue"/> object associated with this dialog box is deleted.
        /// </summary>
        public event WorkQueueItemDeletedListener WorkQueueItemDeleted;

        public event OnShowEventHandler OnShow;

        public event OnHideEventHandler OnHide;

        #endregion Events

        #region Public Properties

        public bool IsShown
        {
            get { return ViewState["IsShown"] != null && (bool) ViewState["IsShown"]; }
            set { ViewState["IsShown"] = value; }
        }

        /// <summary>
        /// Sets / Gets the <see cref="ServerEntityKey"/> of the <see cref="WorkQueue"/> item associated with this dialog
        /// </summary>
        public ServerEntityKey WorkQueueItemKey
        {
            get
            {
                if (ViewState["WorkQueueItemKey"] == null) return null;
                else return (ServerEntityKey) ViewState["WorkQueueItemKey"];
            }
            set { ViewState["WorkQueueItemKey"] = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PreDeleteConfirmDialog.Confirmed += PreDeleteConfirmDialog_Confirmed;
            PreDeleteConfirmDialog.Cancel += Hide;
            MessageBox.Cancel += Hide;
            MessageBox.Confirmed += delegate { Hide(); };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack && IsShown)
            {
                DataBind();
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void PreDeleteConfirmDialog_Confirmed(object data)
        {
            Hide();

            var key = data as ServerEntityKey;
            if (key != null)
            {
                var adaptor = new WorkQueueAdaptor();
                Model.WorkQueue item = adaptor.Get(key);
                if (item == null)
                {
                    MessageBox.Message = SR.WorkQueueNotAvailable;
                    MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    MessageBox.Show();
                }
                else
                {
                    if (item.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress)
                    {
                        MessageBox.Message = SR.WorkQueueBeingProcessed_CannotDelete;
                        MessageBox.MessageType =
                            MessageBox.MessageTypeEnum.ERROR;
                        MessageBox.Show();
                        return;
                    }

                    try
                    {
                        bool successful;
                        var controller = new WorkQueueController();
                        var items = new List<Model.WorkQueue>();
                        items.Add(item);

                        successful = controller.DeleteWorkQueueItems(items);
                        if (successful)
                        {
                            Platform.Log(LogLevel.Info, "Work Queue item deleted by user : Item Key={0}",
                                         item.GetKey().Key);

                            if (WorkQueueItemDeleted != null)
                                WorkQueueItemDeleted(item);

                            if (OnHide != null) OnHide();
                        }
                        else
                        {
                            Platform.Log(LogLevel.Error,
                                         "PreResetConfirmDialog_Confirmed: Unable to delete work queue item. GUID={0}",
                                         item.GetKey().Key);

                            MessageBox.Message = SR.WorkQueueDeleteFailed;
                            MessageBox.MessageType =
                                MessageBox.MessageTypeEnum.ERROR;
                            MessageBox.Show();
                        }
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error,
                                     "PreResetConfirmDialog_Confirmed: Unable to delete work queue item. GUID={0} : {1}",
                                     item.GetKey().Key, e.StackTrace);

                        MessageBox.Message = String.Format(SR.WorkQueueDeleteFailed_WithException, e.Message);
                        MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
                        MessageBox.Show();
                    }
                }
            }
        }

        #endregion Private Methods

        private Model.WorkQueue WorkQueue
        {
            get
            {
                if (_workQueue == null)
                {
                    if (WorkQueueItemKey != null)
                    {
                        var adaptor = new WorkQueueAdaptor();
                        _workQueue = adaptor.Get(WorkQueueItemKey);
                    }
                }

                return _workQueue;
            }
        }

        public ServerEntityKey ServerPartitionKey
        {
            get { return WorkQueue.ServerPartitionKey; }
        }

        #region Public Methods



        public override void DataBind()
        {
            if (WorkQueue != null)
            {
                PreDeleteConfirmDialog.Data = WorkQueueItemKey;
                PreDeleteConfirmDialog.MessageType = MessageBox.MessageTypeEnum.YESNO;
                PreDeleteConfirmDialog.Message = WorkQueue.WorkQueueTypeEnum.Equals(WorkQueueTypeEnum.ExternalEdit) 
                    ? SR.WorkQueueDeleteConfirmExternalEdit 
                    : SR.WorkQueueDeleteConfirm;

                if (WorkQueue.WillResultInDataChanged())
                {
                    PreDeleteConfirmDialog.WarningMessage = SR.WorkQueueDeleteWarning;
                }
            }
            else
            {
                MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
                MessageBox.Message = SR.WorkQueueNotAvailable;
            }
            base.DataBind();
        }

        /// <summary>
        /// Displays the dialog box for deleting <see cref="WorkQueue"/> entry.
        /// </summary>
        /// <remarks>
        /// The <see cref="WorkQueueItemKey"/> to be deleted must be set prior to calling <see cref="Show"/>.
        /// </remarks>
        public void Show()
        {
            IsShown = true;
            DataBind();
            if (OnShow != null) OnShow();
        }

        protected override void OnPreRender(EventArgs e)
        {
            MessageBox.Close();
            PreDeleteConfirmDialog.Close();
            if (IsShown)
            {
                if (WorkQueue != null)
                {
                    PreDeleteConfirmDialog.Show();
                }
                else
                    MessageBox.Show();
            }


            base.OnPreRender(e);
        }

        /// <summary>
        /// Closes the dialog box
        /// </summary>
        public void Hide()
        {
            IsShown = false;

            if (OnHide != null) OnHide();
        }

        #endregion Public Methods
    }
}