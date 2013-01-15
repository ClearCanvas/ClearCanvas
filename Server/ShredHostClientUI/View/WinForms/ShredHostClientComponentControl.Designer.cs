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

namespace ClearCanvas.Server.ShredHostClientUI.View.WinForms
{
    partial class ShredHostClientComponentControl
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
            ClearCanvas.Desktop.Selection selection3 = new ClearCanvas.Desktop.Selection();
            this._shredCollectionTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._toggleButton = new System.Windows.Forms.Button();
            this._shredHostGroupBox = new System.Windows.Forms.GroupBox();
            this._shredHostGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _shredCollectionTable
            // 
            this._shredCollectionTable.Location = new System.Drawing.Point(4, 117);
            this._shredCollectionTable.Margin = new System.Windows.Forms.Padding(4);
            this._shredCollectionTable.MenuModel = null;
            this._shredCollectionTable.Name = "_shredCollectionTable";
            this._shredCollectionTable.ReadOnly = false;
            this._shredCollectionTable.Selection = selection3;
            this._shredCollectionTable.Size = new System.Drawing.Size(314, 250);
            this._shredCollectionTable.TabIndex = 2;
            this._shredCollectionTable.Table = null;
            this._shredCollectionTable.ToolbarModel = null;
            this._shredCollectionTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._shredCollectionTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _toggleButton
            // 
            this._toggleButton.Location = new System.Drawing.Point(6, 35);
            this._toggleButton.Name = "_toggleButton";
            this._toggleButton.Size = new System.Drawing.Size(299, 57);
            this._toggleButton.TabIndex = 1;
            this._toggleButton.Text = "Start/Stop";
            this._toggleButton.UseVisualStyleBackColor = true;
            // 
            // _shredHostGroupBox
            // 
            this._shredHostGroupBox.Controls.Add(this._toggleButton);
            this._shredHostGroupBox.Location = new System.Drawing.Point(6, 6);
            this._shredHostGroupBox.Name = "_shredHostGroupBox";
            this._shredHostGroupBox.Size = new System.Drawing.Size(311, 111);
            this._shredHostGroupBox.TabIndex = 3;
            this._shredHostGroupBox.TabStop = false;
            this._shredHostGroupBox.Text = "ShredHost";
            // 
            // ShredHostClientComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._shredHostGroupBox);
            this.Controls.Add(this._shredCollectionTable);
            this.Name = "ShredHostClientComponentControl";
            this.Size = new System.Drawing.Size(322, 371);
            this._shredHostGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _shredCollectionTable;
        private System.Windows.Forms.Button _toggleButton;
        private System.Windows.Forms.GroupBox _shredHostGroupBox;
    }
}
