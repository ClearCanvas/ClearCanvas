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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using Resources;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move
{
    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel", ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel.js")]
    public partial class MovePanel : AJAXScriptControl
    {
        [Serializable]
        private class MoveRequest
        {
            private IList<Study> _studies;
            private IList<Device> _devices;

            public IList<Study> Studies
            {
                get { return _studies; }
                set { _studies = value; }
            }

            public IList<Device> DestinationDevices
            {
                get { return _devices; }
                set { _devices = value; }
            }
        }
        private ServerPartition _partition;
        private DeviceConfigurationController _theController;
        private IList<Study> _studies = new List<Study>();

        [ExtenderControlProperty]
        [ClientPropertyName("MoveButtonClientID")]
        public string MoveButtonClientID
        {
            get { return MoveButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("DeviceListClientID")]
        public string DeviceListClientID
        {
            get { return DeviceGridPanel.TheGrid.ClientID; }
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        public StudyGridView StudyGridView
        {
            get { return this.StudyGridPanel; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            UpdateUI();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _theController = new DeviceConfigurationController();

            // setup child controls
            GridPagerTop.InitializeGridPager(SR.GridPagerDeviceSingleItem, SR.GridPagerDeviceMultipleItems, DeviceGridPanel.TheGrid, delegate { return DeviceGridPanel.Devices.Count; }, ImageServerConstants.GridViewPagerPosition.Top);
            DeviceGridPanel.Pager = GridPagerTop;

            MoveConfirmation.Confirmed += delegate(object data)
                                              {

                    IList<Device> devices = data as IList<Device>;

                    StudyController studyController = new StudyController();
                    foreach (Study study in StudyGridView.StudyList)
                    {
                        foreach (Device device in devices)
                            studyController.MoveStudy(study, device);
                    }


                                              };

        }
        /// <summary>
        /// Updates the device list window in the panel.
        /// </summary>
        /// <remarks>
        /// This method should only be called when necessary as the information in the list window needs to be transmitted back to the client.
        /// If the list is not changed, call <seealso cref="LoadDevices()"/> instead.
        /// </remarks>
        public void UpdateUI()
        {
            LoadDevices();
            Device dev = this.DeviceGridPanel.SelectedDevice;

            if (dev == null)
            {
                // no device being selected
                MoveButton.Enabled = false;
            }
            else
            {
                MoveButton.Enabled = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ServerPartitionDataAdapter adaptor = new ServerPartitionDataAdapter();
            ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
            criteria.AeTitle.EqualTo(Request.QueryString[ImageServerConstants.QueryStrings.ServerAE]);
            _partition = adaptor.GetFirst(criteria);

            for(int i=1;;i++)
            {
                string qs = string.Format(ImageServerConstants.QueryStrings.StudyUID + "{0}", i);
                string studyuid = Request.QueryString[qs];

                if(!string.IsNullOrEmpty(studyuid))
                {
                    _studies.Add(LoadStudy(studyuid));
                }
                else
                {
                    break;
                }
            }

            StudyGridPanel.StudyList = _studies;

            LoadDevices();

            if (DHCPFilter.Items.Count == 0)
            {
                DHCPFilter.Items.Add(new ListItem(SR.All));
                DHCPFilter.Items.Add(new ListItem(SR.DHCP));
                DHCPFilter.Items.Add(new ListItem(SR.NoDHCP));
            }

            IList<DeviceTypeEnum> deviceTypes = DeviceTypeEnum.GetAll();

            if (DeviceTypeFilter.Items.Count == 0)
            {
                foreach (DeviceTypeEnum t in deviceTypes)
                {
                    DeviceTypeFilter.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(t), t.Lookup));
                }
            }
            else
            {
                ListItem[] typeItems = new ListItem[DeviceTypeFilter.Items.Count];
                DeviceTypeFilter.Items.CopyTo(typeItems, 0);
                DeviceTypeFilter.Items.Clear();
                int count = 0;
                foreach (DeviceTypeEnum t in deviceTypes)
                {
                    DeviceTypeFilter.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(t), t.Lookup));
                    DeviceTypeFilter.Items[count].Selected = typeItems[count].Selected;
                    count++;
                }
            }
        }

        protected Study LoadStudy(string studyInstanceUID)
        {
            if (String.IsNullOrEmpty(studyInstanceUID))
                return null;

            if (_partition == null)
                return null;

            StudyAdaptor studyAdaptor = new StudyAdaptor();
            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.StudyInstanceUid.EqualTo(studyInstanceUID);
            criteria.ServerPartitionKey.EqualTo(Partition.Key);
            return studyAdaptor.GetFirst(criteria);
        }

        /// <summary>
        /// Load the devices for the partition based on the filters specified in the filter panel.
        /// </summary>
        /// <remarks>
        /// This method only reloads and binds the list bind to the internal grid. <seealso cref="UpdateUI()"/> should be called
        /// to explicit update the list in the grid. 
        /// <para>
        /// This is intentionally so that the list can be reloaded so that it is available to other controls during postback.  In
        /// some cases we may not want to refresh the list if there's no change. Calling <seealso cref="UpdateUI()"/> will
        /// give performance hit as the data will be transfered back to the browser.
        ///  
        /// </para>
        /// </remarks>
        public void LoadDevices()
        {
            DeviceSelectCriteria criteria = new DeviceSelectCriteria();

            // only query for device in this partition
            criteria.ServerPartitionKey.EqualTo(Partition.GetKey());

            if (!String.IsNullOrEmpty(AETitleFilter.TrimText))
            {
                string key = AETitleFilter.TrimText + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.AeTitle.Like(key);
            }

            if (!String.IsNullOrEmpty(DescriptionFilter.TrimText))
            {
                string key = DescriptionFilter.TrimText + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.Description.Like(key);
            }

            if (!String.IsNullOrEmpty(IPAddressFilter.TrimText))
            {
                string key = IPAddressFilter.TrimText + "%";
                key = key.Replace("*", "%");
                key = key.Replace("?", "_");
                criteria.IpAddress.Like(key);
            }

            if (DHCPFilter.SelectedIndex != 0)
            {
                if (DHCPFilter.SelectedIndex == 1)
                    criteria.Dhcp.EqualTo(true);
                else
                    criteria.Dhcp.EqualTo(false);
            }

            if (DeviceTypeFilter.SelectedIndex > -1)
            {
                List<DeviceTypeEnum> types = new List<DeviceTypeEnum>();
                foreach (ListItem item in DeviceTypeFilter.Items)
                {
                    if (item.Selected)
                    {
                        types.Add(DeviceTypeEnum.GetEnum(item.Value));
                    }
                }
                criteria.DeviceTypeEnum.In(types);
            }

            // only enabled devices and devices that allow retrieves.
            criteria.Enabled.EqualTo(true);
            criteria.AllowRetrieve.EqualTo(true);

            DeviceGridPanel.Devices = _theController.GetDevices(criteria);
            DeviceGridPanel.RefreshCurrentPage();
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            LoadDevices();
        }

        protected void MoveButton_Click(object sender, EventArgs e)
        {
            if (DeviceGridPanel.SelectedDevice != null)
            {
                string destinationDevices = string.Empty;
                string separator = string.Empty;
                int i = 0;
                IList<Device> devices = DeviceGridPanel.SelectedDevices;
                foreach(Device device in devices)
                {
                    if (i > 0) separator = ", ";
                    destinationDevices = device.AeTitle + separator + destinationDevices;
                    i++;
                }
                                       
                MoveConfirmation.Message = DialogHelper.createConfirmationMessage(string.Format(SR.MoveStudyMessage, destinationDevices));
                
                MoveConfirmation.Message += DialogHelper.createStudyTable(StudyGridView.StudyList);
                    
                MoveConfirmation.MessageType = MessageBox.MessageTypeEnum.OKCANCEL;

                MoveConfirmation.Data = DeviceGridPanel.SelectedDevices;
                MoveConfirmation.Show();
            }
        }
    }
}