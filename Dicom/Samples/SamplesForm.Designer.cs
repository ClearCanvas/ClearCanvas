#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

namespace ClearCanvas.Dicom.Samples
{
    partial class SamplesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SamplesSplitContainer = new System.Windows.Forms.SplitContainer();
            this.SamplesTabs = new System.Windows.Forms.TabControl();
            this.StorageScuTab = new System.Windows.Forms.TabPage();
            this._buttonStorageScuClearFiles = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this._textBoxStorageScuLocalAe = new System.Windows.Forms.TextBox();
            this._buttonStorageScuVerify = new System.Windows.Forms.Button();
            this._buttonStorageScuSelectDirectory = new System.Windows.Forms.Button();
            this._buttonStorageScuConnect = new System.Windows.Forms.Button();
            this.buttonStorageScuSelectFiles = new System.Windows.Forms.Button();
            this._textBoxStorageScuRemotePort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._textBoxStorageScuRemoteHost = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._textBoxStorageScuRemoteAe = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.StorageScpTab = new System.Windows.Forms.TabPage();
            this._checkBoxStorageScpList = new System.Windows.Forms.CheckBox();
            this._checkBoxStorageScpRLE = new System.Windows.Forms.CheckBox();
            this._checkBoxStorageScpJ2KLossy = new System.Windows.Forms.CheckBox();
            this._checkBoxStorageScpJ2KLossless = new System.Windows.Forms.CheckBox();
            this._checkBoxStorageScpJpegLossy = new System.Windows.Forms.CheckBox();
            this._checkBoxStorageScpJpegLossless = new System.Windows.Forms.CheckBox();
            this._checkBoxStorageScpBitbucket = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this._textBoxStorageScpAeTitle = new System.Windows.Forms.TextBox();
            this._buttonStorageScuSelectStorageLocation = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this._textBoxStorageScpStorageLocation = new System.Windows.Forms.TextBox();
            this._buttonStorageScpStartStop = new System.Windows.Forms.Button();
            this._textBoxStorageScpPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.CompressionTab = new System.Windows.Forms.TabPage();
            this._savePixelsButton = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this._sourceTransferSyntaxCombo = new System.Windows.Forms.ComboBox();
            this._destinationSyntaxCombo = new System.Windows.Forms.ComboBox();
            this._destinationPathTextBox = new System.Windows.Forms.TextBox();
            this._sourcePathTextBox = new System.Windows.Forms.TextBox();
            this._saveFileButton = new System.Windows.Forms.Button();
            this._openFileButton = new System.Windows.Forms.Button();
            this.queryScuTab = new System.Windows.Forms.TabPage();
            this.label18 = new System.Windows.Forms.Label();
            this.comboBoxQueryScuQueryLevel = new System.Windows.Forms.ComboBox();
            this.textBoxQueryScuMaxResults = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.comboBoxQueryScuQueryType = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxQueryScuLocalAe = new System.Windows.Forms.TextBox();
            this.textBoxQueryScuRemotePort = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxQueryScuRemoteHost = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxQueryScuRemoteAe = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxQueryMessage = new System.Windows.Forms.TextBox();
            this.buttonQueryScuSearch = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.buttonSendDicomdir = new System.Windows.Forms.Button();
            this._textBoxDicomdirRemotePort = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this._textBoxDicomdirRemoteHost = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this._textBoxDicomdirRemoteAe = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this._buttonOpenDicomdir = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this._textBoxDicomdir = new System.Windows.Forms.TextBox();
            this.moveScuTab = new System.Windows.Forms.TabPage();
            this.label23 = new System.Windows.Forms.Label();
            this.comboBoxMoveScuQueryLevel = new System.Windows.Forms.ComboBox();
            this.textBoxMoveScuMoveDestination = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.comboBoxMoveScuQueryType = new System.Windows.Forms.ComboBox();
            this.label26 = new System.Windows.Forms.Label();
            this.textBoxMoveScuLocalAe = new System.Windows.Forms.TextBox();
            this.textBoxMoveScuRemotePort = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.textBoxMoveScuRemoteHost = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.textBoxMoveScuRemoteAe = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.textBoxMoveMessage = new System.Windows.Forms.TextBox();
            this.buttonMoveScuMove = new System.Windows.Forms.Button();
            this._buttonOutputClearLog = new System.Windows.Forms.Button();
            this.OutputTextBox = new System.Windows.Forms.TextBox();
            this.openFileDialogStorageScu = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialogStorageScp = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserDialogStorageScu = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this._editSopDestinationPathTextBox = new System.Windows.Forms.TextBox();
            this._editSopSourcePathTextBox = new System.Windows.Forms.TextBox();
            this._editSopSaveFileButton = new System.Windows.Forms.Button();
            this._editSopOpenFileButton = new System.Windows.Forms.Button();
            this._editSopTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.SamplesSplitContainer)).BeginInit();
            this.SamplesSplitContainer.Panel1.SuspendLayout();
            this.SamplesSplitContainer.Panel2.SuspendLayout();
            this.SamplesSplitContainer.SuspendLayout();
            this.SamplesTabs.SuspendLayout();
            this.StorageScuTab.SuspendLayout();
            this.StorageScpTab.SuspendLayout();
            this.CompressionTab.SuspendLayout();
            this.queryScuTab.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.moveScuTab.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // SamplesSplitContainer
            // 
            this.SamplesSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SamplesSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.SamplesSplitContainer.Name = "SamplesSplitContainer";
            this.SamplesSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SamplesSplitContainer.Panel1
            // 
            this.SamplesSplitContainer.Panel1.Controls.Add(this.SamplesTabs);
            this.SamplesSplitContainer.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // SamplesSplitContainer.Panel2
            // 
            this.SamplesSplitContainer.Panel2.Controls.Add(this._buttonOutputClearLog);
            this.SamplesSplitContainer.Panel2.Controls.Add(this.OutputTextBox);
            this.SamplesSplitContainer.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SamplesSplitContainer.Size = new System.Drawing.Size(924, 590);
            this.SamplesSplitContainer.SplitterDistance = 155;
            this.SamplesSplitContainer.TabIndex = 0;
            // 
            // SamplesTabs
            // 
            this.SamplesTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SamplesTabs.Controls.Add(this.StorageScuTab);
            this.SamplesTabs.Controls.Add(this.StorageScpTab);
            this.SamplesTabs.Controls.Add(this.CompressionTab);
            this.SamplesTabs.Controls.Add(this.queryScuTab);
            this.SamplesTabs.Controls.Add(this.tabPage1);
            this.SamplesTabs.Controls.Add(this.moveScuTab);
            this.SamplesTabs.Controls.Add(this.tabPage2);
            this.SamplesTabs.Location = new System.Drawing.Point(3, 0);
            this.SamplesTabs.Name = "SamplesTabs";
            this.SamplesTabs.SelectedIndex = 0;
            this.SamplesTabs.Size = new System.Drawing.Size(918, 152);
            this.SamplesTabs.TabIndex = 0;
            // 
            // StorageScuTab
            // 
            this.StorageScuTab.Controls.Add(this._buttonStorageScuClearFiles);
            this.StorageScuTab.Controls.Add(this.label7);
            this.StorageScuTab.Controls.Add(this._textBoxStorageScuLocalAe);
            this.StorageScuTab.Controls.Add(this._buttonStorageScuVerify);
            this.StorageScuTab.Controls.Add(this._buttonStorageScuSelectDirectory);
            this.StorageScuTab.Controls.Add(this._buttonStorageScuConnect);
            this.StorageScuTab.Controls.Add(this.buttonStorageScuSelectFiles);
            this.StorageScuTab.Controls.Add(this._textBoxStorageScuRemotePort);
            this.StorageScuTab.Controls.Add(this.label3);
            this.StorageScuTab.Controls.Add(this._textBoxStorageScuRemoteHost);
            this.StorageScuTab.Controls.Add(this.label2);
            this.StorageScuTab.Controls.Add(this._textBoxStorageScuRemoteAe);
            this.StorageScuTab.Controls.Add(this.label1);
            this.StorageScuTab.Location = new System.Drawing.Point(4, 22);
            this.StorageScuTab.Name = "StorageScuTab";
            this.StorageScuTab.Padding = new System.Windows.Forms.Padding(3);
            this.StorageScuTab.Size = new System.Drawing.Size(910, 126);
            this.StorageScuTab.TabIndex = 0;
            this.StorageScuTab.Text = "StorageSCU";
            this.StorageScuTab.UseVisualStyleBackColor = true;
            // 
            // _buttonStorageScuClearFiles
            // 
            this._buttonStorageScuClearFiles.Location = new System.Drawing.Point(552, 81);
            this._buttonStorageScuClearFiles.Name = "_buttonStorageScuClearFiles";
            this._buttonStorageScuClearFiles.Size = new System.Drawing.Size(100, 23);
            this._buttonStorageScuClearFiles.TabIndex = 12;
            this._buttonStorageScuClearFiles.Text = "Clear Files";
            this._buttonStorageScuClearFiles.UseVisualStyleBackColor = true;
            this._buttonStorageScuClearFiles.Click += new System.EventHandler(this._buttonStorageScuClearFiles_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Local AE";
            // 
            // _textBoxStorageScuLocalAe
            // 
            this._textBoxStorageScuLocalAe.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScuLocalAETitle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._textBoxStorageScuLocalAe.Location = new System.Drawing.Point(9, 34);
            this._textBoxStorageScuLocalAe.Name = "_textBoxStorageScuLocalAe";
            this._textBoxStorageScuLocalAe.Size = new System.Drawing.Size(100, 20);
            this._textBoxStorageScuLocalAe.TabIndex = 10;
            this._textBoxStorageScuLocalAe.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScuLocalAETitle;
            // 
            // _buttonStorageScuVerify
            // 
            this._buttonStorageScuVerify.Location = new System.Drawing.Point(413, 81);
            this._buttonStorageScuVerify.Name = "_buttonStorageScuVerify";
            this._buttonStorageScuVerify.Size = new System.Drawing.Size(100, 23);
            this._buttonStorageScuVerify.TabIndex = 9;
            this._buttonStorageScuVerify.Text = "Verify";
            this._buttonStorageScuVerify.UseVisualStyleBackColor = true;
            this._buttonStorageScuVerify.Click += new System.EventHandler(this._buttonStorageScuVerify_Click);
            // 
            // _buttonStorageScuSelectDirectory
            // 
            this._buttonStorageScuSelectDirectory.Location = new System.Drawing.Point(9, 81);
            this._buttonStorageScuSelectDirectory.Name = "_buttonStorageScuSelectDirectory";
            this._buttonStorageScuSelectDirectory.Size = new System.Drawing.Size(100, 23);
            this._buttonStorageScuSelectDirectory.TabIndex = 8;
            this._buttonStorageScuSelectDirectory.Text = "Select Directory";
            this._buttonStorageScuSelectDirectory.UseVisualStyleBackColor = true;
            this._buttonStorageScuSelectDirectory.Click += new System.EventHandler(this.buttonStorageScuSelectDirectory_Click);
            // 
            // _buttonStorageScuConnect
            // 
            this._buttonStorageScuConnect.Location = new System.Drawing.Point(274, 81);
            this._buttonStorageScuConnect.Name = "_buttonStorageScuConnect";
            this._buttonStorageScuConnect.Size = new System.Drawing.Size(100, 23);
            this._buttonStorageScuConnect.TabIndex = 7;
            this._buttonStorageScuConnect.Text = "Connect";
            this._buttonStorageScuConnect.UseVisualStyleBackColor = true;
            this._buttonStorageScuConnect.Click += new System.EventHandler(this.ButtonStorageScuConnectClick);
            // 
            // buttonStorageScuSelectFiles
            // 
            this.buttonStorageScuSelectFiles.Location = new System.Drawing.Point(140, 81);
            this.buttonStorageScuSelectFiles.Name = "buttonStorageScuSelectFiles";
            this.buttonStorageScuSelectFiles.Size = new System.Drawing.Size(100, 23);
            this.buttonStorageScuSelectFiles.TabIndex = 6;
            this.buttonStorageScuSelectFiles.Text = "Select Files";
            this.buttonStorageScuSelectFiles.UseVisualStyleBackColor = true;
            this.buttonStorageScuSelectFiles.Click += new System.EventHandler(this.ButtonStorageScuSelectFilesClick);
            // 
            // _textBoxStorageScuRemotePort
            // 
            this._textBoxStorageScuRemotePort.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScuRemotePort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._textBoxStorageScuRemotePort.Location = new System.Drawing.Point(413, 35);
            this._textBoxStorageScuRemotePort.Name = "_textBoxStorageScuRemotePort";
            this._textBoxStorageScuRemotePort.Size = new System.Drawing.Size(100, 20);
            this._textBoxStorageScuRemotePort.TabIndex = 5;
            this._textBoxStorageScuRemotePort.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScuRemotePort;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(410, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Remote Port";
            // 
            // _textBoxStorageScuRemoteHost
            // 
            this._textBoxStorageScuRemoteHost.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScuRemoteHost", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._textBoxStorageScuRemoteHost.Location = new System.Drawing.Point(274, 35);
            this._textBoxStorageScuRemoteHost.Name = "_textBoxStorageScuRemoteHost";
            this._textBoxStorageScuRemoteHost.Size = new System.Drawing.Size(100, 20);
            this._textBoxStorageScuRemoteHost.TabIndex = 3;
            this._textBoxStorageScuRemoteHost.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScuRemoteHost;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(271, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Remote Host";
            // 
            // _textBoxStorageScuRemoteAe
            // 
            this._textBoxStorageScuRemoteAe.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScuRemoteAETitle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._textBoxStorageScuRemoteAe.Location = new System.Drawing.Point(140, 35);
            this._textBoxStorageScuRemoteAe.Name = "_textBoxStorageScuRemoteAe";
            this._textBoxStorageScuRemoteAe.Size = new System.Drawing.Size(100, 20);
            this._textBoxStorageScuRemoteAe.TabIndex = 1;
            this._textBoxStorageScuRemoteAe.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScuRemoteAETitle;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(137, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Remote AE";
            // 
            // StorageScpTab
            // 
            this.StorageScpTab.Controls.Add(this._checkBoxStorageScpList);
            this.StorageScpTab.Controls.Add(this._checkBoxStorageScpRLE);
            this.StorageScpTab.Controls.Add(this._checkBoxStorageScpJ2KLossy);
            this.StorageScpTab.Controls.Add(this._checkBoxStorageScpJ2KLossless);
            this.StorageScpTab.Controls.Add(this._checkBoxStorageScpJpegLossy);
            this.StorageScpTab.Controls.Add(this._checkBoxStorageScpJpegLossless);
            this.StorageScpTab.Controls.Add(this._checkBoxStorageScpBitbucket);
            this.StorageScpTab.Controls.Add(this.label6);
            this.StorageScpTab.Controls.Add(this._textBoxStorageScpAeTitle);
            this.StorageScpTab.Controls.Add(this._buttonStorageScuSelectStorageLocation);
            this.StorageScpTab.Controls.Add(this.label5);
            this.StorageScpTab.Controls.Add(this._textBoxStorageScpStorageLocation);
            this.StorageScpTab.Controls.Add(this._buttonStorageScpStartStop);
            this.StorageScpTab.Controls.Add(this._textBoxStorageScpPort);
            this.StorageScpTab.Controls.Add(this.label4);
            this.StorageScpTab.Location = new System.Drawing.Point(4, 22);
            this.StorageScpTab.Name = "StorageScpTab";
            this.StorageScpTab.Padding = new System.Windows.Forms.Padding(3);
            this.StorageScpTab.Size = new System.Drawing.Size(910, 126);
            this.StorageScpTab.TabIndex = 1;
            this.StorageScpTab.Text = "StorageSCP";
            this.StorageScpTab.UseVisualStyleBackColor = true;
            // 
            // _checkBoxStorageScpList
            // 
            this._checkBoxStorageScpList.AutoSize = true;
            this._checkBoxStorageScpList.Location = new System.Drawing.Point(540, 80);
            this._checkBoxStorageScpList.Name = "_checkBoxStorageScpList";
            this._checkBoxStorageScpList.Size = new System.Drawing.Size(42, 17);
            this._checkBoxStorageScpList.TabIndex = 14;
            this._checkBoxStorageScpList.Text = "List";
            this._checkBoxStorageScpList.UseVisualStyleBackColor = true;
            // 
            // _checkBoxStorageScpRLE
            // 
            this._checkBoxStorageScpRLE.AutoSize = true;
            this._checkBoxStorageScpRLE.Location = new System.Drawing.Point(771, 100);
            this._checkBoxStorageScpRLE.Name = "_checkBoxStorageScpRLE";
            this._checkBoxStorageScpRLE.Size = new System.Drawing.Size(47, 17);
            this._checkBoxStorageScpRLE.TabIndex = 13;
            this._checkBoxStorageScpRLE.Text = "RLE";
            this._checkBoxStorageScpRLE.UseVisualStyleBackColor = true;
            // 
            // _checkBoxStorageScpJ2KLossy
            // 
            this._checkBoxStorageScpJ2KLossy.AutoSize = true;
            this._checkBoxStorageScpJ2KLossy.Location = new System.Drawing.Point(771, 77);
            this._checkBoxStorageScpJ2KLossy.Name = "_checkBoxStorageScpJ2KLossy";
            this._checkBoxStorageScpJ2KLossy.Size = new System.Drawing.Size(110, 17);
            this._checkBoxStorageScpJ2KLossy.TabIndex = 12;
            this._checkBoxStorageScpJ2KLossy.Text = "JPEG 2000 Lossy";
            this._checkBoxStorageScpJ2KLossy.UseVisualStyleBackColor = true;
            // 
            // _checkBoxStorageScpJ2KLossless
            // 
            this._checkBoxStorageScpJ2KLossless.AutoSize = true;
            this._checkBoxStorageScpJ2KLossless.Location = new System.Drawing.Point(771, 55);
            this._checkBoxStorageScpJ2KLossless.Name = "_checkBoxStorageScpJ2KLossless";
            this._checkBoxStorageScpJ2KLossless.Size = new System.Drawing.Size(123, 17);
            this._checkBoxStorageScpJ2KLossless.TabIndex = 11;
            this._checkBoxStorageScpJ2KLossless.Text = "JPEG 2000 Lossless";
            this._checkBoxStorageScpJ2KLossless.UseVisualStyleBackColor = true;
            // 
            // _checkBoxStorageScpJpegLossy
            // 
            this._checkBoxStorageScpJpegLossy.AutoSize = true;
            this._checkBoxStorageScpJpegLossy.Location = new System.Drawing.Point(771, 32);
            this._checkBoxStorageScpJpegLossy.Name = "_checkBoxStorageScpJpegLossy";
            this._checkBoxStorageScpJpegLossy.Size = new System.Drawing.Size(83, 17);
            this._checkBoxStorageScpJpegLossy.TabIndex = 10;
            this._checkBoxStorageScpJpegLossy.Text = "JPEG Lossy";
            this._checkBoxStorageScpJpegLossy.UseVisualStyleBackColor = true;
            // 
            // _checkBoxStorageScpJpegLossless
            // 
            this._checkBoxStorageScpJpegLossless.AutoSize = true;
            this._checkBoxStorageScpJpegLossless.Location = new System.Drawing.Point(771, 9);
            this._checkBoxStorageScpJpegLossless.Name = "_checkBoxStorageScpJpegLossless";
            this._checkBoxStorageScpJpegLossless.Size = new System.Drawing.Size(96, 17);
            this._checkBoxStorageScpJpegLossless.TabIndex = 9;
            this._checkBoxStorageScpJpegLossless.Text = "JPEG Lossless";
            this._checkBoxStorageScpJpegLossless.UseVisualStyleBackColor = true;
            // 
            // _checkBoxStorageScpBitbucket
            // 
            this._checkBoxStorageScpBitbucket.AutoSize = true;
            this._checkBoxStorageScpBitbucket.Location = new System.Drawing.Point(440, 80);
            this._checkBoxStorageScpBitbucket.Name = "_checkBoxStorageScpBitbucket";
            this._checkBoxStorageScpBitbucket.Size = new System.Drawing.Size(71, 17);
            this._checkBoxStorageScpBitbucket.TabIndex = 8;
            this._checkBoxStorageScpBitbucket.Text = "Bitbucket";
            this._checkBoxStorageScpBitbucket.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(118, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "AE Title";
            // 
            // _textBoxStorageScpAeTitle
            // 
            this._textBoxStorageScpAeTitle.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScpAETitle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._textBoxStorageScpAeTitle.Location = new System.Drawing.Point(118, 37);
            this._textBoxStorageScpAeTitle.Name = "_textBoxStorageScpAeTitle";
            this._textBoxStorageScpAeTitle.Size = new System.Drawing.Size(124, 20);
            this._textBoxStorageScpAeTitle.TabIndex = 6;
            this._textBoxStorageScpAeTitle.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScpAETitle;
            // 
            // _buttonStorageScuSelectStorageLocation
            // 
            this._buttonStorageScuSelectStorageLocation.Location = new System.Drawing.Point(644, 35);
            this._buttonStorageScuSelectStorageLocation.Name = "_buttonStorageScuSelectStorageLocation";
            this._buttonStorageScuSelectStorageLocation.Size = new System.Drawing.Size(75, 23);
            this._buttonStorageScuSelectStorageLocation.TabIndex = 5;
            this._buttonStorageScuSelectStorageLocation.Text = "Select...";
            this._buttonStorageScuSelectStorageLocation.UseVisualStyleBackColor = true;
            this._buttonStorageScuSelectStorageLocation.Click += new System.EventHandler(this._buttonStorageScuSelectStorageLocation_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(440, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Storage Location";
            // 
            // _textBoxStorageScpStorageLocation
            // 
            this._textBoxStorageScpStorageLocation.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScpStorageFolder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._textBoxStorageScpStorageLocation.Location = new System.Drawing.Point(440, 37);
            this._textBoxStorageScpStorageLocation.Name = "_textBoxStorageScpStorageLocation";
            this._textBoxStorageScpStorageLocation.Size = new System.Drawing.Size(198, 20);
            this._textBoxStorageScpStorageLocation.TabIndex = 3;
            this._textBoxStorageScpStorageLocation.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScpStorageFolder;
            // 
            // _buttonStorageScpStartStop
            // 
            this._buttonStorageScpStartStop.Location = new System.Drawing.Point(6, 34);
            this._buttonStorageScpStartStop.Name = "_buttonStorageScpStartStop";
            this._buttonStorageScpStartStop.Size = new System.Drawing.Size(75, 23);
            this._buttonStorageScpStartStop.TabIndex = 2;
            this._buttonStorageScpStartStop.Text = "Start";
            this._buttonStorageScpStartStop.UseVisualStyleBackColor = true;
            this._buttonStorageScpStartStop.Click += new System.EventHandler(this.ButtonStorageScpStartStopClick);
            // 
            // _textBoxStorageScpPort
            // 
            this._textBoxStorageScpPort.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScpPort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._textBoxStorageScpPort.Location = new System.Drawing.Point(291, 37);
            this._textBoxStorageScpPort.Name = "_textBoxStorageScpPort";
            this._textBoxStorageScpPort.Size = new System.Drawing.Size(100, 20);
            this._textBoxStorageScpPort.TabIndex = 1;
            this._textBoxStorageScpPort.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScpPort;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(288, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Port";
            // 
            // CompressionTab
            // 
            this.CompressionTab.Controls.Add(this._savePixelsButton);
            this.CompressionTab.Controls.Add(this.label11);
            this.CompressionTab.Controls.Add(this.label10);
            this.CompressionTab.Controls.Add(this.label9);
            this.CompressionTab.Controls.Add(this.label8);
            this.CompressionTab.Controls.Add(this._sourceTransferSyntaxCombo);
            this.CompressionTab.Controls.Add(this._destinationSyntaxCombo);
            this.CompressionTab.Controls.Add(this._destinationPathTextBox);
            this.CompressionTab.Controls.Add(this._sourcePathTextBox);
            this.CompressionTab.Controls.Add(this._saveFileButton);
            this.CompressionTab.Controls.Add(this._openFileButton);
            this.CompressionTab.Location = new System.Drawing.Point(4, 22);
            this.CompressionTab.Name = "CompressionTab";
            this.CompressionTab.Padding = new System.Windows.Forms.Padding(3);
            this.CompressionTab.Size = new System.Drawing.Size(910, 126);
            this.CompressionTab.TabIndex = 2;
            this.CompressionTab.Text = "Compression";
            this.CompressionTab.UseVisualStyleBackColor = true;
            // 
            // _savePixelsButton
            // 
            this._savePixelsButton.Location = new System.Drawing.Point(742, 72);
            this._savePixelsButton.Name = "_savePixelsButton";
            this._savePixelsButton.Size = new System.Drawing.Size(98, 23);
            this._savePixelsButton.TabIndex = 10;
            this._savePixelsButton.Text = "Save Pixels";
            this._savePixelsButton.UseVisualStyleBackColor = true;
            this._savePixelsButton.Click += new System.EventHandler(this.SavePixelsButtonClick);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(517, 58);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(137, 13);
            this.label11.TabIndex = 9;
            this.label11.Text = "Destination Transfer Syntax";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(514, 13);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(118, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Source Transfer Syntax";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 14);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Source File Path";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 59);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(104, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Destination File Path";
            // 
            // _sourceTransferSyntaxCombo
            // 
            this._sourceTransferSyntaxCombo.FormattingEnabled = true;
            this._sourceTransferSyntaxCombo.Location = new System.Drawing.Point(514, 30);
            this._sourceTransferSyntaxCombo.Name = "_sourceTransferSyntaxCombo";
            this._sourceTransferSyntaxCombo.Size = new System.Drawing.Size(182, 21);
            this._sourceTransferSyntaxCombo.TabIndex = 5;
            // 
            // _destinationSyntaxCombo
            // 
            this._destinationSyntaxCombo.FormattingEnabled = true;
            this._destinationSyntaxCombo.Location = new System.Drawing.Point(514, 73);
            this._destinationSyntaxCombo.Name = "_destinationSyntaxCombo";
            this._destinationSyntaxCombo.Size = new System.Drawing.Size(182, 21);
            this._destinationSyntaxCombo.TabIndex = 4;
            // 
            // _destinationPathTextBox
            // 
            this._destinationPathTextBox.Location = new System.Drawing.Point(9, 75);
            this._destinationPathTextBox.Name = "_destinationPathTextBox";
            this._destinationPathTextBox.ReadOnly = true;
            this._destinationPathTextBox.Size = new System.Drawing.Size(358, 20);
            this._destinationPathTextBox.TabIndex = 3;
            // 
            // _sourcePathTextBox
            // 
            this._sourcePathTextBox.Location = new System.Drawing.Point(9, 30);
            this._sourcePathTextBox.Name = "_sourcePathTextBox";
            this._sourcePathTextBox.ReadOnly = true;
            this._sourcePathTextBox.Size = new System.Drawing.Size(358, 20);
            this._sourcePathTextBox.TabIndex = 2;
            // 
            // _saveFileButton
            // 
            this._saveFileButton.Location = new System.Drawing.Point(373, 73);
            this._saveFileButton.Name = "_saveFileButton";
            this._saveFileButton.Size = new System.Drawing.Size(75, 23);
            this._saveFileButton.TabIndex = 1;
            this._saveFileButton.Text = "Save File";
            this._saveFileButton.UseVisualStyleBackColor = true;
            this._saveFileButton.Click += new System.EventHandler(this.SaveFileButtonClick);
            // 
            // _openFileButton
            // 
            this._openFileButton.Location = new System.Drawing.Point(373, 28);
            this._openFileButton.Name = "_openFileButton";
            this._openFileButton.Size = new System.Drawing.Size(75, 23);
            this._openFileButton.TabIndex = 0;
            this._openFileButton.Text = "Open File";
            this._openFileButton.UseVisualStyleBackColor = true;
            this._openFileButton.Click += new System.EventHandler(this.OpenFileButtonClick);
            // 
            // queryScuTab
            // 
            this.queryScuTab.Controls.Add(this.label18);
            this.queryScuTab.Controls.Add(this.comboBoxQueryScuQueryLevel);
            this.queryScuTab.Controls.Add(this.textBoxQueryScuMaxResults);
            this.queryScuTab.Controls.Add(this.label17);
            this.queryScuTab.Controls.Add(this.label16);
            this.queryScuTab.Controls.Add(this.comboBoxQueryScuQueryType);
            this.queryScuTab.Controls.Add(this.label12);
            this.queryScuTab.Controls.Add(this.textBoxQueryScuLocalAe);
            this.queryScuTab.Controls.Add(this.textBoxQueryScuRemotePort);
            this.queryScuTab.Controls.Add(this.label13);
            this.queryScuTab.Controls.Add(this.textBoxQueryScuRemoteHost);
            this.queryScuTab.Controls.Add(this.label14);
            this.queryScuTab.Controls.Add(this.textBoxQueryScuRemoteAe);
            this.queryScuTab.Controls.Add(this.label15);
            this.queryScuTab.Controls.Add(this.textBoxQueryMessage);
            this.queryScuTab.Controls.Add(this.buttonQueryScuSearch);
            this.queryScuTab.Location = new System.Drawing.Point(4, 22);
            this.queryScuTab.Name = "queryScuTab";
            this.queryScuTab.Padding = new System.Windows.Forms.Padding(3);
            this.queryScuTab.Size = new System.Drawing.Size(910, 126);
            this.queryScuTab.TabIndex = 3;
            this.queryScuTab.Text = "QuerySCU";
            this.queryScuTab.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(154, 70);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(33, 13);
            this.label18.TabIndex = 25;
            this.label18.Text = "Level";
            // 
            // comboBoxQueryScuQueryLevel
            // 
            this.comboBoxQueryScuQueryLevel.FormattingEnabled = true;
            this.comboBoxQueryScuQueryLevel.Items.AddRange(new object[] {
            "PATIENT",
            "STUDY",
            "SERIES",
            "IMAGE"});
            this.comboBoxQueryScuQueryLevel.Location = new System.Drawing.Point(154, 87);
            this.comboBoxQueryScuQueryLevel.Name = "comboBoxQueryScuQueryLevel";
            this.comboBoxQueryScuQueryLevel.Size = new System.Drawing.Size(100, 21);
            this.comboBoxQueryScuQueryLevel.TabIndex = 24;
            this.comboBoxQueryScuQueryLevel.Text = "STUDY";
            this.comboBoxQueryScuQueryLevel.SelectedIndexChanged += new System.EventHandler(this.ComboBoxQueryScuQueryLevelSelectedIndexChanged);
            // 
            // textBoxQueryScuMaxResults
            // 
            this.textBoxQueryScuMaxResults.Location = new System.Drawing.Point(288, 87);
            this.textBoxQueryScuMaxResults.Name = "textBoxQueryScuMaxResults";
            this.textBoxQueryScuMaxResults.Size = new System.Drawing.Size(100, 20);
            this.textBoxQueryScuMaxResults.TabIndex = 23;
            this.textBoxQueryScuMaxResults.Text = "-1";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(285, 71);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(136, 13);
            this.label17.TabIndex = 22;
            this.label17.Text = "Max Results (-1 = unlimited)";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(23, 71);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(62, 13);
            this.label16.TabIndex = 21;
            this.label16.Text = "Query Type";
            // 
            // comboBoxQueryScuQueryType
            // 
            this.comboBoxQueryScuQueryType.FormattingEnabled = true;
            this.comboBoxQueryScuQueryType.Items.AddRange(new object[] {
            "Study Root",
            "Patient Root"});
            this.comboBoxQueryScuQueryType.Location = new System.Drawing.Point(23, 87);
            this.comboBoxQueryScuQueryType.Name = "comboBoxQueryScuQueryType";
            this.comboBoxQueryScuQueryType.Size = new System.Drawing.Size(121, 21);
            this.comboBoxQueryScuQueryType.TabIndex = 20;
            this.comboBoxQueryScuQueryType.Text = "Study Root";
            this.comboBoxQueryScuQueryType.SelectedIndexChanged += new System.EventHandler(this.ComboBoxQueryScuQueryTypeSelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(23, 14);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(50, 13);
            this.label12.TabIndex = 19;
            this.label12.Text = "Local AE";
            // 
            // textBoxQueryScuLocalAe
            // 
            this.textBoxQueryScuLocalAe.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScuLocalAETitle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxQueryScuLocalAe.Location = new System.Drawing.Point(23, 31);
            this.textBoxQueryScuLocalAe.Name = "textBoxQueryScuLocalAe";
            this.textBoxQueryScuLocalAe.Size = new System.Drawing.Size(100, 20);
            this.textBoxQueryScuLocalAe.TabIndex = 18;
            this.textBoxQueryScuLocalAe.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScuLocalAETitle;
            // 
            // textBoxQueryScuRemotePort
            // 
            this.textBoxQueryScuRemotePort.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScuRemotePort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxQueryScuRemotePort.Location = new System.Drawing.Point(427, 32);
            this.textBoxQueryScuRemotePort.Name = "textBoxQueryScuRemotePort";
            this.textBoxQueryScuRemotePort.Size = new System.Drawing.Size(100, 20);
            this.textBoxQueryScuRemotePort.TabIndex = 17;
            this.textBoxQueryScuRemotePort.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScuRemotePort;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(424, 15);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(66, 13);
            this.label13.TabIndex = 16;
            this.label13.Text = "Remote Port";
            // 
            // textBoxQueryScuRemoteHost
            // 
            this.textBoxQueryScuRemoteHost.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScuRemoteHost", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxQueryScuRemoteHost.Location = new System.Drawing.Point(288, 32);
            this.textBoxQueryScuRemoteHost.Name = "textBoxQueryScuRemoteHost";
            this.textBoxQueryScuRemoteHost.Size = new System.Drawing.Size(100, 20);
            this.textBoxQueryScuRemoteHost.TabIndex = 15;
            this.textBoxQueryScuRemoteHost.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScuRemoteHost;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(285, 15);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(69, 13);
            this.label14.TabIndex = 14;
            this.label14.Text = "Remote Host";
            // 
            // textBoxQueryScuRemoteAe
            // 
            this.textBoxQueryScuRemoteAe.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScuRemoteAETitle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxQueryScuRemoteAe.Location = new System.Drawing.Point(154, 32);
            this.textBoxQueryScuRemoteAe.Name = "textBoxQueryScuRemoteAe";
            this.textBoxQueryScuRemoteAe.Size = new System.Drawing.Size(100, 20);
            this.textBoxQueryScuRemoteAe.TabIndex = 13;
            this.textBoxQueryScuRemoteAe.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScuRemoteAETitle;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(151, 15);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(61, 13);
            this.label15.TabIndex = 12;
            this.label15.Text = "Remote AE";
            // 
            // textBoxQueryMessage
            // 
            this.textBoxQueryMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxQueryMessage.Location = new System.Drawing.Point(556, 6);
            this.textBoxQueryMessage.Multiline = true;
            this.textBoxQueryMessage.Name = "textBoxQueryMessage";
            this.textBoxQueryMessage.Size = new System.Drawing.Size(348, 114);
            this.textBoxQueryMessage.TabIndex = 1;
            // 
            // buttonQueryScuSearch
            // 
            this.buttonQueryScuSearch.Location = new System.Drawing.Point(452, 87);
            this.buttonQueryScuSearch.Name = "buttonQueryScuSearch";
            this.buttonQueryScuSearch.Size = new System.Drawing.Size(75, 23);
            this.buttonQueryScuSearch.TabIndex = 0;
            this.buttonQueryScuSearch.Text = "Search";
            this.buttonQueryScuSearch.UseVisualStyleBackColor = true;
            this.buttonQueryScuSearch.Click += new System.EventHandler(this.buttonQueryScuSearch_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.buttonSendDicomdir);
            this.tabPage1.Controls.Add(this._textBoxDicomdirRemotePort);
            this.tabPage1.Controls.Add(this.label20);
            this.tabPage1.Controls.Add(this._textBoxDicomdirRemoteHost);
            this.tabPage1.Controls.Add(this.label21);
            this.tabPage1.Controls.Add(this._textBoxDicomdirRemoteAe);
            this.tabPage1.Controls.Add(this.label22);
            this.tabPage1.Controls.Add(this._buttonOpenDicomdir);
            this.tabPage1.Controls.Add(this.label19);
            this.tabPage1.Controls.Add(this._textBoxDicomdir);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(910, 126);
            this.tabPage1.TabIndex = 4;
            this.tabPage1.Text = "DICOMDIR Reader";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // buttonSendDicomdir
            // 
            this.buttonSendDicomdir.Location = new System.Drawing.Point(426, 77);
            this.buttonSendDicomdir.Name = "buttonSendDicomdir";
            this.buttonSendDicomdir.Size = new System.Drawing.Size(150, 23);
            this.buttonSendDicomdir.TabIndex = 12;
            this.buttonSendDicomdir.Text = "Send DICOMDIR Files";
            this.buttonSendDicomdir.UseVisualStyleBackColor = true;
            this.buttonSendDicomdir.Click += new System.EventHandler(this.ButtonSendDicomdirClick);
            // 
            // _textBoxDicomdirRemotePort
            // 
            this._textBoxDicomdirRemotePort.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScuRemotePort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._textBoxDicomdirRemotePort.Location = new System.Drawing.Point(282, 80);
            this._textBoxDicomdirRemotePort.Name = "_textBoxDicomdirRemotePort";
            this._textBoxDicomdirRemotePort.Size = new System.Drawing.Size(100, 20);
            this._textBoxDicomdirRemotePort.TabIndex = 11;
            this._textBoxDicomdirRemotePort.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScuRemotePort;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(279, 63);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(66, 13);
            this.label20.TabIndex = 10;
            this.label20.Text = "Remote Port";
            // 
            // _textBoxDicomdirRemoteHost
            // 
            this._textBoxDicomdirRemoteHost.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScuRemoteHost", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._textBoxDicomdirRemoteHost.Location = new System.Drawing.Point(143, 80);
            this._textBoxDicomdirRemoteHost.Name = "_textBoxDicomdirRemoteHost";
            this._textBoxDicomdirRemoteHost.Size = new System.Drawing.Size(100, 20);
            this._textBoxDicomdirRemoteHost.TabIndex = 9;
            this._textBoxDicomdirRemoteHost.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScuRemoteHost;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(140, 63);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(69, 13);
            this.label21.TabIndex = 8;
            this.label21.Text = "Remote Host";
            // 
            // _textBoxDicomdirRemoteAe
            // 
            this._textBoxDicomdirRemoteAe.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScuRemoteAETitle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._textBoxDicomdirRemoteAe.Location = new System.Drawing.Point(9, 80);
            this._textBoxDicomdirRemoteAe.Name = "_textBoxDicomdirRemoteAe";
            this._textBoxDicomdirRemoteAe.Size = new System.Drawing.Size(100, 20);
            this._textBoxDicomdirRemoteAe.TabIndex = 7;
            this._textBoxDicomdirRemoteAe.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScuRemoteAETitle;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 63);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(61, 13);
            this.label22.TabIndex = 6;
            this.label22.Text = "Remote AE";
            // 
            // _buttonOpenDicomdir
            // 
            this._buttonOpenDicomdir.Location = new System.Drawing.Point(524, 23);
            this._buttonOpenDicomdir.Name = "_buttonOpenDicomdir";
            this._buttonOpenDicomdir.Size = new System.Drawing.Size(123, 23);
            this._buttonOpenDicomdir.TabIndex = 2;
            this._buttonOpenDicomdir.Text = "Open DICOMDIR";
            this._buttonOpenDicomdir.UseVisualStyleBackColor = true;
            this._buttonOpenDicomdir.Click += new System.EventHandler(this.ButtonOpenDicomdirClick);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(9, 7);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(80, 13);
            this.label19.TabIndex = 1;
            this.label19.Text = "DICOMDIR File";
            // 
            // _textBoxDicomdir
            // 
            this._textBoxDicomdir.Location = new System.Drawing.Point(9, 23);
            this._textBoxDicomdir.Name = "_textBoxDicomdir";
            this._textBoxDicomdir.ReadOnly = true;
            this._textBoxDicomdir.Size = new System.Drawing.Size(509, 20);
            this._textBoxDicomdir.TabIndex = 0;
            // 
            // moveScuTab
            // 
            this.moveScuTab.Controls.Add(this.label23);
            this.moveScuTab.Controls.Add(this.comboBoxMoveScuQueryLevel);
            this.moveScuTab.Controls.Add(this.textBoxMoveScuMoveDestination);
            this.moveScuTab.Controls.Add(this.label24);
            this.moveScuTab.Controls.Add(this.label25);
            this.moveScuTab.Controls.Add(this.comboBoxMoveScuQueryType);
            this.moveScuTab.Controls.Add(this.label26);
            this.moveScuTab.Controls.Add(this.textBoxMoveScuLocalAe);
            this.moveScuTab.Controls.Add(this.textBoxMoveScuRemotePort);
            this.moveScuTab.Controls.Add(this.label27);
            this.moveScuTab.Controls.Add(this.textBoxMoveScuRemoteHost);
            this.moveScuTab.Controls.Add(this.label28);
            this.moveScuTab.Controls.Add(this.textBoxMoveScuRemoteAe);
            this.moveScuTab.Controls.Add(this.label29);
            this.moveScuTab.Controls.Add(this.textBoxMoveMessage);
            this.moveScuTab.Controls.Add(this.buttonMoveScuMove);
            this.moveScuTab.Location = new System.Drawing.Point(4, 22);
            this.moveScuTab.Name = "moveScuTab";
            this.moveScuTab.Padding = new System.Windows.Forms.Padding(3);
            this.moveScuTab.Size = new System.Drawing.Size(910, 126);
            this.moveScuTab.TabIndex = 5;
            this.moveScuTab.Text = "MoveSCU";
            this.moveScuTab.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(148, 70);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(33, 13);
            this.label23.TabIndex = 41;
            this.label23.Text = "Level";
            // 
            // comboBoxMoveScuQueryLevel
            // 
            this.comboBoxMoveScuQueryLevel.FormattingEnabled = true;
            this.comboBoxMoveScuQueryLevel.Items.AddRange(new object[] {
            "PATIENT",
            "STUDY",
            "SERIES",
            "IMAGE"});
            this.comboBoxMoveScuQueryLevel.Location = new System.Drawing.Point(148, 87);
            this.comboBoxMoveScuQueryLevel.Name = "comboBoxMoveScuQueryLevel";
            this.comboBoxMoveScuQueryLevel.Size = new System.Drawing.Size(100, 21);
            this.comboBoxMoveScuQueryLevel.TabIndex = 40;
            this.comboBoxMoveScuQueryLevel.Text = "STUDY";
            this.comboBoxMoveScuQueryLevel.SelectedIndexChanged += new System.EventHandler(this.ComboBoxMoveScuQueryLevelSelectedIndexChanged);
            // 
            // textBoxMoveScuMoveDestination
            // 
            this.textBoxMoveScuMoveDestination.Location = new System.Drawing.Point(282, 87);
            this.textBoxMoveScuMoveDestination.Name = "textBoxMoveScuMoveDestination";
            this.textBoxMoveScuMoveDestination.Size = new System.Drawing.Size(100, 20);
            this.textBoxMoveScuMoveDestination.TabIndex = 39;
            this.textBoxMoveScuMoveDestination.Text = "StorageSCP";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(279, 71);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(90, 13);
            this.label24.TabIndex = 38;
            this.label24.Text = "Move Destination";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(17, 71);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(62, 13);
            this.label25.TabIndex = 37;
            this.label25.Text = "Query Type";
            // 
            // comboBoxMoveScuQueryType
            // 
            this.comboBoxMoveScuQueryType.FormattingEnabled = true;
            this.comboBoxMoveScuQueryType.Items.AddRange(new object[] {
            "Study Root",
            "Patient Root"});
            this.comboBoxMoveScuQueryType.Location = new System.Drawing.Point(17, 87);
            this.comboBoxMoveScuQueryType.Name = "comboBoxMoveScuQueryType";
            this.comboBoxMoveScuQueryType.Size = new System.Drawing.Size(121, 21);
            this.comboBoxMoveScuQueryType.TabIndex = 36;
            this.comboBoxMoveScuQueryType.Text = "Study Root";
            this.comboBoxMoveScuQueryType.SelectedIndexChanged += new System.EventHandler(this.ComboBoxMoveScuQueryTypeSelectedIndexChanged);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(17, 14);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(50, 13);
            this.label26.TabIndex = 35;
            this.label26.Text = "Local AE";
            // 
            // textBoxMoveScuLocalAe
            // 
            this.textBoxMoveScuLocalAe.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScuLocalAETitle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxMoveScuLocalAe.Location = new System.Drawing.Point(17, 31);
            this.textBoxMoveScuLocalAe.Name = "textBoxMoveScuLocalAe";
            this.textBoxMoveScuLocalAe.Size = new System.Drawing.Size(100, 20);
            this.textBoxMoveScuLocalAe.TabIndex = 34;
            this.textBoxMoveScuLocalAe.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScuLocalAETitle;
            // 
            // textBoxMoveScuRemotePort
            // 
            this.textBoxMoveScuRemotePort.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScuRemotePort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxMoveScuRemotePort.Location = new System.Drawing.Point(421, 32);
            this.textBoxMoveScuRemotePort.Name = "textBoxMoveScuRemotePort";
            this.textBoxMoveScuRemotePort.Size = new System.Drawing.Size(100, 20);
            this.textBoxMoveScuRemotePort.TabIndex = 33;
            this.textBoxMoveScuRemotePort.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScuRemotePort;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(418, 15);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(66, 13);
            this.label27.TabIndex = 32;
            this.label27.Text = "Remote Port";
            // 
            // textBoxMoveScuRemoteHost
            // 
            this.textBoxMoveScuRemoteHost.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScuRemoteHost", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxMoveScuRemoteHost.Location = new System.Drawing.Point(282, 32);
            this.textBoxMoveScuRemoteHost.Name = "textBoxMoveScuRemoteHost";
            this.textBoxMoveScuRemoteHost.Size = new System.Drawing.Size(100, 20);
            this.textBoxMoveScuRemoteHost.TabIndex = 31;
            this.textBoxMoveScuRemoteHost.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScuRemoteHost;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(279, 15);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(69, 13);
            this.label28.TabIndex = 30;
            this.label28.Text = "Remote Host";
            // 
            // textBoxMoveScuRemoteAe
            // 
            this.textBoxMoveScuRemoteAe.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ClearCanvas.Dicom.Samples.Properties.Settings.Default, "ScuRemoteAETitle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxMoveScuRemoteAe.Location = new System.Drawing.Point(148, 32);
            this.textBoxMoveScuRemoteAe.Name = "textBoxMoveScuRemoteAe";
            this.textBoxMoveScuRemoteAe.Size = new System.Drawing.Size(100, 20);
            this.textBoxMoveScuRemoteAe.TabIndex = 29;
            this.textBoxMoveScuRemoteAe.Text = global::ClearCanvas.Dicom.Samples.Properties.Settings.Default.ScuRemoteAETitle;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(145, 15);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(61, 13);
            this.label29.TabIndex = 28;
            this.label29.Text = "Remote AE";
            // 
            // textBoxMoveMessage
            // 
            this.textBoxMoveMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMoveMessage.Location = new System.Drawing.Point(550, 6);
            this.textBoxMoveMessage.Multiline = true;
            this.textBoxMoveMessage.Name = "textBoxMoveMessage";
            this.textBoxMoveMessage.Size = new System.Drawing.Size(348, 114);
            this.textBoxMoveMessage.TabIndex = 27;
            // 
            // buttonMoveScuMove
            // 
            this.buttonMoveScuMove.Location = new System.Drawing.Point(446, 87);
            this.buttonMoveScuMove.Name = "buttonMoveScuMove";
            this.buttonMoveScuMove.Size = new System.Drawing.Size(75, 23);
            this.buttonMoveScuMove.TabIndex = 26;
            this.buttonMoveScuMove.Text = "Move";
            this.buttonMoveScuMove.UseVisualStyleBackColor = true;
            this.buttonMoveScuMove.Click += new System.EventHandler(this.buttonMoveScuMove_Click);
            // 
            // _buttonOutputClearLog
            // 
            this._buttonOutputClearLog.Location = new System.Drawing.Point(16, 3);
            this._buttonOutputClearLog.Name = "_buttonOutputClearLog";
            this._buttonOutputClearLog.Size = new System.Drawing.Size(75, 23);
            this._buttonOutputClearLog.TabIndex = 1;
            this._buttonOutputClearLog.Text = "Clear Log";
            this._buttonOutputClearLog.UseVisualStyleBackColor = true;
            this._buttonOutputClearLog.Click += new System.EventHandler(this._buttonOutputClearLog_Click);
            // 
            // OutputTextBox
            // 
            this.OutputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputTextBox.Location = new System.Drawing.Point(3, 30);
            this.OutputTextBox.MaxLength = 65536;
            this.OutputTextBox.Multiline = true;
            this.OutputTextBox.Name = "OutputTextBox";
            this.OutputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.OutputTextBox.Size = new System.Drawing.Size(921, 401);
            this.OutputTextBox.TabIndex = 0;
            this.OutputTextBox.WordWrap = false;
            // 
            // openFileDialogStorageScu
            // 
            this.openFileDialogStorageScu.FileName = "openFileDialogStorageScu";
            this.openFileDialogStorageScu.Filter = "DICOM files|*.dcm|All files|*.*";
            this.openFileDialogStorageScu.Title = "Open DICOM File";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "dcm";
            this.saveFileDialog.Title = "Save DICOM File";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this._editSopTextBox);
            this.tabPage2.Controls.Add(this.label30);
            this.tabPage2.Controls.Add(this.label31);
            this.tabPage2.Controls.Add(this._editSopDestinationPathTextBox);
            this.tabPage2.Controls.Add(this._editSopSourcePathTextBox);
            this.tabPage2.Controls.Add(this._editSopSaveFileButton);
            this.tabPage2.Controls.Add(this._editSopOpenFileButton);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(910, 126);
            this.tabPage2.TabIndex = 6;
            this.tabPage2.Text = "Edit SOP";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(6, 8);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(85, 13);
            this.label30.TabIndex = 13;
            this.label30.Text = "Source File Path";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(6, 53);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(104, 13);
            this.label31.TabIndex = 12;
            this.label31.Text = "Destination File Path";
            // 
            // _editSopDestinationPathTextBox
            // 
            this._editSopDestinationPathTextBox.Location = new System.Drawing.Point(9, 69);
            this._editSopDestinationPathTextBox.Name = "_editSopDestinationPathTextBox";
            this._editSopDestinationPathTextBox.ReadOnly = true;
            this._editSopDestinationPathTextBox.Size = new System.Drawing.Size(358, 20);
            this._editSopDestinationPathTextBox.TabIndex = 11;
            // 
            // _editSopSourcePathTextBox
            // 
            this._editSopSourcePathTextBox.Location = new System.Drawing.Point(9, 24);
            this._editSopSourcePathTextBox.Name = "_editSopSourcePathTextBox";
            this._editSopSourcePathTextBox.ReadOnly = true;
            this._editSopSourcePathTextBox.Size = new System.Drawing.Size(358, 20);
            this._editSopSourcePathTextBox.TabIndex = 10;
            // 
            // _editSopSaveFileButton
            // 
            this._editSopSaveFileButton.Location = new System.Drawing.Point(373, 67);
            this._editSopSaveFileButton.Name = "_editSopSaveFileButton";
            this._editSopSaveFileButton.Size = new System.Drawing.Size(75, 23);
            this._editSopSaveFileButton.TabIndex = 9;
            this._editSopSaveFileButton.Text = "Save File";
            this._editSopSaveFileButton.UseVisualStyleBackColor = true;
            this._editSopSaveFileButton.Click += new System.EventHandler(this._editSopSaveFileButton_Click);
            // 
            // _editSopOpenFileButton
            // 
            this._editSopOpenFileButton.Location = new System.Drawing.Point(373, 22);
            this._editSopOpenFileButton.Name = "_editSopOpenFileButton";
            this._editSopOpenFileButton.Size = new System.Drawing.Size(75, 23);
            this._editSopOpenFileButton.TabIndex = 8;
            this._editSopOpenFileButton.Text = "Open File";
            this._editSopOpenFileButton.UseVisualStyleBackColor = true;
            this._editSopOpenFileButton.Click += new System.EventHandler(this._editSopOpenFileButton_Click);
            // 
            // _editSopTextBox
            // 
            this._editSopTextBox.Dock = System.Windows.Forms.DockStyle.Right;
            this._editSopTextBox.Location = new System.Drawing.Point(457, 3);
            this._editSopTextBox.Multiline = true;
            this._editSopTextBox.Name = "_editSopTextBox";
            this._editSopTextBox.Size = new System.Drawing.Size(450, 120);
            this._editSopTextBox.TabIndex = 14;
            // 
            // SamplesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 590);
            this.Controls.Add(this.SamplesSplitContainer);
            this.Name = "SamplesForm";
            this.Text = "ClearCanvas.Dicom.Samples";
            this.SamplesSplitContainer.Panel1.ResumeLayout(false);
            this.SamplesSplitContainer.Panel2.ResumeLayout(false);
            this.SamplesSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SamplesSplitContainer)).EndInit();
            this.SamplesSplitContainer.ResumeLayout(false);
            this.SamplesTabs.ResumeLayout(false);
            this.StorageScuTab.ResumeLayout(false);
            this.StorageScuTab.PerformLayout();
            this.StorageScpTab.ResumeLayout(false);
            this.StorageScpTab.PerformLayout();
            this.CompressionTab.ResumeLayout(false);
            this.CompressionTab.PerformLayout();
            this.queryScuTab.ResumeLayout(false);
            this.queryScuTab.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.moveScuTab.ResumeLayout(false);
            this.moveScuTab.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer SamplesSplitContainer;
        private System.Windows.Forms.TabControl SamplesTabs;
        private System.Windows.Forms.TabPage StorageScuTab;
        private System.Windows.Forms.TabPage StorageScpTab;
        private System.Windows.Forms.TextBox OutputTextBox;
        private System.Windows.Forms.Button _buttonStorageScuConnect;
        private System.Windows.Forms.Button buttonStorageScuSelectFiles;
        private System.Windows.Forms.TextBox _textBoxStorageScuRemotePort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _textBoxStorageScuRemoteHost;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _textBoxStorageScuRemoteAe;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _buttonStorageScuSelectDirectory;
        private System.Windows.Forms.OpenFileDialog openFileDialogStorageScu;
        private System.Windows.Forms.Button _buttonStorageScpStartStop;
        private System.Windows.Forms.TextBox _textBoxStorageScpPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _textBoxStorageScpStorageLocation;
        private System.Windows.Forms.Button _buttonStorageScuSelectStorageLocation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogStorageScp;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogStorageScu;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox _textBoxStorageScpAeTitle;
        private System.Windows.Forms.Button _buttonStorageScuVerify;
        private System.Windows.Forms.Button _buttonOutputClearLog;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox _textBoxStorageScuLocalAe;
        private System.Windows.Forms.Button _buttonStorageScuClearFiles;
		private System.Windows.Forms.TabPage CompressionTab;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox _sourceTransferSyntaxCombo;
		private System.Windows.Forms.ComboBox _destinationSyntaxCombo;
		private System.Windows.Forms.TextBox _destinationPathTextBox;
		private System.Windows.Forms.TextBox _sourcePathTextBox;
		private System.Windows.Forms.Button _saveFileButton;
		private System.Windows.Forms.Button _openFileButton;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.Button _savePixelsButton;
		private System.Windows.Forms.TabPage queryScuTab;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textBoxQueryScuLocalAe;
		private System.Windows.Forms.TextBox textBoxQueryScuRemotePort;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textBoxQueryScuRemoteHost;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textBoxQueryScuRemoteAe;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox textBoxQueryMessage;
		private System.Windows.Forms.Button buttonQueryScuSearch;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.ComboBox comboBoxQueryScuQueryType;
		private System.Windows.Forms.TextBox textBoxQueryScuMaxResults;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.ComboBox comboBoxQueryScuQueryLevel;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.TextBox _textBoxDicomdir;
		private System.Windows.Forms.Button _buttonOpenDicomdir;
		private System.Windows.Forms.Button buttonSendDicomdir;
		private System.Windows.Forms.TextBox _textBoxDicomdirRemotePort;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TextBox _textBoxDicomdirRemoteHost;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.TextBox _textBoxDicomdirRemoteAe;
		private System.Windows.Forms.Label label22;
        private System.Windows.Forms.CheckBox _checkBoxStorageScpBitbucket;
        private System.Windows.Forms.TabPage moveScuTab;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.ComboBox comboBoxMoveScuQueryLevel;
        private System.Windows.Forms.TextBox textBoxMoveScuMoveDestination;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.ComboBox comboBoxMoveScuQueryType;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox textBoxMoveScuLocalAe;
        private System.Windows.Forms.TextBox textBoxMoveScuRemotePort;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox textBoxMoveScuRemoteHost;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox textBoxMoveScuRemoteAe;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox textBoxMoveMessage;
        private System.Windows.Forms.Button buttonMoveScuMove;
        private System.Windows.Forms.CheckBox _checkBoxStorageScpRLE;
        private System.Windows.Forms.CheckBox _checkBoxStorageScpJ2KLossy;
        private System.Windows.Forms.CheckBox _checkBoxStorageScpJ2KLossless;
        private System.Windows.Forms.CheckBox _checkBoxStorageScpJpegLossy;
        private System.Windows.Forms.CheckBox _checkBoxStorageScpJpegLossless;
        private System.Windows.Forms.CheckBox _checkBoxStorageScpList;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.TextBox _editSopDestinationPathTextBox;
        private System.Windows.Forms.TextBox _editSopSourcePathTextBox;
        private System.Windows.Forms.Button _editSopSaveFileButton;
        private System.Windows.Forms.Button _editSopOpenFileButton;
        private System.Windows.Forms.TextBox _editSopTextBox;
    }
}

