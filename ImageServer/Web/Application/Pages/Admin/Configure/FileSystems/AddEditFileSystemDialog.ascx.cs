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
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.WebControls;
using Resources;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems
{
    //
    // Dialog for adding a new device or editting an existing one.
    //
    public partial class AddFilesystemDialog : UserControl
    {
        #region private variables

        // The server partitions that the new device can be associated with
        // This list will be determined by the user level permission.
        private IList<FilesystemTierEnum> _tiers = new List<FilesystemTierEnum>();

        private bool _editMode;
        private Filesystem _filesystem;

        #endregion

        #region public members

        /// <summary>
        /// Sets or gets the list of filesystem tiers which users are allowed to pick.
        /// </summary>
        public IList<FilesystemTierEnum> FilesystemTiers
        {
            set { _tiers = value; }

            get { return _tiers; }
        }

        /// <summary>
        /// Sets the dialog in edit mode or gets a value indicating whether the dialog is in edit mode.
        /// </summary>
        public bool EditMode
        {
            set
            {
                _editMode = value;
                ViewState[ "EditMode"] = value;
            }
            get { return _editMode; }
        }

        /// <summary>
        /// Sets or gets the filesystem users are working on.
        /// </summary>
        public Filesystem FileSystem
        {
            set
            {
                _filesystem = value;
                ViewState[ "_FileSystem"] = value;
            }
            get { return _filesystem; }
        }

        #endregion // public members

        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="OKClicked"/>.
        /// </summary>
        /// <param name="filesystem">The device being added.</param>
        public delegate void OKClickedEventHandler(Filesystem filesystem);

        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OKClickedEventHandler OKClicked;

        #endregion Events

        #region Public delegates

        #endregion // public delegates

        #region Protected methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RegisterClientSideScripts();


            HighWatermarkTextBox.Attributes["onkeyup"] = "RecalculateWatermark()";
            LowWatermarkTextBox.Attributes["onkeyup"] = "RecalculateWatermark()";

            EditFileSystemValidationSummary.HeaderText = ErrorMessages.EditFileSystemValidationError;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
            }
            else
            {
                // reload the filesystem information user is working on
                if (ViewState[ "EditMode"] != null)
                    _editMode = (bool)ViewState[ "EditMode"];

                FileSystem = ViewState[ "_FileSystem"] as Filesystem;
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
                    OKClicked(FileSystem);
                Close();
            }
            else
            {
                // TODO: Add mechanism to select the first tab where the error occurs
                Show(false);
            }
        }

        

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion Protected methods

        #region Private methods

        private void RegisterClientSideScripts()
        {
            ScriptTemplate template = new ScriptTemplate(typeof(AddFilesystemDialog).Assembly, "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.Filesystem.js");
            template.Replace("@@HW_PERCENTAGE_INPUT_CLIENTID@@", HighWatermarkTextBox.ClientID);
            template.Replace("@@HW_SIZE_CLIENTID@@", HighWatermarkSize.ClientID);
            template.Replace("@@LW_PERCENTAGE_INPUT_CLIENTID@@", LowWatermarkTextBox.ClientID);
            template.Replace("@@LW_SIZE_CLIENTID@@", LowWaterMarkSize.ClientID);
            template.Replace("@@PATH_INPUT_CLIENTID@@", PathTextBox.ClientID);
            template.Replace("@@TOTAL_SIZE_INDICATOR_CLIENTID@@", TotalSizeIndicator.ClientID);
            template.Replace("@@USED_SIZE_INDICATOR_CLIENTID@@", UsedSizeIndicator.ClientID);
            template.Replace("@@TOTAL_SIZE_CLIENTID@@", TotalSize.ClientID);
            template.Replace("@@USED_SIZE_CLIENTID@@", AvailableSize.ClientID);

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID+"_scripts", template.Script, true);
        }


        private void UpdateUI()
        {
            // Update the title and OK button text. Changing the image is the only way to do this, since the 
            // SkinID cannot be set dynamically after Page_PreInit.
            if (EditMode)
            {
                ModalDialog.Title = SR.DialogEditFileSystemTitle;
                OKButton.Visible = false;
                UpdateButton.Visible = true;
            }
            else
            {
                ModalDialog.Title = SR.DialogAddFileSystemTitle;
                OKButton.Visible = true;
                UpdateButton.Visible = false;
            }

            // update the dropdown list
            TiersDropDownList.Items.Clear();
            foreach (FilesystemTierEnum tier in _tiers)
            {
                TiersDropDownList.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(tier), tier.Lookup));
            }

            if (FileSystem == null)
            {
                // Clear input
                DescriptionTextBox.Text = string.Empty;
                PathTextBox.Text = string.Empty;
                ReadCheckBox.Checked = true;
                WriteCheckBox.Checked = true;
                LowWatermarkTextBox.Text = (80.00f).ToString();
                HighWatermarkTextBox.Text = (90.00f).ToString();

                TiersDropDownList.SelectedIndex = 0;
            }
            else if (Page.IsValid)
            {
                // set the data using the info in the filesystem to be editted
                DescriptionTextBox.Text = FileSystem.Description;
                PathTextBox.Text = FileSystem.FilesystemPath;
                ReadCheckBox.Checked = FileSystem.Enabled && (FileSystem.ReadOnly || (FileSystem.WriteOnly == false));
                WriteCheckBox.Checked = FileSystem.Enabled && (FileSystem.WriteOnly || (FileSystem.ReadOnly == false));
                LowWatermarkTextBox.Text = FileSystem.LowWatermark.ToString();
                HighWatermarkTextBox.Text = FileSystem.HighWatermark.ToString();
                TiersDropDownList.SelectedValue = FileSystem.FilesystemTierEnum.Lookup;
            }
        }

        private void SaveData()
        {
            
            if (FileSystem == null)
            {
                // create a filesystem 
                FileSystem = new Filesystem
                    {
                        LowWatermark = 80.00M, 
                        HighWatermark = 90.00M
                    };
            }

            FileSystem.Description = DescriptionTextBox.Text.Trim();
            FileSystem.FilesystemPath = PathTextBox.Text.Trim();
            FileSystem.ReadOnly = ReadCheckBox.Checked && WriteCheckBox.Checked == false;
            FileSystem.WriteOnly = WriteCheckBox.Checked && ReadCheckBox.Checked == false;
            FileSystem.Enabled = ReadCheckBox.Checked || WriteCheckBox.Checked;

            Decimal lowWatermark;
            if (Decimal.TryParse(LowWatermarkTextBox.Text, NumberStyles.Number, null, out lowWatermark))
                FileSystem.LowWatermark = lowWatermark;

            Decimal highWatermark;
			if (Decimal.TryParse(HighWatermarkTextBox.Text, NumberStyles.Number, null, out highWatermark))
                FileSystem.HighWatermark = highWatermark;

            FileSystem.FilesystemTierEnum = FilesystemTiers[TiersDropDownList.SelectedIndex];
        }

        #endregion Private methods

        #region Public methods

        /// <summary>
        /// Displays the add device dialog box.
        /// </summary>
        public void Show(bool updateUI)
        {
            if (updateUI)
                UpdateUI();
            else
            {
                if (EditMode)
                {
                    ModalDialog.Title = SR.DialogEditFileSystemTitle;
                } else
                {
                    ModalDialog.Title = SR.DialogAddFileSystemTitle;
                }
            }


            if (Page.IsValid)
            {
                TabContainer1.ActiveTabIndex = 0;
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

        public AddFilesystemDialog()
        {
            _editMode = false;
        }

        #endregion Public methods
    }
}
