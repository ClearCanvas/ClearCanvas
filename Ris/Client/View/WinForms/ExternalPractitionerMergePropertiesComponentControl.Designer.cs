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
    partial class ExternalPractitionerMergePropertiesComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExternalPractitionerMergePropertiesComponentControl));
			this._name = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._billingNumber = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._licenseNumber = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._instruction = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._extendedProperties = new ClearCanvas.Ris.Client.View.WinForms.ExtendedPropertyChoicesTable();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _name
			// 
			this._name.DataSource = null;
			this._name.DisplayMember = "";
			resources.ApplyResources(this._name, "_name");
			this._name.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._name.Name = "_name";
			this._name.Value = null;
			// 
			// _billingNumber
			// 
			this._billingNumber.DataSource = null;
			this._billingNumber.DisplayMember = "";
			resources.ApplyResources(this._billingNumber, "_billingNumber");
			this._billingNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._billingNumber.Name = "_billingNumber";
			this._billingNumber.Value = null;
			// 
			// _licenseNumber
			// 
			this._licenseNumber.DataSource = null;
			this._licenseNumber.DisplayMember = "";
			resources.ApplyResources(this._licenseNumber, "_licenseNumber");
			this._licenseNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._licenseNumber.Name = "_licenseNumber";
			this._licenseNumber.Value = null;
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._licenseNumber, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this._instruction, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._billingNumber, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this._name, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// _instruction
			// 
			resources.ApplyResources(this._instruction, "_instruction");
			this.tableLayoutPanel1.SetColumnSpan(this._instruction, 3);
			this._instruction.Name = "_instruction";
			// 
			// groupBox1
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 3);
			this.groupBox1.Controls.Add(this._extendedProperties);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// _extendedProperties
			// 
			resources.ApplyResources(this._extendedProperties, "_extendedProperties");
			this._extendedProperties.Name = "_extendedProperties";
			// 
			// ExternalPractitionerMergePropertiesComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ExternalPractitionerMergePropertiesComponentControl";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _name;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _billingNumber;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _licenseNumber;
		private ExtendedPropertyChoicesTable _extendedProperties;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label _instruction;
		private System.Windows.Forms.GroupBox groupBox1;
    }
}
