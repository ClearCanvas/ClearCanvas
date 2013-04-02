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
using ClearCanvas.ImageServer.Core.Query;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using Resources;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly:
    WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DevicePanel.js",
        "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices
{
    /// <summary>
    /// Panel to display list of devices for a particular server partition.
    /// </summary>
    [ClientScriptResource(
        ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DevicePanel",
        ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DevicePanel.js")]    
    public partial class DevicePanel : AJAXScriptControl
    {
        #region Private members

        // the controller used for interaction with the database.
        private DeviceConfigurationController _theController;
        // the partition whose information will be displayed in this panel

        #endregion Private members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteDeviceButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string EditButtonClientID
        {
            get { return EditDeviceButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("DeviceListClientID")]
        public string DeviceListClientID
        {
            get { return DeviceGridViewControl1.TheGrid.ClientID; }
        }


        /// <summary>
        /// Sets/Gets the partition whose information is displayed in this panel.
        /// </summary>
        public ServerPartition ServerPartition { get; set; }

        public Default EnclosingPage { get; set; }

        #endregion

        #region Protected Methods

        protected void Clear()
        {
            AETitleFilter.Text = string.Empty;
            IPAddressFilter.Text = string.Empty;
            StatusFilter.SelectedIndex = 0;
            DHCPFilter.SelectedIndex = 0;
        }

        internal void Reset()
        {
            Clear();
            DeviceGridViewControl1.Reset();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _theController = new DeviceConfigurationController();

            // setup child controls
            GridPagerTop.InitializeGridPager(SR.GridPagerDeviceSingleItem, SR.GridPagerDeviceMultipleItems,
                                             DeviceGridViewControl1.TheGrid,
                                             () => DeviceGridViewControl1.Devices.Count,
                                             ImageServerConstants.GridViewPagerPosition.Top);
            DeviceGridViewControl1.Pager = GridPagerTop;
            GridPagerTop.Reset();

            StatusFilter.Items.Add(new ListItem(SR.All));
            StatusFilter.Items.Add(new ListItem(SR.Enabled));
            StatusFilter.Items.Add(new ListItem(SR.Disabled));

            DHCPFilter.Items.Add(new ListItem(SR.All));
            DHCPFilter.Items.Add(new ListItem(SR.DHCP));
            DHCPFilter.Items.Add(new ListItem(SR.NoDHCP));
        }

        /// <summary>
        /// Determines if filters are being specified.
        /// </summary>
        /// <returns></returns>
        protected bool HasFilters()
        {
            return AETitleFilter.TrimText.Length > 0 || IPAddressFilter.TrimText.Length > 0 || StatusFilter.SelectedIndex > 0 ||
                   DHCPFilter.SelectedIndex > 0;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Refresh();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ServerPartition != null)
            {

                // This make sure we have the list to work with. 
                // the list may be out-dated if the add/update event is fired later
                // In those cases, the list must be refreshed again.
                LoadDevices();

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
                    var typeItems = new ListItem[DeviceTypeFilter.Items.Count];
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
        }

        #endregion Protected methods

        /// <summary>
        /// Load the devices for the partition based on the filters specified in the filter panel.
        /// </summary>
        /// <remarks>
        /// This method only reloads and binds the list bind to the internal grid. <seealso cref="Refresh"/> should be called
        /// to explicit update the list in the grid. 
        /// <para>
        /// This is intentionally so that the list can be reloaded so that it is available to other controls during postback.  In
        /// some cases we may not want to refresh the list if there's no change. Calling <seealso cref="Refresh"/> will
        /// give performance hit as the data will be transfered back to the browser.
        ///  
        /// </para>
        /// </remarks>
        public void LoadDevices()
        {
            if (ServerPartition == null)
                return;

            var criteria = new DeviceSelectCriteria();

            // only query for device in this partition
            criteria.ServerPartitionKey.EqualTo(ServerPartition.GetKey());

            if (!String.IsNullOrEmpty(AETitleFilter.TrimText))
            {
                QueryHelper.SetGuiStringCondition(criteria.AeTitle,
                                                  SearchHelper.LeadingAndTrailingWildCard(AETitleFilter.TrimText));
            }

            if (!String.IsNullOrEmpty(DescriptionFilter.TrimText))
            {
                QueryHelper.SetGuiStringCondition(criteria.Description,
                                                  SearchHelper.LeadingAndTrailingWildCard(DescriptionFilter.TrimText));
            }

            if (!String.IsNullOrEmpty(IPAddressFilter.TrimText))
            {
                QueryHelper.SetGuiStringCondition(criteria.IpAddress,
                                                  SearchHelper.TrailingWildCard(IPAddressFilter.TrimText));
            }

            if (StatusFilter.SelectedIndex != 0)
            {
                criteria.Enabled.EqualTo(StatusFilter.SelectedIndex == 1);
            }

            if (DHCPFilter.SelectedIndex != 0)
            {
                criteria.Dhcp.EqualTo(DHCPFilter.SelectedIndex == 1);
            }

            if (DeviceTypeFilter.SelectedIndex > -1)
            {
                var types = new List<DeviceTypeEnum>();
                foreach (ListItem item in DeviceTypeFilter.Items)
                {
                    if (item.Selected)
                    {
                        types.Add(DeviceTypeEnum.GetEnum(item.Value));
                    }
                }
                criteria.DeviceTypeEnum.In(types);
            }

            DeviceGridViewControl1.Devices = _theController.GetDevices(criteria);
            DeviceGridViewControl1.RefreshCurrentPage();
        }

        /// <summary>
        /// Updates the device list window in the panel.
        /// </summary>
        /// <remarks>
        /// This method should only be called when necessary as the information in the list window needs to be transmitted back to the client.
        /// If the list is not changed, call <seealso cref="LoadDevices()"/> instead.
        /// </remarks>
        public void Refresh()
        {
            if (ServerPartition != null)
            {

                LoadDevices();
                Device dev = DeviceGridViewControl1.SelectedDevice;

                if (dev == null)
                {
                    // no device being selected
                    EditDeviceButton.Enabled = false;
                    DeleteDeviceButton.Enabled = false;
                }
                else
                {
                    EditDeviceButton.Enabled = true;
                    DeleteDeviceButton.Enabled = true;
                }

                SearchUpdatePanel.Update();
            }
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            LoadDevices();
        }

        protected void AddDeviceButton_Click(object sender, ImageClickEventArgs e)
        {
            EnclosingPage.OnAddDevice(_theController, ServerPartition);
        }

        protected void EditDeviceButton_Click(object sender, ImageClickEventArgs e)
        {
            Device dev = DeviceGridViewControl1.SelectedDevice;
            if (dev != null)
            {
                EnclosingPage.OnEditDevice(_theController, ServerPartition, dev);
            }
        }

        protected void DeleteDeviceButton_Click(object sender, ImageClickEventArgs e)
        {
            Device dev = DeviceGridViewControl1.SelectedDevice;
            if (dev != null)
            {
                EnclosingPage.OnDeleteDevice(_theController, ServerPartition, dev);
            }
        }
    }
}