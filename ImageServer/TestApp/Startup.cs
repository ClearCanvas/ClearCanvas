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
using System.Windows.Forms;
using ClearCanvas.ImageServer.TestApp.PerfCFind;
using ClearCanvas.ImageServer.TestApp.PerfMon;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class Startup : Form
    {
        public Startup()
        {
            InitializeComponent();
        }


        private void TestRule_Click(object sender, EventArgs e)
        {
            var test = new TestDicomFileForm();
            test.Show();
        }

        private void TestHeaderStreamButton_Click(object sender, EventArgs e)
        {
            var test = new TestHeaderStreamingForm();
            test.Show();
        }

        private void buttonCompression_Click(object sender, EventArgs e)
        {
            var test = new TestCompressionForm();
            test.Show();
        }

        private void buttonEditStudy_Click(object sender, EventArgs e)
        {
            var test = new TestEditStudyForm();
            test.Show();
        }

        private void RandomImageSender_Click(object sender, EventArgs e)
        {
            var test = new TestSendImagesForm();
            test.Show();
        }

        private void ExtremeStreaming_Click(object sender, EventArgs e)
        {
            var test = new ImageStreamingStressTest();
            test.Show();
        }


        private void UsageTracking_Click(object sender, EventArgs e)
        {
        }

        private void ProductVerify_Click(object sender, EventArgs e)
        {
            var test = new ProductVerificationTest();
            test.Show();
        }

        private void DatabaseGenerator_Click(object sender, EventArgs e)
        {
            var form = new GenerateDatabase();
            form.Show();
        }

		private void _archiveTestBtn_Click(object sender, EventArgs e)
		{
			var form = new ArchiveTestForm();
			form.Show();
		}

		private void _perfMon_Click(object sender, EventArgs e)
		{
			new CFindPerformanceTestForm().Show();
		}
    }
}