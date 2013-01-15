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
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Configuration.PartitionArchive)]
    public partial class Default : BasePage
    {
        #region Private Members
        // used for database interaction
        private PartitionArchiveConfigController _controller = new PartitionArchiveConfigController();

        #endregion

        #region Protected Methods

        protected void SetupEventHandlers()
        {
            AddEditPartitionDialog.OKClicked += AddEditPartitionDialog_OKClicked;
            DeleteConfirmDialog.Confirmed += DeleteConfirmDialog_Confirmed;
        }


        public void UpdateUI()
        {
			SearchPanel.Refresh();
            PageContent.Update();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SetupEventHandlers();

            ServerPartitionSelector.PartitionChanged += delegate(ServerPartition partition)
            {
                SearchPanel.ServerPartition = partition;
                SearchPanel.Reset();
            };

            ServerPartitionSelector.SetUpdatePanel(PageContent);

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle(Titles.PartitionArchivesPageTitle);

            if (ServerPartitionSelector.IsEmpty)
            {
                SearchPanel.Visible = false;
            }
            else
            {
                SearchPanel.ServerPartition = ServerPartitionSelector.SelectedPartition;

                UpdateUI();
            }

            
        }

        #endregion Protected Methods

        #region Private Methods

        private void AddEditPartitionDialog_OKClicked(Model.PartitionArchive partition)
        {
            if (AddEditPartitionDialog.EditMode)
            {
                // Add partition into db and refresh the list
                if (_controller.UpdatePartition(partition))
                {
                    UpdateUI();
                }
            }
            else
            {
                // Add partition into db and refresh the list
                if (_controller.AddPartition(partition))
                {
                    UpdateUI();
                }
            }
        }

        private void DeleteConfirmDialog_Confirmed(object data)
        {
            var key = data as ServerEntityKey;

            Model.PartitionArchive pa = Model.PartitionArchive.Load(key);

            _controller.Delete(pa);

            SearchPanel.Refresh();
        }

        #endregion

        #region Public Methods

        public void AddPartition(ServerPartition partition)
        {
            // display the add dialog
            AddEditPartitionDialog.PartitionArchive = null;
            AddEditPartitionDialog.EditMode = false;
            AddEditPartitionDialog.Show(true);
			AddEditPartitionDialog.Partition = partition;
		}

        public void EditPartition(Model.PartitionArchive partitionArchive)
        {
            AddEditPartitionDialog.PartitionArchive = partitionArchive;
            AddEditPartitionDialog.EditMode = true;
            AddEditPartitionDialog.Show(true);
        	AddEditPartitionDialog.Partition = ServerPartition.Load(partitionArchive.ServerPartitionKey);
        }

        public void DeletePartition(Model.PartitionArchive partitionArchive)
        {
            DeleteConfirmDialog.Message = String.Format(SR.AdminPartitionArchive_DeleteDialog_AreYouSure, partitionArchive.Description);
            DeleteConfirmDialog.MessageType = MessageBox.MessageTypeEnum.YESNO;
            DeleteConfirmDialog.Data = partitionArchive.GetKey();
            DeleteConfirmDialog.Show();
        }

        #endregion
 
    }
}