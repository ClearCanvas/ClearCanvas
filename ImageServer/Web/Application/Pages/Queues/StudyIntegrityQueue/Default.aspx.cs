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
using System.Security.Permissions;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Model;
using Resources;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.StudyIntegrityQueue.Search)]
    public partial class Default : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ServerPartition activePartition = null;

            if (!IsPostBack && !Page.IsAsync)
            {
                string patientID = Request["PatientID"];
                string patientName = Request["PatientName"];
                string partitionKey = Request["PartitionKey"];
                string reason = Request["Reason"];

                if (!string.IsNullOrEmpty(patientID) && !string.IsNullOrEmpty(patientName) &&
                    !string.IsNullOrEmpty(partitionKey))
                {
                    if (!string.IsNullOrEmpty(partitionKey))
                    {
                        var controller = new ServerPartitionConfigController();
                        activePartition = controller.GetPartition(new ServerEntityKey("ServerPartition", partitionKey));
                    }
                }
                if (string.IsNullOrEmpty(reason))
                {
                    if (!string.IsNullOrEmpty(partitionKey))
                    {
                        var controller = new ServerPartitionConfigController();
                        activePartition = controller.GetPartition(new ServerEntityKey("ServerPartition", partitionKey));
                    }
                }
            }

            ServerPartitionSelector.PartitionChanged += delegate(ServerPartition partition)
            {
                SearchPanel.ServerPartition = partition;
                SearchPanel.Reset();
            };

            ServerPartitionSelector.SetUpdatePanel(PageContent);

            if (activePartition != null)
            {
                ServerPartitionSelector.SelectedPartition = activePartition;
            }

            SetPageTitle(Titles.StudyIntegrityQueuePageTitle);
        }

        public void OnReconcileItem(ReconcileDetails details)
        {
            if (details.StudyIntegrityQueueItem.StudyIntegrityReasonEnum == StudyIntegrityReasonEnum.Duplicate)
            {
                DuplicateSopReconcileDialog.StudyIntegrityQueueItem = details.StudyIntegrityQueueItem;
                DuplicateSopReconcileDialog.DuplicateEntryDetails = details as DuplicateEntryDetails;

                DuplicateSopReconcileDialog.DataBind();
                DuplicateSopReconcileDialog.Show();
            }
            else
            {
                ReconcileDialog.ReconcileDetails = details;
                ReconcileDialog.StudyIntegrityQueueItem = details.StudyIntegrityQueueItem;
                ReconcileDialog.Show();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ServerPartitionSelector.IsEmpty)
            {
                SearchPanel.Visible = false;
            }
            else
            {
                SearchPanel.ServerPartition = ServerPartitionSelector.SelectedPartition;
            }
        }
    }
}