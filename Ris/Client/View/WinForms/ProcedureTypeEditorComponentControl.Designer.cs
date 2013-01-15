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
    partial class ProcedureTypeEditorComponentControl
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
			this._acceptButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._id = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._defaultDuration = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this._defaultModality = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			((System.ComponentModel.ISupportInitialize)(this._defaultDuration)).BeginInit();
			this.SuspendLayout();
			// 
			// _acceptButton
			// 
			this._acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._acceptButton.Location = new System.Drawing.Point(268, 156);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 5;
			this._acceptButton.Text = "OK";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(349, 156);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 6;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _id
			// 
			this._id.LabelText = "ID";
			this._id.Location = new System.Drawing.Point(11, 7);
			this._id.Margin = new System.Windows.Forms.Padding(2);
			this._id.Mask = "";
			this._id.Name = "_id";
			this._id.PasswordChar = '\0';
			this._id.Size = new System.Drawing.Size(413, 41);
			this._id.TabIndex = 0;
			this._id.ToolTip = null;
			this._id.Value = null;
			// 
			// _name
			// 
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(11, 53);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.PasswordChar = '\0';
			this._name.Size = new System.Drawing.Size(413, 41);
			this._name.TabIndex = 1;
			this._name.ToolTip = null;
			this._name.Value = null;
			// 
			// _defaultDuration
			// 
			this._defaultDuration.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this._defaultDuration.Location = new System.Drawing.Point(313, 117);
			this._defaultDuration.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._defaultDuration.Name = "_defaultDuration";
			this._defaultDuration.Size = new System.Drawing.Size(105, 20);
			this._defaultDuration.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(310, 99);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(109, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Default Duration (min)";
			// 
			// _defaultModality
			// 
			this._defaultModality.DataSource = null;
			this._defaultModality.DisplayMember = "";
			this._defaultModality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._defaultModality.LabelText = "Default Modality";
			this._defaultModality.Location = new System.Drawing.Point(11, 98);
			this._defaultModality.Margin = new System.Windows.Forms.Padding(2);
			this._defaultModality.Name = "_defaultModality";
			this._defaultModality.Size = new System.Drawing.Size(282, 41);
			this._defaultModality.TabIndex = 2;
			this._defaultModality.Value = null;
			// 
			// ProcedureTypeEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._defaultModality);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._defaultDuration);
			this.Controls.Add(this._name);
			this.Controls.Add(this._id);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Name = "ProcedureTypeEditorComponentControl";
			this.Size = new System.Drawing.Size(458, 191);
			((System.ComponentModel.ISupportInitialize)(this._defaultDuration)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button _acceptButton;
		private System.Windows.Forms.Button _cancelButton;
		private ClearCanvas.Desktop.View.WinForms.TextField _id;
		private ClearCanvas.Desktop.View.WinForms.TextField _name;
		private System.Windows.Forms.NumericUpDown _defaultDuration;
		private System.Windows.Forms.Label label2;
		private Desktop.View.WinForms.ComboBoxField _defaultModality;
    }
}
