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
using System.Windows.Forms;
using ClearCanvas.Common.UsageTracking;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class UsageTrackingForm : Form
    {
        private readonly UsageMessage _message;

        public UsageTrackingForm()
        {
            InitializeComponent();
            UsageUtilities.MessageEvent += DisplayMessage;

            _message = UsageUtilities.GetUsageMessage();

            textBoxVersion.Text = _message.Version;
            textBoxProduct.Text = _message.Product;
            textBoxOS.Text = _message.OS;
            textBoxRegion.Text = _message.Region;
            textBoxLicense.Text = _message.LicenseString;
            textBoxComponent.Text = _message.Component;
            textBoxMachineIdentifier.Text = _message.MachineIdentifier;
            textBoxEdition.Text = _message.Edition;
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            _message.Version = textBoxVersion.Text;
            _message.Product = textBoxProduct.Text;
            _message.OS = textBoxOS.Text;
            _message.Region = textBoxRegion.Text;
            _message.LicenseString = textBoxLicense.Text;
            _message.Component = textBoxComponent.Text;
            _message.MachineIdentifier = textBoxMachineIdentifier.Text;
            _message.Edition = textBoxEdition.Text;
            _message.MessageType = UsageType.Other;

            if (!string.IsNullOrEmpty(textBoxAppKey.Text) 
                && !string.IsNullOrEmpty(textBoxAppValue.Text))
            {
                UsageApplicationData d = new UsageApplicationData
                                             {
                                                 Key = textBoxAppKey.Text,
                                                 Value = textBoxAppValue.Text
                                             };
                _message.AppData = new List<UsageApplicationData>
                                       {
                                           d
                                       };
            }

            UsageUtilities.Register(_message, UsageTrackingThread.Background);
        }

        private static void DisplayMessage(object o, ItemEventArgs<DisplayMessage> m)
        {
            MessageBox.Show(m.Item.Message, m.Item.Title);
        }
    }
}
