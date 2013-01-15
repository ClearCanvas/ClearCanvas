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

namespace ClearCanvas.Desktop.Explorer.View.WinForms
{
    partial class ExplorerConfigurationComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExplorerConfigurationComponentControl));
			this._launchAsGroupBox = new System.Windows.Forms.GroupBox();
			this._launchAsShelf = new System.Windows.Forms.RadioButton();
			this._launchAsWorkspace = new System.Windows.Forms.RadioButton();
			this._launchAtStartup = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this._launchAsGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// _launchAsGroupBox
			// 
			this._launchAsGroupBox.Controls.Add(this._launchAsShelf);
			this._launchAsGroupBox.Controls.Add(this._launchAsWorkspace);
			resources.ApplyResources(this._launchAsGroupBox, "_launchAsGroupBox");
			this._launchAsGroupBox.Name = "_launchAsGroupBox";
			this._launchAsGroupBox.TabStop = false;
			// 
			// _launchAsShelf
			// 
			resources.ApplyResources(this._launchAsShelf, "_launchAsShelf");
			this._launchAsShelf.Name = "_launchAsShelf";
			this._launchAsShelf.UseVisualStyleBackColor = true;
			// 
			// _launchAsWorkspace
			// 
			resources.ApplyResources(this._launchAsWorkspace, "_launchAsWorkspace");
			this._launchAsWorkspace.Name = "_launchAsWorkspace";
			this._launchAsWorkspace.UseVisualStyleBackColor = true;
			// 
			// _launchAtStartup
			// 
			resources.ApplyResources(this._launchAtStartup, "_launchAtStartup");
			this._launchAtStartup.Name = "_launchAtStartup";
			this._launchAtStartup.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// ExplorerConfigurationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label1);
			this.Controls.Add(this._launchAtStartup);
			this.Controls.Add(this._launchAsGroupBox);
			this.Name = "ExplorerConfigurationComponentControl";
			this._launchAsGroupBox.ResumeLayout(false);
			this._launchAsGroupBox.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.GroupBox _launchAsGroupBox;
		private System.Windows.Forms.RadioButton _launchAsShelf;
		private System.Windows.Forms.RadioButton _launchAsWorkspace;
		private System.Windows.Forms.CheckBox _launchAtStartup;
		private System.Windows.Forms.Label label1;
    }
}
