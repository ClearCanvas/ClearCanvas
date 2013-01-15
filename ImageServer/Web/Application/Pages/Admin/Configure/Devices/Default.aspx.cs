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
using System.Security.Permissions;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Model;
using Resources;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Configuration.Devices)]
    public partial class Default : BasePage
    {
        #region Private members

        // the controller used for database interaction
        private readonly DeviceConfigurationController _controller = new DeviceConfigurationController();

        #endregion Private members

        public ServerEntityKey CurrentPartition
        {
            get
            {
                return ServerPartitionSelector.SelectedPartition.Key;
            }
        }


        #region Protected methods

        /// <summary>
        /// Set up the event handlers for child controls.
        /// </summary>
        protected void SetupEventHandlers()
        {
            AddEditDeviceControl1.OKClicked += delegate(Device dev)
                                                   {
                                                       if (AddEditDeviceControl1.EditMode)
                                                       {
                                                           // Commit the change into database
                                                           if (_controller.UpdateDevice(dev))
                                                           {
                                                               SearchPanel.Refresh();
                                                           }
                                                       }
                                                       else
                                                       {
                                                           // Create new device in the database
                                                           Device newDev = _controller.AddDevice(dev);
                                                           if (newDev != null)
                                                           {
                                                               SearchPanel.Refresh();
                                                           }
                                                       }
													   UpdateUI();
                                                   };


            DeleteConfirmation.Confirmed += delegate(object data)
                                                {
                                                    // delete the device and reload the affected partition.
                                                    if (DeleteConfirmation.MessageType == MessageBox.MessageTypeEnum.YESNO)
                                                    {                                            
                                                        var dev = data as Device;
                                                        if (dev != null)
                                                        {
                                                            if (_controller.GetRelatedWorkQueueCount(dev) == 0)
                                                            {
                                                                _controller.DeleteDevice(dev);
                                                            }

                                                            SearchPanel.Refresh();
                                                        }
                                                        UpdateUI();
                                                    }
                                                };
            SetPageTitle(Titles.DevicesPageTitle);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddEditDeviceControl1.DevicePage = this;
            SearchPanel.EnclosingPage = this;

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

		protected void UpdateUI()
		{
			AddEditDeviceControl1.UpdateLabels();
			DialogUpdatePanel.Update();
		}

        #endregion  Protected methods

        #region Public Methods

        public void OnAddDevice(DeviceConfigurationController controller, ServerPartition serverPartition)
        {
            // Populate the add device dialog and display it
            AddEditDeviceControl1.EditMode = false;
            AddEditDeviceControl1.Device = null;
            IList<ServerPartition> list = new List<ServerPartition>();
            list.Add(serverPartition);
            AddEditDeviceControl1.Partitions = list;
            AddEditDeviceControl1.Show(true);
        }

        public void OnEditDevice(DeviceConfigurationController controller, ServerPartition serverPartition, Device dev)
        {
            // Populate the edit device dialog and display it
            AddEditDeviceControl1.EditMode = true;
            AddEditDeviceControl1.Device = dev;
            AddEditDeviceControl1.Partitions = controller.GetServerPartitions();
            AddEditDeviceControl1.Show(true);
        }

        public void OnDeleteDevice(DeviceConfigurationController controller, ServerPartition serverPartition, Device dev)
        {
            if (controller.GetRelatedWorkQueueCount(dev) > 0)
            {
                DeleteConfirmation.Message = string.Format(ErrorMessages.AdminDevices_DeleteDevice_PendingWorkQueue,dev.AeTitle);
                DeleteConfirmation.MessageType = MessageBox.MessageTypeEnum.ERROR;             
            }
            else
            {
                DeleteConfirmation.Message = string.Format(SR.AdminDevices_DeleteDevice_AreYouSure, dev.AeTitle, serverPartition.AeTitle);
                DeleteConfirmation.MessageType = MessageBox.MessageTypeEnum.YESNO;
            }

            DeleteConfirmation.Data = dev;
            DeleteConfirmation.Show();
        }

        
        #endregion
    }
}