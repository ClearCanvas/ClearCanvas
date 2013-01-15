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
    partial class AddressesSummaryControl
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
            this._addressList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _addressList
            // 
            this._addressList.AutoSize = true;
            this._addressList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._addressList.Location = new System.Drawing.Point(0, 0);
            this._addressList.Margin = new System.Windows.Forms.Padding(4, 4, 19, 4);
            this._addressList.MenuModel = null;
            this._addressList.MultiLine = true;
            this._addressList.MultiSelect = false;
            this._addressList.Name = "_addressList";
            this._addressList.ReadOnly = false;
            this._addressList.Selection = selection1;
            this._addressList.Size = new System.Drawing.Size(129, 118);
            this._addressList.TabIndex = 2;
            this._addressList.Table = null;
            this._addressList.ToolbarModel = null;
            this._addressList.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._addressList.ItemDoubleClicked += new System.EventHandler(this._addressList_ItemDoubleClicked);
            // 
            // AddressesSummaryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this._addressList);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "AddressesSummaryControl";
            this.Size = new System.Drawing.Size(129, 118);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _addressList;

    }
}
