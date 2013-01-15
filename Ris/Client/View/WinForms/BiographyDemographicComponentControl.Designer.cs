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
    partial class BiographyDemographicComponentControl
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
			this._selectedProfile = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._demoHostPanel = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _selectedProfile
			// 
			this._selectedProfile.DataSource = null;
			this._selectedProfile.DisplayMember = "";
			this._selectedProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._selectedProfile.LabelText = "Selected Profile";
			this._selectedProfile.Location = new System.Drawing.Point(0, 0);
			this._selectedProfile.Margin = new System.Windows.Forms.Padding(0);
			this._selectedProfile.Name = "_selectedProfile";
			this._selectedProfile.Size = new System.Drawing.Size(262, 41);
			this._selectedProfile.TabIndex = 3;
			this._selectedProfile.Value = null;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._demoHostPanel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._selectedProfile, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(1, 2);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(703, 558);
			this.tableLayoutPanel1.TabIndex = 4;
			// 
			// _demoHostPanel
			// 
			this._demoHostPanel.AutoSize = true;
			this._demoHostPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._demoHostPanel.BackColor = System.Drawing.SystemColors.ControlDark;
			this._demoHostPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._demoHostPanel.Location = new System.Drawing.Point(3, 41);
			this._demoHostPanel.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this._demoHostPanel.Name = "_demoHostPanel";
			this._demoHostPanel.Padding = new System.Windows.Forms.Padding(1);
			this._demoHostPanel.Size = new System.Drawing.Size(700, 517);
			this._demoHostPanel.TabIndex = 4;
			// 
			// BiographyDemographicComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "BiographyDemographicComponentControl";
			this.Padding = new System.Windows.Forms.Padding(1, 2, 2, 7);
			this.Size = new System.Drawing.Size(706, 567);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _selectedProfile;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel _demoHostPanel;


    }
}
