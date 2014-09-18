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
using ClearCanvas.ImageServer.Common.ExternalRequest;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using AuthorityTokens=ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Configuration.ServerPartitions)]
    public partial class Default : BasePage
    {
        #region Private Members

        // used for database interaction
        private ServerPartitionConfigController _controller;

        #endregion

        #region Protected Methods

        protected void Initialize()
        {
            _controller = new ServerPartitionConfigController();

            ServerPartitionPanel.Controller = _controller;

            SetPageTitle(Titles.ServerPartitionsPageTitle);

            SetupEventHandlers();
        }

        protected void SetupEventHandlers()
        {
            AddEditPartitionDialog.OKClicked += AddEditPartitionDialog_OKClicked;
            deleteConfirmBox.Confirmed += DeleteConfirmDialog_Confirmed;
        }


        protected void UpdateUI()
        {
            ServerPartitionPanel.UpdateUI();
            UpdatePanel.Update();
        }

        protected override void OnInit(EventArgs e)
        {
            ServerPartitionPanel.EnclosingPage = this;

            base.OnInit(e);

            Initialize();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UpdateUI();
        }

        #endregion Protected Methods

        #region Private Methods
#if false
		private ImageServerDuplicateSopPolicyEnum GetDup(DuplicateSopPolicyEnum e)
		{
			if (e == DuplicateSopPolicyEnum.AcceptLatest)
				 return ImageServerDuplicateSopPolicyEnum.AcceptLatest;
			if (e == DuplicateSopPolicyEnum.CompareDuplicates)
				return ImageServerDuplicateSopPolicyEnum.CompareDuplicates;
			if (e == DuplicateSopPolicyEnum.RejectDuplicates)
				return ImageServerDuplicateSopPolicyEnum.RejectDuplicates;
			if (e == DuplicateSopPolicyEnum.SendSuccess)
				return ImageServerDuplicateSopPolicyEnum.SendSuccess;

			return ImageServerDuplicateSopPolicyEnum.SendSuccess;
		}
#endif
        private void AddEditPartitionDialog_OKClicked(ServerPartitionInfo info)
        {
            if (AddEditPartitionDialog.EditMode)
            {
                // Add partition into db and refresh the list
                if (_controller.UpdatePartition(info.Partition, info.GroupsWithDataAccess))
                {
                    UpdateUI();
                }
            }
            else
            {
#if false
	            var state = EnterpriseConfigurationBridge.AddServerPartition(new AddServerPartitionRequest
		            {
			            AcceptAnyDevice = info.Partition.AcceptAnyDevice,
			            AcceptLatestReport = info.Partition.AcceptLatestReport,
			            AeTitle = info.Partition.AeTitle,
			            AuditDeleteStudy = info.Partition.AuditDeleteStudy,
			            AutoInsertDevice = info.Partition.AutoInsertDevice,
			            DefaultRemotePort = info.Partition.DefaultRemotePort,
			            Description = info.Partition.Description,
			            DuplicateSopPolicy = GetDup(info.Partition.DuplicateSopPolicyEnum),
			            Enabled = info.Partition.Enabled,
			            GroupsWithDataAccess = info.GroupsWithDataAccess,
			            MatchAccessionNumber = info.Partition.MatchAccessionNumber,
			            Port = info.Partition.Port,

			            PartitionFolder = info.Partition.PartitionFolder,
			            MatchPatientsName = info.Partition.MatchPatientsName,
			            MatchPatientId = info.Partition.MatchPatientId,
			            MatchPatientsBirthDate = info.Partition.MatchPatientsBirthDate,
			            MatchIssuerOfPatientId = info.Partition.MatchIssuerOfPatientId,
			            MatchPatientsSex = info.Partition.MatchPatientsSex
		            });
				if (state.ExternalRequestState == ExternalRequestStateEnum.Complete )
					UpdateUI();
#else

	// Add partition into db and refresh the list
                if (_controller.AddPartition(info.Partition, info.GroupsWithDataAccess))
                {
                    UpdateUI();
                }
#endif
            }
        }

        private void DeleteConfirmDialog_Confirmed(object data)
        {
            var partition = data as ServerPartition;
            if (partition != null)
            {
                if (!_controller.Delete(partition))
                {
                    UpdateUI();

                    MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    MessageBox.Message = ErrorMessages.AdminPartition_DeletePartition_Failed;
                    MessageBox.Show();
                }
                else
                {
                    UpdateUI();
                    if (ServerPartitionPanel.Partitions != null && ServerPartitionPanel.Partitions.Count == 0)
                    {
                        MessageBox.MessageType = MessageBox.MessageTypeEnum.INFORMATION;
                        MessageBox.Message = String.Format(SR.AdminPartition_DeletePartition_Successful_AddOneNow, partition.AeTitle);
                        MessageBox.Show();
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public void AddPartition()
        {
            // display the add dialog
            AddEditPartitionDialog.Partition = null;
            AddEditPartitionDialog.EditMode = false;
            AddEditPartitionDialog.Show(true);
        }

        public void EditPartition(ServerPartition selectedPartition)
        {
            AddEditPartitionDialog.Partition = selectedPartition;
            AddEditPartitionDialog.EditMode = true;
            AddEditPartitionDialog.Show(true);
        }

        public void DeletePartition(ServerPartition selectedPartition)
        {
            deleteConfirmBox.Data = selectedPartition;
            deleteConfirmBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
            deleteConfirmBox.Message = Server.HtmlEncode(String.Format(SR.AdminPartition_DeletePartitionDialog_AreYouSure, selectedPartition.AeTitle));
            deleteConfirmBox.MessageStyle = "color: red; font-weight: bold;";
            deleteConfirmBox.Show();
        }

        #endregion
    }
}