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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultipleProceduresEditorComponentControl));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._enableScheduledDuration = new System.Windows.Forms.CheckBox();
			this._scheduledTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._enablePerformingDepartment = new System.Windows.Forms.CheckBox();
			this._enableLaterality = new System.Windows.Forms.CheckBox();
			this._enablePortable = new System.Windows.Forms.CheckBox();
			this._enableCheckIn = new System.Windows.Forms.CheckBox();
			this._performingDepartment = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._laterality = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._portable = new System.Windows.Forms.CheckBox();
			this._enableScheduledDateTime = new System.Windows.Forms.CheckBox();
			this._schedulingCode = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._checkedIn = new System.Windows.Forms.CheckBox();
			this._enableSchedulingCode = new System.Windows.Forms.CheckBox();
			this._scheduledDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this.panel1 = new System.Windows.Forms.Panel();
			this._duration = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this._modality = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._performingFacility = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._enablePerformingFacility = new System.Windows.Forms.CheckBox();
			this._enableModality = new System.Windows.Forms.CheckBox();
			this._acceptButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._duration)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._enableScheduledDuration, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._scheduledTime, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this._enableLaterality, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this._enablePortable, 0, 7);
			this.tableLayoutPanel1.Controls.Add(this._enableCheckIn, 0, 8);
			this.tableLayoutPanel1.Controls.Add(this._laterality, 1, 5);
			this.tableLayoutPanel1.Controls.Add(this._portable, 1, 7);
			this.tableLayoutPanel1.Controls.Add(this._enableScheduledDateTime, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._schedulingCode, 1, 6);
			this.tableLayoutPanel1.Controls.Add(this._checkedIn, 1, 8);
			this.tableLayoutPanel1.Controls.Add(this._enableSchedulingCode, 0, 6);
			this.tableLayoutPanel1.Controls.Add(this._scheduledDate, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this._performingFacility, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this._enablePerformingFacility, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._performingDepartment, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this._modality, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this._enableModality, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this._enablePerformingDepartment, 0, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// _enableScheduledDuration
			// 
			resources.ApplyResources(this._enableScheduledDuration, "_enableScheduledDuration");
			this._enableScheduledDuration.Name = "_enableScheduledDuration";
			this._enableScheduledDuration.UseVisualStyleBackColor = true;
			// 
			// _scheduledTime
			// 
			resources.ApplyResources(this._scheduledTime, "_scheduledTime");
			this._scheduledTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._scheduledTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._scheduledTime.Name = "_scheduledTime";
			this._scheduledTime.Nullable = true;
			this._scheduledTime.ShowDate = false;
			this._scheduledTime.ShowTime = true;
			this._scheduledTime.Value = null;
			// 
			// _enablePerformingDepartment
			// 
			resources.ApplyResources(this._enablePerformingDepartment, "_enablePerformingDepartment");
			this._enablePerformingDepartment.Name = "_enablePerformingDepartment";
			this._enablePerformingDepartment.UseVisualStyleBackColor = true;
			// 
			// _enableLaterality
			// 
			resources.ApplyResources(this._enableLaterality, "_enableLaterality");
			this._enableLaterality.Name = "_enableLaterality";
			this._enableLaterality.UseVisualStyleBackColor = true;
			// 
			// _enablePortable
			// 
			resources.ApplyResources(this._enablePortable, "_enablePortable");
			this._enablePortable.Name = "_enablePortable";
			this._enablePortable.UseVisualStyleBackColor = true;
			// 
			// _enableCheckIn
			// 
			resources.ApplyResources(this._enableCheckIn, "_enableCheckIn");
			this._enableCheckIn.Name = "_enableCheckIn";
			this._enableCheckIn.UseVisualStyleBackColor = true;
			// 
			// _performingDepartment
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._performingDepartment, 2);
			this._performingDepartment.DataSource = null;
			this._performingDepartment.DisplayMember = "";
			resources.ApplyResources(this._performingDepartment, "_performingDepartment");
			this._performingDepartment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._performingDepartment.Name = "_performingDepartment";
			this._performingDepartment.Value = null;
			// 
			// _laterality
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._laterality, 2);
			this._laterality.DataSource = null;
			this._laterality.DisplayMember = "";
			resources.ApplyResources(this._laterality, "_laterality");
			this._laterality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._laterality.Name = "_laterality";
			this._laterality.Value = null;
			// 
			// _portable
			// 
			resources.ApplyResources(this._portable, "_portable");
			this.tableLayoutPanel1.SetColumnSpan(this._portable, 2);
			this._portable.Name = "_portable";
			this._portable.UseVisualStyleBackColor = true;
			// 
			// _enableScheduledDateTime
			// 
			resources.ApplyResources(this._enableScheduledDateTime, "_enableScheduledDateTime");
			this._enableScheduledDateTime.Name = "_enableScheduledDateTime";
			this._enableScheduledDateTime.UseVisualStyleBackColor = true;
			// 
			// _schedulingCode
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._schedulingCode, 2);
			this._schedulingCode.DataSource = null;
			this._schedulingCode.DisplayMember = "";
			resources.ApplyResources(this._schedulingCode, "_schedulingCode");
			this._schedulingCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._schedulingCode.Name = "_schedulingCode";
			this._schedulingCode.Value = null;
			// 
			// _checkedIn
			// 
			resources.ApplyResources(this._checkedIn, "_checkedIn");
			this.tableLayoutPanel1.SetColumnSpan(this._checkedIn, 2);
			this._checkedIn.Name = "_checkedIn";
			this._checkedIn.UseVisualStyleBackColor = true;
			// 
			// _enableSchedulingCode
			// 
			resources.ApplyResources(this._enableSchedulingCode, "_enableSchedulingCode");
			this._enableSchedulingCode.Name = "_enableSchedulingCode";
			this._enableSchedulingCode.UseVisualStyleBackColor = true;
			// 
			// _scheduledDate
			// 
			resources.ApplyResources(this._scheduledDate, "_scheduledDate");
			this._scheduledDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._scheduledDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._scheduledDate.Name = "_scheduledDate";
			this._scheduledDate.Nullable = true;
			this._scheduledDate.Value = null;
			// 
			// panel1
			// 
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Controls.Add(this._duration);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Name = "panel1";
			// 
			// _duration
			// 
			this._duration.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			resources.ApplyResources(this._duration, "_duration");
			this._duration.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._duration.Name = "_duration";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// _modality
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._modality, 2);
			this._modality.DataSource = null;
			this._modality.DisplayMember = "";
			resources.ApplyResources(this._modality, "_modality");
			this._modality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._modality.Name = "_modality";
			this._modality.Value = null;
			// 
			// _performingFacility
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._performingFacility, 2);
			this._performingFacility.DataSource = null;
			this._performingFacility.DisplayMember = "";
			resources.ApplyResources(this._performingFacility, "_performingFacility");
			this._performingFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._performingFacility.Name = "_performingFacility";
			this._performingFacility.Value = null;
			// 
			// _enablePerformingFacility
			// 
			resources.ApplyResources(this._enablePerformingFacility, "_enablePerformingFacility");
			this._enablePerformingFacility.Name = "_enablePerformingFacility";
			this._enablePerformingFacility.UseVisualStyleBackColor = true;
			// 
			// _enableModality
			// 
			resources.ApplyResources(this._enableModality, "_enableModality");
			this._enableModality.Name = "_enableModality";
			this._enableModality.UseVisualStyleBackColor = true;
			// 
			// _acceptButton
			// 
			resources.ApplyResources(this._acceptButton, "_acceptButton");
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// MultipleProceduresEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "MultipleProceduresEditorComponentControl";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._duration)).EndInit();
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
		private Desktop.View.WinForms.ComboBoxField _performingFacility;
    }
}
