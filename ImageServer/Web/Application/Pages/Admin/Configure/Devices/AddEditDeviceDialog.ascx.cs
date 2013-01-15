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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using Resources;
using ClearCanvas.ImageServer.Web.Common;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices
{
    //
    // Dialog for adding a new device or editting an existing device.
    //
    public partial class AddEditDeviceDialog : UserControl
    {
        #region private variables

        // The server partitions that the new device can be associated with
        // This list will be determined by the user level permission.
        // device being editted/added
        private Device _device;
        private bool _editMode;
        private IList<ServerPartition> _partitions = new List<ServerPartition>();

        #endregion

        #region public members

        /// <summary>
        /// Sets the list of partitions users allowed to pick.
        /// </summary>
        public IList<ServerPartition> Partitions
        {
            set { _partitions = value; }

            get { return _partitions; }
        }

        /// <summary>
        /// Sets or gets the value which indicates whether the dialog is in edit mode.
        /// </summary>
        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;
                ViewState["EditMode"] = value;
            }
        }

        /// <summary>
        /// Sets/Gets the current editing device.
        /// </summary>
        public Device Device
        {
            set
            {
                _device = value;
                // put into viewstate to retrieve later
                ViewState["EditedDevice"] = _device;
            }
            get { return _device; }
        }

        public Default DevicePage
        {
            get; set;
        }

        #endregion // public members

        #region Events

        #region Delegates

        /// <summary>
        /// Defines the event handler for <seealso cref="OKClicked"/>.
        /// </summary>
        /// <param name="device">The device being added.</param>
        public delegate void OnOKClickedEventHandler(Device device);

        #endregion

        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OnOKClickedEventHandler OKClicked;

        #endregion Events

        #region Public delegates

        #endregion // public delegates

        #region Protected methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TabContainer1.ActiveTabIndex = 0;

            DHCPCheckBox.InputAttributes.Add("onClick", "EnableDisableIp();");

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID,
                                                        @"<script language='javascript'>
                            function EnableDisableIp()
                            {
                                var checkBox = document.getElementById('" +
                                                        DHCPCheckBox.ClientID +
                                                        @"');
                                var ipBox = document.getElementById('" +
                                                        IPAddressTextBox.ClientID +
                                                        @"');
                                ipBox.disabled=checkBox.checked;         
                            }
                        </script>");

            EditDeviceValidationSummary.HeaderText = ErrorMessages.EditStudyValidationError;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
            }
            else
            {
                if (ViewState["EditMode"] != null)
                    _editMode = (bool) ViewState["EditMode"];

				if (ViewState["EditedDevice"] != null)
				{
					_device = ViewState["EditedDevice"] as Device;
				    DeviceValidator.OriginalAeTitle = _device.AeTitle;
				}

                DeviceValidator.Partition = DevicePage.CurrentPartition;
            }

            AllowStorageCheckBox.Attributes.Add("onclick", "AllowStorage_Changed()");
            AcceptKOPR.Attributes.Add("onclick", "AcceptKOPR_Changed()");
        }


        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveData();

                if (OKClicked != null)
                    OKClicked(Device);

                Close();
            }
            else
            {
                Show(false);
            }
        }

        /// <summary>
        /// Handles event when user clicks on "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void SaveData()
        {
            if (Device == null)
            {
                Device = new Device();
            }

            Device.Enabled = ActiveCheckBox.Checked;
            Device.AeTitle = AETitleTextBox.Text;
            Device.Description = DescriptionTextBox.Text;
            Device.Dhcp = DHCPCheckBox.Checked;
            Device.IpAddress = IPAddressTextBox.Text;
            int port;
            if (Int32.TryParse(PortTextBox.Text, out port))
                Device.Port = port;
            Device.ServerPartitionKey = DevicePage.CurrentPartition;
            Device.AllowStorage = AllowStorageCheckBox.Checked;
            Device.AllowQuery = AllowQueryCheckBox.Checked;
            Device.AllowRetrieve = AllowRetrieveCheckBox.Checked;
            Device.AllowAutoRoute = AllowAutoRouteCheckBox.Checked;

            if (AllowStorageCheckBox.Checked)
            {
                Device.AcceptKOPR = AcceptKOPR.Checked;
            }
            else
            {
                Device.AcceptKOPR = false;
            }

            Device.ThrottleMaxConnections = ThrottleSettingsTab.MaxConnections;
            Device.DeviceTypeEnum = DeviceTypeEnum.GetEnum(DeviceTypeDropDownList.SelectedItem.Value);
        }

        #endregion Protected methods

        #region Public methods
		public void UpdateLabels()
		{
			// Update the title and OK button text. Changing the image is the only way to do this, since the 
			// SkinID cannot be set dynamically after Page_PreInit.
			if (EditMode)
			{
				ModalDialog1.Title = SR.DialogEditDeviceTitle;
				DeviceTypeDropDownList.Items.FindByValue(Device.DeviceTypeEnum.Lookup).Selected = true;
			    OKButton.Visible = false;
			    UpdateButton.Visible = true;
			}
			else
			{
				ModalDialog1.Title = SR.DialogAddDeviceTitle;
				OKButton.Visible = true;
                UpdateButton.Visible = false;
			}

		}
        public void UpdateUI()
        {
            // update the dropdown list
            DeviceTypeDropDownList.Items.Clear();
            IList<DeviceTypeEnum> deviceTypes = DeviceTypeEnum.GetAll();
            foreach (DeviceTypeEnum t in deviceTypes)
            {
                DeviceTypeDropDownList.Items.Add(
                    new ListItem(ServerEnumDescription.GetLocalizedDescription(t), t.Lookup)
                    );
            }

        	UpdateLabels();

            // Update the rest of the fields
            if (Device == null)
            {
                AETitleTextBox.Text = SR.DeviceAE;
                IPAddressTextBox.Text = string.Empty;
                ActiveCheckBox.Checked = true;
                DHCPCheckBox.Checked = false;
                DescriptionTextBox.Text = string.Empty;
                PortTextBox.Text = SR.DeviceDefaultPort;
                AllowStorageCheckBox.Checked = true;
                AllowQueryCheckBox.Checked = true;
                AllowRetrieveCheckBox.Checked = true;
                AllowAutoRouteCheckBox.Checked = true;
                ThrottleSettingsTab.MaxConnections = UICommonSettings.Admin.Device.MaxConnections;
            }
            else if (Page.IsValid)
            {
                AETitleTextBox.Text = Device.AeTitle;
                IPAddressTextBox.Text = Device.IpAddress;
                ActiveCheckBox.Checked = Device.Enabled;
                DHCPCheckBox.Checked = Device.Dhcp;
                DescriptionTextBox.Text = Device.Description;
                PortTextBox.Text = Device.Port.ToString();
                AcceptKOPR.Checked = Device.AcceptKOPR;
                AllowStorageCheckBox.Checked = Device.AllowStorage;
                AllowQueryCheckBox.Checked = Device.AllowQuery;
                AllowRetrieveCheckBox.Checked = Device.AllowRetrieve;
                AllowAutoRouteCheckBox.Checked = Device.AllowAutoRoute;
                ThrottleSettingsTab.MaxConnections = Device.ThrottleMaxConnections;
            }
        }

        /// <summary>
        /// Displays the add/edit device dialog box.
        /// </summary>
        public void Show(bool updateUi)
        {
            if (updateUi)
                UpdateUI();
            else
            {
                if (EditMode)
                {
                    ModalDialog1.Title = SR.DialogEditDeviceTitle;
                } else
                {
                    ModalDialog1.Title = SR.DialogAddDeviceTitle;
                }
            }

            IPAddressTextBox.Enabled = !DHCPCheckBox.Checked;

            TabContainer1.ActiveTabIndex = 0;

            ModalDialog1.Show();
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            ModalDialog1.Hide();
        }

        #endregion Public methods
    }
}