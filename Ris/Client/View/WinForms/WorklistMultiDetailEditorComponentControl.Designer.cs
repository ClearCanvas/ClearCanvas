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
    partial class WorklistMultiDetailEditorComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorklistMultiDetailEditorComponentControl));
			this._worklistCategory = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._defaultName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._worklistClassTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _worklistCategory
			// 
			this._worklistCategory.DataSource = null;
			this._worklistCategory.DisplayMember = "";
			this._worklistCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._worklistCategory, "_worklistCategory");
			this._worklistCategory.Name = "_worklistCategory";
			this._worklistCategory.Value = null;
			// 
			// _defaultName
			// 
			resources.ApplyResources(this._defaultName, "_defaultName");
			this._defaultName.Mask = "";
			this._defaultName.Name = "_defaultName";
			this._defaultName.Value = null;
			// 
			// _worklistClassTableView
			// 
			resources.ApplyResources(this._worklistClassTableView, "_worklistClassTableView");
			this._worklistClassTableView.Name = "_worklistClassTableView";
			this._worklistClassTableView.ReadOnly = false;
			this._worklistClassTableView.ItemDoubleClicked += new System.EventHandler(this._worklistClassTableView_ItemDoubleClicked);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// WorklistMultiDetailEditorComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label1);
			this.Controls.Add(this._worklistClassTableView);
			this.Controls.Add(this._defaultName);
			this.Controls.Add(this._worklistCategory);
			this.Name = "WorklistMultiDetailEditorComponentControl";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _worklistCategory;
        private ClearCanvas.Desktop.View.WinForms.TextField _defaultName;
        private ClearCanvas.Desktop.View.WinForms.TableView _worklistClassTableView;
        private System.Windows.Forms.Label label1;
    }
}
