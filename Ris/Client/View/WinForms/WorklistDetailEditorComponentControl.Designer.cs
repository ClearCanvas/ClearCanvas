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
    partial class WorklistDetailEditorComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorklistDetailEditorComponentControl));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._ownerGroupBox = new System.Windows.Forms.GroupBox();
			this._groups = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._radioGroup = new System.Windows.Forms.RadioButton();
			this._radioPersonal = new System.Windows.Forms.RadioButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this._category = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._worklistClass = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._description = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
			this._classDescription = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this._ownerGroupBox.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._ownerGroupBox, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// _ownerGroupBox
			// 
			resources.ApplyResources(this._ownerGroupBox, "_ownerGroupBox");
			this._ownerGroupBox.Controls.Add(this._groups);
			this._ownerGroupBox.Controls.Add(this._radioGroup);
			this._ownerGroupBox.Controls.Add(this._radioPersonal);
			this._ownerGroupBox.Name = "_ownerGroupBox";
			this._ownerGroupBox.TabStop = false;
			// 
			// _groups
			// 
			resources.ApplyResources(this._groups, "_groups");
			this._groups.DataSource = null;
			this._groups.DisplayMember = "";
			this._groups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._groups.Name = "_groups";
			this._groups.Value = null;
			// 
			// _radioGroup
			// 
			resources.ApplyResources(this._radioGroup, "_radioGroup");
			this._radioGroup.Name = "_radioGroup";
			this._radioGroup.TabStop = true;
			this._radioGroup.UseVisualStyleBackColor = true;
			// 
			// _radioPersonal
			// 
			resources.ApplyResources(this._radioPersonal, "_radioPersonal");
			this._radioPersonal.Name = "_radioPersonal";
			this._radioPersonal.TabStop = true;
			this._radioPersonal.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._category);
			this.panel1.Controls.Add(this._worklistClass);
			this.panel1.Controls.Add(this._name);
			this.panel1.Controls.Add(this._description);
			this.panel1.Controls.Add(this._classDescription);
			this.panel1.Controls.Add(this._okButton);
			this.panel1.Controls.Add(this._cancelButton);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// _category
			// 
			resources.ApplyResources(this._category, "_category");
			this._category.DataSource = null;
			this._category.DisplayMember = "";
			this._category.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._category.Name = "_category";
			this._category.Value = null;
			// 
			// _worklistClass
			// 
			resources.ApplyResources(this._worklistClass, "_worklistClass");
			this._worklistClass.DataSource = null;
			this._worklistClass.DisplayMember = "";
			this._worklistClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._worklistClass.Name = "_worklistClass";
			this._worklistClass.Value = null;
			// 
			// _name
			// 
			resources.ApplyResources(this._name, "_name");
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.Value = null;
			// 
			// _description
			// 
			resources.ApplyResources(this._description, "_description");
			this._description.Name = "_description";
			this._description.Value = null;
			// 
			// _classDescription
			// 
			resources.ApplyResources(this._classDescription, "_classDescription");
			this._classDescription.Mask = "";
			this._classDescription.Name = "_classDescription";
			this._classDescription.ReadOnly = true;
			this._classDescription.Value = null;
			// 
			// _okButton
			// 
			resources.ApplyResources(this._okButton, "_okButton");
			this._okButton.Name = "_okButton";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// WorklistDetailEditorComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "WorklistDetailEditorComponentControl";
			this.tableLayoutPanel1.ResumeLayout(false);
			this._ownerGroupBox.ResumeLayout(false);
			this._ownerGroupBox.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private ClearCanvas.Desktop.View.WinForms.TextField _name;
		private ClearCanvas.Desktop.View.WinForms.TextAreaField _description;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _cancelButton;
		private ClearCanvas.Desktop.View.WinForms.TextField _classDescription;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _category;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _worklistClass;
		private System.Windows.Forms.GroupBox _ownerGroupBox;
		private System.Windows.Forms.RadioButton _radioGroup;
		private System.Windows.Forms.RadioButton _radioPersonal;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _groups;


	}
}
