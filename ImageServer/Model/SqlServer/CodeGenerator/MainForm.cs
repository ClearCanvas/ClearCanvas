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
using System.IO;
using System.Windows.Forms;

namespace ClearCanvas.ImageServer.Model.SqlServer.CodeGenerator
{
    public partial class MainForm : Form
    {
        private String _basePath;

        public MainForm()
        {
            InitializeComponent();

            _basePath = Path.GetDirectoryName(Application.ExecutablePath);
            DirectoryInfo dir = new DirectoryInfo(_basePath);
            dir = dir.Parent.Parent.Parent.Parent;
            _basePath = dir.FullName;

            textBoxModelFolder.Text = _basePath;
            textBoxEntityInterfaceFolder.Text = Path.Combine(_basePath, "EntityBrokers");
            textBoxEntityImplementationFolder.Text = Path.Combine(Path.Combine(_basePath, "SqlServer"), "EntityBrokers");

            comboBoxDatabase.Items.Add("ImageServer");
            comboBoxDatabase.Items.Add("MigrationTool");
            comboBoxDatabase.Items.Add("UsageTracking");
        }

        private void buttonGenerateCode_Click(object sender, EventArgs e)
        {
            Generator generator = new Generator
                                      {
                                          ModelNamespace = textBoxModelNamespace.Text,
                                          ImageServerModelFolder = textBoxModelFolder.Text,
                                          EntityImplementationFolder = textBoxEntityImplementationFolder.Text,
                                          EntityImplementationNamespace = textBoxEntityImplementationNamespace.Text,
                                          EntityInterfaceFolder = textBoxEntityInterfaceFolder.Text,
                                          EntityInterfaceNamespace = textBoxEntityInterfaceNamespace.Text
                                      };

            if (comboBoxDatabase.Text.Equals("ImageServer"))
            {
                generator.GenerateResxFile = true;
                generator.ConnectionStringName = "ImageServerConnectString";
                generator.Proprietary = false;
            }
            else if (comboBoxDatabase.Text.Equals("MigrationTool"))
            {
                generator.ConnectionStringName = "MonarchConnectString";
                generator.Proprietary = true;
            }
            else if (comboBoxDatabase.Text.Equals("UsageTracking"))
            {
                generator.ConnectionStringName = "UsageTrackingConnectString";
                generator.Proprietary = true;
            }

            generator.Generate();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Description = "Select Root ImageServer Project Folder";
            folderBrowserDialog.SelectedPath = textBoxModelFolder.Text;

            folderBrowserDialog.ShowDialog();

            textBoxModelFolder.Text = folderBrowserDialog.SelectedPath;
            textBoxEntityInterfaceFolder.Text = Path.Combine(folderBrowserDialog.SelectedPath, "EntityBrokers");
            textBoxEntityImplementationFolder.Text =
                Path.Combine(Path.Combine(folderBrowserDialog.SelectedPath, "SqlServer"), "EntityBrokers");
        }

        private void textBoxEntityImplementationNamespace_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDatabase.SelectedItem.Equals("ImageServer"))
            {
	            var curDir = new DirectoryInfo(Environment.CurrentDirectory);
	            var modelPath = curDir.Parent.Parent.Parent.Parent.FullName;
                textBoxModelNamespace.Text = "ClearCanvas.ImageServer.Model";
                textBoxEntityInterfaceNamespace.Text = "ClearCanvas.ImageServer.Model.EntityBrokers";
                textBoxEntityImplementationNamespace.Text = "ClearCanvas.ImageServer.Model.SqlServer.EntityBrokers";
	            textBoxModelFolder.Text = modelPath;
                textBoxEntityInterfaceFolder.Text = Path.Combine(modelPath,"EntityBrokers");
                textBoxEntityImplementationFolder.Text = Path.Combine(modelPath,@"SqlServer\EntityBrokers");
            }
            if (comboBoxDatabase.SelectedItem.Equals("MigrationTool"))
            {
                textBoxModelNamespace.Text = "ClearCanvas.Migration.Dicom.Model";
                textBoxEntityInterfaceNamespace.Text = "ClearCanvas.Migration.Dicom.Model.EntityBrokers";
                textBoxEntityImplementationNamespace.Text = "ClearCanvas.Migration.Dicom.Model.SqlServer.EntityBrokers";
                textBoxModelFolder.Text = "d:\\Projects\\Jin\\Migration\\Dicom\\Model";
                textBoxEntityInterfaceFolder.Text = "D:\\Projects\\Jin\\Migration\\Dicom\\Model\\EntityBrokers";
                textBoxEntityImplementationFolder.Text = "d:\\Projects\\Jin\\Migration\\Dicom\\Model\\SqlServer\\EntityBrokers";
            }
            if (comboBoxDatabase.SelectedItem.Equals("UsageTracking"))
            {
                textBoxModelNamespace.Text = "ClearCanvas.UsageTracking.Model";
                textBoxEntityInterfaceNamespace.Text = "ClearCanvas.UsageTracking.Model.EntityBrokers";
                textBoxEntityImplementationNamespace.Text = "ClearCanvas.UsageTracking.Model.SqlServer.EntityBrokers";
                textBoxModelFolder.Text = "d:\\Projects\\Jin\\UsageTracking\\Model";
                textBoxEntityInterfaceFolder.Text = "d:\\Projects\\Jin\\UsageTracking\\Model\\EntityBrokers";
                textBoxEntityImplementationFolder.Text = "d:\\Projects\\Jin\\UsageTracking\\Model\\SqlServer\\EntityBrokers";
            }
            Refresh();
        }
    }
}