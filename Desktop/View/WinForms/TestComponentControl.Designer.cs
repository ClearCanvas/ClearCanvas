#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.Desktop.View.WinForms
{
    partial class TestComponentControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this._label = new System.Windows.Forms.Label();
			this._text = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this._showMessageBox = new System.Windows.Forms.Button();
			this._showDialogBox = new System.Windows.Forms.Button();
			this._close = new System.Windows.Forms.Button();
			this._setTitle = new System.Windows.Forms.Button();
			this._modify = new System.Windows.Forms.Button();
			this._accept = new System.Windows.Forms.Button();
			this._showWorkspaceDialog = new System.Windows.Forms.Button();
			this._buttonCrashThread = new System.Windows.Forms.Button();
			this._buttonCrashUI = new System.Windows.Forms.Button();
			this._crashThreadPool = new System.Windows.Forms.Button();
			this._delayCrash = new System.Windows.Forms.CheckBox();
			this._crashDelay = new System.Windows.Forms.NumericUpDown();
			this._catchAndReport = new System.Windows.Forms.CheckBox();
			this._circumventCrash = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._errorAlertButton = new System.Windows.Forms.Button();
			this._infoAlertButton = new System.Windows.Forms.Button();
			this._warningAlertButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this._crashDelay)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _label
			// 
			this._label.AutoSize = true;
			this._label.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._label.Location = new System.Drawing.Point(40, 104);
			this._label.Name = "_label";
			this._label.Size = new System.Drawing.Size(58, 26);
			this._label.TabIndex = 0;
			this._label.Text = "label";
			// 
			// _text
			// 
			this._text.Location = new System.Drawing.Point(45, 150);
			this._text.Name = "_text";
			this._text.Size = new System.Drawing.Size(199, 20);
			this._text.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(42, 130);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(28, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Text";
			// 
			// _showMessageBox
			// 
			this._showMessageBox.Location = new System.Drawing.Point(45, 260);
			this._showMessageBox.Name = "_showMessageBox";
			this._showMessageBox.Size = new System.Drawing.Size(132, 23);
			this._showMessageBox.TabIndex = 4;
			this._showMessageBox.Text = "Message Box";
			this._showMessageBox.UseVisualStyleBackColor = true;
			this._showMessageBox.Click += new System.EventHandler(this._showMessageBox_Click);
			// 
			// _showDialogBox
			// 
			this._showDialogBox.Location = new System.Drawing.Point(183, 260);
			this._showDialogBox.Name = "_showDialogBox";
			this._showDialogBox.Size = new System.Drawing.Size(132, 23);
			this._showDialogBox.TabIndex = 5;
			this._showDialogBox.Text = "Dialog Box";
			this._showDialogBox.UseVisualStyleBackColor = true;
			this._showDialogBox.Click += new System.EventHandler(this._showDialogBox_Click);
			// 
			// _close
			// 
			this._close.Location = new System.Drawing.Point(379, 306);
			this._close.Name = "_close";
			this._close.Size = new System.Drawing.Size(74, 23);
			this._close.TabIndex = 7;
			this._close.Text = "Close";
			this._close.UseVisualStyleBackColor = true;
			this._close.Click += new System.EventHandler(this._close_Click);
			// 
			// _setTitle
			// 
			this._setTitle.Location = new System.Drawing.Point(253, 149);
			this._setTitle.Name = "_setTitle";
			this._setTitle.Size = new System.Drawing.Size(62, 23);
			this._setTitle.TabIndex = 2;
			this._setTitle.Text = "Set Title";
			this._setTitle.UseVisualStyleBackColor = true;
			this._setTitle.Click += new System.EventHandler(this._setTitle_Click);
			// 
			// _modify
			// 
			this._modify.Location = new System.Drawing.Point(45, 206);
			this._modify.Name = "_modify";
			this._modify.Size = new System.Drawing.Size(74, 23);
			this._modify.TabIndex = 3;
			this._modify.Text = "Modify";
			this._modify.UseVisualStyleBackColor = true;
			this._modify.Click += new System.EventHandler(this._modify_Click);
			// 
			// _accept
			// 
			this._accept.Location = new System.Drawing.Point(299, 306);
			this._accept.Name = "_accept";
			this._accept.Size = new System.Drawing.Size(74, 23);
			this._accept.TabIndex = 6;
			this._accept.Text = "OK";
			this._accept.UseVisualStyleBackColor = true;
			this._accept.Click += new System.EventHandler(this._accept_Click);
			// 
			// _showWorkspaceDialog
			// 
			this._showWorkspaceDialog.Location = new System.Drawing.Point(321, 260);
			this._showWorkspaceDialog.Name = "_showWorkspaceDialog";
			this._showWorkspaceDialog.Size = new System.Drawing.Size(132, 23);
			this._showWorkspaceDialog.TabIndex = 9;
			this._showWorkspaceDialog.Text = "Workspace Dialog";
			this._showWorkspaceDialog.UseVisualStyleBackColor = true;
			this._showWorkspaceDialog.Click += new System.EventHandler(this._showWorkspaceDialogBox_Click);
			// 
			// _buttonCrashThread
			// 
			this._buttonCrashThread.Location = new System.Drawing.Point(351, 44);
			this._buttonCrashThread.Name = "_buttonCrashThread";
			this._buttonCrashThread.Size = new System.Drawing.Size(102, 23);
			this._buttonCrashThread.TabIndex = 10;
			this._buttonCrashThread.Text = "Crash Thread";
			this._buttonCrashThread.UseVisualStyleBackColor = true;
			this._buttonCrashThread.Click += new System.EventHandler(this._buttonCrashThread_Click);
			// 
			// _buttonCrashUI
			// 
			this._buttonCrashUI.Location = new System.Drawing.Point(351, 15);
			this._buttonCrashUI.Name = "_buttonCrashUI";
			this._buttonCrashUI.Size = new System.Drawing.Size(102, 23);
			this._buttonCrashUI.TabIndex = 11;
			this._buttonCrashUI.Text = "Crash UI";
			this._buttonCrashUI.UseVisualStyleBackColor = true;
			this._buttonCrashUI.Click += new System.EventHandler(this._buttonCrashUI_Click);
			// 
			// _crashThreadPool
			// 
			this._crashThreadPool.Location = new System.Drawing.Point(351, 73);
			this._crashThreadPool.Name = "_crashThreadPool";
			this._crashThreadPool.Size = new System.Drawing.Size(102, 23);
			this._crashThreadPool.TabIndex = 12;
			this._crashThreadPool.Text = "Crash ThreadPool";
			this._crashThreadPool.UseVisualStyleBackColor = true;
			this._crashThreadPool.Click += new System.EventHandler(this._crashThreadPool_Click);
			// 
			// _delayCrash
			// 
			this._delayCrash.AutoSize = true;
			this._delayCrash.Location = new System.Drawing.Point(153, 21);
			this._delayCrash.Name = "_delayCrash";
			this._delayCrash.Size = new System.Drawing.Size(83, 17);
			this._delayCrash.TabIndex = 13;
			this._delayCrash.Text = "Delay Crash";
			this._delayCrash.UseVisualStyleBackColor = true;
			// 
			// _crashDelay
			// 
			this._crashDelay.Location = new System.Drawing.Point(242, 21);
			this._crashDelay.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
			this._crashDelay.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this._crashDelay.Name = "_crashDelay";
			this._crashDelay.Size = new System.Drawing.Size(83, 20);
			this._crashDelay.TabIndex = 14;
			this._crashDelay.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// _catchAndReport
			// 
			this._catchAndReport.AutoSize = true;
			this._catchAndReport.Location = new System.Drawing.Point(153, 67);
			this._catchAndReport.Name = "_catchAndReport";
			this._catchAndReport.Size = new System.Drawing.Size(105, 17);
			this._catchAndReport.TabIndex = 15;
			this._catchAndReport.Text = "Catch and report";
			this._catchAndReport.UseVisualStyleBackColor = true;
			// 
			// _circumventCrash
			// 
			this._circumventCrash.AutoSize = true;
			this._circumventCrash.Location = new System.Drawing.Point(153, 44);
			this._circumventCrash.Name = "_circumventCrash";
			this._circumventCrash.Size = new System.Drawing.Size(79, 17);
			this._circumventCrash.TabIndex = 16;
			this._circumventCrash.Text = "Circumvent";
			this._circumventCrash.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._errorAlertButton);
			this.groupBox1.Controls.Add(this._infoAlertButton);
			this.groupBox1.Controls.Add(this._warningAlertButton);
			this.groupBox1.Location = new System.Drawing.Point(351, 115);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(119, 114);
			this.groupBox1.TabIndex = 17;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Alerts";
			// 
			// _errorAlertButton
			// 
			this._errorAlertButton.Location = new System.Drawing.Point(8, 75);
			this._errorAlertButton.Name = "_errorAlertButton";
			this._errorAlertButton.Size = new System.Drawing.Size(102, 23);
			this._errorAlertButton.TabIndex = 15;
			this._errorAlertButton.Text = "Error";
			this._errorAlertButton.UseVisualStyleBackColor = true;
			this._errorAlertButton.Click += new System.EventHandler(this._errorAlertButton_Click);
			// 
			// _infoAlertButton
			// 
			this._infoAlertButton.Location = new System.Drawing.Point(8, 17);
			this._infoAlertButton.Name = "_infoAlertButton";
			this._infoAlertButton.Size = new System.Drawing.Size(102, 23);
			this._infoAlertButton.TabIndex = 14;
			this._infoAlertButton.Text = "Info";
			this._infoAlertButton.UseVisualStyleBackColor = true;
			this._infoAlertButton.Click += new System.EventHandler(this._infoAlertButton_Click);
			// 
			// _warningAlertButton
			// 
			this._warningAlertButton.Location = new System.Drawing.Point(8, 46);
			this._warningAlertButton.Name = "_warningAlertButton";
			this._warningAlertButton.Size = new System.Drawing.Size(102, 23);
			this._warningAlertButton.TabIndex = 13;
			this._warningAlertButton.Text = "Warning";
			this._warningAlertButton.UseVisualStyleBackColor = true;
			this._warningAlertButton.Click += new System.EventHandler(this._warningAlertButton_Click);
			// 
			// TestComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this._circumventCrash);
			this.Controls.Add(this._catchAndReport);
			this.Controls.Add(this._crashDelay);
			this.Controls.Add(this._delayCrash);
			this.Controls.Add(this._crashThreadPool);
			this.Controls.Add(this._buttonCrashUI);
			this.Controls.Add(this._buttonCrashThread);
			this.Controls.Add(this._showWorkspaceDialog);
			this.Controls.Add(this._accept);
			this.Controls.Add(this._modify);
			this.Controls.Add(this._setTitle);
			this.Controls.Add(this._close);
			this.Controls.Add(this._showDialogBox);
			this.Controls.Add(this._showMessageBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._text);
			this.Controls.Add(this._label);
			this.Name = "TestComponentControl";
			this.Size = new System.Drawing.Size(483, 353);
			((System.ComponentModel.ISupportInitialize)(this._crashDelay)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _label;
        private System.Windows.Forms.TextBox _text;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _showMessageBox;
        private System.Windows.Forms.Button _showDialogBox;
        private System.Windows.Forms.Button _close;
        private System.Windows.Forms.Button _setTitle;
        private System.Windows.Forms.Button _modify;
        private System.Windows.Forms.Button _accept;
		private System.Windows.Forms.Button _showWorkspaceDialog;
		private System.Windows.Forms.Button _buttonCrashThread;
		private System.Windows.Forms.Button _buttonCrashUI;
		private System.Windows.Forms.Button _crashThreadPool;
		private System.Windows.Forms.CheckBox _delayCrash;
		private System.Windows.Forms.NumericUpDown _crashDelay;
		private System.Windows.Forms.CheckBox _catchAndReport;
		private System.Windows.Forms.CheckBox _circumventCrash;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button _errorAlertButton;
		private System.Windows.Forms.Button _infoAlertButton;
		private System.Windows.Forms.Button _warningAlertButton;

    }
}
