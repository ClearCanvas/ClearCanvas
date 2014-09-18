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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Security;
using Resources;
using AjaxControlToolkit;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions
{
    //
    // Dialog for adding new Partition.
    //
    public partial class AddEditPartitionDialog : UserControl
    {
        #region Private Members

        private bool _editMode;
        // device being editted/added
        private ServerPartition _partition;

        #endregion

        #region Public Properties

        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;
                ViewState[ "EditMode"] = value;
            }
        }

	    /// <summary>
	    /// Sets/Gets the current editing partition.
	    /// </summary>
	    public ServerPartition Partition
	    {
		    set
		    {
			    _partition = value;
			    // put into viewstate to retrieve later
			    ViewState["EditedPartition"] = _partition;

			    dataAccessPanel.Partition = _partition;

			    if (value != null && !Page.IsPostBack)
			    {
				    ServerPartitionValidator.OriginalAeTitle = value.AeTitle;
				    PartitionFolderValidator.OriginalPartitionFolder = value.PartitionFolder;
			    }

			    bool enabled = _partition == null || !_partition.ServerPartitionTypeEnum.Equals(ServerPartitionTypeEnum.VFS);

			    // Disable some of the buttons when editing a VFS partition

			    StudyMatchingTabPanel.Visible = enabled;
				DataAccessTab.Visible = enabled;

			    AETitleTextBox.Enabled = enabled;
			    PortTextBox.Enabled = enabled;
			    EnabledCheckBox.Enabled = enabled;
			    DuplicateSopDropDownList.Enabled = enabled;
			    AcceptLatestReportCheckBox.Enabled = enabled;
		    }
		    get { return _partition; }
	    }

	    #endregion // public members

        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="OKClicked"/>.
        /// </summary>
        /// <param name="partition">The partition being added.</param>
        public delegate void OnOKClickedEventHandler(ServerPartitionInfo partition);

        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OnOKClickedEventHandler OKClicked;

        #endregion Events

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);


            //TODO: rewrite this. It's not pretty. 
            if (!SessionManager.Current.User.IsInRole(ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.AuthorityGroup))
            {
                for(int i=-0; i<ServerPartitionTabContainer.Tabs.Count;i++)
                {
                    if (ServerPartitionTabContainer.Tabs[i].ID == "DataAccessTab")
                    {
                        ServerPartitionTabContainer.Tabs.RemoveAt(i);
                        break;
                    }
                }
            }
            ServerPartitionTabContainer.ActiveTabIndex = 0;

            AutoInsertDeviceCheckBox.InputAttributes.Add("onclick", "EnableDisable();");

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID,
                                                        @"<script language='javascript'>
                    function EnableDisable()
                    {  
                         var autoInsertCheck = document.getElementById('" +
                                                        AutoInsertDeviceCheckBox.ClientID +
                                                        @"');
                         var defaultPortInput = document.getElementById('" +
                                                        DefaultRemotePortTextBox.ClientID +
                                                        @"');
                         defaultPortInput.disabled = !autoInsertCheck.checked;
                    }
                </script>");

            EditPartitionValidationSummary.HeaderText = ErrorMessages.EditPartitionValidationError;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                if (ViewState[ "EditMode"] != null)
                    _editMode = (bool) ViewState[ "EditMode"];

                if (ViewState[ "EditedPartition"] != null)
                {
                    _partition = ViewState[ "EditedPartition"] as ServerPartition;
                    ServerPartitionValidator.OriginalAeTitle = _partition.AeTitle;
                	PartitionFolderValidator.OriginalPartitionFolder = _partition.PartitionFolder;
                }
            }
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
                {
                    OKClicked(new ServerPartitionInfo()
                                  {
                                      Partition = Partition, 
                                      GroupsWithDataAccess = new List<String>(dataAccessPanel.SelectedDataAccessGroupRefs)
                                  });
                }

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
        

        protected void UpdateUI()
        {
            // Update the title and OK button text. Changing the image is the only way to do this, since the 
            // SkinID cannot be set dynamically after Page_PreInit.
    

            // update the dropdown list
            DuplicateSopDropDownList.Items.Clear();
            foreach (DuplicateSopPolicyEnum policyEnum in DuplicateSopPolicyEnum.GetAll())
            {
	            if (policyEnum.Equals(DuplicateSopPolicyEnum.AcceptLatest))
		            continue;

            	var item = new ListItem(ServerEnumDescription.GetLocalizedDescription(policyEnum), policyEnum.Lookup);
                DuplicateSopDropDownList.Items.Add(item);
            }

            if (Partition == null)
            {
                AETitleTextBox.Text = "SERVERAE";
                DescriptionTextBox.Text = string.Empty;
                PortTextBox.Text = "104";
                PartitionFolderTextBox.Text = "SERVERAE";
                EnabledCheckBox.Checked = true;
                AutoInsertDeviceCheckBox.Checked = true;
                AcceptAnyDeviceCheckBox.Checked = true;
                DefaultRemotePortTextBox.Text = "104";

                AutoInsertDeviceCheckBox.Enabled = true;
                DefaultRemotePortTextBox.Enabled = true;                

                DuplicateSopDropDownList.SelectedIndex = 0;
                AcceptLatestReportCheckBox.Checked = true;

                MatchPatientName.Checked = true;
                MatchPatientID.Checked = true;
                MatchPatientBirthDate.Checked = true;
                MatchPatientSex.Checked = true;
                MatchAccessionNumber.Checked = true;
                MatchIssuer.Checked = true;
                AuditDeleteStudyCheckBox.Checked = false;  //TODO: Load from system setting instead
            }
            else if (Page.IsValid)
                // only update the UI with the partition if the data is valid, otherwise, keep them on the screen
            {
                AETitleTextBox.Text = Partition.AeTitle;
                DescriptionTextBox.Text = Partition.Description;
                PortTextBox.Text = Partition.Port.ToString();
                PartitionFolderTextBox.Text = Partition.PartitionFolder;
                EnabledCheckBox.Checked = Partition.Enabled;
                AutoInsertDeviceCheckBox.Checked = Partition.AutoInsertDevice;
                AcceptAnyDeviceCheckBox.Checked = Partition.AcceptAnyDevice;
                DefaultRemotePortTextBox.Text = Partition.DefaultRemotePort.ToString();

                DefaultRemotePortTextBox.Enabled = Partition.AutoInsertDevice;

				if (!Partition.DuplicateSopPolicyEnum.Equals(DuplicateSopPolicyEnum.AcceptLatest))
					DuplicateSopDropDownList.SelectedValue = Partition.DuplicateSopPolicyEnum.Lookup;
                AcceptLatestReportCheckBox.Checked = Partition.AcceptLatestReport;

                MatchPatientName.Checked = Partition.MatchPatientsName;
                MatchPatientID.Checked = Partition.MatchPatientId;
                MatchPatientBirthDate.Checked = Partition.MatchPatientsBirthDate;
                MatchPatientSex.Checked = Partition.MatchPatientsSex;
                MatchAccessionNumber.Checked = Partition.MatchAccessionNumber;
                MatchIssuer.Checked= Partition.MatchIssuerOfPatientId;
                AuditDeleteStudyCheckBox.Checked = Partition.AuditDeleteStudy; 
            }
        }


        #region Private Methods


        private void SaveData()
        {
            if (Partition == null)
            {
                Partition = new ServerPartition { ServerPartitionTypeEnum = ServerPartitionTypeEnum.Standard };
            }


            Partition.Enabled = EnabledCheckBox.Checked;
            Partition.AeTitle = AETitleTextBox.Text;
            Partition.Description = DescriptionTextBox.Text;
            Partition.PartitionFolder = PartitionFolderTextBox.Text;

            int port;
            if (Int32.TryParse(PortTextBox.Text, out port))
                Partition.Port = port;

            Partition.AcceptAnyDevice = AcceptAnyDeviceCheckBox.Checked;
            Partition.AutoInsertDevice = AutoInsertDeviceCheckBox.Checked;
            if (Int32.TryParse(DefaultRemotePortTextBox.Text, out port))
                Partition.DefaultRemotePort = port;

            Partition.DuplicateSopPolicyEnum = DuplicateSopPolicyEnum.GetAll()[DuplicateSopDropDownList.SelectedIndex];
            Partition.AcceptLatestReport = AcceptLatestReportCheckBox.Checked;
            Partition.MatchPatientsName = MatchPatientName.Checked;
            Partition.MatchPatientId = MatchPatientID.Checked;
            Partition.MatchPatientsBirthDate = MatchPatientBirthDate.Checked;
            Partition.MatchPatientsSex = MatchPatientSex.Checked;
            Partition.MatchAccessionNumber = MatchAccessionNumber.Checked;
            Partition.MatchIssuerOfPatientId = MatchIssuer.Checked;
            Partition.AuditDeleteStudy = AuditDeleteStudyCheckBox.Checked;
        }

        #endregion Private Methods

        #endregion Protected methods

        #region Public Methods

        /// <summary>
        /// Displays the add device dialog box.
        /// </summary>
        public void Show(bool updateUI)
        {
			if (EditMode)
			{
				ModalDialog.Title = SR.DialogEditPartitionTitle;
				OKButton.Visible = false;
                UpdateButton.Visible = true;
            }
			else
			{
				ModalDialog.Title = SR.DialogAddPartitionTitle;
				OKButton.Visible = true;
                UpdateButton.Visible = false;
			}

            if (updateUI)
                UpdateUI();

            if (Page.IsValid)
            {
                ServerPartitionTabContainer.ActiveTabIndex = 0;
            }

            ModalDialog.Show();
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            ModalDialog.Hide();   
        }

        #endregion Public methods
    }

    public class ServerPartitionInfo
    {
        public ServerPartition Partition { get; set; }
        public List<string> GroupsWithDataAccess { get; set; }
    }
}
