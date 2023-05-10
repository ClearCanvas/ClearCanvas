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

namespace ClearCanvas.Samples.Google.Calendar.View.WinForms
{
    partial class SchedulingComponentControl
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
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            this._followUpDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._comment = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._addButton = new System.Windows.Forms.Button();
            this._patientInfo = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._appointmentsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _followUpDate
            // 
            this._followUpDate.LabelText = "Follow up date";
            this._followUpDate.Location = new System.Drawing.Point(6, 142);
            this._followUpDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._followUpDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._followUpDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._followUpDate.Name = "_followUpDate";
            this._followUpDate.Nullable = false;
            this._followUpDate.ShowTime = false;
            this._followUpDate.Size = new System.Drawing.Size(200, 50);
            this._followUpDate.TabIndex = 1;
            this._followUpDate.Value = null;
            // 
            // _comment
            // 
            this._comment.LabelText = "Comment";
            this._comment.Location = new System.Drawing.Point(6, 48);
            this._comment.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._comment.Name = "_comment";
            this._comment.Size = new System.Drawing.Size(290, 76);
            this._comment.TabIndex = 2;
            this._comment.Value = null;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._addButton);
            this.groupBox1.Controls.Add(this._comment);
            this.groupBox1.Controls.Add(this._followUpDate);
            this.groupBox1.Location = new System.Drawing.Point(3, 383);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(312, 218);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Follow-up";
            // 
            // _addButton
            // 
            this._addButton.Location = new System.Drawing.Point(221, 169);
            this._addButton.Name = "_addButton";
            this._addButton.Size = new System.Drawing.Size(75, 23);
            this._addButton.TabIndex = 3;
            this._addButton.Text = "Add";
            this._addButton.UseVisualStyleBackColor = true;
            this._addButton.Click += new System.EventHandler(this._addButton_Click);
            // 
            // _patientInfo
            // 
            this._patientInfo.LabelText = "Patient";
            this._patientInfo.Location = new System.Drawing.Point(9, 15);
            this._patientInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._patientInfo.Mask = "";
            this._patientInfo.Name = "_patientInfo";
            this._patientInfo.ReadOnly = true;
            this._patientInfo.Size = new System.Drawing.Size(306, 50);
            this._patientInfo.TabIndex = 4;
            this._patientInfo.Value = null;
            // 
            // _appointmentsTableView
            // 
            this._appointmentsTableView.Location = new System.Drawing.Point(9, 82);
            this._appointmentsTableView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._appointmentsTableView.MenuModel = null;
            this._appointmentsTableView.Name = "_appointmentsTableView";
            this._appointmentsTableView.ReadOnly = false;
            this._appointmentsTableView.Selection = selection1;
            this._appointmentsTableView.Size = new System.Drawing.Size(306, 281);
            this._appointmentsTableView.TabIndex = 5;
            this._appointmentsTableView.Table = null;
            this._appointmentsTableView.ToolbarModel = null;
            this._appointmentsTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._appointmentsTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // SchedulingComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._appointmentsTableView);
            this.Controls.Add(this._patientInfo);
            this.Controls.Add(this.groupBox1);
            this.Name = "SchedulingComponentControl";
            this.Size = new System.Drawing.Size(325, 604);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.DateTimeField _followUpDate;
        private ClearCanvas.Desktop.View.WinForms.TextAreaField _comment;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button _addButton;
        private ClearCanvas.Desktop.View.WinForms.TextField _patientInfo;
        private ClearCanvas.Desktop.View.WinForms.TableView _appointmentsTableView;
    }
}
