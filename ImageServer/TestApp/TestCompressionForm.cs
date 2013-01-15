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
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class TestCompressionForm : Form
    {
        public TestCompressionForm()
        {
            InitializeComponent();

			//TODO: this can now just use DicomCodecRegistry.GetCodecTransferSyntaxes()
            this.comboBoxCompressionType.Items.Add(TransferSyntax.Jpeg2000ImageCompression);
            this.comboBoxCompressionType.Items.Add(TransferSyntax.Jpeg2000ImageCompressionLosslessOnly);
            this.comboBoxCompressionType.Items.Add(TransferSyntax.RleLossless);
            this.comboBoxCompressionType.Items.Add(TransferSyntax.JpegExtendedProcess24);
            this.comboBoxCompressionType.Items.Add(TransferSyntax.JpegBaselineProcess1);
            this.comboBoxCompressionType.Items.Add(TransferSyntax.JpegLosslessNonHierarchicalProcess14);
        }

        private void buttonDecompress_Click(object sender, EventArgs e)
        {
            if (this.textBoxSourceFile.Text.Length == 0 || this.textBoxDestinationFile.Text.Length == 0)
            {
                MessageBox.Show("Invalid source or destination filename");
                return;
            }

            DicomFile dicomFile = new DicomFile(textBoxSourceFile.Text);

            dicomFile.Load();

            dicomFile.Filename = textBoxDestinationFile.Text;

            dicomFile.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian);

            dicomFile.Save();
        }

        private void buttonBrowseSourceFile_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = textBoxSourceFile.Text;
            openFileDialog.ShowDialog();
            this.textBoxSourceFile.Text = this.openFileDialog.FileName;
        }

        private void buttonBrowseDestinationFile_Click(object sender, EventArgs e)
        {
            this.saveFileDialog.FileName = this.textBoxDestinationFile.Text;
            saveFileDialog.ShowDialog();
            textBoxDestinationFile.Text = saveFileDialog.FileName;
        }

        private void buttonCompress_Click(object sender, EventArgs e)
        {
            TransferSyntax syntax = this.comboBoxCompressionType.SelectedItem as TransferSyntax;
            if (syntax == null)
            {
                MessageBox.Show("Transfer syntax not selected");
                return;
            }

            DicomFile dicomFile = new DicomFile(textBoxSourceFile.Text);

            dicomFile.Load();
            if (dicomFile.TransferSyntax.Encapsulated)
            {
                MessageBox.Show(String.Format("Message encoded as {0}, cannot compress.", dicomFile.TransferSyntax));
                return;
            }

            dicomFile.Filename = textBoxDestinationFile.Text;

            dicomFile.ChangeTransferSyntax(syntax);

            dicomFile.Save();

        }
    }
}