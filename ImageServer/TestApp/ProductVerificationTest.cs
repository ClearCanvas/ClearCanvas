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
using System.ServiceModel;
using System.Windows.Forms;
using ClearCanvas.ImageServer.Common.ServiceModel;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class ProductVerificationTest : Form
    {
        public ProductVerificationTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProductVerificationTester test = new ProductVerificationTester();
            ProductVerificationResponse result = test.Verify();

            ComponentName.Text = String.Format("ComponentName: {0}", result.ComponentName);
            ManifestVerified.Text = String.Format("ManifestVerified: {0}", result.IsManifestValid);
        }
    }

    public class ProductVerificationTester
    {
        public ProductVerificationResponse Verify()
        {
            int port = 9998;
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.None);
            EndpointAddress endpoint = new EndpointAddress(String.Format("http://localhost:{0}/ClearCanvas.ImageServer.Common.ServiceModel.IProductVerificationService", port));

            IProductVerificationService service = ChannelFactory<IProductVerificationService>.CreateChannel(binding, endpoint);


            var result = service.Verify(new ProductVerificationRequest());
            return result;
        }
    }
}
