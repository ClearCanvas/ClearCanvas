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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class MultipleProceduresEditorComponentControl
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._enablePerformingFacility = new System.Windows.Forms.CheckBox();
			this._scheduledTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._enablePerformingDepartment = new System.Windows.Forms.CheckBox();
			this._enableLaterality = new System.Windows.Forms.CheckBox();
			this._enablePortable = new System.Windows.Forms.CheckBox();
			this._enableCheckIn = new System.Windows.Forms.CheckBox();
			this._performingFacility = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._performingDepartment = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._laterality = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._portable = new System.Windows.Forms.CheckBox();
			this._enableScheduledDateTime = new System.Windows.Forms.CheckBox();
			this._schedulingCode = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._checkedIn = new System.Windows.Forms.CheckBox();
			this._enableSchedulingCode = new System.Windows.Forms.CheckBox();
			this._scheduledDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._acceptButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._duration = new System.Windows.Forms.NumericUpDown();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this._modality = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._enableScheduledDuration = new System.Windows.Forms.CheckBox();
			this._enableModality = new System.Windows.Forms.CheckBox();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._duration)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this._enableScheduledDuration, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._enablePerformingFacility, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this._scheduledTime, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this._enablePerformingDepartment, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this._enableLaterality, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this._enablePortable, 0, 7);
			this.tableLayoutPanel1.Controls.Add(this._enableCheckIn, 0, 8);
			this.tableLayoutPanel1.Controls.Add(this._performingFacility, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this._performingDepartment, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this._laterality, 1, 5);
			this.tableLayoutPanel1.Controls.Add(this._portable, 1, 7);
			this.tableLayoutPanel1.Controls.Add(this._enableScheduledDateTime, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._schedulingCode, 1, 6);
			this.tableLayoutPanel1.Controls.Add(this._checkedIn, 1, 8);
			this.tableLayoutPanel1.Controls.Add(this._enableSchedulingCode, 0, 6);
			this.tableLayoutPanel1.Controls.Add(this._scheduledDate, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this._modality, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this._enableModality, 0, 2);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 4);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
			this.tableLayoutPanel1.RowCount = 9;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(494, 370);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// _enablePerformingFacility
			// 
			this._enablePerformingFacility.AutoSize = true;
			this._enablePerformingFacility.Dock = System.Windows.Forms.DockStyle.Fill;
			this._enablePerformingFacility.Location = new System.Drawing.Point(3, 142);
			this._enablePerformingFacility.Name = "_enablePerformingFacility";
			this._enablePerformingFacility.Size = new System.Drawing.Size(19, 39);
			this._enablePerformingFacility.TabIndex = 3;
			this._enablePerformingFacility.UseVisualStyleBackColor = true;
			// 
			// _scheduledTime
			// 
			this._scheduledTime.Dock = System.Windows.Forms.DockStyle.Fill;
			this._scheduledTime.LabelText = "Scheduled Time";
			this._scheduledTime.Location = new System.Drawing.Point(254, 2);
			this._scheduledTime.Margin = new System.Windows.Forms.Padding(2);
			this._scheduledTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._scheduledTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._scheduledTime.Name = "_scheduledTime";
			this._scheduledTime.Nullable = true;
			this._scheduledTime.ShowDate = false;
			this._scheduledTime.ShowTime = true;
			this._scheduledTime.Size = new System.Drawing.Size(223, 41);
			this._scheduledTime.TabIndex = 2;
			this._scheduledTime.Value = null;
			// 
			// _enablePerformingDepartment
			// 
			this._enablePerformingDepartment.AutoSize = true;
			this._enablePerformingDepartment.Dock = System.Windows.Forms.DockStyle.Fill;
			this._enablePerformingDepartment.Location = new System.Drawing.Point(3, 187);
			this._enablePerformingDepartment.Name = "_enablePerformingDepartment";
			this._enablePerformingDepartment.Size = new System.Drawing.Size(19, 39);
			this._enablePerformingDepartment.TabIndex = 5;
			this._enablePerformingDepartment.UseVisualStyleBackColor = true;
			// 
			// _enableLaterality
			// 
			this._enableLaterality.AutoSize = true;
			this._enableLaterality.Dock = System.Windows.Forms.DockStyle.Fill;
			this._enableLaterality.Location = new System.Drawing.Point(3, 232);
			this._enableLaterality.Name = "_enableLaterality";
			this._enableLaterality.Size = new System.Drawing.Size(19, 39);
			this._enableLaterality.TabIndex = 7;
			this._enableLaterality.UseVisualStyleBackColor = true;
			// 
			// _enablePortable
			// 
			this._enablePortable.AutoSize = true;
			this._enablePortable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._enablePortable.Location = new System.Drawing.Point(3, 322);
			this._enablePortable.Name = "_enablePortable";
			this._enablePortable.Size = new System.Drawing.Size(19, 17);
			this._enablePortable.TabIndex = 11;
			this._enablePortable.UseVisualStyleBackColor = true;
			// 
			// _enableCheckIn
			// 
			this._enableCheckIn.AutoSize = true;
			this._enableCheckIn.Dock = System.Windows.Forms.DockStyle.Fill;
			this._enableCheckIn.Location = new System.Drawing.Point(3, 345);
			this._enableCheckIn.Name = "_enableCheckIn";
			this._enableCheckIn.Size = new System.Drawing.Size(19, 22);
			this._enableCheckIn.TabIndex = 13;
			this._enableCheckIn.UseVisualStyleBackColor = true;
			// 
			// _performingFacility
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._performingFacility, 2);
			this._performingFacility.DataSource = null;
			this._performingFacility.DisplayMember = "";
			this._performingFacility.Dock = System.Windows.Forms.DockStyle.Fill;
			this._performingFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._performingFacility.LabelText = "Performing Facility";
			this._performingFacility.Location = new System.Drawing.Point(27, 141);
			this._performingFacility.Margin = new System.Windows.Forms.Padding(2);
			this._performingFacility.Name = "_performingFacility";
			this._performingFacility.Size = new System.Drawing.Size(450, 41);
			this._performingFacility.TabIndex = 4;
			this._performingFacility.Value = null;
			// 
			// _performingDepartment
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._performingDepartment, 2);
			this._performingDepartment.DataSource = null;
			this._performingDepartment.DisplayMember = "";
			this._performingDepartment.Dock = System.Windows.Forms.DockStyle.Fill;
			this._performingDepartment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._performingDepartment.LabelText = "Performing Department";
			this._performingDepartment.Location = new System.Drawing.Point(27, 186);
			this._performingDepartment.Margin = new System.Windows.Forms.Padding(2);
			this._performingDepartment.Name = "_performingDepartment";
			this._performingDepartment.Size = new System.Drawing.Size(450, 41);
			this._performingDepartment.TabIndex = 6;
			this._performingDepartment.Value = null;
			// 
			// _laterality
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._laterality, 2);
			this._laterality.DataSource = null;
			this._laterality.DisplayMember = "";
			this._laterality.Dock = System.Windows.Forms.DockStyle.Fill;
			this._laterality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._laterality.LabelText = "Laterality";
			this._laterality.Location = new System.Drawing.Point(27, 231);
			this._laterality.Margin = new System.Windows.Forms.Padding(2);
			this._laterality.Name = "_laterality";
			this._laterality.Size = new System.Drawing.Size(450, 41);
			this._laterality.TabIndex = 8;
			this._laterality.Value = null;
			// 
			// _portable
			// 
			this._portable.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this._portable, 2);
			this._portable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._portable.Location = new System.Drawing.Point(31, 322);
			this._portable.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
			this._portable.Name = "_portable";
			this._portable.Size = new System.Drawing.Size(445, 17);
			this._portable.TabIndex = 12;
			this._portable.Text = "Portable";
			this._portable.UseVisualStyleBackColor = true;
			// 
			// _enableScheduledDateTime
			// 
			this._enableScheduledDateTime.AutoSize = true;
			this._enableScheduledDateTime.Dock = System.Windows.Forms.DockStyle.Fill;
			this._enableScheduledDateTime.Location = new System.Drawing.Point(3, 3);
			this._enableScheduledDateTime.Name = "_enableScheduledDateTime";
			this._enableScheduledDateTime.Size = new System.Drawing.Size(19, 39);
			this._enableScheduledDateTime.TabIndex = 0;
			this._enableScheduledDateTime.UseVisualStyleBackColor = true;
			// 
			// _schedulingCode
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._schedulingCode, 2);
			this._schedulingCode.DataSource = null;
			this._schedulingCode.DisplayMember = "";
			this._schedulingCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this._schedulingCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._schedulingCode.LabelText = "Scheduling Code";
			this._schedulingCode.Location = new System.Drawing.Point(27, 276);
			this._schedulingCode.Margin = new System.Windows.Forms.Padding(2);
			this._schedulingCode.Name = "_schedulingCode";
			this._schedulingCode.Size = new System.Drawing.Size(450, 41);
			this._schedulingCode.TabIndex = 10;
			this._schedulingCode.Value = null;
			// 
			// _checkedIn
			// 
			this._checkedIn.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this._checkedIn, 2);
			this._checkedIn.Dock = System.Windows.Forms.DockStyle.Fill;
			this._checkedIn.Location = new System.Drawing.Point(31, 345);
			this._checkedIn.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
			this._checkedIn.Name = "_checkedIn";
			this._checkedIn.Size = new System.Drawing.Size(445, 22);
			this._checkedIn.TabIndex = 14;
			this._checkedIn.Text = "Patient is checked-in";
			this._checkedIn.UseVisualStyleBackColor = true;
			// 
			// _enableSchedulingCode
			// 
			this._enableSchedulingCode.AutoSize = true;
			this._enableSchedulingCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this._enableSchedulingCode.Location = new System.Drawing.Point(3, 277);
			this._enableSchedulingCode.Name = "_enableSchedulingCode";
			this._enableSchedulingCode.Size = new System.Drawing.Size(19, 39);
			this._enableSchedulingCode.TabIndex = 9;
			this._enableSchedulingCode.UseVisualStyleBackColor = true;
			// 
			// _scheduledDate
			// 
			this._scheduledDate.Dock = System.Windows.Forms.DockStyle.Fill;
			this._scheduledDate.LabelText = "Scheduled Date";
			this._scheduledDate.Location = new System.Drawing.Point(27, 2);
			this._scheduledDate.Margin = new System.Windows.Forms.Padding(2);
			this._scheduledDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._scheduledDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._scheduledDate.Name = "_scheduledDate";
			this._scheduledDate.Nullable = true;
			this._scheduledDate.Size = new System.Drawing.Size(223, 41);
			this._scheduledDate.TabIndex = 1;
			this._scheduledDate.Value = null;
			// 
			// _acceptButton
			// 
			this._acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._acceptButton.Location = new System.Drawing.Point(325, 388);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 1;
			this._acceptButton.Text = "OK";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(406, 388);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 2;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _duration
			// 
			this._duration.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this._duration.Location = new System.Drawing.Point(3, 18);
			this._duration.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._duration.Name = "_duration";
			this._duration.Size = new System.Drawing.Size(65, 20);
			this._duration.TabIndex = 15;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this._duration);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Location = new System.Drawing.Point(28, 48);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(221, 43);
			this.panel1.TabIndex = 16;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 2);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(101, 13);
			this.label1.TabIndex = 17;
			this.label1.Text = "Scheduled Duration";
			// 
			// _modality
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._modality, 2);
			this._modality.DataSource = null;
			this._modality.DisplayMember = "";
			this._modality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._modality.LabelText = "Modality";
			this._modality.Location = new System.Drawing.Point(27, 96);
			this._modality.Margin = new System.Windows.Forms.Padding(2);
			this._modality.Name = "_modality";
			this._modality.Size = new System.Drawing.Size(450, 41);
			this._modality.TabIndex = 17;
			this._modality.Value = null;
			// 
			// _enableScheduledDuration
			// 
			this._enableScheduledDuration.AutoSize = true;
			this._enableScheduledDuration.Dock = System.Windows.Forms.DockStyle.Fill;
			this._enableScheduledDuration.Location = new System.Drawing.Point(3, 48);
			this._enableScheduledDuration.Name = "_enableScheduledDuration";
			this._enableScheduledDuration.Size = new System.Drawing.Size(19, 43);
			this._enableScheduledDuration.TabIndex = 18;
			this._enableScheduledDuration.UseVisualStyleBackColor = true;
			// 
			// _enableModality
			// 
			this._enableModality.AutoSize = true;
			this._enableModality.Dock = System.Windows.Forms.DockStyle.Fill;
			this._enableModality.Location = new System.Drawing.Point(3, 97);
			this._enableModality.Name = "_enableModality";
			this._enableModality.Size = new System.Drawing.Size(19, 39);
			this._enableModality.TabIndex = 19;
			this._enableModality.UseVisualStyleBackColor = true;
			// 
			// MultipleProceduresEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "MultipleProceduresEditorComponentControl";
			this.Size = new System.Drawing.Size(501, 414);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._duration)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button _acceptButton;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.CheckBox _enableScheduledDateTime;
		private System.Windows.Forms.CheckBox _enablePerformingDepartment;
		private System.Windows.Forms.CheckBox _enableLaterality;
		private System.Windows.Forms.CheckBox _enablePortable;
		private System.Windows.Forms.CheckBox _enableCheckIn;
		private System.Windows.Forms.CheckBox _portable;
		private System.Windows.Forms.CheckBox _checkedIn;
		private System.Windows.Forms.CheckBox _enablePerformingFacility;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _scheduledTime;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _performingFacility;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _performingDepartment;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _laterality;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _schedulingCode;
		private System.Windows.Forms.CheckBox _enableSchedulingCode;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _scheduledDate;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.NumericUpDown _duration;
		private System.Windows.Forms.Label label1;
		private Desktop.View.WinForms.ComboBoxField _modality;
		private System.Windows.Forms.CheckBox _enableScheduledDuration;
		private System.Windows.Forms.CheckBox _enableModality;
    }
}
